using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;
using SkiaSharp;

namespace avalonia_animation
{
    public class ElementRenderer : Control
    {
        public ElementRenderer()
        {
            ClipToBounds = true;
        }

        private List<Zombie> _zombies;
        private Timer _testTimer;

        public void Start()
        {
            // Stop all previous rendering
            Stop();

            // Spawn some zombies for movement
            const int count = 100;
            _zombies = Enumerable.Range(0, count).Select(i =>
                new Zombie(i / (double) count * Bounds.Width, i / (double) count * Bounds.Height, 50, 50,
                    0, 0, Bounds.Width, Bounds.Height)).ToList();

            // Every few moments: update zombie positions and visuals
            var startTime = DateTime.Now;
            _testTimer = new Timer(ignored =>
            {
                // Logically update positions of the zombies
                foreach (var zombie in _zombies)
                    zombie.Move((DateTime.Now - startTime).TotalSeconds);
            }, null, 50, 15);
        }

        public void Stop() => _testTimer?.Change(-1, -1);

        /// <summary>
        /// Custom zombie draw operation.
        /// </summary>
        private class ZombieDrawOp : ICustomDrawOperation
        {
            private static readonly SKPaint DotFill = new SKPaint {Style = SKPaintStyle.Fill, Color = SKColors.Black, IsAntialias = true};
            private static readonly SKPaint ZombieFill = new SKPaint {Style = SKPaintStyle.Fill, Color = SKColors.LimeGreen};
            private static readonly SKPaint ZombieOutline = new SKPaint {Style = SKPaintStyle.Stroke, Color = SKColors.Black, StrokeWidth = 5, IsAntialias = true};
            private static readonly SKPaint ZombieDebug = new SKPaint {Style = SKPaintStyle.Fill, Color = SKColors.OrangeRed, IsAntialias = true};
            private static readonly FormattedText NoSkia = new FormattedText {Text = "Current rendering API is not Skia"};
            private readonly List<Zombie> _zombies;

            public ZombieDrawOp(Rect bounds, List<Zombie> zombies)
            {
                _zombies = zombies;
                Bounds = bounds;
            }

            public void Dispose()
            {
                // No-op
            }

            public Rect Bounds { get; }
            public bool HitTest(Point p) => false;
            public bool Equals(ICustomDrawOperation other) => false;

            /// <summary>
            /// Rotates the given path around cx, cy and then pushes it to nx, ny.
            /// This method assumes that cx, cy is the center to be used for rotation and positioning.
            /// </summary>
            /// <param name="path">The path to transform.</param>
            /// <param name="degrees">The degrees to rotate by.</param>
            /// <param name="cx">The center x-coordinate.</param>
            /// <param name="cy">The center y-coordinate.</param>
            /// <param name="nx">The new center x-coordinate to move the path to.</param>
            /// <param name="ny">The new center y-coordinate to move the path to.</param>
            private static void RotateAndTranslate(SKPath path, float degrees, float cx, float cy, float nx, float ny)
            {
                var result = SKMatrix.CreateIdentity();
                var translate = SKMatrix.CreateTranslation(-cx, -cy);
                var rotate = SKMatrix.CreateRotationDegrees(degrees);
                var translate2 = SKMatrix.CreateTranslation(nx, ny);
                result = result.PostConcat(translate);
                result = result.PostConcat(rotate);
                result = result.PostConcat(translate2);
                path.Transform(result);
            }

            private void RenderContent(SKCanvas canvas)
            {
                RenderDots(canvas);
                RenderZombies(canvas);
            }

            private void RenderDots(SKCanvas canvas)
            {
                for (var x = 100; x < Bounds.Width; x += 100)
                for (var y = 100; y < Bounds.Height; y += 100)
                    canvas.DrawCircle(x, y, 10, DotFill);
            }

            private void RenderZombies(SKCanvas canvas)
            {
                if (_zombies == null) return;
                foreach (var t in _zombies)
                {
                    // Determine coordinates
                    var (l, w) = ((float) t.LengthX, (float) t.LengthY);
                    var (cx, cy) = (l / 2.0f, w / 2.0f);
                    var (px, py) = ((float) t.X, (float) t.Y);
                    // Draw rectangle with orientation marker
                    var path = new SKPath();
                    path.MoveTo(cx, cy);
                    path.LineTo(l, cy);
                    path.AddRect(new SKRect(0, 0, l, w));
                    RotateAndTranslate(path, (float) Util.RadToDeg(t.Orientation), cx, cy, px, py);
                    canvas.DrawPath(path, ZombieFill);
                    canvas.DrawPath(path, ZombieOutline);
                }
            }

            public void Render(IDrawingContextImpl context)
            {
                var canvas = (context as ISkiaDrawingContextImpl)?.SkCanvas;
                if (canvas == null)
                    context.DrawText(Brushes.Black, new Point(), NoSkia.PlatformImpl);
                else
                {
                    canvas.Save();
                    RenderContent(canvas);
                    canvas.Restore();
                }
            }
        }


        public override void Render(DrawingContext context)
        {
            context.Custom(new ZombieDrawOp(new Rect(0, 0, Bounds.Width, Bounds.Height), _zombies));
            Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
        }
    }
}