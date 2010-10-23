using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Membus.WpfTwitterClient.Frame.UI
{
    /// <summary>
    /// Interaction logic for InteractionButton.xaml
    /// </summary>
    public partial class InteractionButton : UserControl
    {
        private const int MillisecondsToPopupClose = 50;
        private readonly object isOpenLock = new object();
        private volatile bool isOpen;
        private volatile bool popupCloseTimerCancelled;
        private readonly Timer popupCloseTimer;

        public InteractionButton()
        {
            InitializeComponent();
            popupCloseTimer = new Timer(onTimer,null, -1, -1);
            Loaded += onLoaded;
        }

        public DoubleAnimationBase GradientAnimation
        {
            get { return (DoubleAnimationBase) TryFindResource("gradientTimeline"); }
        }

        private void onLoaded(object sender, RoutedEventArgs e)
        {
            var geometryDrawing = unfreezeGeometry();
            applyGradientAnimation(geometryDrawing);
        }

        private GeometryDrawing unfreezeGeometry()
        {
            var b = (Brush)TryFindResource("attentionbutton_info");
            var drawingGroup = (DrawingGroup) img.Drawing;
            var dgUnfrozen = drawingGroup.Clone();
            var geometryDrawing = ((GeometryDrawing)dgUnfrozen.Children[1]);
            geometryDrawing.Brush = b.Clone();
            img.Drawing = dgUnfrozen;
            return geometryDrawing;
        }

        private void onButtonMouseEnter(object sender, MouseEventArgs e)
        {
            popupCloseTimerCancelled = true;
            popupBorder.Opacity = 0.5;
            if (isOpen)
                return;
            lock (isOpenLock)
            {
                if (isOpen) return;
                openPopup();
                isOpen = true;
            }
        }

        private void onButtonMouseLeave(object sender, MouseEventArgs e)
        {
            popupCloseTimerCancelled = false;
            popupCloseTimer.Change(MillisecondsToPopupClose, -1);
        }

        private void onPopupMouseEnter(object sender, MouseEventArgs e)
        {
            popupCloseTimerCancelled = true;
            popupBorder.Opacity = 1;
        }

        private void onTimer(object state)
        {
            if (popupCloseTimerCancelled || !isOpen)
                return;
            lock (isOpenLock)
            {
                if (popupCloseTimerCancelled || !isOpen)
                    return;
                closePopup();
            }
        }

        private void applyGradientAnimation(GeometryDrawing geometryDrawing)
        {
            var s = ((RadialGradientBrush) geometryDrawing.Brush).GradientStops[1];
            var c = GradientAnimation.CreateClock();
            s.ApplyAnimationClock(GradientStop.OffsetProperty, c);
        }

        private void openPopup()
        {
            var openStory = (Storyboard) TryFindResource("open");
            popupBorder.Opacity = 0.6;
            popup.IsOpen = true;
            openStory.Begin();
        }

        private void closePopup()
        {
            Dispatcher.Invoke(new Action(()=>
                                             {
                                                 var closeStory = (Storyboard)TryFindResource("close");
                                                 closeStory.Completed += (s, e) =>
                                                                             {
                                                                                 popup.IsOpen = false;
                                                                                 isOpen = false;
                                                                             };
                                                 closeStory.Begin();
                                             }));
        }
    }
}
