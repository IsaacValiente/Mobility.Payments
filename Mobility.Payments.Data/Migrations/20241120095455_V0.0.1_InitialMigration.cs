namespace Mobility.Payments.Data.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class V001_InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Balance = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    UserRole = table.Column<int>(type: "int", nullable: false),
                    SysCreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SysModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    SenderId = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    ReceiverId = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    SysCreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SysModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payment_User_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "User",
                        principalColumn: "Username");
                    table.ForeignKey(
                        name: "FK_Payment_User_SenderId",
                        column: x => x.SenderId,
                        principalTable: "User",
                        principalColumn: "Username");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payment_ReceiverId",
                table: "Payment",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_SenderId",
                table: "Payment",
                column: "SenderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
