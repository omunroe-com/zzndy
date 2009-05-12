----------------------------------------------------------------
--This is a template for the database schema changes scripting--
--DO NOT MODIFY anything unless the opposite mentioned		  --
----------------------------------------------------------------

BEGIN TRANSACTION

BEGIN TRY
	DECLARE @updateAcceptedFlag int

	EXEC @updateAcceptedFlag = [dbo].[BEGIN_UPDATE]

	-- The 'XXX' must be replaced with the number (order) of the script
	--	It is the first three digits in the script name that contains this SQL
			@version = 105

	IF (@updateAcceptedFlag = 0)
	BEGIN
		-- Place your SQL below, preferably - inside the quotes---------
		-- REMEMBER: try to avoid DROPs/CREATEs, use ALTER instead;
		--			 always check preconditions on the schema object you're going to modify
		--			 remove all GO statements, they won't work and will cause errors
		EXECUTE dbo.sp_executesql @statement = N'
ALTER PROCEDURE [PROC_CLONE_FISCAL_TERMS] @TAX_SYSTEM_ID DECIMAL (12,0), @NEW_TAX_SYSTEM_ID DECIMAL (12,0) OUTPUT
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @ERROR_PROCEDURE NVARCHAR(255);

	DECLARE @ERROR_MESSAGE NVARCHAR(255);

	BEGIN TRY
		BEGIN TRANSACTION

		--
		-- Create temporary tables (Just copy tables structure)
		--

		SELECT * INTO #TAX_SYSTEM_FISCAL_TERMS
			FROM TAX_SYSTEM WHERE 42=10;

		SELECT * INTO #TAX_SYSTEM_SHEETS_FISCAL_TERMS
			FROM TAX_SYSTEM_SHEETS WHERE 42=10;

		--
		-- ''Backup'' specified object to temporaties
		--

		INSERT INTO #TAX_SYSTEM_FISCAL_TERMS
			SELECT * FROM TAX_SYSTEM
			WHERE TAX_SYSTEM_ID = @TAX_SYSTEM_ID;

		INSERT INTO #TAX_SYSTEM_SHEETS_FISCAL_TERMS
			SELECT * FROM TAX_SYSTEM_SHEETS
			WHERE TAX_SYSTEM_ID = @TAX_SYSTEM_ID;

		DECLARE @NAME VARCHAR(50)
		SELECT @NAME = ''Copy of '' + TAX_SYSTEM_NAME FROM #TAX_SYSTEM_FISCAL_TERMS WHERE TAX_SYSTEM_ID = @TAX_SYSTEM_ID;

		DECLARE @N INT;
		SET @N = 2;

		WHILE EXISTS (SELECT TAX_SYSTEM_NAME FROM TAX_SYSTEM WHERE TAX_SYSTEM_NAME = @NAME)
		BEGIN
			SELECT @NAME = ''Copy ('' + CONVERT(VARCHAR(2), @N) + '') of '' + TAX_SYSTEM_NAME
				FROM #TAX_SYSTEM_FISCAL_TERMS WHERE TAX_SYSTEM_ID = @TAX_SYSTEM_ID;

			SET @N = @N + 1;
		END

		UPDATE #TAX_SYSTEM_FISCAL_TERMS SET TAX_SYSTEM_NAME = @NAME WHERE TAX_SYSTEM_ID = @TAX_SYSTEM_ID;

		--
		-- Get new IDs
		--

		EXEC sp_GenerateNumericIdentity @NEW_TAX_SYSTEM_ID OUTPUT, ''TAX_SYSTEM'', ''TAX_SYSTEM_ID'';

		--
		-- Update IDs
		--

		UPDATE #TAX_SYSTEM_FISCAL_TERMS SET TAX_SYSTEM_ID = @NEW_TAX_SYSTEM_ID WHERE TAX_SYSTEM_ID = @TAX_SYSTEM_ID;

		UPDATE #TAX_SYSTEM_SHEETS_FISCAL_TERMS SET TAX_SYSTEM_ID = @NEW_TAX_SYSTEM_ID WHERE TAX_SYSTEM_ID = @TAX_SYSTEM_ID;

		--
		-- ''Restore'' copy
		--

		INSERT INTO TAX_SYSTEM
			SELECT * FROM #TAX_SYSTEM_FISCAL_TERMS
			WHERE TAX_SYSTEM_ID = @NEW_TAX_SYSTEM_ID;

		INSERT INTO TAX_SYSTEM_SHEETS
			SELECT * FROM #TAX_SYSTEM_SHEETS_FISCAL_TERMS
			WHERE TAX_SYSTEM_ID = @NEW_TAX_SYSTEM_ID;

		--
		-- Drop temporaries
		--

		DROP TABLE #TAX_SYSTEM_FISCAL_TERMS;

		DROP TABLE #TAX_SYSTEM_SHEETS_FISCAL_TERMS;

		--
		-- Update tag
		--

		UPDATE TAX_SYSTEM SET CREATED_BY=''USER'' WHERE TAX_SYSTEM_ID = @NEW_TAX_SYSTEM_ID;

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ERROR_PROCEDURE = ERROR_PROCEDURE(), @ERROR_MESSAGE = ERROR_MESSAGE();

		RAISERROR(''%s failed: %s'', 15, 1, @ERROR_PROCEDURE, @ERROR_MESSAGE);

		RETURN 1
	END CATCH

END
		'
		----------------------------------------------------------------
	END
END TRY
BEGIN CATCH
	DECLARE @ErrorMessage NVARCHAR(4000)
	DECLARE @ErrorSeverity INT
	DECLARE @ErrorState INT

	SELECT
		@ErrorMessage = ERROR_MESSAGE(),
		@ErrorSeverity = ERROR_SEVERITY(),
		@ErrorState = ERROR_STATE()

	IF (@@TRANCOUNT > 0)
		ROLLBACK TRANSACTION

	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH

IF (@@TRANCOUNT > 0)
	COMMIT TRANSACTION