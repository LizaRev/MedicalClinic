using System.Text.Json.Serialization;

namespace MedicalClinic.Core.DataTransfer;

public class PatientJson
{
    [JsonPropertyName("ID")]
    public int Id { get; set; }

    [JsonPropertyName("ПІБ пацієнта")]
    public string FullName { get; set; } = "";

    [JsonPropertyName("Номер телефону")]
    public string PhoneNumber { get; set; } = ""; 

    [JsonPropertyName("Дата народження")]
    public string DateOfBirth { get; set; } = "";
}
