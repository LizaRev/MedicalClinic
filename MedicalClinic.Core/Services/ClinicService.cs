using MedicalClinic.Core.Exceptions;
using MedicalClinic.Core.Interfaces;
using MedicalClinic.Core.Models;
using MedicalClinic.Core.Repositories;

namespace MedicalClinic.Core.Services;

public class ClinicService : IClinicService
{
    private readonly Repository<Doctor> _doctors;
    private readonly Repository<Patient> _patients;
    private readonly Repository<Appointment> _appointments;
    private readonly Repository<MedicalRecord> _medicalRecords;

    public ClinicService()
    {
        _doctors = new Repository<Doctor>();
        _patients = new Repository<Patient>();
        _appointments = new Repository<Appointment>();
        _medicalRecords = new Repository<MedicalRecord>();
    }

    public void AddDoctor(Doctor doctor)
    {
        if (doctor == null)
        {
            throw new ArgumentException("Необхідно вказати лікаря.");
        }

        foreach (Doctor existing in _doctors.GetAll())
        {
            if (existing.Id == doctor.Id)
            {
                throw new ArgumentException("Лікар із таким ID вже існує.");
            }
        }

        _doctors.Add(doctor);
    }

    public void AddPatient(Patient patient)
    {
        if (patient == null)
        {
            throw new InvalidPatientDataException("Дані пацієнта не можуть бути порожніми.");
        }

        foreach (Patient existing in _patients.GetAll())
        {
            if (existing.Id == patient.Id)
            {
                throw new InvalidPatientDataException("Пацієнт із таким ID вже існує.");
            }
        }

        _patients.Add(patient);
    }

    public List<Doctor> GetDoctors()
    {
        return _doctors.GetAll();
    }

    public List<Patient> GetPatients()
    {
        return _patients.GetAll();
    }

    public List<Appointment> GetAppointments()
    {
        return _appointments.GetAll();
    }

    public List<MedicalRecord> GetMedicalRecords()
    {
        return _medicalRecords.GetAll();
    }

    public Appointment BookAppointment(int id, Patient patient, Doctor doctor, DateTime appointmentDate, int durationMinutes = 60)
    {
        if (id <= 0)
        {
            throw new ArgumentException("ID прийому має бути додатним числом.");
        }

        foreach (Appointment existing in _appointments.GetAll())
        {
            if (existing.Id == id)
            {
                throw new ArgumentException("Прийом із таким ID вже існує.");
            }
        }

        if (patient == null)
        {
            throw new InvalidPatientDataException("Необхідно вибрати пацієнта.");
        }

        if (!_patients.GetAll().Contains(patient))
        {
            throw new InvalidPatientDataException("Пацієнт не зареєстрований у клініці.");
        }

        if (doctor == null || !_doctors.GetAll().Contains(doctor))
        {
            throw new ArgumentException("Лікар не зареєстрований у клініці.");
        }

        if (appointmentDate <= DateTime.Now)
        {
            throw new ArgumentException("Дата прийому має бути в майбутньому.");
        }

        if (durationMinutes <= 0)
        {
            throw new ArgumentException("Тривалість прийому має бути більшою за нуль.");
        }

        DateTime newEndDate = appointmentDate.AddMinutes(durationMinutes);

        foreach (Appointment existing in _appointments.GetAll())
        {
            if (existing.Doctor.Id == doctor.Id &&
                existing.Status == AppointmentStatus.Scheduled)
            {
                DateTime existingStart = existing.AppointmentDate;
                DateTime existingEnd = existing.EndDateTime;

                if (appointmentDate < existingEnd && newEndDate > existingStart)
                {
                    throw new DoctorScheduleConflictException(
                        "Лікар уже зайнятий у цей проміжок часу. " +
                        $"Вільний час потрібно обрати після " +
                        $"{existingEnd:dd.MM.yyyy HH:mm} або до " +
                        $"{existingStart:dd.MM.yyyy HH:mm}."
                    );
                }
            }
        }

        Appointment newAppointment = new Appointment(
            id,
            patient,
            doctor,
            appointmentDate,
            doctor.ConsultationPrice,
            durationMinutes
        );

        _appointments.Add(newAppointment);

        return newAppointment;
    }

    public void AddMedicalRecord(MedicalRecord record)
    {
        if (record == null)
        {
            throw new ArgumentException("Медична картка не може бути порожньою.");
        }

        foreach (MedicalRecord existing in _medicalRecords.GetAll())
        {
            if (existing.Id == record.Id)
            {
                throw new ArgumentException("Медичний запис із таким ID вже існує.");
            }
        }

        _medicalRecords.Add(record);
    }

    public List<MedicalRecord> GetPatientHistory(int patientId)
    {
        if (patientId <= 0)
        {
            throw new ArgumentException("ID пацієнта має бути додатним числом.");
        }

        return _medicalRecords.Find(record => record.Patient.Id == patientId);
    }

    public decimal CalculateTotalCost(int patientId)
    {
        if (patientId <= 0)
        {
            throw new ArgumentException("ID пацієнта має бути додатним числом.");
        }

        decimal total = 0;

        foreach (Appointment appointment in _appointments.GetAll())
        {
            if (appointment.Patient.Id == patientId &&
                appointment.Status == AppointmentStatus.Completed)
            {
                total += appointment.ServiceCost;
            }
        }

        return total;
    }

    public void CompleteAppointment(int appointmentId)
    {
        if (appointmentId <= 0)
        {
            throw new ArgumentException("ID прийому має бути додатним числом.");
        }

        foreach (Appointment appointment in _appointments.GetAll())
        {
            if (appointment.Id == appointmentId)
            {
                if (appointment.Status == AppointmentStatus.Completed)
                {
                    throw new InvalidOperationException("Цей прийом уже завершений.");
                }

                if (appointment.Status == AppointmentStatus.Cancelled)
                {
                    throw new InvalidOperationException("Скасований прийом не можна завершити.");
                }

                appointment.Status = AppointmentStatus.Completed;
                return;
            }
        }

        throw new ArgumentException("Прийом не знайдено.");
    }

    public void LoadDoctors(List<Doctor> doctors)
    {
        foreach (Doctor doctor in doctors)
        {
            AddDoctor(doctor);
        }
    }

    public void LoadPatients(List<Patient> patients)
    {
        foreach (Patient patient in patients)
        {
            AddPatient(patient);
        }
    }

    public void LoadAppointments(List<Appointment> appointments)
    {
        foreach (Appointment appointment in appointments)
        {
            _appointments.Add(appointment);
        }
    }

    public void LoadMedicalRecords(List<MedicalRecord> records)
    {
        foreach (MedicalRecord record in records)
        {
            AddMedicalRecord(record);
        }
    }
}
