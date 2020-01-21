using CsvHelper.Configuration;

namespace sfa.Tl.Marketing.Communication.DataLoad.Read
{
    internal sealed class ProviderReadDataMap : ClassMap<ProviderReadData>
    {
        internal ProviderReadDataMap()
        {
            Map(m => m.Name).Name("Name");
            Map(m => m.Address).Name("Address");
            Map(m => m.Postcode).Name("Post Code");
            Map(m => m.IsConstruction).Name("Construction course ")
                .TypeConverterOption.BooleanValues(true, true, "yes")
                .TypeConverterOption.BooleanValues(false, true, "no", "-");
            Map(m => m.IsDigital).Name("Digital course ")
                .TypeConverterOption.BooleanValues(true, true, "yes")
                .TypeConverterOption.BooleanValues(false, true, "no", "-");
            Map(m => m.IsEducation).Name("Education course ")
                .TypeConverterOption.BooleanValues(true, true, "yes")
                .TypeConverterOption.BooleanValues(false, true, "no", "-");
            Map(m => m.Website).Name("Website (T level specific if available) ");
        }
    }
}