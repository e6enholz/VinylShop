using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VinylShop.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "brands",
                columns: table => new
                {
                    id_brand = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    country = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_brands", x => x.id_brand);
                });

            migrationBuilder.CreateTable(
                name: "conditions",
                columns: table => new
                {
                    id_condition = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conditions", x => x.id_condition);
                });

            migrationBuilder.CreateTable(
                name: "drive_types",
                columns: table => new
                {
                    id_drive_type = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_drive_types", x => x.id_drive_type);
                });

            migrationBuilder.CreateTable(
                name: "genres",
                columns: table => new
                {
                    id_genre = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_genres", x => x.id_genre);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id_role = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id_role);
                });

            migrationBuilder.CreateTable(
                name: "statuses",
                columns: table => new
                {
                    id_status = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    min_spend = table.Column<decimal>(type: "numeric", nullable: false),
                    discount_percentage = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_statuses", x => x.id_status);
                });

            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    id_player = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    model = table.Column<string>(type: "text", nullable: false),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    brandId = table.Column<int>(type: "integer", nullable: false),
                    driveTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_players", x => x.id_player);
                    table.ForeignKey(
                        name: "FK_players_brands_brandId",
                        column: x => x.brandId,
                        principalTable: "brands",
                        principalColumn: "id_brand",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_players_drive_types_driveTypeId",
                        column: x => x.driveTypeId,
                        principalTable: "drive_types",
                        principalColumn: "id_drive_type",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vinyls",
                columns: table => new
                {
                    id_vinyl = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    album = table.Column<string>(type: "text", nullable: false),
                    artist = table.Column<string>(type: "text", nullable: false),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    genreId = table.Column<int>(type: "integer", nullable: false),
                    conditionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vinyls", x => x.id_vinyl);
                    table.ForeignKey(
                        name: "FK_vinyls_conditions_conditionId",
                        column: x => x.conditionId,
                        principalTable: "conditions",
                        principalColumn: "id_condition",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_vinyls_genres_genreId",
                        column: x => x.genreId,
                        principalTable: "genres",
                        principalColumn: "id_genre",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id_user = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    login = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    statusId = table.Column<int>(type: "integer", nullable: false),
                    roleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id_user);
                    table.ForeignKey(
                        name: "FK_users_roles_roleId",
                        column: x => x.roleId,
                        principalTable: "roles",
                        principalColumn: "id_role",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_users_statuses_statusId",
                        column: x => x.statusId,
                        principalTable: "statuses",
                        principalColumn: "id_status",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "deliveries",
                columns: table => new
                {
                    id_delivery = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    delivery_address = table.Column<string>(type: "text", nullable: false),
                    status_text = table.Column<string>(type: "text", nullable: false),
                    orderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    userId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deliveries", x => x.id_delivery);
                    table.ForeignKey(
                        name: "FK_deliveries_users_userId",
                        column: x => x.userId,
                        principalTable: "users",
                        principalColumn: "id_user",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "brands",
                columns: new[] { "id_brand", "country", "description", "name" },
                values: new object[,]
                {
                    { 1, "Япония", "Легендарные вертушки", "Technics" },
                    { 2, "Япония", "Отличный выбор для старта", "Audio-Technica" }
                });

            migrationBuilder.InsertData(
                table: "conditions",
                columns: new[] { "id_condition", "description", "name" },
                values: new object[,]
                {
                    { 1, "Абсолютно новая, не проигрывалась, возможно, запечатанная. Конверт и вложения в идеальном состоянии.", "Mint (M)" },
                    { 2, "Пластинка выглядит как новая, без видимых дефектов, проигрывалась несколько раз. Звук чистый, без посторонних шумов.", "Near Mint (NM)" },
                    { 3, "Имеются следы использования, мелкие поверхностные царапины, не влияющие на качество звука. Допускается лёгкий треск в паузах.", "Excellent (EX)" },
                    { 4, "Качество звука заметно ухудшилось. Возможны некоторые искажения, глубокие и длинные царапины. Обложка и вложения пострадали от сгибов, повреждений краёв, разрывов корешка, обесцвечивания и т. п.", "Very Good (VG)" },
                    { 5, "Ещё можно проигрывать, но с пластинкой обращались неправильно, она имеет заметный шум и даже может «прыгать». Конверт и вложения порваны, испачканы, испорчены.", "Fair (F)" }
                });

            migrationBuilder.InsertData(
                table: "drive_types",
                columns: new[] { "id_drive_type", "description", "name" },
                values: new object[,]
                {
                    { 1, "Двигатель вращает диск напрямую", "Прямой" },
                    { 2, "Передача через пассик", "Ременной" }
                });

            migrationBuilder.InsertData(
                table: "genres",
                columns: new[] { "id_genre", "description", "name" },
                values: new object[,]
                {
                    { 1, "Classic Rock and Metal", "Rock" },
                    { 2, "Smooth and Bebop Jazz", "Jazz" },
                    { 3, "Modern and Synth Pop", "Pop" }
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id_role", "name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "User" }
                });

            migrationBuilder.InsertData(
                table: "statuses",
                columns: new[] { "id_status", "discount_percentage", "min_spend", "name" },
                values: new object[,]
                {
                    { 1, 0, 0m, "Новичок" },
                    { 2, 3, 10000m, "Бронзовый" },
                    { 3, 5, 30000m, "Серебряный" },
                    { 4, 10, 70000m, "Золотой" },
                    { 5, 15, 150000m, "Платиновый" }
                });

            migrationBuilder.InsertData(
                table: "players",
                columns: new[] { "id_player", "brandId", "driveTypeId", "model", "price" },
                values: new object[,]
                {
                    { 1, 2, 1, "AT-LP120X", 35000m },
                    { 2, 1, 1, "SL-1200", 85000m }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id_user", "email", "login", "password", "roleId", "statusId" },
                values: new object[,]
                {
                    { 1, "admin@vinyl.ru", "admin", "admin", 1, 3 },
                    { 2, "ivan@mail.ru", "ivan_vinyl", "password123", 2, 1 }
                });

            migrationBuilder.InsertData(
                table: "vinyls",
                columns: new[] { "id_vinyl", "album", "artist", "conditionId", "genreId", "price" },
                values: new object[,]
                {
                    { 1, "The Dark Side of the Moon", "Pink Floyd", 1, 1, 4500m },
                    { 2, "Kind of Blue", "Miles Davis", 2, 2, 3800m }
                });

            migrationBuilder.InsertData(
                table: "deliveries",
                columns: new[] { "id_delivery", "delivery_address", "orderDate", "status_text", "userId" },
                values: new object[] { 1, "г. Санкт-Петербург, ул. Большая Морская, д. 18", new DateTime(2026, 5, 30, 3, 47, 30, 554, DateTimeKind.Utc).AddTicks(1439), "В пути", 2 });

            migrationBuilder.CreateIndex(
                name: "IX_deliveries_userId",
                table: "deliveries",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_players_brandId",
                table: "players",
                column: "brandId");

            migrationBuilder.CreateIndex(
                name: "IX_players_driveTypeId",
                table: "players",
                column: "driveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_users_roleId",
                table: "users",
                column: "roleId");

            migrationBuilder.CreateIndex(
                name: "IX_users_statusId",
                table: "users",
                column: "statusId");

            migrationBuilder.CreateIndex(
                name: "IX_vinyls_conditionId",
                table: "vinyls",
                column: "conditionId");

            migrationBuilder.CreateIndex(
                name: "IX_vinyls_genreId",
                table: "vinyls",
                column: "genreId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "deliveries");

            migrationBuilder.DropTable(
                name: "players");

            migrationBuilder.DropTable(
                name: "vinyls");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "brands");

            migrationBuilder.DropTable(
                name: "drive_types");

            migrationBuilder.DropTable(
                name: "conditions");

            migrationBuilder.DropTable(
                name: "genres");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "statuses");
        }
    }
}
