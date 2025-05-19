-- Создание базы данных
CREATE DATABASE CompanyAuthDB;
GO

USE CompanyAuthDB;
GO

-- Создание таблицы пользователей
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('admin', 'user')),
    IsBlocked BIT NOT NULL DEFAULT 0,
    FisrtIn BIT NOT NULL DEFAULT 0,
    LastLoginDate DATETIME NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Добавление тестового администратора (пароль: admin123)
INSERT INTO Users (Username, Password, Role)
VALUES ('admin', 'admin123', 'admin');
GO

-- Добавление тестового пользователя (пароль: user123)
INSERT INTO Users (Username, Password, Role)
VALUES ('user', 'user123', 'user');
GO