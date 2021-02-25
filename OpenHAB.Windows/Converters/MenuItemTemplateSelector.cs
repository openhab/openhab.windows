using OpenHAB.Windows.ViewModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace OpenHAB.Windows.Converters
{
    /// <summary>Template selector to differeniate between sitemaps and general menu items.</summary>
    [ContentProperty(Name = "SiteMapMenuItemTemplate")]
    public class MenuItemTemplateSelector : DataTemplateSelector
    {
        /// <summary>Gets or sets the sitemap menuitem template.</summary>
        /// <value>The site map menu item template.</value>
        public DataTemplate SiteMapMenuItemTemplate
        {
            get;
            set;
        }

        /// <summary>Gets or sets the item temlate for default menuitems.</summary>
        /// <value>The default item temlate.</value>
        public DataTemplate DefaultItemTemlate
        {
            get; set;
        }

        /// <inheritdoc/>
        protected override DataTemplate SelectTemplateCore(object item)
        {
            SitemapViewModel sitemapMenuItem = item as SitemapViewModel;

            if (sitemapMenuItem != null)
            {
                return SiteMapMenuItemTemplate;
            }

            return DefaultItemTemlate;
        }
    }
}
