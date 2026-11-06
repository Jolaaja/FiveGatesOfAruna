# Five Gates of Aruna

Prototipe game platformer aksi 2D yang dibuat dengan Unity. Proyek ini mencakup kontrol karakter, pertarungan sederhana, tiga jenis musuh, serta sistem koin dan upgrade damage.

## Menjalankan proyek

1. Tambahkan folder repository ini melalui Unity Hub.
2. Buka scene `Assets/Scenes/Prototype.unity`.
3. Gunakan Unity **2023.1.22f1**, lalu tekan **Play**.

## Kontrol

| Tombol | Fungsi |
| --- | --- |
| `A` / `D` atau panah kiri / kanan | Bergerak |
| `Left Shift` | Berlari |
| `Space` | Melompat |
| `J` atau klik kiri | Menyerang |
| `U` | Membeli upgrade damage jika koin mencukupi |

## Data musuh

Prefab musuh berada di `Assets/Prefabs`, sedangkan konfigurasi statistiknya menggunakan aset `EnemyStats` di `Assets/Data`.

| Musuh | Health | Damage | Kecepatan | Cooldown serangan | Armor | Hadiah koin |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Grunt | 50 | 10 | 2.0 | 1.5 detik | 20% | 5 |
| Brute | 100 | 20 | 1.5 | 1.3 detik | 30% | 10 |
| Stalker | 180 | 35 | 1.0 | 1.0 detik | 40% | 20 |

Musuh memberikan koin saat dikalahkan. Koin dapat digunakan untuk meningkatkan damage pemain. Nilai `cost` pada data musuh belum digunakan untuk sistem pembelian.

## Isi repository

- `Assets/` — scene, script, prefab, data, dan aset visual.
- `Packages/` dan `ProjectSettings/` — konfigurasi proyek Unity.
- `Deliverables/` — Unity package, laporan, workbook, dan screenshot dari Unity Editor.

Build WebGL maupun Windows tidak disertakan.
