--
-- Tables for saving tax engine calculation results
-- Updated on 12/26/2008: renmed columns to include units
--

IF NOT EXISTS (SELECT name FROM sysobjects WHERE id = object_id(N'[dbo].[PT_OTHER_INDICATOR]')
AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
CREATE TABLE [dbo].[PT_OTHER_INDICATOR](
	[TAX_NODE_ID] [decimal](12, 0) NOT NULL,
	[ASSET_EQUIVALENT_RESERVES_MMBBL] [decimal](11, 2) NOT NULL,                
	[OPERATING_EXPENSES] [decimal](11, 2) NOT NULL,
	[EXPLORATION_CAPITAL] [decimal](11, 2) NOT NULL,
	[DEVELOPMENT_CAPITAL] [decimal](11, 2) NOT NULL,
	[OTHER_CAPITAL] [decimal](11, 2) NOT NULL,
	[AVERAGE_PRICE_DOLLAR_BBL] [decimal](6, 2) NOT NULL,						
	[UNIT_OPERATING_EXENSES_DOLLAR_BBL] [decimal](6, 2) NOT NULL,				
	[UNIT_EXPLORATION_CAPITAL_DOLLAR_BBL] [decimal](6, 2) NOT NULL,				
	[UNIT_DEVELOPMENT_CAPITAL_DOLLAR_BBL] [decimal](6, 2) NOT NULL,				
	[UNIT_OTHER_CAPITAL_DOLLAR_BBL] [decimal](6, 2) NOT NULL,					
	[PRODUCING_LIFE_YEARS] [decimal](3, 0) NOT NULL,							
	[PROJECT_LIFE_YEARS] [decimal](3, 0) NOT NULL,								
	[GROUP_RATE_OF_RETURN_PCT] [decimal](5, 2) NOT NULL,						
	[NOC_RATE_OF_RETURN_PCT] [decimal](5, 2) NOT NULL,							
	[GOVERNMENT_RATE_OF_RETURN_PCT] [decimal](5, 2) NOT NULL,						
 CONSTRAINT [PK_PT_OTHER_INDICATOR] PRIMARY KEY CLUSTERED 
(
	[TAX_NODE_ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT name FROM sysobjects WHERE id = object_id(N'[dbo].[FK_PT_OTHER_INDICATOR_TAX_NODE]')
AND OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[PT_OTHER_INDICATOR]  WITH CHECK ADD  CONSTRAINT [FK_PT_OTHER_INDICATOR_TAX_NODE] FOREIGN KEY([TAX_NODE_ID])
REFERENCES [dbo].[TAX_NODE] ([TAX_NODE_ID])

GO
IF NOT EXISTS (SELECT name FROM sysobjects WHERE id = object_id(N'[dbo].[PT_ECONOMIC_INDICATOR]')
AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
CREATE TABLE [dbo].[PT_ECONOMIC_INDICATOR](
	[PT_ECONOMIC_INDICATOR_ID] [decimal](12, 0) NOT NULL,
	[TAX_NODE_ID] [decimal](12, 0) NOT NULL,
	[DISCOUNT_RATE_PCT] [decimal](5, 2) NOT NULL, 
	[GROUP_INCOME] [decimal](11, 2) NOT NULL,	
	[GROUP_OPEX] [decimal](11, 2) NOT NULL,		
	[GROUP_CAPEX] [decimal](11, 2) NOT NULL,	
	[GROUP_GOVERNMENT_PAYMENTS] [decimal](11, 2) NOT NULL,		
	[GROUP_CASH_FLOW] [decimal](11, 2) NOT NULL,				
	[GROUP_PAYOUT_YEARS] [decimal](5, 2) NOT NULL,			
	[GROUP_PROFITABILITY] [decimal](5, 2) NOT NULL,			
	[GROUP_GOVERNMENT_TAKE] [decimal](11, 2) NOT NULL,		
	[GOVERNMENT_CASH_FLOW] [decimal](11, 2) NOT NULL,		
 CONSTRAINT [PK_PT_ECONOMIC_INDICATOR] PRIMARY KEY CLUSTERED 
(
	[PT_ECONOMIC_INDICATOR_ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT name FROM sysobjects WHERE id = object_id(N'[dbo].[FK_PT_ECONOMIC_INDICATOR_TAX_NODE]')
AND OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[PT_ECONOMIC_INDICATOR]  WITH CHECK ADD  CONSTRAINT [FK_PT_ECONOMIC_INDICATOR_TAX_NODE] FOREIGN KEY([TAX_NODE_ID])
REFERENCES [dbo].[TAX_NODE] ([TAX_NODE_ID])

GO
IF EXISTS (SELECT name FROM sysobjects WHERE id = object_id(N'[dbo].[PT_CON_CASH_FLOW_DATA]')
AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
AND NOT EXISTS (SELECT 
			syscolumns.name AS [column], sysobjects.name AS [table], systypes.name AS [type]
		FROM syscolumns 
			LEFT JOIN sysobjects ON syscolumns.id = sysobjects.id
			LEFT JOIN systypes ON syscolumns.xtype = systypes.xtype
		WHERE sysobjects.type='U' AND syscolumns.name='GROUP_CAPITAL_EXPENDITURE' AND sysobjects.name='PT_CON_CASH_FLOW_DATA'
	)
	ALTER TABLE [dbo].[PT_CON_CASH_FLOW_DATA]
	ADD  
		[GROUP_INCOME] [decimal](11, 2) NOT NULL,                     
		[GROUP_GOVERNMENT_PAYMENT] [decimal](11, 2) NOT NULL,		
		[GROUP_OPERATING_EXPENSE] [decimal](11, 2) NOT NULL,		
		[GROUP_CAPITAL_EXPENDITURE] [decimal](11, 2) NOT NULL,		
		[AFTER_TAX_CASH_FLOW] [decimal](11, 2) NOT NULL				