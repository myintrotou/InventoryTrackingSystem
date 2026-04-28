-- Optimized Warehouse Schema for SQLite
-- Indices added for frequently searched items and performance

-- 1. Suppliers Table
CREATE TABLE Suppliers (
    SupplierID INTEGER PRIMARY KEY AUTOINCREMENT,
    SupplierName TEXT NOT NULL,
    ContactName TEXT,
    Phone TEXT,
    Email TEXT
);

-- 2. Products Table (Stock Levels)
CREATE TABLE Products (
    ProductID INTEGER PRIMARY KEY AUTOINCREMENT,
    ProductName TEXT NOT NULL,
    StockQuantity INTEGER DEFAULT 0,
    ReorderLevel INTEGER DEFAULT 10,
    SupplierID INTEGER,
    FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID)
);

-- Optimization: Index for searching products by name
CREATE INDEX idx_products_name ON Products(ProductName);

-- Optimization: Index for stock quantity (useful for low stock filtering)
CREATE INDEX idx_products_stock ON Products(StockQuantity);

-- 3. Orders Table
CREATE TABLE Orders (
    OrderID INTEGER PRIMARY KEY AUTOINCREMENT,
    ProductID INTEGER,
    OrderDate TEXT DEFAULT (datetime('now')),
    Quantity INTEGER NOT NULL,
    Status TEXT DEFAULT 'Pending',
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

-- Optimization: Index for SupplierID to speed up joins
CREATE INDEX idx_products_supplier ON Products(SupplierID);

-- Seed Data
INSERT INTO Suppliers (SupplierName, ContactName, Phone, Email)
VALUES ('TechCorp Solutions', 'John Doe', '555-0101', 'contact@techcorp.com'),
       ('Global Parts Inc', 'Jane Smith', '555-0202', 'sales@globalparts.com');

INSERT INTO Products (ProductName, StockQuantity, ReorderLevel, SupplierID)
VALUES ('Intel i9 Processor', 5, 10, 1),
       ('RTX 4090 GPU', 2, 5, 2),
       ('16GB DDR5 RAM', 50, 20, 1);
