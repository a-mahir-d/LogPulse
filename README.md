# [LogPulse](https://logpulse.ahmetmahirdemirelli.com)

* SignalR ve Dapper tabanlı gerçek zamanlı merkezi log izleme dashboard uygulaması.
* A real-time centralized log monitoring dashboard built with SignalR and Dapper.

---

## 🛠️ Tech Stack / Teknolojiler

### 🇹🇷 Türkçe
### Backend (.NET 10)
* **Dapper:** PostgreSQL (Neon) üzerinde ham SQL operasyonları ve veri eşleme.
* **SignalR:** Token doğrulamalı, çift yönlü canlı log akışı (WebSockets).
* **Background Services:** Otomatik DB şema kontrolü, log simülasyonu ve saatlik veri temizliği (`PeriodicTimer`).
* **JWT Authentication:** Güvenli token tabanlı kimlik doğrulama yönetimi.

### Frontend (Angular 21)
* **Angular Signals:** Yüksek frekanslı veri akışı için reaktif durum yönetimi.
* **Tailwind CSS v4:** Koyu/açık tema uyumlu minimal arayüz tasarımı.
* **RxJS Streams:** Canlı tablolar için bellek korumalı kuyruk yönetimi (`switchMap`, `takeUntil`).

### English
### Backend (.NET 10)
* **Dapper:** Raw SQL operations and lightweight data mapping on PostgreSQL (Neon).
* **SignalR:** Token-authenticated, bi-directional live log streaming (WebSockets).
* **Background Services:** Automated DB schema verification, log simulation, and hourly data cleanup (PeriodicTimer).
* **JWT Authentication:** Secure token-based authentication management.

### Frontend (Angular 21)
* **Angular Signals:** Reactive state management optimized for high-frequency data streams.
* **Tailwind CSS v4:** Minimal user interface design with native dark/light mode support.
* **RxJS Streams:** Memory-safe adaptive queue management (switchMap, takeUntil) for live tables.

---

## ⚙️ Core Logic / Temel Mantık

### 🇹🇷 Türkçe
* **Canlı Akış:** Üretilen loglar SignalR hattı üzerinden sayfayı yenilemeye gerek kalmadan arayüze aktarılır.
* **Bellek Optimizasyonu:** Yüksek hızlı akışlarda tarayıcının kilitlenmesini önlemek için kullanıcı sınırına (0-500) göre eski loglar diziden (`pop`) atılır.
* **Otomatik Durdurma (Auto-Stop):** Hub bağlantısı kesildiğinde (aktif kullanıcı kalmadığında) arka plan simülatörü sunucu kaynaklarını korumak için otomatik durdurulur.
* **Saatlik Temizlik:** Bir background worker her saat başı `TRUNCATE ... RESTART IDENTITY` çalıştırarak veritabanı kotasını korur.

###  English
* **Live Streaming:** Logs stream directly into the UI via SignalR without requiring page refreshes.
* **Memory Protection:** To prevent browser lagging, the frontend limits the array size (0-500) and drops older logs using `pop()`.
* **Resource Optimization (Auto-Stop):** The background simulator automatically stops when no active clients are connected to the SignalR Hub.
* **Hourly Cleanup:** A dedicated background service executes a `TRUNCATE ... RESTART IDENTITY` command every hour to maintain database quotas.
