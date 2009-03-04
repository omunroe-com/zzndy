--
-- Default AssetBank database name is ABDB.
--
USE ABDB
GO
--
-- Always rewrite stored procedure.
--
IF EXISTS(SELECT name FROM sys.objects WHERE type='P' AND name='SP_CREATE_SHADOW_TABLE')
    DROP PROCEDURE [SP_CREATE_SHADOW_TABLE]
GO
--
-- Add shadow table if not exists.
--
CREATE PROCEDURE SP_CREATE_SHADOW_TABLE @for_table VARCHAR(50)
AS BEGIN
	SET NOCOUNT ON;

    --
    -- Master table must exist.
    --
    IF NOT EXISTS (SELECT name FROM sys.objects WHERE type='U' AND name=@for_table)
    BEGIN
        RAISERROR('Table %s not found.', 15, 1, @for_table)
        RETURN
    END

    DECLARE @shadow_table VARCHAR(57)
    SET @shadow_table = @for_table + '_SHADOW'
    
	DECLARE @sql NVARCHAR(133)

    --
    -- If shadow table alredy exists drop it.
    --
    IF EXISTS (SELECT name FROM sys.objects WHERE type='U' AND name=@shadow_table) 
    BEGIN
		SET @sql = N'DROP TABLE ' + @shadow_table;
		EXECUTE sp_executesql @stmt=@sql;
		PRINT 'ATTENTION: Old version of shadow table ' + @shadow_table + ' removed.';
	END

    --
    -- Create copy of master table.
    --
    SET @sql = N'SELECT TOP 1 * INTO ' + @shadow_table + ' FROM ' + @for_table
    EXECUTE sp_executesql @stmt=@sql

    SET @sql = N'DELETE FROM ' + @shadow_table
    EXECUTE sp_executesql @stmt=@sql
END
GO

--
-- Create tables to backup field domain object.
--
EXEC SP_CREATE_SHADOW_TABLE 'FIELD_HEADER';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'FIELD_ADDITIONAL';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'FIELD_CONTRACTS_BLOCKS';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'FIELD_PHASE_DEVELOPMENT';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'FIELD_RESERVOIRS';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'FIELD_RESV_LITHOLOGIES';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'PT_CON_CASH_FLOW_DATA';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'TAX_NODE';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'PT_DETAIL_CASH_FLOW_DATA';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'PT_DETAIL_CASH_FLOW_GROUP';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'PT_DETAIL_CASH_FLOW_TIMESERIES';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'PT_ECONOMIC_INDICATOR';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'PT_OTHER_INDICATOR';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'INV_ASS';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'INV_ASS_DATA';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'INV_ASS_TUPLE_DATA';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'BLOCK_HEADER';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'BLOCK_ADDITIONAL';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'GROUP_PARTNER_INTERESTS';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'CONTRACT_HEADER';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'CONTRACT_ADDITIONAL';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'FIELD_COMPLEX';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'GLOBAL_ASSUMPTIONS';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'GAS_PRICE_DATA';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'GAS_PRICE';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'INFLATION_DATA';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'INFLATION';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'LIQUID_PRICE_DATA';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'LIQUID_PRICE';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'COMPANY_HEADER';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'COMPANY_ADDITIONAL';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'TAX_SYSTEM';  
GO  
EXEC SP_CREATE_SHADOW_TABLE 'TAX_SYSTEM_SHEETS';  
GO  