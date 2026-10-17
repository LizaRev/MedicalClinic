namespace MedicalClinic.Core.Validators;

public static class ValidationPatterns
{
    public const string FullName = @"^[\p{L}]+([ '-’]+\p{L}+)*$";
    public const string PhoneNumber = @"^\d{10}$";
    public const string Specialization = @"^[\p{L}]+([ '-’]+\p{L}+)*$";
}
