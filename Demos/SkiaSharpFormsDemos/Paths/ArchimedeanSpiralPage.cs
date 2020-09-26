using System;

using Xamarin.Forms;

using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace SkiaSharpFormsDemos.Paths
{
    // No .xaml is needed since the entire page
    // is generated programatically.
    public class ArchimedeanSpiralPage : ContentPage
    {
        public ArchimedeanSpiralPage()
        {
            Title = "Archimedean Spiral";

            // Creates the canvas and subscribes to its draw call.
            SKCanvasView canvasView = new SKCanvasView();
            canvasView.PaintSurface += OnCanvasViewPaintSurface;
            Content = canvasView;
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            // Get information about the canvas state.
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            // Clear prior calls rendering.
            canvas.Clear();

            // Define a coordinate at the screen center.
            SKPoint center = new SKPoint(info.Width / 2, info.Height / 2);
            // Limit the radius based on the smaller screen dimension.
            float radius = Math.Min(center.X, center.Y);

            // Ensure that the path object will be properly disposed
            // of after the completion of this block.
            using (SKPath path = new SKPath())
            {
                // The maths to draw the spiral.
                for (float angle = 0; angle < 3600; angle += 1)
                {
                    // Calculate the current radius (expands at a linear rate).
                    float scaledRadius = radius * angle / 3600;
                    // Calculate the current angle.
                    double radians = Math.PI * angle / 180;
                    // Convert from angle to coords and add the center offset.
                    float x = center.X + scaledRadius * (float)Math.Cos(radians);
                    float y = center.Y + scaledRadius * (float)Math.Sin(radians);
                    SKPoint point = new SKPoint(x, y);

                    // The first cycle of this loop must use move to
                    // otherwise it will draw a line from the default
                    // cursor position to the center of the screen.
                    if (angle == 0)
                    {
                        path.MoveTo(point);
                    }
                    else
                    {
                        path.LineTo(point);
                    }
                }

                // Define the style of the path to draw.
                SKPaint paint = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = SKColors.Red,
                    StrokeWidth = 5
                };

                // Invoke the canvas to draw the defined.
                canvas.DrawPath(path, paint);
            }
            // At the closure of this block the generated path is disposed.
        }
    }
}
