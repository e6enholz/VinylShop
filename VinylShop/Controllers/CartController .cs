using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinylShop.DBContext;
using VinylShop.Models;

namespace VinylShop.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        // 1. Страница Корзины (Показ товаров, подсчет суммы и скидки)
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var user = await _context.Users
                .Include(u => u.Status)
                .FirstOrDefaultAsync(u => u.id_user == userId);

            var cartIdsString = HttpContext.Session.GetString("Cart") ?? "";
            var playerCartIdsString = HttpContext.Session.GetString("PlayerCart") ?? "";

            var cartVinyls = new List<Vinyl>();
            var cartPlayers = new List<Player>();
            decimal total = 0;
            int discount = user?.Status?.discount_percentage ?? 0;

            // Пластинки
            if (!string.IsNullOrEmpty(cartIdsString))
            {
                var ids = cartIdsString.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).Select(int.Parse).ToList();
                foreach (var id in ids)
                {
                    var vinyl = await _context.Vinyls.FindAsync(id);
                    if (vinyl != null) cartVinyls.Add(vinyl);
                }
            }

            // Проигрыватели
            if (!string.IsNullOrEmpty(playerCartIdsString))
            {
                var pIds = playerCartIdsString.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).Select(int.Parse).ToList();
                foreach (var id in pIds)
                {
                    var player = await _context.Players.Include(p => p.Brand).FirstOrDefaultAsync(p => p.id_player == id);
                    if (player != null) cartPlayers.Add(player);
                }
            }

            total = cartVinyls.Sum(v => v.price) + cartPlayers.Sum(p => p.price);
            if (discount > 0)
            {
                total = total - (total * discount / 100);
            }

            ViewBag.CartPlayers = cartPlayers;
            ViewBag.CartTotal = total;
            ViewBag.Discount = discount;

            return View(cartVinyls); // Передаем список винила как основную модель
        }

        // 2. Добавление пластинки в сессию (Срабатывает по кнопке из каталога)
        [HttpPost]
        public IActionResult AddToCart(int vinylId)
        {
            var cart = HttpContext.Session.GetString("Cart") ?? "";

            if (string.IsNullOrEmpty(cart))
            {
                cart = vinylId.ToString();
            }
            else
            {
                cart = cart + "," + vinylId;
            }

            HttpContext.Session.SetString("Cart", cart);

            // После добавления возвращаем юзера обратно на список пластинок
            return RedirectToAction("Index", "Vinyls");
        }

        [HttpPost]
        public IActionResult AddPlayerToCart(int playerId)
        {
            var cart = HttpContext.Session.GetString("PlayerCart") ?? "";
            if (string.IsNullOrEmpty(cart)) cart = playerId.ToString();
            else cart += "," + playerId;

            HttpContext.Session.SetString("PlayerCart", cart);
            return RedirectToAction("Index", "Players"); // или как там у тебя контроллер проигрывателей называется
        }

        // 3. Полная очистка корзины
        public IActionResult Clear()
        {
            HttpContext.Session.Remove("Cart");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult RemoveVinyl(int vinylId)
        {
            var cart = HttpContext.Session.GetString("Cart") ?? "";
            if (!string.IsNullOrEmpty(cart))
            {
                var ids = cart.Split(',').ToList();
                // Удаляем один конкретный ID
                ids.Remove(vinylId.ToString());
                HttpContext.Session.SetString("Cart", string.Join(",", ids));
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult RemovePlayer(int playerId)
        {
            var cart = HttpContext.Session.GetString("PlayerCart") ?? "";
            if (!string.IsNullOrEmpty(cart))
            {
                var ids = cart.Split(',').ToList();
                // Удаляем один конкретный ID вертушки
                ids.Remove(playerId.ToString());
                HttpContext.Session.SetString("PlayerCart", string.Join(",", ids));
            }
            return RedirectToAction(nameof(Index));
        }
    }
}