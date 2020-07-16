using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
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
            _canvas = this.Find<Canvas>("CanvasDrawingBoard");
            _elementRenderer = this.Find<ElementRenderer>("CustomSkia");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private readonly Canvas _canvas;
        private CanvasRenderer _canvasRenderer;

        private ElementRenderer _elementRenderer;
        
        #region Button handlers

        public void CanvasStart_Click(object sender, RoutedEventArgs args)
        {
            _canvasRenderer?.Stop();
            _canvasRenderer = new CanvasRenderer(_canvas);
            _canvasRenderer.Start();
        }
        
        public void CanvasStop_Click(object sender, RoutedEventArgs args)
        {
            _canvasRenderer?.Stop();
        }
        
        public void CustomSkiaStart_Click(object sender, RoutedEventArgs args)
        {
            _elementRenderer?.Stop();
            _elementRenderer?.Start();
        }
        
        public void CustomSkiaStop_Click(object sender, RoutedEventArgs args)
        {
            _elementRenderer?.Stop();
        }
        
        #endregion
    }
}