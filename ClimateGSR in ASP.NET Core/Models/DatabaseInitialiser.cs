using System.Collections.Generic;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ClimateGSR_in_ASP.NET_Core.Models
{
    /// <summary>
    /// Seeds missing climate data without deleting existing accounts or scores.
    /// Reads and merges country, emissions, temperature, demographic, and continent data from App_Data CSV/text files.
    /// </summary>
    public static class DatabaseInitialiser
    {
        /// <summary>
        /// Seeds the database with country and continent records parsed and aggregated from raw data files in ~/App_Data/.
        /// </summary>
        /// <param name="context">The database context to seed.</param>
        /// <param name="contentRootPath">The app content root (IWebHostEnvironment.ContentRootPath).</param>
        public static void Seed(ClimateGSRDbContext context, string contentRootPath)
        {
            if (context.Countries.Any()) return;
            string dir = Path.Combine(contentRootPath, "App_Data");
            var inv = CultureInfo.InvariantCulture;
            var countries = new Dictionary<string, Country>();

            // 1. Emissions: column 36 = 2024 (0 = code, 1 = name, 2 = 1990)
            // Parse CO2 emissions CSV and create initial country dictionary indexed by ISO code.
            foreach (var line in File.ReadLines(Path.Combine(dir, "CO2.csv")).Skip(1))
            {
                var p = line.Split(',');
                double value;
                if (p.Length < 37 || p[0] == "WLD") continue; // Exclude World aggregate
                if (!double.TryParse(p[36], NumberStyles.Float, inv, out value)) continue;

                countries[p[0]] = new Country
                {
                    IsoCode = p[0],
                    Name = p[1].Trim(),
                    Emissions = value
                };
            }

            // 2. Temperature: assumes columns Entity, Code, Year, Value.
            // Parse temperature data for year 2025 and attach average temperature to matching countries.
            foreach (var line in File.ReadLines(Path.Combine(dir, "temperature.csv")).Skip(1))
            {
                var p = line.Split(',');
                Country? c;
                double temp;
                if (p.Length < 4 || p[2] != "2025") continue;
                if (!countries.TryGetValue(p[1], out c)) continue; // also drops OWID aggregates
                if (double.TryParse(p[3], NumberStyles.Float, inv, out temp))
                    c.AverageTemperature = temp;
            }

            // 3. GeoNames: continent code mapping, surface area, and fallback population
            var continentNames = new Dictionary<string, string>
            {
                { "AF", "Africa" }, { "AN", "Antarctica" }, { "AS", "Asia" },
                { "EU", "Europe" }, { "NA", "North America" }, { "OC", "Oceania" },
                { "SA", "South America" }
            };

            var continents = new Dictionary<string, Continent>();
            foreach (var line in File.ReadLines(Path.Combine(dir, "countries.txt")))
            {
                if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line)) continue;

                var p = line.Split('\t');
                Country? c;
                string? continentName;
                double area;
                int population;

                if (p.Length < 9) continue;
                if (!countries.TryGetValue(p[1], out c)) continue;          // p[1] = ISO3 code
                if (!continentNames.TryGetValue(p[8], out continentName)) continue;
                if (!double.TryParse(p[6], NumberStyles.Float, inv, out area)) continue;
                int.TryParse(p[7], NumberStyles.Integer, inv, out population);

                // Group countries under distinct Continent entities
                Continent? cont;
                if (!continents.TryGetValue(continentName, out cont))
                {
                    cont = new Continent { Name = continentName };
                    continents[continentName] = cont;
                }

                c.Continent = cont;
                c.Area = area;
                c.Population = population;   // fallback, overwritten below where OWID has more recent data
                cont.TotalCountries++;
            }

            // 4. OWID population: Entity, Code, Year, Population (2023 = latest available)
            // Enrich countries with latest population figures from Our World in Data.
            foreach (var line in File.ReadLines(Path.Combine(dir, "population.csv")).Skip(1))
            {
                var p = line.Split(',');
                Country? c;
                int pop;
                if (p.Length < 4 || p[2] != "2023" || p[1].Length != 3) continue;
                if (!countries.TryGetValue(p[1], out c)) continue;
                if (int.TryParse(p[3], NumberStyles.Integer, inv, out pop) && pop > 0)
                    c.Population = pop;
            }

            // 5. Per capita calculation once population values are finalized
            foreach (var c in countries.Values)
                c.EmissionsPerCapita = c.Population > 0 ? c.Emissions / c.Population : 0;

            // Only persist countries successfully assigned to a continent; continents are cascaded via the entity graph
            var toSave = countries.Values.Where(c => c.Continent != null).ToList();

            context.Set<Country>().AddRange(toSave);
            context.SaveChanges();

        }
    }
}
