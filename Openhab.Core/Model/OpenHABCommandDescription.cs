using System.Collections.Generic;

namespace OpenHAB.Core.Model
{
    /// <summary>
    /// A mapping for an OpenHAB Widget.
    /// </summary>
    public class OpenHABCommandDescription
    {
        /// <summary>
        /// Gets or sets the CommandOptions.
        /// </summary>
        public ICollection<OpenHABCommandOptions> CommandOptions { get; set; }

        /// <summary>Initializes a new instance of the <see cref="OpenHABCommandDescription" /> class.</summary>
        /// <param name="commandOptions">The command options.</param>
        public OpenHABCommandDescription(ICollection<OpenHABCommandOptions> commandOptions)
        {
            CommandOptions = commandOptions;
        }
    }
}