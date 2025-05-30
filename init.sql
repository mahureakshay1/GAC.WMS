USE [GacWms]
GO
SET IDENTITY_INSERT [dbo].[Customers] ON 


IF NOT EXISTS (SELECT * FROM [dbo].[Customers] WHERE [dbo].[Customers].[CustomerId] = 1)
BEGIN
	INSERT [dbo].[Customers] ([CustomerId], [CompanyName], [ContactPersonName], [Address], [Contact], [CreatedAt], [UpdatedAt]) VALUES (1, N'Company 1', N'Name 1', N'Address 1', N'12345678', CAST(0x0000B2C60168FC7B AS DateTime), CAST(0x0000B2C60168FC7B AS DateTime))
END;

IF NOT EXISTS (SELECT * FROM [dbo].[Customers] WHERE [dbo].[Customers].[CustomerId] = 2)
BEGIN
	INSERT [dbo].[Customers] ([CustomerId], [CompanyName], [ContactPersonName], [Address], [Contact], [CreatedAt], [UpdatedAt]) VALUES (2, N'Company 2', N'Name 2', N'Address 2', N'12345678', CAST(0x0000B2C60168FC7B AS DateTime), CAST(0x0000B2C60168FC7B AS DateTime))
END;

IF NOT EXISTS (SELECT * FROM [dbo].[Customers] WHERE [dbo].[Customers].[CustomerId] = 3)
BEGIN
	INSERT [dbo].[Customers] ([CustomerId], [CompanyName], [ContactPersonName], [Address], [Contact], [CreatedAt], [UpdatedAt]) VALUES (3, N'Company 3', N'Name 3', N'Address 3', N'12345678', CAST(0x0000B2C60168FC7B AS DateTime), CAST(0x0000B2C60168FC7B AS DateTime))
END;

IF NOT EXISTS (SELECT * FROM [dbo].[Customers] WHERE [dbo].[Customers].[CustomerId] = 4)
BEGIN
	INSERT [dbo].[Customers] ([CustomerId], [CompanyName], [ContactPersonName], [Address], [Contact], [CreatedAt], [UpdatedAt]) VALUES (4, N'Company 4', N'Name 4', N'Address 4', N'12345678', CAST(0x0000B2C60168FC7B AS DateTime), CAST(0x0000B2C60168FC7B AS DateTime))
END;

SET IDENTITY_INSERT [dbo].[Customers] OFF

SET IDENTITY_INSERT [dbo].[Products] ON 

IF NOT EXISTS (SELECT * FROM [dbo].[Products] WHERE [dbo].[Products].[ProductId] = 1)
BEGIN
	INSERT [dbo].[Products] ([ProductId], [Code], [Title], [Description], [Length], [Width], [Height], [Weight], [UnitOfDimension], [QuantityAvailable], [UnitOfQuantity], [CreatedAt], [UpdatedAt]) VALUES (1, N'P1', N'Product1', N'Product1 description', CAST(1.00 AS Decimal(18, 2)), CAST(1.00 AS Decimal(18, 2)), CAST(1.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), 1, 50000, 1, CAST(0x0000B2C60167BD0F AS DateTime), CAST(0x0000B2C60167BD0F AS DateTime))
END;

IF NOT EXISTS (SELECT * FROM [dbo].[Products] WHERE [dbo].[Products].[ProductId] = 2)
BEGIN
	INSERT [dbo].[Products] ([ProductId], [Code], [Title], [Description], [Length], [Width], [Height], [Weight], [UnitOfDimension], [QuantityAvailable], [UnitOfQuantity], [CreatedAt], [UpdatedAt]) VALUES (2, N'P2', N'Product2', N'Product2 description', CAST(2.00 AS Decimal(18, 2)), CAST(2.00 AS Decimal(18, 2)), CAST(2.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), 1, 10000, 1, CAST(0x0000B2C60167BD0F AS DateTime), CAST(0x0000B2C60167BD0F AS DateTime))
END;

IF NOT EXISTS (SELECT * FROM [dbo].[Products] WHERE [dbo].[Products].[ProductId] = 3)
BEGIN
	INSERT [dbo].[Products] ([ProductId], [Code], [Title], [Description], [Length], [Width], [Height], [Weight], [UnitOfDimension], [QuantityAvailable], [UnitOfQuantity], [CreatedAt], [UpdatedAt]) VALUES (3, N'P3', N'Product3', N'Product3 description', CAST(3.00 AS Decimal(18, 2)), CAST(3.00 AS Decimal(18, 2)), CAST(3.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), 1, 20000, 1, CAST(0x0000B2C60167BD0F AS DateTime), CAST(0x0000B2C60167BD0F AS DateTime))
END;
SET IDENTITY_INSERT [dbo].[Products] OFF
