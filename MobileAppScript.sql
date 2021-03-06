USE [Footprints-MobileApp]
GO
/****** Object:  StoredProcedure [dbo].[AddUser]    Script Date: 13/01/2015 8:31:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[AddUser]
@UMail NVARCHAR(100),
@CompanyId INT,
@BillingId NVARCHAR(254),
@TokenValue NVARCHAR(100),
@TokenType INT
AS
INSERT INTO Users(UMail,CompanyId,BillingId) VALUES(@UMail,@CompanyId,@BillingId)

DECLARE @RtnId AS INT
SET @RtnId = @@IDENTITY

INSERT INTO Tokens(UserId,Value,StartTime,TokenType) VALUES (@RtnId,@TokenValue,GETDATE(),@TokenType)

GO
/****** Object:  Table [dbo].[Companies]    Script Date: 13/01/2015 8:31:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Companies](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](254) NULL,
 CONSTRAINT [PK_Companies] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Incidents]    Script Date: 13/01/2015 8:31:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Incidents](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FPIncidentId] [int] NULL,
	[UserId] [int] NULL,
	[SubmitterEmail] [nchar](100) NULL,
	[Status] [int] NULL,
	[Type] [int] NULL,
 CONSTRAINT [PK_Incidents] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tokens]    Script Date: 13/01/2015 8:31:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tokens](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[Value] [nvarchar](100) NULL,
	[StartTime] [datetime] NULL,
	[TokenType] [int] NULL,
 CONSTRAINT [PK_Tokens] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserIncidentMapping]    Script Date: 13/01/2015 8:31:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserIncidentMapping](
	[UserId] [int] NOT NULL,
	[IncidentId] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Users]    Script Date: 13/01/2015 8:31:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UMail] [nvarchar](100) NULL,
	[Password] [nvarchar](200) NULL,
	[CompanyId] [int] NULL,
	[BillingId] [nvarchar](254) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
