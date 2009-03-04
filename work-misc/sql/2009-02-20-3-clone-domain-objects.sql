IF EXISTS (SELECT name FROM sys.objects WHERE type='P' AND name='SP_CLONE_FIELD')
	DROP PROCEDURE [SP_CLONE_FIELD]
GO
CREATE PROCEDURE [SP_CLONE_FIELD] @FIE_ID DECIMAL (12,0), @NEW_FIE_ID DECIMAL (12,0) OUTPUT
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

		SELECT * INTO #FIELD_HEADER_FIELD
			FROM FIELD_HEADER WHERE 42=10;

		SELECT * INTO #TAX_NODE_FIELD
			FROM TAX_NODE WHERE 42=10;

		SELECT * INTO #INV_ASS_FIELD
			FROM INV_ASS WHERE 42=10;

		SELECT * INTO #FIELD_ADDITIONAL_FIELD
			FROM FIELD_ADDITIONAL WHERE 42=10;

		SELECT * INTO #FIELD_CONTRACTS_BLOCKS_FIELD
			FROM FIELD_CONTRACTS_BLOCKS WHERE 42=10;

		SELECT * INTO #FIELD_RESERVOIRS_FIELD
			FROM FIELD_RESERVOIRS WHERE 42=10;

		SELECT * INTO #FIELD_RESV_LITHOLOGIES_FIELD
			FROM FIELD_RESV_LITHOLOGIES WHERE 42=10;

		SELECT * INTO #PT_CON_CASH_FLOW_DATA_FIELD
			FROM PT_CON_CASH_FLOW_DATA WHERE 42=10;

		SELECT * INTO #PT_DETAIL_CASH_FLOW_GROUP_FIELD
			FROM PT_DETAIL_CASH_FLOW_GROUP WHERE 42=10;

		SELECT * INTO #PT_DETAIL_CASH_FLOW_TIMESERIES_FIELD
			FROM PT_DETAIL_CASH_FLOW_TIMESERIES WHERE 42=10;

		SELECT * INTO #PT_DETAIL_CASH_FLOW_DATA_FIELD
			FROM PT_DETAIL_CASH_FLOW_DATA WHERE 42=10;

		SELECT * INTO #PT_ECONOMIC_INDICATOR_FIELD
			FROM PT_ECONOMIC_INDICATOR WHERE 42=10;

		SELECT * INTO #PT_OTHER_INDICATOR_FIELD
			FROM PT_OTHER_INDICATOR WHERE 42=10;

		SELECT * INTO #INV_ASS_DATA_FIELD
			FROM INV_ASS_DATA WHERE 42=10;

		SELECT * INTO #INV_ASS_TUPLE_DATA_FIELD
			FROM INV_ASS_TUPLE_DATA WHERE 42=10;

		SELECT * INTO #FIELD_PHASE_DEVELOPMENT_FIELD
			FROM FIELD_PHASE_DEVELOPMENT WHERE 42=10;

		--
		-- Declare id variables
		--

		DECLARE @TAX_NODE_ID DECIMAL(12,0)

		DECLARE @INV_ASS_ORIGINAL_ID DECIMAL(12,0)

		DECLARE @INV_ASS_REDIST_ID DECIMAL(12,0)

		--
		-- Set id variables
		--

		SELECT @TAX_NODE_ID = TAX_NODE_ID FROM FIELD_ADDITIONAL WHERE FIE_ID = @FIE_ID;

		SELECT @INV_ASS_ORIGINAL_ID = INV_ASS_ORIGINAL_ID FROM FIELD_ADDITIONAL WHERE FIE_ID = @FIE_ID;

		SELECT @INV_ASS_REDIST_ID = INV_ASS_REDIST_ID FROM FIELD_ADDITIONAL WHERE FIE_ID = @FIE_ID;

		--
		-- 'Backup' specifyed object to temporaties
		--

		INSERT INTO #FIELD_HEADER_FIELD
			SELECT * FROM FIELD_HEADER
			WHERE FIE_ID = @FIE_ID;

		INSERT INTO #TAX_NODE_FIELD
			SELECT * FROM TAX_NODE
			WHERE TAX_NODE_ID = @TAX_NODE_ID;

		INSERT INTO #INV_ASS_FIELD
			SELECT * FROM INV_ASS
			WHERE INV_ASS_ID = @INV_ASS_ORIGINAL_ID;

		INSERT INTO #INV_ASS_FIELD
			SELECT * FROM INV_ASS
			WHERE INV_ASS_ID = @INV_ASS_REDIST_ID;

		INSERT INTO #FIELD_ADDITIONAL_FIELD
			SELECT * FROM FIELD_ADDITIONAL
			WHERE FIE_ID = @FIE_ID;

		INSERT INTO #FIELD_CONTRACTS_BLOCKS_FIELD
			SELECT * FROM FIELD_CONTRACTS_BLOCKS
			WHERE FIE_ID = @FIE_ID;

		INSERT INTO #FIELD_RESERVOIRS_FIELD
			SELECT * FROM FIELD_RESERVOIRS
			WHERE FIE_ID = @FIE_ID;

		INSERT INTO #FIELD_RESV_LITHOLOGIES_FIELD
			SELECT * FROM FIELD_RESV_LITHOLOGIES
			WHERE FIE_ID = @FIE_ID;

		INSERT INTO #PT_CON_CASH_FLOW_DATA_FIELD
			SELECT * FROM PT_CON_CASH_FLOW_DATA
			WHERE TAX_NODE_ID = @TAX_NODE_ID;

		INSERT INTO #PT_DETAIL_CASH_FLOW_GROUP_FIELD
			SELECT * FROM PT_DETAIL_CASH_FLOW_GROUP
			WHERE TAX_NODE_ID = @TAX_NODE_ID;

		INSERT INTO #PT_DETAIL_CASH_FLOW_TIMESERIES_FIELD
			SELECT * FROM PT_DETAIL_CASH_FLOW_TIMESERIES
			WHERE TAX_NODE_ID = @TAX_NODE_ID;

		INSERT INTO #PT_DETAIL_CASH_FLOW_DATA_FIELD
			SELECT * FROM PT_DETAIL_CASH_FLOW_DATA
			WHERE TAX_NODE_ID = @TAX_NODE_ID;

		INSERT INTO #PT_ECONOMIC_INDICATOR_FIELD
			SELECT * FROM PT_ECONOMIC_INDICATOR
			WHERE TAX_NODE_ID = @TAX_NODE_ID;

		INSERT INTO #PT_OTHER_INDICATOR_FIELD
			SELECT * FROM PT_OTHER_INDICATOR
			WHERE TAX_NODE_ID = @TAX_NODE_ID;

		INSERT INTO #INV_ASS_DATA_FIELD
			SELECT * FROM INV_ASS_DATA
			WHERE INV_ASS_ID = @INV_ASS_ORIGINAL_ID;

		INSERT INTO #INV_ASS_DATA_FIELD
			SELECT * FROM INV_ASS_DATA
			WHERE INV_ASS_ID = @INV_ASS_REDIST_ID;

		INSERT INTO #INV_ASS_TUPLE_DATA_FIELD
			SELECT * FROM INV_ASS_TUPLE_DATA
			WHERE INV_ASS_ID = @INV_ASS_ORIGINAL_ID;

		INSERT INTO #INV_ASS_TUPLE_DATA_FIELD
			SELECT * FROM INV_ASS_TUPLE_DATA
			WHERE INV_ASS_ID = @INV_ASS_REDIST_ID;

		INSERT INTO #FIELD_PHASE_DEVELOPMENT_FIELD
			SELECT * FROM FIELD_PHASE_DEVELOPMENT
			WHERE FIE_ID = @FIE_ID;

		DECLARE @NAME VARCHAR(50)
		SELECT @NAME = 'Copy of ' + FIELD_NAME FROM #FIELD_HEADER_FIELD WHERE FIE_ID = @FIE_ID;

		DECLARE @N INT;
		SET @N = 2;

		WHILE EXISTS (SELECT FIELD_NAME FROM FIELD_HEADER WHERE FIELD_NAME = @NAME)
		BEGIN
			SELECT @NAME = 'Copy (' + CONVERT(VARCHAR(2), @N) + ') of ' + FIELD_NAME
				FROM #FIELD_HEADER_FIELD WHERE FIE_ID = @FIE_ID;

			SET @N = @N + 1;
		END

		UPDATE #FIELD_HEADER_FIELD SET FIELD_NAME = @NAME WHERE FIE_ID = @FIE_ID;

		--
		-- Get new IDs
		--

		DECLARE @NEW_TAX_NODE_ID DECIMAL (12, 0);

		DECLARE @NEW_INV_ASS_ORIGINAL_ID DECIMAL (12, 0);

		DECLARE @NEW_INV_ASS_REDIST_ID DECIMAL (12, 0);

		EXEC sp_GenerateNumericIdentity @NEW_FIE_ID OUTPUT, 'FIELD_HEADER', 'FIE_ID';

		EXEC sp_GenerateNumericIdentity @NEW_TAX_NODE_ID OUTPUT, 'TAX_NODE', 'TAX_NODE_ID';

		EXEC sp_GenerateNumericIdentity @NEW_INV_ASS_ORIGINAL_ID OUTPUT, 'INV_ASS', 'INV_ASS_ID';

		EXEC sp_GenerateNumericIdentity @NEW_INV_ASS_REDIST_ID OUTPUT, 'INV_ASS', 'INV_ASS_ID';

		--
		-- Update IDs
		--

		UPDATE #FIELD_HEADER_FIELD SET FIE_ID = @NEW_FIE_ID WHERE FIE_ID = @FIE_ID;

		UPDATE #TAX_NODE_FIELD SET TAX_NODE_ID = @NEW_TAX_NODE_ID WHERE TAX_NODE_ID = @TAX_NODE_ID;

		UPDATE #INV_ASS_FIELD SET INV_ASS_ID = @NEW_INV_ASS_ORIGINAL_ID WHERE INV_ASS_ID = @INV_ASS_ORIGINAL_ID;

		UPDATE #INV_ASS_FIELD SET INV_ASS_ID = @NEW_INV_ASS_REDIST_ID WHERE INV_ASS_ID = @INV_ASS_REDIST_ID;

		UPDATE #FIELD_ADDITIONAL_FIELD SET FIE_ID = @NEW_FIE_ID WHERE FIE_ID = @FIE_ID;

		UPDATE #FIELD_ADDITIONAL_FIELD SET INV_ASS_ORIGINAL_ID = @NEW_INV_ASS_ORIGINAL_ID WHERE INV_ASS_ORIGINAL_ID = @INV_ASS_ORIGINAL_ID;

		UPDATE #FIELD_ADDITIONAL_FIELD SET INV_ASS_REDIST_ID = @NEW_INV_ASS_REDIST_ID WHERE INV_ASS_REDIST_ID = @INV_ASS_REDIST_ID;

		UPDATE #FIELD_ADDITIONAL_FIELD SET TAX_NODE_ID = @NEW_TAX_NODE_ID WHERE TAX_NODE_ID = @TAX_NODE_ID;

		UPDATE #FIELD_CONTRACTS_BLOCKS_FIELD SET FIE_ID = @NEW_FIE_ID WHERE FIE_ID = @FIE_ID;

		UPDATE #FIELD_RESERVOIRS_FIELD SET FIE_ID = @NEW_FIE_ID WHERE FIE_ID = @FIE_ID;

		UPDATE #FIELD_RESV_LITHOLOGIES_FIELD SET FIE_ID = @NEW_FIE_ID WHERE FIE_ID = @FIE_ID;

		UPDATE #PT_CON_CASH_FLOW_DATA_FIELD SET TAX_NODE_ID = @NEW_TAX_NODE_ID WHERE TAX_NODE_ID = @TAX_NODE_ID;

		UPDATE #PT_DETAIL_CASH_FLOW_GROUP_FIELD SET TAX_NODE_ID = @NEW_TAX_NODE_ID WHERE TAX_NODE_ID = @TAX_NODE_ID;

		UPDATE #PT_DETAIL_CASH_FLOW_TIMESERIES_FIELD SET TAX_NODE_ID = @NEW_TAX_NODE_ID WHERE TAX_NODE_ID = @TAX_NODE_ID;

		UPDATE #PT_DETAIL_CASH_FLOW_DATA_FIELD SET TAX_NODE_ID = @NEW_TAX_NODE_ID WHERE TAX_NODE_ID = @TAX_NODE_ID;

		UPDATE #PT_ECONOMIC_INDICATOR_FIELD SET TAX_NODE_ID = @NEW_TAX_NODE_ID WHERE TAX_NODE_ID = @TAX_NODE_ID;

		UPDATE #PT_OTHER_INDICATOR_FIELD SET TAX_NODE_ID = @NEW_TAX_NODE_ID WHERE TAX_NODE_ID = @TAX_NODE_ID;

		UPDATE #INV_ASS_DATA_FIELD SET INV_ASS_ID = @NEW_INV_ASS_ORIGINAL_ID WHERE INV_ASS_ID = @INV_ASS_ORIGINAL_ID;

		UPDATE #INV_ASS_DATA_FIELD SET INV_ASS_ID = @NEW_INV_ASS_REDIST_ID WHERE INV_ASS_ID = @INV_ASS_REDIST_ID;

		UPDATE #INV_ASS_TUPLE_DATA_FIELD SET INV_ASS_ID = @NEW_INV_ASS_ORIGINAL_ID WHERE INV_ASS_ID = @INV_ASS_ORIGINAL_ID;

		UPDATE #INV_ASS_TUPLE_DATA_FIELD SET INV_ASS_ID = @NEW_INV_ASS_REDIST_ID WHERE INV_ASS_ID = @INV_ASS_REDIST_ID;

		UPDATE #FIELD_PHASE_DEVELOPMENT_FIELD SET FIE_ID = @NEW_FIE_ID WHERE FIE_ID = @FIE_ID;

		DECLARE @NEW_RESV_ID DECIMAL(12, 0);

		DECLARE CURS CURSOR
		FOR SELECT DISTINCT RESV_ID
			FROM #FIELD_RESERVOIRS_FIELD
			WHERE FIE_ID = @NEW_FIE_ID;

		OPEN CURS;

		DECLARE @RESV_ID DECIMAL(12, 0);

		FETCH NEXT FROM CURS INTO @RESV_ID;

		WHILE @@FETCH_STATUS = 0
		BEGIN
			EXEC sp_GenerateNumericIdentity @NEW_RESV_ID OUTPUT, 'FIELD_RESERVOIRS', 'RESV_ID';

			UPDATE #FIELD_RESERVOIRS_FIELD
				SET RESV_ID = @NEW_RESV_ID
				WHERE RESV_ID = @RESV_ID;

			FETCH NEXT FROM CURS INTO @RESV_ID;
		END

		CLOSE CURS;
		DEALLOCATE CURS;


		DECLARE @NEW_PT_DETAIL_CASH_FLOW_GROUP_ID DECIMAL(12, 0);

		DECLARE CURS CURSOR
		FOR SELECT DISTINCT PT_DETAIL_CASH_FLOW_GROUP_ID
			FROM #PT_DETAIL_CASH_FLOW_GROUP_FIELD
			WHERE TAX_NODE_ID = @NEW_TAX_NODE_ID;

		OPEN CURS;

		DECLARE @PT_DETAIL_CASH_FLOW_GROUP_ID DECIMAL(12, 0);

		FETCH NEXT FROM CURS INTO @PT_DETAIL_CASH_FLOW_GROUP_ID;

		WHILE @@FETCH_STATUS = 0
		BEGIN
			EXEC sp_GenerateNumericIdentity @NEW_PT_DETAIL_CASH_FLOW_GROUP_ID OUTPUT, 'PT_DETAIL_CASH_FLOW_GROUP', 'PT_DETAIL_CASH_FLOW_GROUP_ID';

			UPDATE #PT_DETAIL_CASH_FLOW_GROUP_FIELD
				SET PT_DETAIL_CASH_FLOW_GROUP_ID = @NEW_PT_DETAIL_CASH_FLOW_GROUP_ID
				WHERE PT_DETAIL_CASH_FLOW_GROUP_ID = @PT_DETAIL_CASH_FLOW_GROUP_ID;

			UPDATE #PT_DETAIL_CASH_FLOW_TIMESERIES_FIELD SET PT_DETAIL_CASH_FLOW_GROUP_ID = @NEW_PT_DETAIL_CASH_FLOW_GROUP_ID WHERE PT_DETAIL_CASH_FLOW_GROUP_ID = @PT_DETAIL_CASH_FLOW_GROUP_ID;

			FETCH NEXT FROM CURS INTO @PT_DETAIL_CASH_FLOW_GROUP_ID;
		END

		CLOSE CURS;
		DEALLOCATE CURS;


		DECLARE @NEW_PT_DETAIL_CASH_FLOW_TIMESERIES_ID DECIMAL(12, 0);

		DECLARE CURS CURSOR
		FOR SELECT DISTINCT PT_DETAIL_CASH_FLOW_TIMESERIES_ID
			FROM #PT_DETAIL_CASH_FLOW_TIMESERIES_FIELD
			WHERE TAX_NODE_ID = @NEW_TAX_NODE_ID;

		OPEN CURS;

		DECLARE @PT_DETAIL_CASH_FLOW_TIMESERIES_ID DECIMAL(12, 0);

		FETCH NEXT FROM CURS INTO @PT_DETAIL_CASH_FLOW_TIMESERIES_ID;

		WHILE @@FETCH_STATUS = 0
		BEGIN
			EXEC sp_GenerateNumericIdentity @NEW_PT_DETAIL_CASH_FLOW_TIMESERIES_ID OUTPUT, 'PT_DETAIL_CASH_FLOW_TIMESERIES', 'PT_DETAIL_CASH_FLOW_TIMESERIES_ID';

			UPDATE #PT_DETAIL_CASH_FLOW_TIMESERIES_FIELD
				SET PT_DETAIL_CASH_FLOW_TIMESERIES_ID = @NEW_PT_DETAIL_CASH_FLOW_TIMESERIES_ID
				WHERE PT_DETAIL_CASH_FLOW_TIMESERIES_ID = @PT_DETAIL_CASH_FLOW_TIMESERIES_ID;

			UPDATE #PT_DETAIL_CASH_FLOW_DATA_FIELD SET PT_DETAIL_CASH_FLOW_TIMESERIES_ID = @NEW_PT_DETAIL_CASH_FLOW_TIMESERIES_ID WHERE PT_DETAIL_CASH_FLOW_TIMESERIES_ID = @PT_DETAIL_CASH_FLOW_TIMESERIES_ID;

			FETCH NEXT FROM CURS INTO @PT_DETAIL_CASH_FLOW_TIMESERIES_ID;
		END

		CLOSE CURS;
		DEALLOCATE CURS;


		DECLARE @NEW_PT_ECONOMIC_INDICATOR_ID DECIMAL(12, 0);

		DECLARE CURS CURSOR
		FOR SELECT DISTINCT PT_ECONOMIC_INDICATOR_ID
			FROM #PT_ECONOMIC_INDICATOR_FIELD
			WHERE TAX_NODE_ID = @NEW_TAX_NODE_ID;

		OPEN CURS;

		DECLARE @PT_ECONOMIC_INDICATOR_ID DECIMAL(12, 0);

		FETCH NEXT FROM CURS INTO @PT_ECONOMIC_INDICATOR_ID;

		WHILE @@FETCH_STATUS = 0
		BEGIN
			EXEC sp_GenerateNumericIdentity @NEW_PT_ECONOMIC_INDICATOR_ID OUTPUT, 'PT_ECONOMIC_INDICATOR', 'PT_ECONOMIC_INDICATOR_ID';

			UPDATE #PT_ECONOMIC_INDICATOR_FIELD
				SET PT_ECONOMIC_INDICATOR_ID = @NEW_PT_ECONOMIC_INDICATOR_ID
				WHERE PT_ECONOMIC_INDICATOR_ID = @PT_ECONOMIC_INDICATOR_ID;

			FETCH NEXT FROM CURS INTO @PT_ECONOMIC_INDICATOR_ID;
		END

		CLOSE CURS;
		DEALLOCATE CURS;


		DECLARE @NEW_PHASE_ID DECIMAL(12, 0);

		DECLARE CURS CURSOR
		FOR SELECT DISTINCT PHASE_ID
			FROM #FIELD_PHASE_DEVELOPMENT_FIELD
			WHERE FIE_ID = @NEW_FIE_ID;

		OPEN CURS;

		DECLARE @PHASE_ID DECIMAL(12, 0);

		FETCH NEXT FROM CURS INTO @PHASE_ID;

		WHILE @@FETCH_STATUS = 0
		BEGIN
			EXEC sp_GenerateNumericIdentity @NEW_PHASE_ID OUTPUT, 'FIELD_PHASE_DEVELOPMENT', 'PHASE_ID';

			UPDATE #FIELD_PHASE_DEVELOPMENT_FIELD
				SET PHASE_ID = @NEW_PHASE_ID
				WHERE PHASE_ID = @PHASE_ID;

			FETCH NEXT FROM CURS INTO @PHASE_ID;
		END

		CLOSE CURS;
		DEALLOCATE CURS;


		--
		-- 'Restore' copy
		--

		INSERT INTO FIELD_HEADER
			SELECT * FROM #FIELD_HEADER_FIELD
			WHERE FIE_ID = @NEW_FIE_ID;

		INSERT INTO TAX_NODE
			SELECT * FROM #TAX_NODE_FIELD
			WHERE TAX_NODE_ID = @NEW_TAX_NODE_ID;

		INSERT INTO INV_ASS
			SELECT * FROM #INV_ASS_FIELD
			WHERE INV_ASS_ID = @NEW_INV_ASS_ORIGINAL_ID;

		INSERT INTO INV_ASS
			SELECT * FROM #INV_ASS_FIELD
			WHERE INV_ASS_ID = @NEW_INV_ASS_REDIST_ID;

		INSERT INTO FIELD_ADDITIONAL
			SELECT * FROM #FIELD_ADDITIONAL_FIELD
			WHERE FIE_ID = @NEW_FIE_ID;

		INSERT INTO FIELD_CONTRACTS_BLOCKS
			SELECT * FROM #FIELD_CONTRACTS_BLOCKS_FIELD
			WHERE FIE_ID = @NEW_FIE_ID;

		INSERT INTO FIELD_RESERVOIRS
			SELECT * FROM #FIELD_RESERVOIRS_FIELD
			WHERE FIE_ID = @NEW_FIE_ID;

		INSERT INTO FIELD_RESV_LITHOLOGIES
			SELECT * FROM #FIELD_RESV_LITHOLOGIES_FIELD
			WHERE FIE_ID = @NEW_FIE_ID;

		INSERT INTO PT_CON_CASH_FLOW_DATA
			SELECT * FROM #PT_CON_CASH_FLOW_DATA_FIELD
			WHERE TAX_NODE_ID = @NEW_TAX_NODE_ID;

		INSERT INTO PT_DETAIL_CASH_FLOW_GROUP
			SELECT * FROM #PT_DETAIL_CASH_FLOW_GROUP_FIELD
			WHERE TAX_NODE_ID = @NEW_TAX_NODE_ID;

		INSERT INTO PT_DETAIL_CASH_FLOW_TIMESERIES
			SELECT * FROM #PT_DETAIL_CASH_FLOW_TIMESERIES_FIELD
			WHERE TAX_NODE_ID = @NEW_TAX_NODE_ID;

		INSERT INTO PT_DETAIL_CASH_FLOW_DATA
			SELECT * FROM #PT_DETAIL_CASH_FLOW_DATA_FIELD
			WHERE TAX_NODE_ID = @NEW_TAX_NODE_ID;

		INSERT INTO PT_ECONOMIC_INDICATOR
			SELECT * FROM #PT_ECONOMIC_INDICATOR_FIELD
			WHERE TAX_NODE_ID = @NEW_TAX_NODE_ID;

		INSERT INTO PT_OTHER_INDICATOR
			SELECT * FROM #PT_OTHER_INDICATOR_FIELD
			WHERE TAX_NODE_ID = @NEW_TAX_NODE_ID;

		INSERT INTO INV_ASS_DATA
			SELECT * FROM #INV_ASS_DATA_FIELD
			WHERE INV_ASS_ID = @NEW_INV_ASS_ORIGINAL_ID;

		INSERT INTO INV_ASS_DATA
			SELECT * FROM #INV_ASS_DATA_FIELD
			WHERE INV_ASS_ID = @NEW_INV_ASS_REDIST_ID;

		INSERT INTO INV_ASS_TUPLE_DATA
			SELECT * FROM #INV_ASS_TUPLE_DATA_FIELD
			WHERE INV_ASS_ID = @NEW_INV_ASS_ORIGINAL_ID;

		INSERT INTO INV_ASS_TUPLE_DATA
			SELECT * FROM #INV_ASS_TUPLE_DATA_FIELD
			WHERE INV_ASS_ID = @NEW_INV_ASS_REDIST_ID;

		INSERT INTO FIELD_PHASE_DEVELOPMENT
			SELECT * FROM #FIELD_PHASE_DEVELOPMENT_FIELD
			WHERE FIE_ID = @NEW_FIE_ID;

		--
		-- Drop temporaries
		--

		DROP TABLE #FIELD_HEADER_FIELD;

		DROP TABLE #TAX_NODE_FIELD;

		DROP TABLE #INV_ASS_FIELD;

		DROP TABLE #FIELD_ADDITIONAL_FIELD;

		DROP TABLE #FIELD_CONTRACTS_BLOCKS_FIELD;

		DROP TABLE #FIELD_RESERVOIRS_FIELD;

		DROP TABLE #FIELD_RESV_LITHOLOGIES_FIELD;

		DROP TABLE #PT_CON_CASH_FLOW_DATA_FIELD;

		DROP TABLE #PT_DETAIL_CASH_FLOW_GROUP_FIELD;

		DROP TABLE #PT_DETAIL_CASH_FLOW_TIMESERIES_FIELD;

		DROP TABLE #PT_DETAIL_CASH_FLOW_DATA_FIELD;

		DROP TABLE #PT_ECONOMIC_INDICATOR_FIELD;

		DROP TABLE #PT_OTHER_INDICATOR_FIELD;

		DROP TABLE #INV_ASS_DATA_FIELD;

		DROP TABLE #INV_ASS_TUPLE_DATA_FIELD;

		DROP TABLE #FIELD_PHASE_DEVELOPMENT_FIELD;

		--
		-- Update tag
		--

		UPDATE FIELD_ADDITIONAL SET CREATED_BY='USER' WHERE FIE_ID = @NEW_FIE_ID;

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ERROR_PROCEDURE = ERROR_PROCEDURE(), @ERROR_MESSAGE = ERROR_MESSAGE();

		RAISERROR('%s failed: %s', 15, 1, @ERROR_PROCEDURE, @ERROR_MESSAGE);

		RETURN 1
	END CATCH

END
GO



GRANT EXEC ON [SP_CLONE_FIELD] TO [abu]
GO

IF EXISTS (SELECT name FROM sys.objects WHERE type='P' AND name='SP_CLONE_BLOCK')
	DROP PROCEDURE [SP_CLONE_BLOCK]
GO
CREATE PROCEDURE [SP_CLONE_BLOCK] @GA_ID DECIMAL (12,0), @NEW_GA_ID DECIMAL (12,0) OUTPUT
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

		SELECT * INTO #GROUP_PARTNER_INTERESTS_BLOCK
			FROM GROUP_PARTNER_INTERESTS WHERE 42=10;

		SELECT * INTO #CONTRACT_HEADER_BLOCK
			FROM CONTRACT_HEADER WHERE 42=10;

		SELECT * INTO #BLOCK_HEADER_BLOCK
			FROM BLOCK_HEADER WHERE 42=10;

		SELECT * INTO #CONTRACT_ADDITIONAL_BLOCK
			FROM CONTRACT_ADDITIONAL WHERE 42=10;

		SELECT * INTO #BLOCK_ADDITIONAL_BLOCK
			FROM BLOCK_ADDITIONAL WHERE 42=10;

		SELECT * INTO #FIELD_CONTRACTS_BLOCKS_BLOCK
			FROM FIELD_CONTRACTS_BLOCKS WHERE 42=10;

		--
		-- Declare id variables
		--

		DECLARE @PAR_ID DECIMAL(12,0)

		DECLARE @EPC_ID DECIMAL(12,0)

		--
		-- Set id variables
		--

		SELECT @PAR_ID = PAR_ID FROM BLOCK_HEADER WHERE GA_ID = @GA_ID;

		SELECT @EPC_ID = EPC_ID FROM BLOCK_HEADER WHERE GA_ID = @GA_ID;

		--
		-- 'Backup' specifyed object to temporaties
		--

		INSERT INTO #GROUP_PARTNER_INTERESTS_BLOCK
			SELECT * FROM GROUP_PARTNER_INTERESTS
			WHERE PAR_ID = @PAR_ID;

		INSERT INTO #CONTRACT_HEADER_BLOCK
			SELECT * FROM CONTRACT_HEADER
			WHERE EPC_ID = @EPC_ID;

		INSERT INTO #BLOCK_HEADER_BLOCK
			SELECT * FROM BLOCK_HEADER
			WHERE GA_ID = @GA_ID;

		INSERT INTO #CONTRACT_ADDITIONAL_BLOCK
			SELECT * FROM CONTRACT_ADDITIONAL
			WHERE EPC_ID = @EPC_ID;

		INSERT INTO #BLOCK_ADDITIONAL_BLOCK
			SELECT * FROM BLOCK_ADDITIONAL
			WHERE GA_ID = @GA_ID;

		INSERT INTO #FIELD_CONTRACTS_BLOCKS_BLOCK
			SELECT * FROM FIELD_CONTRACTS_BLOCKS
			WHERE GA_ID = @GA_ID;

		DECLARE @NAME VARCHAR(50)
		SELECT @NAME = 'Copy of ' + BLOCK_NAME FROM #BLOCK_HEADER_BLOCK WHERE GA_ID = @GA_ID;

		DECLARE @N INT;
		SET @N = 2;

		WHILE EXISTS (SELECT BLOCK_NAME FROM BLOCK_HEADER WHERE BLOCK_NAME = @NAME)
		BEGIN
			SELECT @NAME = 'Copy (' + CONVERT(VARCHAR(2), @N) + ') of ' + BLOCK_NAME
				FROM #BLOCK_HEADER_BLOCK WHERE GA_ID = @GA_ID;

			SET @N = @N + 1;
		END

		UPDATE #BLOCK_HEADER_BLOCK SET BLOCK_NAME = @NAME WHERE GA_ID = @GA_ID;

		--
		-- Get new IDs
		--

		DECLARE @NEW_PAR_ID DECIMAL (12, 0);

		DECLARE @NEW_EPC_ID DECIMAL (12, 0);

		EXEC sp_GenerateNumericIdentity @NEW_PAR_ID OUTPUT, 'GROUP_PARTNER_INTERESTS', 'PAR_ID';

		EXEC sp_GenerateNumericIdentity @NEW_EPC_ID OUTPUT, 'CONTRACT_HEADER', 'EPC_ID';

		EXEC sp_GenerateNumericIdentity @NEW_GA_ID OUTPUT, 'BLOCK_HEADER', 'GA_ID';

		--
		-- Update IDs
		--

		UPDATE #GROUP_PARTNER_INTERESTS_BLOCK SET PAR_ID = @NEW_PAR_ID WHERE PAR_ID = @PAR_ID;

		UPDATE #CONTRACT_HEADER_BLOCK SET EPC_ID = @NEW_EPC_ID WHERE EPC_ID = @EPC_ID;

		UPDATE #BLOCK_HEADER_BLOCK SET GA_ID = @NEW_GA_ID WHERE GA_ID = @GA_ID;

		UPDATE #BLOCK_HEADER_BLOCK SET PAR_ID = @NEW_PAR_ID WHERE PAR_ID = @PAR_ID;

		UPDATE #BLOCK_HEADER_BLOCK SET EPC_ID = @NEW_EPC_ID WHERE EPC_ID = @EPC_ID;

		UPDATE #CONTRACT_ADDITIONAL_BLOCK SET EPC_ID = @NEW_EPC_ID WHERE EPC_ID = @EPC_ID;

		UPDATE #BLOCK_ADDITIONAL_BLOCK SET GA_ID = @NEW_GA_ID WHERE GA_ID = @GA_ID;

		UPDATE #FIELD_CONTRACTS_BLOCKS_BLOCK SET GA_ID = @NEW_GA_ID WHERE GA_ID = @GA_ID;

		UPDATE #FIELD_CONTRACTS_BLOCKS_BLOCK SET EPC_ID = @NEW_EPC_ID WHERE EPC_ID = @EPC_ID;

		UPDATE #BLOCK_HEADER_BLOCK SET EPC_ID = @NEW_EPC_ID WHERE EPC_ID = @EPC_ID;

		UPDATE #FIELD_CONTRACTS_BLOCKS_BLOCK SET GA_ID = @NEW_GA_ID WHERE EPC_ID = @EPC_ID;

		--
		-- 'Restore' copy
		--

		INSERT INTO GROUP_PARTNER_INTERESTS
			SELECT * FROM #GROUP_PARTNER_INTERESTS_BLOCK
			WHERE PAR_ID = @NEW_PAR_ID;

		INSERT INTO CONTRACT_HEADER
			SELECT * FROM #CONTRACT_HEADER_BLOCK
			WHERE EPC_ID = @NEW_EPC_ID;

		INSERT INTO BLOCK_HEADER
			SELECT * FROM #BLOCK_HEADER_BLOCK
			WHERE GA_ID = @NEW_GA_ID;

		INSERT INTO CONTRACT_ADDITIONAL
			SELECT * FROM #CONTRACT_ADDITIONAL_BLOCK
			WHERE EPC_ID = @NEW_EPC_ID;

		INSERT INTO BLOCK_ADDITIONAL
			SELECT * FROM #BLOCK_ADDITIONAL_BLOCK
			WHERE GA_ID = @NEW_GA_ID;

		INSERT INTO FIELD_CONTRACTS_BLOCKS
			SELECT * FROM #FIELD_CONTRACTS_BLOCKS_BLOCK
			WHERE GA_ID = @NEW_GA_ID;

		--
		-- Drop temporaries
		--

		DROP TABLE #GROUP_PARTNER_INTERESTS_BLOCK;

		DROP TABLE #CONTRACT_HEADER_BLOCK;

		DROP TABLE #BLOCK_HEADER_BLOCK;

		DROP TABLE #CONTRACT_ADDITIONAL_BLOCK;

		DROP TABLE #BLOCK_ADDITIONAL_BLOCK;

		DROP TABLE #FIELD_CONTRACTS_BLOCKS_BLOCK;

		--
		-- Update tag
		--

		UPDATE BLOCK_ADDITIONAL SET CREATED_BY='USER' WHERE GA_ID = @NEW_GA_ID;

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ERROR_PROCEDURE = ERROR_PROCEDURE(), @ERROR_MESSAGE = ERROR_MESSAGE();

		RAISERROR('%s failed: %s', 15, 1, @ERROR_PROCEDURE, @ERROR_MESSAGE);

		RETURN 1
	END CATCH

END
GO



GRANT EXEC ON [SP_CLONE_BLOCK] TO [abu]
GO

IF EXISTS (SELECT name FROM sys.objects WHERE type='P' AND name='SP_CLONE_COMPLEX')
	DROP PROCEDURE [SP_CLONE_COMPLEX]
GO
CREATE PROCEDURE [SP_CLONE_COMPLEX] @FIELD_COMPLEX_ID DECIMAL (12,0), @NEW_FIELD_COMPLEX_ID DECIMAL (12,0) OUTPUT
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

		SELECT * INTO #INV_ASS_COMPLEX
			FROM INV_ASS WHERE 42=10;

		SELECT * INTO #FIELD_COMPLEX_COMPLEX
			FROM FIELD_COMPLEX WHERE 42=10;

		SELECT * INTO #INV_ASS_DATA_COMPLEX
			FROM INV_ASS_DATA WHERE 42=10;

		SELECT * INTO #INV_ASS_TUPLE_DATA_COMPLEX
			FROM INV_ASS_TUPLE_DATA WHERE 42=10;

		--
		-- Declare id variables
		--

		DECLARE @INV_ASS_ID DECIMAL(12,0)

		--
		-- Set id variables
		--

		SELECT @INV_ASS_ID = INV_ASS_ID FROM FIELD_COMPLEX WHERE FIELD_COMPLEX_ID = @FIELD_COMPLEX_ID;

		--
		-- 'Backup' specifyed object to temporaties
		--

		INSERT INTO #INV_ASS_COMPLEX
			SELECT * FROM INV_ASS
			WHERE INV_ASS_ID = @INV_ASS_ID;

		INSERT INTO #FIELD_COMPLEX_COMPLEX
			SELECT * FROM FIELD_COMPLEX
			WHERE FIELD_COMPLEX_ID = @FIELD_COMPLEX_ID;

		INSERT INTO #INV_ASS_DATA_COMPLEX
			SELECT * FROM INV_ASS_DATA
			WHERE INV_ASS_ID = @INV_ASS_ID;

		INSERT INTO #INV_ASS_TUPLE_DATA_COMPLEX
			SELECT * FROM INV_ASS_TUPLE_DATA
			WHERE INV_ASS_ID = @INV_ASS_ID;

		DECLARE @NAME VARCHAR(50)
		SELECT @NAME = 'Copy of ' + FIELD_COMPLEX_NAME FROM #FIELD_COMPLEX_COMPLEX WHERE FIELD_COMPLEX_ID = @FIELD_COMPLEX_ID;

		DECLARE @N INT;
		SET @N = 2;

		WHILE EXISTS (SELECT FIELD_COMPLEX_NAME FROM FIELD_COMPLEX WHERE FIELD_COMPLEX_NAME = @NAME)
		BEGIN
			SELECT @NAME = 'Copy (' + CONVERT(VARCHAR(2), @N) + ') of ' + FIELD_COMPLEX_NAME
				FROM #FIELD_COMPLEX_COMPLEX WHERE FIELD_COMPLEX_ID = @FIELD_COMPLEX_ID;

			SET @N = @N + 1;
		END

		UPDATE #FIELD_COMPLEX_COMPLEX SET FIELD_COMPLEX_NAME = @NAME WHERE FIELD_COMPLEX_ID = @FIELD_COMPLEX_ID;

		--
		-- Get new IDs
		--

		DECLARE @NEW_INV_ASS_ID DECIMAL (12, 0);

		EXEC sp_GenerateNumericIdentity @NEW_INV_ASS_ID OUTPUT, 'INV_ASS', 'INV_ASS_ID';

		EXEC sp_GenerateNumericIdentity @NEW_FIELD_COMPLEX_ID OUTPUT, 'FIELD_COMPLEX', 'FIELD_COMPLEX_ID';

		--
		-- Update IDs
		--

		UPDATE #INV_ASS_COMPLEX SET INV_ASS_ID = @NEW_INV_ASS_ID WHERE INV_ASS_ID = @INV_ASS_ID;

		UPDATE #FIELD_COMPLEX_COMPLEX SET FIELD_COMPLEX_ID = @NEW_FIELD_COMPLEX_ID WHERE FIELD_COMPLEX_ID = @FIELD_COMPLEX_ID;

		UPDATE #FIELD_COMPLEX_COMPLEX SET INV_ASS_ID = @NEW_INV_ASS_ID WHERE INV_ASS_ID = @INV_ASS_ID;

		UPDATE #INV_ASS_DATA_COMPLEX SET INV_ASS_ID = @NEW_INV_ASS_ID WHERE INV_ASS_ID = @INV_ASS_ID;

		UPDATE #INV_ASS_TUPLE_DATA_COMPLEX SET INV_ASS_ID = @NEW_INV_ASS_ID WHERE INV_ASS_ID = @INV_ASS_ID;

		--
		-- 'Restore' copy
		--

		INSERT INTO INV_ASS
			SELECT * FROM #INV_ASS_COMPLEX
			WHERE INV_ASS_ID = @NEW_INV_ASS_ID;

		INSERT INTO FIELD_COMPLEX
			SELECT * FROM #FIELD_COMPLEX_COMPLEX
			WHERE FIELD_COMPLEX_ID = @NEW_FIELD_COMPLEX_ID;

		INSERT INTO INV_ASS_DATA
			SELECT * FROM #INV_ASS_DATA_COMPLEX
			WHERE INV_ASS_ID = @NEW_INV_ASS_ID;

		INSERT INTO INV_ASS_TUPLE_DATA
			SELECT * FROM #INV_ASS_TUPLE_DATA_COMPLEX
			WHERE INV_ASS_ID = @NEW_INV_ASS_ID;

		--
		-- Drop temporaries
		--

		DROP TABLE #INV_ASS_COMPLEX;

		DROP TABLE #FIELD_COMPLEX_COMPLEX;

		DROP TABLE #INV_ASS_DATA_COMPLEX;

		DROP TABLE #INV_ASS_TUPLE_DATA_COMPLEX;

		--
		-- Update tag
		--

		UPDATE FIELD_COMPLEX SET CREATED_BY='USER' WHERE FIELD_COMPLEX_ID = @NEW_FIELD_COMPLEX_ID;

		--
		-- Clone all child fields
		--

		DECLARE @FIE_ID DECIMAL(12, 0);
		DECLARE @NEW_FIE_ID DECIMAL(12, 0);

		DECLARE FIELDS_CURSOR CURSOR
		FOR SELECT FIE_ID
			FROM FIELD_ADDITIONAL
			WHERE FIELD_COMPLEX_ID = @FIELD_COMPLEX_ID;

		OPEN FIELDS_CURSOR;

		FETCH NEXT FROM FIELDS_CURSOR INTO @FIE_ID;

		WHILE @@FETCH_STATUS = 0
		BEGIN
			EXEC SP_CLONE_FIELD @FIE_ID, @NEW_FIE_ID OUTPUT;
			UPDATE FIELD_ADDITIONAL SET
				FIELD_COMPLEX_ID = @NEW_FIELD_COMPLEX_ID
				WHERE FIE_ID = @NEW_FIE_ID

			FETCH NEXT FROM FIELDS_CURSOR INTO @FIE_ID;
		END

		CLOSE FIELDS_CURSOR;
		DEALLOCATE FIELDS_CURSOR;

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ERROR_PROCEDURE = ERROR_PROCEDURE(), @ERROR_MESSAGE = ERROR_MESSAGE();

		RAISERROR('%s failed: %s', 15, 1, @ERROR_PROCEDURE, @ERROR_MESSAGE);

		RETURN 1
	END CATCH

END
GO



GRANT EXEC ON [SP_CLONE_COMPLEX] TO [abu]
GO

IF EXISTS (SELECT name FROM sys.objects WHERE type='P' AND name='SP_CLONE_COMPANY')
	DROP PROCEDURE [SP_CLONE_COMPANY]
GO
CREATE PROCEDURE [SP_CLONE_COMPANY] @PU_ID DECIMAL (12,0), @NEW_PU_ID DECIMAL (12,0) OUTPUT
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
		-- 'Backup' specifyed object to temporaties
		--

		INSERT INTO #COMPANY_HEADER_COMPANY
			SELECT * FROM COMPANY_HEADER
			WHERE PU_ID = @PU_ID;

		INSERT INTO #COMPANY_ADDITIONAL_COMPANY
			SELECT * FROM COMPANY_ADDITIONAL
			WHERE PU_ID = @PU_ID;

		DECLARE @NAME VARCHAR(50)
		SELECT @NAME = 'Copy of ' + COMPANY_NAME FROM #COMPANY_HEADER_COMPANY WHERE PU_ID = @PU_ID;

		DECLARE @N INT;
		SET @N = 2;

		WHILE EXISTS (SELECT COMPANY_NAME FROM COMPANY_HEADER WHERE COMPANY_NAME = @NAME)
		BEGIN
			SELECT @NAME = 'Copy (' + CONVERT(VARCHAR(2), @N) + ') of ' + COMPANY_NAME
				FROM #COMPANY_HEADER_COMPANY WHERE PU_ID = @PU_ID;

			SET @N = @N + 1;
		END

		UPDATE #COMPANY_HEADER_COMPANY SET COMPANY_NAME = @NAME WHERE PU_ID = @PU_ID;

		--
		-- Get new IDs
		--

		EXEC sp_GenerateNumericIdentity @NEW_PU_ID OUTPUT, 'COMPANY_HEADER', 'PU_ID';

		--
		-- Update IDs
		--

		UPDATE #COMPANY_HEADER_COMPANY SET PU_ID = @NEW_PU_ID WHERE PU_ID = @PU_ID;

		UPDATE #COMPANY_ADDITIONAL_COMPANY SET PU_ID = @NEW_PU_ID WHERE PU_ID = @PU_ID;

		--
		-- 'Restore' copy
		--

		INSERT INTO COMPANY_HEADER
			SELECT * FROM #COMPANY_HEADER_COMPANY
			WHERE PU_ID = @NEW_PU_ID;

		INSERT INTO COMPANY_ADDITIONAL
			SELECT * FROM #COMPANY_ADDITIONAL_COMPANY
			WHERE PU_ID = @NEW_PU_ID;

		--
		-- Drop temporaries
		--

		DROP TABLE #COMPANY_HEADER_COMPANY;

		DROP TABLE #COMPANY_ADDITIONAL_COMPANY;

		--
		-- Update tag
		--

		UPDATE COMPANY_ADDITIONAL SET CREATED_BY='USER' WHERE PU_ID = @NEW_PU_ID;

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ERROR_PROCEDURE = ERROR_PROCEDURE(), @ERROR_MESSAGE = ERROR_MESSAGE();

		RAISERROR('%s failed: %s', 15, 1, @ERROR_PROCEDURE, @ERROR_MESSAGE);

		RETURN 1
	END CATCH

END
GO



GRANT EXEC ON [SP_CLONE_COMPANY] TO [abu]
GO

IF EXISTS (SELECT name FROM sys.objects WHERE type='P' AND name='SP_CLONE_TAXSYSTEM')
	DROP PROCEDURE [SP_CLONE_TAXSYSTEM]
GO
CREATE PROCEDURE [SP_CLONE_TAXSYSTEM] @TAX_SYSTEM_ID DECIMAL (12,0), @NEW_TAX_SYSTEM_ID DECIMAL (12,0) OUTPUT
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

		SELECT * INTO #TAX_SYSTEM_TAXSYSTEM
			FROM TAX_SYSTEM WHERE 42=10;

		SELECT * INTO #TAX_SYSTEM_SHEETS_TAXSYSTEM
			FROM TAX_SYSTEM_SHEETS WHERE 42=10;

		--
		-- Declare id variables
		--

		--
		-- Set id variables
		--

		--
		-- 'Backup' specifyed object to temporaties
		--

		INSERT INTO #TAX_SYSTEM_TAXSYSTEM
			SELECT * FROM TAX_SYSTEM
			WHERE TAX_SYSTEM_ID = @TAX_SYSTEM_ID;

		INSERT INTO #TAX_SYSTEM_SHEETS_TAXSYSTEM
			SELECT * FROM TAX_SYSTEM_SHEETS
			WHERE TAX_SYSTEM_ID = @TAX_SYSTEM_ID;

		DECLARE @NAME VARCHAR(50)
		SELECT @NAME = 'Copy of ' + TAX_SYSTEM_NAME FROM #TAX_SYSTEM_TAXSYSTEM WHERE TAX_SYSTEM_ID = @TAX_SYSTEM_ID;

		DECLARE @N INT;
		SET @N = 2;

		WHILE EXISTS (SELECT TAX_SYSTEM_NAME FROM TAX_SYSTEM WHERE TAX_SYSTEM_NAME = @NAME)
		BEGIN
			SELECT @NAME = 'Copy (' + CONVERT(VARCHAR(2), @N) + ') of ' + TAX_SYSTEM_NAME
				FROM #TAX_SYSTEM_TAXSYSTEM WHERE TAX_SYSTEM_ID = @TAX_SYSTEM_ID;

			SET @N = @N + 1;
		END

		UPDATE #TAX_SYSTEM_TAXSYSTEM SET TAX_SYSTEM_NAME = @NAME WHERE TAX_SYSTEM_ID = @TAX_SYSTEM_ID;

		--
		-- Get new IDs
		--

		EXEC sp_GenerateNumericIdentity @NEW_TAX_SYSTEM_ID OUTPUT, 'TAX_SYSTEM', 'TAX_SYSTEM_ID';

		--
		-- Update IDs
		--

		UPDATE #TAX_SYSTEM_TAXSYSTEM SET TAX_SYSTEM_ID = @NEW_TAX_SYSTEM_ID WHERE TAX_SYSTEM_ID = @TAX_SYSTEM_ID;

		UPDATE #TAX_SYSTEM_SHEETS_TAXSYSTEM SET TAX_SYSTEM_ID = @NEW_TAX_SYSTEM_ID WHERE TAX_SYSTEM_ID = @TAX_SYSTEM_ID;

		--
		-- 'Restore' copy
		--

		INSERT INTO TAX_SYSTEM
			SELECT * FROM #TAX_SYSTEM_TAXSYSTEM
			WHERE TAX_SYSTEM_ID = @NEW_TAX_SYSTEM_ID;

		INSERT INTO TAX_SYSTEM_SHEETS
			SELECT * FROM #TAX_SYSTEM_SHEETS_TAXSYSTEM
			WHERE TAX_SYSTEM_ID = @NEW_TAX_SYSTEM_ID;

		--
		-- Drop temporaries
		--

		DROP TABLE #TAX_SYSTEM_TAXSYSTEM;

		DROP TABLE #TAX_SYSTEM_SHEETS_TAXSYSTEM;

		--
		-- Update tag
		--

		UPDATE TAX_SYSTEM SET CREATED_BY='USER' WHERE TAX_SYSTEM_ID = @NEW_TAX_SYSTEM_ID;

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ERROR_PROCEDURE = ERROR_PROCEDURE(), @ERROR_MESSAGE = ERROR_MESSAGE();

		RAISERROR('%s failed: %s', 15, 1, @ERROR_PROCEDURE, @ERROR_MESSAGE);

		RETURN 1
	END CATCH

END
GO



GRANT EXEC ON [SP_CLONE_TAXSYSTEM] TO [abu]
GO