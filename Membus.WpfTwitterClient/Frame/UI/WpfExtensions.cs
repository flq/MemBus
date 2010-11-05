using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Membus.WpfTwitterClient.Frame.UI
{
    public static class WpfExtensions
    {
        /// <summary>
        /// Creates an ImageSource from any UIElement, processing it to the target size
        /// </summary>
        public static ImageSource ToImage(this UIElement source, Size targetSize)
        {
            var actualHeight = source.RenderSize.Height;
            var actualWidth = source.RenderSize.Width;

            var scale = new Size(targetSize.Width/source.RenderSize.Width, targetSize.Height/source.RenderSize.Height);

            var renderTarget = new RenderTargetBitmap((int)targetSize.Width, (int)targetSize.Height, 96, 96, PixelFormats.Pbgra32);

            var sourceBrush = new VisualBrush(source);
            var drawingVisual = new DrawingVisual();
            var drawingContext = drawingVisual.RenderOpen();

            using (drawingContext)
            {
                drawingContext.PushTransform(new ScaleTransform(scale.Width, scale.Height));
                drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0), new Point(actualWidth, actualHeight)));
            }
            renderTarget.Render(drawingVisual);
            return renderTarget;
        }

        public static void Invoke(this Dispatcher dispatcher, Action action)
        {
            dispatcher.Invoke(DispatcherPriority.Normal, action);
        }
    }
}