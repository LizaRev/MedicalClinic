using MedicalClinic.Core.Models;

namespace MedicalClinic.Core.Interfaces;

public interface IClinicService
{
    void AddDoctor(Doctor doctor);
    void AddPatient(Patient patient);

    List<Doctor> GetDoctors();
    List<Patient> GetPatients();
    List<Appointment> GetAppointments();
    List<MedicalRecord> GetMedicalRecords();

    Appointment BookAppointment(
        int id,
        Patient patient,
        Doctor doctor,
        DateTime appointmentDate,
        int durationMinutes = 60
    );

    void AddMedicalRecord(MedicalRecord record);
    List<MedicalRecord> GetPatientHistory(int patientId);
    decimal CalculateTotalCost(int patientId);
    void CompleteAppointment(int appointmentId);

    void LoadDoctors(List<Doctor> doctors);
    void LoadPatients(List<Patient> patients);
    void LoadAppointments(List<Appointment> appointments);
    void LoadMedicalRecords(List<MedicalRecord> records);
}
