using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransactionAggregation.Api.Migrations
{
    /// <inheritdoc />
    public partial class IdempotencyKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Merchant",
                table: "AggregatedTransactions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdempotencyKey",
                table: "AggregatedTransactions",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedTransactions_IdempotencyKey",
                table: "AggregatedTransactions",
                column: "IdempotencyKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AggregatedTransactions_IdempotencyKey",
                table: "AggregatedTransactions");

            migrationBuilder.DropColumn(
                name: "IdempotencyKey",
                table: "AggregatedTransactions");

            migrationBuilder.AlterColumn<string>(
                name: "Merchant",
                table: "AggregatedTransactions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);
        }
    }
}
