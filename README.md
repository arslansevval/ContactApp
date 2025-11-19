# ContactApp

## 📌 Proje Hakkında  
ContactApp, şirket içi çalışan (Employee) ve iletişim bilgileri (ContactInfo) takibi için geliştirilmiş bir ASP.NET Core + PostgreSQL + React uygulamasıdır.  
Backend tarafında Entity Framework Core kullanılarak veritabanı işlemleri, JWT ile kimlik doğrulama, FluentValidation ile input doğrulama yapılmaktadır. Frontend tarafında React, MUI (Material UI) ve Vite ile modern bir kullanıcı arayüzü sunmaktadır.

## 🧱 Teknoloji Stack  
- Backend  
  - .NET 10 (Preview) / ASP.NET Core  
  - Entity Framework Core  
  - PostgreSQL  
  - JWT ile kimlik doğrulama  
  - FluentValidation  
- Frontend  
  - React  
  - Vite  
  - Material UI (MUI)  
- Containerization  
  - Docker  
  - Docker Compose

## 🚀 Başlarken

### Gereksinimler  
- Docker & Docker Compose  
- .NET SDK (yerel geliştirme için)  
- Node.js & npm/yarn (yerel frontend çalıştırma için)

### Adım‑Adım Kurulum  
1. Depoyu klonlayın:  
   ```bash
   git clone https://github.com/arslansevval/ContactApp.git
   cd ContactApp
   
2.Ortam değişkenlerini (.env) veya appsettings.json içindeki veritabanı bağlantılarını kontrol edin. 
Örnek:
```bash
"ConnectionStrings": {
  "DefaultConnection": "Host=contactapp-db;Port=5432;Database=ContactAppDb;Username=postgres;Password=1234"
} 
```
3.Docker Compose ile tüm servislere birden başlatın:
```bash
docker compose up -d
```
4.Backend API endpoint'leri http://localhost:5001 portunda erişilebilir olacaktır (Docker Compose yapılandırmasına göre değişebilir).
5.Frontend arayüzü http://localhost:5173 adresinde açın.

6. Kullanıcı giriş bilgileri
```bashs
email: admin
password: admin123

email: user
password:user123
