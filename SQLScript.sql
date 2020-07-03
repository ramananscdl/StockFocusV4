USE [SFDB]
GO
/****** Object:  Table [dbo].[Portfolio]    Script Date: 03-07-2020 22:49:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Portfolio](
	[PortfolioId] [int] IDENTITY(1,1) NOT NULL,
	[PortfolioName] [nvarchar](50) NULL,
	[IsDefault] [bit] NULL,
 CONSTRAINT [PK_Portfolio] PRIMARY KEY CLUSTERED 
(
	[PortfolioId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Segment]    Script Date: 03-07-2020 22:49:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Segment](
	[SegmentId] [int] IDENTITY(1,1) NOT NULL,
	[SegmentName] [nvarchar](50) NULL,
 CONSTRAINT [PK_Segment] PRIMARY KEY CLUSTERED 
(
	[SegmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Stocks]    Script Date: 03-07-2020 22:49:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Stocks](
	[StockId] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](50) NULL,
	[StockName] [varchar](100) NULL,
	[SegmentId] [int] NULL,
	[Exchange] [varchar](10) NULL,
 CONSTRAINT [PK_Stocks] PRIMARY KEY CLUSTERED 
(
	[StockId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Transaction]    Script Date: 03-07-2020 22:49:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Transaction](
	[TransactionId] [int] IDENTITY(1,1) NOT NULL,
	[TransactionDate] [date] NULL,
	[Quantity] [int] NULL,
	[TransactionType] [bit] NULL,
	[StockId] [int] NULL,
	[Amount] [money] NULL,
	[PortfolioId] [int] NULL,
	[Code] [varchar](50) NULL,
 CONSTRAINT [PK_Transaction] PRIMARY KEY CLUSTERED 
(
	[TransactionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Portfolio] ADD  CONSTRAINT [DF_Portfolio_IsDefault]  DEFAULT ((0)) FOR [IsDefault]
GO




 
GO
/****** Object:  UserDefinedFunction [dbo].[GetAdjustedPriceByStockId]    Script Date: 03-07-2020 22:50:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE FUNCTION [dbo].[GetAdjustedPriceByStockId]
(
 @StockId int,
 @PortfolioId int
)
RETURNS  money
AS
BEGIN


 DECLARE @Purchase money;
 DECLARE @Sale money
 DECLARE @PQty int;
 Declare @SQty int
 DECLARE @Result money


  select @Purchase =  ((sum([Transaction].Quantity * [Transaction].Amount))/sum( [Transaction].Quantity))     from [Transaction] 
  where StockId = @StockId and PortfolioId = @PortfolioId and TransactionType = 1
   
    select @Sale =  ((sum([Transaction].Quantity * [Transaction].Amount))/sum( [Transaction].Quantity))    from [Transaction] 
  where StockId = @StockId and PortfolioId = @PortfolioId and TransactionType = 0

   select @PQty =  sum( [Transaction].Quantity)     from [Transaction] 
  where StockId = @StockId and PortfolioId = @PortfolioId and TransactionType = 1

     select @SQty =  sum( [Transaction].Quantity)     from [Transaction] 
  where StockId = @StockId and PortfolioId = @PortfolioId and TransactionType = 0

  if((@PQty - @SQty ) < 1) 
	SET @Result = 0.00;
  ELSE
	 
	SET @Result =   ISNULL(((@Purchase * @PQty) -  (@Sale * @Sqty)  ) / (@PQty-@Sqty), (@Purchase * @PQty)/ @PQty)

	RETURN @Result;

END
GO
/****** Object:  UserDefinedFunction [dbo].[GetAveragePurchasePriceByStockId]    Script Date: 03-07-2020 22:50:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE FUNCTION [dbo].[GetAveragePurchasePriceByStockId]
(
 @StockId int,
 @PortfolioId int
)
RETURNS  money
AS
BEGIN


 DECLARE @result money;

  select @result =  ((sum([Transaction].Quantity * [Transaction].Amount))/sum( [Transaction].Quantity))    from [Transaction] 
  where StockId = @StockId and PortfolioId = @PortfolioId and TransactionType = 1
   
	 
	RETURN  ISNULL(@result,0)

END
GO
/****** Object:  UserDefinedFunction [dbo].[GetStockQuantity]    Script Date: 03-07-2020 22:50:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--SELECT [dbo].[GetStockQuantity] (4)
 
CREATE FUNCTION [dbo].[GetStockQuantity]  
(
	@StockId int,
	@PortfolioId int
)
RETURNS int
AS
BEGIN
	  
	  DECLARE @BuyQty int;
	  DECLARE @SellQty int;
	  Select @BuyQty = ISNULL(SUM( [Quantity]),0) from  [Transaction] where TransactionType =1 and  StockId=@StockId and PortfolioId=@PortfolioId;
	   Select @SellQty = ISNULL(SUM( [Quantity]),0)  from  [Transaction] where TransactionType =0 and  StockId=@StockId and PortfolioId=@PortfolioId;
RETURN @BuyQty - @SellQty;

END
GO


---------------
 
GO
/****** Object:  StoredProcedure [dbo].[DeletePortfolio]    Script Date: 03-07-2020 22:51:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE  PROCEDURE [dbo].[DeletePortfolio]   
( 
@PortfolioId int 
 )
 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	 DECLARE @IsDefault bit;
	 DECLARE @LastPortfolioId int;

	 SELECT @IsDefault=  IsDefault  FROM Portfolio WHERE        (PortfolioId = @PortfolioId);

	 DELETE FROM   Portfolio 	WHERE        (PortfolioId = @PortfolioId);

    SELECT TOP(1) @LastPortfolioId = PortfolioId from Portfolio  ORDER BY PortfolioId DESC

	PRINT @LastPortfolioId;


	UPDATE Portfolio SET IsDefault =1 Where (PortfolioId = @LastPortfolioId);

Select  @PortfolioId  as  PortfolioId;
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteTransaction]    Script Date: 03-07-2020 22:51:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE  PROCEDURE [dbo].[DeleteTransaction]   
( 
@TransactionId int 
 )
 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	 
	 Delete From [Transaction] where TransactionId = @TransactionId;

Select  @TransactionId  as  TransactionId;
END
GO
/****** Object:  StoredProcedure [dbo].[EditPortfolio]    Script Date: 03-07-2020 22:51:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE  PROCEDURE [dbo].[EditPortfolio]  -- 'Seconds',2,1  
(
@PortfolioName nvarchar(50),
@PortfolioId int,
@IsDefault bit 

)
 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF(@IsDefault = 1) 
	BEGIN
		UPDATE       Portfolio
		SET  IsDefault =  0;
	END

	 UPDATE       Portfolio
	SET                PortfolioName = @PortfolioName, IsDefault = @IsDefault
	WHERE        (PortfolioId = @PortfolioId)

Select  @PortfolioId  as  PortfolioId;
END
GO
/****** Object:  StoredProcedure [dbo].[EditStock]    Script Date: 03-07-2020 22:51:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE  PROCEDURE [dbo].[EditStock]  
(
@StockName nvarchar(1000),
@StockId int,
@SegmentId int,
@Code varchar(50),
@Exchange varchar(10)

)
 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
 UPDATE       Stocks
SET                Code = @Code, StockName = @StockName, SegmentId = @SegmentId, Exchange = @Exchange
WHERE        (StockId = @StockId)
	 
Select  @StockId  as  StockId;
END
GO
/****** Object:  StoredProcedure [dbo].[EditTransaction]    Script Date: 03-07-2020 22:51:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE  PROCEDURE [dbo].[EditTransaction]    
(
@Transactionid int,
@TransactionDate date,
@Quantity int,	 
@TransactionType bit,
@StockId int,
@Amount money,
@PortfolioId int 

)
 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @Code varchar(50);
	SELECT @Code = Code From Stocks where StockId = @StockId;

UPDATE       [Transaction]
SET                TransactionDate = @TransactionDate, Quantity = @Quantity, TransactionType = @TransactionType, StockId = @StockId, Amount = @Amount, PortfolioId = @PortfolioId, Code = @Code
WHERE        (TransactionId = @Transactionid)

Select  @Transactionid as TransactionId;
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllSegments]    Script Date: 03-07-2020 22:51:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE PROCEDURE [dbo].[GetAllSegments]
 
AS
BEGIN
 
	 SELECT  SegmentId, SegmentName from Segment;


END
GO
/****** Object:  StoredProcedure [dbo].[GetLastUpdatedDate]    Script Date: 03-07-2020 22:51:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE PROCEDURE [dbo].[GetLastUpdatedDate]
 
AS
BEGIN
 
 select top(1) TransactionDate from [Transaction] order by TransactionDate desc
 
END
GO
/****** Object:  StoredProcedure [dbo].[getNetRealizedProfitByProfileId]    Script Date: 03-07-2020 22:51:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE PROCEDURE [dbo].[getNetRealizedProfitByProfileId] 
	-- Add the parameters for the stored procedure here
	 (@PortfolioId int)
AS
BEGIN
	 
	SET NOCOUNT ON;
 
	SELECT    SUM(Case when TransactionType = 0 THEN Amount*Quantity  WHEN TransactionType = 1 THEN Amount*Quantity * (-1) END)  as NRP  
	FROM            [Transaction] where  [dbo].GetStockQuantity(StockId, @PortfolioId) =0  AND PortfolioId = @PortfolioId

END
GO
/****** Object:  StoredProcedure [dbo].[GetPortfolios]    Script Date: 03-07-2020 22:51:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE PROCEDURE [dbo].[GetPortfolios]
 
AS
BEGIN
 
	 SELECT        PortfolioId, PortfolioName, [IsDefault] 
FROM            Portfolio


END
GO
/****** Object:  StoredProcedure [dbo].[GetStockList]    Script Date: 03-07-2020 22:51:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE PROCEDURE [dbo].[GetStockList]
 
AS
BEGIN
 
SELECT        StockId, StockName, Code, SegmentId, Exchange
FROM            Stocks
ORDER By StockName
END
GO
/****** Object:  StoredProcedure [dbo].[GetStocksByPortfolioId]    Script Date: 03-07-2020 22:51:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE  PROCEDURE [dbo].[GetStocksByPortfolioId]   
 ( @PortfolioId int)
 
AS
BEGIN
 
	SET NOCOUNT ON;
 
SELECT        Stocks.StockId, Stocks.Code, Stocks.StockName, Stocks.SegmentId, Stocks.Exchange, 
[dbo].GetStockQuantity(Stocks.StockId, @PortfolioId) AS TotalQuantity,
[dbo].[GetAveragePurchasePriceByStockId](Stocks.StockId, @PortfolioId) PurchasePrice, 
[dbo].[GetAdjustedPriceByStockId](Stocks.StockId, @PortfolioId) AdjustedPrice
FROM            Stocks INNER JOIN
                         [Transaction] ON Stocks.StockId = [Transaction].StockId  
 WHERE PortfolioId  = @PortfolioId
 GROUP BY Stocks.StockId, Stocks.Code, Stocks.StockName, Stocks.SegmentId, Stocks.Exchange
						
 
END
GO
/****** Object:  StoredProcedure [dbo].[GetTransactionByStock]    Script Date: 03-07-2020 22:51:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE PROCEDURE [dbo].[GetTransactionByStock] (  @stockid int , @PortfolioId int)
	 
AS
BEGIN
 
	SET NOCOUNT ON;
	SELECT        StockId, TransactionDate, Quantity, TransactionType, Amount, PortfolioId, Code, TransactionId
FROM            [Transaction]
WHERE        (StockId = @StockId) AND (PortfolioId = @PortfolioId)


END
GO
/****** Object:  StoredProcedure [dbo].[InsertPortfolio]    Script Date: 03-07-2020 22:51:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE PROCEDURE [dbo].[InsertPortfolio]  
 (
 
 @PortfolioName nvarchar(50) ,
 @IsDefault bit
 )
AS
BEGIN
 
	SET NOCOUNT ON;

	IF(@IsDefault = 1) 
	BEGIN
		UPDATE       Portfolio
		SET  IsDefault =  0;
	END

 INSERT INTO Portfolio
                         (PortfolioName, IsDefault)
VALUES        (@PortfolioName,@IsDefault)

Select @@IDENTITY as PortfolioId


END
GO
/****** Object:  StoredProcedure [dbo].[InsertSegment]    Script Date: 03-07-2020 22:51:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE PROCEDURE [dbo].[InsertSegment]
 (
 
 @SegmentName nvarchar(50) 
 )
AS
BEGIN
 
	SET NOCOUNT ON;

 INSERT INTO Segment
                         (SegmentName)
VALUES        (@SegmentName)

Select @@IDENTITY as SegmentId


END
GO
/****** Object:  StoredProcedure [dbo].[InsertStock]    Script Date: 03-07-2020 22:51:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE PROCEDURE [dbo].[InsertStock]
 (
 @Code varchar(50),
 @StockName varchar(100),
 @SegmentId int ,
 @Exchange varchar(10)
 )
AS
BEGIN
 
	SET NOCOUNT ON;

INSERT INTO Stocks
                         (Code, StockName, SegmentId, Exchange)
VALUES        (@Code,@StockName,@SegmentId,@Exchange)

Select @@IDENTITY as StockId


END
GO
/****** Object:  StoredProcedure [dbo].[InsertTransaction]    Script Date: 03-07-2020 22:51:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE  PROCEDURE [dbo].[InsertTransaction]   --
(
@TransactionDate date,
@Quantity int,	 
@TransactionType bit,
@StockId int,
@Amount money,
@PortfolioId int 

)
 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @Code varchar(50);
	SELECT @Code = Code From Stocks where StockId = @StockId;

 INSERT INTO [Transaction]
                         (TransactionDate, Quantity, TransactionType, StockId, Amount, PortfolioId, Code)
VALUES        (@TransactionDate,@Quantity,@TransactionType,@StockId,@Amount,@PortfolioId,@Code)

Select  @@IDENTITY as TransactionId;
END
GO



Insert into [Portfolio]   ([PortfolioName]  ,[IsDefault]) Values ('DefaultPF',1)