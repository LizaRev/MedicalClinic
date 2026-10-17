namespace MedicalClinic.Core.Interfaces;

public interface IRepository<T> where T : class
{
    void Add(T item);
    bool Remove(T item);
    List<T> GetAll();
    List<T> Find(Predicate<T> match);
}