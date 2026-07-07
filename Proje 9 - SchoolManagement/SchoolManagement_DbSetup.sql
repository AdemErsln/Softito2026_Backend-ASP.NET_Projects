-- =====================================================
-- SchoolManagement Veritabanı Kurulum Scripti
-- =====================================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'SchoolManagementDb')
BEGIN
    CREATE DATABASE SchoolManagementDb;
END
GO

USE SchoolManagementDb;
GO

-- =====================================================
-- TABLOLAR
-- =====================================================

-- 1. Adminler (Admins) Tablosu
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Admins')
BEGIN
    CREATE TABLE Admins (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(100) NOT NULL UNIQUE,
        Password NVARCHAR(200) NOT NULL
    );
END
GO

-- 2. Sınıflar (Classrooms) Tablosu
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Classrooms')
BEGIN
    CREATE TABLE Classrooms (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Capacity INT NOT NULL DEFAULT 20,
        Location NVARCHAR(100) NULL
    );
END
GO

-- 3. Öğrenciler (Students) Tablosu
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Students')
BEGIN
    CREATE TABLE Students (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        FirstName NVARCHAR(100) NOT NULL,
        LastName NVARCHAR(100) NOT NULL,
        StudentNumber NVARCHAR(50) NOT NULL UNIQUE,
        Email NVARCHAR(150) NULL,
        ClassroomId INT NULL,
        FOREIGN KEY (ClassroomId) REFERENCES Classrooms(ID) ON DELETE SET NULL
    );
END
GO

-- =====================================================
-- ÖRNEK (SEED) VERİLER
-- =====================================================

-- Admin verisi ekle
IF NOT EXISTS (SELECT 1 FROM Admins)
BEGIN
    INSERT INTO Admins (Username, Password) VALUES ('admin', '123456');
END
GO

-- Sınıf verileri ekle
IF NOT EXISTS (SELECT 1 FROM Classrooms)
BEGIN
    INSERT INTO Classrooms (Name, Capacity, Location) VALUES
    (N'10-A', 25, N'A Blok - 1. Kat'),
    (N'10-B', 20, N'A Blok - 1. Kat'),
    (N'11-A', 30, N'B Blok - 2. Kat'),
    (N'12-Fen A', 15, N'Lab Binası - 1. Kat');
END
GO

-- Öğrenci verileri ekle
IF NOT EXISTS (SELECT 1 FROM Students)
BEGIN
    DECLARE @Class10A INT = (SELECT TOP 1 ID FROM Classrooms WHERE Name = N'10-A');
    DECLARE @Class10B INT = (SELECT TOP 1 ID FROM Classrooms WHERE Name = N'10-B');
    DECLARE @Class11A INT = (SELECT TOP 1 ID FROM Classrooms WHERE Name = N'11-A');

    INSERT INTO Students (FirstName, LastName, StudentNumber, Email, ClassroomId) VALUES
    (N'Ahmet', N'Yılmaz', N'20260001', N'ahmet.yilmaz@school.edu', @Class10A),
    (N'Ayşe', N'Kaya', N'20260002', N'ayse.kaya@school.edu', @Class10A),
    (N'Mehmet', N'Demir', N'20260003', N'mehmet.demir@school.edu', @Class10B),
    (N'Fatma', N'Çelik', N'20260004', N'fatma.celik@school.edu', @Class11A),
    (N'Mustafa', N'Şahin', N'20260005', N'mustafa.sahin@school.edu', NULL),
    (N'Zeynep', N'Öztürk', N'20260006', N'zeynep.ozturk@school.edu', NULL);
END
GO
