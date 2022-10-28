using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FakeXiecheng.API.Migrations
{
    public partial class RemoveOrderId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "LineItems");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "308660dc-ae51-480f-824d-7dca6714c3e2",
                column: "ConcurrencyStamp",
                value: "8b822815-dae6-48d3-a2eb-d358e9a2db13");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "90184155-dee0-40c9-bb1e-b5ed07afc04e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ed04c8e7-768e-429c-ae66-39a4d2e73b01", "AQAAAAEAACcQAAAAEE0hsh6qA6Y5XANF59NMPrmT1gF1p8mnmTE3TnM48Thx8+zzLy7iepXnG7+Id+uAMA==", "36cfbc62-5f6b-425c-9edb-37971539b34c" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "LineItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "308660dc-ae51-480f-824d-7dca6714c3e2",
                column: "ConcurrencyStamp",
                value: "b10f8f34-ddfa-4d02-a8b6-c7e5b4be054f");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "90184155-dee0-40c9-bb1e-b5ed07afc04e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cfd91871-b9f2-48a0-adc6-19cc4111bfd0", "AQAAAAEAACcQAAAAELAPJoqYxUK377ZnMDhOKXzkq40eHEAI5uM42JLNRaVwxZZog2XimqqhVko57EGNOA==", "e9248074-1f6a-4427-a94a-70fada906bd4" });
        }
    }
}
