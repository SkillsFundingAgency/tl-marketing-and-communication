using System.Collections.Generic;
using System.Linq;
using sfa.Tl.Marketing.Communication.Models.Dto;
using sfa.Tl.Marketing.Communication.Models.Entities;

namespace sfa.Tl.Marketing.Communication.Models.Extensions
{
    public static class EntityExtensions
    {
        public static IList<QualificationEntity> ToQualificationEntityList(
            this IEnumerable<Qualification> qualifications)
        {
            return qualifications
                .Select(qualification =>
                    new QualificationEntity
                    {
                        PartitionKey = "qualifications",
                        RowKey = qualification.Id.ToString(),
                        Id = qualification.Id,
                        Route = qualification.Route,
                        Name = qualification.Name
                    }).ToList();
        }

        public static IList<Qualification> ToQualificationList(this IEnumerable<QualificationEntity> qualificationEntities)
        {
            return qualificationEntities
                .Select(q =>
                    new Qualification
                    {
                        Id = q.Id,
                        Route = q.Route,
                        Name = q.Name
                    }).ToList();
        }

        public static IList<ProviderEntity> ToProviderEntityList(this IEnumerable<Provider> providers)
        {
            return providers
                .Select(provider =>
                    new ProviderEntity
                    {
                        PartitionKey = "providers",
                        RowKey = provider.UkPrn.ToString(),
                        UkPrn = provider.UkPrn,
                        Name = provider.Name
                    }).ToList();
        }

        public static IList<Provider> ToProviderList(this IEnumerable<ProviderEntity> providerEntities)
        {
            return providerEntities
                .Select(provider =>
                    new Provider
                    {
                        UkPrn = provider.UkPrn,
                        Name = provider.Name,
                        Locations = new List<Location>()
                    }).ToList();
        }

        public static IList<LocationEntity> ToLocationEntityList(this IEnumerable<Location> locations, string partitionKey)
        {
            return locations?
                .Select(
                    location =>
                        new LocationEntity
                        {
                            PartitionKey = partitionKey,
                            RowKey = location.Postcode,
                            Name = location.Name,
                            Postcode = location.Postcode,
                            Latitude = location.Latitude,
                            Longitude = location.Longitude,
                            Town = location.Town,
                            Website = location.Website,
                            DeliveryYears = location.DeliveryYears.Select(
                                deliveryYear =>
                                    new DeliveryYearEntity
                                    {
                                        Year = deliveryYear.Year,
                                        Qualifications = deliveryYear.Qualifications.ToList()
                                    }
                            ).ToList()
                        }).ToList() ?? new List<LocationEntity>();
        }

        public static IList<Location> ToLocationList(this IEnumerable<LocationEntity> locationEntities)
        {
            return locationEntities?
                .Select(
                    location =>
                        new Location
                        {
                            Name = location.Name,
                            Postcode = location.Postcode,
                            Latitude = location.Latitude,
                            Longitude = location.Longitude,
                            Town = location.Town,
                            Website = location.Website,
                            DeliveryYears = location.DeliveryYears.Select(
                                deliveryYear =>
                                    new DeliveryYearDto
                                    {
                                        Year = deliveryYear.Year,
                                        Qualifications = deliveryYear.Qualifications.ToList()
                                    }).ToList()
                        }).ToList() ?? new List<Location>();
        }
    }
}
