using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Canopy.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    User_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    User_Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    User_Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    User_Nickname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    User_Img = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    User_Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    User_DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    User_LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.User_Id);
                });

            migrationBuilder.CreateTable(
                name: "Group",
                columns: table => new
                {
                    Group_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Group_Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Group_Creator = table.Column<int>(type: "int", nullable: false),
                    Group_DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.Group_Id);
                    table.ForeignKey(
                        name: "FK_Group_User_Group_Creator",
                        column: x => x.Group_Creator,
                        principalTable: "User",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Notification_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    Notification_Type = table.Column<int>(type: "int", nullable: false),
                    Notification_Payload = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Is_Read = table.Column<bool>(type: "bit", nullable: false),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Notification_Id);
                    table.ForeignKey(
                        name: "FK_Notifications_User_User_Id",
                        column: x => x.User_Id,
                        principalTable: "User",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSecurity",
                columns: table => new
                {
                    UserSecurity_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    Failed_Login_Attempts = table.Column<int>(type: "int", nullable: false),
                    Lockout_Until = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Last_Failed_Attempt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Password_Changed_Date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Two_Factor_Enabled = table.Column<bool>(type: "bit", nullable: false),
                    Two_Factor_Secret = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Recovery_Codes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Security_Questions_Answered = table.Column<bool>(type: "bit", nullable: false),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Modified_Date = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSecurity", x => x.UserSecurity_Id);
                    table.ForeignKey(
                        name: "FK_UserSecurity_User_User_Id",
                        column: x => x.User_Id,
                        principalTable: "User",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Chat",
                columns: table => new
                {
                    Chat_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Chat_Group = table.Column<int>(type: "int", nullable: false),
                    Chat_DateStarted = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Chat_IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chat", x => x.Chat_Id);
                    table.ForeignKey(
                        name: "FK_Chat_Group_Chat_Group",
                        column: x => x.Chat_Group,
                        principalTable: "Group",
                        principalColumn: "Group_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    Project_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Project_Creator = table.Column<int>(type: "int", nullable: false),
                    Project_Group = table.Column<int>(type: "int", nullable: true),
                    Project_Status = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Project_Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Project_Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Project_DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Project_Deadline = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.Project_Id);
                    table.ForeignKey(
                        name: "FK_Project_Group",
                        column: x => x.Project_Group,
                        principalTable: "Group",
                        principalColumn: "Group_Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Project_User",
                        column: x => x.Project_Creator,
                        principalTable: "User",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserGroups",
                columns: table => new
                {
                    UserGroup_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    Group_Id = table.Column<int>(type: "int", nullable: false),
                    InvitedBy_Id = table.Column<int>(type: "int", nullable: false),
                    Invited_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Joined_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Role_In_Group = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false),
                    Invitation_Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroups", x => x.UserGroup_Id);
                    table.ForeignKey(
                        name: "FK_UserGroups_Group_Group_Id",
                        column: x => x.Group_Id,
                        principalTable: "Group",
                        principalColumn: "Group_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGroups_User_InvitedBy_Id",
                        column: x => x.InvitedBy_Id,
                        principalTable: "User",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserGroups_User_User_Id",
                        column: x => x.User_Id,
                        principalTable: "User",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    Message_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message_Chat = table.Column<int>(type: "int", nullable: false),
                    Message_User = table.Column<int>(type: "int", nullable: false),
                    Message_Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message_Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message_DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.Message_Id);
                    table.ForeignKey(
                        name: "FK_Message_Chat",
                        column: x => x.Message_Chat,
                        principalTable: "Chat",
                        principalColumn: "Chat_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Message_User",
                        column: x => x.Message_User,
                        principalTable: "User",
                        principalColumn: "User_Id");
                });

            migrationBuilder.CreateTable(
                name: "PlannedTask",
                columns: table => new
                {
                    Task_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Task_Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Task_Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Task_Status = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Task_Creator = table.Column<int>(type: "int", nullable: false),
                    Task_Group = table.Column<int>(type: "int", nullable: true),
                    Task_Project = table.Column<int>(type: "int", nullable: true),
                    Task_AssignedTo = table.Column<int>(type: "int", nullable: false),
                    Task_DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Task_DeadLine = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlannedTask", x => x.Task_Id);
                    table.ForeignKey(
                        name: "FK_PlannedTask_AssignedTo",
                        column: x => x.Task_AssignedTo,
                        principalTable: "User",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlannedTask_Group",
                        column: x => x.Task_Group,
                        principalTable: "Group",
                        principalColumn: "Group_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlannedTask_Project",
                        column: x => x.Task_Project,
                        principalTable: "Project",
                        principalColumn: "Project_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlannedTask_User",
                        column: x => x.Task_Creator,
                        principalTable: "User",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectMember",
                columns: table => new
                {
                    ProjectMember_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Project_Id = table.Column<int>(type: "int", nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Added_Date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMember", x => x.ProjectMember_Id);
                    table.ForeignKey(
                        name: "FK_ProjectMember_Project",
                        column: x => x.Project_Id,
                        principalTable: "Project",
                        principalColumn: "Project_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectMember_User",
                        column: x => x.User_Id,
                        principalTable: "User",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageAttachments",
                columns: table => new
                {
                    Attachment_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message_Id = table.Column<int>(type: "int", nullable: false),
                    File_Path = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    File_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    File_Size_Bytes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mime_Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Uploaded_Date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageAttachments", x => x.Attachment_Id);
                    table.ForeignKey(
                        name: "FK_MessageAttachment_Message",
                        column: x => x.Message_Id,
                        principalTable: "Message",
                        principalColumn: "Message_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageSeenStatus",
                columns: table => new
                {
                    Message_Id = table.Column<int>(type: "int", nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    Seen_Date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageSeenStatus", x => new { x.Message_Id, x.User_Id });
                    table.ForeignKey(
                        name: "FK_MessageSeenStatus_Message",
                        column: x => x.Message_Id,
                        principalTable: "Message",
                        principalColumn: "Message_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageSeenStatus_User",
                        column: x => x.User_Id,
                        principalTable: "User",
                        principalColumn: "User_Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chat_Chat_Group",
                table: "Chat",
                column: "Chat_Group");

            migrationBuilder.CreateIndex(
                name: "IX_Group_Group_Creator",
                table: "Group",
                column: "Group_Creator");

            migrationBuilder.CreateIndex(
                name: "IX_Message_Message_Chat",
                table: "Message",
                column: "Message_Chat");

            migrationBuilder.CreateIndex(
                name: "IX_Message_Message_User",
                table: "Message",
                column: "Message_User");

            migrationBuilder.CreateIndex(
                name: "IX_MessageAttachments_Message_Id",
                table: "MessageAttachments",
                column: "Message_Id");

            migrationBuilder.CreateIndex(
                name: "IX_MessageSeenStatus_User_Id",
                table: "MessageSeenStatus",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_User_Id",
                table: "Notifications",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PlannedTask_Task_AssignedTo",
                table: "PlannedTask",
                column: "Task_AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_PlannedTask_Task_Creator",
                table: "PlannedTask",
                column: "Task_Creator");

            migrationBuilder.CreateIndex(
                name: "IX_PlannedTask_Task_Group",
                table: "PlannedTask",
                column: "Task_Group");

            migrationBuilder.CreateIndex(
                name: "IX_PlannedTask_Task_Project",
                table: "PlannedTask",
                column: "Task_Project");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Project_Creator",
                table: "Project",
                column: "Project_Creator");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Project_Group",
                table: "Project",
                column: "Project_Group");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Project_Title",
                table: "Project",
                column: "Project_Title");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMember_Is_Active",
                table: "ProjectMember",
                column: "Is_Active");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMember_Unique_Project_User",
                table: "ProjectMember",
                columns: new[] { "Project_Id", "User_Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMember_User_Id",
                table: "ProjectMember",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_User_User_Email",
                table: "User",
                column: "User_Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_User_UserName",
                table: "User",
                column: "User_UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_Group_Id",
                table: "UserGroups",
                column: "Group_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_InvitedBy_Id",
                table: "UserGroups",
                column: "InvitedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_User_Id",
                table: "UserGroups",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserSecurity_User_Id",
                table: "UserSecurity",
                column: "User_Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSecurity_UserSecurity_Id",
                table: "UserSecurity",
                column: "UserSecurity_Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageAttachments");

            migrationBuilder.DropTable(
                name: "MessageSeenStatus");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PlannedTask");

            migrationBuilder.DropTable(
                name: "ProjectMember");

            migrationBuilder.DropTable(
                name: "UserGroups");

            migrationBuilder.DropTable(
                name: "UserSecurity");

            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropTable(
                name: "Chat");

            migrationBuilder.DropTable(
                name: "Group");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}

