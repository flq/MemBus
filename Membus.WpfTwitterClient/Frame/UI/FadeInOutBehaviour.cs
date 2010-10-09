using System;
using System.Collections.Concurrent;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media.Animation;

namespace Membus.WpfTwitterClient.Frame.UI
{
    public class FadeInOutBehaviour : Behavior<FrameworkElement>
    {
        private static readonly ConcurrentDictionary<FrameworkElement, bool> elements = new ConcurrentDictionary<FrameworkElement, bool>();

        static FadeInOutBehaviour()
        {
            UIElement.VisibilityProperty
                .AddOwner(typeof(FrameworkElement),
                          new FrameworkPropertyMetadata(Visibility.Visible, new PropertyChangedCallback(onVisibilityChanged), onCoerceVisibility)); 
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            elements.TryAdd(AssociatedObject, false);
        }

        protected override void OnDetaching()
        {
            bool animationValue;
            elements.TryRemove(AssociatedObject, out animationValue);
            base.OnDetaching();
        }

        private static object onCoerceVisibility(DependencyObject d, object baseValue)
        {
            var fe = d as FrameworkElement;
            if (fe == null)
                return baseValue;

            if (checkAndUpdateAnimationStartedFlag(fe))
                return baseValue;

            // If we get here, it means we have to start fade in or fade out
            // animation. In any case return value of this method will be
            // Visibility.Visible. 

            var visibility = (Visibility)baseValue;

            var da = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(200))
            };

            // This will trigger value coercion again
            // but checkAndUpdateAnimationStartedFlag() function will reture true
            // this time, and animation will not be triggered.
            da.Completed += (o, e) =>
            {
                fe.Visibility = visibility;
            };

            if (visibility == Visibility.Collapsed || visibility == Visibility.Hidden)
            {
                da.From = 1.0;
                da.To = 0.0;
            }
            else
            {
                da.From = 0.0;
                da.To = 1.0;
            }

            fe.BeginAnimation(UIElement.OpacityProperty, da);
            return Visibility.Visible;
        }

        private static void onVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        private static bool checkAndUpdateAnimationStartedFlag(FrameworkElement fe)
        {
            if (!elements.ContainsKey(fe))
                return true; // don't need to animate unhooked elements.

            bool animationStarted;
            var success = elements.TryGetValue(fe, out animationStarted);
            if (success)
                elements.TryUpdate(fe, !animationStarted, animationStarted);
            
            return animationStarted;
        }
    }
}