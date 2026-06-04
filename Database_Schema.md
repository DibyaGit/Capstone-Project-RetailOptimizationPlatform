# Database Entity Relationship (ER) Diagram

This diagram visualizes the 1-to-Many relationship between Inventory and Orders, illustrating how our Foreign Key enforces data integrity.

```mermaid
erDiagram
    INVENTORY ||--o{ ORDER : "has many"
    
    INVENTORY {
        int Id PK "Primary Key"
        string ItemName "Max 100 chars"
        string Sku "Unique Index, Max 20 chars"
        int StockQuantity "Check Constraint >= 0"
        decimal Price "Check Constraint >= 0"
    }
    
    ORDER {
        int Id PK "Primary Key"
        string CustomerName "Max 100 chars"
        datetime OrderDate 
        decimal TotalAmount "Check Constraint > 0"
        int InventoryId FK "Foreign Key to Inventory"
    }
    
    SUPPLIER {
        int Id PK "Primary Key"
        string Name "Max 150 chars"
        string ContactPerson "Max 100 chars"
        string Email
        string PhoneNumber "Max 20 chars"
        string City "Max 100 chars"
        string State "Max 100 chars, Nullable"
        string Country "Max 100 chars"
        string PostalCode "Max 20 chars, Nullable"
        int LeadTimeDays
        string PaymentTerms "Max 50 chars"
        decimal CreditLimit
        bool IsActive
        decimal Rating "Check Constraint 0-5"
        string Notes "Max 500 chars, Nullable"
        datetime CreatedDate
        datetime LastUpdated
    }