using CityInfo.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API
{
    public class CitiesDataStore
    {
        public static CitiesDataStore Current { get; } = new CitiesDataStore();
        public List<CityDTO> Cities { get; set; }

        public CitiesDataStore()
        {
            Cities = new List<CityDTO>()
            {
                new CityDTO()
                {
                    Id = 1,
                    Name = "New York",
                    Description = "The one with that big park.",
                    PointsOfinterest = new List<PointOfInterestDTO>()
                    {
                        new PointOfInterestDTO()
                        {
                            Id = 1,
                            Name = "Central Park",
                            Description = "The most visited one."
                        },
                        new PointOfInterestDTO()
                        {
                            Id = 2,
                            Name = "Empire State Building",
                            Description = "A very big building."
                        }
                    }
                },
                new CityDTO()
                {
                    Id = 1,
                    Name = "Antwerp",
                    Description = "The one with the cathedral that was never finished.",
                    PointsOfinterest = new List<PointOfInterestDTO>()
                    {
                        new PointOfInterestDTO()
                        {
                            Id = 1,
                            Name = "Central Park",
                            Description = "The most visited one."
                        },
                        new PointOfInterestDTO()
                        {
                            Id = 2,
                            Name = "Empire State Building",
                            Description = "A very big building."
                        }
                    }
                },
                new CityDTO()
                {
                    Id = 1,
                    Name = "Paris",
                    Description = "The one with that big tower.",
                    PointsOfinterest = new List<PointOfInterestDTO>()
                    {
                        new PointOfInterestDTO()
                        {
                            Id = 1,
                            Name = "Central Park",
                            Description = "The most visited one."
                        },
                        new PointOfInterestDTO()
                        {
                            Id = 2,
                            Name = "Empire State Building",
                            Description = "A very big building."
                        }
                    }
                }
            };
        }
    }
}
