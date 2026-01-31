-- 1. Таблица USERS (Пользователи) - исправлено на USERS (без 2)
CREATE TABLE [dbo].[USERS] (
    [ID_USER] INT IDENTITY(1,1) PRIMARY KEY,
    [SURNAME] NVARCHAR(50) NOT NULL,
    [PATRONYMIC] NVARCHAR(50) NULL,
    [NAME] NVARCHAR(50) NOT NULL,
    [PHONE] NVARCHAR(20) NOT NULL,
    [EMAIL] NVARCHAR(100) NULL,
    [URL] NVARCHAR(255) NULL,
    [USERSTATUS] NVARCHAR(50) DEFAULT 'Активен'
)
GO

-- 2. Таблица EMPLOYEES (Сотрудники)
CREATE TABLE [dbo].[EMPLOYEES] (
    [ID_EMPLOYEE] INT IDENTITY(1,1) PRIMARY KEY,
    [SURNAME] NVARCHAR(50) NOT NULL,
    [NAME] NVARCHAR(50) NOT NULL,
    [PATRONYMIC] NVARCHAR(50) NULL,
    [POST] NVARCHAR(100) NOT NULL,
    [PHONE] NVARCHAR(20) NOT NULL,
    [EMAIL] NVARCHAR(100) NULL
)
GO

-- 3. Таблица CATALOGS (Товарные группы/Каталоги)
CREATE TABLE [dbo].[CATALOGS] (
    [ID_CATALOG] INT IDENTITY(1,1) PRIMARY KEY,
    [NAME] NVARCHAR(100) NOT NULL,
    [DESCRIPTION] NVARCHAR(500) NULL
)
GO

-- 4. Таблица PRODUCTS (Товары)
CREATE TABLE [dbo].[PRODUCTS] (
    [ID_PRODUCT] INT IDENTITY(1,1) PRIMARY KEY,
    [NAME] NVARCHAR(100) NOT NULL,
    [DESCRIPTION] NVARCHAR(500) NULL,
    [PRICE] DECIMAL(10,2) NOT NULL,
    [QUANTITY] INT DEFAULT 0,
    [RATING] DECIMAL(3,1) NULL,
    [ID_CATALOG] INT NOT NULL,
    CONSTRAINT [FK_PRODUCTS_CATALOGS] FOREIGN KEY ([ID_CATALOG]) 
        REFERENCES [CATALOGS]([ID_CATALOG]) ON DELETE CASCADE
)
GO

-- 5. Таблица ORDERS (Заказы) - ВАЖНО: связь с USERS и PRODUCTS
CREATE TABLE [dbo].[ORDERS] (
    [ID_ORDER] INT IDENTITY(1,1) PRIMARY KEY,
    [ID_USER] INT NOT NULL,
    [ID_PRODUCT] INT NOT NULL,
    [ID_EMPLOYEE] INT NOT NULL,
    [ORDER_DATE] DATETIME DEFAULT GETDATE(),
    [NUMBER] INT NOT NULL DEFAULT 1,
    [STATUS] NVARCHAR(50) DEFAULT 'Новый',
    CONSTRAINT [FK_ORDERS_USERS] FOREIGN KEY ([ID_USER]) 
        REFERENCES [USERS]([ID_USER]) ON DELETE CASCADE,
    CONSTRAINT [FK_ORDERS_PRODUCTS] FOREIGN KEY ([ID_PRODUCT]) 
        REFERENCES [PRODUCTS]([ID_PRODUCT]) ON DELETE CASCADE,
    CONSTRAINT [FK_ORDERS_EMPLOYEES] FOREIGN KEY ([ID_EMPLOYEE]) 
        REFERENCES [EMPLOYEES]([ID_EMPLOYEE]) ON DELETE CASCADE
)
GO

-- 6. Таблица BILLS (Счета)
CREATE TABLE [dbo].[BILLS] (
    [ID_BILL] INT IDENTITY(1,1) PRIMARY KEY,
    [ID_ORDER] INT NOT NULL,
    [BILL_DATE] DATETIME DEFAULT GETDATE(),
    [AMOUNT] DECIMAL(10,2) NOT NULL,
    [PAYMENT_STATUS] NVARCHAR(50) DEFAULT 'Не оплачен',
    CONSTRAINT [FK_BILLS_ORDERS] FOREIGN KEY ([ID_ORDER]) 
        REFERENCES [ORDERS]([ID_ORDER]) ON DELETE CASCADE
)
GO

-- Создаем представление ViewORDER как в методичке
CREATE VIEW [dbo].[ViewORDER] 
AS
SELECT 
    P.[NAME] AS PRODUCTSNAME,
    P.[DESCRIPTION],
    P.[PRICE],
    O.[NUMBER],
    O.[ORDER_DATE] AS ORDERTIME,
    U.[SURNAME] AS USERSURNAME,
    U.[NAME] AS USERNAME,
    U.[PATRONYMIC],
    U.[PHONE],
    E.[SURNAME] AS EMPLOYEESURNAME,
    E.[POST],
    (P.[PRICE] * O.[NUMBER]) AS ORDERSUM
FROM 
    [ORDERS] O
    INNER JOIN [PRODUCTS] P ON O.[ID_PRODUCT] = P.[ID_PRODUCT]
    INNER JOIN [EMPLOYEES] E ON O.[ID_EMPLOYEE] = E.[ID_EMPLOYEE]
    INNER JOIN [USERS] U ON O.[ID_USER] = U.[ID_USER]
GO

-- Добавляем тестовые данные
INSERT INTO [dbo].[CATALOGS] ([NAME], [DESCRIPTION]) VALUES
('Ноутбуки', 'Переносные компьютеры'),
('Смартфоны', 'Мобильные телефоны'),
('Компьютеры', 'Стационарные ПК'),
('Периферия', 'Клавиатуры, мыши, мониторы')
GO

INSERT INTO [dbo].[PRODUCTS] ([NAME], [DESCRIPTION], [PRICE], [QUANTITY], [RATING], [ID_CATALOG]) VALUES
('Ноутбук ASUS X515', '15.6", Intel Core i5, 8GB RAM', 45000.00, 10, 4.5, 1),
('iPhone 14', '128GB, Space Black', 79900.00, 5, 4.8, 2),
('ПК Gaming Pro', 'Intel i7, 16GB RAM, RTX 3060', 85000.00, 3, 4.7, 3),
('Мышь Logitech MX Master 3', 'Беспроводная, для дизайнеров', 8990.00, 20, 4.6, 4)
GO

INSERT INTO [dbo].[USERS] ([SURNAME], [PATRONYMIC], [NAME], [PHONE], [EMAIL], [URL], [USERSTATUS]) VALUES
('Иванов', 'Иванович', 'Иван', '+79161234567', 'ivanov@mail.ru', 'www.ivanov.ru', 'Активен'),
('Петрова', 'Сергеевна', 'Мария', '+79167654321', 'petrova@gmail.com', NULL, 'Активен'),
('Сидоров', 'Алексеевич', 'Алексей', '+79035556677', 'sidorov@yandex.ru', 'www.sidorov.com', 'Неактивен')
GO

INSERT INTO [dbo].[EMPLOYEES] ([SURNAME], [NAME], [PATRONYMIC], [POST], [PHONE], [EMAIL]) VALUES
('Кузнецов', 'Дмитрий', 'Викторович', 'Менеджер по продажам', '+79161112233', 'manager@shop.ru'),
('Смирнова', 'Ольга', 'Игоревна', 'Консультант', '+79162223344', 'consultant@shop.ru')
GO

-- Вставляем заказы
INSERT INTO [dbo].[ORDERS] ([ID_USER], [ID_PRODUCT], [ID_EMPLOYEE], [ORDER_DATE], [NUMBER], [STATUS]) VALUES
(1, 1, 1, '2024-01-15T10:30:00', 1, 'Выполнен'),
(1, 3, 1, '2024-01-20T14:45:00', 1, 'В обработке'),
(2, 2, 2, '2024-01-18T11:20:00', 1, 'Доставка'),
(3, 4, 2, '2024-01-22T09:15:00', 2, 'Новый')
GO

-- Вставляем счета
INSERT INTO [dbo].[BILLS] ([ID_ORDER], [BILL_DATE], [AMOUNT], [PAYMENT_STATUS]) VALUES
(1, '2024-01-15T10:35:00', 45000.00, 'Оплачен'),
(2, '2024-01-20T14:50:00', 85000.00, 'Не оплачен'),
(3, '2024-01-18T11:25:00', 79900.00, 'Оплачен')
GO

-- Создаем индексы для повышения производительности
CREATE INDEX IX_USERS_SURNAME ON [USERS]([SURNAME])
CREATE INDEX IX_PRODUCTS_NAME ON [PRODUCTS]([NAME])
CREATE INDEX IX_ORDERS_DATE ON [ORDERS]([ORDER_DATE])
CREATE INDEX IX_ORDERS_USER ON [ORDERS]([ID_USER])
CREATE INDEX IX_ORDERS_PRODUCT ON [ORDERS]([ID_PRODUCT])
GO

-- Процедура для получения статистики
CREATE PROCEDURE [dbo].[GetOrderStatistics]
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    SELECT 
        COUNT(*) AS TotalOrders,
        SUM(P.PRICE * O.NUMBER) AS TotalAmount,
        AVG(P.PRICE * O.NUMBER) AS AverageOrder
    FROM [ORDERS] O
    INNER JOIN [PRODUCTS] P ON O.ID_PRODUCT = P.ID_PRODUCT
    WHERE O.ORDER_DATE BETWEEN @StartDate AND @EndDate
END
GO

-- Триггер для автоматического создания счета при новом заказе
CREATE TRIGGER [dbo].[trg_CreateBillOnOrder]
ON [dbo].[ORDERS]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [BILLS] ([ID_ORDER], [BILL_DATE], [AMOUNT], [PAYMENT_STATUS])
    SELECT 
        i.ID_ORDER,
        GETDATE(), -- текущая дата
        (p.PRICE * i.NUMBER),
        'Не оплачен' -- статус по умолчанию
    FROM inserted i
    INNER JOIN [PRODUCTS] p ON i.ID_PRODUCT = p.ID_PRODUCT
    -- Исключаем заказы, для которых уже есть счета
    WHERE NOT EXISTS (
        SELECT 1 FROM [BILLS] b WHERE b.ID_ORDER = i.ID_ORDER
    )
END
GO

-- Функция для проверки доступности товара
CREATE FUNCTION [dbo].[CheckProductAvailability]
(
    @ProductID INT,
    @RequestedQuantity INT
)
RETURNS BIT
AS
BEGIN
    DECLARE @Available BIT = 0
    
    IF EXISTS (
        SELECT 1 
        FROM [PRODUCTS] 
        WHERE ID_PRODUCT = @ProductID 
        AND QUANTITY >= @RequestedQuantity
    )
    BEGIN
        SET @Available = 1
    END
    
    RETURN @Available
END
GO

-- Информация о таблицах для справки
DECLARE @UsersCount INT, @EmployeesCount INT, @CatalogsCount INT, @ProductsCount INT, @OrdersCount INT, @BillsCount INT

SELECT @UsersCount = COUNT(*) FROM [USERS]
SELECT @EmployeesCount = COUNT(*) FROM [EMPLOYEES]
SELECT @CatalogsCount = COUNT(*) FROM [CATALOGS]
SELECT @ProductsCount = COUNT(*) FROM [PRODUCTS]
SELECT @OrdersCount = COUNT(*) FROM [ORDERS]
SELECT @BillsCount = COUNT(*) FROM [BILLS]

PRINT '=== СОЗДАНА БАЗА ДАННЫХ "COMPUTER_SHOP" ==='
PRINT 'Таблицы:'
PRINT '1. USERS (Пользователи) - ' + CAST(@UsersCount AS VARCHAR(10)) + ' записей'
PRINT '2. EMPLOYEES (Сотрудники) - ' + CAST(@EmployeesCount AS VARCHAR(10)) + ' записей'
PRINT '3. CATALOGS (Каталоги) - ' + CAST(@CatalogsCount AS VARCHAR(10)) + ' записей'
PRINT '4. PRODUCTS (Товары) - ' + CAST(@ProductsCount AS VARCHAR(10)) + ' записей'
PRINT '5. ORDERS (Заказы) - ' + CAST(@OrdersCount AS VARCHAR(10)) + ' записей'
PRINT '6. BILLS (Счета) - ' + CAST(@BillsCount AS VARCHAR(10)) + ' записей'
PRINT ''
PRINT 'Связи:'
PRINT '- ORDERS.ID_USER -> USERS.ID_USER (1 пользователь → много заказов)'
PRINT '- ORDERS.ID_PRODUCT -> PRODUCTS.ID_PRODUCT'
PRINT '- ORDERS.ID_EMPLOYEE -> EMPLOYEES.ID_EMPLOYEE'
PRINT '- PRODUCTS.ID_CATALOG -> CATALOGS.ID_CATALOG'
PRINT '- BILLS.ID_ORDER -> ORDERS.ID_ORDER'
PRINT ''
PRINT 'Представления:'
PRINT '1. ViewORDER - сводная информация о заказах (содержит ORDERSUM = PRICE * NUMBER)'
PRINT ''
PRINT '=== БД ГОТОВА К ИСПОЛЬЗОВАНИЮ В VISUAL STUDIO ==='
PRINT 'Для создания подтаблицы в форме "Пользователи" (как на Рисунке 23 методички):'
PRINT '1. В Visual Studio добавьте источник данных для таблицы USERS'
PRINT '2. Разверните таблицу USERS - увидите подтаблицу ORDERS'
PRINT '3. Перетащите USERS на форму для ленточной формы'
PRINT '4. Перетащите ORDERS из подтаблицы для создания DataGridView'
PRINT ''
PRINT 'Строка подключения:'
PRINT 'Data Source=localhost\SQLEXPRESS;Initial Catalog=COMPUTER_SHOP;Integrated Security=True'
GO