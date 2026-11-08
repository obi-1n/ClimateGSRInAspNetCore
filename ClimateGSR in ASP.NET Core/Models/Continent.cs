using System;
using System.Collections.Generic;
using System.Linq;

namespace ClimateGSR_in_ASP.NET_Core.Models
{
    /// <summary>
    /// Represents a geographic continent entity containing related countries.
    /// </summary>
    public class Continent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Continent"/> class.
        /// Pre-populates the navigation collection of countries.
        /// </summary>
        public Continent()
        {
            Countries = new HashSet<Country>();
        }

        /// <summary>
        /// Primary key identifier for the continent.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the continent (e.g., Africa, Europe, Asia).
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Total number of countries associated with this continent.
        /// </summary>
        public int TotalCountries { get; set; }

        /// <summary>
        /// Navigation collection of countries belonging to this continent.
        /// </summary>
        public virtual ICollection<Country> Countries { get; set; }
    }
}