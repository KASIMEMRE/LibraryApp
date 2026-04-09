using LibraryApp.Entities;
using LibraryApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApp.Interfaces
{
    internal interface ILibraryService 
    {
        void KitapEkle(Book kitap);
        void KitapListele();

        void KullaniciEkle(User kullanici);
        void KullaniciListele();

        void KitapOduncAl(int kitapIndex, User kullanici);
        void KitapIadeEt(int kitapIndex, User kullanici);

        void KitapSil(int kitapIndex, User kullanici);
        void KullaniciSil(int kullaniciIndex, User kullanici);
        User Login(string ad, string soyad);
    }
}
