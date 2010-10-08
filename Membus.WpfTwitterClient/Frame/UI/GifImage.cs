using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Membus.WpfTwitterClient.Frame.UI
{
    /// <summary>
    /// Courtesy of a Stackoverflow solution shown here:
    /// http://stackoverflow.com/questions/210922/how-do-i-get-an-animated-gif-to-work-in-wpf
    /// </summary>
    public class GifImage : Image
    {
        GifBitmapDecoder gf;
        Int32Animation anim;
        bool animationIsWorking;

        public int FrameIndex
        {
            get { return (int)GetValue(FrameIndexProperty); }
            set { SetValue(FrameIndexProperty, value); }
        }

        public static readonly DependencyProperty FrameIndexProperty =
            DependencyProperty.Register("FrameIndex", typeof(int), typeof(GifImage), new UIPropertyMetadata(0, onChangingFrameIndex));

        public static readonly DependencyProperty FramesPerSecondProperty =
            DependencyProperty.Register("FramesPerSecond", typeof (int), typeof (GifImage), new PropertyMetadata(10));

        public int FramesPerSecond
        {
            get { return (int) GetValue(FramesPerSecondProperty); }
            set { SetValue(FramesPerSecondProperty, value); }
        }

        public static readonly DependencyProperty GifUriProperty =
            DependencyProperty.Register("GifUri", typeof (string), typeof (GifImage));

        public string GifUri
        {
            get { return (string) GetValue(GifUriProperty); }
            set { SetValue(GifUriProperty, value); }
        }


        public GifImage()
        {
            Initialized += onInitialized;
        }

        private void onInitialized(object sender, EventArgs e)
        {
            gf = new GifBitmapDecoder(new Uri(GifUri), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            var millisecondPerFrame = 1000.0 / Convert.ToDouble(FramesPerSecond);
            anim = new Int32Animation(0, gf.Frames.Count - 1, TimeSpan.FromMilliseconds(millisecondPerFrame * gf.Frames.Count))
            {
                RepeatBehavior = RepeatBehavior.Forever
            };
            Source = gf.Frames[0];
        }

        private static void onChangingFrameIndex(DependencyObject obj, DependencyPropertyChangedEventArgs ev)
        {
            var ob = (GifImage)obj;
            ob.Source = ob.gf.Frames[(int)ev.NewValue];
            ob.InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            if (animationIsWorking) return;
            BeginAnimation(FrameIndexProperty, anim);
            animationIsWorking = true;
        }
    }
}