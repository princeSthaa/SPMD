using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using SPMD.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SPMD.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            context.Database.Migrate();

            // 1. Seed Roles
            if (!context.Roles.Any())
            {
                var roles = Enum.GetValues<RoleName>().Select(r => new Role { Name = r }).ToList();
                context.Roles.AddRange(roles);
                context.SaveChanges();
            }

            // 2. Seed Medicines
            if (!context.Medicines.Any())
            {
                var medicines = new List<Medicine>
                {
                    new Medicine { MedicineName = "Amoxicillin 500mg", Brand = "GSK", GenericName = "Penicillin", Strength = "500mg", DosageForm = "Capsule", StockThreshold = 50, PricePerUnit = 2.50m },
                    new Medicine { MedicineName = "Paracetamol 500mg", Brand = "Panadol", GenericName = "Paracetamol", Strength = "500mg", DosageForm = "Tablet", StockThreshold = 100, PricePerUnit = 0.50m },
                    new Medicine { MedicineName = "Ibuprofen 400mg", Brand = "Advil", GenericName = "Ibuprofen", Strength = "400mg", DosageForm = "Tablet", StockThreshold = 30, PricePerUnit = 1.00m },
                    new Medicine { MedicineName = "Cetirizine 10mg", Brand = "Zyrtec", GenericName = "Cetirizine", Strength = "10mg", DosageForm = "Tablet", StockThreshold = 20, PricePerUnit = 0.75m },
                    new Medicine { MedicineName = "Metformin 500mg", Brand = "Glucophage", GenericName = "Metformin", Strength = "500mg", DosageForm = "Tablet", StockThreshold = 40, PricePerUnit = 1.20m },
                    new Medicine { MedicineName = "Warfarin 5mg", Brand = "Coumadin", GenericName = "Warfarin", Strength = "5mg", DosageForm = "Tablet", StockThreshold = 10, PricePerUnit = 3.00m },
                    new Medicine { MedicineName = "Aspirin 81mg", Brand = "Bayer", GenericName = "Aspirin", Strength = "81mg", DosageForm = "Tablet", StockThreshold = 50, PricePerUnit = 0.25m }
                };
                context.Medicines.AddRange(medicines);
                context.SaveChanges();

                // 3. Seed Batches for Medicines
                var amox_med = medicines.First(m => m.MedicineName.Contains("Amoxicillin"));
                var para_med = medicines.First(m => m.MedicineName.Contains("Paracetamol"));

                context.MedicineBatches.AddRange(new List<MedicineBatch>
                {
                    // Expired batch
                    new MedicineBatch { MedicineId = amox_med.MedicineId, BatchNumber = "AMX-EXP-01", ExpiryDate = DateTime.UtcNow.AddMonths(-1), QuantityAvailable = 100 },
                    // Soon to expire batch (FEFO test)
                    new MedicineBatch { MedicineId = amox_med.MedicineId, BatchNumber = "AMX-FEFO-01", ExpiryDate = DateTime.UtcNow.AddMonths(1), QuantityAvailable = 50 },
                    // Long term batch
                    new MedicineBatch { MedicineId = amox_med.MedicineId, BatchNumber = "AMX-OK-01", ExpiryDate = DateTime.UtcNow.AddYears(2), QuantityAvailable = 500 },
                    
                    // Paracetamol batches
                    new MedicineBatch { MedicineId = para_med.MedicineId, BatchNumber = "PARA-01", ExpiryDate = DateTime.UtcNow.AddMonths(6), QuantityAvailable = 200 },
                    new MedicineBatch { MedicineId = para_med.MedicineId, BatchNumber = "PARA-02", ExpiryDate = DateTime.UtcNow.AddYears(1), QuantityAvailable = 300 }
                });
                context.SaveChanges();
            }

            // 4. Seed Users (Doctor, Pharmacist, Patient)
            var superAdminRole = context.Roles.FirstOrDefault(r => r.Name == RoleName.SuperAdmin);
            if (superAdminRole == null)
            {
                // Fallback: If roles aren't seeded for some reason, re-seed them
                var roles = Enum.GetValues<RoleName>().Select(r => new Role { Name = r }).ToList();
                context.Roles.AddRange(roles);
                context.SaveChanges();
                superAdminRole = context.Roles.First(r => r.Name == RoleName.SuperAdmin);
            }
            
            // Ensure 'admin' user exists with 'admin' password
            var adminUser = context.Users.Include(u => u.Role).FirstOrDefault(u => u.Username == "admin" || (u.Role != null && u.Role.Name == RoleName.SuperAdmin));
            if (adminUser == null)
            {
                adminUser = new User
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"),
                    Email = "admin@spmd.com",
                    RoleId = superAdminRole.RoleId,
                    RequiresPasswordChange = true
                };
                context.Users.Add(adminUser);
            }
            else
            {
                // Update existing admin to the requested credentials
                adminUser.Username = "admin";
                adminUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin");
                adminUser.RoleId = superAdminRole.RoleId; // Ensure it has correct role
                context.Users.Update(adminUser);
            }
            context.SaveChanges();

            if (!context.Users.Any(u => u.Username != "admin"))
            {
                var doctorRole = context.Roles.FirstOrDefault(r => r.Name == RoleName.Doctor);
                var pharmacistRole = context.Roles.FirstOrDefault(r => r.Name == RoleName.Pharmacist);
                var patientRole = context.Roles.FirstOrDefault(r => r.Name == RoleName.Patient);

                if (doctorRole != null && pharmacistRole != null && patientRole != null)
                {
                    var doctorUser = new User 
                    { 
                        Username = "dr_smith", 
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"), 
                        Email = "smith@spmd.com", 
                        RoleId = doctorRole.RoleId 
                    };

                    var pharmacistUser = new User 
                    { 
                        Username = "ph_jones", 
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"), 
                        Email = "jones@spmd.com", 
                        RoleId = pharmacistRole.RoleId 
                    };

                    var patientUser = new User 
                    { 
                        Username = "jdoe", 
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"), 
                        Email = "jdoe@gmail.com", 
                        RoleId = patientRole.RoleId 
                    };

                    context.Users.AddRange(doctorUser, pharmacistUser, patientUser);
                    context.SaveChanges();

                    // 5. Create Profiles
                    context.Doctors.Add(new Doctor { UserId = doctorUser.UserId, FirstName = "John", LastName = "Smith", Speciality = "General Physician" });
                    context.Pharmacists.Add(new Pharmacist { UserId = pharmacistUser.UserId, FirstName = "Sarah", LastName = "Jones" });
                    
                    var patient = new Patient 
                    { 
                        UserId = patientUser.UserId, 
                        FirstName = "John", 
                        LastName = "Doe", 
                        Gender = "Male", 
                        Phone = "123-456-7890", 
                        Email = "jdoe@gmail.com", 
                        Address = "123 Medical St" 
                    };
                    context.Patients.Add(patient);
                    context.SaveChanges();

                    // 6. Seed Allergies for John Doe
                    var amox = context.Medicines.First(m => m.MedicineName.Contains("Amoxicillin"));
                    context.AllergyRecords.Add(new AllergyRecord 
                    { 
                        PatientId = patient.PatientId, 
                        Substance = "Penicillin", 
                        Reaction = "Anaphylaxis", 
                        Severity = SeverityLevel.Severe,
                        MedicineId = amox.MedicineId
                    });

                    // Add another patient with mild allergy
                    var jane = new Patient 
                    { 
                        FirstName = "Jane", 
                        LastName = "Smith", 
                        Gender = "Female", 
                        Phone = "987-654-3210", 
                        Email = "jane@example.com", 
                        Address = "456 Health Ave" 
                    };
                    context.Patients.Add(jane);
                    context.SaveChanges();

                    context.AllergyRecords.Add(new AllergyRecord 
                    { 
                        PatientId = jane.PatientId, 
                        Substance = "NSAIDs", 
                        Reaction = "Mild Rash", 
                        Severity = SeverityLevel.Low 
                    });
                    context.SaveChanges();
                }
            }

            // 7. Seed Drug Interactions
            if (!context.DrugInteractions.Any())
            {
                context.DrugInteractions.AddRange(new List<DrugInteraction>
                {
                    new DrugInteraction 
                    { 
                        GenericNameA = "Warfarin", 
                        GenericNameB = "Aspirin", 
                        Description = "Increased risk of bleeding due to additive antiplatelet effects and anticoagulant activity.",
                        Severity = SeverityLevel.Severe
                    },
                    new DrugInteraction 
                    { 
                        GenericNameA = "Lisinopril", 
                        GenericNameB = "Spironolactone", 
                        Description = "Increased risk of hyperkalemia (high potassium levels).",
                        Severity = SeverityLevel.Moderate
                    },
                    new DrugInteraction
                    {
                        GenericNameA = "Ibuprofen",
                        GenericNameB = "Warfarin",
                        Description = "NSAIDs may enhance the anticoagulant effect of Warfarin.",
                        Severity = SeverityLevel.Severe
                    }
                });
                context.SaveChanges();
            }
        }
    }
}
