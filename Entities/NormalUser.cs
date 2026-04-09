using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApp.Entities
{
    internal class NormalUser : User
    {
        public override int MaxKitapSayisi => 2; // Normal kullanıcılar en fazla 2 kitap ödünç alabilirler

    }
}
