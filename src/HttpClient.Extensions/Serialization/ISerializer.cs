namespace HttpClient.Extensions.Serialization
{
    public interface ISerializer
    {
        string Serialize(object obj);
    }
}