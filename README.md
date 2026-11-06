# Q-Ledger: QA Document Ledger & Quality Gatekeeper System

**Q-Ledger** adalah sistem berbasis web *Quality Assurance Document Ledger & Quality Gatekeeper* untuk industri manufaktur elektronik dan perakitan presisi tinggi. Aplikasi ini mendigitalkan seluruh pencatatan logbook manual, menegakkan disiplin gerbang mutu (*gatekeeper validation*), menyediakan visualisasi antrean *Andon* real-time, mengelola investigasi cacat mendalam menggunakan analisis 5-Why, serta menyediakan buku besar mutu terintegrasi dengan kemampuan ekspor data Excel audit.

---

## 🚀 Fitur Utama

1. **Serah Terima MO & Validasi Gatekeeper (`/Handover`)**
   - Pemindaian barcode Nomor *Manufacturing Order* (MO).
   - Validasi otomatis 3 pintu:
     - **Cek Duplikasi MO:** Mencegah pendaftaran ganda sebelum lot sebelumnya selesai.
     - **Checklist Mesin Harian:** Memastikan mesin produksi telah lolos inspeksi harian sebelum hasil part diterima.
     - **Line Defect Lockout (NRS Guard):** Menolak serah terima secara otomatis jika lini bersangkutan sedang memiliki tiket cacat terbuka (*Open Defect*).
   - Penandaan dokumen mutu khusus (*Document Flags*): `ARI`, `NRS`, `IFC`, `RRS`, `EFR`, `SPLIT`, dan `TOPUP`.

2. **OQA Dashboard & TV Andon Real-Time (`/Queue`)**
   - Visualisasi antrean masuk berbasis prinsip FIFO (*First In First Out*).
   - Kartu antrean beridentitas warna lini manufaktur (*Switch 1* warna Amber `#b8740c`, *Relay 1* warna Forest Green `#1f6645`, *Relay 2* warna Steel Blue `#2b5c8f`).
   - Indikator lot yang sedang aktif di meja inspeksi (*In Progress*).
   - Sinkronisasi instan antar-perangkat menggunakan **ASP.NET Core SignalR** tanpa perlu me-refresh peramban.

3. **Pos Kerja Inspeksi QA (`/Inspection`)**
   - Pencatatan otomatis waktu mulai (*StartTime*) dan durasi inspeksi per lot.
   - Formulir putusan mutu AQL:
     - **PASS (OK):** Status ditutup menjadi `Finish`, part siap dikirim ke gudang barang jadi (*Finished Goods*).
     - **HOLD (NG):** Lot ditahan dan sistem secara otomatis menerbitkan tiket ketidaksesuaian mutu (*NRS Ticket*).

4. **Pengendalian Defect & Investigasi 5-Why (`/Nrs`)**
   - Analisis akar penyebab masalah (*Root Cause Analysis*) standar industri:
     - **Pilar Occurrence:** 5 tingkat "Mengapa" menelusuri penyebab fisik mesin hingga kelemahan sistematis.
     - **Pilar Outflow:** 3 tingkat "Mengapa" menelusuri kegagalan sistem deteksi lini produksi.
   - Tindakan Karantina Darurat (*Containment Action*) & Tindakan Pencegahan Permanen (*Countermeasure*).
   - Keputusan Disposisi Fisik: `REWORK` (dikerjakan ulang), `SCRAP` (diafkir), atau `SPECIAL RELEASE` (dispensasi khusus).
   - *Supervisor Verification & Approval* untuk membuka kembali status blokir lini produksi.

5. **OQA Ledger & Ekspor Data Excel (`/Ledger`)**
   - Buku besar transaksi mutu digital (*Single Source of Truth*).
   - Penelusuran menyeluruh (*Traceability*) dari Nomor MO, Kode Part, Lot, Qty, Tanggal Masuk, Durasi Pengujian, Nama Inspektor, hingga Putusan Akhir.
   - Filter interaktif berdasarkan rentang tanggal, lini, status, dan jenis dokumen.
   - Ekspor laporan berformat spreadsheet resmi (`.xlsx`) secara instan melalui **ClosedXML**.

6. **Anti-Slop Industrial UI Design**
   - Mengikuti kaidah desain profesional manufaktur (bebas dari efek visual generik AI, shadow mengambang berlebih, maupun sudut membulat ekstrem).
   - Sidebar navigasi dinamis yang dapat dibuka-tutup (*collapsible*) dengan penyimpanan preferensi di `localStorage`.

---

## 🛠️ Tech Stack

- **Backend / Framework:** C# ASP.NET Core 8.0 MVC
- **Database ORM:** Entity Framework Core 8.0 (Code-First)
- **Database Engine:** Microsoft SQL Server
- **Real-Time Engine:** ASP.NET Core SignalR
- **Excel Generator:** ClosedXML (v0.105.1)
- **Frontend / Styling:** HTML5, CSS3 Custom (Anti-Slop Industrial Design), Bootstrap 5, FontAwesome 6, SweetAlert2, jQuery

---

## ⚙️ Panduan Menjalankan Sistem

### 1. Prasyarat
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Microsoft SQL Server (LocalDB atau SQL Server Instance)

### 2. Konfigurasi Database
Pastikan *connection string* di file `appsettings.json` telah mengarah ke server SQL Server lokal Anda:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=QLedgerDB;Integrated Security=SSPI;TrustServerCertificate=True;"
}
```

### 3. Migrasi Database & Seeding Data
Buka terminal di root folder project lalu jalankan:
```powershell
dotnet ef database update
```
*(Sistem akan secara otomatis membuat database `QLedgerDB`, seluruh relasi tabel, data master lini/produk/pengguna, dan data transaksi awal).*

### 4. Jalankan Aplikasi
```powershell
dotnet run
```
Buka browser pada URL yang ditampilkan di terminal (default: `http://localhost:5049` atau `http://localhost:5055`).

---

## 📄 Lisensi
Dikembangkan untuk keperluan personal project & portofolio profesional Quality Assurance System.
