using System;
using System.Collections.Generic;
using System.Linq;

namespace ClimateGSR_in_ASP.NET_Core.Models
{
    /// <summary>
    /// Represents a country entity storing geographic, population, climate, and emissions data.
    /// </summary>
    public class Country
    {
        /// <summary>
        /// Unique database primary key identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ISO 3166-1 alpha-3 country code (e.g., USA, GBR, FRA).
        /// </summary>
        public string IsoCode { get; set; } = string.Empty;

        /// <summary>
        /// Official or commonly recognized name of the country.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Geographic continent where the country is located.
        /// </summary>
        public virtual Continent Continent { get; set; } = null!;

        /// <summary>
        /// Total recorded population of the country.
        /// </summary>
        public int Population { get; set; }

        /// <summary>
        /// Total surface area of the country in square kilometers (km²).
        /// </summary>
        public double Area { get; set; }

        /// <summary>
        /// Total annual CO2 emissions in metric tonnes.
        /// </summary>
        public double Emissions { get; set; }

        /// <summary>
        /// Annual CO2 emissions per capita in metric tonnes per person.
        /// </summary>
        public double EmissionsPerCapita { get; set; }

        /// <summary>
        /// Average annual temperature in degrees Celsius (°C), if recorded.
        /// </summary>
        public double? AverageTemperature { get; set; }
    }
}