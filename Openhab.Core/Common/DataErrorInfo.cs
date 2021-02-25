using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace OpenHAB.Core.Common
{
    /// <summary>
    ///   Implementation of INotifyDataErrorInfo.
    /// </summary>
    public class DataErrorInfo : INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errors;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataErrorInfo"/> class.
        /// </summary>
        public DataErrorInfo()
        {
            _errors = new Dictionary<string, List<string>>();
        }

        /// <summary>
        ///   Adds the specified error to the errors collection if it is not
        ///   already present, inserting it in the first position if isWarning is
        ///   false. Raises the ErrorsChanged event if the collection changes.
        /// </summary>
        /// <param name = "propertyName">Name of the property.</param>
        /// <param name = "error">The error.</param>
        /// <param name = "isWarning">if set to <c>true</c> [is warning].</param>
        public void AddError(string propertyName, string error, bool isWarning)
        {
            if (!_errors.ContainsKey(propertyName))
            {
                _errors[propertyName] = new List<string>();
            }

            if (!_errors[propertyName].Contains(error))
            {
                if (isWarning)
                {
                    _errors[propertyName].Add(error);
                }
                else
                {
                    _errors[propertyName].Insert(0, error);
                }

                InvokeErrorsChanged(propertyName);
            }
        }

        /// <summary>
        ///   Removes the specified error from the errors collection if it is
        ///   present. Raises the ErrorsChanged event if the collection changes.
        /// </summary>
        /// <param name = "propertyName">Name of the property.</param>
        /// <param name = "error">The error.</param>
        public void RemoveError(string propertyName, string error)
        {
            if (_errors.ContainsKey(propertyName) && _errors[propertyName].Contains(error))
            {
                _errors[propertyName].Remove(error);

                if (_errors[propertyName].Count == 0)
                {
                    _errors.Remove(propertyName);
                }

                InvokeErrorsChanged(propertyName);
            }
        }

        /// <summary>
        ///   Raises the errors changed.
        /// </summary>
        /// <param name = "propertyName">Name of the property.</param>
        public void InvokeErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
            {
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        ///   Occurs when the validation errors have changed for a property or for the entire object.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        ///   Gets the validation errors for a specified property or for the entire object.
        /// </summary>
        /// <param name = "propertyName">The name of the property to retrieve validation errors for, or null or <see
        ///    cref = "string.Empty" /> to retrieve errors for the entire object.</param>
        /// <returns>
        ///   The validation errors for the property or object.
        /// </returns>
        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_errors.ContainsKey(propertyName))
            {
                return null;
            }

            return _errors[propertyName];
        }

        /// <summary>
        ///   Gets a value indicating whether gets a value that indicates whether the object has validation errors.
        /// </summary>
        /// <returns>true if the object currently has validation errors; otherwise, false.</returns>
        public bool HasErrors
        {
            get
            {
                return _errors.Count > 0;
            }
        }
    }
}