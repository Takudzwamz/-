using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Faculty.Data.Migrations
{
    public partial class Model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.CreateTable(
                name: "Cathedras",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cathedras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarkValues",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarkValues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    CathedraId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_AspNetUsers_AccountId",
                        column: x => x.AccountId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Cathedras_CathedraId",
                        column: x => x.CathedraId,
                        principalTable: "Cathedras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    CourseId = table.Column<int>(nullable: false),
                    CathedraId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Groups_Cathedras_CathedraId",
                        column: x => x.CathedraId,
                        principalTable: "Cathedras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Groups_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Statements",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(nullable: false),
                    SubjectId = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false),
                    PeriodBegin = table.Column<DateTime>(nullable: false),
                    PeriodEnd = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Statements_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Statements_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Statements_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    PaymentTypeId = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    GroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_AspNetUsers_AccountId",
                        column: x => x.AccountId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Students_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Students_PaymentTypes_PaymentTypeId",
                        column: x => x.PaymentTypeId,
                        principalTable: "PaymentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Marks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatementId = table.Column<int>(nullable: false),
                    StudentId = table.Column<int>(nullable: false),
                    MarkValueId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Marks_MarkValues_MarkValueId",
                        column: x => x.MarkValueId,
                        principalTable: "MarkValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Marks_Statements_StatementId",
                        column: x => x.StatementId,
                        principalTable: "Statements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Marks_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Cathedras",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Кафедра Математического и Прикладного Анализа" },
                    { 2, "Кафедра Вычислительной Математики и Прикладных Информационных Технологий" },
                    { 3, "Кафедра Математических Методов Исследования Операций" },
                    { 4, "Кафедра Математического Обеспечения ЭВМ" },
                    { 5, "Кафедра ERP-систем и бизнес процессов" },
                    { 6, "Кафедра Системного анализа и управления" },
                    { 7, "Кафедра Программного Обеспечения и Администрирования Информационных Систем" },
                    { 8, "Лаборатория Вычислительной Техники" },
                    { 9, "Кафедра Механики и компьютерного моделирования" }
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "Number" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 },
                    { 3, 3 },
                    { 4, 4 },
                    { 5, 5 }
                });

            migrationBuilder.InsertData(
                table: "MarkValues",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 5, "Отлично" },
                    { 4, "Хорошо" },
                    { 2, "Неудовлетворительно" },
                    { 3, "Удовлетворительно" }
                });

            migrationBuilder.InsertData(
                table: "PaymentTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Очная" },
                    { 2, "Заочная" }
                });

            migrationBuilder.InsertData(
                table: "Subjects",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 33, "Компьютерные сети" },
                    { 25, "Математическая статистика" },
                    { 26, "Вычислительные методы" },
                    { 27, "Архитектура современных микропроцессоров" },
                    { 28, "Проектирование информационных систем" },
                    { 29, "Физика" },
                    { 30, "Архитектура мобильных устройств" },
                    { 31, "Программирование на С#" },
                    { 32, "Математические основы компьютерной графики" },
                    { 34, "Компьютерная графика" },
                    { 44, "Методы компиляции" },
                    { 36, " Методы оптимизации" },
                    { 37, "Программируемые микроконтроллеры" },
                    { 38, "Java-программирование" },
                    { 39, "Введение в язык программирования Python" },
                    { 40, "Введение в Unix" },
                    { 41, "Информационная безопасность и защита информации" },
                    { 42, "Прикладная теория графов" },
                    { 43, "Философия" },
                    { 24, "Экономика" },
                    { 35, "Основы облачных вычислений" },
                    { 23, "Операционные системы" },
                    { 13, "Архитектура вычислительных систем" },
                    { 21, "Физическая культура" },
                    { 1, "Русский язык для устной и письменной коммуникации" },
                    { 2, "Физическая культура" },
                    { 3, "Иностранный язык " },
                    { 4, "Линейная алгебра" },
                    { 5, "Математический анализ" },
                    { 6, "Информатика и программирование" },
                    { 7, "Дискретная математика" },
                    { 8, "Практикум на ЭВМ по программированию" },
                    { 9, "Аналитическая геометрия" },
                    { 22, "Неклассические логики" },
                    { 10, "Пакеты прикладных программ" },
                    { 12, "Линейная алгебра" },
                    { 45, "Реляционные СУБД" },
                    { 14, "Алгоритмы и анализ сложности" },
                    { 15, "Правоведение" },
                    { 16, "Теория вероятностей" },
                    { 17, "Языки и методы программирования" },
                    { 18, "Математическая логика и теория алгоритмов" },
                    { 19, "Базы данных" },
                    { 20, "Дифференциальные уравнения" },
                    { 11, "История" },
                    { 46, "Введение в UML-технологии" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "AccountId", "CathedraId", "Name" },
                values: new object[,]
                {
                    { 7, null, 1, "Комиссаров Августин Мэлсович" },
                    { 54, null, 7, "Щукин Юстин Валентинович" },
                    { 45, null, 7, "Некрасов Артур Мэлсович" },
                    { 41, null, 7, "Поляков Гордей Пантелеймонович" },
                    { 38, null, 7, "Зуев Елисей Дмитриевич" },
                    { 27, null, 7, "Медведев Роберт Куприянович" },
                    { 24, null, 7, "Панфилов Болеслав Протасьевич" },
                    { 11, null, 7, "Гурьев Трофим Федорович" },
                    { 6, null, 7, "Блинов Иосиф Адольфович" },
                    { 58, null, 7, "Цветков Семен Даниилович" },
                    { 65, null, 6, "Сергеев Овидий Христофорович" },
                    { 62, null, 6, "Филатов Аввакуум Пантелеймонович" },
                    { 59, null, 6, "Тимофеев Емельян Аркадьевич" },
                    { 55, null, 6, "Муравьёв Владислав Федорович" },
                    { 52, null, 6, "Суворов Моисей Станиславович" },
                    { 50, null, 6, "Фролов Вениамин Всеволодович" },
                    { 48, null, 6, "Горшков Тарас Петрович" },
                    { 34, null, 6, "Панов Вилен Христофорович" },
                    { 17, null, 6, "Соболев Федор Федотович" },
                    { 64, null, 6, "Архипов Аввакум Геннадьевич" },
                    { 9, null, 6, "Русаков Константин Денисович" },
                    { 60, null, 7, "Якушев Лев Никитевич" },
                    { 75, null, 7, "Харитонов Исак Михайлович" },
                    { 85, null, 9, "Шарапов Харитон Федотович" },
                    { 83, null, 9, "Лыткин Роберт Донатович" },
                    { 79, null, 9, "Фомичёв Александр Филиппович" },
                    { 72, null, 9, "Лазарев Лев Леонидович" },
                    { 69, null, 9, "Шаров Владлен Романович" },
                    { 61, null, 9, "Денисов Кирилл Юрьевич" },
                    { 51, null, 9, "Лазарев Ярослав Леонидович" },
                    { 32, null, 9, "Шубин Федор Геннадиевич" },
                    { 66, null, 7, "Рогов Виктор Эдуардович" },
                    { 30, null, 9, "Смирнов Ефим Богданович" },
                    { 10, null, 9, "Крылов Мстислав Владимирович" },
                    { 68, null, 8, "Козлов Нинель Константинович" },
                    { 39, null, 8, "Афанасьев Герасим Фролович" },
                    { 8, null, 8, "Исаков Юлий Павлович" },
                    { 2, null, 8, "Аксёнов Казимир Ярославович" },
                    { 87, null, 7, "Ермаков Вадим Валентинович" },
                    { 86, null, 7, "Борисов Лука Андреевич" },
                    { 84, null, 7, "Крюков Самуил Васильевич" },
                    { 18, null, 9, "Блохин Константин Васильевич" },
                    { 1, null, 6, "Абрамов Елисей Владленович" },
                    { 25, null, 6, "Максимов Тарас Ростиславович" },
                    { 63, null, 5, "Емельянов Рудольф Данилович" },
                    { 47, null, 3, "Мухин Власий Евсеевич" },
                    { 67, null, 5, "Сергеев Велорий Созонович" },
                    { 31, null, 3, "Власов Ибрагил Авксентьевич" },
                    { 22, null, 3, "Баранов Антон Иосифович" },
                    { 4, null, 3, "Фролов Рудольф Матвеевич" },
                    { 46, null, 2, "Кудрявцев Вадим Владимирович" },
                    { 42, null, 2, "Буров Макар Феликсович" },
                    { 37, null, 2, "Пестов Сергей Протасьевич" },
                    { 5, null, 2, "Ершов Константин Тимурович" },
                    { 82, null, 1, "Королёв Максимилиан Вадимович" },
                    { 81, null, 1, "Кошелев Валентин Мартынович" },
                    { 78, null, 1, "Кулагин Абрам Максович" },
                    { 74, null, 1, "Галкин Митрофан Валерьянович" },
                    { 73, null, 1, "Никонов Оскар Артёмович" },
                    { 70, null, 1, "Пономарёв Леонард Андреевич" },
                    { 43, null, 1, "Ершов Петр Авдеевич" },
                    { 40, null, 1, "Костин Рудольф Алексеевич" },
                    { 33, null, 1, "Белов Валерий Донатович" },
                    { 19, null, 1, "Антонов Лев Тимофеевич" },
                    { 53, null, 3, "Шубин Натан Александрович" },
                    { 56, null, 3, "Лаврентьев Яков Всеволодович" },
                    { 36, null, 3, "Кузьмин Аскольд Эльдарович" },
                    { 76, null, 3, "Васильев Мечеслав Улебович" },
                    { 49, null, 5, "Исаев Касьян Дмитриевич" },
                    { 35, null, 5, "Жуков Панкрат Евгеньевич" },
                    { 23, null, 5, "Зимин Леонтий Дамирович" },
                    { 21, null, 5, "Воробьёв Ибрагил Авдеевич" },
                    { 71, null, 3, "Коновалов Тарас Сергеевич" },
                    { 16, null, 5, "Агафонов Виссарион Филатович" },
                    { 14, null, 5, "Ковалёв Юлий Мэлсович" },
                    { 12, null, 5, "Селезнёв Кондратий Станиславович" },
                    { 80, null, 4, "Буров Мартин Давидович" },
                    { 20, null, 5, "Русаков Вадим Робертович" },
                    { 44, null, 4, "Максимов Игнат Оскарович" },
                    { 57, null, 4, "Волков Клим Геласьевич" },
                    { 3, null, 4, "Родионов Гарри Львович" },
                    { 13, null, 4, "Савин Панкрат Давидович" },
                    { 77, null, 3, "Кононов Май Георгьевич" },
                    { 26, null, 4, "Брагин Арнольд Антонинович" },
                    { 28, null, 4, "Трофимов Эльдар Борисович" },
                    { 29, null, 4, "Панов Гурий Георгьевич" },
                    { 15, null, 4, "Зыков Лев Вениаминович" }
                });

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "CathedraId", "CourseId", "Name" },
                values: new object[,]
                {
                    { 8, 6, 5, "71 группа (БИ)" },
                    { 6, 1, 1, "5 группа (ПМиИ)" },
                    { 7, 3, 1, "61 группа (ФИиИТ)" },
                    { 4, 4, 2, "3 группа (ПМиИ)" },
                    { 1, 2, 4, "1 группа (Механика и мат. моделирование)" },
                    { 2, 4, 4, "21 группа (Фундаментальная математика и механика) УВЦ" },
                    { 5, 2, 4, "4 группа (ПМиИ)" },
                    { 3, 3, 5, "2 группа (ПМиИ)" },
                    { 9, 7, 5, "91 группа (МО и АИС)" }
                });

            migrationBuilder.InsertData(
                table: "Statements",
                columns: new[] { "Id", "EmployeeId", "GroupId", "PeriodBegin", "PeriodEnd", "SubjectId" },
                values: new object[] { 1, 1, 1, new DateTime(2020, 6, 2, 0, 43, 11, 183, DateTimeKind.Local).AddTicks(4717), new DateTime(2020, 6, 17, 0, 43, 11, 188, DateTimeKind.Local).AddTicks(363), 1 });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "AccountId", "GroupId", "IsActive", "Name", "PaymentTypeId" },
                values: new object[,]
                {
                    { 13, null, 6, true, "Фомичёв Леонид Вячеславович", 2 },
                    { 81, null, 3, true, "Кузнецов Донат Онисимович", 2 },
                    { 32, null, 3, true, "Ситников Устин Витальевич", 2 },
                    { 18, null, 3, true, "Силин Юлий Тарасович", 1 },
                    { 16, null, 3, true, "Абрамов Родион Константинович", 2 },
                    { 8, null, 3, true, "Власов Ермолай Данилович", 1 },
                    { 2, null, 3, true, "Кабанов Кирилл Григорьевич", 2 },
                    { 70, null, 5, true, "Кудрявцев Кассиан Егорович", 1 },
                    { 5, null, 8, true, "Калашников Митрофан Глебович", 2 },
                    { 68, null, 5, true, "Никитин Евгений Никитевич", 2 },
                    { 36, null, 5, true, "Кузнецов Архип Тимурович", 1 },
                    { 29, null, 5, true, "Селиверстов Артур Лукьевич", 1 },
                    { 26, null, 5, true, "Исаков Дональд Борисович", 2 },
                    { 23, null, 5, true, "Вишняков Аркадий Проклович", 2 },
                    { 6, null, 5, true, "Терентьев Юлиан Мэлорович", 2 },
                    { 73, null, 2, true, "Дорофеев Нелли Альбертович", 2 },
                    { 64, null, 2, true, "Фролов Варлаам Георгьевич", 2 },
                    { 61, null, 2, true, "Беляев Арсений Аристархович", 1 },
                    { 45, null, 5, true, "Крюков Авраам Алексеевич", 1 },
                    { 59, null, 2, true, "Трофимов Руслан Проклович", 1 },
                    { 11, null, 8, true, "Гордеев Арсен Ярославович", 1 },
                    { 20, null, 8, true, "Панов Дмитрий Артемович", 1 },
                    { 75, null, 9, true, "Горшков Эрик Георгьевич", 1 },
                    { 57, null, 9, true, "Куликов Лука Мартынович", 1 },
                    { 52, null, 9, true, "Гусев Тимофей Константинович", 1 },
                    { 50, null, 9, true, "Нестеров Афанасий Феликсович", 2 },
                    { 49, null, 9, true, "Киселёв Платон Григорьевич", 2 },
                    { 41, null, 9, true, "Лаврентьев Касьян Пантелеймонович", 2 },
                    { 39, null, 9, true, "Щербаков Константин Анатольевич", 1 },
                    { 38, null, 9, true, "Михеев Ермак Макарович", 2 },
                    { 17, null, 8, true, "Богданов Фрол Улебович", 2 },
                    { 27, null, 9, true, "Фёдоров Алексей Степанович", 1 },
                    { 19, null, 9, true, "Чернов Гордей Алексеевич", 2 },
                    { 9, null, 9, true, "Нестеров Мартин Тихонович", 1 },
                    { 77, null, 8, true, "Шаров Пантелей Созонович", 1 },
                    { 71, null, 8, true, "Туров Нелли Еремеевич", 1 },
                    { 47, null, 8, true, "Логинов Влас Данилович", 1 },
                    { 46, null, 8, true, "Мухин Исаак Геннадиевич", 2 },
                    { 43, null, 8, true, "Доронин Вальтер Степанович", 1 },
                    { 24, null, 8, true, "Григорьев Лука Адольфович", 2 },
                    { 25, null, 9, true, "Туров Герасим Миронович", 1 },
                    { 56, null, 2, true, "Емельянов Ермак Артемович", 1 },
                    { 55, null, 2, true, "Андреев Богдан Егорович", 2 },
                    { 33, null, 2, true, "Копылов Май Мэлсович", 2 },
                    { 40, null, 4, true, "Кузнецов Овидий Ростиславович", 1 },
                    { 37, null, 4, true, "Игнатов Аскольд Леонидович", 2 },
                    { 12, null, 4, true, "Суханов Матвей Кириллович", 2 },
                    { 84, null, 7, true, "Соболев Корнелий Юрьевич", 1 },
                    { 60, null, 7, true, "Уваров Максим Данилович", 2 },
                    { 44, null, 7, true, "Князев Давид Геласьевич", 2 },
                    { 42, null, 7, true, "Гуляев Даниил Эльдарович", 1 },
                    { 10, null, 7, true, "Рябов Августин Артёмович", 1 },
                    { 4, null, 7, true, "Блинов Ян Пантелеймонович", 1 },
                    { 85, null, 6, true, "Фролов Август Филиппович", 2 },
                    { 74, null, 6, true, "Мышкин Нелли Егорович", 2 },
                    { 66, null, 6, true, "Зуев Кассиан Мэлсович", 1 },
                    { 54, null, 6, true, "Фёдоров Геннадий Федосеевич", 1 },
                    { 53, null, 6, true, "Колобов Модест Яковович", 1 },
                    { 51, null, 6, true, "Соколов Ипполит Ярославович", 1 },
                    { 35, null, 6, true, "Ефремов Демьян Лукьянович", 2 },
                    { 14, null, 6, true, "Дроздов Константин Мэлсович", 2 },
                    { 58, null, 4, true, "Александров Ростислав Созонович", 1 },
                    { 65, null, 4, true, "Игнатов Ефим Геласьевич", 2 },
                    { 67, null, 4, true, "Горбунов Эдуард Тарасович", 2 },
                    { 72, null, 4, true, "Лыткин Гордий Егорович", 2 },
                    { 30, null, 2, true, "Дмитриев Родион Рудольфович", 1 },
                    { 28, null, 2, true, "Новиков Панкрат Митрофанович", 1 },
                    { 21, null, 2, true, "Наумов Герасим Данилович", 1 },
                    { 15, null, 2, true, "Марков Максимилиан Богданович", 1 },
                    { 7, null, 2, true, "Журавлёв Панкратий Глебович", 2 },
                    { 3, null, 2, true, "Антонов Карл Филатович", 2 },
                    { 1, null, 2, true, "Ефремов Сергей Иосифович", 2 },
                    { 80, null, 1, true, "Федотов Парамон Ярославович", 2 },
                    { 82, null, 9, true, "Беляков Самуил Иринеевич", 1 },
                    { 76, null, 1, true, "Копылов Геннадий Феликсович", 2 },
                    { 63, null, 1, true, "Белов Виталий Андреевич", 2 },
                    { 62, null, 1, true, "Коновалов Герасим Миронович", 1 },
                    { 48, null, 1, true, "Шарапов Агафон Христофорович", 1 },
                    { 34, null, 1, true, "Дементьев Максимилиан Тарасович", 2 },
                    { 31, null, 1, true, "Юдин Исак Онисимович", 2 },
                    { 22, null, 1, true, "Панфилов Виссарион Валентинович", 1 },
                    { 79, null, 4, true, "Журавлёв Севастьян Владленович", 2 },
                    { 78, null, 4, true, "Голубев Клим Валерьевич", 1 },
                    { 69, null, 1, true, "Максимов Кирилл Кириллович", 2 },
                    { 83, null, 9, true, "Меркушев Артем Валерьянович", 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_AccountId",
                table: "Employees",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CathedraId",
                table: "Employees",
                column: "CathedraId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_CathedraId",
                table: "Groups",
                column: "CathedraId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_CourseId",
                table: "Groups",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Marks_MarkValueId",
                table: "Marks",
                column: "MarkValueId");

            migrationBuilder.CreateIndex(
                name: "IX_Marks_StatementId",
                table: "Marks",
                column: "StatementId");

            migrationBuilder.CreateIndex(
                name: "IX_Marks_StudentId",
                table: "Marks",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Statements_EmployeeId",
                table: "Statements",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Statements_GroupId",
                table: "Statements",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Statements_SubjectId",
                table: "Statements",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_AccountId",
                table: "Students",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_GroupId",
                table: "Students",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_PaymentTypeId",
                table: "Students",
                column: "PaymentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Marks");

            migrationBuilder.DropTable(
                name: "MarkValues");

            migrationBuilder.DropTable(
                name: "Statements");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "PaymentTypes");

            migrationBuilder.DropTable(
                name: "Cathedras");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
