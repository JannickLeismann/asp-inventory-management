using InventoryManagement.Data;
using InventoryManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
    [Authorize]
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Overview()
        {
            var items = _context.Items.ToList();
            return View(items);
        }

        public IActionResult CreateEdit(int id)
        {
            if(id != 0)
            {
                // Edit
                var item = _context.Items.Find(id);
                return View(item);
            }

            // Create new
            return View();
        }

        [HttpPost]
        public IActionResult CreateEditItem(Item item)
        {
            if(item.Id == 0)
            {
                // Create
                _context.Items.Add(item);
            } else
            {
                // Edit
                _context.Items.Update(item);
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(Overview));
        }

        public IActionResult Delete(int id)
        {
            var item = _context.Items.Find(id);

            if(item != null)
            {
                _context.Items.Remove(item);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Overview));
        }
    }
}
