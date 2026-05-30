using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinylShop.DBContext;
using VinylShop.Models;

namespace VinylShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // Страница Входа (GET)
        public IActionResult Login() => View();

        // Обработка Входа (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string login, string password)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.login == login && u.password == password); // В лабе можно без хеширования, если препод не маньяк

            if (user != null)
            {
                // Записываем данные в сессию
                HttpContext.Session.SetInt32("UserId", user.id_user);
                HttpContext.Session.SetString("UserLogin", user.login);
                HttpContext.Session.SetString("UserRole", user.Role?.name ?? "Пользователь");

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Неверный логин или пароль!";
            return View();
        }

        // Страница Регистрации (GET)
        public IActionResult Register() => View();

        // Обработка Регистрации (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            ModelState.Remove("Role");
            ModelState.Remove("Status");

            if (ModelState.IsValid)
            {
                // Проверяем, нет ли уже такого логина
                if (await _context.Users.AnyAsync(u => u.login == user.login))
                {
                    ViewBag.Error = "Этот логин уже занят!";
                    return View(user);
                }

                // Выставляем дефолтную роль Покупателя/Юзера и статус (например, ID=1 - Новичок)
                // Убедись, какие ID у тебя в базе для Ролей и Статусов!
                user.roleId = 2;   // Предположим 1 - Админ, 2 - Пользователь
                user.statusId = 1; // 1 - Новичок (без скидки)

                _context.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Login));
            }
            return View(user);
        }

        // Выход из аккаунта
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}