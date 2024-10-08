USE [AdventureWorks2022]
GO

/****** Object:  View [dbo].[Sales]    Script Date: 08/10/2024 9:01:26 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[Sales] AS
SELECT 
    soh.SalesOrderID,
    soh.OrderDate,
    soh.DueDate,
    soh.Status AS OrderStatus,
    soh.TotalDue,
    
    -- Customer Information
    c.CustomerID,
    p.FirstName,
    p.LastName,
    p.Title,
    
    -- Sales Order Details
    sod.SalesOrderDetailID,
    sod.OrderQty,
    sod.LineTotal,
    
    -- Product Information
    prod.ProductID,
    prod.Name AS ProductName,
    prod.ProductNumber,
    prod.StandardCost,
    prod.ListPrice

FROM 
    Sales.SalesOrderHeader AS soh
    
    -- Join with SalesOrderDetail to get line item information
    JOIN Sales.SalesOrderDetail AS sod
    ON soh.SalesOrderID = sod.SalesOrderID
    
    -- Join with Product to get product details
    JOIN Production.Product AS prod
    ON sod.ProductID = prod.ProductID
    
    -- Join with Customer to get customer info
    JOIN Sales.Customer AS c
    ON soh.CustomerID = c.CustomerID
    
    -- Join with Person to get customer name and email
    JOIN Person.Person AS p
    ON c.PersonID = p.BusinessEntityID;
GO


