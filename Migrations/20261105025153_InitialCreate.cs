using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QLedger.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Lines",
                columns: table => new
                {
                    LineId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LineCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LineName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Area = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lines", x => x.LineId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DefaultSamplingQty = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RfidTag = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "MachineChecklists",
                columns: table => new
                {
                    ChecklistId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LineId = table.Column<int>(type: "int", nullable: false),
                    CheckDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Shift = table.Column<int>(type: "int", nullable: false),
                    IsPassed = table.Column<bool>(type: "bit", nullable: false),
                    NgCount = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CheckedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineChecklists", x => x.ChecklistId);
                    table.ForeignKey(
                        name: "FK_MachineChecklists_Lines_LineId",
                        column: x => x.LineId,
                        principalTable: "Lines",
                        principalColumn: "LineId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InspectionLedgers",
                columns: table => new
                {
                    LedgerId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LineId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    LotNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrderQty = table.Column<int>(type: "int", nullable: false),
                    QueueNumber = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsARI = table.Column<bool>(type: "bit", nullable: false),
                    IsNRS = table.Column<bool>(type: "bit", nullable: false),
                    IsIFC = table.Column<bool>(type: "bit", nullable: false),
                    IsRRS = table.Column<bool>(type: "bit", nullable: false),
                    IsEFR = table.Column<bool>(type: "bit", nullable: false),
                    IsSplit = table.Column<bool>(type: "bit", nullable: false),
                    IsTopup = table.Column<bool>(type: "bit", nullable: false),
                    PdPicId = table.Column<int>(type: "int", nullable: false),
                    PdInTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QaInspectorId = table.Column<int>(type: "int", nullable: true),
                    InspectionLevel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    QaStartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QaFinishTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QaJudgement = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionLedgers", x => x.LedgerId);
                    table.ForeignKey(
                        name: "FK_InspectionLedgers_Lines_LineId",
                        column: x => x.LineId,
                        principalTable: "Lines",
                        principalColumn: "LineId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InspectionLedgers_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InspectionLedgers_Users_PdPicId",
                        column: x => x.PdPicId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InspectionLedgers_Users_QaInspectorId",
                        column: x => x.QaInspectorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NrsReports",
                columns: table => new
                {
                    NrsId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NrsNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    LedgerId = table.Column<long>(type: "bigint", nullable: false),
                    DefectCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DefectQty = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    WhyOccur1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    WhyOccur2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    WhyOccur3 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    WhyOccur4 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    WhyOccur5 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    WhyOutflow1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    WhyOutflow2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    WhyOutflow3 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TemporaryAction = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PermanentAction = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TargetDate = table.Column<DateOnly>(type: "date", nullable: true),
                    PicUserId = table.Column<int>(type: "int", nullable: true),
                    Disposition = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ApprovedById = table.Column<int>(type: "int", nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SupervisorNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NrsReports", x => x.NrsId);
                    table.ForeignKey(
                        name: "FK_NrsReports_InspectionLedgers_LedgerId",
                        column: x => x.LedgerId,
                        principalTable: "InspectionLedgers",
                        principalColumn: "LedgerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NrsReports_Users_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NrsReports_Users_PicUserId",
                        column: x => x.PicUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Lines",
                columns: new[] { "LineId", "Area", "IsActive", "LineCode", "LineName" },
                values: new object[,]
                {
                    { 1, "SWITCH 1", true, "SW-01", "Switch Assembly 1" },
                    { 2, "RELAY 1", true, "RL-01", "Relay Production Line 1" },
                    { 3, "RELAY 2", true, "RL-02", "Relay Production Line 2" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Category", "DefaultSamplingQty", "IsActive", "ItemCode", "ItemName" },
                values: new object[,]
                {
                    { 1, "Power Relay", 50, true, "G2R-1-24VDC", "Power Relay G2R 24VDC" },
                    { 2, "PCB Relay", 50, true, "G6RL-1-12VDC", "Slim Miniature Relay G6RL 12VDC" },
                    { 3, "Industrial Switch", 30, true, "A3U-301-RED", "Pushbutton Switch A3U Series" },
                    { 4, "Power Relay", 50, true, "LY2-0-24VDC", "General Purpose Relay LY2" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CreatedAt", "FullName", "IsActive", "RfidTag", "Role", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Budi Santoso", true, "RFID-PD-001", "PRODUCTION", "budi" },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Siti Rahmawati", true, "RFID-QA-001", "INSPECTOR", "siti" },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Joko Widodo SPV", true, "RFID-QA-SPV", "SUPERVISOR", "joko" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionLedgers_LineId",
                table: "InspectionLedgers",
                column: "LineId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionLedgers_PdPicId",
                table: "InspectionLedgers",
                column: "PdPicId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionLedgers_ProductId",
                table: "InspectionLedgers",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionLedgers_QaInspectorId",
                table: "InspectionLedgers",
                column: "QaInspectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Lines_LineCode",
                table: "Lines",
                column: "LineCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MachineChecklists_LineId",
                table: "MachineChecklists",
                column: "LineId");

            migrationBuilder.CreateIndex(
                name: "IX_NrsReports_ApprovedById",
                table: "NrsReports",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_NrsReports_LedgerId",
                table: "NrsReports",
                column: "LedgerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NrsReports_NrsNumber",
                table: "NrsReports",
                column: "NrsNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NrsReports_PicUserId",
                table: "NrsReports",
                column: "PicUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ItemCode",
                table: "Products",
                column: "ItemCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MachineChecklists");

            migrationBuilder.DropTable(
                name: "NrsReports");

            migrationBuilder.DropTable(
                name: "InspectionLedgers");

            migrationBuilder.DropTable(
                name: "Lines");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
