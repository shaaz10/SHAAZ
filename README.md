# EventGuard Insurance System - Capstone Project

## Overview
EventGuard is a comprehensive full-stack insurance management system designed for handling event insurance lifecycles—from initial customer requests to policy generation, document validation, and claim processing.

The system features:
- **Backend**: ASP.NET Core Web API with Entity Framework Core and SQL Server.
- **Frontend**: Modern Angular 21 with TailWind CSS or SCSS styling options.
- **AI Microservice**: Python-based microservice (Flask) for risk assessment and fraud detection.
- **Documentation**: Extensive diagrams and documentation in the `Docs` folder.

---

## 📂 Project Structure

- `backend/`: Contains the .NET Core API and the Python AI microservice.
- `frontend/`: Contains two versions of the Angular customer/admin portal.
- `Docs/`: Contains project artifacts, ER diagrams, sequence diagrams, and SRS.

---

## 🚀 How to Run the System

To run the full system, you need to start the three main components in the following order:

### 1. Backend (.NET Web API)
The central hub for data and business logic.
1. Navigate to: `EventInsuranceSystem/backend/EventInsurance.API`
2. Ensure you have the .NET 8.0 SDK installed.
3. Database: The system uses `(localdb)\MSSQLLocalDB`. Connection string is located in `appsettings.json`.
4. Run the application:
   ```bash
   dotnet restore
   dotnet run
   ```
5. The API will typically be available at `http://localhost:5266` or `http://localhost:5147` (check your console output).

### 2. AI Microservice (Python)
Used for document confidence analysis and fraud scoring.
1. Navigate to: `EventInsuranceSystem/backend/EventInsurance.FastAPI`
2. Create and activate a virtual environment:
   ```bash
   python -m venv venv
   .\venv\Scripts\activate  # Windows
   ```
3. Install dependencies:
   ```bash
   pip install -r requirements.txt
   ```
4. Run the microservice:
   ```bash
   python main.py
   ```
5. The service runs on `http://127.0.0.1:8000`.

### 3. Frontend (Angular)
The user interface for Customers, Agents, Admins, and Claims Officers.
1. Choose one of the versions in `EventInsuranceSystem/frontend/` (e.g., `event-insurance-tailwind`).
2. Navigate to the selected folder.
3. Install dependencies:
   ```bash
   npm install
   ```
4. Start the development server:
   ```bash
   npm start
   ```
5. Open your browser to `http://localhost:4200`.

---

## 👤 User Roles & Access
The application uses role-based access control. You can log in using predefined accounts listed on the login page or create new ones via the Registration page.

- **Customer**: Apply for insurance, upload documents, pay premiums, and file claims.
- **Agent**: Review assigned requests, suggest policies, and escalate to admins.
- **Admin**: Assign agents, give final application approvals, and monitor system financials.
- **Claims Officer**: Trigger AI fraud detection on claims and approve/reject payouts.

---

## 📄 Technical Documentation
For more detailed information on the system architecture, database schema, and workflows, please refer to the documents in the `Docs` directory:
- [ER Diagram](Docs/ER.pdf)
- [Business Requirements](Docs/Business_Requirement_Dcoument_Capstone_Project.pdf)
- [Sequence Diagrams](Docs/sequences/)
