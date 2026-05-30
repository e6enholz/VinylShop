using Microsoft.EntityFrameworkCore;
using VinylShop.Models;

namespace VinylShop.DBContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Genre> Genres { get; set; } = null!;
        public DbSet<Condition> Conditions { get; set; } = null!;
        public DbSet<Vinyl> Vinyls { get; set; } = null!;
        public DbSet<TypeDrive> TypeDrives { get; set; } = null!;
        public DbSet<Brand> Brands { get; set; } = null!;
        public DbSet<Player> Players { get; set; } = null!;
        public DbSet<Status> Statuses { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Delivery> Deliveries { get; set; } = null!;
        public DbSet<OrderVinyl> OrderVinyls { get; set; } = null!;
        public DbSet<OrderPlayer> OrderPlayers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Genre>().HasData(
                new Genre { id_genre = 1, name = "Rock", description = "Classic Rock and Metal" },
                new Genre { id_genre = 2, name = "Jazz", description = "Smooth and Bebop Jazz" },
                new Genre { id_genre = 3, name = "Pop", description = "Modern and Synth Pop" }
            );

            modelBuilder.Entity<Condition>().HasData(
                new Condition
                {
                    id_condition = 1,
                    name = "Mint (M)",
                    description = "Абсолютно новая, не проигрывалась, возможно, запечатанная. Конверт и вложения в идеальном состоянии."
                },
                new Condition
                {
                    id_condition = 2,
                    name = "Near Mint (NM)",
                    description = "Пластинка выглядит как новая, без видимых дефектов, проигрывалась несколько раз. Звук чистый, без посторонних шумов."
                },
                new Condition
                {
                    id_condition = 3,
                    name = "Excellent (EX)",
                    description = "Имеются следы использования, мелкие поверхностные царапины, не влияющие на качество звука. Допускается лёгкий треск в паузах."
                },
                new Condition
                {
                    id_condition = 4,
                    name = "Very Good (VG)",
                    description = "Качество звука заметно ухудшилось. Возможны некоторые искажения, глубокие и длинные царапины. Обложка и вложения пострадали от сгибов, повреждений краёв, разрывов корешка, обесцвечивания и т. п."
                },
                new Condition
                {
                    id_condition = 5,
                    name = "Fair (F)",
                    description = "Ещё можно проигрывать, но с пластинкой обращались неправильно, она имеет заметный шум и даже может «прыгать». Конверт и вложения порваны, испачканы, испорчены."
                }
            );

            modelBuilder.Entity<TypeDrive>().HasData(
                new TypeDrive { id_drive_type = 1, name = "Прямой", description = "Двигатель вращает диск напрямую" },
                new TypeDrive { id_drive_type = 2, name = "Ременной", description = "Передача через пассик" }
            );

            modelBuilder.Entity<Brand>().HasData(
                new Brand { id_brand = 1, name = "Technics", country = "Япония", description = "Легендарные вертушки" },
                new Brand { id_brand = 2, name = "Audio-Technica", country = "Япония", description = "Отличный выбор для старта" }
            );

            modelBuilder.Entity<Status>().HasData(
                new Status { id_status = 1, name = "Новичок", min_spend = 0, discount_percentage = 0 },
                new Status { id_status = 2, name = "Бронзовый", min_spend = 10000, discount_percentage = 3 },
                new Status { id_status = 3, name = "Серебряный", min_spend = 30000, discount_percentage = 5 },
                new Status { id_status = 4, name = "Золотой", min_spend = 70000, discount_percentage = 10},
                new Status { id_status = 5, name = "Платиновый", min_spend = 150000, discount_percentage = 15}
            );

            modelBuilder.Entity<Role>().HasData(
                new Role { id_role = 1, name = "Admin" },
                new Role { id_role = 2, name = "User" }
            );

            modelBuilder.Entity<User>().HasData(
                new User { id_user = 1, login = "admin", email = "admin@vinyl.ru", password = "admin", roleId = 1, statusId = 3 },
                new User { id_user = 2, login = "user", email = "user@mail.ru", password = "user", roleId = 2, statusId = 1 }
            );

            modelBuilder.Entity<Vinyl>().HasData(
                new Vinyl { id_vinyl = 1, album = "The Dark Side of the Moon", artist = "Pink Floyd", price = 4500, genreId = 1, conditionId = 1 },
                new Vinyl { id_vinyl = 2, album = "Kind of Blue", artist = "Miles Davis", price = 3800, genreId = 2, conditionId = 2 }
            );

            modelBuilder.Entity<Player>().HasData(
                new Player { id_player = 1, model = "AT-LP120X", price = 35000, brandId = 2, driveTypeId = 1 },
                new Player { id_player = 2, model = "SL-1200", price = 85000, brandId = 1, driveTypeId = 1 }
            );

        }
    }
}
