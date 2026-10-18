using System.Text.Json;
using MedicalClinic.Core.Models;
using MedicalClinic.Core.Services;

namespace MedicalClinic.ConsoleApp;

internal class Program
{
    private static void Main()
    {
        string dataDirectory = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "../../../../Data")
        );

        DoctorJsonFileService doctorFile = new DoctorJsonFileService(
            Path.Combine(dataDirectory, "doctors.json")
        );

        PatientJsonFileService patientFile = new PatientJsonFileService(
            Path.Combine(dataDirectory, "patients.json")
        );

        JsonFileService<Appointment> appointmentFile =
            new JsonFileService<Appointment>(
                Path.Combine(dataDirectory, "appointments.json")
            );

        JsonFileService<MedicalRecord> medicalRecordFile =
            new JsonFileService<MedicalRecord>(
                Path.Combine(dataDirectory, "medicalRecords.json")
            );

        List<Doctor> doctors;
        List<Patient> patients;
        List<Appointment> appointments;
        List<MedicalRecord> medicalRecords;

        try
        {
            doctors = doctorFile.Load();
            patients = patientFile.Load();
            appointments = appointmentFile.Load();
            medicalRecords = medicalRecordFile.Load();
        }
        catch (JsonException ex)
        {
            Console.WriteLine("Помилка читання JSON: " + ex.Message);
            return;
        }
        catch (IOException ex)
        {
            Console.WriteLine("Помилка читання файлів: " + ex.Message);
            return;
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("Немає дозволу на читання файлів.");
            return;
        }

        ClinicService clinicService = new ClinicService();

        try
        {
            clinicService.LoadDoctors(doctors);
            clinicService.LoadPatients(patients);
            clinicService.LoadAppointments(appointments);
            clinicService.LoadMedicalRecords(medicalRecords);
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                "Не вдалося завантажити дані клініки: " + ex.Message
            );
            return;
        }

        void SaveData()
        {
            doctorFile.Save(clinicService.GetDoctors());
            patientFile.Save(clinicService.GetPatients());
            appointmentFile.Save(clinicService.GetAppointments());
            medicalRecordFile.Save(clinicService.GetMedicalRecords());

            Console.WriteLine("Дані успішно збережено.");
        }

        Console.WriteLine("Дані клініки завантажено.");
        Console.WriteLine($"Лікарів: {doctors.Count}");
        Console.WriteLine($"Пацієнтів: {patients.Count}");
        Console.WriteLine($"Прийомів: {appointments.Count}");
        Console.WriteLine($"Медичних записів: {medicalRecords.Count}");
        Console.WriteLine("Папка даних: " + dataDirectory);

        ClinicMenu menu = new ClinicMenu(clinicService, SaveData);

        menu.Run();

        try
        {
            SaveData();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Не вдалося зберегти дані: " + ex.Message);
        }
    }
}