using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using SharpDX.Direct3D11;

namespace avalonia_animation
{
    /// <summary>
    /// A simple 'zombie' roaming an area.
    /// </summary>
    public class Zombie
    {
        public Zombie(double x, double y, double lengthX, double lengthY, double fenceMinX, double fenceMinY, double fenceMaxX, double fenceMaxY)
        {
            // Set attributes
            X = x;
            Y = y;
            LengthX = lengthX;
            LengthY = lengthY;
            Fence = new ZombieFence {MinX = fenceMinX, MinY = fenceMinY, MaxX = fenceMaxX, MaxY = fenceMaxY};
            // Init visual
            var frame = new GeometryDrawing
            {
                Geometry = new RectangleGeometry(new Rect(new Size(lengthX, lengthY))),
                Brush = Brushes.LimeGreen,
                Pen = new Pen(Brushes.Black, 5)
            };
            var orientIndicator = new GeometryDrawing
            {
                Geometry = new LineGeometry(new Point(lengthX / 2.0, lengthY / 2.0), new Point(lengthX, lengthY / 2.0)),
                Brush = Brushes.Black,
                Pen = new Pen(Brushes.Black, 5)
            };
            var negativeLineExample = new GeometryDrawing
            {
                Geometry = new LineGeometry(new Point(lengthX / 2.0, lengthY / 2.0), new Point(-lengthX, lengthY / 2.0)),
                Brush = Brushes.OrangeRed,
                Pen = new Pen(Brushes.OrangeRed, 5)
            };
            var drawingGroup = new DrawingGroup();
            drawingGroup.Children.Add(frame);
            drawingGroup.Children.Add(orientIndicator);
            drawingGroup.Children.Add(negativeLineExample);
            var presenter = new DrawingPresenter
            {
                Drawing = drawingGroup,
                RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative)
            };
            Visual = presenter;
        }

        public static double RadToDeg(double rad) => rad * (180.0 / Math.PI);

        private static readonly Random Randomizer = new Random(0);

        private const double Speed = 20;

        public IControl Visual { get; }

        private ZombieFence Fence { get; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Orientation { get; set; }
        public double LengthX { get; set; }
        public double LengthY { get; set; }

        private double _lastMove = double.NaN;

        public void UpdateVisual()
        {
            var t = new TransformGroup();
            t.Children.Add(new RotateTransform(RadToDeg(Orientation)));
            // This needs to convert X/Y given as center coordinates to top-left coordinates
            // Alternatively, the anchor should already be centered similar to the rotation and RenderTransformOrigin (0.5,0.5)
            t.Children.Add(new TranslateTransform(X, Y));
            Visual.RenderTransform = t;
        }

        public void Move(double time)
        {
            var timeDelta = time - _lastMove;
            _lastMove = time;
            if (double.IsNaN(timeDelta))
                return;

            var newX = X + timeDelta * Speed * Math.Cos(Orientation);
            var newY = Y + timeDelta * Speed * Math.Sin(Orientation);

            var randomTurn = false;
            if (newX < Fence.MinX)
            {
                newX = Fence.MinX;
                randomTurn = true;
            }

            if (newX > Fence.MaxX)
            {
                newX = Fence.MaxX;
                randomTurn = true;
            }

            if (newY < Fence.MinY)
            {
                newY = Fence.MinY;
                randomTurn = true;
            }

            if (newY > Fence.MaxY)
            {
                newY = Fence.MaxY;
                randomTurn = true;
            }

            if (randomTurn)
                Orientation = Randomizer.NextDouble() * 2 * Math.PI;

            X = newX;
            Y = newY;
        }

        private class ZombieFence
        {
            public double MinX;
            public double MinY;
            public double MaxX;
            public double MaxY;
        }
    }
}