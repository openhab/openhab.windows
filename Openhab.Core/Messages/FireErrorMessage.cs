using OpenHAB.Core.Common;

namespace OpenHAB.Core.Messages
{
    /// <summary>
    /// Triggers a visual error message.
    /// </summary>
    public class FireErrorMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FireErrorMessage"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        public FireErrorMessage(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Gets or sets the error text.
        /// </summary>
        /// <value>The text.</value>
        public string ErrorMessage
        {
            get; set;
        }
    }
}
