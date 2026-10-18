using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using MedicalClinic.Core.DataTransfer;
using MedicalClinic.Core.Models;

namespace MedicalClinic.ConsoleApp;

public class PatientJsonFileService
{
    private readonly string _filePath;

    public PatientJsonFileService(string filePath)
    {
        _filePath = filePath; 
    }

    public List<Patient> Load()
    {
        if (!File.Exists(_filePath))
        {
            return new List<Patient>();
        }

        string json = File.ReadAllText(_filePath);

        using JsonDocument document = JsonDocument.Parse(json);

        if (document.RootElement.ValueKind != JsonValueKind.Array ||
            document.RootElement.GetArrayLength() == 0)
        {
            return new List<Patient>();
        }

        if (document.RootElement[0].TryGetProperty(
            "ПІБ пацієнта", out _))
        {
            List<PatientJson> savedPatients =
                JsonSerializer.Deserialize<List<PatientJson>>(json)
                ?? new List<PatientJson>();

            List<Patient> result = new List<Patient>();

            foreach (PatientJson saved in savedPatients)
            {
                if (!DateTime.TryParseExact(
                    saved.DateOfBirth,
                    "dd.MM.yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime dateOfBirth))
                {
                    throw new JsonException(
                        "Некоректна дата народження пацієнта."
                    );
                }

                result.Add(new Patient(
                    saved.Id,
                    saved.FullName,
                    saved.PhoneNumber,
                    dateOfBirth
                ));
            }

            return result;
        }

        return JsonSerializer.Deserialize<List<Patient>>(json)
            ?? new List<Patient>();
    }

    public void Save(List<Patient> patients)
    {
        string? directory = Path.GetDirectoryName(_filePath);

        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        List<PatientJson> savedPatients = new List<PatientJson>();

        foreach (Patient patient in patients)
        {
            savedPatients.Add(new PatientJson
            {
                Id = patient.Id,
                FullName = patient.FullName,
                PhoneNumber = patient.PhoneNumber,
                DateOfBirth = patient.DateOfBirth.ToString(
                    "dd.MM.yyyy",
                    CultureInfo.InvariantCulture
                )
            });
        }

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        string result = JsonSerializer.Serialize(savedPatients, options);
        File.WriteAllText(_filePath, result);
    }
}
