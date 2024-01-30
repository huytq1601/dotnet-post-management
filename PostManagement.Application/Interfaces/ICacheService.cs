namespace PostManagement.Application.Interfaces
{
    public interface ICacheService
    {
        void SetWithKeyTracking(string key, object value);
        void RemoveByPrefix(string prefix);
        void Remove(string key);
    }
}
