using OpenHAB.Windows.ViewModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace OpenHAB.Windows.Controls
{
    /// <summary>Dialog control for connection configruation.</summary>
    /// <seealso cref="ContentDialog" />
    /// <seealso cref="IComponentConnector" />
    /// <seealso cref="IComponentConnector2" />
    public sealed partial class ConnectionDialog : ContentDialog
    {
        /// <summary>Initializes a new instance of the <see cref="ConnectionDialog"/> class.</summary>
        public ConnectionDialog()
        {
            this.InitializeComponent();
        }
    }
}
