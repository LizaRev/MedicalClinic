using MedicalClinic.Core.Models;

namespace MedicalClinic.Core.Services;

public class DoctorPriceComparer : IComparer<Doctor>
{
    public int Compare(Doctor? first, Doctor? second)
    {
        if (first == null && second == null)
        {
            return 0;
        }

        if (first == null)
        {
            return -1;
        }

        if (second == null)
        {
            return 1;
        }

        return first.ConsultationPrice.CompareTo(
            second.ConsultationPrice
        );
    }
}
