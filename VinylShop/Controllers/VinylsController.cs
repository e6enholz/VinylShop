using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinylShop.DBContext;
using VinylShop.Models;

namespace VinylShop.Controllers
{
    public class VinylsController : Controller
    {
        private readonly AppDbContext _context;

        public VinylsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // .Include подтягивает связанные таблицы Genre и Condition (как JOIN в SQL)
            var vinyls = await _context.Vinyls
                .Include(v => v.Genre)
                .Include(v => v.Condition)
                .ToListAsync();

            return View(vinyls);
        }

        // Просмотр детальной информации об одной пластинке
        public async Task<IActionResult> Details(int id)
        {
            var vinyl = await _context.Vinyls
                .Include(v => v.Genre)
                .Include(v => v.Condition)
                .FirstOrDefaultAsync(v => v.id_vinyl == id);

            if (vinyl == null)
            {
                return NotFound();
            }

            return View(vinyl);
        }

        // 1. Открываем форму создания (GET)
        public IActionResult Create()
        {
            // Вытягиваем жанры и состояния из базы, чтобы засунуть их в выпадающие списки
            ViewBag.id_genre = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Genres, "id_genre", "name");
            ViewBag.id_condition = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Conditions, "id_condition", "name");
            return View();
        }

        // 2. Ловим данные из формы и сохраняем в базу (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("album,artist,price,genreId,conditionId")] Vinyl vinyl)
        {
            // Убираем проверку навигационных свойств, так как форма шлет только цифры-айдишники
            ModelState.Remove("Genre");
            ModelState.Remove("Condition");

            if (ModelState.IsValid)
            {
                _context.Add(vinyl);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Если упало, пересоздаем списки, используя правильные свойства модели
            ViewBag.id_genre = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Genres, "id_genre", "name", vinyl.genreId);
            ViewBag.id_condition = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Conditions, "id_condition", "name", vinyl.conditionId);
            return View(vinyl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var vinyl = await _context.Vinyls.FindAsync(id);
            if (vinyl != null)
            {
                _context.Vinyls.Remove(vinyl);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index)); // Перезагружаем каталог
        }
    }
}
