using MedicalClinic.Core.Interfaces;

namespace MedicalClinic.Core.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly List<T> _items = new List<T>();

    public void Add(T item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        _items.Add(item);
    }

    public bool Remove(T item)
    {
        return _items.Remove(item);
    }

    public List<T> GetAll()
    {
        return new List<T>(_items);
    }

    public List<T> Find(Predicate<T> match)
    {
        return _items.FindAll(match);
    }
}