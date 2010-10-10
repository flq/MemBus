using System;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media.Animation;

namespace Membus.WpfTwitterClient.Frame.UI
{
    public class FadeOnVisibilityBehaviour : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty VisibleProperty =
            DependencyProperty.Register("Visible", typeof (bool), typeof (FadeOnVisibilityBehaviour), new PropertyMetadata(false, onVisibilityChanged));

        public bool Visible
        {
            get { return (bool) GetValue(VisibleProperty); }
            set { SetValue(VisibleProperty, value); }
        }


        private static void onVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fadeInOutBehaviour = (FadeOnVisibilityBehaviour) d;

            if (e.OldValue.Equals(e.NewValue))
                return;

            var obj = fadeInOutBehaviour.AssociatedObject;

            var visible = (bool) e.NewValue;

            var da = new DoubleAnimation
                         {
                             Duration = new Duration(TimeSpan.FromSeconds(1))
                         };


            da.Completed += (o, a) =>
                                {
                                    if (!visible) //Collapse after animation
                                        obj.Visibility = Visibility.Collapsed;
                                };

            if (visible)
            {
                da.From = 0.0;
                da.To = 1.0;
            }
            else
            {
                da.From = 1.0;
                da.To = 0.0;
            }
            if (visible)
            {
                // Visible before animation
                obj.Opacity = 0.0;
                obj.Visibility = Visibility.Visible;
            }
            obj.BeginAnimation(UIElement.OpacityProperty, da);
        }
    }
}