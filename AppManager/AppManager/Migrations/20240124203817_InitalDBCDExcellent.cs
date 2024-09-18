using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_2_Web_Api.Migrations
{
    public partial class InitalDBCDExcellent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Exipres = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiTokens", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Medias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PositionGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PositionGroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Positions_PositionGroups_PositionGroupId",
                        column: x => x.PositionGroupId,
                        principalTable: "PositionGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaffUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fullname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhotoAvatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PasswordResetToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResetTokenExpires = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsStatus = table.Column<bool>(type: "bit", nullable: true),
                    AreaId = table.Column<int>(type: "int", nullable: true),
                    NotificationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffUsers_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StaffUsers_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StaffUsers_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Distributors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SaleManagementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SalesId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsStatus = table.Column<bool>(type: "bit", nullable: true),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PhotoAvatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordResetToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResetTokenExpires = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AreaId = table.Column<int>(type: "int", nullable: true),
                    NotificationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Distributors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Distributors_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Distributors_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Distributors_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Distributors_StaffUsers_CreateBy",
                        column: x => x.CreateBy,
                        principalTable: "StaffUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Distributors_StaffUsers_SaleManagementId",
                        column: x => x.SaleManagementId,
                        principalTable: "StaffUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Distributors_StaffUsers_SalesId",
                        column: x => x.SalesId,
                        principalTable: "StaffUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsStatus = table.Column<bool>(type: "bit", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PathOfTheArticle = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_StaffUsers_CreateBy",
                        column: x => x.CreateBy,
                        principalTable: "StaffUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StaffUserStaffUser",
                columns: table => new
                {
                    StaffInteriorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffSuperiorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffUserStaffUser", x => new { x.StaffInteriorId, x.StaffSuperiorId });
                    table.ForeignKey(
                        name: "FK_StaffUserStaffUser_StaffUsers_StaffInteriorId",
                        column: x => x.StaffInteriorId,
                        principalTable: "StaffUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StaffUserStaffUser_StaffUsers_StaffSuperiorId",
                        column: x => x.StaffSuperiorId,
                        principalTable: "StaffUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskForVisit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VisitId = table.Column<int>(type: "int", nullable: false),
                    AssignedStaffUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportingStaffUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskForVisit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskForVisit_StaffUsers_AssignedStaffUserId",
                        column: x => x.AssignedStaffUserId,
                        principalTable: "StaffUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TaskForVisit_StaffUsers_ReportingStaffUserId",
                        column: x => x.ReportingStaffUserId,
                        principalTable: "StaffUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsStatus = table.Column<bool>(type: "bit", nullable: true),
                    SecurityCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhotoAvatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PasswordResetToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResetTokenExpires = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AreaId = table.Column<int>(type: "int", nullable: true),
                    NotificationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_StaffUsers_CreateBy",
                        column: x => x.CreateBy,
                        principalTable: "StaffUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ParentCommentId = table.Column<int>(type: "int", nullable: true),
                    Comment1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_TaskForVisit_TaskId",
                        column: x => x.TaskId,
                        principalTable: "TaskForVisit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhotoPathAssigned",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaskForVisitId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoPathAssigned", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhotoPathAssigned_TaskForVisit_TaskForVisitId",
                        column: x => x.TaskForVisitId,
                        principalTable: "TaskForVisit",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PhotoPathReporting",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaskForVisitId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoPathReporting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhotoPathReporting_TaskForVisit_TaskForVisitId",
                        column: x => x.TaskForVisitId,
                        principalTable: "TaskForVisit",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Visits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Calendar = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Time = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DistributorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GuestOfVisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PurposeOfVisit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaskForVisitId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Visits_Distributors_DistributorId",
                        column: x => x.DistributorId,
                        principalTable: "Distributors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Visits_StaffUsers_CreateBy",
                        column: x => x.CreateBy,
                        principalTable: "StaffUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Visits_StaffUsers_GuestOfVisitId",
                        column: x => x.GuestOfVisitId,
                        principalTable: "StaffUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Visits_TaskForVisit_TaskForVisitId",
                        column: x => x.TaskForVisitId,
                        principalTable: "TaskForVisit",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GrantPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Module = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Permission = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DistributorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StaffUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrantPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GrantPermissions_Distributors_DistributorId",
                        column: x => x.DistributorId,
                        principalTable: "Distributors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GrantPermissions_StaffUsers_StaffUserId",
                        column: x => x.StaffUserId,
                        principalTable: "StaffUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GrantPermissions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_TaskId",
                table: "Comments",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Distributors_AreaId",
                table: "Distributors",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Distributors_CreateBy",
                table: "Distributors",
                column: "CreateBy");

            migrationBuilder.CreateIndex(
                name: "IX_Distributors_NotificationId",
                table: "Distributors",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_Distributors_PositionId",
                table: "Distributors",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Distributors_SaleManagementId",
                table: "Distributors",
                column: "SaleManagementId");

            migrationBuilder.CreateIndex(
                name: "IX_Distributors_SalesId",
                table: "Distributors",
                column: "SalesId");

            migrationBuilder.CreateIndex(
                name: "IX_GrantPermissions_DistributorId",
                table: "GrantPermissions",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_GrantPermissions_StaffUserId",
                table: "GrantPermissions",
                column: "StaffUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GrantPermissions_UserId",
                table: "GrantPermissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoPathAssigned_TaskForVisitId",
                table: "PhotoPathAssigned",
                column: "TaskForVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoPathReporting_TaskForVisitId",
                table: "PhotoPathReporting",
                column: "TaskForVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_PositionGroupId",
                table: "Positions",
                column: "PositionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CreateBy",
                table: "Posts",
                column: "CreateBy");

            migrationBuilder.CreateIndex(
                name: "IX_StaffUsers_AreaId",
                table: "StaffUsers",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffUsers_NotificationId",
                table: "StaffUsers",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffUsers_PositionId",
                table: "StaffUsers",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffUserStaffUser_StaffSuperiorId",
                table: "StaffUserStaffUser",
                column: "StaffSuperiorId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskForVisit_AssignedStaffUserId",
                table: "TaskForVisit",
                column: "AssignedStaffUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskForVisit_ReportingStaffUserId",
                table: "TaskForVisit",
                column: "ReportingStaffUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AreaId",
                table: "Users",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreateBy",
                table: "Users",
                column: "CreateBy");

            migrationBuilder.CreateIndex(
                name: "IX_Users_NotificationId",
                table: "Users",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PositionId",
                table: "Users",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_CreateBy",
                table: "Visits",
                column: "CreateBy");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_DistributorId",
                table: "Visits",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_GuestOfVisitId",
                table: "Visits",
                column: "GuestOfVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_TaskForVisitId",
                table: "Visits",
                column: "TaskForVisitId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiTokens");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "GrantPermissions");

            migrationBuilder.DropTable(
                name: "Medias");

            migrationBuilder.DropTable(
                name: "PhotoPathAssigned");

            migrationBuilder.DropTable(
                name: "PhotoPathReporting");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "StaffUserStaffUser");

            migrationBuilder.DropTable(
                name: "Visits");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Distributors");

            migrationBuilder.DropTable(
                name: "TaskForVisit");

            migrationBuilder.DropTable(
                name: "StaffUsers");

            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "PositionGroups");
        }
    }
}
