using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Today;

        // Navigation Property
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
