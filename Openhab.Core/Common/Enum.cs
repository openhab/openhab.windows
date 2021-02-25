using System;

namespace OpenHAB.Core.Common
{
    /// <summary>
    /// Now you can use Generics to have cleaner code when enum parsing!.
    /// </summary>
    /// <typeparam name="T">Enum type.</typeparam>
    /// <example>
    /// CT.Organ organNewParse = Enum.Parse("LENS");.
    /// </example>
    public static class Enum<T>
    {
        /// <summary>Parses the text to enum value.</summary>
        /// <param name="value">The value.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static T Parse(string value, bool ignoreCase = true)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }
    }
}
