/**
 * Tables with CREATED_BY/MODIFYED_BY columns
 */
add_tagged('FIELD_PHASE_DEVELOPMENT');
add_tagged('GLOBAL_ASSUMPTIONS');
add_tagged('CONTRACT_ADDITIONAL');
add_tagged('FIELD_ADDITIONAL');
add_tagged('LIQUID_PRICE');
add_tagged('GAS_PRICE');
add_tagged('INFLATION');
add_tagged('INV_ASS');
add_tagged('TAX_SYSTEM');
add_tagged('TAX_NODE');
add_tagged('FIELD_COMPLEX');
add_tagged('BLOCK_ADDITIONAL');
add_tagged('COMPANY_ADDITIONAL');

function init_field()
{
    // FIELD
    node('FIELD_HEADER');
    node('FIELD_ADDITIONAL', 'FIE_ID', 'FIELD_HEADER');
    node('FIELD_CONTRACTS_BLOCKS', 'FIE_ID', 'FIELD_HEADER');
    node('FIELD_PHASE_DEVELOPMENT', 'FIE_ID', 'FIELD_ADDITIONAL');
    node('FIELD_RESERVOIRS', 'FIE_ID', 'FIELD_HEADER');
    node('FIELD_RESV_LITHOLOGIES', 'FIE_ID', 'FIELD_HEADER');
    node('PT_CON_CASH_FLOW_DATA', 'TAX_NODE_ID', 'TAX_NODE');
    node('PT_DETAIL_CASH_FLOW_DATA', 'TAX_NODE_ID', 'TAX_NODE');
    node('PT_DETAIL_CASH_FLOW_GROUP', 'TAX_NODE_ID', 'TAX_NODE');
    node('PT_DETAIL_CASH_FLOW_TIMESERIES', 'TAX_NODE_ID', 'TAX_NODE');
    node('PT_ECONOMIC_INDICATOR', 'TAX_NODE_ID', 'TAX_NODE');
    node('PT_OTHER_INDICATOR', 'TAX_NODE_ID', 'TAX_NODE');
    node('FIELD_ADDITIONAL', 'INV_ASS_ORIGINAL_ID', 'INV_ASS', 'INV_ASS_ID');
    node('FIELD_ADDITIONAL', 'INV_ASS_REDIST_ID', 'INV_ASS', 'INV_ASS_ID');
    node('FIELD_ADDITIONAL', 'TAX_NODE_ID', 'TAX_NODE');
    node('INV_ASS_DATA', 'INV_ASS_ID', 'INV_ASS');
    node('INV_ASS_TUPLE_DATA', 'INV_ASS_ID', 'INV_ASS');
}

function init_block()
{
    // BLOCK
    node('BLOCK_HEADER');
    node('BLOCK_ADDITIONAL', 'GA_ID', 'BLOCK_HEADER');
    node('GROUP_PARTNER_INTERESTS');
    node('BLOCK_HEADER', 'PAR_ID', 'GROUP_PARTNER_INTERESTS');
    node('CONTRACT_HEADER', 'PAR_ID', 'GROUP_PARTNER_INTERESTS');
}

function init_complex()
{
    // COMPLEX
    node('FIELD_COMPLEX', 'INV_ASS_ID', 'INV_ASS');
    node('INV_ASS_DATA', 'INV_ASS_ID', 'INV_ASS');
    node('INV_ASS_TUPLE_DATA', 'INV_ASS_ID', 'INV_ASS');
}

function init_globals()
{
    // GLOBALS
    node('GLOBAL_ASSUMPTIONS');

    node('GAS_PRICE_DATA', 'GAS_PRICE_ID', 'GAS_PRICE');
    node('INFLATION_DATA', 'INFLATION_ID', 'INFLATION');
    node('LIQUID_PRICE_DATA', 'LIQUID_PRICE_ID', 'LIQUID_PRICE');
    node('GAS_PRICE', 'GLOBAL_ASSUMPTIONS_ID', 'GLOBAL_ASSUMPTIONS');
    node('LIQUID_PRICE', 'GLOBAL_ASSUMPTIONS_ID', 'GLOBAL_ASSUMPTIONS');
    node('INFLATION', 'GLOBAL_ASSUMPTIONS_ID', 'GLOBAL_ASSUMPTIONS');

    // Special case for GLOBALS' child objects
    add_tee('GAS_PRICE_ID', '(SELECT GAS_PRICE_ID FROM GAS_PRICE<Suffix> WHERE GLOBAL_ASSUMPTIONS_ID = @GLOBAL_ASSUMPTIONS_ID)');
    add_tee('LIQUID_PRICE_ID', '(SELECT LIQUID_PRICE_ID FROM LIQUID_PRICE<Suffix> WHERE GLOBAL_ASSUMPTIONS_ID = @GLOBAL_ASSUMPTIONS_ID)');
    add_tee('INFLATION_ID', '(SELECT INFLATION_ID FROM INFLATION<Suffix> WHERE GLOBAL_ASSUMPTIONS_ID = @GLOBAL_ASSUMPTIONS_ID)');

    // When creating new identities, GLOBALS' child objects need to have their id's updated
    add_update_ids('GLOBALS',
            "\t\tDECLARE @NEW_<Id> DECIMAL(12, 0);\n"
                    + "\t\tDECLARE CURS CURSOR\n"
                    + "\t\tFOR SELECT DISTINCT <Id> FROM <Table> WHERE GLOBAL_ASSUMPTIONS_ID = @NEW_GLOBAL_ASSUMPTIONS_ID;\n"
                    + "\n"
                    + "\t\tOPEN CURS;\n"
                    + "\t\tDECLARE @<Id> DECIMAL(12, 0);\n"
                    + "\n"
                    + "\t\tFETCH NEXT FROM CURS INTO @<Id>;\n"
                    + "\t\tWHILE @@FETCH_STATUS = 0\n"
                    + "\t\tBEGIN\n"
                    + "\t\t\tEXEC sp_GenerateNumericIdentity @NEW_<Id> OUTPUT, '<Table>', '<Id>';\n"
                    + "\t\t\tUPDATE <Table> SET GLOBAL_ASSUMPTIONS_ID = @NEW_GLOBAL_ASSUMPTIONS_ID WHERE <Id> = @<Id>;\n"
                    + "\t\t\tUPDATE <Table> SET <Id> = @NEW_<Id> WHERE <Id> = @<Id>;\n"
                    + "\t\t\tUPDATE <Table>_DATA SET <Id> = @NEW_<Id> WHERE <Id> = @<Id>;\n"
                    + "\n"
                    + "\t\t\tFETCH NEXT FROM CURS INTO @<Id>;\n"
                    + "\t\tEND\n"
                    + "\n"
                    + "\t\tCLOSE CURS;\n"
                    + "\t\tDEALLOCATE CURS;\n"
            );
}