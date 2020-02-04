using CsvHelper.Configuration;

namespace sfa.Tl.Marketing.Communication.DataLoad.Read
{
    internal sealed class ProviderReadDataMap : ClassMap<ProviderReadData>
    {
        internal ProviderReadDataMap()
        {
            Map(m => m.ProviderName).Name("Provider Name");
            Map(m => m.VenueName).Name("Venue Name");
            Map(m => m.Postcode).Name("Postcode");
            Map(m => m.Town).Name("Town");

            Map(m => m.IsDigitalProduction).Name("Digital Production, Design and Development")
                .TypeConverterOption.BooleanValues(true, true, "yes")
                .TypeConverterOption.BooleanValues(false, true, "no", "-");

            Map(m => m.IsDigitalBusiness).Name("Digital Business")
                .TypeConverterOption.BooleanValues(true, true, "yes")
                .TypeConverterOption.BooleanValues(false, true, "no", "-");

            Map(m => m.IsDigitalSupport).Name("Digital Support and Services")
                .TypeConverterOption.BooleanValues(true, true, "yes")
                .TypeConverterOption.BooleanValues(false, true, "no", "-");
            
            Map(m => m.IsDesign).Name("Design, Surveying and Planning")
                .TypeConverterOption.BooleanValues(true, true, "yes")
                .TypeConverterOption.BooleanValues(false, true, "no", "-");
            
            Map(m => m.IsBuildingServices).Name("Building Services Engineering")
                .TypeConverterOption.BooleanValues(true, true, "yes")
                .TypeConverterOption.BooleanValues(false, true, "no", "-");
            
            Map(m => m.IsConstruction).Name("Onsite Construction")
                .TypeConverterOption.BooleanValues(true, true, "yes")
                .TypeConverterOption.BooleanValues(false, true, "no", "-");
            
            Map(m => m.IsEducation).Name("Education")
                .TypeConverterOption.BooleanValues(true, true, "yes")
                .TypeConverterOption.BooleanValues(false, true, "no", "-");
            
            Map(m => m.IsHealth).Name("Health")
                .TypeConverterOption.BooleanValues(true, true, "yes")
                .TypeConverterOption.BooleanValues(false, true, "no", "-");
            
            Map(m => m.IsHealthCare).Name("Healthcare Science")
                .TypeConverterOption.BooleanValues(true, true, "yes")
                .TypeConverterOption.BooleanValues(false, true, "no", "-");
            
            Map(m => m.IsScience).Name("Science")
                .TypeConverterOption.BooleanValues(true, true, "yes")
                .TypeConverterOption.BooleanValues(false, true, "no", "-");

            Map(m => m.Website).Name("URL");
            Map(m => m.CourseYear).Name("Course Year");
        }
    }
}
