namespace sfa.Tl.Marketing.Communication.Application.Interfaces
{
    public interface IJsonConvertor
    {
        T DeserializeObject<T>(string json);
        string SerializeObject(object data);
    }
}
