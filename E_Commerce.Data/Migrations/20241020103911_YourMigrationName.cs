using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class YourMigrationName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "PromoCodes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Produts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "PromocodeId",
                table: "Produts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Produts_PromocodeId",
                table: "Produts",
                column: "PromocodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Produts_PromoCodes_PromocodeId",
                table: "Produts",
                column: "PromocodeId",
                principalTable: "PromoCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produts_PromoCodes_PromocodeId",
                table: "Produts");

            migrationBuilder.DropIndex(
                name: "IX_Produts_PromocodeId",
                table: "Produts");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "PromoCodes");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Produts");

            migrationBuilder.DropColumn(
                name: "PromocodeId",
                table: "Produts");
        }
    }
}
