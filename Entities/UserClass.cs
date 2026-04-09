using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApp.Entities
{
    internal class User
    {
        public string Ad { get;  set; }
        public string Soyad { get;  set; }
        public int Id { get;  set; }
        public int type { get; set; }   

        public virtual int MaxKitapSayisi { get; }

        public List<Book> AldigiKitaplar { get; set; } = new List<Book>();

        public void BilgiYazdir()
        {
            Console.WriteLine("Ad: " + Ad);
            Console.WriteLine("Soyad: " + Soyad);
            Console.WriteLine("ID: " + Id);
        }

    }

}
