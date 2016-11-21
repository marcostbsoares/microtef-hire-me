CREATE TABLE [dbo].[Table]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [CardholderName] NVARCHAR(100) NULL, 
    [CardNumber] NVARCHAR(20) NULL, 
    [ExpirationDate] DATE NULL, 
    [CardType] NVARCHAR(20) NULL, 
    [CardBrand] NVARCHAR(20) NULL, 
    [HasPassword] BIT NULL, 
    [HashedPassword] NVARCHAR(128) NULL, 
    [CreditLimit] MONEY NULL, 
    [UsedCredit] MONEY NULL, 
    [Balance] MONEY NULL, 
    CONSTRAINT [CK_Table_Column] CHECK ((HasPassword = 1 AND CardType = 'Magnetic') OR (HasPassword = 0))
)
