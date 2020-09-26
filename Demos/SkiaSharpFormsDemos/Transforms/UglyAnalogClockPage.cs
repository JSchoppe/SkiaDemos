using System;

using Xamarin.Forms;

using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace SkiaSharpFormsDemos.Transforms
{
    public class UglyAnalogClockPage : ContentPage
    {
        SKCanvasView canvasView;
        bool pageIsActive;

        public UglyAnalogClockPage()
        {
            Title = "Ugly Analog Clock";

            // Initialize the canvas stuff and bind to draw call.
            canvasView = new SKCanvasView();
            canvasView.PaintSurface += OnCanvasViewPaintSurface;
            Content = canvasView;
        }

        // Handles initialization of timer.
        protected override void OnAppearing()
        {
            base.OnAppearing();
            pageIsActive = true;

            // Create a timer that ticks every second.
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                // Force a new draw call.
                canvasView.InvalidateSurface();
                return pageIsActive;
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            pageIsActive = false;
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            // Retrieve canvas state.
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;
            // Clear anything drawn in previous calls.
            canvas.Clear();

            using (SKPaint strokePaint = new SKPaint())
            using (SKPaint fillPaint = new SKPaint())
            {
                // Define clock styling.
                strokePaint.Style = SKPaintStyle.Stroke;
                strokePaint.Color = SKColors.Black;
                strokePaint.StrokeCap = SKStrokeCap.Round;
                fillPaint.Style = SKPaintStyle.Fill;
                fillPaint.Color = SKColors.Gray;

                // Transform for 100-radius circle centered at origin
                canvas.Translate(info.Width / 2f, info.Height / 2f);
                canvas.Scale(Math.Min(info.Width / 200f, info.Height / 200f));

                // Hour and minute marks
                for (int angle = 0; angle < 360; angle += 6)
                {
                    // Draw the dot based on whether it is a one minute or five minute mark.
                    canvas.DrawCircle(0, -90, angle % 30 == 0 ? 4 : 2, fillPaint);
                    // Pivot the canvas.
                    canvas.RotateDegrees(6);
                }

                DateTime dateTime = DateTime.Now;

                // Hour hand
                strokePaint.StrokeWidth = 20;
                canvas.Save(); // <-- We about to f*** everything up, give us an undo.
                canvas.RotateDegrees(30 * dateTime.Hour + dateTime.Minute / 2f);
                canvas.DrawLine(0, 0, 0, -50, strokePaint);
                canvas.Restore(); // Now we use that undo.

                // Minute hand
                strokePaint.StrokeWidth = 10;
                canvas.Save();
                canvas.RotateDegrees(6 * dateTime.Minute + dateTime.Second / 10f);
                canvas.DrawLine(0, 0, 0, -70, strokePaint);
                canvas.Restore();

                // Second hand
                strokePaint.StrokeWidth = 2;
                canvas.Save();
                canvas.RotateDegrees(6 * dateTime.Second);
                canvas.DrawLine(0, 10, 0, -80, strokePaint);
                canvas.Restore();
            }
            // Properly dispose of stroke paint and fill paint.
        }
    }
}
