namespace MedicalClinic.Core.Exceptions;

public class InvalidPatientDataException : Exception
{
    public InvalidPatientDataException(string message)
        : base(message)
    {
    }
}