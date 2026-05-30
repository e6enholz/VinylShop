using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinylShop.DBContext; // Твой неймспейс контекста
using VinylShop.Models;

namespace VinylShop.Controllers
{
    public class PlayersController : Controller
    {
        private readonly AppDbContext _context;

        public PlayersController(AppDbContext context)
        {
            _context = context;
        }

        // Список всех вертушек
        public async Task<IActionResult> Index()
        {
            var players = await _context.Players
                .Include(p => p.Brand)
                .Include(p => p.DriveType)
                .ToListAsync();

            return View(players);
        }

        // Детали конкретного проигрывателя
        public async Task<IActionResult> Details(int id)
        {
            var player = await _context.Players
                .Include(p => p.Brand)
                .Include(p => p.DriveType)
                .FirstOrDefaultAsync(p => p.id_player == id);

            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        public IActionResult Create()
        {
            // Загружаем бренды и приводы для выпадающих списков
            ViewBag.id_brand = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Brands, "id_brand", "name");
            ViewBag.id_drive_type = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.TypeDrives, "id_drive_type", "name");
            return View();
        }

        // 2. Сохранение проигрывателя (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("model,price,brandId,driveTypeId")] Player player)
        {
            // Отключаем валидацию навигационных свойств, чтобы не было пустых ошибок
            ModelState.Remove("Brand");
            ModelState.Remove("DriveType");

            if (ModelState.IsValid)
            {
                _context.Add(player);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.id_brand = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Brands, "id_brand", "name", player.brandId);
            ViewBag.id_drive_type = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.TypeDrives, "id_drive_type", "name", player.driveTypeId);
            return View(player);
        }

        // 3. Удаление проигрывателя (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player != null)
            {
                _context.Players.Remove(player);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}