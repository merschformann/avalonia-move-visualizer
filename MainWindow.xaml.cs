using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace avalonia_animation
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Get drawing board
            var drawingBoard = this.Find<Canvas>("CanvasDrawingBoard");

            // Spawn some zombies for movement
            const int count = 10;
            _zombies = Enumerable.Range(0, count).Select(i =>
                new Zombie(i / (double) count * drawingBoard.Width, i / (double) count * drawingBoard.Height, 50, 50,
                    0, 0, drawingBoard.Width, drawingBoard.Height)).ToList();

            // Add zombie visuals to canvas
            foreach (var zombie in _zombies)
                drawingBoard.Children.Add(zombie.Visual);

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

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private readonly List<Zombie> _zombies;
        private readonly Timer _testTimer;

        private void Update()
        {
            // Update visual of all zombies
            foreach (var zombie in _zombies)
                zombie.UpdateVisual();
        }
    }
}