using System;

using Xamarin.Forms;

using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace SkiaSharpFormsDemos.Curves
{
    public class RegionPaintPage : ContentPage
    {
        public RegionPaintPage()
        {
            Title = "Region Paint";

            // Creates the canvas and subscribes to its draw call.
            SKCanvasView canvasView = new SKCanvasView();
            canvasView.PaintSurface += OnCanvasViewPaintSurface;
            Content = canvasView;
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            // Retrieve canvas state.
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            // Clear any drawing from previous draw call.
            canvas.Clear();

            int radius = 10;

            // Create circular path
            using (SKPath circlePath = new SKPath())
            {
                circlePath.AddCircle(0, 0, radius);

                // Create circular region
                using (SKRegion circleRegion = new SKRegion())
                {
                    // Defines a region that is 20 x 20 pixel units.
                    circleRegion.SetRect(new SKRectI(-radius, -radius, radius, radius));
                    circleRegion.SetPath(circlePath);

                    // Set transform to move it to center and scale up
                    canvas.Translate(info.Width / 2, info.Height / 2);
                    // This scales the region to be proportional to our arbitrary units.
                    canvas.Scale(Math.Min(info.Width / 2, info.Height / 2) / radius);

                    // Fill region
                    using (SKPaint fillPaint = new SKPaint())
                    {
                        fillPaint.Style = SKPaintStyle.Fill;
                        fillPaint.Color = SKColors.Orange;

                        // The circle appears pixelated because our
                        // defined region defines the pixel count.
                        canvas.DrawRegion(circleRegion, fillPaint);
                    }
                    // Fill paint is properly disposed of.

                    // Stroke path for comparison
                    using (SKPaint strokePaint = new SKPaint())
                    {
                        strokePaint.Style = SKPaintStyle.Stroke;
                        strokePaint.Color = SKColors.Blue;
                        strokePaint.StrokeWidth = 0.1f;

                        // The drawn path is not pixelated as it
                        // is not bounded by a region.
                        canvas.DrawPath(circlePath, strokePaint);
                    }
                    // Stoke paint is properly disposed of.
                }
                // Circle region is properly disposed of.
            }
            // Circle path is properly disposed of.
        }
    }
}
