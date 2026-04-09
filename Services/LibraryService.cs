using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryApp.Entities;
using LibraryApp.Interfaces;
using System.IO;
using System.Text.Json;
using System.Data.SqlClient;


namespace LibraryApp.Services
{
    internal class LibraryService : ILibraryService
    {
        string connectionString = "Server=.\\SQLEXPRESS;Database=LibraryDB;Trusted_Connection=True;";

        private string kitapDosyaYolu = "kitaplar.json";
        private string kullaniciDosyaYolu = "kullanicilar.json";

        private List<Book> kitaplar = new List<Book>();
        private List<User> kullanicilar = new List<User>();

        public void KitapEkle(Book kitap)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "INSERT INTO Books (KitapAd, Yazar, SayfaSayisi, Tur, OduncVerildiMi) VALUES (@ad, @yazar, @sayfa, @tur, 0)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ad", kitap.KitapAdi ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@yazar", kitap.Yazar ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@sayfa", kitap.SayfaSayisi);
                    cmd.Parameters.AddWithValue("@tur", (int)kitap.Tur);
                    cmd.ExecuteNonQuery();
                }
            }

            kitaplar.Add(kitap);
            Console.WriteLine("Kitap eklendi!");
        }

        public void KitapListele()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM Books";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    int i = 1;
                    while (reader.Read())
                    {
                        Console.WriteLine($"{i}. {reader["KitapAd"]} ");
                        i++;
                    }
                }
            }
        }

        public void KullaniciEkle(User kullanici)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Users (Ad, Soyad, UserType) VALUES (@ad, @soyad, @type); SELECT CAST(SCOPE_IDENTITY() AS int);";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ad", kullanici.Ad ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@soyad", kullanici.Soyad ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@type", (int)kullanici.type);
                        var result = cmd.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int newId))
                        {
                            kullanici.Id = newId;
                        }
                        else
                        {
                            Console.WriteLine("Kayıt başarılı ama Id alınamadı.");
                        }
                    }
                }

                kullanicilar.Add(kullanici);
                Console.WriteLine("Kullanıcı eklendi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Veritabanına kaydedilirken hata: " + ex.Message);
            }
        }

        public void KullaniciListele()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Id, Ad, Soyad, UserType FROM Users";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    int i = 1;
                    while (reader.Read())
                    {
                        Console.WriteLine($"{i}. [{reader["Id"]}] {reader["Ad"]} {reader["Soyad"]}");
                        i++;
                    }
                }
            }
        }

        public void KitapOduncAl(int kitapIndex, User kullanici)
        {
            if (kitapIndex < 0 || kitapIndex >= kitaplar.Count)
            {
                Console.WriteLine("Geçersiz seçim!");
                return;
            }

            if (kullanici == null)
            {
                Console.WriteLine("Geçersiz kullanıcı!");
                return;
            }

            if (kullanici.AldigiKitaplar.Count >= kullanici.MaxKitapSayisi)
            {
                Console.WriteLine("Kitap alma limitine ulaştınız!");
                return;
            }

            Book kitap = kitaplar[kitapIndex];

            if (kitap.OduncVerildiMi)
            {
                Console.WriteLine("Bu kitap zaten ödünçte!");
                return;
            }

            kitap.OduncAl(kullanici);
            kullanici.AldigiKitaplar.Add(kitap);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Loans (BookAd, UserId, AlisTarihi) VALUES (@bookAd, @userId, @tarih)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@bookAd", kitap.KitapAdi);
                    cmd.Parameters.AddWithValue("@userId", kullanici.Id);
                    cmd.Parameters.AddWithValue("@tarih", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }

            Console.WriteLine($"{kullanici.Ad} kitabı başarıyla aldı.");
        }

        public void KitapIadeEt(int kitapIndex, User kullanici)
        {
            if (kitapIndex < 0 || kitapIndex >= kitaplar.Count)
            {
                Console.WriteLine("Geçersiz seçim!");
                return;
            }

            if (kullanici == null)
            {
                Console.WriteLine("Geçersiz kullanıcı!");
                return;
            }

            Book kitap = kitaplar[kitapIndex];

            if (kitap.IadeEt(kullanici))
            {
                if (kullanici.AldigiKitaplar.Contains(kitap))
                {
                    kullanici.AldigiKitaplar.Remove(kitap);
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Loans SET IadeTarihi = @tarih WHERE BookAd = @bookAd AND UserId = @userId AND IadeTarihi IS NULL";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ad", kitap.KitapAdi ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@yazar", kitap.Yazar ?? (object)DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }

                Console.WriteLine($"{kullanici.Ad} kitabı başarıyla iade etti.");
            }
            else
            {
                Console.WriteLine("Bu kitabı iade edemezsiniz!");
            }
        }

        public void KitapSil(int kitapIndex, User kullanici)
        {
            if (!(kullanici is AdminUser))
            {
                Console.WriteLine("Bu işlemi sadece admin yapabilir!");
                return;
            }

            if (kitapIndex < 0 || kitapIndex >= kitaplar.Count)
            {
                Console.WriteLine("Geçersiz seçim!");
                return;
            }

            Book kitap = kitaplar[kitapIndex];

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM Books WHERE KitapAd = @ad AND Yazar = @yazar";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ad", kitap.KitapAdi ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@yazar", kitap.Yazar ?? (object)DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }

            kitaplar.RemoveAt(kitapIndex);
            Console.WriteLine("Kitap silindi!");
        }

        public void KullaniciSil(int index, User kullanici)
        {
            if (!(kullanici is AdminUser))
            {
                Console.WriteLine("Bu işlemi sadece admin yapabilir!");
                return;
            }

            if (index < 0 || index >= kullanicilar.Count)
            {
                Console.WriteLine("Geçersiz seçim!");
                return;
            }

            User silinecek = kullanicilar[index];

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM Users WHERE Id = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", silinecek.Id);
                    cmd.ExecuteNonQuery();
                }
            }

            kullanicilar.RemoveAt(index);
            Console.WriteLine("Kullanıcı silindi!");
        }

        public User Login(string ad, string soyad)
        {
            var kullanici = kullanicilar.FirstOrDefault(x => x.Ad == ad && x.Soyad == soyad);

            if (kullanici == null)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Id, Ad, Soyad, UserType FROM Users WHERE Ad = @ad AND Soyad = @soyad";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ad", ad ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@soyad", soyad ?? (object)DBNull.Value);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var u = new User
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Ad = reader["Ad"].ToString(),
                                    Soyad = reader["Soyad"].ToString(),
                                    //type = (UserType)Convert.ToInt32(reader["UserType"])
                                };
                                kullanicilar.Add(u);
                                kullanici = u;
                            }
                        }
                    }
                }
            }

            if (kullanici == null)
            {
                Console.WriteLine("Kullanıcı bulunamadı.");
                return null;
            }

            Console.WriteLine($"Hoşgeldiniz {kullanici.Ad}");
            return kullanici;
        }

    }
}
