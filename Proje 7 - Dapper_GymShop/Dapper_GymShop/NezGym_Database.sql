-- =====================================================
-- NezGym Veritabanı - Tablo ve Stored Procedure Scripti
-- =====================================================
-- Bu dosyayı SQL Server Management Studio'da (SSMS) açarak
-- sırasıyla çalıştırınız.
-- =====================================================

-- Veritabanını oluştur (yoksa)
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'NezGym')
BEGIN
    CREATE DATABASE NezGym;
END
GO

USE NezGym;
GO

-- =====================================================
-- TABLOLAR
-- =====================================================

-- Adminler Tablosu
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Admins')
BEGIN
    CREATE TABLE Admins (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(100) NOT NULL,
        Password NVARCHAR(200) NOT NULL
    );

    -- Varsayılan admin kullanıcısı
    INSERT INTO Admins (Username, Password) VALUES ('admin', '123456');
END
GO

-- Ürünler Tablosu
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Products')
BEGIN
    CREATE TABLE Products (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        ProductName NVARCHAR(200) NOT NULL,
        Price DECIMAL(18,2) NOT NULL DEFAULT 0,
        Stock INT NOT NULL DEFAULT 0,
        ImageURL NVARCHAR(500) NULL
    );

    -- Örnek ürünler
    INSERT INTO Products (ProductName, Price, Stock, ImageURL) VALUES
    (N'Altıgen Dambıl 20kg', 89.00, 25, '/assets/product-dumbbell-DA9apR2T.jpg'),
    (N'Pro Halter Eldiveni', 34.00, 50, '/assets/product-gloves-CVZn7jGF.jpg'),
    (N'Shaker Şişe 700ml', 19.00, 100, '/assets/product-shaker-gyQKVzoF.jpg'),
    (N'Direnç Lastiği Seti', 29.00, 40, '/assets/product-bands-C9Jf1S6s.jpg'),
    (N'Krom Dambıl 15kg', 69.00, 30, '/assets/product-dumbbell-DA9apR2T.jpg'),
    (N'Taktik Eldiven Siyah', 39.00, 60, '/assets/product-gloves-CVZn7jGF.jpg');
END
GO

-- Siparişler Tablosu
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders')
BEGIN
    CREATE TABLE Orders (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        CustomerName NVARCHAR(200) NOT NULL,
        CustomerPhone NVARCHAR(50) NULL,
        ShippingAddress NVARCHAR(500) NULL,
        OrderedProductName NVARCHAR(200) NOT NULL,
        Quantity INT NOT NULL DEFAULT 1
    );
END
GO

-- Kargo Firmaları Tablosu
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ShippingCompanies')
BEGIN
    CREATE TABLE ShippingCompanies (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        CompanyName NVARCHAR(200) NOT NULL,
        ShippingPrice DECIMAL(18,2) NOT NULL DEFAULT 0,
        EstimatedDeliveryDays INT NOT NULL DEFAULT 3
    );

    -- Örnek kargo firmaları
    INSERT INTO ShippingCompanies (CompanyName, ShippingPrice, EstimatedDeliveryDays) VALUES
    (N'Yurtiçi Kargo', 45.00, 2),
    (N'Aras Kargo', 40.00, 3),
    (N'MNG Kargo', 35.00, 3),
    (N'Sürat Kargo', 50.00, 1);
END
GO


-- =====================================================
-- STORED PROCEDURES
-- =====================================================

-- ===================== GİRİŞ ========================

-- Admin Login: Kullanıcı adı ve şifre ile giriş kontrolü
IF OBJECT_ID('sp_AdminLogin', 'P') IS NOT NULL DROP PROCEDURE sp_AdminLogin;
GO
CREATE PROCEDURE sp_AdminLogin
    @Username NVARCHAR(100),
    @Password NVARCHAR(200)
AS
BEGIN
    SELECT ID, Username, Password
    FROM Admins
    WHERE Username = @Username AND Password = @Password;
END
GO

-- =================== ÜRÜNLER ========================

-- Tüm ürünleri listele
IF OBJECT_ID('sp_GetIndexProducts', 'P') IS NOT NULL DROP PROCEDURE sp_GetIndexProducts;
GO
CREATE PROCEDURE sp_GetIndexProducts
AS
BEGIN
    SELECT ID, ProductName, Price, Stock, ImageURL
    FROM Products
    ORDER BY ID DESC;
END
GO

-- Yeni ürün ekle
IF OBJECT_ID('sp_AddProduct', 'P') IS NOT NULL DROP PROCEDURE sp_AddProduct;
GO
CREATE PROCEDURE sp_AddProduct
    @ProductName NVARCHAR(200),
    @Price DECIMAL(18,2),
    @Stock INT,
    @ImageURL NVARCHAR(500)
AS
BEGIN
    INSERT INTO Products (ProductName, Price, Stock, ImageURL)
    VALUES (@ProductName, @Price, @Stock, @ImageURL);
END
GO

-- Ürün güncelle
IF OBJECT_ID('sp_UpdateProduct', 'P') IS NOT NULL DROP PROCEDURE sp_UpdateProduct;
GO
CREATE PROCEDURE sp_UpdateProduct
    @ID INT,
    @ProductName NVARCHAR(200),
    @Price DECIMAL(18,2),
    @Stock INT,
    @ImageURL NVARCHAR(500)
AS
BEGIN
    UPDATE Products
    SET ProductName = @ProductName,
        Price = @Price,
        Stock = @Stock,
        ImageURL = @ImageURL
    WHERE ID = @ID;
END
GO

-- Ürün sil
IF OBJECT_ID('sp_DeleteProduct', 'P') IS NOT NULL DROP PROCEDURE sp_DeleteProduct;
GO
CREATE PROCEDURE sp_DeleteProduct
    @ID INT
AS
BEGIN
    DELETE FROM Products WHERE ID = @ID;
END
GO

-- ================== SİPARİŞLER ======================

-- Tüm siparişleri listele
IF OBJECT_ID('sp_GetOrders', 'P') IS NOT NULL DROP PROCEDURE sp_GetOrders;
GO
CREATE PROCEDURE sp_GetOrders
AS
BEGIN
    SELECT ID, CustomerName, CustomerPhone, ShippingAddress, OrderedProductName, Quantity
    FROM Orders
    ORDER BY ID DESC;
END
GO

-- ================ KARGO FİRMALARI ===================

-- Tüm kargo firmalarını listele
IF OBJECT_ID('sp_GetShippingCompanies', 'P') IS NOT NULL DROP PROCEDURE sp_GetShippingCompanies;
GO
CREATE PROCEDURE sp_GetShippingCompanies
AS
BEGIN
    SELECT ID, CompanyName, ShippingPrice, EstimatedDeliveryDays
    FROM ShippingCompanies
    ORDER BY ID;
END
GO

-- Yeni kargo firması ekle
IF OBJECT_ID('sp_AddShipping', 'P') IS NOT NULL DROP PROCEDURE sp_AddShipping;
GO
CREATE PROCEDURE sp_AddShipping
    @CompanyName NVARCHAR(200),
    @ShippingPrice DECIMAL(18,2),
    @EstimatedDeliveryDays INT
AS
BEGIN
    INSERT INTO ShippingCompanies (CompanyName, ShippingPrice, EstimatedDeliveryDays)
    VALUES (@CompanyName, @ShippingPrice, @EstimatedDeliveryDays);
END
GO

-- Kargo firması güncelle
IF OBJECT_ID('sp_UpdateShipping', 'P') IS NOT NULL DROP PROCEDURE sp_UpdateShipping;
GO
CREATE PROCEDURE sp_UpdateShipping
    @ID INT,
    @CompanyName NVARCHAR(200),
    @ShippingPrice DECIMAL(18,2),
    @EstimatedDeliveryDays INT
AS
BEGIN
    UPDATE ShippingCompanies
    SET CompanyName = @CompanyName,
        ShippingPrice = @ShippingPrice,
        EstimatedDeliveryDays = @EstimatedDeliveryDays
    WHERE ID = @ID;
END
GO

-- Kargo firması sil
IF OBJECT_ID('sp_DeleteShipping', 'P') IS NOT NULL DROP PROCEDURE sp_DeleteShipping;
GO
CREATE PROCEDURE sp_DeleteShipping
    @ID INT
AS
BEGIN
    DELETE FROM ShippingCompanies WHERE ID = @ID;
END
GO

-- ================ YÖNETİCİLER ======================

-- Tüm adminleri listele
IF OBJECT_ID('sp_GetAdmins', 'P') IS NOT NULL DROP PROCEDURE sp_GetAdmins;
GO
CREATE PROCEDURE sp_GetAdmins
AS
BEGIN
    SELECT ID, Username, Password
    FROM Admins
    ORDER BY ID;
END
GO

-- Yeni admin ekle
IF OBJECT_ID('sp_AddAdmin', 'P') IS NOT NULL DROP PROCEDURE sp_AddAdmin;
GO
CREATE PROCEDURE sp_AddAdmin
    @Username NVARCHAR(100),
    @Password NVARCHAR(200)
AS
BEGIN
    INSERT INTO Admins (Username, Password)
    VALUES (@Username, @Password);
END
GO

-- Admin sil
IF OBJECT_ID('sp_DeleteAdmin', 'P') IS NOT NULL DROP PROCEDURE sp_DeleteAdmin;
GO
CREATE PROCEDURE sp_DeleteAdmin
    @ID INT
AS
BEGIN
    DELETE FROM Admins WHERE ID = @ID;
END
GO

PRINT '✅ NezGym veritabanı, tablolar ve prosedürler başarıyla oluşturuldu!';
GO
