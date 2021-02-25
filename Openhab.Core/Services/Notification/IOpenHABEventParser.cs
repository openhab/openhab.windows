using OpenHAB.Core.Model.Event;

namespace OpenHAB.Core.Services
{
    /// <summary>openHAB event parser.</summary>
    public interface IOpenHABEventParser
    {
        /// <summary>Parses the openHAB evnet message.</summary>
        /// <param name="message">The message.</param>
        /// <returns>
        ///   openHAB event object <see cref="OpenHABEvent"/>.
        /// </returns>
        OpenHABEvent Parse(string message);
    }
}