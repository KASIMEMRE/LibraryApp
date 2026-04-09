using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApp.Entities
{
    internal class AdminUser : User
    {
        public override int MaxKitapSayisi => int.MaxValue; // Admin kullanıcılar sınırsız kitap ödünç alabilirler

    }
}
