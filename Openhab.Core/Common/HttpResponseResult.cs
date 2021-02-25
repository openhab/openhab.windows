using System;
using System.Net;

namespace OpenHAB.Core.Common
{
    /// <summary>HTTP response result.</summary>
    /// <typeparam name="T">Content Type.</typeparam>
    public class HttpResponseResult<T>
    {
        /// <summary>Initializes a new instance of the <see cref="HttpResponseResult{T}" /> class.</summary>
        /// <param name="content">The content.</param>
        /// <param name="statusCode">The status code.</param>
        public HttpResponseResult(T content, HttpStatusCode? statusCode)
            : this(content, statusCode, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="HttpResponseResult{T}" /> class.</summary>
        /// <param name="content">The content.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="exception">The exception.</param>
        public HttpResponseResult(T content, HttpStatusCode? statusCode, Exception exception)
        {
            Content = content;
            StatusCode = statusCode;
            Exception = exception;
        }

        /// <summary>Gets the deserailized HTTP response content.</summary>
        /// <value>Deserailized HTTP response content.</value>
        public T Content
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the exception when available.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception
        {
            get;
            private set;
        }

        /// <summary>Gets the http status code.</summary>
        /// <value>The status code.</value>
        public HttpStatusCode? StatusCode
        {
            get; private set;
        }
    }
}
