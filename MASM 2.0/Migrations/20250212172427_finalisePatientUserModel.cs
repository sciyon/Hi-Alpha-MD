using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MASM_2._0.Migrations
{
    /// <inheritdoc />
    public partial class finalisePatientUserModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EmailVerified",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailVerified",
                table: "AspNetUsers");
        }
    }
}
