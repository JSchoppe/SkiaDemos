using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Xamarin.Forms;

using SkiaSharp;
using SkiaSharp.Views.Forms;

using TouchTracking;

namespace SkiaSharpFormsDemos.Transforms
{
    // xaml is used to define the touch interaction.
    public partial class BitmapScatterViewPage : ContentPage
    {
        List<TouchManipulationBitmap> bitmapCollection =
            new List<TouchManipulationBitmap>();

        Dictionary<long, TouchManipulationBitmap> bitmapDictionary =
            new Dictionary<long, TouchManipulationBitmap>();

        public BitmapScatterViewPage()
        {
            InitializeComponent();

            // Load in all the available bitmaps
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            string[] resourceIDs = assembly.GetManifestResourceNames();
            SKPoint position = new SKPoint();

            foreach (string resourceID in resourceIDs)
            {
                // Can resources for image files.
                if (resourceID.EndsWith(".png") ||
                    resourceID.EndsWith(".jpg"))
                {
                    // Use a stream to read the files.
                    using (Stream stream = assembly.GetManifestResourceStream(resourceID))
                    {
                        // Load each file into a bitmap object.
                        SKBitmap bitmap = SKBitmap.Decode(stream);
                        bitmapCollection.Add(new TouchManipulationBitmap(bitmap)
                        {
                            // Create a new transformation matrix for this bitmap.
                            Matrix = SKMatrix.MakeTranslation(position.X, position.Y),
                        });
                        // Add 100 pixels offset for each image generated.
                        position.X += 100;
                        position.Y += 100;
                    }
                    // Properly dispose of the stream.
                }
            }
        }

        // Called when the user touches the screen.
        void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            // Convert Xamarin.Forms point to pixels
            Point pt = args.Location;
            SKPoint point =
                new SKPoint((float)(canvasView.CanvasSize.Width * pt.X / canvasView.Width),
                            (float)(canvasView.CanvasSize.Height * pt.Y / canvasView.Height));

            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    // Check for pressed hits from top to bottom.
                    for (int i = bitmapCollection.Count - 1; i >= 0; i--)
                    {
                        TouchManipulationBitmap bitmap = bitmapCollection[i];

                        if (bitmap.HitTest(point))
                        {
                            // Move bitmap to end of collection
                            bitmapCollection.Remove(bitmap);
                            bitmapCollection.Add(bitmap);

                            // Keep track of this drag state.
                            bitmapDictionary.Add(args.Id, bitmap);
                            bitmap.ProcessTouchEvent(args.Id, args.Type, point);
                            // Force the canvas to refresh.
                            canvasView.InvalidateSurface();
                            break;
                        }
                    }
                    break;

                case TouchActionType.Moved:
                    if (bitmapDictionary.ContainsKey(args.Id))
                    {
                        TouchManipulationBitmap bitmap = bitmapDictionary[args.Id];
                        bitmap.ProcessTouchEvent(args.Id, args.Type, point);
                        canvasView.InvalidateSurface();
                    }
                    break;

                case TouchActionType.Released:
                case TouchActionType.Cancelled:
                    if (bitmapDictionary.ContainsKey(args.Id))
                    {
                        TouchManipulationBitmap bitmap = bitmapDictionary[args.Id];
                        bitmap.ProcessTouchEvent(args.Id, args.Type, point);
                        bitmapDictionary.Remove(args.Id);
                        canvasView.InvalidateSurface();
                    }
                    break;
            }
        }

        // Runs every time the canvas makes a draw call.
        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKCanvas canvas = args.Surface.Canvas;
            // Clear what was previously on the canvas.
            canvas.Clear();

            // Redraw each of the bitmaps.
            foreach (TouchManipulationBitmap bitmap in bitmapCollection)
            {
                bitmap.Paint(canvas);
            }
        }
    }
}
