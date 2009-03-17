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

var restore_before_delete = {};
var restore_after_restore = {};

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
    node('PT_DETAIL_CASH_FLOW_GROUP', 'TAX_NODE_ID', 'TAX_NODE');
    node('PT_DETAIL_CASH_FLOW_TIMESERIES', 'TAX_NODE_ID', 'TAX_NODE');
    node('PT_DETAIL_CASH_FLOW_DATA', 'TAX_NODE_ID', 'TAX_NODE');
    node('PT_ECONOMIC_INDICATOR', 'TAX_NODE_ID', 'TAX_NODE');
    node('PT_OTHER_INDICATOR', 'TAX_NODE_ID', 'TAX_NODE');
    node('FIELD_ADDITIONAL', 'INV_ASS_ORIGINAL_ID', 'INV_ASS', 'INV_ASS_ID');
    node('FIELD_ADDITIONAL', 'INV_ASS_REDIST_ID', 'INV_ASS', 'INV_ASS_ID');
    node('FIELD_ADDITIONAL', 'TAX_NODE_ID', 'TAX_NODE');
    node('PT_CASHFLOW_GRAPH', 'FIE_ID', 'FIELD_ADDITIONAL');
    node('INV_ASS_DATA', 'INV_ASS_ID', 'INV_ASS');
    node('INV_ASS_TUPLE_DATA', 'INV_ASS_ID', 'INV_ASS');
    node('FIELD_WELL_TABLE', 'FIE_ID', 'FIELD_HEADER');

    add_name('FIELD_HEADER', 'FIELD_NAME');

    add_uniq('FIELD_RESERVOIRS', 'RESV_ID');
    add_uniq('PT_DETAIL_CASH_FLOW_GROUP');
    add_uniq('PT_DETAIL_CASH_FLOW_TIMESERIES');
    add_uniq('PT_ECONOMIC_INDICATOR');
    add_uniq('FIELD_PHASE_DEVELOPMENT', 'PHASE_ID');

    add_update_ids(
            'PT_DETAIL_CASH_FLOW_TIMESERIES',
            '\t\t\tUPDATE #PT_DETAIL_CASH_FLOW_DATA{tmpSufix} SET {id} = @NEW_{id} WHERE {id} = @{id};\n'
            );

    add_update_ids(
            'PT_DETAIL_CASH_FLOW_GROUP',
            '\t\t\tUPDATE #PT_DETAIL_CASH_FLOW_TIMESERIES{tmpSufix} SET {id} = @NEW_{id} WHERE {id} = @{id};\n'
            );
}

function init_block()
{
    // BLOCK
    node('BLOCK_HEADER');
    node('BLOCK_ADDITIONAL', 'GA_ID', 'BLOCK_HEADER');
    node('GROUP_PARTNER_INTERESTS');
    node('BLOCK_HEADER', 'PAR_ID', 'GROUP_PARTNER_INTERESTS');
    node('BLOCK_HEADER', 'EPC_ID', 'CONTRACT_HEADER');

    node('CONTRACT_ADDITIONAL', 'EPC_ID', 'CONTRACT_HEADER');
    node('FIELD_CONTRACTS_BLOCKS', 'GA_ID', 'BLOCK_HEADER');
    node('FIELD_CONTRACTS_BLOCKS', 'EPC_ID', 'CONTRACT_HEADER');

    //add_where('BLOCK_HEADER', "LAST_STAGE_FLAG='Y'");
    //add_where('BLOCK_HEADER', "BLOCK_VALIDITY_FLAG='Y'");

    add_name('BLOCK_HEADER', 'BLOCK_NAME');
}

function init_complex()
{
    // COMPLEX
    add_id('FIELD_COMPLEX', 'FIELD_COMPLEX_ID');
    node('FIELD_COMPLEX', 'INV_ASS_ID', 'INV_ASS');
    node('INV_ASS_DATA', 'INV_ASS_ID', 'INV_ASS');
    node('INV_ASS_TUPLE_DATA', 'INV_ASS_ID', 'INV_ASS');

    add_name('FIELD_COMPLEX', 'FIELD_COMPLEX_NAME');

    restore_before_delete['COMPLEX'] =
    '\t\t--\n'
            + '\t\t-- Save fieds dependant on this complex\n'
            + '\t\t--\n\n'
            + '\t\tSELECT FIE_ID INTO #TEMP_FIELD_IDS\n'
            + '\t\t\tFROM FIELD_ADDITIONAL\n'
            + '\t\t\tWHERE FIELD_COMPLEX_ID = @FIELD_COMPLEX_ID;\n'
            + '\n'
            + '\t\t--\n'
            + '\t\t-- Unbind dependant fields\n'
            + '\t\t--\n\n'
            + '\t\tUPDATE FIELD_ADDITIONAL\n'
            + '\t\t\tSET FIELD_COMPLEX_ID = NULL\n'
            + '\t\t\tFROM FIELD_ADDITIONAL\n'
            + '\t\t\tINNER JOIN #TEMP_FIELD_IDS\n'
            + '\t\t\t\tON #TEMP_FIELD_IDS.FIE_ID = FIELD_ADDITIONAL.FIE_ID;';

    restore_after_restore['COMPLEX'] =
    '\t\t--\n'
            + '\t\t-- Restore link between fields and complex being restored\n'
            + '\t\t--\n\n'
            + '\t\tUPDATE FIELD_ADDITIONAL\n'
            + '\t\t\tSET FIELD_COMPLEX_ID = @FIELD_COMPLEX_ID\n'
            + '\t\t\tFROM FIELD_ADDITIONAL\n'
            + '\t\t\tINNER JOIN #TEMP_FIELD_IDS\n'
            + '\t\t\tON #TEMP_FIELD_IDS.FIE_ID = FIELD_ADDITIONAL.FIE_ID;\n'
            + '\n'
            + '\t\tDROP TABLE #TEMP_FIELD_IDS;';

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
    add_tee('GAS_PRICE_ID', '(SELECT GAS_PRICE_ID FROM GAS_PRICE{suffix} WHERE GLOBAL_ASSUMPTIONS_ID = @GLOBAL_ASSUMPTIONS_ID)');
    add_tee('LIQUID_PRICE_ID', '(SELECT LIQUID_PRICE_ID FROM LIQUID_PRICE{suffix} WHERE GLOBAL_ASSUMPTIONS_ID = @GLOBAL_ASSUMPTIONS_ID)');
    add_tee('INFLATION_ID', '(SELECT INFLATION_ID FROM INFLATION{suffix} WHERE GLOBAL_ASSUMPTIONS_ID = @GLOBAL_ASSUMPTIONS_ID)');

    add_uniq('GAS_PRICE');
    add_uniq('LIQUID_PRICE');
    add_uniq('INFLATION');

    add_name('GLOBAL_ASSUMPTIONS', 'GLOBALS_NAME');

    var update_parent_sql = '\t\t\tUPDATE #{table}{tmpSufix} SET {parentId} = @NEW_{parentId} WHERE {id} = @{id};\n';
    var update_data_sql = '\t\t\tUPDATE #{table}_DATA{tmpSufix} SET {id} = @NEW_{id} WHERE {id} = @{id};\n';

    add_update_ids('GAS_PRICE', update_parent_sql);
    add_update_ids('GAS_PRICE', update_data_sql);

    add_update_ids('LIQUID_PRICE', update_parent_sql);
    add_update_ids('LIQUID_PRICE', update_data_sql);

    add_update_ids('INFLATION', update_parent_sql);
    add_update_ids('INFLATION', update_data_sql);
}

function init_company()
{
    // COMPANY
    node('COMPANY_HEADER');
    node('COMPANY_ADDITIONAL', 'PU_ID', 'COMPANY_HEADER');

    //add_where('COMPANY_HEADER', "LAST_STAGE_FLAG='Y'");
    //add_where('COMPANY_HEADER', "STAGE_VALIDITY_FLAG='Y'");

    add_name('COMPANY_HEADER', 'COMPANY_NAME');
}

function init_tax_system()
{
    // TAXSYSTEM
    node('TAX_SYSTEM');
    node('TAX_SYSTEM_SHEETS', 'TAX_SYSTEM_ID', 'TAX_SYSTEM');

    add_name('TAX_SYSTEM', 'TAX_SYSTEM_NAME');
}
