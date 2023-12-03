SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[TiposCuentas_Insertar]
	@Nombre nvarchar(50),
	@UsuarioId int
AS
BEGIN
	SET NOCOUNT ON;

    DECLARE @Orden int;
	SELECT @Orden = COALESCE(MAX(Orden), 0) + 1 FROM TiposCuentas WHERE UsuarioId = @UsuarioId

	INSERT INTO TiposCuentas(Nombre, UsuarioId, Orden) VALUES (@Nombre, @UsuarioId, @Orden);

	SELECT SCOPE_IDENTITY();
END
GO
-----------------------------------------------------------------------------
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Transacciones_Actualizar]
	@Id int,
	@FechaTransaccion datetime,
	@Monto decimal(18,2),
	@MontoAnterior decimal(18,2),
	@CuentaId int,
	@CuentaIdAnterior int,
	@CategoriaId int,
	@Nota nvarchar(1000) = NULL

AS
BEGIN
	SET NOCOUNT ON;

    -- Revertir trn anterior
	UPDATE Cuentas SET Balance -= @MontoAnterior WHERE Id = @CuentaIdAnterior;

	-- Realizar la nueva transaccion
	UPDATE Cuentas SET Balance += @Monto WHERE Id = @CuentaId;

	UPDATE Transacciones SET Monto = ABS(@Monto), FechaTransaccion = @FechaTransaccion,
	CategoriaId = @CategoriaId, CuentaId = @CuentaId, Nota = @Nota
	WHERE Id = @Id;
END
GO
-----------------------------------------------------------------------------
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Transacciones_Borrar]
	@Id int
AS
BEGIN
	SET NOCOUNT ON;

    DECLARE @Monto decimal(18,2);
	DECLARE @CuentaId int;
	DECLARE @TipoOperacionId int;

	--carga las variables
	SELECT @Monto = Monto, @CuentaId = CuentaId, @TipoOperacionId = cat.TipoOperacionId
	FROM Transacciones
	INNER JOIN Categorias cat ON cat.Id = Transacciones.CategoriaId
	WHERE Transacciones.Id = @Id;

	DECLARE @FactorMultiplicativo int = 1;
	
	IF(@TipoOperacionId = 2)
		SET @FactorMultiplicativo = -1;

	SET @Monto = @Monto * @FactorMultiplicativo;

	UPDATE Cuentas SET Balance -= @Monto WHERE Id = @CuentaId;

	DELETE Transacciones WHERE Id = @Id;
END
GO
-----------------------------------------------------------------------------
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Transacciones_Insertar] 
	@UsuarioId int,
	@FechaTransaccion date,
	@Monto decimal(18,2),
	@CategoriaId int,
	@CuentaId int,
	@Nota nvarchar(1000) = NULL
AS
BEGIN
	SET NOCOUNT ON;

    INSERT INTO Transacciones(UsuarioId, FechaTransaccion, Monto, CategoriaId, CuentaId, Nota)
	VALUES (@UsuarioId, @FechaTransaccion, ABS(@Monto), @CategoriaId, @CuentaId, @Nota);

	UPDATE Cuentas SET Balance += @Monto WHERE Id = @CuentaId;

	SELECT SCOPE_IDENTITY();
END
GO
-----------------------------------------------------------------------------
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE CrearDatosUsuarioNuevo 
	@UsuarioId int
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Efectivo nvarchar(50) = 'Efectivo';
	DECLARE @CuentasDeBanco nvarchar(50) = 'Cuentas de banco';
	DECLARE @Tarjetas nvarchar(50) = 'Tarjetas';

	INSERT INTO TiposCuentas(Nombre,UsuarioId,Orden)
	VALUES (@Efectivo, @UsuarioId, 1), 
	       (@CuentasDeBanco, @UsuarioId, 2),
		   (@Tarjetas, @UsuarioId, 3);

	INSERT INTO Cuentas (Nombre, Balance, TipoCuentaId)
	SELECT Nombre, 0, Id
	FROM TiposCuentas
	WHERE UsuarioId = @UsuarioId;

	INSERT INTO Categorias (Nombre, TipoOperacionId, UsuarioId)
	VALUES	('Libros', 2, @UsuarioId),
			('Salario', 1, @UsuarioId),
			('Mesada', 1, @UsuarioId),
		    ('Comida', 2, @UsuarioId);
END
GO