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
        }

        private static readonly Random Randomizer = new Random(0);

        private const double Speed = 20;

        private ZombieFence Fence { get; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Orientation { get; set; }
        public double LengthX { get; set; }
        public double LengthY { get; set; }

        private double _lastMove = double.NaN;

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