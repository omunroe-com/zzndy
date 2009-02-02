sp_configure 'clr enabled', 1;
GO
RECONFIGURE
GO
PRINT 'DONE'
GO
IF EXISTS (SELECT name FROM sysobjects WHERE xtype = 'FS' AND name = 'GetValidNsNumberOrNull')
	DROP FUNCTION GetValidNsNumberOrNull
GO
IF EXISTS (SELECT name FROM sys.assemblies WHERE name = 'Xolved.CustomSqlValidation')
	DROP ASSEMBLY [Xolved.CustomSqlValidation] 
GO
PRINT 'Loading assembly'
GO
CREATE ASSEMBLY [Xolved.CustomSqlValidation] 
AUTHORIZATION [dbo] 
FROM 'C:\Xolved.CustomSqlValidation.dll ' 
WITH PERMISSION_SET = SAFE
GO
CREATE FUNCTION GetValidNsNumberOrNull ( @phone NVARCHAR(100) )
RETURNS NVARCHAR(21)
AS
EXTERNAL NAME [Xolved.CustomSqlValidation].UserDefinedFunctions.GetValidNsNumberOrNull
GO

PRINT 'Creating synchronization procedures (in connection with XDB_cw_MAIN)'
GO
IF EXISTS (SELECT name FROM sysobjects WHERE xtype = 'P' AND name = 'p_replicationExternalId')
	DROP PROCEDURE p_replicationExternalId
GO
PRINT '        creating p_replicationExternalId'
GO
CREATE PROCEDURE p_replicationExternalId	(
	@internalID VARCHAR(255),
	@externalID VARCHAR(255) OUTPUT
)
AS
BEGIN
	IF @internalID IS NULL OR @internalID = '' 
		SET @externalID = @internalID
	ELSE
		SELECT @externalID = DB_NAME() + ':' + @internalID
END
GO
IF EXISTS (SELECT name FROM sysobjects WHERE xtype = 'P' AND name = 'p_pushMessage')
	DROP PROCEDURE p_pushMessage
GO
PRINT '        creating p_pushMessage'
GO
CREATE PROCEDURE p_pushMessage(@msg NVARCHAR(1000), @message NVARCHAR(MAX) OUTPUT)
AS BEGIN
	IF @message IS NULL SET @message = ''
	SET @message = @message + @msg + '
'
END
GO
IF EXISTS (SELECT name FROM sysobjects WHERE xtype = 'P' AND name = 'p_getCustomListRefAsRef')
	DROP PROCEDURE p_getCustomListRefAsRef
GO
PRINT '        creating p_getCustomListRefAsRef'
GO
CREATE PROCEDURE p_getCustomListRefAsRef(@listRef VARCHAR(100) OUTPUT, @message NVARCHAR(MAX) OUTPUT)
AS BEGIN
	IF @listRef IS NULL OR PATINDEX('%:%', @listRef) = 0 GOTO FAIL
	DECLARE @listName NVARCHAR(255)
	DECLARE @value NVARCHAR(255)
	DECLARE @msg NVARCHAR(1000)

	EXECUTE [dbo].[p_unzipList] @listRef, @listName OUTPUT, @value OUTPUT
	IF @listName IS NULL BEGIN
		SET @msg = 'List not found for reference ' + @listRef
		EXECUTE p_pushMessage @msg, @message OUTPUT
		GOTO FAIL
	END
	IF @value IS NULL BEGIN
		SET @msg = 'List item not found for reference ' + @listRef
		EXECUTE p_pushMessage @msg, @message OUTPUT
		GOTO FAIL
	END

	EXECUTE [XDB_cw_MAIN].[dbo].[p_zipList] @listName, @value, @listRef OUTPUT
	IF @listRef IS NULL BEGIN
		SET @msg = 'Cannot map list ''' + @listName + ''' item ''' + @value + '''  to consolidation'
		EXECUTE p_pushMessage @msg, @message OUTPUT
		GOTO FAIL
	END

	RETURN 0

	FAIL:
		SET @listRef = NULL
		RETURN 1
END
GO
IF EXISTS (SELECT name FROM sysobjects WHERE xtype = 'P' AND name = 'p_getCustomListRefAsString')
	DROP PROCEDURE p_getCustomListRefAsString
GO
PRINT '        creating p_getCustomListRefAsString'
GO
CREATE PROCEDURE p_getCustomListRefAsString(@listRef NVARCHAR(100) OUTPUT, @message NVARCHAR(MAX) OUTPUT)
AS BEGIN
	IF @listRef IS NULL OR PATINDEX('%:%', @listRef) = 0 GOTO FAIL
	DECLARE @listName NVARCHAR(255)
	DECLARE @value NVARCHAR(255)
	DECLARE @msg NVARCHAR(1000)

	EXECUTE [dbo].[p_unzipList] @listRef, @listName OUTPUT, @value OUTPUT
	IF @listName IS NULL BEGIN
		SET @msg = 'List not found for reference ' + @listRef
		EXECUTE p_pushMessage @msg, @message OUTPUT
		GOTO FAIL
	END
	IF @listRef IS NULL BEGIN
		SET @msg = 'List item not found for reference ' + @listRef
		EXECUTE p_pushMessage @msg, @message OUTPUT
		GOTO FAIL
	END

	SET @listRef = @value
	RETURN 0
	FAIL:
		SET @listRef = NULL
		RETURN 1

END
GO

IF EXISTS (SELECT name FROM sysobjects WHERE xtype = 'P' AND name = 'p_replicateCustomerCustomFields')
	DROP PROCEDURE p_replicateCustomerCustomFields
GO
PRINT '        creating p_replicateCustomerCustomFields stub'
GO
---
--- Copy custom fields for specified entity with specified id into the target table 
--- for an entity with specified externalID
---
CREATE PROCEDURE p_replicateCustomerCustomFields	(
	@internalID VARCHAR(255),
	@externalID VARCHAR(255),
	@entityName VARCHAR(99),
	@message VARCHAR(MAX) OUTPUT
)
AS
BEGIN
	PRINT 'IMPLEMENTED ON RUNTIME'
END
GO

IF EXISTS (SELECT name FROM sysobjects WHERE xtype = 'P' AND name = 'p_makeCustomFieldsProc')
	DROP PROCEDURE p_makeCustomFieldsProc
GO
PRINT '        creating p_makeCustomFieldsProc'
GO
CREATE PROCEDURE p_makeCustomFieldsProc	(
	@entityName VARCHAR(99),
	@message VARCHAR(MAX) OUTPUT
)
AS BEGIN
	DECLARE @nl CHAR(4)
	SET @nl = '
'
	DEClARE @intSql VARCHAR(MAX)
	DECLARE @updSql VARCHAR(MAX)

	SET @intSql = 
		'CREATE PROCEDURE p_replicate' + @entityName + 'CustomFields('
			+ '@internalID VARCHAR(255),'
			+ '@externalID VARCHAR(255),'
			+ '@message VARCHAR(MAX) OUTPUT'
		+ ')'
		+ 'AS '
		+ 'BEGIN
'

	-- Create an entry in target table if it does not exist
	SET @intSql = @intSql +
		'IF NOT EXISTS (SELECT localID '
		+	'FROM [XDB_cw_MAIN].[dbo].[' + @entityName + '_customFields] '
		+	'WHERE parentExternalID = @externalID) '
		+'INSERT INTO [XDB_cw_MAIN].[dbo].[' + @entityName + '_customFields] (parentID, parentExternalID) '
		+'VALUES (@externalID, @externalID)' + @nl
	
	-- Iterate over all custom fields that are defined both in child
	-- and in parent tables
	DECLARE c_customFields CURSOR FAST_FORWARD
	FOR
		SELECT c.fieldName, c.fieldType, p.fieldType FROM [dbo].[CustomFieldsSetup] AS c
		  JOIN [XDB_cw_MAIN].[dbo].[CustomFieldsSetup] AS p 
		    ON (c.fieldName = p.fieldName 
			   AND (c.fieldType = p.fieldType OR
				   (c.fieldType = '_listRecord' AND p.fieldType = '_freeFormText'))
			   )
		 WHERE  c.appliesTo LIKE '%'+@entityName+'%'
			AND p.appliesTo LIKE '%'+@entityName+'%'
			AND c.toSynchronize = 1

	DECLARE @fieldName VARCHAR(101)
	DECLARE @targetFieldType VARCHAR(101)
	DECLARE @fieldType VARCHAR(101)
	
	SET @updSql = 'UPDATE [to] SET '

	OPEN c_customFields

	FETCH NEXT FROM c_customFields INTO @fieldName, @fieldType, @targetFieldType

	WHILE @@FETCH_STATUS = 0
	BEGIN

		IF @fieldType = '_listRecord' BEGIN
			SET @intSql = @intSql + 'DECLARE @' + @fieldName + 'Val VARCHAR(255)' + @nl
				+ 'SELECT @' + @fieldName + 'Val = ' + @fieldName + ' '
				+ 'FROM [dbo].[' + @entityName + '_customFields] '
				+ 'WHERE parentID = @internalID' + @nl
			IF @targetFieldType = '_listRecord' BEGIN
				SET @intSql = @intSql + 
					'EXECUTE p_getCustomListRefAsRef @' + @fieldName + 'Val OUTPUT, @message OUTPUT' + @nl
			END -- @targetFieldType = '_listRecord'
			ELSE BEGIN
				SET @intSql = @intSql + 
					'EXECUTE p_getCustomListRefAsString @' + @fieldName + 'Val OUTPUT, @message OUTPUT' + @nl
			END
			SET @updSql = @updSql + @fieldName + ' = @' + @fieldName + 'Val, '

		END --IF @fieldType = '_listRecord'
		ELSE BEGIN
			SET @updSql = @updSql + @fieldName + ' = [from].' + @fieldName + ', '
		END
		
		NEXT:
		FETCH NEXT FROM c_customFields INTO @fieldName, @fieldType, @targetFieldType
	END

	CLOSE c_customFields
	DEALLOCATE c_customFields

	SET @updSql = @updSql + 
		+ 'parentExternalID = @externalID ' + @nl -- to deal with commas
		+ 'FROM '
		+	'[XDB_cw_MAIN].[dbo].[' + @entityName + '_customFields] AS [to],'
		+	'[dbo].[' + @entityName + '_customFields] AS [from] '
		+ 'WHERE '
		+	'[to].parentExternalID = @externalID AND ' 
		+	'[from].parentID =       @internalID' + @nl
		+ 'END -- Proc'

	
	DECLARE @dropSql VARCHAR(255)

	SET @dropSql = 
		'IF EXISTS (SELECT name FROM sysobjects WHERE xtype = ''P'' AND name = ''p_replicate'+@entityName+'CustomFields'') ' +
			'DROP PROCEDURE p_replicate'+@entityName+'CustomFields'

	EXECUTE ( @dropSql )

	EXECUTE ( @intSql + @updSql )
END
GO
IF EXISTS (SELECT name FROM sysobjects WHERE xtype = 'P' AND name = 'p_replicateCustomer')
	DROP PROCEDURE p_replicateCustomer
GO
PRINT '        creating p_replicateCustomer'
GO
---
---	Create (if required) a customer in target table and update it with data
---
CREATE PROCEDURE p_replicateCustomer	(
	@internalID VARCHAR(255), 
	@externalID VARCHAR(255), 
	@externalParent VARCHAR(100)
)
AS
BEGIN
	DECLARE @isUpdated BIT
	SET @isUpdated = 0
	IF NOT EXISTS (SELECT localID FROM [XDB_cw_MAIN].[dbo].[Customer] WHERE externalID = @externalID) 
		INSERT INTO [XDB_cw_MAIN].[dbo].[Customer] (externalID) VALUES (@externalID)
	ELSE
		SET @isUpdated = 1

	UPDATE [to] SET
		externalId            = @externalID,
		customForm            = '-2',--[from].customForm,
		entityId              = [from].entityId,
		isPerson              = [from].isPerson,
-- IF isPerson THEN
		salutation            = CASE [from].isPerson WHEN 1 THEN ISNULL([from].salutation, '') ELSE NULL END,
		firstName             = CASE [from].isPerson WHEN 1 THEN ISNULL([from].firstName, '') ELSE NULL END,
		middleName            = CASE [from].isPerson WHEN 1 THEN ISNULL([from].middleName, '') ELSE NULL END,
		lastName              = CASE [from].isPerson WHEN 1 THEN ISNULL([from].lastName, '') ELSE NULL END,
		homePhone             = CASE [from].isPerson WHEN 1 THEN ISNULL([dbo].GetValidNsNumberOrNull([from].homePhone), '') ELSE NULL END,
		mobilePhone           = CASE [from].isPerson WHEN 1 THEN ISNULL([dbo].GetValidNsNumberOrNull([from].mobilePhone), '') ELSE NULL END,		
-- END IF
		companyName           = [from].companyName,
		entityStatus          = [from].entityStatus,
		entityStatus_external = [from].entityStatus_external,
		entityStatus_type     = [from].entityStatus_type,
		parent_external       = @externalParent,
		parent_type           = [from].parent_type,
		phone                 = ISNULL([dbo].GetValidNsNumberOrNull([from].phone), ''),
		fax                   = ISNULL([dbo].GetValidNsNumberOrNull([from].fax), ''),
		email                 = ISNULL([from].email, ''),
		url                   = ISNULL([from].url, ''),
		isInactive            = [from].isInactive,
		title                 = ISNULL([from].title, ''),
		printOnCheckAs        = [from].printOnCheckAs,
		altPhone              = ISNULL([dbo].GetValidNsNumberOrNull([from].altPhone), ''),
		altEmail              = ISNULL([from].altEmail, ''),
		comments              = [from].comments,
		emailPreference       = [from].emailPreference,
		vatRegNumber          = [from].vatRegNumber,
		accountNumber         = [from].accountNumber,
		creditLimit           = [from].creditLimit,
		taxable               = [from].taxable,
		resaleNumber          = [from].resaleNumber,
		startDate             = [from].startDate,
		endDate               = [from].endDate,
		reminderDays          = [from].reminderDays,
		thirdPartyAcct        = [from].thirdPartyAcct,
		thirdPartyZipcode     = [from].thirdPartyZipcode,
		password              = [from].password,
		password2             = [from].password2,
		lastPageVisited       = [from].lastPageVisited,
		lastVisit             = [from].lastVisit,
		localCreateFromNSDate = [from].localCreateFromNSDate,
		localUpdateFromNSDate = [from].localUpdateFromNSDate,
--		openingBalance                 = [from].openingBalance,
--		openingBalanceDate             = [from].openingBalanceDate,
--		parentID                       = [from].parentID,
--		parentLocalID                  = [from].parentLocalID,
--		parent                         = [from].parent,
--		customForm_external            = [from].customForm_external,
--		customForm_type                = [from].customForm_type,
--		defaultAddress                 = [from].defaultAddress,
--		category                       = [from].category,
--		category_external              = [from].category_external,
--		category_type                  = [from].category_type,
--		language                       = [from].language,
--		dateCreated                    = [from].dateCreated,
--		image                          = [from].image,
--		image_external                 = [from].image_external,
--		image_type                     = [from].image_type,
--		salesRep                       = [from].salesRep,
--		salesRep_external              = [from].salesRep_external,
--		salesRep_type                  = [from].salesRep_type,
--		territory                      = [from].territory,
--		territory_external             = [from].territory_external,
--		territory_type                 = [from].territory_type,
--		partner                        = [from].partner,
--		partner_external               = [from].partner_external,
--		partner_type                   = [from].partner_type,
--		salesGroup                     = [from].salesGroup,
--		salesGroup_external            = [from].salesGroup_external,
--		salesGroup_type                = [from].salesGroup_type,
--      creditHoldOverride             = [from].creditHoldOverride,
--		balance                        = [from].balance,
--		terms                          = [from].terms,
--		terms_external                 = [from].terms_external,
--		terms_type                     = [from].terms_type,
--		overdueBalance                 = [from].overdueBalance,
--		daysOverdue                    = [from].daysOverdue,
--		priceLevel                     = [from].priceLevel,
--		priceLevel_external            = [from].priceLevel_external,
--		priceLevel_type                = [from].priceLevel_type,
--		currency                       = [from].currency,
--		currency_external              = [from].currency_external,
--		currency_type                  = [from].currency_type,
--		prefCCProcessor                = [from].prefCCProcessor,
--		prefCCProcessor_external       = [from].prefCCProcessor_external,
--		prefCCProcessor_type           = [from].prefCCProcessor_type,
--		shipComplete                   = [from].shipComplete,
--		taxItem                        = [from].taxItem,
--		taxItem_external               = [from].taxItem_external,
--		taxItem_type                   = [from].taxItem_type,
--		shippingItem                   = [from].shippingItem,
--		shippingItem_external          = [from].shippingItem_external,
--		shippingItem_type              = [from].shippingItem_type,
--		thirdPartyCountry              = [from].thirdPartyCountry,
--		giveAccess                     = [from].giveAccess,
--		accessRole                     = [from].accessRole,
--		accessRole_external            = [from].accessRole_external,
--		accessRole_type                = [from].accessRole_type,
--		sendEmail                      = [from].sendEmail,
--      requirePwdChange               = [from].requirePwdChange,
--		accessHelp                     = [from].accessHelp,
--		campaignCategory               = [from].campaignCategory,
--		campaignCategory_external      = [from].campaignCategory_external,
--		campaignCategory_type          = [from].campaignCategory_type,
--		leadSource                     = [from].leadSource,
--		leadSource_external            = [from].leadSource_external,
--		leadSource_type                = [from].leadSource_type,
--		webLead                        = [from].webLead,
--		unsubscribe                    = [from].unsubscribe,
--		referrer                       = [from].referrer,
--		keywords                       = [from].keywords,
--		clickStream                    = [from].clickStream,
--		visits                         = [from].visits,
--		firstVisit                     = [from].firstVisit,
--		billPay                        = [from].billPay,
--		lastModifiedDate               = [from].lastModifiedDate,
--		openingBalanceAccount          = [from].openingBalanceAccount,
--		openingBalanceAccount_external = [from].openingBalanceAccount_external,
--		openingBalanceAccount_type     = [from].openingBalanceAccount_type,
--		stage                          = [from].stage,
		isUpdated = @isUpdated
	FROM
		[XDB_cw_MAIN].[dbo].[Customer] AS [to],
		  [dbo].[Customer] AS [from]
	WHERE 
		  [to].externalID = @externalID AND
		[from].internalID = @internalID
END
GO
IF EXISTS (SELECT name FROM sysobjects WHERE xtype = 'P' AND name = 'p_replicateCustomerAddressbook')
	DROP PROCEDURE p_replicateCustomerAddressbook
GO
PRINT '        creating p_replicateCustomerAddressbook'
GO
---
--- Create a copy of an addressbook identified by localID in external 
--- database for a Customer identified by parentID
---
CREATE PROCEDURE p_replicateCustomerAddressbook (@parentID VARCHAR(100), @localID UNIQUEIDENTIFIER)
AS
BEGIN
	INSERT INTO [XDB_cw_MAIN].[dbo].[CustomerAddressbook] 
	(
		defaultShipping,
		defaultBilling,
		isResidential,
		label,
		attention,
		addressee,
		phone,
		addr1,
		addr2,
		city,
		zip,
		country,
		addrText,
		override,
		state,
		localCreateFromNSDate,
		localUpdateFromNSDate,
		parentExternalID
	)
	SELECT 
		defaultShipping,
		defaultBilling,
		isResidential,
		label,
		attention,
		addressee,
		phone,
		addr1,
		addr2,
		city,
		zip,
		country,
		addrText,
		override,
		state,
		localCreateFromNSDate,
		localUpdateFromNSDate,
		@parentID
	FROM
		[dbo].[CustomerAddressbook] AS [from]
	WHERE localID = @localID
END
GO
IF EXISTS (SELECT name FROM sysobjects WHERE xtype = 'P' AND name = 'p_replicateCustomerAddressbookList')
	DROP PROCEDURE p_replicateCustomerAddressbookList
GO
PRINT '        creating p_replicateCustomerAddressbookList'
GO
---
--- delete customer addressbooks from target table for specified parent(external)ID
--- iterate over all addressbook associated with wpecified internalID and 
---	replicate them
---
CREATE PROCEDURE p_replicateCustomerAddressbookList	(
	@internalID VARCHAR(255),
	@externalID VARCHAR(255)
)
AS
BEGIN
	DELETE FROM [XDB_cw_MAIN].[dbo].[CustomerAddressbook] WHERE parentExternalID = @externalID

	DECLARE @ab_localID UNIQUEIDENTIFIER

	DECLARE c_addressbook CURSOR FAST_FORWARD
	FOR
		SELECT localID
		FROM [dbo].[CustomerAddressbook]
		WHERE parentID = @internalID

	OPEN c_addressbook

	FETCH NEXT FROM c_addressbook INTO @ab_localID

	WHILE @@FETCH_STATUS = 0
	BEGIN
		EXECUTE [dbo].[p_replicateCustomerAddressbook] @externalID, @ab_localID
		FETCH NEXT FROM c_addressbook INTO @ab_localID
	END

	CLOSE c_addressbook
	DEALLOCATE c_addressbook
END
GO
IF EXISTS (SELECT name FROM sysobjects WHERE xtype = 'P' AND name = 'p_synchronizeCustomer')
	DROP PROCEDURE p_synchronizeCustomer
GO
PRINT '        creating p_synchronizeCustomer'
GO
CREATE PROCEDURE p_synchronizeCustomer (
	@internalID VARCHAR(255), 
	@message VARCHAR(MAX) OUTPUT
)
AS
BEGIN
	DECLARE @errorCode INT
	DECLARE @actionComment VARCHAR(255)

	DECLARE @externalID VARCHAR(255)
	DECLARE @parentExternal VARCHAR(100)

	SET @actionComment = 'construct externalID'
	EXECUTE p_replicationExternalId @internalID, @externalID OUTPUT

	SELECT @errorCode = @@ERROR
	IF (@errorCode != 0) GOTO PROBLEM_IN_SYNCHRONIZE_CUSTOMER

	SET @actionComment = 'get parent externalId'

	SELECT @parentExternal = parent
		FROM [dbo].[Customer]
		WHERE internalID = @internalID

	SELECT @errorCode = @@ERROR
	IF (@errorCode != 0) GOTO PROBLEM_IN_SYNCHRONIZE_CUSTOMER

	SET @actionComment = 'construct parent external externalID (yes)'
	
	EXECUTE p_replicationExternalId @parentExternal, @parentExternal OUTPUT
	SELECT @errorCode = @@ERROR
	IF (@errorCode != 0) GOTO PROBLEM_IN_SYNCHRONIZE_CUSTOMER

	SET @actionComment = 'make a copy of customer (bodyfields)'

	EXECUTE p_replicateCustomer @internalID, @externalID, @parentExternal
	SELECT @errorCode = @@ERROR
	IF (@errorCode != 0) GOTO PROBLEM_IN_SYNCHRONIZE_CUSTOMER

	SET @actionComment = 'copy custom fields'

	EXECUTE p_replicateCustomerCustomFields @internalID, @externalID, @message OUTPUT
	SELECT @errorCode = @@ERROR
	IF (@errorCode != 0) GOTO PROBLEM_IN_SYNCHRONIZE_CUSTOMER

	SET @actionComment = 'copy balance'

	UPDATE [XDB_cw_MAIN].[dbo].[Customer_customFields]
	   SET custentity_child_balance = c.balance 
	  FROM [dbo].[Customer] AS c 
	  JOIN [XDB_cw_MAIN].[dbo].[Customer_customFields] AS f
	    ON f.parentExternalID = @externalID
	 WHERE c.internalID = @internalID

	SELECT @errorCode = @@ERROR
	IF (@errorCode != 0) GOTO PROBLEM_IN_SYNCHRONIZE_CUSTOMER

	SET @actionComment = 'set multi-store owner flag in consolidation'

	IF EXISTS (SELECT internalID FROM [dbo].[Contact] AS co
		  JOIN [dbo].[Contact_customFields] AS cf ON cf.parentID = co.internalID
		 WHERE co.company = @internalID
		   AND cf.custentity_multi_store_owner = 1)
	BEGIN
		UPDATE [XDB_cw_MAIN].[dbo].[Customer_customFields]
		   SET custentity_is_multi_store_owner = 1
		 WHERE parentExternalID = @externalID
	END
	  
	SELECT @errorCode = @@ERROR
	IF (@errorCode != 0) GOTO PROBLEM_IN_SYNCHRONIZE_CUSTOMER

--	SET @actionComment = 'copy all associated addressbooks'
--	EXECUTE p_replicateCustomerAddressbookList @internalID, @externalID
--	SELECT @errorCode = @@ERROR
--	IF (@errorCode != 0) GOTO PROBLEM_IN_SYNCHRONIZE_CUSTOMER

	RETURN 0

	PROBLEM_IN_SYNCHRONIZE_CUSTOMER:
	IF (@errorCode != 0) BEGIN
		PRINT N'Unexpected error occurred! Were trying to ' + @actionComment + ' (Customer #'+@internalID+'). Errorcode: ' + CAST(@errorCode AS VARCHAR(20))
		IF @message IS NULL SET @message = ''
		SET @message = @message + 'Sync error while trying to ' + @actionComment + ' (Customer #'+@internalID+'). Errorcode: ' + CAST(@errorCode AS VARCHAR(20)) + '

'
		RETURN 1
	END
END
GO
IF EXISTS (SELECT name FROM sysobjects WHERE xtype = 'P' AND name = 'p_synchronizeCustomersSince')
	DROP PROCEDURE p_synchronizeCustomersSince
GO
PRINT '        creating p_synchronizeCustomersSince'
GO
CREATE PROCEDURE p_synchronizeCustomersSince (
	@horizon DATETIME,
	@recordsAffected INT OUTPUT,
	@message NVARCHAR(MAX) OUTPUT
)
AS
BEGIN
	EXECUTE p_makeCustomFieldsProc 'Customer', @message OUTPUT

	BEGIN TRANSACTION

	DECLARE @internalID VARCHAR(255)
	DECLARE @RC int
	
	SET @recordsAffected = 0

	DECLARE c_customers CURSOR FAST_FORWARD 
	FOR
		SELECT c.internalID FROM Customer AS c
		INNER JOIN Customer_customFields AS f
		ON c.internalID = f.parentID
		WHERE NOT c.localUpdateFromNSDate IS NULL
			AND c.localUpdateFromNSDate >= @horizon
			AND f.custentity_is_master = 1

	OPEN c_customers

	FETCH NEXT FROM c_customers INTO @internalID

	WHILE @@FETCH_STATUS = 0
	BEGIN
		EXECUTE @RC = p_synchronizeCustomer @internalID, @message OUTPUT
		IF @RC != 0 GOTO PROBLEM_IN_SYNCHRONIZE_CUSTOMERS_SINCE
		SET @recordsAffected = @recordsAffected + 1
		FETCH NEXT FROM c_customers INTO @internalID
	END

	CLOSE c_customers
	DEALLOCATE c_customers

	COMMIT TRANSACTION
	RETURN 0
	
	PROBLEM_IN_SYNCHRONIZE_CUSTOMERS_SINCE:
	ROLLBACK TRANSACTION
	RETURN 1
END
GO
IF EXISTS (SELECT name FROM sysobjects WHERE xtype = 'P' AND name = 'p_AssertCustomFieldsSync')
	DROP PROCEDURE p_AssertCustomFieldsSync
GO
PRINT '        creating p_AssertCustomFieldsSync'
GO
--
-- Check if syncronization is avaiable for specified entity
-- if not, the 1 is returned and @entity is filled with the 
-- list of fields that cannot be synchronized
--
CREATE PROCEDURE p_AssertCustomFieldsSync @entity VARCHAR(MAX) OUTPUT
AS BEGIN
	DECLARE @fields VARCHAR(MAX)

	SELECT @fields = ISNULL(@fields + ';', '') + fieldName
	FROM [dbo].[CustomFieldsSetup]
	WHERE 
		NOT fieldName IN (
			SELECT c.fieldName
			FROM [dbo].[CustomFieldsSetup] AS c
			JOIN [XDB_cw_MAIN].[dbo].[CustomFieldsSetup] AS p
			ON 
				c.fieldName = p.fieldName 
				AND (c.fieldType = p.fieldType OR (c.fieldType = '_listRecord' AND p.fieldType = '_freeFormText'))
			WHERE
				c.toSynchronize = 1
				AND c.appliesTo like '%'+@entity+'%'
				AND p.appliesTo like '%'+@entity+'%'
		)
		AND appliesTo like '%'+@entity+'%'
		AND toSynchronize = 1

	IF NOT EXISTS (SELECT fieldName FROM [XDB_cw_MAIN].[dbo].[CustomFieldsSetup] 
		WHERE fieldName = 'custentity_is_multi_store_owner'
		  AND appliesTo like '%'+@entity+'%') 
		SET @fields = ISNULL(@fields, '') + ';custentity_is_multi_store_owner (in consolidation)'

	IF NOT EXISTS (SELECT fieldName FROM [XDB_cw_MAIN].[dbo].[CustomFieldsSetup] 
		WHERE fieldName = 'custentity_child_balance'
		  AND appliesTo like '%'+@entity+'%') 
		SET @fields = ISNULL(@fields, '') + ';custentity_child_balance (in consolidation)'

	IF NOT EXISTS (SELECT fieldName FROM [dbo].[CustomFieldsSetup] 
		WHERE fieldName = 'custentity_multi_store_owner'
		  AND appliesTo like '%Contact%') 
		SET @fields = ISNULL(@fields, '') + ';custentity_multi_store_owner (for Contact)'

	IF @fields IS NULL RETURN 0
	ELSE BEGIN
		SET @entity = @fields
		RETURN 1
	END
END
GO

IF EXISTS (SELECT name FROM sysobjects WHERE xtype = 'P' AND name = 'p_DesyncCustomFields')
	DROP PROCEDURE p_DesyncCustomFields
GO
PRINT '        creating p_DesyncCustomFields'
GO
--
-- Mark all fields in local DB that cannot be synchrronized
-- as not scheduled for synchronization to make sync possible
--
CREATE PROCEDURE p_DesyncCustomFields @entity VARCHAR(100) 
AS BEGIN
	UPDATE [dbo].[CustomFieldsSetup]
	   SET toSynchronize = 0
	WHERE 
		NOT fieldName IN (
			SELECT c.fieldName
			FROM [dbo].[CustomFieldsSetup] AS c
			JOIN [XDB_cw_MAIN].[dbo].[CustomFieldsSetup] AS p
			ON 
				c.fieldName = p.fieldName 
				AND (c.fieldType = p.fieldType OR (c.fieldType = '_listRecord' AND p.fieldType = '_freeFormText'))
			WHERE
				c.toSynchronize = 1
				AND c.appliesTo like '%'+@entity+'%'
				AND p.appliesTo like '%'+@entity+'%'
		)
		AND appliesTo like '%'+@entity+'%'
		AND toSynchronize = 1
END
GO
PRINT ''
GO
EXECUTE p_DesyncCustomFields 'Customer'
GO
