using System;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace SkiaSharpFormsDemos.JethrosExample
{
    public sealed class InfuriatingLoadingRing : ContentPage
    {
        private const uint SECONDS2MILLIS = 1000;

        private float revolveDegrees, rotateDegrees;
        private float radius;
        private float hue;
        private SKCanvasView canvasView;

        public InfuriatingLoadingRing()
        {
            Title = "Wouldn't it be funny if I never loaded and there's no way you could know?";

            canvasView = new SKCanvasView();
            canvasView.PaintSurface += OnCanvasViewPaintSurface;
            Content = canvasView;
        }

        // Start animation cycles.
        protected override void OnAppearing()
        {
            base.OnAppearing();

            Func<bool> loop = () => true;

            // Animate the overall revolve.
            new Animation((interpolant) =>
            {
                revolveDegrees = 360 * (float)interpolant;
                canvasView.InvalidateSurface();
            }
            ).Commit(this, "revolveAnimation", length: 3 * SECONDS2MILLIS, repeat: loop);
            // Animate the local revolve.
            new Animation((interpolant) =>
            {
                rotateDegrees = 360 * (float)interpolant;
            }
            ).Commit(this, "rotateAnimation", length: 1 * SECONDS2MILLIS, repeat: loop);
            new Animation((interpolant) =>
            {
                radius = 0.8f + (float)Math.Sin(interpolant * 2 * Math.PI) * 0.1f;
            }
            ).Commit(this, "pulseAnimation", length: 2 * SECONDS2MILLIS, repeat: loop);
            new Animation((interpolant) =>
            {
                hue = 360 * (float)interpolant;
            }
            ).Commit(this, "hueShiftAnimation", length: 4 * SECONDS2MILLIS, repeat: loop);
        }
        // Close animation cycles.
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            this.AbortAnimation("revolveAnimation");
            this.AbortAnimation("rotateAnimation");
            this.AbortAnimation("pulseAnimation");
            this.AbortAnimation("hueShiftAnimation");
        }

        // Implement drawing cycle.
        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            //canvas.Clear();

            using (SKPaint fillPaint = new SKPaint {
                Style = SKPaintStyle.Stroke,
                Color = SKColor.FromHsv(hue, 100, 100),
                StrokeWidth = 10
            })
            {
                // Translate to center of canvas
                canvas.Translate(info.Width / 2, info.Height / 2);

                for (int angle = 0; angle < 360; angle += 45)
                {
                    canvas.Save();

                    // Rotate around center of canvas
                    canvas.RotateDegrees(revolveDegrees + angle);
                    // Translate horizontally
                    float madLad = Math.Min(info.Width, info.Height) / 3;
                    canvas.Translate(madLad * radius, 0);
                    // Rotate around center of object
                    canvas.RotateDegrees(rotateDegrees);
                    // Draw a square
                    float apothem = 30 + angle * 0.1f;
                    canvas.DrawRect(new SKRect(-apothem, -apothem, apothem, apothem), fillPaint);

                    canvas.Restore();
                }
            }
        }
    }
}
