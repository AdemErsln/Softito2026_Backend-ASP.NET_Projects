using Microsoft.Data.SqlClient;
using System;

namespace NezHotel
{
    public static class DatabaseHelper
    {
        public static readonly string ConnectionString = "Server=.;Database=NezHotel;Trusted_Connection=True;TrustServerCertificate=true;";
        private static readonly string MasterConnectionString = "Server=.;Database=master;Trusted_Connection=True;TrustServerCertificate=true;";

        public static void InitializeDatabase()
        {
            try
            {
                // 1. Create database if not exists
                using (var masterConn = new SqlConnection(MasterConnectionString))
                {
                    masterConn.Open();
                    string checkDbSql = "SELECT database_id FROM sys.databases WHERE name = 'NezHotel'";
                    bool dbExists = false;
                    using (var checkCmd = new SqlCommand(checkDbSql, masterConn))
                    {
                        var result = checkCmd.ExecuteScalar();
                        dbExists = result != null && result != DBNull.Value;
                    }

                    if (!dbExists)
                    {
                        using (var createCmd = new SqlCommand("CREATE DATABASE NezHotel", masterConn))
                        {
                            createCmd.ExecuteNonQuery();
                        }
                    }
                }

                // 2. Create tables
                using (var conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    // Musteriler Table
                    string createMusteriler = @"
                        IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Musteriler]') AND type in (N'U'))
                        BEGIN
                            CREATE TABLE [dbo].[Musteriler] (
                                [MusteriID] INT IDENTITY(1,1) PRIMARY KEY,
                                [AdSoyad] NVARCHAR(100) NOT NULL,
                                [Telefon] NVARCHAR(50),
                                [Email] NVARCHAR(100)
                            )
                        END";
                    using (var cmd = new SqlCommand(createMusteriler, conn)) { cmd.ExecuteNonQuery(); }

                    // Odalar Table
                    string createOdalar = @"
                        IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Odalar]') AND type in (N'U'))
                        BEGIN
                            CREATE TABLE [dbo].[Odalar] (
                                [OdaID] INT IDENTITY(1,1) PRIMARY KEY,
                                [OdaNo] INT NOT NULL,
                                [OdaTipi] NVARCHAR(50) NOT NULL,
                                [Fiyat] DECIMAL(18,2) NOT NULL,
                                [Durum] NVARCHAR(50) DEFAULT 'Boş'
                            )
                        END";
                    using (var cmd = new SqlCommand(createOdalar, conn)) { cmd.ExecuteNonQuery(); }

                    // Rezervasyonlar Table
                    string createRezervasyonlar = @"
                        IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Rezervasyonlar]') AND type in (N'U'))
                        BEGIN
                            CREATE TABLE [dbo].[Rezervasyonlar] (
                                [RezervasyonID] INT IDENTITY(1,1) PRIMARY KEY,
                                [MusteriID] INT NOT NULL,
                                [OdaID] INT NOT NULL,
                                [GirisTarihi] DATETIME NOT NULL,
                                [CikisTarihi] DATETIME NOT NULL,
                                [ToplamTutar] DECIMAL(18,2) NULL
                            )
                        END";
                    using (var cmd = new SqlCommand(createRezervasyonlar, conn)) { cmd.ExecuteNonQuery(); }

                    // YoneticiUsers Table (for Identity/Login)
                    string createUsers = @"
                        IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[YoneticiUsers]') AND type in (N'U'))
                        BEGIN
                            CREATE TABLE [dbo].[YoneticiUsers] (
                                [UserID] INT IDENTITY(1,1) PRIMARY KEY,
                                [Username] NVARCHAR(100) NOT NULL UNIQUE,
                                [Password] NVARCHAR(200) NOT NULL
                            )
                        END";
                    using (var cmd = new SqlCommand(createUsers, conn)) { cmd.ExecuteNonQuery(); }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database initialization failed: " + ex.Message);
            }
        }
    }
}
