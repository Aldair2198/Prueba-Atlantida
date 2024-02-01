USE [TcDataContext]
GO

/****** Object:  Table [dbo].[Transacciones]    Script Date: 1/31/2024 6:24:19 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Transacciones](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NumeroTarjeta] [varchar](16) NULL,
	[Fecha] [datetime] NULL,
	[Descripcion] [varchar](max) NULL,
	[AbonoCargo] [char](5) NULL,
	[Monto] [numeric](10, 2) NULL,
	[Estado] [char](5) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Transacciones] ADD  DEFAULT ('A') FOR [Estado]
GO

ALTER TABLE [dbo].[Transacciones]  WITH CHECK ADD CHECK  (([estado]='E' OR [estado]='A'))
GO


