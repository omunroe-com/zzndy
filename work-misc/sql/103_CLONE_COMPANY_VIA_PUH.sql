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
			@version = 103

	IF (@updateAcceptedFlag = 0)
	BEGIN
		-- Place your SQL below, preferably - inside the quotes---------
		-- REMEMBER: try to avoid DROPs/CREATEs, use ALTER instead; 
		--			 always check preconditions on the schema object you're going to modify
		--			 remove all GO statements, they won't work and will cause errors
		EXECUTE dbo.sp_executesql @statement = N'
ALTER PROCEDURE [PROC_CLONE_COMPANY] @PUH_ID DECIMAL (12,0), @NEW_PUH_ID DECIMAL (12,0) OUTPUT
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

		SELECT * INTO #COMPANY_HEADER_COMPANY
			FROM COMPANY_HEADER WHERE 42=10;

		SELECT * INTO #COMPANY_ADDITIONAL_COMPANY
			FROM COMPANY_ADDITIONAL WHERE 42=10;

		--
		-- Declare id variables
		--

		--
		-- Set id variables
		--

		--
		-- ''Backup'' specified object to temporaties
		--

		INSERT INTO #COMPANY_HEADER_COMPANY
			SELECT * FROM COMPANY_HEADER
			WHERE PUH_ID = @PUH_ID;

		INSERT INTO #COMPANY_ADDITIONAL_COMPANY
			SELECT * FROM COMPANY_ADDITIONAL
			WHERE PUH_ID = @PUH_ID;

		DECLARE @NAME VARCHAR(50)
		SELECT @NAME = ''Copy of '' + COMPANY_NAME FROM #COMPANY_HEADER_COMPANY WHERE PUH_ID = @PUH_ID;

		DECLARE @N INT;
		SET @N = 2;

		WHILE EXISTS (SELECT COMPANY_NAME FROM COMPANY_HEADER WHERE COMPANY_NAME = @NAME)
		BEGIN
			SELECT @NAME = ''Copy ('' + CONVERT(VARCHAR(2), @N) + '') of '' + COMPANY_NAME
				FROM #COMPANY_HEADER_COMPANY WHERE PUH_ID = @PUH_ID;

			SET @N = @N + 1;
		END

		UPDATE #COMPANY_HEADER_COMPANY SET COMPANY_NAME = @NAME WHERE PUH_ID = @PUH_ID;

		--
		-- Get new IDs
		--

		EXEC sp_GenerateNumericIdentity @NEW_PUH_ID OUTPUT, ''COMPANY_HEADER'', ''PUH_ID'';

		--
		-- Update IDs
		--

		UPDATE #COMPANY_HEADER_COMPANY SET PUH_ID = @NEW_PUH_ID WHERE PUH_ID = @PUH_ID;

		UPDATE #COMPANY_ADDITIONAL_COMPANY SET PUH_ID = @NEW_PUH_ID WHERE PUH_ID = @PUH_ID;

		DECLARE @NEW_PU_ID DECIMAL(12, 0);

		EXEC sp_GenerateNumericIdentity @NEW_PU_ID OUTPUT, ''COMPANY_HEADER'', ''PU_ID'';

		UPDATE COMPANY_HEADER SET PU_ID = @NEW_PU_ID WHERE PUH_ID = @NEW_PUH_ID;

		UPDATE COMPANY_ADDITIONAL SET PU_ID = @NEW_PU_ID WHERE PUH_ID = @NEW_PUH_ID;

		--
		-- ''Restore'' copy
		--

		INSERT INTO COMPANY_HEADER
			SELECT * FROM #COMPANY_HEADER_COMPANY
			WHERE PUH_ID = @NEW_PUH_ID;

		INSERT INTO COMPANY_ADDITIONAL
			SELECT * FROM #COMPANY_ADDITIONAL_COMPANY
			WHERE PUH_ID = @NEW_PUH_ID;

		--
		-- Drop temporaries
		--

		DROP TABLE #COMPANY_HEADER_COMPANY;

		DROP TABLE #COMPANY_ADDITIONAL_COMPANY;

		--
		-- Update tag
		--

		UPDATE COMPANY_ADDITIONAL SET CREATED_BY=''USER'' WHERE PUH_ID = @NEW_PUH_ID;

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