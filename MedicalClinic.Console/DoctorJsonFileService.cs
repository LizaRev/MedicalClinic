using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using MedicalClinic.Core.DataTransfer;
using MedicalClinic.Core.Models;

namespace MedicalClinic.ConsoleApp;

public class DoctorJsonFileService
{
    private readonly string _filePath;

    public DoctorJsonFileService(string filePath)
    {
        _filePath = filePath;
    }

    public List<Doctor> Load()
    {
        if (!File.Exists(_filePath))
        {
            return new List<Doctor>();
        }

        string json = File.ReadAllText(_filePath);

        using JsonDocument document = JsonDocument.Parse(json);

        if (document.RootElement.ValueKind != JsonValueKind.Array ||
            document.RootElement.GetArrayLength() == 0)
        {
            return new List<Doctor>();
        }

        JsonSerializerOptions options = new JsonSerializerOptions();

        if (document.RootElement[0].TryGetProperty("ПІБ лікаря", out _))
        {
            List<DoctorJson> savedDoctors =
                JsonSerializer.Deserialize<List<DoctorJson>>(json, options)
                ?? new List<DoctorJson>();

            List<Doctor> result = new List<Doctor>();

            foreach (DoctorJson saved in savedDoctors)
            {
                string priceText = saved.ConsultationPrice
                    .Replace("грн", "", StringComparison.OrdinalIgnoreCase)
                    .Trim()
                    .Replace(',', '.');

                if (!decimal.TryParse(
                    priceText,
                    NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture,
                    out decimal price))
                {
                    throw new JsonException("Некоректна ціна лікаря.");
                }

                int months = ParseExperience(saved.Experience);

                result.Add(new Doctor(
                    saved.Id,
                    saved.FullName,
                    saved.Specialization,
                    price,
                    months
                ));
            }

            return result;
        }

        return JsonSerializer.Deserialize<List<Doctor>>(json, options)
            ?? new List<Doctor>();
    }

    public void Save(List<Doctor> doctors)
    {
        string? directory = Path.GetDirectoryName(_filePath);

        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        List<DoctorJson> savedDoctors = new List<DoctorJson>();

        foreach (Doctor doctor in doctors)
        {
            savedDoctors.Add(new DoctorJson
            {
                Id = doctor.Id,
                FullName = doctor.FullName,
                Specialization = doctor.Specialization,
                ConsultationPrice = doctor.ConsultationPrice
                    .ToString("0.##", CultureInfo.InvariantCulture) + " грн",
                Experience = FormatExperience(doctor.ExperienceMonths)
            });
        }

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        string json = JsonSerializer.Serialize(savedDoctors, options);
        File.WriteAllText(_filePath, json);
    }

    private static string FormatExperience(int totalMonths)
    {
        int years = totalMonths / 12;
        int months = totalMonths % 12;

        string yearWord;

        if (years % 10 == 1 && years % 100 != 11)
            yearWord = "рік";
        else if (years % 10 >= 2 && years % 10 <= 4 &&
                 (years % 100 < 12 || years % 100 > 14))
            yearWord = "роки";
        else
            yearWord = "років";

        string result = years + " " + yearWord;

        if (months > 0)
        {
            string monthWord;

            if (months == 1)
                monthWord = "місяць";
            else if (months >= 2 && months <= 4)
                monthWord = "місяці";
            else
                monthWord = "місяців";

            result += " " + months + " " + monthWord;
        }

        return result;
    }

    private static int ParseExperience(string experience)
    {
        string[] parts = experience.Split(
            ' ',
            StringSplitOptions.RemoveEmptyEntries
        );

        int years = 0;
        int months = 0;

        for (int i = 0; i + 1 < parts.Length; i += 2)
        {
            if (!int.TryParse(parts[i], out int number) || number < 0)
            {
                throw new JsonException("Некоректний стаж лікаря.");
            }

            if (parts[i + 1].StartsWith("рок") ||
                parts[i + 1] == "рік")
            {
                years = number;
            }
            else if (parts[i + 1].StartsWith("місяц"))
            {
                months = number;
            }
            else
            {
                throw new JsonException("Некоректний стаж лікаря.");
            }
        }

        return years * 12 + months;
    }
}
