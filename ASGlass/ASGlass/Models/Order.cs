using ASGlass.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ASGlass.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }
        public int? ProductId { get; set; }
        [Required]
        [StringLength(maximumLength: 50)]
        public string FullName { get; set; }
        [Required]
        [StringLength(maximumLength: 100)]
        public string Email { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public DateTime CreatedAt { get; set; }
        public OrderStatus Status { get; set; }
        public double Price { get; set; }
        public int OrderNumber { get; set; }

        public int Count { get; set; }
        public double? Uzunluq { get; set; }
        public double? En { get; set; }
        public double? Diametr { get; set; }
        public string Shape { get; set; }
        public string Color { get; set; }
        public string Polish { get; set; }
        public string Corner { get; set; }
        public string Thickness { get; set; }
        public bool? IsAccessory { get; set; }
        public AppUser AppUser { get; set; }
        public Product Product { get; set; }

    }
}
