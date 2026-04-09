using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApp.Entities
{
    internal class Loan
    {
        public int Id { get; set; }

        public int BookId { get; set; }
        public int UserId { get; set; }

        public DateTime AlisTarihi { get; set; }
        public DateTime? IadeTarihi { get; set; }
    }
}
