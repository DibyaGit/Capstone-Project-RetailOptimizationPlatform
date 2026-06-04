
-- Retail Optimization Platform - Database Setup & Advanced SQL Concepts
-- Fulfills requirement: DDL/DML/Triggers & Stored Procedures


USE [RetailOptimizationDb]; 
GO

-- 1. COMPLEX OPERATION: TRIGGER
-- Automatically decrements the StockQuantity in the Inventories table 
-- whenever a new Order is successfully inserted.
CREATE OR ALTER TRIGGER trg_AutoDecrementStock
ON Orders
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- Update the inventory table by subtracting 1 from the stock for the purchased item
    UPDATE i
    SET i.StockQuantity = i.StockQuantity - 1
    FROM Inventories i
    INNER JOIN inserted ins ON i.Id = ins.InventoryId
    WHERE i.StockQuantity > 0; -- Prevent negative stock purely at the trigger level
END;
GO

-- 2. COMPLEX OPERATION: STORED PROCEDURE
-- Generates a revenue report by joining Orders and Inventories
CREATE OR ALTER PROCEDURE sp_GenerateRevenueReport
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        i.ItemName,
        i.Sku,
        COUNT(o.Id) AS TotalUnitsSold,
        ISNULL(SUM(o.TotalAmount), 0.00) AS TotalRevenueGenerated
    FROM Inventories i
    LEFT JOIN Orders o ON i.Id = o.InventoryId
    GROUP BY i.ItemName, i.Sku
    ORDER BY TotalRevenueGenerated DESC;
END;
GO