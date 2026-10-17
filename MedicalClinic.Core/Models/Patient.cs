using System;
using System.Text.RegularExpressions;
using MedicalClinic.Core.Exceptions;
using MedicalClinic.Core.Validators;

namespace MedicalClinic.Core.Models;

public class Patient : Entity
{
    private string _fullName = "";
    private string _phoneNumber = "";
    private DateTime _dateOfBirth;

    public string FullName
    {
        get { return _fullName; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidPatientDataException(
                    "ПІБ пацієнта не може бути порожнім."
                );
            }

            string name = value.Trim();

            if (!Regex.IsMatch(
                name,
                ValidationPatterns.FullName))
            {
                throw new InvalidPatientDataException(
                    "ПІБ має містити лише літери, пробіли, дефіс або апостроф."
                );
            }

            _fullName = name;
        }
    }

    public string PhoneNumber
    {
        get { return _phoneNumber; }
        set
        {
            if (string.IsNullOrWhiteSpace(value) ||
                !Regex.IsMatch(
                    value.Trim(),
                    ValidationPatterns.PhoneNumber))
            {
                throw new InvalidPatientDataException(
                    "Номер телефону має містити рівно 10 цифр, наприклад 0956476578."
                );
            }

            _phoneNumber = value.Trim();
        }
    }

    public DateTime DateOfBirth
    {
        get { return _dateOfBirth; }
        set
        {
            if (value.Date >= DateTime.Today)
            {
                throw new ArgumentException(
                    "Дата народження має бути в минулому."
                );
            }

            _dateOfBirth = value;
        }
    }

    public Patient(
        int id,
        string fullName,
        string phoneNumber,
        DateTime dateOfBirth
    ) : base(id)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
        DateOfBirth = dateOfBirth;
    }
}
