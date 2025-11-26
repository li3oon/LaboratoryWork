CREATE DATABASE MMM;
GO

USE MMM;
GO

-- 2. Таблица магазин
IF OBJECT_ID('dbo.Магазин', 'U') IS NOT NULL DROP TABLE dbo.Магазин;
CREATE TABLE dbo.Магазин
(
    код_магазина INT IDENTITY(1,1) PRIMARY KEY,
    название NVARCHAR(200) NOT NULL,
    адрес NVARCHAR(500) NULL,
    телефон NVARCHAR(50) NULL,
    email NVARCHAR(200) NULL,
    дата_добавления DATETIME DEFAULT GETDATE()
);
GO

-- 3. Таблица модель (каталог моделей/позиций)
IF OBJECT_ID('dbo.Модель', 'U') IS NOT NULL DROP TABLE dbo.Модель;
CREATE TABLE dbo.Модель
(
    код_модели INT IDENTITY(1,1) PRIMARY KEY,
    артикул NVARCHAR(100) NULL,
    название NVARCHAR(250) NOT NULL,
    описание NVARCHAR(MAX) NULL,
    цена DECIMAL(18,2) NOT NULL DEFAULT 0,
    миним_количество INT NOT NULL DEFAULT 0,
    код_магазина INT NULL, -- если модель привязана к магазину
    CONSTRAINT FK_Модель_Магазин FOREIGN KEY (код_магазина) REFERENCES dbo.Магазин(код_магазина)
);
GO

-- 4. Таблица готовая_продукция (конкретные изготовленные изделия)
IF OBJECT_ID('dbo.Готовая_продукция', 'U') IS NOT NULL DROP TABLE dbo.Готовая_продукция;
CREATE TABLE dbo.Готовая_продукция
(
    код_продукции INT IDENTITY(1,1) PRIMARY KEY,
    код_модели INT NOT NULL,
    серийный_номер NVARCHAR(200) NULL,
    дата_изготовления DATE NULL,
    состояние NVARCHAR(100) NULL,
    цена DECIMAL(18,2) NULL,
    CONSTRAINT FK_Готовая_продукция_Модель FOREIGN KEY (код_модели) REFERENCES dbo.Модель(код_модели)
);
GO

-- 5. Таблица заказ
IF OBJECT_ID('dbo.Заказ', 'U') IS NOT NULL DROP TABLE dbo.Заказ;
CREATE TABLE dbo.Заказ
(
    код_заказа INT IDENTITY(1,1) PRIMARY KEY,
    дата_заказа DATETIME NOT NULL DEFAULT GETDATE(),
    код_магазина INT NULL,
    клиент NVARCHAR(250) NULL,
    сумма DECIMAL(18,2) NULL,
    статус NVARCHAR(50) DEFAULT 'Новый',
    CONSTRAINT FK_Заказ_Магазин FOREIGN KEY (код_магазина) REFERENCES dbo.Магазин(код_магазина)
);
GO

-- 6. Таблица состав_заказа (строки заказа)
IF OBJECT_ID('dbo.Состав_заказа', 'U') IS NOT NULL DROP TABLE dbo.Состав_заказа;
CREATE TABLE dbo.Состав_заказа
(
    код_заказа INT NOT NULL,
    код_продукции INT NOT NULL,
    количество INT NOT NULL DEFAULT 1,
    цена_за_ед DECIMAL(18,2) NULL,
    PRIMARY KEY (код_заказа, код_продукции),
    CONSTRAINT FK_СоставЗаказа_Заказ FOREIGN KEY (код_заказа) REFERENCES dbo.Заказ(код_заказа) ON DELETE CASCADE,
    CONSTRAINT FK_СоставЗаказа_Продукция FOREIGN KEY (код_продукции) REFERENCES dbo.Готовая_продукция(код_продукции)
);
GO

-- Индексы (по типовым полям)
CREATE INDEX IX_Модель_артикул ON dbo.Модель(артикул);
CREATE INDEX IX_Готовая_продукция_код_модели ON dbo.Готовая_продукция(код_модели);
CREATE INDEX IX_Заказ_дата ON dbo.Заказ(дата_заказа);
GO

-- Пример: вставим пару записей в Модель и Магазин
INSERT INTO dbo.Магазин (название, адрес, телефон, email) VALUES
('Магазин №1','г. Москва, ул. Примерная, 1','+7(495)111-22-33','shop1@example.com');

INSERT INTO dbo.Модель (артикул, название, описание, цена, миним_количество, код_магазина)
VALUES
('ART-001','Модель A','Описание модели A', 1250.00, 5, 1),
('ART-002','Модель B','Описание модели B', 2300.50, 2, 1);
GO
