# Architecture & Data Flow Diagrams

## System Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                    Retail Optimization Platform                  │
│                         (.NET 8 Architecture)                    │
└─────────────────────────────────────────────────────────────────┘

┌──────────────────────┐
│   Client Browser     │
│  (HTML/CSS/JS)       │
└──────────┬───────────┘
		   │
	┌──────┴────────┐
	│               │
	▼               ▼
┌─────────────┐  ┌──────────────┐
│  Web App    │  │  REST API    │
│ (MVC) :5001 │  │      :5002   │
│             │  │              │
│ - Dashboard │  │ - Auth       │
│ - Inventory │  │ - Inventory  │
│ - Orders    │  │ - Orders     │
└──────┬──────┘  └───────┬──────┘
	   │                 │
	   └────────┬────────┘
				│
				▼
		┌──────────────────────┐
		│  Application Layer   │
		│  (Core Project)      │
		│                      │
		│ - Services           │
		│ - Interfaces         │
		│ - Entities           │
		│ - Business Logic     │
		└──────────┬───────────┘
				   │
				   ▼
		┌──────────────────────┐
		│  Data Access Layer   │
		│  (Data Project)      │
		│                      │
		│ - DbContext          │
		│ - Repositories       │
		│ - Migrations         │
		└──────────┬───────────┘
				   │
				   ▼
		┌──────────────────────┐
		│   SQL Server         │
		│  (LocalDB)           │
		│                      │
		│ - Inventory table    │
		│ - Orders table       │
		│ - Constraints        │
		└──────────────────────┘
```

---

## Application Layers

```
┌────────────────────────────────────────────────────────────┐
│               PRESENTATION LAYER                           │
│  (RetailOptimizationPlatform.Web)                         │
│                                                            │
│  Controllers:                                              │
│  ├── HomeController (Dashboard)                           │
│  ├── InventoryController (CRUD)                           │
│  ├── OrderController (CRUD)                               │
│  ├── ReportsController (Analytics Reports)                │
│  └── SuppliersController (CRUD)                           │
│                                                            │
│  Views:                                                    │
│  ├── /Home (Dashboard with charts)                        │
│  ├── /Inventory (Index, Create, Edit, Delete)            │
│  ├── /Orders (Index, Create, Edit, Delete)               │
│  ├── /Reports (Analytics, SP Revenue Reports)            │
│  └── /Suppliers (Index, Create, Edit, Delete)            │
│                                                            │
│  Features: MVC, Razor, Bootstrap 5, Chart.js             │
└────────────────────────────────────────────────────────────┘
							▲
							│ Uses
							│
┌────────────────────────────────────────────────────────────┐
│             BUSINESS LOGIC LAYER                           │
│  (RetailOptimizationPlatform.Core)                        │
│                                                            │
│  Entities:                                                 │
│  ├── Inventory                                             │
│  ├── Order                                                 │
│  └── Supplier                                              │
│                                                            │
│  Services:                                                 │
│  ├── InventoryService                                      │
│  └── AnalyticsService (SaaS metrics calculation)           │
│                                                            │
│  Interfaces:                                               │
│  ├── IInventoryRepository                                  │
│  ├── IOrderRepository                                      │
│  ├── ISupplierRepository                                   │
│  ├── IInventoryService                                     │
│  └── IAnalyticsService                                     │
│                                                            │
│  Exceptions:                                               │
│  └── InventoryNotFoundException                            │
└────────────────────────────────────────────────────────────┘
							▲
							│ Uses
							│
┌────────────────────────────────────────────────────────────┐
│              DATA ACCESS LAYER                             │
│  (RetailOptimizationPlatform.Data)                        │
│                                                            │
│  ApplicationDbContext (EF Core)                            │
│  ├── DbSet<Inventory>                                      │
│  ├── DbSet<Order>                                          │
│  └── DbSet<Supplier>                                       │
│                                                            │
│  Repositories:                                             │
│  ├── InventoryRepository (CRUD)                            │
│  ├── OrderRepository (CRUD & raw SQL Command / Stored Proc)│
│  └── SupplierRepository (CRUD & Search string filtering)   │
│                                                            │
│  Migrations: Version control for schema                    │
└────────────────────────────────────────────────────────────┘
							▲
							│ SQL
							│
					┌───────────────┐
					│  SQL Server   │
					│   (LocalDB)   │
					└───────────────┘
```

---

## Database Schema

```
┌─────────────────────────────────────┐
│         INVENTORY TABLE             │
├─────────────────────────────────────┤
│ PK  Id            INT               │
│     ItemName      NVARCHAR(100)     │
│     Sku           NVARCHAR(20)  [U] │
│     StockQuantity INT        [C]    │
│     Price         DECIMAL(18,2)[C]  │
└─────────────────────────────────────┘
		▲
		│ 1 ─────────── ∞
		│ 1 FK relationship
		│
┌─────────────────────────────────────┐
│          ORDERS TABLE               │
├─────────────────────────────────────┤
│ PK  Id            INT               │
│     CustomerName  NVARCHAR(100)     │
│     OrderDate     DATETIME          │
│     TotalAmount   DECIMAL(18,2)     │
│ FK  InventoryId   INT (RESTRICT)    │
└─────────────────────────────────────┘

Legend:
[U] = Unique constraint
[C] = Check constraint (>= 0)
FK  = Foreign Key
PK  = Primary Key
```

---

## Request-Response Flow

```
USER INTERACTION (Web Browser)
		   │
		   ▼
	┌──────────────┐
	│ HTTP Request │
	└──────┬───────┘
		   │
		   ▼
	┌──────────────────────┐
	│ MVC Router           │
	│ Maps URL to          │
	│ Controller/Action    │
	└──────┬───────────────┘
		   │
		   ▼
	┌──────────────────────┐
	│ Controller Action    │
	│ (e.g., GetInventory) │
	└──────┬───────────────┘
		   │
		   ▼
	┌──────────────────────┐
	│ Dependency           │
	│ Injection resolves   │
	│ Repository           │
	└──────┬───────────────┘
		   │
		   ▼
	┌──────────────────────┐
	│ Repository calls     │
	│ DbContext            │
	│ (EF Core)            │
	└──────┬───────────────┘
		   │
		   ▼
	┌──────────────────────┐
	│ EF Core generates    │
	│ SQL Query            │
	└──────┬───────────────┘
		   │
		   ▼
	┌──────────────────────┐
	│ SQL Server executes  │
	│ Query                │
	└──────┬───────────────┘
		   │
		   ▼
	┌──────────────────────┐
	│ Database returns     │
	│ Results              │
	└──────┬───────────────┘
		   │
		   ▼
	┌──────────────────────┐
	│ EF Core maps         │
	│ to Entity Objects    │
	└──────┬───────────────┘
		   │
		   ▼
	┌──────────────────────┐
	│ Controller processes │
	│ & passes to View     │
	└──────┬───────────────┘
		   │
		   ▼
	┌──────────────────────┐
	│ Razor View renders   │
	│ HTML                 │
	└──────┬───────────────┘
		   │
		   ▼
	┌──────────────────────┐
	│ HTTP Response        │
	│ (HTML + CSS + JS)    │
	└──────┬───────────────┘
		   │
		   ▼
	BROWSER DISPLAYS PAGE
```

---

## API Authentication Flow

```
┌──────────────────────────────────────────────────┐
│  CLIENT APPLICATION                              │
└──────────────────┬───────────────────────────────┘
				   │
				   │ 1. POST /api/auth/login
				   │    { "username": "admin", "password": "password123" }
				   │
				   ▼
┌──────────────────────────────────────────────────┐
│  API - AUTH CONTROLLER                           │
│                                                  │
│  if (username == "admin" && password correct)   │
│  {                                               │
│    - Generate JWT Token                          │
│    - Include claims (username, role: Admin)      │
│    - Return token                                │
│  }                                               │
└──────────────────┬───────────────────────────────┘
				   │
				   │ 2. Response with token
				   │    { "token": "eyJhbGc..." }
				   │
				   ▼
		STORE TOKEN IN MEMORY/STORAGE
				   │
				   │
				   ▼
		┌──────────────────────────────────────┐
		│ 3. NEXT REQUEST with token          │
		│                                      │
		│ GET /api/inventoryapi                │
		│ Headers: {                           │
		│   Authorization: "Bearer eyJhbGc..." │
		│ }                                    │
		└──────────────┬───────────────────────┘
					   │
					   ▼
		┌──────────────────────────────────────┐
		│ API - VALIDATE TOKEN                 │
		│                                      │
		│ - Check signature                    │
		│ - Check expiration                   │
		│ - Extract claims                     │
		│ - Verify role (if needed)            │
		└──────────────┬───────────────────────┘
					   │
			┌──────────┴──────────┐
			│                     │
	 ✅ VALID              ❌ INVALID
	 Return data           Return 401
```

---

## CRUD Operations Flow

```
CREATE (Inventory Item)
┌─────────────────────────────────────┐
│ 1. User fills form on Create page   │
│    ItemName, SKU, Qty, Price        │
└──────────────┬──────────────────────┘
			   │
			   ▼
		┌──────────────┐
		│ Validation   │
		│ (Client-side)│
		└──────┬───────┘
			   │
			   ▼
		┌──────────────────────┐
		│ POST /Inventory      │
		│ Create(Inventory)    │
		└──────┬───────────────┘
			   │
			   ▼
		┌──────────────┐
		│ Validation   │
		│ (Server-side)│
		└──────┬───────┘
			   │
			   ▼
		┌──────────────────────┐
		│ AddAsync(inventory)  │
		│ Save to Database     │
		└──────┬───────────────┘
			   │
			   ▼
		┌──────────────┐
		│ Success ✓    │
		│ Redirect     │
		└──────────────┘

READ (Get Inventory)
┌─────────────────────────────────────┐
│ 1. User navigates to /Inventory     │
└──────────────┬──────────────────────┘
			   │
			   ▼
		┌──────────────────────┐
		│ GET /Inventory       │
		│ Index()              │
		└──────┬───────────────┘
			   │
			   ▼
		┌──────────────────────┐
		│ GetAllAsync()        │
		│ Fetch from Database  │
		└──────┬───────────────┘
			   │
			   ▼
		┌──────────────┐
		│ Return View  │
		│ with items   │
		└──────┬───────┘
			   │
			   ▼
		DISPLAY TABLE OF ITEMS

UPDATE (Inventory Item)
┌─────────────────────────────────────┐
│ 1. User clicks Edit button          │
└──────────────┬──────────────────────┘
			   │
			   ▼
		┌──────────────────────┐
		│ GET /Inventory/Edit/5│
		│ Edit(id)             │
		└──────┬───────────────┘
			   │
			   ▼
		┌──────────────────────┐
		│ GetByIdAsync(5)      │
		│ Load existing item   │
		└──────┬───────────────┘
			   │
			   ▼
		┌──────────────┐
		│ Show form    │
		│ with data    │
		└──────┬───────┘
			   │
			   ▼
		┌──────────────┐
		│ User modifies│
		│ fields       │
		└──────┬───────┘
			   │
			   ▼
		┌──────────────────────┐
		│ POST /Inventory/Edit │
		│ Edit(id, inventory)  │
		└──────┬───────────────┘
			   │
			   ▼
		┌──────────────────────┐
		│ UpdateAsync(item)    │
		│ Save changes         │
		└──────┬───────────────┘
			   │
			   ▼
		┌──────────────┐
		│ Success ✓    │
		│ Redirect     │
		└──────────────┘

DELETE (Inventory Item)
┌─────────────────────────────────────┐
│ 1. User clicks Delete button        │
└──────────────┬──────────────────────┘
			   │
			   ▼
		┌──────────────────────┐
		│ GET /Inventory/Delete│
		│ Delete(id)           │
		└──────┬───────────────┘
			   │
			   ▼
		┌──────────────────────┐
		│ GetByIdAsync(id)     │
		│ Load for confirmation│
		└──────┬───────────────┘
			   │
			   ▼
		┌──────────────┐
		│ Show delete  │
		│ confirmation │
		└──────┬───────┘
			   │
			   ▼
		┌──────────────┐
		│ User confirms│
		│ deletion     │
		└──────┬───────┘
			   │
			   ▼
		┌──────────────────────┐
		│ POST /Inventory/Delete│
		│ DeleteConfirmed(id)   │
		└──────┬───────────────┘
			   │
			   ▼
		┌──────────────────────┐
		│ DeleteAsync(id)      │
		│ Remove from Database │
		└──────┬───────────────┘
			   │
			   ▼
		┌──────────────┐
		│ Success ✓    │
		│ Redirect     │
		└──────────────┘
```

---

## Dependency Injection Resolution

```
Program.cs Configuration
════════════════════════════════════════════════════
builder.Services.AddDbContext<ApplicationDbContext>(...)
builder.Services.AddScoped<IInventoryRepository, 
						   InventoryRepository>();
builder.Services.AddScoped<IOrderRepository, 
						   OrderRepository>();
builder.Services.AddScoped<IInventoryService, 
						   InventoryService>();

When Request Arrives
════════════════════════════════════════════════════

InventoryController Constructor:
	public InventoryController(IInventoryRepository repo)
	{
		// DI Container resolves IInventoryRepository
		// to InventoryRepository instance
		// InventoryRepository needs ApplicationDbContext
		// DI Container provides it
		this._repository = repo;
	}

Resolution Chain
════════════════════════════════════════════════════
1. Request: InventoryController
2. Need: IInventoryRepository
3. Resolved: new InventoryRepository(dbContext)
4. Need: ApplicationDbContext
5. Resolved: new ApplicationDbContext(options)
6. All resolved → Create controller instance
7. Execute action → Return response
```

---

## Data Validation Pipeline

```
USER SUBMITS FORM
	   │
	   ▼
CLIENT-SIDE VALIDATION
├─ Required field check
├─ Email format
├─ Number range
└─ Pattern matching
	   │ ✓ Pass
	   │
	   ▼
SEND TO SERVER
POST /api/endpoint
	   │
	   ▼
SERVER-SIDE VALIDATION
├─ ModelState validation
│  ├─ Data type conversion
│  ├─ Required fields
│  └─ Range/Length checks
├─ Business logic validation
│  ├─ Unique constraints
│  ├─ Foreign key existence
│  └─ Custom rules
└─ Database constraints
   ├─ Check constraints
   ├─ Unique constraints
   └─ Foreign keys
	   │
	   ├─ ✓ All Pass
	   │  │
	   │  ▼
	   │ SAVE TO DATABASE
	   │ SUCCESS → Return 200/201
	   │
	   └─ ✗ Validation Fails
		  │
		  ▼
	   RETURN ERROR
	   ├─ Status: 400/422
	   └─ Message: What failed
```

---

## Session & Security

```
Session Timeline
════════════════════════════════════════════════

00:00 - User logs in
	   │ Credentials valid
	   │ JWT token generated (expires at 01:00)
	   │
00:15 - User makes request
	   │ Token: Valid
	   │ Request: Allowed
	   │
00:59 - User makes request
	   │ Token: Still valid (1 min left)
	   │ Request: Allowed
	   │
01:00 - User makes request
	   │ Token: Expired
	   │ Request: DENIED (401)
	   │ User must login again
	   │
01:05 - User logs in again
	   │ New token generated (expires at 02:05)
	   │ User can continue working

Security Features
════════════════════════════════════════════════

1. Token Signing
   ├─ JWT signed with secret key
   ├─ Cannot be modified without key
   └─ Server verifies signature

2. Token Expiration
   ├─ Token expires after 1 hour
   ├─ User must reauthenticate
   └─ Limits exposure if token stolen

3. Role Claims
   ├─ Token includes user role
   ├─ Admin vs Regular user
   └─ Controls access to features

4. HTTPS
   ├─ All connections encrypted
   ├─ Tokens transmitted securely
   └─ Man-in-the-middle protection
```

---

This architecture represents a **production-ready, scalable N-Tier application** built with .NET 8, following SOLID principles and industry best practices.
