using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TPerformance.Models
{
    public partial class Customer
    {
        [Key]
        public int Id { get; set; }
        public string  UserNAME { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public int CustomerRole { get; set; }
        public ICollection<Topic> Topics { get; set; }
        public Customer()
        {
            Topics = new HashSet<Topic>();
        }

    }
}
