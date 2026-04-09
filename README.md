# 📚 Library Management System | Kütüphane Yönetim Sistemi

> 🚀 A console-based **Library Management System** built with **C# / .NET**, demonstrating real-world backend architecture, OOP principles, and database integration.
> 🚀 C# / .NET ile geliştirilmiş, gerçek dünya backend mimarisini, OOP prensiplerini ve veritabanı entegrasyonunu gösteren konsol tabanlı bir kütüphane yönetim sistemi.

---

## ✨ Features | Özellikler

### 👤 User Management | Kullanıcı Yönetimi

* Normal, Premium ve Admin kullanıcı tipleri
* Rol bazlı yetkilendirme ve kitap alma limitleri

### 📖 Book Management | Kitap Yönetimi

* Kitap ekleme, listeleme ve silme
* Enum ile kategori (BookType) yönetimi

### 🔄 Loan System (Core Feature) | Ödünç Sistemi (Ana Özellik)

* Hangi kullanıcının hangi kitabı aldığı takip edilir
* Ödünç alma ve iade işlemleri
* Basit flag yerine **ilişkisel veri modeli** kullanılır

### 🛡️ Authorization | Yetkilendirme

* Admin’e özel işlemler (kullanıcı/kitap silme)
* Kullanıcı tipine göre kitap alma sınırları

### 🗄️ Database Integration | Veritabanı Entegrasyonu

* SQL Server kullanımı
* ADO.NET ile veri işlemleri
* İlişkisel tablo yapısı (Users, Books, Loans)

---

## 🧠 Architecture | Mimari Yapı

```text
Presentation Layer  →  AppClass (Console UI)
Business Layer      →  Services (LibraryService)
Abstraction Layer   →  Interfaces (ILibraryService)
Data Layer          →  SQL Server (ADO.NET)
Entities            →  Book, User, Loan
```

---

## 🧩 Object-Oriented Design | Nesne Yönelimli Tasarım

* ✅ Encapsulation (Kapsülleme)
* ✅ Inheritance (Kalıtım)
* ✅ Polymorphism (Çok Biçimlilik)
* ✅ Abstraction (Soyutlama)

---

## 🔗 Database Design | Veritabanı Tasarımı

### 📘 Books

* Id
* KitapAd
* Yazar
* SayfaSayisi
* Tur

### 👤 Users

* Id
* Ad
* Soyad
* UserType

### 🔄 Loans

* Id
* BookId
* UserId
* AlisTarihi
* IadeTarihi

---

## ⚙️ Technologies Used | Kullanılan Teknolojiler

* 💻 C#
* ⚙️ .NET (Console Application)
* 🧠 OOP
* 🗄️ SQL Server
* 🔌 ADO.NET
* 🧱 Layered Architecture
* 🔗 Interface-based design

---

## 🚀 Getting Started | Kurulum

1. Repoyu klonla
2. SQL Server bağlantı string’ini ayarla
3. Veritabanı tablolarını oluştur (Books, Users, Loans)
4. Uygulamayı çalıştır

---

## 🎯 Project Purpose | Projenin Amacı

Bu proje:

* Backend geliştirme mantığını öğrenmek
* Gerçek dünya sistem tasarımını anlamak
* Veritabanı odaklı uygulama geliştirmek
* OOP prensiplerini uygulamak

amacıyla geliştirilmiştir.

---

## 👨‍💻 Author

**Kasım Emre Şekersoy**

---

> 💡 This project represents the transition from beginner-level coding to a real backend development mindset.
> 💡 Bu proje, başlangıç seviyesinden gerçek backend geliştirme seviyesine geçişi temsil eder.
