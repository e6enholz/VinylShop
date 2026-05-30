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
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var user = await _context.Users.Include(u => u.Status).FirstOrDefaultAsync(u => u.id_user == userId);
            var userDeliveries = await _context.Deliveries.Where(d => d.userId == userId).ToListAsync();

            // Подтягиваем все связи товаров
            var orderItems = await _context.OrderVinyls.Include(ov => ov.Vinyl).Where(ov => ov.Delivery!.userId == userId).ToListAsync();
            ViewBag.OrderItems = orderItems;

            // --- НАКОПИТЕЛЬНАЯ СИСТЕМА СКИДОК ---
            decimal totalSpent = 0;
            var orderTotals = new Dictionary<int, decimal>();

            foreach (var delivery in userDeliveries)
            {
                // Считаем винил в этом заказе
                var vinylsInOrder = orderItems.Where(oi => oi.deliveryId == delivery.id_delivery && oi.Vinyl != null).Select(oi => oi.Vinyl!).ToList();
                decimal deliveryTotal = vinylsInOrder.Sum(v => v.price);

                // Парсим проигрыватели, если они вшиты в строку адреса: "[Оборудование: «Model X»]"
                if (delivery.delivery_address != null && delivery.delivery_address.Contains("[Оборудование:"))
                {
                    // Извлекаем имена моделей и ищем их цену в базе данных
                    var playersTable = await _context.Players.ToListAsync();
                    foreach (var player in playersTable)
                    {
                        if (delivery.delivery_address.Contains($"«{player.model}»") || delivery.delivery_address.Contains($"«{player.model}»"))
                        {
                            deliveryTotal += player.price;
                        }
                    }
                }

                // Применяем скидку, которая БЫЛА у пользователя на момент заказа (или текущую, для простоты диплома)
                if (user?.Status != null && user.Status.discount_percentage > 0)
                {
                    deliveryTotal = deliveryTotal - (deliveryTotal * user.Status.discount_percentage / 100);
                }

                orderTotals[delivery.id_delivery] = deliveryTotal;

                // Если заказ не отменен, он идет в зачет накопительной скидки
                if (delivery.status_text != "Отменён")
                {
                    totalSpent += deliveryTotal;
                }
            }

            ViewBag.OrderTotals = orderTotals; // Передаем стоимости заказов
            ViewBag.TotalSpent = totalSpent;   // Передаем общую сумму трат

            // Авто-апдейт статуса клиента на основе трат (Простая бизнес-логика для диплома)
            if (user != null)
            {
                var status = await _context.Statuses
                 .OrderByDescending(s => s.min_spend) // Сначала самые дорогие пороги
                 .FirstOrDefaultAsync(s => totalSpent >= s.min_spend);

                if (status != null && user?.statusId != status.id_status)
                {
                    user.statusId = status.id_status;
                    await _context.SaveChangesAsync();
                    // Перезагружаем пользователя
                    user = await _context.Users.Include(u => u.Status).FirstOrDefaultAsync(u => u.id_user == userId);
                }
            }

            // Стандартная логика корзины (похожая на прошлую)
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

            var cartIdsString = HttpContext.Session.GetString("Cart") ?? "";
            var playerCartIdsString = HttpContext.Session.GetString("PlayerCart") ?? "";

            if (string.IsNullOrEmpty(cartIdsString) && string.IsNullOrEmpty(playerCartIdsString))
            {
                return RedirectToAction("Index", "Cart");
            }
            if (string.IsNullOrEmpty(address))
            {
                return RedirectToAction("Index", "Cart");
            }

            // 1. Создаем объект доставки
            var newDelivery = new Delivery
            {
                delivery_address = address,
                status_text = "В обработке",
                orderDate = DateTime.UtcNow,
                userId = userId.Value
            };

            _context.Deliveries.Add(newDelivery);
            await _context.SaveChangesAsync(); // Сначала сохраняем, чтобы получить ID заказа

            // 2. Сохраняем пластинки (как было)
            if (!string.IsNullOrEmpty(cartIdsString))
            {
                var ids = cartIdsString.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).Select(int.Parse).ToList();
                foreach (var vinylId in ids)
                {
                    _context.OrderVinyls.Add(new OrderVinyl
                    {
                        deliveryId = newDelivery.id_delivery,
                        vinylId = vinylId
                    });
                }
            }

            // 3. Сохраняем плееры (НОВЫЙ БЛОК)
            if (!string.IsNullOrEmpty(playerCartIdsString))
            {
                var pIds = playerCartIdsString.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).Select(int.Parse).ToList();
                foreach (var pId in pIds)
                {
                    _context.OrderPlayers.Add(new OrderPlayer
                    {
                        deliveryId = newDelivery.id_delivery,
                        playerId = pId
                    });
                }
            }

            await _context.SaveChangesAsync(); // Сохраняем все добавления разом

            // Чистим сессию
            HttpContext.Session.Remove("Cart");
            HttpContext.Session.Remove("PlayerCart");

            return RedirectToAction("Index", "Orders"); // Перенаправляем на список заказов
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