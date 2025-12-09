🌍 Multilingual Translation Application

C# Windows Forms ile geliştirilmiş, akıllı fallback mekanizmasına sahip çoklu dil çeviri uygulaması.

✨ Özellikler
🎯 Ana Özellikler

3 farklı çeviri modu:

🔄 Otomatik Mod – İnternet varsa API, yoksa yerel veritabanı kullanılır

🌐 API Modu – Çevrimiçi çeviri servisi

💾 Yerel Mod – İnternet gerektirmez

Desteklenen 5 dil:

🇹🇷 Türkçe

🇬🇧 İngilizce

🇩🇪 Almanca

🇫🇷 Fransızca

🇪🇸 İspanyolca

🧠 Akıllı Sistem

200+ kelime & cümle içeren yerel veritabanı

Tam eşleşme yoksa kelime kelime çeviri

API cache sistemi – tekrar eden çevirileri hatırlar

İnternet kontrolü – online/offline algılama

Karakter sayacı

🎨 Kullanıcı Arayüzü

Modern dark theme

Temiz ve sade tasarım

Durum göstergeleri ve renk kodlamaları

Kullanıcı dostu hata mesajları

🚀 Kurulum
Gereksinimler

Visual Studio 2019 / 2022

.NET Framework 4.7.2+

Windows 10 / 11

Yükleme
git clone https://github.com/Recep231/Multilingual-translation.git


Ya da ZIP indirip çıkartın.

Visual Studio ile açmak için:

SmartTranslationApp.sln dosyasına tıklayın
veya

.csproj dosyasını açın

Çalıştırmak için:
F5 → Start Debugging

📖 Kullanım Kılavuzu
1️⃣ Başlangıç

Uygulamayı aç

Çeviri modunu seç:
🔄 Otomatik | 🌐 API | 💾 Yerel

2️⃣ Çeviri Yapma

Kaynak dili seç

Hedef dili seç

Metni yaz veya yapıştır

🚀 AKILLI ÇEVİR butonuna bas

3️⃣ Ek Özellikler

🔄 DEĞİŞTİR → kaynak ve hedef dili yer değiştirir

🗑️ TEMİZLE → tüm alanları temizler

📋 KOPYALA → sonucu panoya kopyalar

🔧 Teknik Detaylar
📁 Proje Yapısı
SmartTranslationApp/
├── Form1.cs                 # Ana form ve iş mantığı
├── Program.cs               # Giriş noktası
├── Translation Database     # Yerel sözlük
└── API Integration          # Çevrimiçi API bağlantıları

🏗️ Teknoloji Stack

Platform: Windows Forms (.NET Framework)

Dil: C#

API: REST

Veri Yapısı: Dictionary

Cache: In-memory

Fallback: Çok katmanlı sistem (local → API → kelime kelime)

📊 Çeviri Akış Mantığı

Tam cümle eşleşmesi ara

Yoksa kelime kelime çevir

İnternet varsa API’ye bağlan

API başarısız → yerel veritabanına dön

🧪 Örnek Test Senaryoları
🔌 Senaryo 1: İnternet VAR

Girdi: hello how are you
Çıktı: merhaba nasılsın
Durum: ✅ API

📴 Senaryo 2: İnternet YOK

Girdi: where is the hotel
Çıktı: otel nerede
Durum: 💾 Yerel veritabanı

⚠️ Senaryo 3: Karmaşık Cümle

Girdi: i need to find a pharmacy quickly
Çıktı: kelime kelime çeviri
Durum: ⚠️ Word-by-word

🐛 Bilinen Sınırlamalar

Maks 5000 karakter çevirilebilir

API yanıt süresi değişebilir

Nadir kelimelerde hata oluşabilir

Şu anda 5 dil destekleniyor

🔮 Gelecek Güncellemeler

Daha fazla dil

Sesli çeviri

OCR ile görselden çeviri

Mobil versiyon

Cloud Sync

👨‍💻 Geliştirici

Recep Yıldırım
GitHub: @Recep231

Proje: Multilingual Translation Application

📝 Lisans

Bu proje MIT lisansı altındadır.
Detaylar için LICENSE dosyasına bakınız.

🤝 Katkıda Bulunma

Fork'la

Yeni branch aç

Commit at

Push et

Pull Request aç

⭐ Destek

Projeyi beğendiysen yıldız vermeyi unutma! ⭐

📁 Dosyaya ekleme talimatı
git add README.md
git commit -m "docs: Add professional README"
git push origin main
