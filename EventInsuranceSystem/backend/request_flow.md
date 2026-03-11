# Insurance Request Data Flow

Here is the straightforward path your data takes when a customer submits a new Insurance Request:

**Angular UI Component** `[customer-dashboard.component.ts]` ➡️ **Angular Service** `[insurance-request.service.ts]` ➡️ **C# API Controller** `[InsuranceRequestController.cs]` ➡️ **C# Application Service** `[InsuranceRequestService.cs]` ➡️ **C# Infrastructure Repository** `[InsuranceRequestRepository.cs]` ➡️ **Entity Framework Core** `[ApplicationDbContext.cs]` ➡️ **SQL Server Database**

---
*If an error happens at the end (SQL Database), it travels backwards through this exact same chain until it is displayed on the Angular UI!*
