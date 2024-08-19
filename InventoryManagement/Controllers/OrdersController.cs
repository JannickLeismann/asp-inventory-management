using InventoryManagement.Data;
using Microsoft.AspNetCore.Mvc;
using InventoryManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace InventoryManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Overview()
        {
            var orders = _context.Orders.ToList();
            return View(orders);
        }

        public IActionResult Delete(int id)
        {
            var order = _context.Orders.Find(id);

            // Delete order items

            var orderItems = _context.OrderItems
                .Where(oi => oi.OrderId == id)
                .ToList();

            _context.OrderItems.RemoveRange(orderItems);

            _context.Orders.Remove(order);

            _context.SaveChanges();

            return RedirectToAction(nameof(Overview));
        }

        public IActionResult CreateEdit(int id)
        {
            if(id != 0)
            {
                // Edit
                ViewBag.AvailableItems = _context.Items
                    .Where(i => i.Stock > 0)
                    .ToList();

                var order = _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                    .SingleOrDefault(o => o.Id == id);


                return View(order);
            }

            return View(new Order());
        }

        [HttpPost]
        public IActionResult CreateEdit(Order order)
        {
            if(order.Id == 0)
            {
                // Create new
                _context.Orders.Add(order);
            } else
            {
                // Update existing one
                _context.Orders.Update(order);
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(Overview));
        }

        [HttpPost]
        public IActionResult AddOrderItem(int orderId, int itemId, int quantity)
        {
            var order = _context.Orders.Find(orderId);

            if(order == null)
            {
                return NotFound();
            }

            var item = _context.Items.Find(itemId);

            int quantityToAdd = Math.Min(quantity, item.Stock);

            item.Stock -= quantityToAdd;

            var newOrderItem = new OrderItem
            {
                OrderId = orderId,
                ItemId = itemId,
                Quantity = quantityToAdd
            };

            _context.OrderItems.Add(newOrderItem);
            _context.SaveChanges();

            return RedirectToAction(nameof(CreateEdit), new { id = orderId });
        }
    }
}
