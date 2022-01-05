using Microsoft.EntityFrameworkCore.Migrations;

namespace ASGlass.Migrations
{
    public partial class CreateSomeItemsInOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Sliders",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Corner",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "Orders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Diametr",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "En",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAccessory",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Polish",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Shape",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Thickness",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Uzunluq",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Corner",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Count",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Diametr",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "En",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsAccessory",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Polish",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Shape",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Thickness",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Uzunluq",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Sliders",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 150,
                oldNullable: true);
        }
    }
}
