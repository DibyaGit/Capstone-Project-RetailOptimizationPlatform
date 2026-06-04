# Retail Optimization Platform - System Architecture

# This diagram illustrates the N-Tier architectural flow of the application, demonstrating the separation of concerns and data flow from the user interface down to the SQL Database.

```mermaid
graph TD
    %% User Interfaces
    Client([Web Browser / User]) -->|HTTP Request| MVC[RetailOptimizationPlatform.Web (MVC)]
    APIClient([Postman / Swagger]) -->|REST / JWT| API[RetailOptimizationPlatform.Api]

    %% Application Layer
    MVC -->|Dependency Injection| Core[RetailOptimizationPlatform.Core]
    API -->|Dependency Injection| Core

    %% Core Layer (Interfaces & Models)
    subgraph Core [Core Project (Business Logic)]
        Interfaces(IRepository Interfaces)
        Entities(Domain Models: Inventory, Order)
    end

    %% Data Layer
    Core -->|Implemented By| Data[RetailOptimizationPlatform.Data]
    
    subgraph Data [Data Project (EF Core)]
        DbContext(ApplicationDbContext)
        Repos(Repository Implementations)
    end

    %% Database
    Data -->|SQL Queries / Migrations| SQL[(MS SQL Server)]
    
    %% Styling
    classDef project fill:#f9f9f9,stroke:#333,stroke-width:2px;
    class MVC,API,Core,Data project;

 