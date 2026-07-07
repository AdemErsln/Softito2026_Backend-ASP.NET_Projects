# 🚀 ASP.NET Core & C# Proje Portföyü

Bu çalışma alanı, modern web mimarileri, veri erişim teknolojileri (EF Core, ADO.NET, Dapper) ve kurumsal raporlama entegrasyonlarını içeren 9 adet C# projesinden oluşmaktadır.

---

## 📗 Proje 1 — Çiftlik Yönetimi (CiftlikYonetimi)
**Teknoloji:** ASP.NET Core MVC · EF Core Code-First · SQL Server

Çiftlik hayvanlarının, yem stoklarının ve üretilen çiftlik ürünlerinin yönetimini sağlayan bilgi sistemi. Code-First yaklaşımıyla kurgulanan veritabanı yapısı sayesinde hayvan türleri ve hayvanlar arasında ilişkisel bir veri tabanı şeması sunar. Admin paneli üzerinden çiftlikteki tüm hayvanların takibi, yeni hayvan eklenmesi, yem stok durumlarının izlenmesi ve günlük üretim çıktılarının raporlanması gerçekleştirilir.

### Öne Çıkan Özellikler:
* EF Core Code-First & Migration Yönetimi
* Bire-Çok (One-to-Many) Veritabanı İlişkileri (Hayvanlar & Hayvan Türleri)
* Yönetici Paneli (Admin Dashboard) üzerinden Hayvan ve Ürün Kayıt Süreçleri
* Yem Stok Takip Sistemi (Miktar ve Tür bazlı) ve Günlük Çiftlik Üretim Verileri

### 📸 Ekran Görüntüleri:
<p align="left">
  <img src="screenshots/Proje 1 - CiftlikYonetimi/Home.png" width="260" alt="Ana Sayfa" />
  <img src="screenshots/Proje 1 - CiftlikYonetimi/Admin.png" width="260" alt="Yönetici Paneli" />
  <img src="screenshots/Proje 1 - CiftlikYonetimi/Animals.png" width="260" alt="Hayvan Listesi" />
</p>

---

## 📘 Proje 2 — Kutuphane
**Teknoloji:** ASP.NET Core MVC · EF Core Db-First · SQL Server · Identity

Kütüphane kaynakları ve kullanıcı işlemlerinin takibi için geliştirilmiş veritabanı öncelikli (Db-First) yönetim sistemi. Identity entegrasyonu ile güvenli kullanıcı kayıt ve giriş sistemine sahiptir. Kitap ve yazar listeleri, arama filtreleri ve gelişmiş raporlama sayfaları bulunur. Raporlar PDF ve Excel çıktıları olarak indirilebilir.

### Öne Çıkan Özellikler:
* EF Core Db-First & SQL Server İlişkileri
* ASP.NET Core Identity ile Güvenli Giriş & Kayıt
* Gelişmiş Arama & Filtreleme Özellikleri
* EPPlus (Excel) & QuestPDF (PDF) Raporlama Entegrasyonu

### 📸 Ekran Görüntüleri:
<p align="left">
  <img src="screenshots/Proje 2 - Kutuphane/Home.png" width="260" alt="Ana Sayfa" />
  <img src="screenshots/Proje 2 - Kutuphane/Books.png" width="260" alt="Kitaplar" />
  <img src="screenshots/Proje 2 - Kutuphane/Authors.png" width="260" alt="Yazarlar" />
</p>

---

## 🚗 Proje 3 — RentaCar
**Teknoloji:** ASP.NET Core MVC · EF Core Code-First · Katmanlı Mimari (Layered Architecture) · SQL Server · Identity

Katmanlı mimari (UI, Business, Data, Model) standartlarına uygun olarak tasarlanmış araç kiralama portalı. Araç envanteri yönetimi, kiralama işlemleri, dinamik arama filtreleri ve gelişmiş istatistiksel raporlama barındırır. Identity tabanlı Login/Register modülüyle güvenli giriş kontrolü sağlar.

### Öne Çıkan Özellikler:
* Katmanlı Mimari (Layered Architecture) Tasarımı
* Code-First Veritabanı ve Migration Yönetimi
* EPPlus Excel ve QuestPDF PDF Rapor Çıktısı
* Login & Register Yetkilendirmesi

### 📸 Ekran Görüntüleri:
<p align="left">
  <img src="screenshots/Proje 3 - RentaCar/Home.png" width="260" alt="Ana Sayfa" />
  <img src="screenshots/Proje 3 - RentaCar/Arac.png" width="260" alt="Araçlar" />
  <img src="screenshots/Proje 3 - RentaCar/Kiralama.png" width="260" alt="Kiralamalar" />
</p>

---

## 🏨 Proje 4 — NezHotel
**Teknoloji:** ASP.NET Core Razor Pages · ADO.NET (Microsoft.Data.SqlClient) · SQL Server · Cookie Authentication

ADO.NET teknolojisi kullanılarak performansı yüksek şekilde kurgulanmış otel rezervasyon ve müşteri takip paneli. Çerez tabanlı (Cookie) güvenlik sistemiyle korunur. Dinamik müşteri arama filtresi, oda yönetimi ve rezervasyon takibi sunar. Excel ve PDF formatında müşteri ve oda raporları üretir.

### Öne Çıkan Özellikler:
* Saf ADO.NET SQL Bağlantısı ve SQL Command Kullanımı
* Çerez Tabanlı Kimlik Doğrulama (Cookie Auth)
* Dark-Glassmorphic Premium Arayüz Tasarımı
* QuestPDF (PDF) ve EPPlus (Excel) Raporlama Entegrasyonu

### 📸 Ekran Görüntüleri:
<p align="left">
  <img src="screenshots/Proje 4 - NezHotel/Home.png" width="260" alt="Ana Sayfa" />
  <img src="screenshots/Proje 4 - NezHotel/Account_Login.png" width="260" alt="Kullanıcı Girişi" />
  <img src="screenshots/Proje 4 - NezHotel/Account_Register.png" width="260" alt="Kullanıcı Kaydı" />
</p>

---

## 💼 Proje 5 — Franchise
**Teknoloji:** ASP.NET Core MVC · EF Core · SQL Server · AJAX & JQuery (SPA Mantığı)

Franchise bayileri ve yatırımcı paketlerinin yönetildiği, sayfa yenilenmeden işlem yapılabilmesi için tamamen AJAX ile kurgulanmış modern bir yönetim paneli. Paket tanımlama, bayi güncelleme ve anlık arama desteğine sahiptir. Excel ve PDF çıktı desteğiyle bayi kayıtları raporlanabilir.

### Öne Çıkan Özellikler:
* Yoğun AJAX ve JQuery Kullanımı (SPA Deneyimi)
* Sayfa Yenilemesiz Hızlı CRUD İşlemleri
* EPPlus Excel ve QuestPDF PDF Raporlama Entegrasyonu
* Yerel SQL Server Bağlantı Yönetimi

### 📸 Ekran Görüntüleri:
<p align="left">
  <img src="screenshots/Proje 5 - Franchise/Home.png" width="260" alt="Ana Sayfa" />
  <img src="screenshots/Proje 5 - Franchise/Admin.png" width="260" alt="Yönetici Paneli" />
  <img src="screenshots/Proje 5 - Franchise/User.png" width="260" alt="Kullanıcı Arayüzü" />
</p>

---

## 🚚 Proje 6 — NezLojistik
**Teknoloji:** ASP.NET Core Web API · ASP.NET Core MVC (Client) · EF Core Code-First · SQL Server

Depo, araç ve sürücü yönetimini sağlayan lojistik takip sistemi. Web API arka ucu ve MVC ön yüzü (Client) olarak ikiye ayrılmıştır. API üzerinden dinamik CRUD işlemleri, arama, PDF/Excel çıktısı ve Login/Register süreçleri gerçekleştirilmektedir.

### Öne Çıkan Özellikler:
* Web API & MVC İstemci Mimarisi (Client-Server Separation)
* EF Core Code-First Veritabanı İlişkileri
* CRUD, Arama, PDF/Excel Raporlama ve Login/Register Entegrasyonu

### 📸 Ekran Görüntüleri:
<p align="left">
  <img src="screenshots/Proje 6 - NezLojistik/Home.png" width="230" alt="Ana Sayfa" />
  <img src="screenshots/Proje 6 - NezLojistik/Account_Login.png" width="230" alt="Kullanıcı Girişi" />
  <img src="screenshots/Proje 6 - NezLojistik/Account_Dashboard.png" width="230" alt="Kullanıcı Paneli" />
  <img src="screenshots/Proje 6 - NezLojistik/Admin_Panel.png" width="230" alt="Yönetim Paneli" />
  <img src="screenshots/Proje 6 - NezLojistik/Admin_Depots.png" width="230" alt="Depo Yönetimi" />
  <img src="screenshots/Proje 6 - NezLojistik/Admin_Drivers.png" width="230" alt="Sürücü Yönetimi" />
  <img src="screenshots/Proje 6 - NezLojistik/Admin_Vehicles.png" width="230" alt="Araç Yönetimi" />
  <img src="screenshots/Proje 6 - NezLojistik/swagger_index.html.png" width="230" alt="Swagger API Dokümanı" />
</p>

---

## 🏋️‍♂️ Proje 7 — Dapper_GymShop
**Teknoloji:** ASP.NET Core MVC · Dapper Micro-ORM · SQL Server Stored Procedures & ADO.NET

Dapper Micro-ORM ve SQL Server Stored Procedure'leri ile ADO.NET teknolojisi kullanılarak geliştirilmiş fitness salonu yönetim ve e-ticaret vitrini. CRUD işlemleri, arama filtreleri, PDF/Excel raporlama ve Login/Register işlemleri barındırır.

### Öne Çıkan Özellikler:
* Dapper ile Yüksek Performanslı Veri Erişimi
* Stored Procedure & ADO.NET Tabanlı Mimari
* CRUD, Arama, Login/Register ve EPPlus (Excel) & QuestPDF (PDF) Raporlaması

### 📸 Ekran Görüntüleri:
<p align="left">
  <img src="screenshots/Proje 7 - Dapper_GymShop/Home.png" width="260" alt="Ana Sayfa Store" />
  <img src="screenshots/Proje 7 - Dapper_GymShop/Admin_Dashboard.png" width="260" alt="Admin Panel" />
  <img src="screenshots/Proje 7 - Dapper_GymShop/Admin_Products.png" width="260" alt="Ürün Yönetimi" />
</p>

---

## ☕ Proje 8 — Kahve Otomatı (KahveOtomati)
**Teknoloji:** ASP.NET Core Web API · EF Core Code-First · SQL Server · Scalar API Documentation

Akıllı kahve otomatlarının takibi, içecek stokları, malzeme yönetimi, satış takibi ve arıza kayıtlarının yönetilmesini sağlayan modern bir Web API projesi. Scalar ile entegre edilmiş interaktif dokümantasyonu sayesinde tüm uç noktalar (endpoints) kolayca test edilebilir.

### Öne Çıkan Özellikler:
* EF Core Code-First ile Tasarlanmış İlişkisel Veritabanı Modelleri
* İçecekler (Icecekler), Satışlar (Satislar), Malzeme Stoku (MalzemeStoklari) ve Arıza Kayıtları (ArizaKayitlari) Yönetimi
* Scalar ve OpenAPI Entegrasyonu ile Modern API Test Arayüzü
* SQL Server ile Güçlü Veri Yönetimi

### 📸 Ekran Görüntüleri:
<p align="left">
  <img src="screenshots/Proje 8 - KahveOtomati/Home.png" width="260" alt="API Karşılama Ekranı" />
  <img src="screenshots/Proje 8 - KahveOtomati/scalar_v1.png" width="260" alt="Scalar Dokümantasyon" />
</p>

---

## 🏗️ Proje 9 — Discord Clone (DiscordClone)
**Teknoloji:** ASP.NET Core MVC · EF Core Code-First · N-Tier (N-Katmanlı) Mimari · Repository & Unit of Work · SQL Server · Identity

Repository ve Unit of Work tasarım desenleri kullanılarak çok katmanlı (Presentation, Data, Model) mimaride geliştirilmiş kapsamlı Discord klonu. Kullanıcılar ASP.NET Core Identity ile güvenli bir şekilde üye olabilir, giriş yapabilir, sunucular ve kanallar oluşturarak mesajlaşabilirler. Arkadaşlık sistemi (Beklemede, Kabul Edildi, Engellendi) ve sunucu üyelik yönetimi gibi gelişmiş sosyal ağ özelliklerine sahiptir.

### Öne Çıkan Özellikler:
* Çok Katmanlı Mimari (Presentation, Data Access ve Domain Models ayrımı)
* Repository ve Unit of Work Tasarım Desenleri ile gevşek bağlı veri erişim katmanı
* ASP.NET Core Identity ile Özelleştirilmiş Üyelik & Rol Yönetimi
* Sunucu (Server), Kanal (Channel) ve Anlık Mesajlaşma (Message) Yönetimi
* Detaylı Arkadaşlık (Friendship) ve Sunucu Üyeliği İlişkisel Veri Modelleri
* DbSeeder ile Başlangıç Rollerinin ve Yönetici Hesaplarının Otomatik Oluşturulması
* Admin Paneli (Dedicated Area) ile Tüm Sunucu, Kanal, Mesaj ve Kullanıcı Kontrolü

### 📸 Ekran Görüntüleri:
<p align="left">
  <img src="screenshots/Proje 9 - DiscordClone/Home.png" width="260" alt="Giriş Sayfası" />
  <img src="screenshots/Proje 9 - DiscordClone/Chat.png" width="260" alt="Mesajlaşma Paneli" />
  <img src="screenshots/Proje 9 - DiscordClone/Admin.png" width="260" alt="Yönetici Paneli" />
</p>
