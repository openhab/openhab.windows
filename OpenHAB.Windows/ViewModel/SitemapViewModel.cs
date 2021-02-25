using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using OpenHAB.Core.Model;
using OpenHAB.Core.SDK;
using OpenHAB.Windows.Services;

namespace OpenHAB.Windows.ViewModel
{
    /// <summary>
    /// A class that represents an OpenHAB sitemap.
    /// </summary>
    public class SitemapViewModel : ViewModelBase<OpenHABSitemap>
    {
        private ObservableCollection<OpenHABWidget> _widgets;
        private IOpenHAB _openHabsdk;

        /// <summary>
        /// Initializes a new instance of the <see cref="SitemapViewModel"/> class.
        /// </summary>
        public SitemapViewModel()
            : base(new OpenHABSitemap())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SitemapViewModel"/> class.
        /// </summary>
        /// <param name="model">Model class for view model.</param>
        public SitemapViewModel(OpenHABSitemap model)
             : base(model)
        {
            _widgets = new ObservableCollection<OpenHABWidget>();
            _openHabsdk = (IOpenHAB)DIService.Instance.Services.GetService(typeof(IOpenHAB));
        }

        /// <summary>
        /// Gets the name of the OpenHAB sitemap.
        /// </summary>
        public string Name
        {
            get
            {
                return Model.Name;
            }
        }

        /// <summary>
        /// Gets the label of the OpenHAB sitemap.
        /// </summary>
        public string Label
        {
            get
            {
                return Model.Label;
            }
        }

        /// <summary>
        /// Gets or sets a collection of widgets of the OpenHAB sitemap.
        /// </summary>
        public ObservableCollection<OpenHABWidget> Widgets
        {
            get
            {
                return _widgets;
            }

            set
            {
                if (Equals(value, _widgets))
                {
                    return;
                }

                _widgets = value;
                OnPropertyChanged();
            }
        }

        public async Task LoadWidgets(OpenHABVersion version)
        {
            Widgets.Clear();

            ICollection<OpenHABWidget> widgetModels = await _openHabsdk.LoadItemsFromSitemap(Model, version).ConfigureAwait(false);
            widgetModels.ToList().ForEach(x => Widgets.Add(x));
        }
    }
}
