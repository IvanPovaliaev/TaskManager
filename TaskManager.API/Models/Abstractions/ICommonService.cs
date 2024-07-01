namespace TaskManager.API.Models.Abstractions
{
    public interface ICommonService<T>
    {
        bool Create(T model);
        bool Update(int id, T model);
        bool Remove(int id);
    }
}
