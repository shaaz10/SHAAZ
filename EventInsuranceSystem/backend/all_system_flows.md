# EventGuard System Workflows (PlantUML)

Here are the detailed PlantUML sequence diagrams for all the key workflows and CRUD operations throughout the system.

## 1. Customer Submits New Request
```plantuml
@startuml
autonumber

actor "Customer" as Customer
participant "Angular UI" as AngularUI
participant "InsuranceRequest Controller" as Controller
participant "InsuranceRequest Service" as AppService
participant "Repository" as Repository
database "Database" as Database
participant "Notification Service" as Notification

Customer -> AngularUI : Fill form & click Submit Request
AngularUI -> Controller : HTTP POST /api/insurancerequest (DTO)

box "Backend Processing" #EEF2F5
Controller -> AppService : CreateRequestAsync(customerId, DTO)
AppService -> Repository : AddAsync(request)
Repository -> Database : Execute SQL INSERT
Database --> Repository : Success
Repository --> AppService : Saved
AppService -> Notification : Notify Customer (Request Submitted)
end box

AppService --> Controller : Success
Controller --> AngularUI : HTTP 200 OK
AngularUI --> Customer : Display success message
@enduml
```

## 2. Admin Assigns Agent to Request
```plantuml
@startuml
autonumber

actor "Admin" as Admin
participant "Angular UI" as AngularUI
participant "InsuranceRequest Controller" as Controller
participant "InsuranceRequest Service" as AppService
participant "Repository" as Repository
database "Database" as Database
participant "Notification Service" as Notification

Admin -> AngularUI : Selects Agent and assigns
AngularUI -> Controller : HTTP POST /api/insurancerequest/assign-agent (DTO)

box "Backend Processing" #EEF2F5
Controller -> AppService : AssignAgentAsync(requestId, agentId)
AppService -> Repository : GetByIdAsync(requestId)
Repository --> AppService : Returns Request entity
AppService -> AppService : Update AssignedAgentId & Status
AppService -> Repository : UpdateAsync(request)
Repository -> Database : Execute SQL UPDATE
Database --> Repository : Success
AppService -> Notification : Notify Agent (New Assignment)
AppService -> Notification : Notify Customer (Agent Assigned)
end box

AppService --> Controller : Success
Controller --> AngularUI : HTTP 200 OK
AngularUI --> Admin : UI updates
@enduml
```

## 3. Agent Creates Policy Suggestion
```plantuml
@startuml
autonumber

actor "Agent" as Agent
participant "Angular UI" as AngularUI
participant "PolicySuggestion Controller" as Controller
participant "PolicySuggestion Service" as AppService
participant "InsuranceRequest Repo" as RequestRepo
participant "PolicySuggestion Repo" as SuggestionRepo
database "Database" as Database
participant "Notification Service" as Notification

Agent -> AngularUI : Fills suggestion details & clicks Submit
AngularUI -> Controller : HTTP POST /api/policysuggestion (DTO)

box "Backend Processing" #EEF2F5
Controller -> AppService : CreateSuggestionAsync(agentId, DTO)
AppService -> SuggestionRepo : AddAsync(suggestion)
SuggestionRepo -> Database : Execute SQL INSERT
AppService -> RequestRepo : GetByIdAsync(requestId)
RequestRepo --> AppService : Returns request
AppService -> AppService : Update Request Status to PolicySuggested
AppService -> RequestRepo : UpdateAsync(request)
RequestRepo -> Database : Execute SQL UPDATE
AppService -> Notification : Notify Customer (New Suggestion)
end box

AppService --> Controller : Success
Controller --> AngularUI : HTTP 200 OK
AngularUI --> Agent : UI updates
@enduml
```

## 4. Customer Selects Policy (Creates Application)
```plantuml
@startuml
autonumber

actor "Customer" as Customer
participant "Angular UI" as AngularUI
participant "PolicyApplication Controller" as Controller
participant "PolicyApplication Service" as AppService
participant "PolicyApplication Repo" as AppRepo
database "Database" as Database
participant "Notification Service" as Notification

Customer -> AngularUI : Clicks "Select This Policy"
AngularUI -> Controller : HTTP POST /api/policyapplication/select (suggestionId)

box "Backend Processing" #EEF2F5
Controller -> AppService : CreateApplicationFromSuggestionAsync(customerId, suggestionId)
AppService -> AppService : Map Suggestion to new PolicyApplication entity
AppService -> AppRepo : AddAsync(application)
AppRepo -> Database : Execute SQL INSERT
AppService -> AppService : Update Request & Suggestion statuses
AppRepo -> Database : Execute SQL UPDATE
AppService -> Notification : Notify Agent (Application Created)
end box

AppService --> Controller : Application ID
Controller --> AngularUI : HTTP 200 OK
AngularUI --> Customer : Shows Application Document Upload Section
@enduml
```

## 5. Customer Uploads Document (With AI Microservice)
```plantuml
@startuml
autonumber

actor "Customer" as Customer
participant "Angular UI" as AngularUI
participant "AppDocument Controller" as Controller
participant "AppDocument Service" as AppService
participant "FastAPI AI" as AIService
participant "AppDocument Repo" as DocRepo
participant "PolicyApplication Repo" as AppRepo
database "Database" as Database
participant "Notification Service" as Notification

Customer -> AngularUI : Selects file and uploads
AngularUI -> Controller : HTTP POST /api/applicationdocument/upload (FormData)

box "Backend Processing" #EEF2F5
Controller -> AppService : UploadDocumentAsync(appId, file)
AppService -> AIService : HTTP POST /analyze-document (Sends File)
Note right of AIService: Evaluates authenticity & extracts data
AIService --> AppService : JSON (ConfidenceScore, RiskScore, Category)
AppService -> DocRepo : AddAsync(documentWithStats)
DocRepo -> Database : Execute SQL INSERT
AppService -> AppRepo : GetByIdAsync(appId)
AppService -> AppService : Aggregate Risk & update Status to UnderAgentReview
AppRepo -> Database : Execute SQL UPDATE
AppService -> Notification : Notify Agent (Documents Uploaded)
end box

AppService --> Controller : Document Details
Controller --> AngularUI : HTTP 200 OK
AngularUI --> Customer : UI Shows Document Authenticated
@enduml
```

## 6. Agent Escalates Application to Admin
```plantuml
@startuml
autonumber

actor "Agent" as Agent
participant "Angular UI" as AngularUI
participant "AgentReview Controller" as Controller
participant "AgentReview Service" as AppService
participant "PolicyApplication Repo" as AppRepo
database "Database" as Database
participant "Notification Service" as Notification

Agent -> AngularUI : Reviews docs, clicks "Escalate to Admin"
AngularUI -> Controller : HTTP POST /api/agentreview/escalate (DTO)

box "Backend Processing" #EEF2F5
Controller -> AppService : EscalateToAdminAsync(appId, DTO)
AppService -> AppRepo : GetByIdAsync(appId)
AppService -> AppService : Update Status to EscalatedToAdmin, set ReviewNotes
AppService -> AppRepo : SaveChangesAsync()
AppRepo -> Database : Execute SQL UPDATE
AppService -> Notification : Notify Customer (Application Escalated)
AppService -> Notification : Notify Admin (Requires Admin Review)
end box

AppService --> Controller : Success
Controller --> AngularUI : HTTP 200 OK
AngularUI --> Agent : UI updates
@enduml
```

## 7. Admin Approves Application (Generates Active Policy)
```plantuml
@startuml
autonumber

actor "Admin" as Admin
participant "Angular UI" as AngularUI
participant "AdminApproval Controller" as Controller
participant "AdminApproval Service" as ApprovalService
participant "ActivePolicy Service" as PolicyService
participant "PolicyApplication Repo" as AppRepo
database "Database" as Database
participant "Notification Service" as Notification

Admin -> AngularUI : Clicks "Approve Application"
AngularUI -> Controller : HTTP POST /api/adminapproval/approve (DTO)

box "Backend Processing" #EEF2F5
Controller -> ApprovalService : ApproveApplicationAsync(appId, DTO)
ApprovalService -> AppRepo : GetByIdAsync(appId)
ApprovalService -> ApprovalService : Update Status to Approved
ApprovalService -> AppRepo : SaveChangesAsync()
AppRepo -> Database : Execute SQL UPDATE
ApprovalService -> Notification : Notify Customer & Agent (Approved)
ApprovalService --> Controller : Approval Success

Note over Controller, PolicyService: Active Policy Triggered Automatically
Controller -> PolicyService : GeneratePolicyFromApplicationAsync(appId)
PolicyService -> PolicyService : Create ActivePolicy (Generate ID, Start/End params)
PolicyService -> Database : Insert ActivePolicy
PolicyService -> PolicyService : Create AgentCommission record
PolicyService -> Database : Insert AgentCommission
PolicyService -> Notification : Notify Customer (Policy Activated) & Agent (Commission Earned)
end box

Controller --> AngularUI : HTTP 200 OK
AngularUI --> Admin : UI updates
@enduml
```

## 8. Customer Makes Payment
```plantuml
@startuml
autonumber

actor "Customer" as Customer
participant "Angular UI" as AngularUI
participant "Payment Controller" as Controller
participant "Payment Service" as AppService
participant "Payment Repo" as PaymentRepo
database "Database" as Database
participant "Notification Service" as Notification

Customer -> AngularUI : Enters amount, TXN ref, clicks Pay
AngularUI -> Controller : HTTP POST /api/payment (DTO)

box "Backend Processing" #EEF2F5
Controller -> AppService : ProcessPaymentAsync(DTO)
AppService -> PaymentRepo : AddAsync(payment)
PaymentRepo -> Database : Execute SQL INSERT
AppService -> Notification : Notify Customer (Payment Received)
AppService -> Notification : Notify Admin (New Payment Logged)
end box

AppService --> Controller : Success
Controller --> AngularUI : HTTP 200 OK
AngularUI --> Customer : UI updates
@enduml
```

## 9. Customer Raises a Claim
```plantuml
@startuml
autonumber

actor "Customer" as Customer
participant "Angular UI" as AngularUI
participant "Claim Controller" as Controller
participant "Claim Service" as AppService
participant "FastAPI AI" as AIService
participant "Claim Repo" as ClaimRepo
database "Database" as Database
participant "Notification Service" as Notification

Customer -> AngularUI : Selects Policy, enters amount & cause -> Submit
AngularUI -> Controller : HTTP POST /api/claim (DTO)

box "Backend Processing" #EEF2F5
Controller -> AppService : CreateClaimAsync(customerId, DTO)
AppService -> AppService : Create Claim Entity

Note over AppService, AIService: Automated AI Fraud Detection Trigger
AppService -> AIService : HTTP POST /detect-fraud (Claim Details)
AIService --> AppService : JSON (FraudScore, FraudIndicator, Reason)

AppService -> AppService : Append AI Evaluation to Claim Entity (Status: Pending)
AppService -> ClaimRepo : AddAsync(claim)
ClaimRepo -> Database : Execute SQL INSERT
AppService -> Notification : Notify Customer (Claim Submitted)
AppService -> Notification : Notify Claims Officer (New Claim to Review)
end box

AppService --> Controller : Success
Controller --> AngularUI : HTTP 200 OK
AngularUI --> Customer : Dashboard refreshes
@enduml
```

## 10. Claims Officer Settles Claim
```plantuml
@startuml
autonumber

actor "ClaimsOfficer" as Officer
participant "Angular UI" as AngularUI
participant "Claim Controller" as Controller
participant "Claim Service" as AppService
participant "Claim Repo" as ClaimRepo
database "Database" as Database
participant "Notification Service" as Notification

Officer -> AngularUI : Reviews AI Fraud Score, clicks Approve Claim
AngularUI -> Controller : HTTP PUT /api/claim/{id}/approve (DTO)

box "Backend Processing" #EEF2F5
Controller -> AppService : ApproveClaimAsync(claimId, officerId, comments)
AppService -> ClaimRepo : GetByIdAsync(claimId)
AppService -> AppService : Update Status to Approved, set Approver ID
AppService -> ClaimRepo : UpdateAsync(claim)
ClaimRepo -> Database : Execute SQL UPDATE
AppService -> Notification : Notify Customer (Claim Settled!)
end box

AppService --> Controller : Success
Controller --> AngularUI : HTTP 200 OK
AngularUI --> Officer : UI updates
@enduml
```
