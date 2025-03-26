# TADS - Tarım Analiz ve Denetleme Sistemi

TADS, tarım alanlarının yönetimi, sulama ve gübreleme işlemlerinin takibi, verim analizi gibi tarımsal süreçleri dijitalleştiren kapsamlı bir web uygulamasıdır.

## Proje Hakkında

TADS, çiftçilerin ve tarım işletmelerinin tarla/parsel yönetimini kolaylaştırmak, sulama ve gübreleme işlemlerini kayıt altına almak ve verim analizlerini gerçekleştirmek için geliştirilmiş bir sistemdir. Rol tabanlı erişim kontrolü sayesinde farklı yetki seviyelerine sahip kullanıcılar sistemi kullanabilir.

## Özellikler

- **Kullanıcı Yönetimi**: Rol tabanlı erişim kontrolü (superadmin, admin, operator, user)
- **Parsel Yönetimi**: Tarlaların kayıt altına alınması, toprak türü ve ekin türü bilgilerinin tutulması
- **Sulama Analizi**: Parsellere göre sulama kayıtlarının tutulması ve analizi
- **Gübreleme Analizi**: Parsellere göre gübreleme kayıtlarının tutulması ve analizi
- **Verim Analizi**: Sulama ve gübreleme verilerine dayalı verim analizi
- **Harita Entegrasyonu**: Parsellerin coğrafi konumlarının harita üzerinde gösterimi

## Teknolojiler

### Frontend
- **React**: v19.0.0
- **TypeScript**: v4.9.5
- **Material-UI**: v6.4.8
- **Axios**: v1.8.4
- **React Router**: v7.4.0
- **Leaflet**: v1.9.4 (Harita entegrasyonu için)
- **JWT Decode**: v4.0.0 (Kimlik doğrulama için)
- **Date-fns**: v2.30.0 (Tarih işlemleri için)

### Backend
- **ASP.NET Core**: v8.0
- **Entity Framework Core**: v8.0.0
- **MySQL**: Pomelo.EntityFrameworkCore.MySql v8.0.0
- **JWT Authentication**: Microsoft.AspNetCore.Authentication.JwtBearer v8.0.0
- **Identity Framework**: Microsoft.AspNetCore.Identity.EntityFrameworkCore v8.0.0
- **MailKit**: v4.4.0 (E-posta işlemleri için)

## Mimari

Proje, modern web uygulaması mimarisine uygun olarak geliştirilmiştir:

- **Frontend**: React ve TypeScript kullanılarak SPA (Single Page Application) olarak geliştirilmiştir.
- **Backend**: ASP.NET Core Web API kullanılarak RESTful servisler sağlanmıştır.
- **Veritabanı**: MySQL veritabanı kullanılmıştır.
- **Kimlik Doğrulama**: JWT tabanlı kimlik doğrulama sistemi kullanılmıştır.
- **API İletişimi**: Axios kütüphanesi ile frontend-backend iletişimi sağlanmıştır.

## Kurulum

### Gereksinimler
- Node.js (v16 veya üzeri)
- .NET 8.0 SDK
- MySQL Server

### Frontend Kurulumu
```bash
cd tads-client
npm install
npm start
```

### Backend Kurulumu
```bash
cd TADS.API
dotnet restore
dotnet run
```

## Kullanım

1. Sisteme giriş yapın (E-posta ve şifre ile)
2. Sidebar menüsünden ilgili modülü seçin
3. Parsel yönetimi sayfasından parselleri yönetin
4. Sulama ve Gübreleme Analizi sayfasından sulama ve gübreleme kayıtlarını yönetin
5. Verim Analizi sekmesinden verim analizlerini görüntüleyin

## Geliştirme Notları

- Frontend geliştirmesi için Visual Studio Code önerilir
- Backend geliştirmesi için Visual Studio 2022 önerilir
- API testleri için Swagger UI kullanılabilir (http://localhost:5001/swagger)
- Veritabanı değişiklikleri için Entity Framework Core Migration kullanılmıştır
