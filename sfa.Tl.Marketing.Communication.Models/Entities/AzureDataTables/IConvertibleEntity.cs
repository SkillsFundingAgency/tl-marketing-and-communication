namespace sfa.Tl.Marketing.Communication.Models.Entities.AzureDataTables
{
    public interface IConvertibleEntity<T, R>
    {
        R Convert();
    }
}
