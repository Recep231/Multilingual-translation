using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartTranslationApp
{
    public partial class Form1 : Form
    {
        private TextBox txtInput, txtOutput;
        private ComboBox cmbFrom, cmbTo, cmbMethod;
        private Button btnTranslate, btnSwap, btnClear, btnCopy;
        private Label lblTitle, lblStatus, lblCharCount;
        private ProgressBar progressBar;

        // Büyük çeviri sözlüğü (200+ kelime/cümle)
        private Dictionary<string, Dictionary<string, string>> translationDB;

        // API cache - daha önce çevrilenleri sakla
        private Dictionary<string, string> apiCache = new Dictionary<string, string>();

        public Form1()
        {
            InitializeTranslationDatabase();
            InitializeComponent();
            LoadLanguages();
            UpdateStatus("✅ Akıllı Çeviri Sistemi Hazır", Color.Green);
        }

        private void InitializeTranslationDatabase()
        {
            translationDB = new Dictionary<string, Dictionary<string, string>>();

            // İngilizce → Türkçe (100+ cümle)
            var enToTr = new Dictionary<string, string>
            {
                // Temel selamlaşmalar
                ["hello"] = "merhaba",
                ["hi"] = "selam",
                ["good morning"] = "günaydın",
                ["good afternoon"] = "tünaydın",
                ["good evening"] = "iyi akşamlar",
                ["good night"] = "iyi geceler",
                ["how are you"] = "nasılsın",
                ["i am fine"] = "iyiyim",
                ["what is your name"] = "adın ne",
                ["my name is"] = "benim adım",
                ["nice to meet you"] = "tanıştığıma memnun oldum",

                // Sorular
                ["where is"] = "nerede",
                ["how much"] = "ne kadar",
                ["what time"] = "saat kaç",
                ["why"] = "niçin",
                ["when"] = "ne zaman",
                ["who"] = "kim",
                ["which"] = "hangi",

                // Günlük ifadeler
                ["thank you"] = "teşekkür ederim",
                ["please"] = "lütfen",
                ["sorry"] = "özür dilerim",
                ["excuse me"] = "affedersiniz",
                ["you are welcome"] = "rica ederim",
                ["i love you"] = "seni seviyorum",
                ["i miss you"] = "seni özledim",
                ["i need help"] = "yardıma ihtiyacım var",
                ["can you help me"] = "bana yardım edebilir misin",

                // Yerler
                ["airport"] = "havaalanı",
                ["station"] = "istasyon",
                ["hotel"] = "otel",
                ["restaurant"] = "restoran",
                ["hospital"] = "hastane",
                ["pharmacy"] = "eczane",
                ["bank"] = "banka",
                ["market"] = "market",

                // Yemek
                ["water"] = "su",
                ["coffee"] = "kahve",
                ["tea"] = "çay",
                ["food"] = "yemek",
                ["breakfast"] = "kahvaltı",
                ["lunch"] = "öğle yemeği",
                ["dinner"] = "akşam yemeği",
                ["bill please"] = "hesap lütfen",

                // Sayılar
                ["one"] = "bir",
                ["two"] = "iki",
                ["three"] = "üç",
                ["four"] = "dört",
                ["five"] = "beş",
                ["six"] = "altı",
                ["seven"] = "yedi",
                ["eight"] = "sekiz",
                ["nine"] = "dokuz",
                ["ten"] = "on",

                // Zaman
                ["today"] = "bugün",
                ["tomorrow"] = "yarın",
                ["yesterday"] = "dün",
                ["now"] = "şimdi",
                ["later"] = "sonra",
                ["morning"] = "sabah",
                ["evening"] = "akşam",
                ["night"] = "gece",

                // Uzun cümleler
                ["where is the nearest hospital"] = "en yakın hastane nerede",
                ["i would like to book a room"] = "bir oda rezerv etmek istiyorum",
                ["how much does this cost"] = "bu ne kadar",
                ["can you speak english"] = "ingilizce konuşabilir misin",
                ["i don't understand"] = "anlamıyorum",
                ["could you repeat that"] = "tekrar edebilir misiniz",
                ["where can i find a taxi"] = "taksi nerede bulabilirim",
                ["what do you recommend"] = "ne önerirsiniz",
                ["is there free wifi here"] = "burada ücretsiz wifi var mı",
                ["i have a reservation"] = "rezervasyonum var",
                ["could i have the menu please"] = "menüyü alabilir miyim lütfen",
                ["when does the museum open"] = "müze ne zaman açılıyor",
                ["which way to the beach"] = "plaja hangi yönden gidilir",
                ["i'm looking for a pharmacy"] = "eczane arıyorum",
                ["do you accept credit cards"] = "kredi kartı kabul ediyor musunuz",
                ["could you call me a taxi"] = "bana bir taksi çağırabilir misiniz",
                ["what is the exchange rate"] = "döviz kuru nedir",
                ["is this seat taken"] = "bu koltuk dolu mu",
                ["could you take a photo"] = "fotoğraf çekebilir misiniz",
                ["i'm allergic to peanuts"] = "yer fıstığına alerjim var"
            };

            translationDB["en"] = new Dictionary<string, string>();
            foreach (var item in enToTr)
            {
                translationDB["en"][item.Key] = item.Value;
            }

            // Türkçe → İngilizce (aynı cümlelerin tersi)
            var trToEn = new Dictionary<string, string>();
            foreach (var item in enToTr)
            {
                trToEn[item.Value] = item.Key;
            }
            translationDB["tr"] = trToEn;

            // Almanca → Türkçe (temel kelimeler)
            translationDB["de"] = new Dictionary<string, string>
            {
                ["hallo"] = "merhaba",
                ["guten morgen"] = "günaydın",
                ["danke"] = "teşekkür ederim",
                ["bitte"] = "lütfen",
                ["entschuldigung"] = "özür dilerim",
                ["ja"] = "evet",
                ["nein"] = "hayır",
                ["wasser"] = "su",
                ["essen"] = "yemek",
                ["hotel"] = "otel",
                ["krankenhaus"] = "hastane"
            };

            // Fransızca → Türkçe
            translationDB["fr"] = new Dictionary<string, string>
            {
                ["bonjour"] = "merhaba",
                ["merci"] = "teşekkür ederim",
                ["s'il vous plaît"] = "lütfen",
                ["excusez-moi"] = "affedersiniz",
                ["oui"] = "evet",
                ["non"] = "hayır",
                ["eau"] = "su",
                ["nourriture"] = "yemek",
                ["hôtel"] = "otel",
                ["hôpital"] = "hastane"
            };

            // İspanyolca → Türkçe
            translationDB["es"] = new Dictionary<string, string>
            {
                ["hola"] = "merhaba",
                ["gracias"] = "teşekkür ederim",
                ["por favor"] = "lütfen",
                ["disculpe"] = "affedersiniz",
                ["sí"] = "evet",
                ["no"] = "hayır",
                ["agua"] = "su",
                ["comida"] = "yemek",
                ["hotel"] = "otel",
                ["hospital"] = "hastane"
            };
        }

        private void InitializeComponent()
        {
            // Form ayarları
            this.Text = "🌍 AKILLI ÇEVİRİ SİSTEMİ";
            this.Size = new Size(800, 600);
            this.BackColor = Color.FromArgb(25, 35, 45);
            this.ForeColor = Color.White;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Arial", 9);

            // Başlık
            lblTitle = new Label
            {
                Text = "🤖 AKILLI FALLBACK ÇEVİRİ SİSTEMİ",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 200, 255),
                Location = new Point(100, 15),
                Size = new Size(600, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Çeviri yöntemi
            Label lblMethod = new Label
            {
                Text = "YÖNTEM:",
                Location = new Point(50, 70),
                Size = new Size(80, 25),
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.LightGray
            };

            cmbMethod = new ComboBox
            {
                Location = new Point(140, 70),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(40, 50, 60),
                ForeColor = Color.White
            };

            cmbMethod.Items.AddRange(new string[] {
                "🔄 OTOMATİK (Önce API, sonra yerel)",
                "🌐 SADECE API (İnternet gerektirir)",
                "💾 SADECE YEREL (İnternet gerekmez)"
            });
            cmbMethod.SelectedIndex = 0;

            // Dil seçimleri
            Label lblFrom = new Label
            {
                Text = "KAYNAK DİL:",
                Location = new Point(50, 110),
                Size = new Size(100, 25),
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.LightGray
            };

            cmbFrom = new ComboBox
            {
                Location = new Point(160, 110),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(40, 50, 60),
                ForeColor = Color.White
            };

            Label lblTo = new Label
            {
                Text = "HEDEF DİL:",
                Location = new Point(400, 110),
                Size = new Size(100, 25),
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.LightGray
            };

            cmbTo = new ComboBox
            {
                Location = new Point(510, 110),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(40, 50, 60),
                ForeColor = Color.White
            };

            // Giriş metin kutusu
            GroupBox gbInput = new GroupBox
            {
                Text = " Çevrilecek Metin ",
                Location = new Point(50, 150),
                Size = new Size(700, 150),
                ForeColor = Color.FromArgb(0, 200, 255),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            txtInput = new TextBox
            {
                Multiline = true,
                Location = new Point(10, 25),
                Size = new Size(680, 90),
                Font = new Font("Arial", 11),
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.FromArgb(30, 40, 50),
                ForeColor = Color.White
            };

            txtInput.TextChanged += (s, e) =>
            {
                lblCharCount.Text = $"Karakter: {txtInput.Text.Length}";
            };

            lblCharCount = new Label
            {
                Text = "Karakter: 0",
                Location = new Point(10, 120),
                Size = new Size(150, 20),
                ForeColor = Color.Gray,
                Font = new Font("Arial", 8)
            };

            gbInput.Controls.Add(txtInput);
            gbInput.Controls.Add(lblCharCount);

            // Çıktı metin kutusu
            GroupBox gbOutput = new GroupBox
            {
                Text = " Çeviri Sonucu ",
                Location = new Point(50, 310),
                Size = new Size(700, 150),
                ForeColor = Color.FromArgb(50, 200, 100),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            txtOutput = new TextBox
            {
                Multiline = true,
                Location = new Point(10, 25),
                Size = new Size(680, 90),
                Font = new Font("Arial", 11),
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                BackColor = Color.FromArgb(30, 40, 50),
                ForeColor = Color.FromArgb(100, 255, 150)
            };

            gbOutput.Controls.Add(txtOutput);

            // Buton paneli
            Panel buttonPanel = new Panel
            {
                Location = new Point(50, 470),
                Size = new Size(700, 50),
                BackColor = Color.Transparent
            };

            btnTranslate = new Button
            {
                Text = "🚀 AKILLI ÇEVİR",
                Location = new Point(0, 0),
                Size = new Size(200, 40),
                Font = new Font("Arial", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 150, 255),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnTranslate.Click += BtnTranslate_Click;

            btnSwap = new Button
            {
                Text = "🔄 DEĞİŞTİR",
                Location = new Point(210, 0),
                Size = new Size(140, 40),
                BackColor = Color.FromArgb(150, 100, 255),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnSwap.Click += BtnSwap_Click;

            btnClear = new Button
            {
                Text = "🗑️ TEMİZLE",
                Location = new Point(360, 0),
                Size = new Size(140, 40),
                BackColor = Color.FromArgb(255, 100, 100),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnClear.Click += BtnClear_Click;

            btnCopy = new Button
            {
                Text = "📋 KOPYALA",
                Location = new Point(510, 0),
                Size = new Size(140, 40),
                BackColor = Color.FromArgb(255, 200, 50),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnCopy.Click += BtnCopy_Click;

            buttonPanel.Controls.AddRange(new Control[]
            {
                btnTranslate, btnSwap, btnClear, btnCopy
            });

            // Progress bar
            progressBar = new ProgressBar
            {
                Location = new Point(50, 525),
                Size = new Size(700, 20),
                Style = ProgressBarStyle.Marquee,
                Visible = false
            };

            // Durum label
            lblStatus = new Label
            {
                Text = "Akıllı sistem hazır - 200+ kelime veritabanı yüklendi",
                Location = new Point(50, 550),
                Size = new Size(700, 25),
                Font = new Font("Arial", 10),
                ForeColor = Color.Green
            };

            // Tüm kontrolleri forma ekle
            this.Controls.AddRange(new Control[]
            {
                lblTitle, lblMethod, cmbMethod,
                lblFrom, cmbFrom, lblTo, cmbTo,
                gbInput, gbOutput,
                buttonPanel, progressBar, lblStatus
            });
        }

        private void LoadLanguages()
        {
            string[] languages = {
                "Türkçe (tr)",
                "İngilizce (en)",
                "Almanca (de)",
                "Fransızca (fr)",
                "İspanyolca (es)"
            };

            foreach (string lang in languages)
            {
                cmbFrom.Items.Add(lang);
                cmbTo.Items.Add(lang);
            }

            cmbFrom.SelectedIndex = 1; // İngilizce
            cmbTo.SelectedIndex = 0;   // Türkçe
        }

        private async void BtnTranslate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtInput.Text))
            {
                UpdateStatus("⚠️ Lütfen metin girin!", Color.Orange);
                return;
            }

            string text = txtInput.Text.Trim().ToLower();
            string sourceLang = GetLangCode(cmbFrom.SelectedItem.ToString());
            string targetLang = GetLangCode(cmbTo.SelectedItem.ToString());
            string method = cmbMethod.SelectedItem.ToString();

            ShowProgress(true);

            try
            {
                string translatedText = "";
                string methodUsed = "";

                if (method.Contains("SADECE YEREL") ||
                   (method.Contains("OTOMATİK") && !CheckInternetConnection()))
                {
                    // Yerel çeviri
                    translatedText = TranslateLocally(text, sourceLang, targetLang);
                    methodUsed = "💾 YEREL VERİTABANI";
                }
                else if (method.Contains("SADECE API") || method.Contains("OTOMATİK"))
                {
                    // Önce API'yi dene
                    UpdateStatus("🌐 API deneniyor...", Color.Cyan);
                    translatedText = await TryTranslateWithAPI(text, sourceLang, targetLang);

                    if (translatedText != text && !translatedText.Contains("[API HATA]"))
                    {
                        methodUsed = "🌐 ÇEVRİMİÇİ API";
                        // API cache'e ekle
                        string cacheKey = $"{sourceLang}|{targetLang}|{text}";
                        apiCache[cacheKey] = translatedText;
                    }
                    else
                    {
                        // API başarısız, yerel çeviriye geç
                        UpdateStatus("API başarısız, yerel çeviri kullanılıyor...", Color.Yellow);
                        translatedText = TranslateLocally(text, sourceLang, targetLang);
                        methodUsed = "💾 YEREL (API başarısız)";
                    }
                }

                // Sonucu göster
                if (!string.IsNullOrEmpty(translatedText) && translatedText != text)
                {
                    txtOutput.Text = $"{translatedText}\n\n[{methodUsed}]";

                    if (methodUsed.Contains("YEREL") && translatedText.Contains("["))
                    {
                        UpdateStatus("⚠️ Yerel veritabanında tam eşleşme yok", Color.Yellow);
                    }
                    else
                    {
                        UpdateStatus($"✅ {methodUsed} ile çevrildi!", Color.Green);
                    }
                }
                else
                {
                    txtOutput.Text = "❌ Çeviri yapılamadı. Lütfen farklı bir metin deneyin.";
                    UpdateStatus("❌ Çeviri başarısız", Color.Red);
                }
            }
            catch (Exception ex)
            {
                txtOutput.Text = $"⚠️ Sistem hatası: {ex.Message}";
                UpdateStatus("❌ Sistem hatası", Color.Red);
            }
            finally
            {
                ShowProgress(false);
            }
        }

        private async Task<string> TryTranslateWithAPI(string text, string sourceLang, string targetLang)
        {
            try
            {
                // Önce cache'e bak
                string cacheKey = $"{sourceLang}|{targetLang}|{text}";
                if (apiCache.ContainsKey(cacheKey))
                {
                    return apiCache[cacheKey] + " [Cache]";
                }

                // RapidAPI üzerinden ücretsiz çeviri API'si (güvenilir)
                string url = $"https://google-translate1.p.rapidapi.com/language/translate/v2";

                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers.Add("X-RapidAPI-Key", "demo_key_123456"); // Demo key
                    client.Headers.Add("X-RapidAPI-Host", "google-translate1.p.rapidapi.com");
                    client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

                    string postData = $"q={Uri.EscapeDataString(text)}&target={targetLang}&source={sourceLang}";

                    string response = await client.UploadStringTaskAsync(url, "POST", postData);

                    // Basit JSON parsing
                    if (response.Contains("\"translatedText\""))
                    {
                        int start = response.IndexOf("\"translatedText\":\"") + 18;
                        int end = response.IndexOf("\"", start);
                        if (end > start)
                        {
                            return response.Substring(start, end - start);
                        }
                    }
                }

                return text + " [API HATA: Format]";
            }
            catch (WebException webEx)
            {
                // API erişim hatası
                return text + " [API HATA: Erişim]";
            }
            catch
            {
                return text + " [API HATA: Genel]";
            }
        }

        private string TranslateLocally(string text, string sourceLang, string targetLang)
        {
            // Önce tam eşleşme ara
            if (translationDB.ContainsKey(sourceLang) && translationDB[sourceLang].ContainsKey(text))
            {
                return translationDB[sourceLang][text];
            }

            // Kelime kelime çeviri
            string[] words = text.Split(new[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> translatedWords = new List<string>();

            foreach (string word in words)
            {
                string translatedWord = word;

                if (translationDB.ContainsKey(sourceLang) && translationDB[sourceLang].ContainsKey(word))
                {
                    translatedWord = translationDB[sourceLang][word];
                }
                else if (sourceLang == "en" && targetLang == "tr")
                {
                    // İngilizce'den Türkçe'ye ek kelime çevirileri
                    translatedWord = TranslateEnglishToTurkish(word);
                }
                else if (sourceLang == "tr" && targetLang == "en")
                {
                    // Türkçe'den İngilizce'ye ek kelime çevirileri
                    translatedWord = TranslateTurkishToEnglish(word);
                }

                translatedWords.Add(translatedWord);
            }

            string result = string.Join(" ", translatedWords);

            // Eğer hiçbir kelime çevrilemediyse
            if (result == text)
            {
                return text + " [Veritabanında eşleşme bulunamadı]";
            }

            return result;
        }

        private string TranslateEnglishToTurkish(string word)
        {
            // Ek İngilizce-Türkçe kelimeler
            Dictionary<string, string> extraDict = new Dictionary<string, string>
            {
                ["the"] = "",
                ["a"] = "bir",
                ["an"] = "bir",
                ["is"] = "",
                ["are"] = "",
                ["am"] = "ben",
                ["you"] = "sen",
                ["he"] = "o",
                ["she"] = "o",
                ["it"] = "o",
                ["we"] = "biz",
                ["they"] = "onlar",
                ["my"] = "benim",
                ["your"] = "senin",
                ["his"] = "onun",
                ["her"] = "onun",
                ["our"] = "bizim",
                ["their"] = "onların",
                ["this"] = "bu",
                ["that"] = "şu",
                ["these"] = "bunlar",
                ["those"] = "şunlar",
                ["here"] = "burada",
                ["there"] = "orada",
                ["and"] = "ve",
                ["but"] = "ama",
                ["or"] = "veya",
                ["because"] = "çünkü",
                ["if"] = "eğer",
                ["very"] = "çok",
                ["too"] = "çok",
                ["so"] = "bu yüzden",
                ["then"] = "sonra",
                ["now"] = "şimdi",
                ["today"] = "bugün",
                ["tomorrow"] = "yarın",
                ["yesterday"] = "dün",
                ["always"] = "her zaman",
                ["never"] = "asla",
                ["sometimes"] = "bazen",
                ["often"] = "sık sık",
                ["usually"] = "genellikle",
                ["maybe"] = "belki",
                ["probably"] = "muhtemelen",
                ["actually"] = "aslında",
                ["really"] = "gerçekten",
                ["only"] = "sadece",
                ["just"] = "sadece",
                ["also"] = "ayrıca",
                ["too"] = "de/da",
                ["again"] = "tekrar",
                ["already"] = "zaten",
                ["almost"] = "neredeyse",
                ["still"] = "hala",
                ["yet"] = "henüz",
                ["even"] = "hatta",
                ["especially"] = "özellikle",
                ["exactly"] = "tam olarak",
                ["exactly"] = "kesinlikle",
                ["absolutely"] = "kesinlikle",
                ["definitely"] = "kesinlikle",
                ["certainly"] = "kesinlikle",
                ["possibly"] = "muhtemelen",
                ["perhaps"] = "belki",
                ["nearly"] = "neredeyse",
                ["hardly"] = "neredeyse hiç",
                ["scarcely"] = "neredeyse hiç",
                ["rarely"] = "nadiren",
                ["seldom"] = "nadiren",
                ["occasionally"] = "ara sıra",
                ["frequently"] = "sıkça",
                ["generally"] = "genellikle",
                ["normally"] = "normalde",
                ["typically"] = "tipik olarak",
                ["basically"] = "temelde",
                ["essentially"] = "esasen",
                ["fundamentally"] = "temelde",
                ["primarily"] = "öncelikle",
                ["mainly"] = "esas olarak",
                ["mostly"] = "çoğunlukla",
                ["partly"] = "kısmen",
                ["slightly"] = "hafifçe",
                ["somewhat"] = "biraz",
                ["quite"] = "oldukça",
                ["rather"] = "oldukça",
                ["fairly"] = "oldukça",
                ["pretty"] = "oldukça",
                ["extremely"] = "son derece",
                ["incredibly"] = "inanılmaz derecede",
                ["remarkably"] = "dikkat çekici şekilde",
                ["significantly"] = "önemli ölçüde",
                ["substantially"] = "önemli ölçüde",
                ["considerably"] = "önemli ölçüde",
                ["sufficiently"] = "yeterince",
                ["adequately"] = "yeterince",
                ["appropriately"] = "uygun şekilde",
                ["properly"] = "düzgün bir şekilde",
                ["correctly"] = "doğru şekilde",
                ["accurately"] = "doğru şekilde",
                ["precisely"] = "tam olarak",
                ["explicitly"] = "açıkça",
                ["clearly"] = "açıkça",
                ["obviously"] = "açıkça",
                ["evidently"] = "açıkça",
                ["apparently"] = "görünüşe göre",
                ["seemingly"] = "görünüşte",
                ["supposedly"] = "sözde",
                ["reportedly"] = "bildirildiğine göre",
                ["allegedly"] = "iddiaya göre",
                ["presumably"] = "muhtemelen",
                ["conceivably"] = "akla yatkın şekilde",
                ["plausibly"] = "makul şekilde",
                ["reasonably"] = "makul şekilde",
                ["logically"] = "mantıksal olarak",
                ["theoretically"] = "teorik olarak",
                ["practically"] = "pratikte",
                ["virtually"] = "neredeyse",
                ["effectively"] = "etkili bir şekilde",
                ["efficiently"] = "verimli şekilde",
                ["productively"] = "üretken şekilde",
                ["successfully"] = "başarılı şekilde",
                ["unsuccessfully"] = "başarısız şekilde",
                ["carefully"] = "dikkatlice",
                ["cautiously"] = "ihtiyatlı şekilde",
                ["prudently"] = "ihtiyatlı şekilde",
                ["wisely"] = "akıllıca",
                ["intelligently"] = "zekice",
                ["cleverly"] = "zekice",
                ["smartly"] = "zekice",
                ["brilliantly"] = "mükemmel şekilde",
                ["excellently"] = "mükemmel şekilde",
                ["superbly"] = "mükemmel şekilde",
                ["magnificently"] = "muhteşem şekilde",
                ["wonderfully"] = "harika şekilde",
                ["beautifully"] = "güzel şekilde",
                ["gorgeously"] = "muhteşem şekilde",
                ["stunningly"] = "şaşırtıcı şekilde",
                ["amazingly"] = "şaşırtıcı şekilde",
                ["astonishingly"] = "şaşırtıcı şekilde",
                ["surprisingly"] = "şaşırtıcı şekilde",
                ["unexpectedly"] = "beklenmedik şekilde",
                ["suddenly"] = "aniden",
                ["abruptly"] = "aniden",
                ["immediately"] = "hemen",
                ["instantly"] = "anında",
                ["promptly"] = "derhal",
                ["quickly"] = "hızlıca",
                ["rapidly"] = "hızlıca",
                ["swiftly"] = "hızlıca",
                ["speedily"] = "hızlıca",
                ["fast"] = "hızlı",
                ["slowly"] = "yavaşça",
                ["gradually"] = "yavaş yavaş",
                ["steadily"] = "istikrarlı şekilde",
                ["constantly"] = "sürekli",
                ["continuously"] = "sürekli",
                ["incessantly"] = "aralıksız",
                ["endlessly"] = "sonsuzca",
                ["eternally"] = "ebedi olarak",
                ["permanently"] = "kalıcı olarak",
                ["temporarily"] = "geçici olarak",
                ["briefly"] = "kısaca",
                ["momentarily"] = "bir anlığına",
                ["shortly"] = "kısa süre sonra",
                ["soon"] = "yakında",
                ["later"] = "daha sonra",
                ["eventually"] = "sonunda",
                ["finally"] = "sonunda",
                ["ultimately"] = "nihayetinde",
                ["initially"] = "başlangıçta",
                ["originally"] = "orijinal olarak",
                ["formerly"] = "eskiden",
                ["previously"] = "önceden",
                ["earlier"] = "daha önce",
                ["before"] = "önce",
                ["after"] = "sonra",
                ["afterward"] = "sonradan",
                ["subsequently"] = "daha sonra",
                ["consequently"] = "sonuç olarak",
                ["therefore"] = "bu nedenle",
                ["thus"] = "böylece",
                ["hence"] = "bu yüzden",
                ["accordingly"] = "buna göre",
                ["meanwhile"] = "bu arada",
                ["simultaneously"] = "aynı anda",
                ["concurrently"] = "eşzamanlı olarak",
                ["together"] = "birlikte",
                ["separately"] = "ayrı ayrı",
                ["individually"] = "bireysel olarak",
                ["collectively"] = "toplu olarak",
                ["jointly"] = "ortaklaşa",
                ["cooperatively"] = "işbirliği içinde",
                ["collaboratively"] = "işbirliği içinde",
                ["independently"] = "bağımsız olarak",
                ["autonomously"] = "özerk olarak",
                ["freely"] = "serbestçe",
                ["voluntarily"] = "gönüllü olarak",
                ["willingly"] = "isteyerek",
                ["reluctantly"] = "isteksizce",
                ["hesitantly"] = "tereddütle",
                ["doubtfully"] = "şüpheyle",
                ["skeptically"] = "şüpheci şekilde",
                ["suspiciously"] = "şüpheli şekilde",
                ["anxiously"] = "endişeyle",
                ["nervously"] = "gergin şekilde",
                ["worriedly"] = "endişeyle",
                ["fearfully"] = "korkuyla",
                ["terrifiedly"] = "dehşet içinde",
                ["horrifiedly"] = "dehşet içinde",
                ["shockingly"] = "şaşırtıcı şekilde",
                ["startlingly"] = "şaşırtıcı şekilde",
                ["alarmingly"] = "alarm verici şekilde",
                ["disturbingly"] = "rahatsız edici şekilde",
                ["troublingly"] = "rahatsız edici şekilde",
                ["upsettingly"] = "üzücü şekilde",
                ["sadly"] = "üzücü şekilde",
                ["unhappily"] = "mutsuzca",
                ["miserably"] = "sefil şekilde",
                ["wretchedly"] = "sefil şekilde",
                ["depressingly"] = "depresif şekilde",
                ["gloomily"] = "kasvetli şekilde",
                ["bleakly"] = "kasvetli şekilde",
                ["drearily"] = "kasvetli şekilde",
                ["dismally"] = "kasvetli şekilde",
                ["cheerlessly"] = "neşesiz şekilde",
                ["joylessly"] = "neşesiz şekilde",
                ["happily"] = "mutlu şekilde",
                ["joyfully"] = "neşeyle",
                ["joyously"] = "neşeyle",
                ["gleefully"] = "neşeyle",
                ["merrily"] = "neşeyle",
                ["cheerfully"] = "neşeyle",
                ["blithely"] = "neşeyle",
                ["jovially"] = "neşeyle",
                ["jubilantly"] = "sevinçle",
                ["exultantly"] = "sevinçle",
                ["triumphantly"] = "zaferle",
                ["victoriously"] = "zaferle",
                ["successfully"] = "başarıyla",
                ["triumphantly"] = "zaferle",
                ["proudly"] = "gururla",
                ["arrogantly"] = "kibirle",
                ["haughtily"] = "kibirle",
                ["conceitedly"] = "kibirle",
                ["smugly"] = "kibirle",
                ["complacent"] = "kayıtsız",
                ["indifferently"] = "kayıtsızca",
                ["apathetically"] = "kayıtsızca",
                ["unconcernedly"] = "kayıtsızca",
                ["nonchalantly"] = "kayıtsızca",
                ["casually"] = "gelişigüzel",
                ["informally"] = "gayri resmi",
                ["formally"] = "resmi olarak",
                ["officially"] = "resmi olarak",
                ["ceremonially"] = "törensel olarak",
                ["ritualistically"] = "ritüelistik olarak",
                ["traditionally"] = "geleneksel olarak",
                ["conventionally"] = "geleneksel olarak",
                ["customarily"] = "geleneksel olarak",
                ["habitually"] = "alışkanlıkla",
                ["regularly"] = "düzenli olarak",
                ["routinely"] = "rutin olarak",
                ["systematically"] = "sistematik olarak",
                ["methodically"] = "yöntemli şekilde",
                ["orderly"] = "düzenli şekilde",
                ["organized"] = "organize",
                ["disorganized"] = "dağınık",
                ["chaotically"] = "kaotik şekilde",
                ["messily"] = "dağınık şekilde",
                ["untidily"] = "dağınık şekilde",
                ["sloppily"] = "dağınık şekilde",
                ["carelessly"] = "dikkatsizce",
                ["recklessly"] = "dikkatsizce",
                ["thoughtlessly"] = "düşüncesizce",
                ["mindlessly"] = "düşüncesizce",
                ["senselessly"] = "anlamsızca",
                ["pointlessly"] = "anlamsızca",
                ["meaninglessly"] = "anlamsızca",
                ["purposelessly"] = "amaçsızca",
                ["aimlessly"] = "amaçsızca",
                ["directionlessly"] = "yönsüzce",
                ["randomly"] = "rastgele",
                ["arbitrarily"] = "keyfi olarak",
                ["capriciously"] = "kaprisli şekilde",
                ["whimsically"] = "kaprisli şekilde",
                ["erratically"] = "düzensiz şekilde",
                ["unpredictably"] = "tahmin edilemez şekilde",
                ["sporadically"] = "seyrek olarak",
                ["intermittently"] = "aralıklı olarak",
                ["occasionally"] = "ara sıra",
                ["infrequently"] = "seyrek olarak",
                ["rarely"] = "nadiren",
                ["seldom"] = "nadiren",
                ["hardly ever"] = "neredeyse hiç",
                ["scarcely ever"] = "neredeyse hiç",
                ["almost never"] = "neredeyse hiç",
                ["never"] = "asla",
                ["always"] = "her zaman",
                ["forever"] = "sonsuza kadar",
                ["eternally"] = "ebedi olarak",
                ["perpetually"] = "sürekli olarak",
                ["incessantly"] = "aralıksız",
                ["unceasingly"] = "durmaksızın",
                ["unendingly"] = "sonu olmadan",
                ["interminably"] = "sonu gelmez şekilde",
                ["endlessly"] = "sonsuzca",
                ["infinitely"] = "sonsuzca",
                ["boundlessly"] = "sınırsızca",
                ["limitlessly"] = "sınırsızca",
                ["unlimitedly"] = "sınırsızca",
                ["restrictedly"] = "sınırlı şekilde",
                ["limitedly"] = "sınırlı şekilde",
                ["finite"] = "sınırlı",
                ["infinite"] = "sonsuz"
            };

            if (extraDict.ContainsKey(word))
                return extraDict[word];

            return word;
        }

        private string TranslateTurkishToEnglish(string word)
        {
            // Ek Türkçe-İngilizce kelimeler
            Dictionary<string, string> extraDict = new Dictionary<string, string>
            {
                ["ve"] = "and",
                ["ama"] = "but",
                ["fakat"] = "but",
                ["ancak"] = "however",
                ["çünkü"] = "because",
                ["eğer"] = "if",
                ["ile"] = "with",
                ["için"] = "for",
                ["kadar"] = "until",
                ["gibi"] = "like",
                ["kadar"] = "as much as",
                ["doğru"] = "true",
                ["yanlış"] = "false",
                ["büyük"] = "big",
                ["küçük"] = "small",
                ["uzun"] = "long",
                ["kısa"] = "short",
                ["geniş"] = "wide",
                ["dar"] = "narrow",
                ["yüksek"] = "high",
                ["alçak"] = "low",
                ["ağır"] = "heavy",
                ["hafif"] = "light",
                ["sıcak"] = "hot",
                ["soğuk"] = "cold",
                ["sert"] = "hard",
                ["yumuşak"] = "soft",
                ["hızlı"] = "fast",
                ["yavaş"] = "slow",
                ["güzel"] = "beautiful",
                ["çirkin"] = "ugly",
                ["iyi"] = "good",
                ["kötü"] = "bad",
                ["yeni"] = "new",
                ["eski"] = "old",
                ["genç"] = "young",
                ["yaşlı"] = "old",
                ["zengin"] = "rich",
                ["fakir"] = "poor",
                ["mutlu"] = "happy",
                ["mutsuz"] = "unhappy",
                ["akıllı"] = "smart",
                ["aptal"] = "stupid",
                ["güçlü"] = "strong",
                ["zayıf"] = "weak",
                ["sağlıklı"] = "healthy",
                ["hasta"] = "sick",
                ["canlı"] = "alive",
                ["ölü"] = "dead",
                ["açık"] = "open",
                ["kapalı"] = "closed",
                ["temiz"] = "clean",
                ["kirli"] = "dirty",
                ["doğru"] = "right",
                ["yanlış"] = "wrong",
                ["kolay"] = "easy",
                ["zor"] = "difficult",
                ["ucuz"] = "cheap",
                ["pahalı"] = "expensive",
                ["boş"] = "empty",
                ["dolu"] = "full",
                ["aç"] = "hungry",
                ["tok"] = "full",
                ["susuz"] = "thirsty",
                ["yorgun"] = "tired",
                ["dinç"] = "energetic",
                ["uyanık"] = "awake",
                ["uykulu"] = "sleepy",
                ["sakin"] = "calm",
                ["sinirli"] = "angry",
                ["korkmuş"] = "scared",
                ["şaşkın"] = "surprised",
                ["üzgün"] = "sad",
                ["neşeli"] = "cheerful",
                ["ciddi"] = "serious",
                ["şaka"] = "joke",
                ["gerçek"] = "real",
                ["hayal"] = "dream",
                ["düş"] = "dream",
                ["rüya"] = "dream",
                ["kâbus"] = "nightmare",
                ["umut"] = "hope",
                ["korku"] = "fear",
                ["sevgi"] = "love",
                ["nefret"] = "hate",
                ["öfke"] = "anger",
                ["mutluluk"] = "happiness",
                ["üzüntü"] = "sadness",
                ["acı"] = "pain",
                ["zevk"] = "pleasure",
                ["başarı"] = "success",
                ["başarısızlık"] = "failure",
                ["zafer"] = "victory",
                ["yenilgi"] = "defeat",
                ["savaş"] = "war",
                ["barış"] = "peace",
                ["özgürlük"] = "freedom",
                ["tutsaklık"] = "captivity",
                ["adalet"] = "justice",
                ["haksızlık"] = "injustice",
                ["doğruluk"] = "truth",
                ["yalan"] = "lie",
                ["gizem"] = "mystery",
                ["sır"] = "secret",
                ["tehlike"] = "danger",
                ["güvenlik"] = "security",
                ["tehlike"] = "danger",
                ["risk"] = "risk",
                ["fırsat"] = "opportunity",
                ["engel"] = "obstacle",
                ["yardım"] = "help",
                ["destek"] = "support",
                ["direnç"] = "resistance",
                ["değişim"] = "change",
                ["istikrar"] = "stability",
                ["gelişme"] = "development",
                ["gerileme"] = "regression",
                ["ilerleme"] = "progress",
                ["durgunluk"] = "stagnation",
                ["büyüme"] = "growth",
                ["küçülme"] = "shrinkage",
                ["artış"] = "increase",
                ["azalış"] = "decrease",
                ["yükseliş"] = "rise",
                ["düşüş"] = "fall",
                ["kazanç"] = "gain",
                ["kayıp"] = "loss",
                ["kâr"] = "profit",
                ["zarar"] = "loss",
                ["gelir"] = "income",
                ["gider"] = "expense",
                ["tasarruf"] = "saving",
                ["israf"] = "waste",
                ["zenginlik"] = "wealth",
                ["yoksulluk"] = "poverty",
                ["refah"] = "prosperity",
                ["sefalet"] = "misery",
                ["lüks"] = "luxury",
                ["yoksunluk"] = "deprivation",
                ["bolluk"] = "abundance",
                ["kıtlık"] = "scarcity",
                ["bereket"] = "plenty",
                ["kuraklık"] = "drought",
                ["yağmur"] = "rain",
                ["kar"] = "snow",
                ["güneş"] = "sun",
                ["ay"] = "moon",
                ["yıldız"] = "star",
                ["gezegen"] = "planet",
                ["galaksi"] = "galaxy",
                ["evren"] = "universe",
                ["uzay"] = "space",
                ["zaman"] = "time",
                ["mekân"] = "space",
                ["boyut"] = "dimension",
                ["renk"] = "color",
                ["şekil"] = "shape",
                ["büyüklük"] = "size",
                ["ağırlık"] = "weight",
                ["hacim"] = "volume",
                ["alan"] = "area",
                ["uzunluk"] = "length",
                ["genişlik"] = "width",
                ["yükseklik"] = "height",
                ["derinlik"] = "depth",
                ["mesafe"] = "distance",
                ["hız"] = "speed",
                ["ivme"] = "acceleration",
                ["kuvvet"] = "force",
                ["enerji"] = "energy",
                ["güç"] = "power",
                ["iş"] = "work",
                ["ısı"] = "heat",
                ["sıcaklık"] = "temperature",
                ["basınç"] = "pressure",
                ["ses"] = "sound",
                ["ışık"] = "light",
                ["renk"] = "color",
                ["koku"] = "smell",
                ["tat"] = "taste",
                ["dokunma"] = "touch",
                ["görme"] = "sight",
                ["işitme"] = "hearing",
                ["koku alma"] = "smell",
                ["tat alma"] = "taste",
                ["dokunma"] = "touch",
                ["duyu"] = "sense",
                ["duygu"] = "emotion",
                ["düşünce"] = "thought",
                ["fikir"] = "idea",
                ["hayal"] = "imagination",
                ["yetenek"] = "ability",
                ["beceri"] = "skill",
                ["deneyim"] = "experience",
                ["bilgi"] = "knowledge",
                ["bilgelik"] = "wisdom",
                ["cehalet"] = "ignorance",
                ["eğitim"] = "education",
                ["öğretim"] = "teaching",
                ["öğrenme"] = "learning",
                ["araştırma"] = "research",
                ["keşif"] = "discovery",
                ["icat"] = "invention",
                ["yenilik"] = "innovation",
                ["geliştirme"] = "development",
                ["ilerleme"] = "progress",
                ["değişim"] = "change",
                ["dönüşüm"] = "transformation",
                ["evrim"] = "evolution",
                ["devrim"] = "revolution",
                ["reform"] = "reform",
                ["iyileştirme"] = "improvement",
                ["bozulma"] = "deterioration",
                ["çöküş"] = "collapse",
                ["yok oluş"] = "extinction",
                ["oluşum"] = "formation",
                ["gelişim"] = "development",
                ["büyüme"] = "growth",
                ["yaşlanma"] = "aging",
                ["ölüm"] = "death",
                ["doğum"] = "birth",
                ["hayat"] = "life",
                ["varoluş"] = "existence",
                ["yokluk"] = "nonexistence",
                ["gerçeklik"] = "reality",
                ["hayal"] = "dream",
                ["rüya"] = "dream",
                ["fantazi"] = "fantasy",
                ["masal"] = "fairy tale",
                ["efsane"] = "legend",
                ["mit"] = "myth",
                ["tarih"] = "history",
                ["geçmiş"] = "past",
                ["şimdi"] = "present",
                ["gelecek"] = "future",
                ["zaman"] = "time",
                ["sonsuzluk"] = "eternity",
                ["an"] = "moment",
                ["saniye"] = "second",
                ["dakika"] = "minute",
                ["saat"] = "hour",
                ["gün"] = "day",
                ["hafta"] = "week",
                ["ay"] = "month",
                ["yıl"] = "year",
                ["yüzyıl"] = "century",
                ["binyıl"] = "millennium",
                ["çağ"] = "age",
                ["devir"] = "era",
                ["dönem"] = "period",
                ["zaman dilimi"] = "time zone",
                ["takvim"] = "calendar",
                ["saat"] = "clock",
                ["zamanlayıcı"] = "timer",
                ["kronometre"] = "chronometer",
                ["zaman ölçer"] = "timekeeper"
            };

            if (extraDict.ContainsKey(word))
                return extraDict[word];

            return word;
        }

        private bool CheckInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private void BtnSwap_Click(object sender, EventArgs e)
        {
            int tempIndex = cmbFrom.SelectedIndex;
            cmbFrom.SelectedIndex = cmbTo.SelectedIndex;
            cmbTo.SelectedIndex = tempIndex;

            string tempText = txtInput.Text;
            txtInput.Text = txtOutput.Text.Replace("[API HATA]", "").Replace("[Veritabanında", "").Trim();
            txtOutput.Clear();

            UpdateStatus("🔄 Diller değiştirildi!", Color.Blue);
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtInput.Clear();
            txtOutput.Clear();
            lblCharCount.Text = "Karakter: 0";
            UpdateStatus("🗑️ Temizlendi!", Color.Gray);
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtOutput.Text))
            {
                Clipboard.SetText(txtOutput.Text.Split('\n')[0]);
                UpdateStatus("📋 Kopyalandı!", Color.Green);
            }
        }

        private string GetLangCode(string selectedText)
        {
            int start = selectedText.LastIndexOf('(') + 1;
            int end = selectedText.LastIndexOf(')');
            if (start > 0 && end > start)
            {
                return selectedText.Substring(start, end - start);
            }
            return "en";
        }

        private void ShowProgress(bool show)
        {
            progressBar.Visible = show;
            btnTranslate.Enabled = !show;
            Application.DoEvents();
        }

        private void UpdateStatus(string message, Color color)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = color;
        }
    }
}