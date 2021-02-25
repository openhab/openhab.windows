using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using OpenHAB.Core;
using OpenHAB.Core.Common;
using OpenHAB.Core.Contracts;
using OpenHAB.Core.Model;
using OpenHAB.Core.Model.Connection;
using OpenHAB.Core.SDK;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace OpenHAB.Windows.ViewModel
{
    /// <summary>
    /// ViewModel for OpenHAB connection dialog.
    /// </summary>
    public class ConnectionDialogViewModel : ViewModelBase<OpenHABConnection>
    {
        private readonly IOpenHAB _openHabsdk;
        private readonly OpenHABHttpClientType _type;
        private string _password;
        private ConnectionProfileViewModel _profile;
        private ObservableCollection<ConnectionProfileViewModel> _profiles;
        private ICommand _selectProfile;
        private string _url;
        private ICommand _urlCheckCommand;
        private OpenHABUrlState _connectionState;
        private string _username;
        private bool? _willIgnoreSSLCertificate;
        private bool? _willIgnoreSSLHostname;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionDialogViewModel"/> class.
        /// </summary>
        /// <param name="connectionConfig">The connection configuration.</param>
        /// <param name="openHabsdk">OpenHABSDK class.</param>
        /// <param name="type">Defines if openHAB instance is local or remote.</param>
        public ConnectionDialogViewModel(OpenHABConnection connectionConfig, IOpenHAB openHabsdk, OpenHABHttpClientType type)
            : base(connectionConfig)
        {
            _openHabsdk = openHabsdk;
            _type = type;
            _connectionState = OpenHABUrlState.Unknown;

            List<ConnectionProfileViewModel> list
                = new List<ConnectionProfileViewModel>(Settings.ConnectionProfiles.Where(x => x.Type == _type).OrderBy(x => x.Id).Select(x => new ConnectionProfileViewModel(x)));

            _profiles = new ObservableCollection<ConnectionProfileViewModel>(list);

            if (Model != null)
            {
                _profile = list.FirstOrDefault(x => x.Id == Model.Profile.Id);
            }

            if (!string.IsNullOrEmpty(Model?.Url))
            {
                CheckConnectionSettings(Model.Url);
            }
        }

        /// <summary>Gets a value indicating whether [host URL] value can be modified.</summary>
        /// <value>
        ///   <c>true</c> if [host URL configuration] can be modified; otherwise, <c>false</c>.</value>
        public bool AllowHostUrlConfiguration
        {
            get => Profile?.AllowHostUrlConfiguration ?? false;
        }

        /// <summary>Gets a value indicating whether [allow ignore SSL certificate] issues option is available.</summary>
        /// <value>
        ///   <c>true</c> if [allow ignore SSL certificate] is available; otherwise, <c>false</c>.</value>
        public bool AllowIgnoreSSLCertificate
        {
            get => Profile?.AllowIgnoreSSLCertificate ?? false;
        }

        /// <summary>Gets a value indicating whether [host URL] value can be modified.</summary>
        /// <value>
        ///   <c>true</c> if [host URL configuration] can be modified; otherwise, <c>false</c>.</value>
        public bool AllowIgnoreSSLHostname
        {
            get => Profile?.AllowIgnoreSSLHostname ?? false;
        }

        /// <summary>
        /// Gets or sets the password for the local OpenHAB connection.
        /// </summary>
        public string Password
        {
            get
            {
                return Model?.Password;
            }

            set
            {
                Set(ref _password, value, true);
                Model.Password = value;
            }
        }

        /// <summary>Gets or sets the profile for the connection.</summary>
        /// <value>The profile.</value>
        public ConnectionProfileViewModel Profile
        {
            get
            {
                return _profile;
            }

            set
            {
                Set(ref _profile, value);

                if (Model != null && value != null)
                {
                    Model.Profile = value.Model;
                }
            }
        }

        /// <summary>Gets or sets the available connection profiles.</summary>
        /// <value>The profiles.</value>
        public ObservableCollection<ConnectionProfileViewModel> Profiles
        {
            get
            {
                return _profiles;
            }

            set
            {
                Set(ref _profiles, value);
            }
        }

        /// <summary>
        /// Gets the save command to persist the settings.
        /// </summary>
        /// <value>The save command.</value>
        public ICommand SelectProfile => _selectProfile ?? (_selectProfile = new ActionCommand(ExecuteSelectProfile));

        /// <summary>
        /// Gets the subtitle.
        /// </summary>
        /// <value>The subtitle.</value>
        public string Subtitle
        {
            get
            {
                if (string.IsNullOrEmpty(Url))
                {
                    return AppResources.Values.GetString("ConnectionNotConfigured");
                }

                if (Uri.TryCreate(Url, UriKind.Absolute, out Uri uri) &&
                    string.Compare(uri.Scheme.ToUpperInvariant(), "HTTPS", StringComparison.InvariantCulture) == 0)
                {
                    return $"{AppResources.Values.GetString("EncryptedConnection")} {Url}";
                }

                return $"{AppResources.Values.GetString("UnencryptedConnection")} {Url}";
            }
        }

        /// <summary>
        /// Gets or sets the URL to the OpenHAB server.
        /// </summary>
        public string Url
        {
            get
            {
                return Model?.Url;
            }

            set
            {
                string tempUrl;
                if (!string.IsNullOrEmpty(value) && !value.EndsWith("/", StringComparison.InvariantCultureIgnoreCase))
                {
                    tempUrl = value + "/";
                }
                else
                {
                    tempUrl = value;
                }

                Set(ref _url, tempUrl, true);
                Model.Url = _url;

                OnPropertyChanged(nameof(Subtitle));
            }
        }

        /// <summary>
        /// Gets the command for local URL check.
        /// </summary>
        /// <value>The local URL check command.</value>
        public ICommand UrlCheckCommand => _urlCheckCommand ?? (_urlCheckCommand = new RelayCommand<object>(CheckConnectionSettings));

        /// <summary>
        /// Gets or sets the state for OpenHab connection.
        /// </summary>
        /// <value>The state of the connection.</value>
        public OpenHABUrlState State
        {
            get => _connectionState;
            set => Set(ref _connectionState, value);
        }

        /// <summary>
        /// Gets or sets the username for the local OpenHAB server connection.
        /// </summary>
        public string Username
        {
            get
            {
                return Model?.Username;
            }

            set
            {
                Set(ref _username, value, true);
                Model.Username = value;
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the application will ignore the SSL certificate.
        /// </summary>
        public bool? WillIgnoreSSLCertificate
        {
            get
            {
                return Model?.WillIgnoreSSLCertificate;
            }

            set
            {
                Set(ref _willIgnoreSSLCertificate, value, true);
                Model.WillIgnoreSSLCertificate = value;
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the application will ignore the SSL hostname.
        /// </summary>
        public bool? WillIgnoreSSLHostname
        {
            get
            {
                return Model?.WillIgnoreSSLHostname;
            }

            set
            {
                Set(ref _willIgnoreSSLHostname, value, true);
                Model.WillIgnoreSSLHostname = value;
            }
        }

        private void CheckConnectionSettings(object parameter)
        {
            if (parameter == null)
            {
                return;
            }

            string url = parameter.ToString();

            Task<HttpResponseResult<bool>> result = _openHabsdk.CheckUrlReachability(this.Model);
            result.ContinueWith(async (task) =>
            {
                OpenHABUrlState urlState = OpenHABUrlState.Unknown;
                if (task.IsCompletedSuccessfully && task.Result.Content)
                {
                    urlState = OpenHABUrlState.OK;
                }
                else
                {
                    urlState = OpenHABUrlState.Failed;
                }

                CoreDispatcher dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    State = urlState;
                });
            });
        }

        private void CreateProfile(IConnectionProfile value)
        {
            Model = value.CreateConnection();
            Model.Profile = value;

            Url = Model.Url;
            Username = Model.Username;
            Password = Model.Password;
            WillIgnoreSSLCertificate = Model.WillIgnoreSSLCertificate;
            WillIgnoreSSLHostname = Model.WillIgnoreSSLHostname;
        }

        private void ExecuteSelectProfile(object obj)
        {
            ConnectionProfileViewModel profile = obj as ConnectionProfileViewModel;

            if (profile != null)
            {
                CreateProfile(profile.Model);

                OnPropertyChanged(nameof(AllowHostUrlConfiguration));
                OnPropertyChanged(nameof(AllowIgnoreSSLCertificate));
                OnPropertyChanged(nameof(AllowIgnoreSSLHostname));
            }
        }
    }
}
