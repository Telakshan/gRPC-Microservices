using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductGrpc.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<float>(type: "real", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CreatedTime", "Description", "Name", "Price", "Status" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 8, 17, 1, 37, 36, 110, DateTimeKind.Utc).AddTicks(9427), "New Xiaomi Phone Mi10T", "Mi10T", 699f, 0 },
                    { 2, new DateTime(2023, 8, 17, 1, 37, 36, 110, DateTimeKind.Utc).AddTicks(9434), "New Huawei Phone P40", "P40", 899f, 0 },
                    { 3, new DateTime(2023, 8, 17, 1, 37, 36, 110, DateTimeKind.Utc).AddTicks(9436), "New Samsung Phone A50", "A50", 399f, 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
