using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApp.Entities
{
    internal class PremiumUser : User
    {
        public override int MaxKitapSayisi => 5; // Premium kullanıcılar en fazla 5 kitap ödünç alabilirler
    }
}
