USE [TcDataContext]
GO

/****** Object:  Table [dbo].[TarjetaCredito]    Script Date: 1/31/2024 6:23:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TarjetaCredito](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NumeroTarjeta] [varchar](16) NULL,
	[Nombres] [nvarchar](max) NULL,
	[Apellidos] [nvarchar](max) NULL,
	[SaldoActual] [float] NOT NULL,
	[Limite] [float] NOT NULL,
	[PorcInteres] [float] NOT NULL,
	[Estado] [varchar](5) NULL,
	[PorcSaldoMinimo] [numeric](9, 4) NULL,
 CONSTRAINT [PK_TarjetaCredito] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[TarjetaCredito] ADD  CONSTRAINT [TC_Estado]  DEFAULT ('A') FOR [Estado]
GO


