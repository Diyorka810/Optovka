namespace Optovka.Model
{
    public interface IInMemoryCache
    {
        bool Contains(string key);
        
        object Get(string key);
        
        T Get<T>(string key) where T : class;
        
        void Remove(string key);
        
        void Set(string key, object value);
    }
}