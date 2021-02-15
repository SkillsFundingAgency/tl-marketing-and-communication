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
                        //Id = provider.UkPrn,
                        UkPrn = provider.UkPrn,
                        Name = provider.Name,
                        Locations = provider.Locations.Select(
                            location =>
                                new LocationEntity
                                {
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
                                    ).ToList(),
                                }).ToList()
                    }).ToList();
        }

        public static IList<Provider> ToProviderList(this IEnumerable<ProviderEntity> providerEntities)
        {
            return providerEntities
                .Select(provider =>
                    new Provider
                    {
                        Id = (int)provider.UkPrn,
                        UkPrn = provider.UkPrn,
                        Name = provider.Name,
                        Locations = provider.Locations.Select(
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
                                }).ToList()
                    }).ToList();
        }
    }
}
