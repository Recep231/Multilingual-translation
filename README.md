🌍 Multilingual Translation Application

C# Windows Forms ile geliştirilmiş, akıllı fallback mekanizmasına sahip çoklu dil çeviri uygulaması.

✨ Özellikler
🎯 Çeviri Modları
Mod	Açıklama
🔄 Otomatik Mod	İnternet varsa API, yoksa yerel veritabanı kullanılır
🌐 API Modu	Çevrimiçi çeviri servisi
💾 Yerel Mod	İnternet gerektirmez
🌐 Desteklenen Diller
Bayrak	Dil
🇹🇷	Türkçe
🇬🇧	İngilizce
🇩🇪	Almanca
🇫🇷	Fransızca
🇪🇸	İspanyolca
🧠 Akıllı Sistem Özellikleri

200+ kelime & cümle içeren yerel veritabanı

Tam eşleşme yoksa kelime kelime çeviri

API Cache – tekrar eden çevirileri kaydeder

Online / Offline algılama

Karakter sayacı

Modern ve sade arayüz

🚀 Kurulum
Gereksinimler

Visual Studio 2019 / 2022

.NET Framework 4.7.2+

Windows 10 / 11

Kurulum Adımları
git clone https://github.com/Recep231/Multilingual-translation.git


Projeyi Visual Studio ile açmak için:

SmartTranslationApp.sln dosyasını çift tıklayın
veya

.csproj ile açın

Çalıştırmak için F5 basın.

📖 Kullanım Kılavuzu
1️⃣ Başlangıç

Uygulamayı aç

Çeviri modunu seç (🔄 / 🌐 / 💾)

2️⃣ Çeviri Yapma

Kaynak dili seç

Hedef dili seç

Metni yaz veya yapıştır

🚀 Akıllı Çevir butonuna tıklayın

3️⃣ Ek Araçlar
Buton	Açıklama
🔄 Değiştir	Kaynak ve hedef dili değiştirir
🗑️ Temizle	Tüm alanları siler
📋 Kopyala	Çeviriyi panoya kopyalar
🔧 Teknik Detaylar
📁 Proje Yapısı
SmartTranslationApp/
├── Form1.cs                 # Ana form ve UI
├── Program.cs               # Uygulama giriş noktası
├── Translation Database     # Yerel sözlük
└── API Integration          # Çevrimiçi API bağlantıları

🏗️ Teknoloji Stack

Windows Forms

C# (.NET Framework)

REST API

Dictionary tabanlı veri yapısı

In-memory cache

Çok katmanlı fallback sistemi

📊 Çeviri Akış Mantığı

Tam cümle eşleşmesi ara

Yoksa kelime kelime çevir

İnternet varsa API’ye bağlan

API başarısızsa → yerel veritabanı

🧪 Test Senaryoları
Senaryo	Girdi	Beklenen Sonuç	Durum
İnternet Var	hello how are you	merhaba nasılsın	🌐 API
İnternet Yok	where is the hotel	otel nerede	💾 Yerel
Karmaşık Cümle	i need to find a pharmacy quickly	kelime kelime çeviri	⚠️ Word-by-word
🐛 Bilinen Sınırlamalar

En fazla 5000 karakter çevirilebilir

API hızına göre gecikme olabilir

Nadir kelimelerde hata oluşabilir

Şu an 5 dil destekleniyor

🔮 Gelecek Güncellemeler

Yeni diller

Sesli çeviri

OCR (resimden metin çeviri)

Mobil sürüm

Cloud senkronizasyonu

👨‍💻 Geliştirici

Recep Yıldırım
GitHub: https://github.com/Recep231

📝 Lisans

MIT Lisansı – LICENSE dosyasına bakabilirsiniz.

🤝 Katkıda Bulunma

Fork'la

Branch oluştur

Commit at

Push et

Pull request aç

⭐ Destek

Projeyi beğendiysen yıldız vermeyi unutma! ⭐

📁 GitHub’a ekleme komutları
git add README.md
git commit -m "docs: Add modern README design"
git push origin main
