using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Praxisarbeit_M295.Migrations
{
    /// <inheritdoc />
    public partial class UpdateServiceOrderAssignedTo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrders_Users_AssignedUserUserId",
                table: "ServiceOrders");

            migrationBuilder.RenameColumn(
                name: "AssignedUserUserId",
                table: "ServiceOrders",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceOrders_AssignedUserUserId",
                table: "ServiceOrders",
                newName: "IX_ServiceOrders_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "AssignedTo",
                table: "ServiceOrders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrders_Users_UserId",
                table: "ServiceOrders",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrders_Users_UserId",
                table: "ServiceOrders");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ServiceOrders",
                newName: "AssignedUserUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceOrders_UserId",
                table: "ServiceOrders",
                newName: "IX_ServiceOrders_AssignedUserUserId");

            migrationBuilder.AlterColumn<int>(
                name: "AssignedTo",
                table: "ServiceOrders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrders_Users_AssignedUserUserId",
                table: "ServiceOrders",
                column: "AssignedUserUserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
