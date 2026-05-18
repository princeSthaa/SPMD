# Smart Prescription and Medication Dispensing System (SPMD)

SPMD is a production-grade, patient-safety-focused medical platform built with ASP.NET Core 10.0. It addresses critical real-world challenges in medication management through intelligent automation and transparent inventory tracking.

## 🌟 Key Real-World Problem Solvers

### 1. Clinical Safety Engine (Error Prevention)
*   **Allergy Validation:** Automatically blocks or flags prescriptions if a patient is allergic to a medication's substance or generic category.
*   **Duplicate Therapy Check:** Detects if a patient is already taking a medication with the same generic name to prevent accidental overdose.
*   **Override Logging:** Requires a written clinical justification if a doctor bypasses a safety warning, ensuring accountability.

### 2. Intelligent Inventory & Supply Chain
*   **FEFO Dispensing (First Expired, First Out):** The system automatically suggests stock from the oldest non-expired batches first.
*   **Expiry Safeguard:** Hard-blocks the dispensing of any medication from a batch that has passed its expiry date.
*   **Low Stock Alerts:** Proactive notifications for pharmacists when inventory hits a critical threshold.

### 3. Customer Accessibility Portal
*   **Live Stock Search:** Public-facing portal allowing patients to check medicine availability (In Stock, Limited, Out of Stock) online, saving unnecessary travel.
*   **Patient History:** Secure dashboard for patients to view their own active prescriptions and clinical history.

### 4. Forensic Accountability
*   **Immutable Audit Logs:** Every clinical and administrative action (Who, What, When, Where) is permanently recorded in a searchable audit trail.

---

## 🏗️ Architecture: "Pure Generic" Abstraction

The project follows a highly scalable, layered architecture designed to minimize code duplication:

*   **Generic Repository Pattern:** A single `Repository<T>` handles all EF Core operations using `where T : class` constraints, eliminating the need for boilerplate per-entity repositories.
*   **Generic Service Layer:** The `BaseService<T>` provides standard CRUD logic that all domain services (User, Medicine, Patient, etc.) inherit and extend.
*   **Domain-Driven Design:** Clear separation between clinical entities (Doctors, Patients) and supply chain entities (Batches, Medicines).

---

## 🔐 Security & Roles

The system employs **Role-Based Access Control (RBAC)** via Cookie and JWT authentication:

| Role | Permissions |
| :--- | :--- |
| **Doctor** | Create Prescriptions, Run Safety Validations, View Patient History. |
| **Pharmacist** | Process Dispensing, Manage Inventory, View Stock Alerts. |
| **Patient** | View Own Prescriptions, Search Medicine Availability. |
| **Admin** | System Audit Log Review, User Management. |

---

## 🛠️ Technology Stack

*   **Framework:** ASP.NET Core 10.0 (Razor Pages)
*   **ORM:** Entity Framework Core 10.0
*   **Database:** SQL Server
*   **Security:** BCrypt.Net (Password Hashing), JWT Bearer & Cookie Authentication
*   **UI:** Bootstrap 5, Razor Syntax

---

## 🚀 Getting Started

1.  **Database Setup:**
    *   Update `ConnectionStrings:DefaultConnection` in `appsettings.json`.
    *   Run `dotnet ef database update`.
2.  **Run Application:**
    *   `dotnet run`
3.  **Default Configuration:**
    *   Register as a `Doctor` to start issuing prescriptions.
    *   Register as a `Pharmacist` to add inventory and dispense meds.
