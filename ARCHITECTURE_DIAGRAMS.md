# Enterprise System Architecture & Data Flow Visualizations

This document contains the high-fidelity structural, relational, and data flow visualizations for the **DC-Analytics: Enterprise Retail Optimization Platform**. 

These diagrams render dynamically as high-resolution SVGs when viewed on GitHub.

---

## 1. Decoupled N-Tier Solution Architecture

The application strictly separates concerns across five distinct projects, preventing coupling between the presentation layer and database execution.

```mermaid
flowchart TD
    Client["Client Browser\n(HTML5, CSS3, JavaScript, Chart.js)"]
    
    subgraph Presentation ["Presentation Layer"]
        MVC["ASP.NET Core MVC Web Application\n(RetailOptimizationPlatform.Web :5194)"]
        API["REST API Gateway & Security Hub\n(RetailOptimizationPlatform.Api :7207)"]
    end
    
    subgraph Core ["Business Logic Layer (Core)"]
        Services["Domain Services\n(InventoryService, AnalyticsService)"]
        Interfaces["Interfaces & Abstract Repository Contracts\n(IInventoryRepository, IAnalyticsService)"]
        Entities["Domain Entities & Core DTOs\n(Inventory, Order, Supplier, SalesTrendDto)"]
    end
    
    subgraph Data ["Data Access Layer (Data)"]
        DbContext["ApplicationDbContext\n(Entity Framework Core)"]
        Repos["Concrete Repository Implementations\n(OrderRepository, InventoryRepository)"]
        Migrations["EF Core Schema Migrations"]
    end
    
    subgraph Persistence ["Data Persistence Layer"]
        SQL["Microsoft SQL Server\n(Azure SQL / LocalDB)"]
    end
    
    Client -->|HTTP Form Posts & Page Navigation| MVC
    Client -->|Asynchronous AJAX & JWT Bearer Header| API
    
    MVC -->|Domain Service Calls| Services
    API -->|Domain Service Calls| Services
    
    Services --> Interfaces
    Entities -.-> Interfaces
    
    Repos -->|Implements| Interfaces
    Repos --> DbContext
    DbContext --> Migrations
    DbContext -->|Parameterized T-SQL| SQL

    style Client fill:#2a7ae2,stroke:#1a5fb4,stroke-width:2px,color:#fff
    style MVC fill:#47a862,stroke:#2d7d42,stroke-width:2px,color:#fff
    style API fill:#47a862,stroke:#2d7d42,stroke-width:2px,color:#fff
    style Services fill:#e29b26,stroke:#b87a15,stroke-width:2px,color:#fff
    style DbContext fill:#9247a8,stroke:#6f2d7d,stroke-width:2px,color:#fff
    style SQL fill:#c93b3b,stroke:#a12626,stroke-width:2px,color:#fff
```

---

## 2. Comprehensive Relational Database Schema (ERD)

This Entity-Relationship Diagram maps the structural relationships, keys, unique constraints, and check constraints enforced by the SQL Server engine.

```mermaid
erDiagram
    INVENTORIES {
        int Id PK "Identity(1,1)"
        nvarchar ItemName "Length 100, Required"
        nvarchar Sku UK "Length 20, Unique Index"
        int StockQuantity "Check >= 0 (No negative stock)"
        decimal Price "Check >= 0.00"
    }
    
    ORDERS {
        int Id PK "Identity(1,1)"
        nvarchar CustomerName "Length 100, Required"
        datetime OrderDate "Default: GETDATE()"
        decimal TotalAmount "Check > 0.00"
        int InventoryId FK "References INVENTORIES(Id)"
    }
    
    SUPPLIERS {
        int Id PK "Identity(1,1)"
        nvarchar Name "Length 150, Required"
        nvarchar Email "Required Format"
        nvarchar City "Length 100"
        int LeadTimeDays "Required"
        decimal CreditLimit "Precision (18,2)"
        bit IsActive "Default: 1 (Active)"
    }
    
    INVENTORIES ||--o{ ORDERS : "1-to-Many (Cascade Restrict Delete)"
```

* **Data Safety Note**: Deletions on the `INVENTORIES` table are restricted if associated orders exist. Stock depletions trigger native rollback constraints if values fall below `0`.

---

## 3. Web Request-Response Execution Lifecycle

The sequence below illustrates the lifecycle of a standard HTTP GET request targeting the MVC controller and returning a compiled Razor View.

```mermaid
sequenceDiagram
    autonumber
    actor Browser as Client Browser
    participant Router as ASP.NET Routing Engine
    participant Ctrl as MVC Controller (e.g. Inventory)
    participant DI as IoC Container (DI)
    participant Service as Business Domain Service
    participant Repo as Repository Layer
    participant EF as DbContext (EF Core)
    participant DB as Azure SQL Database
    participant Razor as Razor Rendering Engine

    Browser->>Router: HTTP GET Request (e.g. /Inventory/Index)
    Router->>Ctrl: Routes to Index() Action
    Ctrl->>DI: Request IInventoryService implementation
    DI-->>Ctrl: Injects InventoryService instance
    Ctrl->>Service: Invokes GetAllInventoryItemsAsync()
    Service->>Repo: Requests entities from IInventoryRepository
    Repo->>EF: Queries context.Inventories.ToListAsync()
    EF->>DB: Executes parameterized SELECT SQL Query
    DB-->>EF: Returns Raw Tabular Result Set
    EF-->>Repo: Maps records into IList<Inventory> Entities
    Repo-->>Service: Returns domain Entities
    Service-->>Ctrl: Maps to InventoryDto list and returns
    Ctrl->>Razor: Passes DTO list to Razor View Model
    Razor->>Razor: Compiles CSHTML and bundles CSS/JS variables
    Razor-->>Browser: Sends compiled HTML document (HTTP 200 OK)
```

---

## 4. Stateless JWT Security Authentication Flow

This diagram illustrates the token issuance lifecycle and subsequent verification of role-based authorization headers for protected RESTful endpoints.

```mermaid
sequenceDiagram
    autonumber
    actor Client as API Client (Frontend/Postman)
    participant Auth as AuthController (Gateway)
    participant DB as SQL Server
    participant Security as JWT Signature Engine
    participant Middleware as Auth Middleware Layer
    participant Endpoint as Protected Endpoint

    Client->>Auth: POST /api/auth/login { Username, Password }
    Auth->>DB: Query Admin credentials
    DB-->>Auth: Verified (Identity OK)
    Auth->>Security: Request cryptographically signed token
    Security->>Security: Sign header + payload with HMAC-SHA256 Secret Key
    Security-->>Auth: Generated JWT Token
    Auth-->>Client: Returns 200 OK with Bearer Token { Token: "eyJhbG..." }
    
    Note over Client: Store JWT in memory or local storage
    
    Client->>Middleware: GET /api/inventoryapi (Header: Authorization "Bearer eyJ...")
    Middleware->>Middleware: Decode header & verify cryptographic signature
    
    alt Signature Valid & Expiry Time Not Exceeded
        Middleware->>Middleware: Extract claims (Role: Admin)
        alt User authorized for [Authorize(Roles = "Admin")]
            Middleware->>Endpoint: Forward execution request
            Endpoint->>Client: Return 200 OK JSON Data
        else User Role Mismatch
            Middleware-->>Client: Return 403 Forbidden
        end
    else Signature Invalid or Token Expired
        Middleware-->>Client: Return 401 Unauthorized
    end
```

---

## 5. CRUD Execution & Asynchronous UI Mutations

To simulate a Single Page Application (SPA) experience within ASP.NET Core MVC, standard form submittals are intercepted by the JavaScript Fetch API to allow real-time DOM updates.

```mermaid
flowchart TD
    Start["User Triggers Action\n(Click Create/Edit/Delete)"] --> Intercept["JavaScript interceptSubmit()\nruns event.preventDefault()"]
    Intercept --> Validate["Client-side validation runs\n(check fields, ranges, unique SKU)"]
    
    Validate -->|Validation Fails| UIError["Display validation errors\nlocally on form"]
    Validate -->|Validation Passes| Fetch["Dispatch async Fetch API request\n(POST/PUT/DELETE)"]
    
    Fetch --> Controller["MVC Controller Action receives payload\n(Model Binder binds DTO)"]
    Controller --> ModelCheck{"ModelState.IsValid?"}
    
    ModelCheck -->|Invalid| ServerError["Return JSON Error Dictionary\n(HTTP 400 Bad Request)"]
    ModelCheck -->|Valid| EFSave["Call DbContext and execute\nSaveChanges / Commit Transaction"]
    
    EFSave --> DBCheck{"SQL Engine constraints\nsucceed?"}
    DBCheck -->|No| DBOldRollback["Transaction rolled back\nReturn HTTP 500 Internal Error"]
    DBCheck -->|Yes| SuccessJSON["Return JSON Success Payload\n{ success = true, id = 5 }"]
    
    ServerError --> ErrorParse["JS parses error object and\npopulates dynamic UI feedback"]
    DBOldRollback --> ErrorParse
    
    SuccessJSON --> JSDOMUpdate["JS updates target table row\nand triggers Bootstrap Toast Notification"]
    
    style Start fill:#f9f9f9,stroke:#333,stroke-width:1px
    style JSDOMUpdate fill:#d4edda,stroke:#28a745,stroke-width:2px,color:#155724
    style ErrorParse fill:#f8d7da,stroke:#dc3545,stroke-width:2px,color:#721c24
```

---

## 6. Dependency Injection Resolution Tree

The inversion of control (IoC) container handles object lifecycles. Transient, Scoped, and Singleton scopes isolate database transactions to single HTTP requests.

```mermaid
graph LR
    HTTP["Incoming HTTP Request Context"] --> RequestScope["Scoped Lifetime Instance Created"]
    RequestScope --> Controller["InventoryController Constructor"]
    Controller -->|Needs Service Abstraction| ServiceInterface["IInventoryService"]
    ServiceInterface -->|DI Resolver maps to| ServiceInstance["InventoryService"]
    ServiceInstance -->|Needs Repo Abstraction| RepoInterface["IInventoryRepository"]
    RepoInterface -->|DI Resolver maps to| RepoInstance["InventoryRepository"]
    RepoInstance -->|Needs database context| DbContext["ApplicationDbContext"]
    DbContext -->|Needs configuration| DbOptions["DbContextOptions<ApplicationDbContext>"]
    
    classDef scope fill:#e1f5fe,stroke:#0288d1,stroke-width:1px;
    class HTTP,RequestScope scope;
```

---

## 7. Comprehensive Data Validation Pipeline

Data flows through multiple validation gates before committing to storage.

```mermaid
flowchart LR
    ClientInput["Client Input"] --> Gate1["Gate 1: Client-Side JS\n- Required checks\n- Email regex patterns\n- Number boundaries"]
    Gate1 --> Gate2["Gate 2: Controller Bind\n- ModelState.IsValid\n- Server data conversion\n- Logical boundary checks"]
    Gate2 --> Gate3["Gate 3: Business Logic\n- Duplicate SKU checks\n- Valid foreign keys\n- Specific thresholds"]
    Gate3 --> Gate4["Gate 4: DB Constraints\n- Unique Index (SKU)\n- Check Constraints\n- Transaction Rollbacks"]
    Gate4 --> DBCommit[("SQL SERVER COMMIT")]
```
