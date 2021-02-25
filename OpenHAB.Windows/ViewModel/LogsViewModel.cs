using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using OpenHAB.Core.Common;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;

namespace OpenHAB.Windows.ViewModel
{
    /// <summary>
    /// ViewModel for application logs.
    /// </summary>
    public class LogsViewModel : ViewModelBase<object>, IDisposable
    {
        private StorageFile _logFile;
        private string _logFileContent;
        private FileSystemWatcher _logFileWatcher;
        private ILogger<LogsViewModel> _logger;
        private string _logFilename = $"{DateTime.Now:yyyy-MM-dd}.log";
        private ICommand _openLogFileCommand;

        private CoreDispatcher _dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

        /// <summary>Initializes a new instance of the <see cref="LogsViewModel" /> class.</summary>
        public LogsViewModel(ILogger<LogsViewModel> logger)
            : base(null)
        {
            _logger = logger;
            LoadLogfileAsync();
        }

        /// <summary>Gets or sets the content of the log.</summary>
        /// <value>The content of the log.</value>
        public string LogContent
        {
            get
            {
                return _logFileContent;
            }

            set
            {
                Set(ref _logFileContent, value);
            }
        }

        /// <summary>Gets the log file path.</summary>
        /// <value>The log file path.</value>
        public string LogFilePath
        {
            get
            {
                if (_logFile != null)
                {
                    return _logFile.Path;
                }

                return string.Empty;
            }
        }

        private async Task LoadLogfileAsync()
        {
            await LoadLogFileContent().ConfigureAwait(false);
            RegisterFileObserver();
        }

        private async Task LoadLogFileContent()
        {
            _logger.LogInformation($"Load current log file: {_logFilename}");

            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            _logFolder = await storageFolder.GetFolderAsync("logs");
            if (_logFolder == null)
            {
                return;
            }

            _logFile = await _logFolder.GetFileAsync(_logFilename);
            if (_logFile == null)
            {
                return;
            }

            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                LogContent = await FileIO.ReadTextAsync(_logFile);
            });
        }

        private void RegisterFileObserver()
        {
            _logger.LogInformation("Register FileSystemWatch to load changed log file.");

            _logFileWatcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(_logFile.Path),
                Filter = _logFilename
            };

            _logFileWatcher.Changed += LogFile_Changed;
            _logFileWatcher.EnableRaisingEvents = true;
        }

        private async void LogFile_Changed(object sender, FileSystemEventArgs e)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                LogContent = await FileIO.ReadTextAsync(_logFile);
            });
        }

        #region Command

        /// <summary>Gets the command to open log files directory.</summary>
        /// <value>The open log file command.</value>
        public ICommand OpenLogFileCommand => _openLogFileCommand ?? (_openLogFileCommand = new ActionCommand(ExecuteOpenLogFileCommand, CanExecuteOpenLogFileCommand));

        private bool CanExecuteOpenLogFileCommand(object arg)
        {
            return true;
        }

        private void ExecuteOpenLogFileCommand(object obj)
        {
            _logger.LogInformation($"Open log files directory: '{_logFolder.Path}'");
            Launcher.LaunchFolderAsync(_logFolder);
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls
        private StorageFolder _logFolder;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _logFileWatcher.Dispose();
                }

                disposedValue = true;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
