SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT name FROM sys.objects WHERE type='P' AND name='sp_GenerateNumericIdentity')
    DROP PROCEDURE [sp_GenerateNumericIdentity]
GO
CREATE PROCEDURE [dbo].[sp_GenerateNumericIdentity]
(
	@maxIdentity DECIMAL(12, 0) OUTPUT,
	@tableName NVARCHAR(50),
	@columnName NVARCHAR(50),
	@n INT = 1 -- Support getting many identities at once
)
WITH EXECUTE AS 'dbo'
AS
BEGIN
	SET NOCOUNT ON

	IF @n < 1 OR @n > 32767
	BEGIN
		RAISERROR('Number of identites to generate must be between 1 and 32767, %i given.', 15, 1, @n);
		RETURN
	END

	BEGIN TRANSACTION
		DECLARE @identity DECIMAL(12, 0)

		SELECT @identity = MAX(ColumnIdentity) 
			FROM AB_NUMERIC_IDENTITIES 
			WHERE TableName = @tableName 
				AND ColumnName = @columnName

		IF (@identity IS NULL)
		BEGIN
			DECLARE @sql NVARCHAR(400);
			SELECT @sql = N'SELECT @identity = MAX(' + @columnName + N') FROM ' + @tableName

			EXEC sp_executesql @sql, N'@identity DECIMAL(12, 0) OUTPUT', @identity OUTPUT
			
			SET @maxIdentity = @identity + 1000000000 + @n
			INSERT INTO AB_NUMERIC_IDENTITIES (TableName, ColumnName, ColumnIdentity) VALUES(@tableName, @columnName, @maxIdentity)
		END
		ELSE
		BEGIN
			SET @maxIdentity = @identity + @n
			UPDATE AB_NUMERIC_IDENTITIES SET ColumnIdentity = @maxIdentity WHERE TableName = @tableName AND ColumnName = @columnName
		END

		IF (@n > 1) -- Return set of @n new identities.
		BEGIN
			WITH RECURSION(ID, COUNTER ) AS
			(
				SELECT 0 AS A, CAST((@identity + 1) AS DECIMAL(12, 0)) AS COUNTER
				UNION ALL
				SELECT T.A, CAST((COUNTER + 1) AS DECIMAL(12, 0)) AS COUNTER FROM (SELECT 0 AS A) T
				INNER JOIN RECURSION ON T.A = RECURSION.ID
			) 
			SELECT TOP (@n) RECURSION.COUNTER FROM RECURSION
				OPTION (MAXRECURSION 32767);
		END

	COMMIT TRANSACTION
END 	
GO
GRANT EXEC ON [sp_GenerateNumericIdentity] TO abu;
