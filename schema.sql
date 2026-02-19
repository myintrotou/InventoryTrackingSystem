-- Inventory Tracking System Schema
-- Created: Nov 2025 (Updated Feb 2026)

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'InventoryDB')
BEGIN
    CREATE DATABASE InventoryDB;
END
GO

USE InventoryDB;
GO

-- 1. Suppliers Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Suppliers]') AND type in (N'U'))
BEGIN
    CREATE TABLE Suppliers (
        SupplierID INT PRIMARY KEY IDENTITY(1,1),
        SupplierName NVARCHAR(100) NOT NULL,
        ContactName NVARCHAR(100),
        Phone NVARCHAR(20),
        Email NVARCHAR(100)
    );
END

-- 2. Products Table (Stock Levels)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND type in (N'U'))
BEGIN
    CREATE TABLE Products (
        ProductID INT PRIMARY KEY IDENTITY(1,1),
        ProductName NVARCHAR(100) NOT NULL,
        StockQuantity INT DEFAULT 0,
        ReorderLevel INT DEFAULT 10,
        SupplierID INT FOREIGN KEY REFERENCES Suppliers(SupplierID)
    );
END

-- 3. Orders Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND type in (N'U'))
BEGIN
    CREATE TABLE Orders (
        OrderID INT PRIMARY KEY IDENTITY(1,1),
        ProductID INT FOREIGN KEY REFERENCES Products(ProductID),
        OrderDate DATETIME DEFAULT GETDATE(),
        Quantity INT NOT NULL,
        Status NVARCHAR(50) DEFAULT 'Pending'
    );
END
GO

-- STORED PROCEDURES (Optimized for performance)

-- Add Stock Procedure
CREATE OR ALTER PROCEDURE sp_AddStock
    @P_Id INT,
    @Qty INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Products 
    SET StockQuantity = StockQuantity + @Qty 
    WHERE ProductID = @P_Id;
END
GO

-- Get Low Stock Report
CREATE OR ALTER PROCEDURE sp_GetLowStock
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ProductName, StockQuantity, ReorderLevel 
    FROM Products 
    WHERE StockQuantity <= ReorderLevel;
END
GO

-- Get Suppliers
CREATE OR ALTER PROCEDURE sp_GetSuppliers
AS
BEGIN
    SET NOCOUNT ON;
    SELECT SupplierID, SupplierName, ContactName, Phone FROM Suppliers;
END
GO

-- Create Order
CREATE OR ALTER PROCEDURE sp_CreateOrder
    @P_Id INT,
    @Qty INT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Orders (ProductID, Quantity, Status)
    VALUES (@P_Id, @Qty, 'Ordered');
    
    -- Optional: Deducting stock if it was a sales order, 
    -- but usually "Orders" in inventory meant purchasing from suppliers.
END
GO

-- SEED DATA (For Testing)
IF NOT EXISTS (SELECT 1 FROM Suppliers)
BEGIN
    INSERT INTO Suppliers (SupplierName, ContactName, Phone, Email)
    VALUES ('TechCorp Solutions', 'John Doe', '555-0101', 'contact@techcorp.com'),
           ('Global Parts Inc', 'Jane Smith', '555-0202', 'sales@globalparts.com');

    INSERT INTO Products (ProductName, StockQuantity, ReorderLevel, SupplierID)
    VALUES ('Intel i9 Processor', 5, 10, 1),
           ('RTX 4090 GPU', 2, 5, 2),
           ('16GB DDR5 RAM', 50, 20, 1);
END
GO
