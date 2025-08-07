using System.ComponentModel.DataAnnotations;
using System.Linq;
using LogiTrack.Models; 

namespace LogiTrack.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        public DateTime DatePlaced { get; set; } = DateTime.UtcNow;

        public List<InventoryItem> Items { get; set; } = new();

        public void AddItem(InventoryItem item)
        {
            Items.Add(item);
        }

        public void RemoveItem(int itemId)
        {
            var itemToRemove = Items.FirstOrDefault(i => i.ItemId == itemId);
            if (itemToRemove != null)
            {
                Items.Remove(itemToRemove);
            }
        }

        public string GetOrderSummary() =>
            $"Order #{OrderId} for {CustomerName} | Items: {Items.Count} | Placed: {DatePlaced:MM/dd/yyyy}";
    }
}
