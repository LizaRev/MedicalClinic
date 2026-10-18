namespace MedicalClinic.Core.Exceptions;

public class DoctorScheduleConflictException : Exception
{
    public DoctorScheduleConflictException(string message) : base(message)
    {
    }
}