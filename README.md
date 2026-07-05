# 🚀 ASP.NET Core & C# Proje Portföyü

Bu çalışma alanı, modern web mimarileri, veri erişim teknolojileri (EF Core, ADO.NET, Dapper) ve kurumsal raporlama entegrasyonlarını içeren 9 adet C# projesinden oluşmaktadır.

---

## 📗 Proje 1 — BorrowBookManager
**Teknoloji:** ASP.NET Core MVC · EF Core Code-First · SQL Server

Kitap ödünç alma yönetim sistemi. Code-First yaklaşımıyla veritabanı şeması oluşturulmuş; ödünç işlemleri, üyeler ve kitaplar için tam CRUD desteği sağlanmıştır. Raporlama sayfasında LINQ sorgulamalarıyla özet istatistikler sunulur ve bu veriler Excel ile PDF formatında indirilebilir.

### Öne Çıkan Özellikler:
* EF Core Code-First & Migration
* LINQ tabanlı raporlama
* EPPlus ile Excel, QuestPDF ile PDF çıktısı

---

## 📘 Proje 2 — Kutuphane
**Teknoloji:** ASP.NET Core MVC · EF Core Db-First · SQL Server · Identity

Kütüphane kaynakları ve kullanıcı işlemlerinin takibi için geliştirilmiş veritabanı öncelikli (Db-First) yönetim sistemi. Identity entegrasyonu ile güvenli kullanıcı kayıt ve giriş sistemine sahiptir. Kitap ve yazar listeleri, arama filtreleri ve gelişmiş raporlama sayfaları bulunur. Raporlar PDF ve Excel çıktıları olarak indirilebilir.

### Öne Çıkan Özellikler:
* EF Core Db-First & SQL Server İlişkileri
* ASP.NET Core Identity ile Güvenli Giriş & Kayıt
* Gelişmiş Arama & Filtreleme Özellikleri
* EPPlus (Excel) & QuestPDF (PDF) Raporlama Entegrasyonu

---

## 🚗 Proje 3 — RentaCar
**Teknoloji:** ASP.NET Core MVC · EF Core Code-First · Katmanlı Mimari (Layered Architecture) · SQL Server · Identity

Katmanlı mimari (UI, Business, Data, Model) standartlarına uygun olarak tasarlanmış araç kiralama portalı. Araç envanteri yönetimi, kiralama işlemleri, dinamik arama filtreleri ve gelişmiş istatistiksel raporlama barındırır. Identity tabanlı Login/Register modülüyle güvenli giriş kontrolü sağlar.

### Öne Çıkan Özellikler:
* Katmanlı Mimari (Layered Architecture) Tasarımı
* Code-First Veritabanı ve Migration Yönetimi
* EPPlus Excel ve QuestPDF PDF Rapor Çıktısı
* Login & Register Yetkilendirmesi

---

## 🏨 Proje 4 — NezHotel
**Teknoloji:** ASP.NET Core Razor Pages · ADO.NET (Microsoft.Data.SqlClient) · SQL Server · Cookie Authentication

ADO.NET teknolojisi kullanılarak performansı yüksek şekilde kurgulanmış otel rezervasyon ve müşteri takip paneli. Çerez tabanlı (Cookie) güvenlik sistemiyle korunur. Dinamik müşteri arama filtresi, oda yönetimi ve rezervasyon takibi sunar. Excel ve PDF formatında müşteri ve oda raporları üretir.

### Öne Çıkan Özellikler:
* Saf ADO.NET SQL Bağlantısı ve SQL Command Kullanımı
* Çerez Tabanlı Kimlik Doğrulama (Cookie Auth)
* Dark-Glassmorphic Premium Arayüz Tasarımı
* QuestPDF (PDF) ve EPPlus (Excel) Raporlama Entegrasyonu

---

## 💼 Proje 5 — Franchise
**Teknoloji:** ASP.NET Core MVC · EF Core · SQL Server · AJAX & JQuery (SPA Mantığı)

Franchise bayileri ve yatırımcı paketlerinin yönetildiği, sayfa yenilenmeden işlem yapılabilmesi için tamamen AJAX ile kurgulanmış modern bir yönetim paneli. Paket tanımlama, bayi güncelleme ve anlık arama desteğine sahiptir. Excel ve PDF çıktı desteğiyle bayi kayıtları raporlanabilir.

### Öne Çıkan Özellikler:
* Yoğun AJAX ve JQuery Kullanımı (SPA Deneyimi)
* Sayfa Yenilemesiz Hızlı CRUD İşlemleri
* EPPlus Excel ve QuestPDF PDF Raporlama Entegrasyonu
* Yerel SQL Server Bağlantı Yönetimi

---

## 🚚 Proje 6 — NezLojistik
**Teknoloji:** ASP.NET Core Web API · ASP.NET Core MVC (Client) · EF Core Code-First · SQL Server

Depo, araç ve sürücü yönetimini sağlayan lojistik takip sistemi. Web API arka ucu ve MVC ön yüzü (Client) olarak ikiye ayrılmıştır. API üzerinden dinamik CRUD işlemleri gerçekleştirilir ve araç durumları, depolar arası taşıma verileri saklanır.

### Öne Çıkan Özellikler:
* Web API & MVC İstemci Mimarisi (Client-Server Separation)
* EF Core Code-First Veritabanı İlişkileri
* Depo, Araç ve Sürücü Takip Modelleri

---

## ☕ Proje 7 — KahveOtomati
**Teknoloji:** ASP.NET Core Web API · EF Core Code-First · SQL Server · Scalar API Dokümantasyonu

Kahve makinesinin malzeme stok durumlarını, ürün fiyatlarını ve satış kayıtlarını yöneten arka uç REST API servisi. EF Core Code-First veritabanı yapısıyla siparişleri ve ürün stoklarını yönetir.

### Öne Çıkan Özellikler:
* RESTful Web API Tasarımı
* Scalar API / Swagger Dokümantasyonu
* Malzeme, Ürün ve Stok Kontrol Algoritmaları

---

## 🏋️‍♂️ Proje 8 — GymShop
**Teknoloji:** ASP.NET Core MVC · Dapper Micro-ORM · SQL Server Stored Procedures

Dapper Micro-ORM ve SQL Server Stored Procedure'leri kullanılarak geliştirilmiş fitness salonu yönetim ve e-ticaret vitrini. Ürün yönetimi, kargo firması yönetimi, sipariş takibi ve admin kayıt sistemini içerir. Ürün envanteri için Excel ve PDF raporlama çıktıları sunar.

### Öne Çıkan Özellikler:
* Dapper ile Yüksek Performanslı Veri Erişimi
* Tamamen Stored Procedure Tabanlı Mimari
* QuestPDF ve EPPlus Raporlama Entegrasyonu

---

## 📱 Proje 9 — QRCodeGenerator
**Teknoloji:** ASP.NET Core MVC · QRCoder Kütüphanesi · SQL Server

Metin, URL veya telefon numaralarını dinamik olarak QR kod görseline dönüştüren ve üretilen kodları veritabanında saklayan QR kod yönetim sistemi. Üretilen QR kodlar PNG formatında indirilebilir ve geçmişe dönük arama ile listelenebilir.

### Öne Çıkan Özellikler:
* QRCoder ile Dinamik QR Kod Üretimi
* Üretilen QR Kodların İndirilmesi ve Saklanması
* Geçmiş Kod Arama ve Filtreleme
