using MedicalClinic.Core.Interfaces;

namespace MedicalClinic.Core.Models;

public abstract class Entity : IIdentifiable
{
    public int Id { get; protected set; }

    protected Entity(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("ID має бути додатним числом.");
        }

        Id = id;
    }
}
