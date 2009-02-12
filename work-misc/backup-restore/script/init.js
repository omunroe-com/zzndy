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

    add_name('FIELD_HEADER', 'FIELD_NAME');

    add_uniq('FIELD_RESERVOIRS', 'RESV_ID');
    add_uniq('PT_DETAIL_CASH_FLOW_GROUP');
    add_uniq('PT_DETAIL_CASH_FLOW_TIMESERIES');
    add_uniq('PT_ECONOMIC_INDICATOR');
    add_uniq('FIELD_PHASE_DEVELOPMENT', 'PHASE_ID');

    add_update_ids(
            'PT_DETAIL_CASH_FLOW_TIMESERIES',
            "\t\t\tUPDATE #PT_DETAIL_CASH_FLOW_DATA SET <Id> = @NEW_<Id> WHERE <Id> = @<Id>;\n"
            );
}

function init_block()
{
    // BLOCK
    node('BLOCK_HEADER');
    node('BLOCK_ADDITIONAL', 'GA_ID', 'BLOCK_HEADER');
    node('GROUP_PARTNER_INTERESTS');
    node('BLOCK_HEADER', 'PAR_ID', 'GROUP_PARTNER_INTERESTS');
    //node('CONTRACT_HEADER', 'PAR_ID', 'GROUP_PARTNER_INTERESTS');
    node('BLOCK_HEADER', 'EPC_ID', 'CONTRACT_HEADER');
    node('BLOCK_HEADER', 'EPC_ID', 'CONTRACT_ADDITIONAL');

    add_uniq();    

    //    [EPCSTG_ID] ASC,
    //	[EPC_ID] ASC

    add_name('BLOCK_HEADER', 'BLOCK_NAME');
}

function init_complex()
{
    // COMPLEX
    node('FIELD_COMPLEX', 'INV_ASS_ID', 'INV_ASS');
    node('INV_ASS_DATA', 'INV_ASS_ID', 'INV_ASS');
    node('INV_ASS_TUPLE_DATA', 'INV_ASS_ID', 'INV_ASS');

    add_name('FIELD_COMPLEX', 'FIELD_COMPLEX_NAME');
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

    add_uniq('GAS_PRICE');
    add_uniq('LIQUID_PRICE');
    add_uniq('INFLATION');

    add_name('GLOBAL_ASSUMPTIONS', 'GLOBALS_NAME');

    var update_parent_sql = '\t\t\tUPDATE #<Table> SET <ParentId> = @NEW_<ParentId> WHERE <Id> = @<Id>;\n';
    var update_data_sql = '\t\t\tUPDATE #<Table>_DATA SET <Id> = @NEW_<Id> WHERE <Id> = @<Id>;\n';

    add_update_ids('GAS_PRICE', update_parent_sql);
    add_update_ids('GAS_PRICE', update_data_sql);

    add_update_ids('LIQUID_PRICE', update_parent_sql);
    add_update_ids('LIQUID_PRICE', update_data_sql);

    add_update_ids('INFLATION', update_parent_sql);
    add_update_ids('INFLATION', update_data_sql);
}