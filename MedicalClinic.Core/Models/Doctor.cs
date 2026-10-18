using System;
using System.Text.RegularExpressions;
using MedicalClinic.Core.Validators;

namespace MedicalClinic.Core.Models;

public class Doctor : Entity, IComparable<Doctor>
{
    private string _fullName = "";
    private string _specialization = "";
    private decimal _consultationPrice;

    public string FullName
    {
        get { return _fullName; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("ПІБ лікаря не може бути порожнім.");
            }

            string name = value.Trim();

            if (!Regex.IsMatch(name, ValidationPatterns.FullName))
            {
                throw new ArgumentException(
                    "ПІБ лікаря має містити лише літери, пробіли, дефіс або апостроф."
                );
            }

            _fullName = name;
        }
    }

    public string Specialization
    {
        get { return _specialization; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Спеціалізація не може бути порожньою.");
            }

            string specialization = value.Trim();

            if (!Regex.IsMatch(specialization, ValidationPatterns.Specialization))
            {
                throw new ArgumentException(
                    "Назва спеціалізації має містити лише літери, пробіли, дефіс або апостроф."
                );
            }

            _specialization = specialization;
        }
    }

    public decimal ConsultationPrice
    {
        get { return _consultationPrice; }
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("Вартість консультації не може бути від'ємною.");
            }

            _consultationPrice = value;
        }
    }

    public int ExperienceMonths { get; private set; }

    public Doctor(
        int id,
        string fullName,
        string specialization,
        decimal consultationPrice,
        int experienceMonths = 0
    ) : base(id)
    {
        FullName = fullName;
        Specialization = specialization;
        ConsultationPrice = consultationPrice;

        if (experienceMonths < 0 || experienceMonths > 840)
        {
            throw new ArgumentException(
                "Стаж лікаря має бути від 0 до 840 місяців (70 років)."
            );
        }

        ExperienceMonths = experienceMonths;
    }

    public int CompareTo(Doctor? other)
    {
        if (other == null)
        {
            return 1;
        }

        return ConsultationPrice.CompareTo(other.ConsultationPrice);
    }
}
