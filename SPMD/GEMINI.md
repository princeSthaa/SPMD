# SPMD: Smart Prescription And Medication Dispensing System

This is the primary project in the solution, responsible for the core logic of prescription management and medication dispensing.

## Domain Model

- **Users & Roles**: Managed through `User` and `Role` entities. Roles: SuperAdmin, Admin, Pharmacist, Doctor, Patient.
- **Core Entities**:
  - `Doctor`: Manages clinical prescriptions and patient records.
  - `Patient`: Recipients of medical care with digital health profiles.
  - `Pharmacist`: Manages inventory and validates dispensing via FEFO.
  - `Prescription`: Clinical orders containing items, dosage, and instructions.
  - `DispensingRecord`: Audit-compliant tracking of medication fulfillment.
  - `Medicine`: Centralized medication catalog with batch-level inventory.
  - `AllergyRecord`: Critical safety data mapping patients to substances/medicines.
  - `DrugInteraction`: Knowledge base for adverse drug-drug interactions (DDI).

## Architecture

- **UI**: Modernized Razor Pages with responsive design and interactive components.
- **Data Access**: Entity Framework Core with Repository Pattern for abstraction.
- **Services**: Heavyweight business logic encapsulation (e.g., `PrescriptionService`, `DispensingService`).
- **Real-Time**: SignalR Hub for instant system-wide notifications.
- **Document Engine**: QuestPDF for clinical-grade document generation.

## ✅ Completed Milestones (Implementation Phase)

### 🏥 Advanced Clinical Safety Engine
- **Full DDI Checker**: Real-time validation of new prescriptions against a patient's entire active medication history and within the new prescription itself.
- **Allergy Guard**: Integrated blocking system for severe allergies and warning system for moderate sensitivities.
- **FEFO Inventory Logic**: "First-Expired-First-Out" batch selection ensures medication safety and minimizes waste.

### 🛡️ Security & Identity Hardening
- **Multi-Role Access Control**: Granular permissions for Doctors, Pharmacists, and Admins.
- **SuperAdmin Protection**: Guardrails preventing self-deletion/deactivation of SuperAdmin accounts.
- **Forced Credential Setup**: Mandatory username/password change upon first login for newly created clinical accounts.
- **Account Deactivation**: Robust system-wide enforcement of account status during authentication.
- **Forgot/Reset Password**: Complete recovery workflow with email simulation for development and SMTP integration for production.

### 📄 Clinical Documentation & UX
- **Digital Prescriptions**: Generation of professional, high-fidelity PDF prescription records using QuestPDF.
- **Live Notifications**: SignalR-powered toast notifications for low stock alerts and new prescription arrivals.
- **Instant Search**: Optimized AJAX lookups for medication inventory and patient records.

## Database Conventions

- **Relationships**: `DeleteBehavior.Restrict` is used to maintain integrity of clinical history.
- **Migrations**: Always run `dotnet build` before adding migrations to ensure type safety.

## Roadmap & Pending Tasks
- [x] Implement Comprehensive Drug-Drug Interaction (DDI) checks.
- [x] Add PDF Generation for prescriptions and patient records.
- [x] Integrate Real-time Notifications for low stock and pending prescriptions.
- [x] Implement Forgot/Reset Password workflow.
- [x] Integrate pricing details (Price Per Unit) into Live Medicine Search UI.
- [x] Integrate pricing and total cost calculation into the Dispensing Audit Trail.
- [ ] Implement Advanced Analytics (Inventory turnover & Clinical trends).
- [ ] Implement Patient-side Dashboard for prescription tracking.

## 📝 Detailed Session Log (Changelog)

### 🏥 Clinical Intelligence
- **Added**: `DrugInteraction` entity and database migration for adverse drug-drug interactions.
- **Added**: Comprehensive DDI validation logic in `PrescriptionService` (cross-references active medications).
- **Added**: Realistic clinical seed data for interactions (Warfarin, Aspirin, Ibuprofen).
- **Added**: `PricePerUnit` field to `Medicine` and `UnitCost` to `MedicineBatch` for financial tracking and auditing.

### 📄 Documentation & UX
- **Added**: `QuestPDF` integration and `PdfService` for high-fidelity digital prescriptions.
- **Added**: Prescription download endpoint and "View PDF" UI integration.
- **Added**: SignalR `NotificationHub` and real-time Toast system in the master layout.
- **Added**: Automated alerts for **New Prescriptions** and **Critical Low Stock**.

### 🛡️ Security & Identity
- **Added**: Complete **Forgot & Reset Password** workflow with secure token simulation.
- **Added**: Resilient `EmailService` with automatic "Simulation Mode" fallback on SMTP failure.
- **Added**: Guardrail logic to protect SuperAdmin accounts from deletion/deactivation.
- **Fixed**: Deactivated users can no longer bypass authentication via cached sessions.
- **Fixed**: Role-mismatch errors for SuperAdmin during clinical workspace login.
- **Updated**: Expanded administrative access for the `Pharmacist` role to include audit logs.

### 🛠️ Maintenance & Optimization
- **Fixed**: Resolved all build warnings (NU1510) and redundant package references.
- **Fixed**: Corrected `SignOutAsync` compilation error in `SetupCredentials`.
- **Added**: Template for Gmail SMTP configuration in `appsettings.json`.
- **Improved**: Build reliability by implementing automated process termination for file-lock issues.
