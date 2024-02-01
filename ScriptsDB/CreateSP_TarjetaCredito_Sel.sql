USE [TcDataContext]
GO

/****** Object:  StoredProcedure [dbo].[TarjetaCredito_Sel]    Script Date: 1/31/2024 6:24:42 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[TarjetaCredito_Sel]
    @TipoFiltro NVARCHAR(50),
    @Filtro NVARCHAR(50) = NULL
AS
BEGIN
    BEGIN TRY
        DECLARE @sql NVARCHAR(MAX);

        IF @TipoFiltro = 'TODOS' 
        BEGIN
            SET @sql = N'SELECT * FROM dbo.TarjetaCredito';
        END
        ELSE IF @TipoFiltro = 'Id' AND @Filtro IS NOT NULL 
        BEGIN
            SET @sql = N'SELECT * FROM dbo.TarjetaCredito WHERE Id = @Filtro';
        END
        ELSE IF @TipoFiltro = 'NumeroTarjeta' AND @Filtro IS NOT NULL 
        BEGIN
            SET @sql = N'SELECT * FROM dbo.TarjetaCredito WHERE NumeroTarjeta = @Filtro';
        END
        ELSE 
        BEGIN
            THROW 50000, 'TipoFiltro y Filtro deben ser proporcionados, a menos que TipoFiltro sea "TODOS"', 1;
        END

        EXEC sp_executesql @sql, N'@Filtro NVARCHAR(50)', @Filtro = @Filtro;
    END TRY
    BEGIN CATCH
        SELECT 
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END;
GO


