using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinylShop.DBContext;
using VinylShop.Models;

namespace VinylShop.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        // Главная страница профиля (Личный кабинет)
        // Главная страница профиля (Личный кабинет)
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var user = await _context.Users.Include(u => u.Status).FirstOrDefaultAsync(u => u.id_user == userId);
            var userDeliveries = await _context.Deliveries.Where(d => d.userId == userId).ToListAsync();

            // 1. Подтягиваем ВСЕ связи товаров для заказов этого пользователя
            var orderVinyls = await _context.OrderVinyls
                .Include(ov => ov.Vinyl)
                .Where(ov => ov.Delivery!.userId == userId)
                .ToListAsync();

            var orderPlayers = await _context.OrderPlayers
                .Include(op => op.Player)
                .Where(op => op.Delivery!.userId == userId)
                .ToListAsync();

            // --- НАКОПИТЕЛЬНАЯ СИСТЕМА СКИДОК ---
            decimal totalSpent = 0;
            var orderTotals = new Dictionary<int, decimal>();

            foreach (var delivery in userDeliveries)
            {
                // Считаем сумму винила
                decimal vinylTotal = orderVinyls
                    .Where(oi => oi.deliveryId == delivery.id_delivery)
                    .Sum(oi => oi.Vinyl?.price ?? 0);

                // Считаем сумму проигрывателей (БЕЗ ПАРСИНГА СТРОК!)
                decimal playerTotal = orderPlayers
                    .Where(op => op.deliveryId == delivery.id_delivery)
                    .Sum(op => op.Player?.price ?? 0);

                decimal deliveryTotal = vinylTotal + playerTotal;

                // Применяем скидку
                if (user?.Status != null && user.Status.discount_percentage > 0)
                {
                    deliveryTotal -= (deliveryTotal * user.Status.discount_percentage / 100);
                }

                orderTotals[delivery.id_delivery] = deliveryTotal;

                // Если заказ не отменен, он идет в зачет накопительной скидки
                if (delivery.status_text != "Отменён")
                {
                    totalSpent += deliveryTotal;
                }
            }

            ViewBag.OrderTotals = orderTotals;
            ViewBag.TotalSpent = totalSpent;
            ViewBag.OrderVinyls = orderVinyls; // Передаем список пластинок
            ViewBag.OrderPlayers = orderPlayers; // Передаем список плееров

            // ... далее логика апдейта статуса и корзины (оставь как было) ...

            // (Код апдейта статуса остается прежним...)
            if (user != null)
            {
                var status = await _context.Statuses
                   .OrderByDescending(s => s.min_spend)
                   .FirstOrDefaultAsync(s => totalSpent >= s.min_spend);

                if (status != null && user?.statusId != status.id_status)
                {
                    user.statusId = status.id_status;
                    await _context.SaveChangesAsync();
                    user = await _context.Users.Include(u => u.Status).FirstOrDefaultAsync(u => u.id_user == userId);
                }
            }

            // (Код корзины...)
            var cartIdsString = HttpContext.Session.GetString("Cart") ?? "";
            var playerCartIdsString = HttpContext.Session.GetString("PlayerCart") ?? "";
            var cartVinyls = new List<Vinyl>();
            var cartPlayers = new List<Player>();

            if (!string.IsNullOrEmpty(cartIdsString))
            {
                var ids = cartIdsString.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).Select(int.Parse).ToList();
                foreach (var id in ids) { var v = await _context.Vinyls.FindAsync(id); if (v != null) cartVinyls.Add(v); }
            }
            if (!string.IsNullOrEmpty(playerCartIdsString))
            {
                var pIds = playerCartIdsString.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).Select(int.Parse).ToList();
                foreach (var id in pIds) { var p = await _context.Players.Include(b => b.Brand).FirstOrDefaultAsync(x => x.id_player == id); if (p != null) cartPlayers.Add(p); }
            }

            decimal currentCartTotal = cartVinyls.Sum(v => v.price) + cartPlayers.Sum(p => p.price);
            if (user?.Status != null && user.Status.discount_percentage > 0)
            {
                currentCartTotal -= (currentCartTotal * user.Status.discount_percentage / 100);
            }

            ViewBag.CartItems = cartVinyls;
            ViewBag.CartPlayers = cartPlayers;
            ViewBag.CartTotal = currentCartTotal;
            ViewBag.Deliveries = userDeliveries;

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrder(string address)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            // 1. Создаем объект заказа
            var newDelivery = new Delivery
            {
                delivery_address = address,
                userId = userId.Value,
                status_text = "Новый",
                orderDate = DateTime.UtcNow
            };

            _context.Deliveries.Add(newDelivery);
            await _context.SaveChangesAsync(); // Сначала сохраняем Delivery, чтобы получить id_delivery

            // 2. Сохраняем винил (если есть в сессии)
            var cart = HttpContext.Session.GetString("Cart");
            if (!string.IsNullOrEmpty(cart))
            {
                var ids = cart.Split(',').Select(int.Parse);
                foreach (var id in ids)
                {
                    _context.OrderVinyls.Add(new OrderVinyl { deliveryId = newDelivery.id_delivery, vinylId = id });
                }
            }

            // 3. Сохраняем Плееры (НОВАЯ ЛОГИКА)
            var playerCart = HttpContext.Session.GetString("PlayerCart");
            if (!string.IsNullOrEmpty(playerCart))
            {
                var pIds = playerCart.Split(',').Select(int.Parse);
                foreach (var id in pIds)
                {
                    _context.OrderPlayers.Add(new OrderPlayer { deliveryId = newDelivery.id_delivery, playerId = id });
                }
            }

            await _context.SaveChangesAsync();

            // 4. Чистим сессию
            HttpContext.Session.Remove("Cart");
            HttpContext.Session.Remove("PlayerCart");

            return RedirectToAction("Index", "Profile"); // Или куда там перекидывать после заказа
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGenre(string name, string description)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin") return Forbid();
            if (!string.IsNullOrEmpty(name))
            {
                _context.Genres.Add(new Genre { name = name, description = description });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBrand(string name, string country, string description)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin") return Forbid();
            if (!string.IsNullOrEmpty(name))
            {
                _context.Brands.Add(new Brand { name = name, country = country ?? "Не указана", description = description });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}