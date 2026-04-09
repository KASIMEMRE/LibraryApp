using LibraryApp.Services;
using System.Data.SqlClient;

namespace LibraryApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var kutuphane = new LibraryService();
            var app = new AppClass(kutuphane);
            app.Run();
        }
    }
}