using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TPerformance.Models
{
    public partial class Topic
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        [ForeignKey("Customer")]
        public int ? CustomerId { get; set; }
        public Customer Customer { get; set; }

    }
}
