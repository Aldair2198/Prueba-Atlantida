USE [TcDataContext]
GO

/****** Object:  StoredProcedure [dbo].[Transacciones_Ins]    Script Date: 1/31/2024 6:26:05 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Transacciones_Ins]
    @NumeroTarjeta VARCHAR(50),
    @Fecha DATE,
	@Descripcion VARCHAR(MAX),
	@AbonoCargo CHAR,
	@Monto NUMERIC,
	@Estado CHAR
AS
BEGIN
    BEGIN TRY
        DECLARE @sql NVARCHAR(MAX);

		IF @NumeroTarjeta IS NOT NULL AND @Fecha IS NOT NULL AND @Descripcion IS NOT NULL AND @AbonoCargo IS NOT NULL AND @Monto IS NOT NULL AND	@Estado IS NOT NULL
        BEGIN
			SET @sql = N'INSERT INTO Transacciones (NumeroTarjeta, Fecha, Descripcion, AbonoCargo, Monto, Estado)
						VALUES (@NumeroTarjeta, @Fecha, @Descripcion, @AbonoCargo, @Monto, @Estado)';
        END
        ELSE 
        BEGIN
            THROW 50000, 'Verifique los valores ingresados', 1;
        END

        EXEC sp_executesql @sql, N'@NumeroTarjeta VARCHAR(50), @Fecha DATE, @Descripcion VARCHAR(MAX), @AbonoCargo CHAR, @Monto NUMERIC, @Estado CHAR',
		@NumeroTarjeta = @NumeroTarjeta, @Fecha = @Fecha, @Descripcion = @Descripcion, @AbonoCargo = @AbonoCargo, @Monto = @Monto, @Estado = @Estado;

    END TRY
    BEGIN CATCH
        SELECT 
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END;
GO


