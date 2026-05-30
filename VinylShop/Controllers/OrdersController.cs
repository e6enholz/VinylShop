using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinylShop.DBContext;

namespace VinylShop.Controllers
{
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // Список всех заказов на доставку
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin") return RedirectToAction("Login", "Account");

            // OrderByDescending по дате
            var deliveries = await _context.Deliveries
                .Include(d => d.User)
                .ThenInclude(u => u.Status) // Добавь это, чтобы в View статус юзера не был null
                .OrderByDescending(d => d.id_delivery)
                .ToListAsync();

            return View(deliveries);
        }

        // Изменение статуса заказа (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, string newStatus)
        {
            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery != null)
            {
                delivery.status_text = newStatus; // Меняем текст статуса
                _context.Update(delivery);
                await _context.SaveChangesAsync(); // Сохраняем в Postgres
            }

            return RedirectToAction(nameof(Index)); // Обновляем страницу заказов
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            // Удаляем связи
            var vinyls = await _context.OrderVinyls.Where(ov => ov.deliveryId == id).ToListAsync();
            var players = await _context.OrderPlayers.Where(op => op.deliveryId == id).ToListAsync();

            _context.OrderVinyls.RemoveRange(vinyls);
            _context.OrderPlayers.RemoveRange(players);

            // Удаляем сам заказ
            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery != null) { _context.Deliveries.Remove(delivery); }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var delivery = await _context.Deliveries
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.id_delivery == id);

            if (delivery == null) return NotFound();

            var vinylItems = await _context.OrderVinyls.Include(ov => ov.Vinyl).Where(ov => ov.deliveryId == id).ToListAsync();
            var playerItems = await _context.OrderPlayers.Include(op => op.Player).Where(op => op.deliveryId == id).ToListAsync();

            ViewBag.VinylItems = vinylItems;
            ViewBag.PlayerItems = playerItems;

            // Считаем сумму и по винилу, и по плеерам
            ViewBag.TotalSum = vinylItems.Sum(i => i.Vinyl?.price ?? 0) + playerItems.Sum(i => i.Player?.price ?? 0);

            return View(delivery);
        }
    }
}