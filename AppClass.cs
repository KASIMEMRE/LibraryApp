using LibraryApp.Entities;
using LibraryApp.Interfaces;
using System;
using System.Collections.Generic;


namespace LibraryApp
{
    internal class AppClass
    {
        private readonly ILibraryService _kutuphane;
        private User _aktifKullanici;

        public AppClass(ILibraryService kutuphane)
        {
            _kutuphane = kutuphane;
        }

        public void Run()
        {
            GirisYap();
            MenuGoster();
        }

        private void GirisYap()
        {
            while (_aktifKullanici == null)
            {
                Console.WriteLine("1 - Giriş Yap");
                Console.WriteLine("2 - Kayıt Ol");
                int secim = int.Parse(Console.ReadLine());

                if (secim == 1)
                {
                    Console.Write("Ad: ");
                    string ad = Console.ReadLine();
                    Console.Write("Soyad: ");
                    string soyad = Console.ReadLine();

                    _aktifKullanici = _kutuphane.Login(ad, soyad);
                }
                else if (secim == 2)
                {
                    KayitOl();
                }
            }
        }

        private void KayitOl()
        {
            Console.Write("Ad: ");
            string ad = Console.ReadLine();
            Console.Write("Soyad: ");
            string soyad = Console.ReadLine();

            int tip;
            while (true)
            {
                Console.WriteLine("Kullanıcı türü:");
                Console.WriteLine("1 - Normal");
                Console.WriteLine("2 - Premium");
                Console.WriteLine("3 - Admin");
                Console.Write("Seçiminiz: ");
                if (int.TryParse(Console.ReadLine(), out tip) && (tip == 1 || tip == 2 || tip == 3))
                    break;
                Console.WriteLine("Geçersiz seçim. Lütfen 1, 2 veya 3 girin.");
            }

            User yeni;
            switch (tip)
            {
                case 1:
                    yeni = new NormalUser();
                    break;
                case 2:
                    yeni = new PremiumUser();
                    break;
                case 3:
                    yeni = new AdminUser();
                    break;
                default:
                    throw new InvalidOperationException("Geçersiz kullanıcı türü.");
            }

            yeni.Ad = ad;
            yeni.Soyad = soyad;

            _kutuphane.KullaniciEkle(yeni);
            _aktifKullanici = yeni;
            Console.WriteLine($"Aktif Kullanıcı: {_aktifKullanici.Ad} ({_aktifKullanici.GetType().Name})");
        }

        private void MenuGoster()
        {
            while (true)
            {
                bool isAdmin = _aktifKullanici is AdminUser;

                Console.WriteLine("\n--- KÜTÜPHANE SİSTEMİ ---");
                Console.WriteLine("1. Kitap Ekle");
                Console.WriteLine("2. Kitapları Listele");
                Console.WriteLine("3. Kullanıcı Ekle");
                Console.WriteLine("4. Kullanıcıları Listele");
                Console.WriteLine("5. Kitap Ödünç Al");
                Console.WriteLine("6. Kitap İade Et");
                Console.WriteLine("7. Çıkış");

                if (isAdmin)
                {
                    Console.WriteLine("8. Kullanıcı Sil");
                    Console.WriteLine("9. Kitap Sil");
                }

                Console.Write("Seçiminiz: ");
                if (!int.TryParse(Console.ReadLine(), out int secim))
                {
                    Console.WriteLine("Geçersiz giriş!");
                    continue;
                }

                switch (secim)
                {
                    case 1: KitapEkle(); break;
                    case 2: _kutuphane.KitapListele(); break;
                    case 3: KullaniciEkle(); break;
                    case 4: _kutuphane.KullaniciListele(); break;
                    case 5: KitapOduncAl(); break;
                    case 6: KitapIadeEt(); break;
                    case 7: Console.WriteLine("Çıkış yapılıyor..."); return;
                    case 8:
                        if (isAdmin) KullaniciSil();
                        else Console.WriteLine("Yetkisiz seçim!");
                        break;
                    case 9:
                        if (isAdmin) KitapSil();
                        else Console.WriteLine("Yetkisiz seçim!");
                        break;
                    default: Console.WriteLine("Geçersiz seçim!"); break;
                }
            }
        }

        private void KitapEkle()
        {
            Console.Write("Kitap adı: ");
            string kitapAd = Console.ReadLine();
            Console.Write("Yazar: ");
            string yazan = Console.ReadLine();
            Console.Write("Sayfa sayısı: ");
            int sayfa = int.Parse(Console.ReadLine());

            Console.WriteLine("Kitap türünü seçiniz:");
            foreach (var value in Enum.GetValues(typeof(BookType)))
                Console.WriteLine($"{(int)value} - {value}");

            int turSecim = int.Parse(Console.ReadLine());
            BookType secilenTur = (BookType)turSecim;

            _kutuphane.KitapEkle(new Book(kitapAd, yazan, sayfa, secilenTur));
        }

        private void KullaniciEkle()
        {
            Console.Write("Ad: ");
            string ad = Console.ReadLine();
            Console.Write("Soyad: ");
            string soyad = Console.ReadLine();

            Console.WriteLine("1 - Normal | 2 - Premium | 3 - Admin");
            int tip = int.Parse(Console.ReadLine());

            User yeni;
            switch (tip)
            {
                case 1:
                    yeni = new NormalUser();
                    break;
                case 2:
                    yeni = new PremiumUser();
                    break;
                case 3:
                    yeni = new AdminUser();
                    break;
                default:
                    Console.WriteLine("Geçersiz seçim!");
                    return;
            }

            yeni.Ad = ad;
            yeni.Soyad = soyad;
            _kutuphane.KullaniciEkle(yeni);
        }

        private void KitapOduncAl()
        {
            _kutuphane.KitapListele();
            Console.Write("Kitap numarası: ");
            int kitapIndex = int.Parse(Console.ReadLine()) - 1;

            _kutuphane.KullaniciListele();
            Console.Write("Kullanıcı numarası: ");
            int kullaniciIndex = int.Parse(Console.ReadLine()) - 1;

            _kutuphane.KitapOduncAl(kitapIndex, _aktifKullanici);
        }

        private void KitapIadeEt()
        {
            _kutuphane.KitapListele();
            Console.Write("İade edilecek kitap numarası: ");
            int kitapIndex = int.Parse(Console.ReadLine()) - 1;

            _kutuphane.KullaniciListele();
            Console.Write("Kullanıcı numarası: ");
            int kullaniciIndex = int.Parse(Console.ReadLine()) - 1;

            _kutuphane.KitapIadeEt(kitapIndex, _aktifKullanici);
        }

        private void KitapSil()
        {
            _kutuphane.KitapListele();
            Console.Write("Silinecek kitap numarası: ");
            int kitapIndex = int.Parse(Console.ReadLine()) - 1;
            _kutuphane.KitapSil(kitapIndex, _aktifKullanici);
        }
        private void KullaniciSil()
        {
            _kutuphane.KullaniciListele();
            Console.Write("Silinecek kullanıcı numarası: ");
            int kullaniciIndex = int.Parse(Console.ReadLine()) - 1;
            _kutuphane.KullaniciSil(kullaniciIndex, _aktifKullanici);
        }
    }
}