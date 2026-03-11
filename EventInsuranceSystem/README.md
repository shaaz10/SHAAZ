# EventGuard Insurance System

## Overview
The EventGuard Insurance System is a comprehensive full-stack application for managing the entire lifecycle of event insurance, including applications, policy generation, payments, and claims management. It includes AI integrations for processing risk assessments on applications and detecting fraud on claims.

## Tech Stack
- **Frontend**: Angular 21 (Zoneless, Signals), Standalone Components, themed with The Hartford's corporate branding. Role-based routing spanning Customer, Agent, Claims Officer, and Admin dashboards.
- **Backend APIs**: ASP.NET Core Web API (C#) handling business logic, authentication (JWT), and database operations.
- **AI Microservice**: FastAPI (Python) executing machine learning models to provide document confidence scores, risk categorization for new applications, and fraud scoring for raised claims.
- **Database**: Entity Framework Core + SQL Server mapping a complex cardinality across Users, Roles, Insurance Requests, Policy Suggestions, Applications, Documents, Active Policies, Payments, Commissions, and Claims.

## Architecture & Lifecycles

### Complete Event Insurance Lifecycle
1. **Request Submission**: A `Customer` creates an `InsuranceRequest` (Status: Submitted).
2. **Assignment**: An `Admin` assigns an `Agent` to the request (Status: AgentAssigned).
3. **Suggestion**: The assigned `Agent` reviews the request and suggests a `PolicyProduct` (creating a `PolicySuggestion`).
4. **Selection & Application**: The `Customer` selects a suggestion, generating a formal `PolicyApplication`.
5. **Document Upload**: The `Customer` uploads necessary `ApplicationDocuments`.
6. **AI Risk Assessment**: The AI FastAPI analyzes the documents, setting a `ConfidenceScore` and `RiskScore`. The application is flagged as UnderAgentReview.
7. **Agent Review & Escalation**: The `Agent` reviews the AI feedback and escalates the application to `Admin` (Status: EscalatedToAdmin).
8. **Admin Final Approval**: The `Admin` provides the final review and approves the application.
9. **Active Policy Generation**: The approved application is converted into an `ActivePolicy` (generating a Policy Number, Start/End Dates).
10. **Commissions & Payments**:
    - An `AgentCommission` record is generated.
    - The `Customer` makes a `Payment` against the Active Policy.
11. **Claims & Settlement**:
    - Should an incident occur, the `Customer` submits a `Claim`.
    - The FastAPI service executes AI Fraud Detection, assigning a `FraudScore` and `FraudFlag`.
    - A `ClaimsOfficer` reviews the claim and either Rejects or Approves/Settles it.

## Role-Based Access
- **Admin**: Oversees platform overview, manages new requests by assigning agents, provides final application approvals, generates active policies, and monitors platform financials (payments/commissions).
- **Agent**: Reviews assigned requests, suggests policies, and evaluates initial customer documents and risk scores before escalating to admins. Monitors their commissions.
- **Claims Officer**: Monitors submitted claims across the system, triggers the AI fraud detection sequence, and determines claim approval or rejection.
- **Customer**: Browses products, submits insurance requests, selects policy suggestions, uploads documents, view their active policies, makes payments, and raises claims.

## Getting Started
1. Run EventInsurance.API backend application.
2. Run EventInsurance.FastAPI AI microservice.
3. Run event-insurance-frontend (npm run build && npm start).
4. Default users are available directly on the login page.
