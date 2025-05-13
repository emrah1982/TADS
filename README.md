# TADS - Tarım Analiz ve Denetleme Sistemi

TADS, tarım alanlarının yönetimi, sulama ve gübreleme işlemlerinin takibi, verim analizi gibi tarımsal süreçleri dijitalleştiren kapsamlı bir web uygulamasıdır.

## Proje Hakkında

TADS, çiftçilerin ve tarım işletmelerinin tarla/parsel yönetimini kolaylaştırmak, sulama ve gübreleme işlemlerini kayıt altına almak ve verim analizlerini gerçekleştirmek için geliştirilmiş bir sistemdir. Rol tabanlı erişim kontrolü sayesinde farklı yetki seviyelerine sahip kullanıcılar sistemi kullanabilir.

## Teknolojiler ve Paketler

### Frontend
- **React**: v19.0.0 - UI bileşenlerini ve kullanıcı arayüzünü oluşturmak için kullanılan JavaScript kütüphanesi
- **Material-UI**: v7.0.0 - Google'ın Material Design ilkelerine dayalı React bileşen kütüphanesi
  - **@mui/icons-material**: v7.0.0 - Material Design ikonları
  - **@mui/x-date-pickers**: v6.20.2 - Gelişmiş tarih seçici bileşenleri
- **React Router**: v7.4.0 - React uygulamalarında sayfa yönlendirmesi için kullanılan kütüphane
- **Axios**: v1.8.4 - HTTP istekleri yapmak için kullanılan Promise tabanlı HTTP istemcisi
- **SignalR**: v8.0.7 - Gerçek zamanlı web uygulamaları için kullanılan Microsoft kütüphanesi
  - **@microsoft/signalr**: Canlı kamera ve drone görüntülerini gerçek zamanlı işlemek için
- **Harita İşlevleri**:
  - **Leaflet**: v1.9.4 - Etkileşimli haritalar için açık kaynaklı JavaScript kütüphanesi
  - **react-leaflet**: v5.0.0 - Leaflet'i React ile kullanmak için
  - **leaflet-draw**: v1.0.4 - Harita üzerinde çizim işlemleri için
  - **@turf/turf**: v7.2.0 - Coğrafi analiz işlemleri için
- **Veri İşleme ve Görselleştirme**:
  - **recharts**: v2.15.1 - React tabanlı grafik ve veri görselleştirme kütüphanesi
  - **date-fns**: v2.30.0 - JavaScript tarih yönetimi kütüphanesi
  - **jspdf**: v3.0.1 ve **jspdf-autotable**: v5.0.2 - PDF rapor oluşturma
- **Kullanıcı Arayüzü İyileştirmeleri**:
  - **notistack**: v3.0.1 - Toast bildirimleri için kütüphane
  - **goober**: v2.1.16 - CSS-in-JS kütüphanesi

### Backend
- **ASP.NET Core**: v9.0 - Microsoft'un çapraz platform web uygulama geliştirme framework'ü
- **Entity Framework Core**: v8.0.3 - Microsoft'un ORM (Object-Relational Mapping) framework'ü
  - **Microsoft.EntityFrameworkCore**: v8.0.3 - Core ORM kütüphanesi
  - **Microsoft.EntityFrameworkCore.Tools**: v8.0.3 - Migration ve veritabanı işlemleri için araçlar
- **Pomelo.EntityFrameworkCore.MySql**: v8.0.0 - MySQL veritabanı entegrasyonu için EF Core sağlayıcısı
- **Kimlik Doğrulama ve Yetkilendirme**:
  - **Microsoft.AspNetCore.Authentication.JwtBearer**: v8.0.3 - JWT tabanlı kimlik doğrulama
  - **Microsoft.AspNetCore.Identity.EntityFrameworkCore**: v8.0.3 - Kullanıcı yönetimi ve rol tabanlı yetkilendirme
- **Gerçek Zamanlı İletişim**:
  - **Microsoft.AspNetCore.SignalR**: v1.2.0 - Gerçek zamanlı web uygulamaları geliştirmek için
- **API Geliştirme ve Dokümantasyon**:
  - **Microsoft.AspNetCore.OpenApi**: v9.0.1
  - **Swashbuckle.AspNetCore**: v8.0.0 - OpenAPI (Swagger) entegrasyonu ve API dokümantasyonu için
- **Veri Haritalama**:
  - **AutoMapper.Extensions.Microsoft.DependencyInjection**: v12.0.1 - Nesneler arası veri haritalama

### Yapay Zeka Modülü
- **FastAPI**: Web API oluşturmak için Python web framework'ü
- **YOLO11**: Bitki hastalıklarını tespit etmek için derin öğrenme modeli
- **OpenCV (cv2)**: Görüntü işleme kütüphanesi
- **NumPy**: Sayısal hesaplamalar için Python kütüphanesi
- **Pillow (PIL)**: Python görüntü işleme kütüphanesi
- **Pydantic**: Veri doğrulama ve ayarlar yönetimi için
- **uvicorn**: ASGI web sunucusu

## Mimari

Proje, modern web uygulaması mimarisine uygun olarak geliştirilmiştir:

- **Frontend**: React ve JavaScript kullanılarak SPA (Single Page Application) olarak geliştirilmiştir.
- **Backend**: ASP.NET Core Web API kullanılarak RESTful servisler sağlanmıştır.
- **Veritabanı**: MySQL veritabanı kullanılmıştır.
- **Kimlik Doğrulama**: JWT tabanlı kimlik doğrulama sistemi kullanılmıştır.
- **Gerçek Zamanlı İletişim**: SignalR ile canlı veri akışı sağlanmıştır.
- **Yapay Zeka Entegrasyonu**: FastAPI ve YOLOv8 kullanılarak bitki hastalıklarının tespiti yapılmıştır.

## Sistem Bileşenleri

### 1. Kullanıcı Yönetimi
- **Rol Tabanlı Erişim Kontrolü**: superadmin, admin, operator, user rolleri
- **Kimlik Doğrulama**: JWT token tabanlı güvenli giriş sistemi
- **Kullanıcı Profili**: Kullanıcı bilgilerini düzenleme ve şifre değiştirme

### 2. Tarla/Parsel Yönetimi
- **Parsel Kaydı**: Tarlaların kayıt altına alınması
- **Toprak Türü ve Ekin Türü**: Parsele özgü toprak ve ekin bilgileri
- **Haritada Görüntüleme**: Parsellerin coğrafi konumlarının harita üzerinde gösterimi

### 3. Sulama ve Gübreleme Analizi
- **Sulama Kayıtları**: Parsellere göre sulama işlemlerinin takibi
- **Gübreleme Kayıtları**: Parsellere göre gübreleme işlemlerinin kaydı
- **Analiz Raporları**: Sulama ve gübreleme verilerine dayalı analiz raporları

### 4. Bitki Sağlığı İzleme
- **Drone Görüntü İşleme**: YOLOv8 modeli ile bitki hastalıklarının tespiti
- **Canlı Kamera**: Gerçek zamanlı görüntü işleme ve hastalık tespiti
- **Harita Entegrasyonu**: Hastalıklı bölgelerin harita üzerinde işaretlenmesi

### 5. Verim Analizi
- **Verim Tahminleri**: Toprak, sulama ve gübreleme verilerine dayalı verim tahminleri
- **İstatistiksel Raporlar**: Geçmiş verim verileri ve karşılaştırmalı analizler

## Kurulum

### Gereksinimler
- Node.js (v16 veya üzeri)
- .NET 9.0 SDK
- MySQL Server
- Python 3.9+ (Yapay Zeka modülü için)

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

### Yapay Zeka Modülü Kurulumu
```bash
cd tads_ai_modules/prediction
pip install -r requirements.txt
python tads_prediction_service.py
```

## Kullanım

1. Sisteme giriş yapın (Kullanıcı adı ve şifre ile)
2. Sidebar menüsünden ilgili modülü seçin
3. Parsel yönetimi sayfasından parselleri yönetin
4. Sulama ve Gübreleme Analizi sayfasından sulama ve gübreleme kayıtlarını yönetin
5. Canlı Kamera sayfasından bitki hastalıklarını gerçek zamanlı tespit edin
6. Verim Analizi sekmesinden verim analizlerini görüntüleyin

## Geliştirme Notları

- Frontend geliştirmesi için Visual Studio Code önerilir
- Backend geliştirmesi için Visual Studio 2022 önerilir
- API testleri için Swagger UI kullanılabilir (http://localhost:5000/swagger)
- Veritabanı değişiklikleri için Entity Framework Core Migration kullanılmıştır
