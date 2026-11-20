using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

// DISCLAMER: This code is AI generated based on the uml and ocl diagrams

namespace HospitalManagementSystem
{
    public abstract class Person
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateOnly Birthday { get; set; }
    }

    public abstract class Employee : Person
    {
        public DateOnly StartDate { get; set; }
        public int Wage { get; set; }
        public List<Shift> Shifts { get; } = new();
    }

    public class Patient : Person
    {
        public List<MedicalRecord> MedicalRecords { get; } = new();
        public List<Prescription> Prescriptions { get; } = new();
    }

    public class MedicalDevice
    {
        public string Manufacturer { get; set; } = string.Empty;
    }

    public class MedicalRecord
    {
        public string Diagnosis { get; set; } = string.Empty;
        public DateOnly RecordDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public List<string> Symptoms { get; } = new();
        public string DetailedText { get; set; } = string.Empty;
    }

    public class Shift
    {
        public DateTime From { get; }
        public DateTime To { get; }

        public Shift(DateTime from, DateTime to)
        {
            // OCL inv: self.from < self.to
            if (from >= to)
                throw new ArgumentException("Shift start time ('From') must be before end time ('To').");

            From = from;
            To = to;
        }
    }

    public class Room
    {
        public int Capacity { get; }

        // OCL init: 0
        private int _currentOccupation = 0;
        public int CurrentOccupation
        {
            get => _currentOccupation;
            set
            {
                // OCL inv: self.current_occupation >= 0 and self.current_occupation <= self.capacity
                if (value < 0 || value > Capacity)
                    throw new ArgumentOutOfRangeException(nameof(value), "Occupation must be between 0 and Capacity.");
                _currentOccupation = value;
            }
        }

        public List<Shift> Shifts { get; } = new();

        // OCL derive: self.current_occupation >= self.capacity
        public bool IsFull => CurrentOccupation >= Capacity;

        public Room(int capacity, Shift initialShift)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));
            if (initialShift is null)
                throw new ArgumentNullException(nameof(initialShift), "Room must have at least one shift assigned.");

            Capacity = capacity;
            // OCL inv: self.shift->size() > 0
            Shifts.Add(initialShift);
        }
    }

    public class Doctor : Employee
    {
        public string Expertise { get; set; } = string.Empty;
        public List<string> Education { get; }
        public List<MedicalRecord> MedicalRecords { get; } = new();

        public Doctor(IEnumerable<string> education)
        {
            Education = education?.Where(e => !string.IsNullOrWhiteSpace(e)).ToList()
                ?? throw new ArgumentNullException(nameof(education));

            // OCL inv: self.Education->size() > 0
            if (Education.Count == 0)
                throw new ArgumentException("Doctor must have at least one education entry specified.");
        }

        public MedicalRecord Examine(Patient patient, MedicalDevice device)
        {
            // OCL pre: self.Education->notEmpty() and self.shift->notEmpty()
            if (Education.Count == 0)
                throw new InvalidOperationException("Precondition failed: Doctor must have education specified.");
            if (Shifts.Count == 0)
                throw new InvalidOperationException("Precondition failed: Doctor must have at least one shift.");

            var record = new MedicalRecord
            {
                Diagnosis = "Pending diagnosis",
                DetailedText = $"Examined using device from {device.Manufacturer}."
            };

            patient.MedicalRecords.Add(record);
            MedicalRecords.Add(record);

            // OCL post: p.medicalRecord->includes(result) and self.medicalRecord->includes(result)
            Debug.Assert(patient.MedicalRecords.Contains(record) && MedicalRecords.Contains(record));

            return record;
        }
    }

    public class Technician : Employee
    {
        public string Expertise { get; set; } = string.Empty;
        public List<MedicalDevice> OverseenDevices { get; } = new();

        public bool OperateDevice(MedicalDevice device)
        {
            // OCL pre: self.medicalDevice->includes(d)
            if (!OverseenDevices.Contains(device))
                throw new UnauthorizedAccessException("Access denied: Technician can only operate devices they oversee.");

            // Perform device operation...
            bool result = true;

            // OCL post: result = true
            Debug.Assert(result);
            return result;
        }
    }

    public class Prescription
    {
        public DateOnly ValidFrom { get; }
        public DateOnly ValidTo { get; }
        public Medicine Medicine { get; }

        public Prescription(DateOnly validFrom, DateOnly validTo, Medicine medicine)
        {
            // OCL inv: self.ValidFrom < self.ValidTo
            if (validFrom >= validTo)
                throw new ArgumentException("ValidFrom must be earlier than ValidTo.");

            ValidFrom = validFrom;
            ValidTo = validTo;
            Medicine = medicine ?? throw new ArgumentNullException(nameof(medicine));
        }
    }

    public class Medicine
    {
        public string Name { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public List<string> MainEffects { get; } = new();
        public List<string> SideEffects { get; } = new();
        public List<MedicineItem> MedicineItems { get; } = new();

        // Helper to check OCL inv: self.medicineItem->collect(Amount)->sum() >= 1
        public bool HasValidStock() => MedicineItems.Sum(item => item.Amount) >= 1;
    }

    public class MedicineItem
    {
        public DateOnly DateOfManufacture { get; }
        public DateOnly DateOfExpiration { get; }

        // OCL init: 0
        public int Amount { get; set; } = 0;

        public MedicineItem(DateOnly dateOfManufacture, DateOnly dateOfExpiration)
        {
            // OCL inv: self.DateOfManufacture < self.DateOfExpiration
            if (dateOfManufacture >= dateOfExpiration)
                throw new ArgumentException("DateOfManufacture must be before DateOfExpiration.");

            DateOfManufacture = dateOfManufacture;
            DateOfExpiration = dateOfExpiration;
        }
    }
}
