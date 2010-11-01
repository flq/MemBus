using System.Windows;
using System.Windows.Controls;

namespace Membus.WpfTwitterClient.Frame.UI
{
    public class InteractionButtonTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is string)
                return tryFindResource("StringForAttention");
            return base.SelectTemplate(item, container);

        }

        private static DataTemplate tryFindResource(string resourceKey)
        {
            var dataTemplate = (DataTemplate)Application.Current.TryFindResource(resourceKey);
            return dataTemplate;
        }
    }
}