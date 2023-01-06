using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Social.Entity.Migrations
{
    public partial class intial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    UserloginId = table.Column<string>(nullable: true),
                    logintypevalue = table.Column<string>(nullable: true),
                    DisplayedUserName = table.Column<string>(nullable: true),
                    EmailConfirmedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BWErrorLog",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Api = table.Column<string>(nullable: true),
                    Method = table.Column<string>(nullable: true),
                    ApiParams = table.Column<string>(nullable: true),
                    Exception = table.Column<string>(nullable: true),
                    ExMsg = table.Column<string>(nullable: true),
                    ExStackTrace = table.Column<string>(nullable: true),
                    InnerException = table.Column<string>(nullable: true),
                    InnerExMsg = table.Column<string>(nullable: true),
                    InnerExStackTrace = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BWErrorLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "category",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityId = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    image = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventColor",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    emptycolor = table.Column<string>(nullable: true),
                    middlecolor = table.Column<string>(nullable: true),
                    crowdedcolor = table.Column<string>(nullable: true),
                    emptynumber = table.Column<int>(nullable: false),
                    middlenumber = table.Column<int>(nullable: false),
                    count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventColor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Interests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityId = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserCodeCheck",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true),
                    Code = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCodeCheck", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LoggedinUser",
                columns: table => new
                {
                    PrimaryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ExpiredOn = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    ProjectId = table.Column<int>(nullable: true),
                    PlatformId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoggedinUser", x => x.PrimaryId);
                    table.ForeignKey(
                        name: "FK_LoggedinUser_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserDetails",
                columns: table => new
                {
                    PrimaryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    ManagerId = table.Column<string>(nullable: true),
                    userName = table.Column<string>(nullable: true),
                    phone = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    UserImage = table.Column<string>(nullable: true),
                    FcmToken = table.Column<string>(nullable: true),
                    pasword = table.Column<string>(nullable: true),
                    lang = table.Column<string>(nullable: true),
                    lat = table.Column<string>(nullable: true),
                    logintype = table.Column<int>(nullable: false),
                    platform = table.Column<int>(nullable: false),
                    userlogintypeid = table.Column<string>(nullable: true),
                    bio = table.Column<string>(nullable: true),
                    Facebook = table.Column<string>(nullable: true),
                    instagram = table.Column<string>(nullable: true),
                    snapchat = table.Column<string>(nullable: true),
                    tiktok = table.Column<string>(nullable: true),
                    pushnotification = table.Column<bool>(nullable: false),
                    allowmylocation = table.Column<bool>(nullable: false),
                    Manualdistancecontrol = table.Column<decimal>(nullable: false),
                    ageto = table.Column<int>(nullable: false),
                    agefrom = table.Column<int>(nullable: false),
                    Filteringaccordingtoage = table.Column<bool>(nullable: false),
                    allowmylocationtype = table.Column<int>(nullable: false),
                    ghostmode = table.Column<bool>(nullable: false),
                    language = table.Column<string>(nullable: true),
                    Gender = table.Column<string>(nullable: true),
                    birthdate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDetails", x => x.PrimaryId);
                    table.ForeignKey(
                        name: "FK_UserDetails_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityId = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    categorieId = table.Column<int>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    status = table.Column<string>(nullable: true),
                    image = table.Column<string>(nullable: true),
                    category = table.Column<string>(nullable: true),
                    lang = table.Column<string>(nullable: true),
                    lat = table.Column<string>(nullable: true),
                    totalnumbert = table.Column<int>(nullable: false),
                    allday = table.Column<bool>(nullable: true),
                    eventdate = table.Column<DateTime>(nullable: true),
                    eventdateto = table.Column<DateTime>(nullable: true),
                    eventfrom = table.Column<TimeSpan>(nullable: true),
                    eventto = table.Column<TimeSpan>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventData_UserDetails_UserId",
                        column: x => x.UserId,
                        principalTable: "UserDetails",
                        principalColumn: "PrimaryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventData_category_categorieId",
                        column: x => x.categorieId,
                        principalTable: "category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FireBaseDatamodel",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    imageUrl = table.Column<string>(nullable: true),
                    Messagetype = table.Column<int>(nullable: true),
                    userid = table.Column<int>(nullable: true),
                    Action_code = table.Column<string>(nullable: true),
                    muit = table.Column<bool>(nullable: false),
                    Action = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FireBaseDatamodel", x => x.id);
                    table.ForeignKey(
                        name: "FK_FireBaseDatamodel_UserDetails_userid",
                        column: x => x.userid,
                        principalTable: "UserDetails",
                        principalColumn: "PrimaryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LinkAccount",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityId = table.Column<string>(nullable: true),
                    LinkAccountname = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    LinkAccounturl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LinkAccount_UserDetails_UserId",
                        column: x => x.UserId,
                        principalTable: "UserDetails",
                        principalColumn: "PrimaryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "listoftags",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityId = table.Column<string>(nullable: true),
                    Tagsname = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    InterestsId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_listoftags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_listoftags_Interests_InterestsId",
                        column: x => x.InterestsId,
                        principalTable: "Interests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_listoftags_UserDetails_UserId",
                        column: x => x.UserId,
                        principalTable: "UserDetails",
                        principalColumn: "PrimaryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Requestes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityId = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    UserRequestId = table.Column<int>(nullable: true),
                    UserblockId = table.Column<int>(nullable: true),
                    blockDate = table.Column<DateTime>(nullable: true),
                    status = table.Column<int>(nullable: false),
                    regestdata = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requestes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requestes_UserDetails_UserId",
                        column: x => x.UserId,
                        principalTable: "UserDetails",
                        principalColumn: "PrimaryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Requestes_UserDetails_UserRequestId",
                        column: x => x.UserRequestId,
                        principalTable: "UserDetails",
                        principalColumn: "PrimaryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Requestes_UserDetails_UserblockId",
                        column: x => x.UserblockId,
                        principalTable: "UserDetails",
                        principalColumn: "PrimaryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserMessages",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    ToUserId = table.Column<int>(nullable: false),
                    startedin = table.Column<DateTime>(nullable: false),
                    muit = table.Column<string>(nullable: true),
                    Tomuit = table.Column<string>(nullable: true),
                    deleteTime = table.Column<TimeSpan>(nullable: true),
                    UserdeleteTime = table.Column<TimeSpan>(nullable: true),
                    delete = table.Column<string>(nullable: true),
                    Todelete = table.Column<string>(nullable: true),
                    deletedate = table.Column<DateTime>(nullable: true),
                    Userdeletedate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserMessages_UserDetails_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "UserDetails",
                        principalColumn: "PrimaryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserMessages_UserDetails_UserId",
                        column: x => x.UserId,
                        principalTable: "UserDetails",
                        principalColumn: "PrimaryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "eventattend",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityId = table.Column<string>(nullable: true),
                    UserattendId = table.Column<int>(nullable: true),
                    stutus = table.Column<int>(nullable: false),
                    JoinDate = table.Column<DateTime>(nullable: true),
                    deletefromeventDate = table.Column<DateTime>(nullable: true),
                    deletefromeventtime = table.Column<TimeSpan>(nullable: true),
                    leveeventchatDate = table.Column<DateTime>(nullable: true),
                    leveeventchattime = table.Column<TimeSpan>(nullable: true),
                    EventDataid = table.Column<int>(nullable: false),
                    note = table.Column<string>(nullable: true),
                    muit = table.Column<bool>(nullable: false),
                    deletedate = table.Column<DateTime>(nullable: true),
                    delettime = table.Column<TimeSpan>(nullable: true),
                    InterestsId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_eventattend", x => x.Id);
                    table.ForeignKey(
                        name: "FK_eventattend_EventData_EventDataid",
                        column: x => x.EventDataid,
                        principalTable: "EventData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_eventattend_Interests_InterestsId",
                        column: x => x.InterestsId,
                        principalTable: "Interests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_eventattend_UserDetails_UserattendId",
                        column: x => x.UserattendId,
                        principalTable: "UserDetails",
                        principalColumn: "PrimaryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventChat",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    EventDataId = table.Column<int>(nullable: false),
                    muit = table.Column<bool>(nullable: false),
                    startedin = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventChat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventChat_EventData_EventDataId",
                        column: x => x.EventDataId,
                        principalTable: "EventData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventChat_UserDetails_UserId",
                        column: x => x.UserId,
                        principalTable: "UserDetails",
                        principalColumn: "PrimaryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventChatAttend",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityId = table.Column<string>(nullable: true),
                    UserattendId = table.Column<int>(nullable: true),
                    stutus = table.Column<int>(nullable: false),
                    ISAdmin = table.Column<bool>(nullable: false),
                    JoinDate = table.Column<DateTime>(nullable: true),
                    Jointime = table.Column<TimeSpan>(nullable: true),
                    deletechatDate = table.Column<DateTime>(nullable: true),
                    deletechattime = table.Column<TimeSpan>(nullable: true),
                    leaveeventDate = table.Column<DateTime>(nullable: true),
                    leaveeventtime = table.Column<TimeSpan>(nullable: true),
                    leveeventchatDate = table.Column<DateTime>(nullable: true),
                    leveeventchattime = table.Column<TimeSpan>(nullable: true),
                    delete = table.Column<bool>(nullable: false),
                    removefromevent = table.Column<bool>(nullable: false),
                    leave = table.Column<bool>(nullable: false),
                    leavechat = table.Column<bool>(nullable: false),
                    EventDataid = table.Column<int>(nullable: false),
                    note = table.Column<string>(nullable: true),
                    muit = table.Column<bool>(nullable: false),
                    deletedate = table.Column<DateTime>(nullable: true),
                    delettime = table.Column<TimeSpan>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventChatAttend", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventChatAttend_EventData_EventDataid",
                        column: x => x.EventDataid,
                        principalTable: "EventData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventChatAttend_UserDetails_UserattendId",
                        column: x => x.UserattendId,
                        principalTable: "UserDetails",
                        principalColumn: "PrimaryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Messagedata",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserMessagessId = table.Column<string>(nullable: true),
                    Messagetype = table.Column<int>(nullable: false),
                    eventjoin = table.Column<bool>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    EventChatAttendId = table.Column<int>(nullable: true),
                    Messages = table.Column<string>(nullable: true),
                    MessagesAttached = table.Column<string>(nullable: true),
                    EventChatID = table.Column<string>(nullable: true),
                    Messagesdate = table.Column<DateTime>(nullable: false),
                    Messagestime = table.Column<TimeSpan>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messagedata", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messagedata_EventChatAttend_EventChatAttendId",
                        column: x => x.EventChatAttendId,
                        principalTable: "EventChatAttend",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messagedata_EventChat_EventChatID",
                        column: x => x.EventChatID,
                        principalTable: "EventChat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messagedata_UserDetails_UserId",
                        column: x => x.UserId,
                        principalTable: "UserDetails",
                        principalColumn: "PrimaryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messagedata_UserMessages_UserMessagessId",
                        column: x => x.UserMessagessId,
                        principalTable: "UserMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MessageAttached",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    MessagedataId = table.Column<string>(nullable: true),
                    Messages = table.Column<string>(nullable: true),
                    attached = table.Column<string>(nullable: true),
                    Messagesdate = table.Column<DateTime>(nullable: false),
                    Messagestime = table.Column<TimeSpan>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageAttached", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageAttached_Messagedata_MessagedataId",
                        column: x => x.MessagedataId,
                        principalTable: "Messagedata",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_eventattend_EventDataid",
                table: "eventattend",
                column: "EventDataid");

            migrationBuilder.CreateIndex(
                name: "IX_eventattend_InterestsId",
                table: "eventattend",
                column: "InterestsId");

            migrationBuilder.CreateIndex(
                name: "IX_eventattend_UserattendId",
                table: "eventattend",
                column: "UserattendId");

            migrationBuilder.CreateIndex(
                name: "IX_EventChat_EventDataId",
                table: "EventChat",
                column: "EventDataId");

            migrationBuilder.CreateIndex(
                name: "IX_EventChat_UserId",
                table: "EventChat",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventChatAttend_EventDataid",
                table: "EventChatAttend",
                column: "EventDataid");

            migrationBuilder.CreateIndex(
                name: "IX_EventChatAttend_UserattendId",
                table: "EventChatAttend",
                column: "UserattendId");

            migrationBuilder.CreateIndex(
                name: "IX_EventData_UserId",
                table: "EventData",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventData_categorieId",
                table: "EventData",
                column: "categorieId");

            migrationBuilder.CreateIndex(
                name: "IX_FireBaseDatamodel_userid",
                table: "FireBaseDatamodel",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_LinkAccount_UserId",
                table: "LinkAccount",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_listoftags_InterestsId",
                table: "listoftags",
                column: "InterestsId");

            migrationBuilder.CreateIndex(
                name: "IX_listoftags_UserId",
                table: "listoftags",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LoggedinUser_UserId",
                table: "LoggedinUser",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageAttached_MessagedataId",
                table: "MessageAttached",
                column: "MessagedataId");

            migrationBuilder.CreateIndex(
                name: "IX_Messagedata_EventChatAttendId",
                table: "Messagedata",
                column: "EventChatAttendId");

            migrationBuilder.CreateIndex(
                name: "IX_Messagedata_EventChatID",
                table: "Messagedata",
                column: "EventChatID");

            migrationBuilder.CreateIndex(
                name: "IX_Messagedata_UserId",
                table: "Messagedata",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Messagedata_UserMessagessId",
                table: "Messagedata",
                column: "UserMessagessId");

            migrationBuilder.CreateIndex(
                name: "IX_Requestes_UserId",
                table: "Requestes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Requestes_UserRequestId",
                table: "Requestes",
                column: "UserRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Requestes_UserblockId",
                table: "Requestes",
                column: "UserblockId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDetails_UserId",
                table: "UserDetails",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserMessages_ToUserId",
                table: "UserMessages",
                column: "ToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMessages_UserId",
                table: "UserMessages",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BWErrorLog");

            migrationBuilder.DropTable(
                name: "eventattend");

            migrationBuilder.DropTable(
                name: "EventColor");

            migrationBuilder.DropTable(
                name: "FireBaseDatamodel");

            migrationBuilder.DropTable(
                name: "LinkAccount");

            migrationBuilder.DropTable(
                name: "listoftags");

            migrationBuilder.DropTable(
                name: "LoggedinUser");

            migrationBuilder.DropTable(
                name: "MessageAttached");

            migrationBuilder.DropTable(
                name: "Requestes");

            migrationBuilder.DropTable(
                name: "UserCodeCheck");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Interests");

            migrationBuilder.DropTable(
                name: "Messagedata");

            migrationBuilder.DropTable(
                name: "EventChatAttend");

            migrationBuilder.DropTable(
                name: "EventChat");

            migrationBuilder.DropTable(
                name: "UserMessages");

            migrationBuilder.DropTable(
                name: "EventData");

            migrationBuilder.DropTable(
                name: "UserDetails");

            migrationBuilder.DropTable(
                name: "category");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
