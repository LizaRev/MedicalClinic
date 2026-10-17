using System.Text.Json.Serialization;

namespace MedicalClinic.Core.DataTransfer;

public class DoctorJson
{
    [JsonPropertyName("ID")]
    public int Id { get; set; }

    [JsonPropertyName("ПІБ лікаря")]
    public string FullName { get; set; } = "";

    [JsonPropertyName("Спеціалізація")]
    public string Specialization { get; set; } = "";

    [JsonPropertyName("Ціна консультації")]
    public string ConsultationPrice { get; set; } = "";

    [JsonPropertyName("Стаж роботи")]
    public string Experience { get; set; } = "";
}
