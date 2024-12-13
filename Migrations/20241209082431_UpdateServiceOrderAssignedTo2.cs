using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Praxisarbeit_M295.Migrations
{
    /// <inheritdoc />
    public partial class UpdateServiceOrderAssignedTo2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrders_Users_UserId",
                table: "ServiceOrders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceOrders_UserId",
                table: "ServiceOrders");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ServiceOrders");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_AssignedTo",
                table: "ServiceOrders",
                column: "AssignedTo");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrders_Users_AssignedTo",
                table: "ServiceOrders",
                column: "AssignedTo",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrders_Users_AssignedTo",
                table: "ServiceOrders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceOrders_AssignedTo",
                table: "ServiceOrders");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ServiceOrders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_UserId",
                table: "ServiceOrders",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrders_Users_UserId",
                table: "ServiceOrders",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
