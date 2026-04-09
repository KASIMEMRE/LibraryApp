using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApp.Entities
{
    internal class Book
    {
        public string KitapAdi { get; private set; }
        public string Yazar { get; private set; }
        public int SayfaSayisi { get; private set; }
        public BookType Tur { get; private set; }


        // Ödünç durumu ve hangi kullanıcıda olduğunu saklayacağız
        public bool OduncVerildiMi { get; private set; } = false;
        public User OduncAlanKullanici { get; private set; } = null;

        public Book() { }

        // Constructor ekleyelim
        public Book(string kitapAdi, string yazar, int sayfaSayisi, BookType tur)
        {
            KitapAdi = kitapAdi;
            Yazar = yazar;
            SayfaSayisi = sayfaSayisi;
            Tur = tur;
        }

        public void BilgiYazdir()
        {
            Console.WriteLine($"Kitap Adı: {KitapAdi}");
            Console.WriteLine($"Yazar: {Yazar}");
            Console.WriteLine($"Sayfa Sayısı: {SayfaSayisi}");
            Console.WriteLine($"Tür: {Tur}");
            Console.WriteLine($"Ödünç Durumu: {(OduncVerildiMi ? $"Ödünçte ({OduncAlanKullanici.Ad})" : "Mevcut")}");
        }

        // Kitap ödünç alma
        public bool OduncAl(User kullanici)
        {
            if (!OduncVerildiMi)
            {
                OduncVerildiMi = true;
                OduncAlanKullanici = kullanici;
                Console.WriteLine($"{KitapAdi} kitabı {kullanici.Ad} tarafından ödünç alındı.");
                return true;
            }
            Console.WriteLine($"{KitapAdi} zaten ödünçte!");
            return false;
        }

        // Kitap iade etme
        public bool IadeEt(User kullanici)
        {
            if (OduncVerildiMi && OduncAlanKullanici == kullanici)
            {
                OduncVerildiMi = false;
                OduncAlanKullanici = null;
                Console.WriteLine($"{KitapAdi} kitabı iade edildi.");
                return true;
            }
            Console.WriteLine("Bu kitabı siz almamışsınız veya zaten iade edilmiş!");
            return false;
        }
    }
} 

