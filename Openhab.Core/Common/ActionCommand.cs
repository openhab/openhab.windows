using System;
using System.Windows.Input;

namespace OpenHAB.Core.Common
{
    /// <summary>
    ///   Action Command for MVVM.
    /// </summary>
    public class ActionCommand : ICommand
    {
        private readonly Func<object, bool> _canExecuteHandler;
        private readonly Action<object> _executeHandler;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ActionCommand" /> class.
        /// </summary>
        /// <param name = "execute">The execute.</param>
        /// <param name = "canExecute">The can execute.</param>
        public ActionCommand(Action<object> execute, Func<object, bool> canExecute)
            : this(execute)
        {
            _canExecuteHandler = canExecute;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ActionCommand" /> class.
        /// </summary>
        /// <param name = "execute">The execute.</param>
        public ActionCommand(Action<object> execute)
        {
            _executeHandler = execute;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Invokes the can execute changed.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void InvokeCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged?.Invoke(this, e);
        }

        /// <summary>
        ///   Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name = "parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            _executeHandler(parameter);
        }

        /// <summary>
        ///   Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name = "parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        ///   true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            if (_canExecuteHandler == null)
            {
                return true;
            }

            return _canExecuteHandler(parameter);
        }
    }
}