using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace avalonia_animation
{
    public class CanvasRenderer
    {
        public CanvasRenderer(Canvas drawingBoard)
        {
            _drawingBoard = drawingBoard;
        }

        private readonly Canvas _drawingBoard;
        private List<Zombie> _zombies;
        private Timer _testTimer;
        private readonly Dictionary<Zombie, IControl> _visuals = new Dictionary<Zombie, IControl>();

        public void Start()
        {
            // Stop all previous rendering
            Stop();

            // Spawn some zombies for movement
            const int count = 100;
            _zombies = Enumerable.Range(0, count).Select(i =>
                new Zombie(i / (double) count * _drawingBoard.Width, i / (double) count * _drawingBoard.Height, 50, 50,
                    0, 0, _drawingBoard.Width, _drawingBoard.Height)).ToList();
            foreach (var zombie in _zombies)
                GenerateZombieVisual(zombie);
            
            // Add zombie visuals to canvas
            Dispatcher.UIThread.Post(() =>
            {
                foreach (var zombie in _zombies)
                    _drawingBoard.Children.Add(_visuals[zombie]);
            });

            // Every few moments: update zombie positions and visuals
            var startTime = DateTime.Now;
            _testTimer = new Timer(ignored =>
            {
                // Logically update positions of the zombies
                foreach (var zombie in _zombies.Skip(1)) // Leave one not moving at all (to show where it is initially drawn)
                    zombie.Move((DateTime.Now - startTime).TotalSeconds);
                // Update visuals accordingly
                Dispatcher.UIThread.Post(Update);
            }, null, 50, 25);
        }

        public void Stop()
        {
            _testTimer?.Change(-1, -1);
            Dispatcher.UIThread.Post(() =>
            {
                foreach (var zombie in _zombies)
                    _drawingBoard.Children.Remove(_visuals[zombie]);
            });
        }

        public void GenerateZombieVisual(Zombie zombie)
        {
            // Put together compound visual
            var frame = new GeometryDrawing
            {
                Geometry = new RectangleGeometry(new Rect(new Size(zombie.LengthX, zombie.LengthY))),
                Brush = Brushes.LimeGreen,
                Pen = new Pen(Brushes.Black, 5)
            };
            var orientIndicator = new GeometryDrawing
            {
                Geometry = new LineGeometry(new Point(zombie.LengthX / 2.0, zombie.LengthY / 2.0), new Point(zombie.LengthX, zombie.LengthY / 2.0)),
                Brush = Brushes.Black,
                Pen = new Pen(Brushes.Black, 5)
            };
            var negativeLineExample = new GeometryDrawing
            {
                Geometry = new LineGeometry(new Point(zombie.LengthX / 2.0, zombie.LengthY / 2.0), new Point(-zombie.LengthX, zombie.LengthY / 2.0)),
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
            // Store visual locally
            _visuals[zombie] = presenter;
        }

        private void Update()
        {
            // Update visual of all zombies
            foreach (var zombie in _zombies)
                UpdateVisual(zombie);
        }

        private void UpdateVisual(Zombie z)
        {
            var t = new TransformGroup();
            t.Children.Add(new RotateTransform(Util.RadToDeg(z.Orientation)));
            // This needs to convert X/Y given as center coordinates to top-left coordinates
            // Alternatively, the anchor should already be centered similar to the rotation and RenderTransformOrigin (0.5,0.5)
            t.Children.Add(new TranslateTransform(z.X, z.Y));
            _visuals[z].RenderTransform = t;
        }
    }
}