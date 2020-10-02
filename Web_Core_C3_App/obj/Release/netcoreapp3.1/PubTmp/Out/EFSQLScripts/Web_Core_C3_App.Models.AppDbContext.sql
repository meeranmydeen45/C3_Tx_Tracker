IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200517203623_Second')
BEGIN
    CREATE TABLE [companies] (
        [ID] int NOT NULL IDENTITY,
        [CompanyName] nvarchar(max) NULL,
        [CompanyCode] int NOT NULL,
        [MobileNumber] float NOT NULL,
        CONSTRAINT [PK_companies] PRIMARY KEY ([ID])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200517203623_Second')
BEGIN
    CREATE TABLE [employees] (
        [Id] int NOT NULL IDENTITY,
        [EmployeeName] nvarchar(max) NULL,
        [CardRequested] bit NULL,
        [DateRequested] datetime2 NULL,
        [CardRcvd] bit NULL,
        [DateRcvd] datetime2 NULL,
        [CardDelivered] bit NULL,
        [DateDelevired] datetime2 NULL,
        [CompanyId] int NOT NULL,
        CONSTRAINT [PK_employees] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_employees_companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [companies] ([ID]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200517203623_Second')
BEGIN
    CREATE INDEX [IX_employees_CompanyId] ON [employees] ([CompanyId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200517203623_Second')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200517203623_Second', N'3.1.3');
END;

GO

