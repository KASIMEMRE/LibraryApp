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
        private readonly string connectionString = "Server=.\\SQLEXPRESS;Database=LibraryDB;Trusted_Connection=True;";

        private readonly List<Book> kitaplar = new List<Book>();
        private readonly List<User> kullanicilar = new List<User>();

        #region DB Helpers
        private void ExecuteNonQuery(string sql, Action<SqlCommand> bind = null)
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                bind?.Invoke(cmd);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private T ExecuteScalar<T>(string sql, Action<SqlCommand> bind = null)
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                bind?.Invoke(cmd);
                conn.Open();
                var result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value) return default(T);
                return (T)Convert.ChangeType(result, typeof(T));
            }
        }

        private void ExecuteReader(string sql, Action<SqlCommand> bind, Action<SqlDataReader> readAction)
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                bind?.Invoke(cmd);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        readAction(reader);
                    }
                }
            }
        }
        #endregion

        public void KitapEkle(Book kitap)
        {
            const string sql = "INSERT INTO Books (KitapAd, Yazar, SayfaSayisi, Tur, OduncVerildiMi) VALUES (@ad, @yazar, @sayfa, @tur, 0)";
            ExecuteNonQuery(sql, cmd =>
            {
                cmd.Parameters.AddWithValue("@ad", kitap.KitapAdi ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@yazar", kitap.Yazar ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@sayfa", kitap.SayfaSayisi);
                cmd.Parameters.AddWithValue("@tur", (int)kitap.Tur);
            });

            kitaplar.Add(kitap);
            Console.WriteLine("Kitap eklendi!");
        }

        public void KitapListele()
        {
            const string sql = "SELECT KitapAd FROM Books";
            int i = 1;
            ExecuteReader(sql, null, reader =>
            {
                Console.WriteLine($"{i}. {reader["KitapAd"]}");
                i++;
            });
        }

        public void KullaniciEkle(User kullanici)
        {
            try
            {
                const string sql = "INSERT INTO Users (Ad, Soyad, UserType) VALUES (@ad, @soyad, @type); SELECT CAST(SCOPE_IDENTITY() AS int);";
                var newId = ExecuteScalar<int?>(sql, cmd =>
                {
                    cmd.Parameters.AddWithValue("@ad", kullanici.Ad ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@soyad", kullanici.Soyad ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@type", (int)kullanici.type);
                });

                if (newId.HasValue)
                    kullanici.Id = newId.Value;
                else
                    Console.WriteLine("Kayıt başarılı ama Id alınamadı.");

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
            const string sql = "SELECT Id, Ad, Soyad FROM Users";
            int i = 1;
            ExecuteReader(sql, null, reader =>
            {
                Console.WriteLine($"{i}. [{reader["Id"]}] {reader["Ad"]} {reader["Soyad"]}");
                i++;
            });
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

            var kitap = kitaplar[kitapIndex];

            if (kitap.OduncVerildiMi)
            {
                Console.WriteLine("Bu kitap zaten ödünçte!");
                return;
            }

            kitap.OduncAl(kullanici);
            kullanici.AldigiKitaplar.Add(kitap);

            const string sql = "INSERT INTO Loans (BookAd, UserId, AlisTarihi) VALUES (@bookAd, @userId, @tarih)";
            ExecuteNonQuery(sql, cmd =>
            {
                cmd.Parameters.AddWithValue("@bookAd", kitap.KitapAdi ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@userId", kullanici.Id);
                cmd.Parameters.AddWithValue("@tarih", DateTime.Now);
            });

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

            var kitap = kitaplar[kitapIndex];

            if (!kitap.IadeEt(kullanici))
            {
                Console.WriteLine("Bu kitabı iade edemezsiniz!");
                return;
            }

            if (kullanici.AldigiKitaplar.Contains(kitap))
                kullanici.AldigiKitaplar.Remove(kitap);

            const string sql = "UPDATE Loans SET IadeTarihi = @tarih WHERE BookAd = @bookAd AND UserId = @userId AND IadeTarihi IS NULL";
            ExecuteNonQuery(sql, cmd =>
            {
                cmd.Parameters.AddWithValue("@tarih", DateTime.Now);
                cmd.Parameters.AddWithValue("@bookAd", kitap.KitapAdi ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@userId", kullanici.Id);
            });

            Console.WriteLine($"{kullanici.Ad} kitabı başarıyla iade etti.");
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

            var kitap = kitaplar[kitapIndex];
            const string sql = "DELETE FROM Books WHERE KitapAd = @ad AND Yazar = @yazar";
            ExecuteNonQuery(sql, cmd =>
            {
                cmd.Parameters.AddWithValue("@ad", kitap.KitapAdi ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@yazar", kitap.Yazar ?? (object)DBNull.Value);
            });

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

            var silinecek = kullanicilar[index];
            const string sql = "DELETE FROM Users WHERE Id = @id";
            ExecuteNonQuery(sql, cmd => cmd.Parameters.AddWithValue("@id", silinecek.Id));

            kullanicilar.RemoveAt(index);
            Console.WriteLine("Kullanıcı silindi!");
        }

        public User Login(string ad, string soyad)
        {
            var kullanici = kullanicilar.FirstOrDefault(x => x.Ad == ad && x.Soyad == soyad);
            if (kullanici != null)
            {
                Console.WriteLine($"Hoşgeldiniz {kullanici.Ad}");
                return kullanici;
            }

            const string sql = "SELECT Id, Ad, Soyad, UserType FROM Users WHERE Ad = @ad AND Soyad = @soyad";
            User found = null;
            ExecuteReader(sql, cmd =>
            {
                cmd.Parameters.AddWithValue("@ad", ad ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@soyad", soyad ?? (object)DBNull.Value);
            },
            reader =>
            {
                var u = new User
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Ad = reader["Ad"].ToString(),
                    Soyad = reader["Soyad"].ToString()
                };
                // Eğer User sınıfında UserType enum/alanı varsa aşağıdaki satırı açabilirsiniz:
                // u.type = (UserType)Convert.ToInt32(reader["UserType"]);
                found = u;
            });

            if (found == null)
            {
                Console.WriteLine("Kullanıcı bulunamadı.");
                return null;
            }

            kullanicilar.Add(found);
            Console.WriteLine($"Hoşgeldiniz {found.Ad}");
            return found;
        }
    }
}
