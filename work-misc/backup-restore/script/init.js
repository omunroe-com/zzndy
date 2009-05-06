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
var clone_before_restore = {};

var paths = {};

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

    //node('INV_ASS_DATA', 'INV_ASS_ID', 'FIELD_PHASE_DEVELOPMENT');
    //node('INV_ASS_TUPLE_DATA', 'INV_ASS_ID', 'FIELD_PHASE_DEVELOPMENT');

    node('INV_ASS_DATA', 'INV_ASS_ID', 'INV_ASS');
    node('INV_ASS_TUPLE_DATA', 'INV_ASS_ID', 'INV_ASS');
    node('FIELD_WELL_TABLE', 'FIE_ID', 'FIELD_HEADER');
    node('PT_SUMMARY_GRAPH', 'FIE_ID', 'FIELD_HEADER');
    node('FIELD_REPORT', 'FIE_ID', 'FIELD_HEADER');

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

    add_path('FIELD', new Path('INV_ASS_TUPLE_DATA', 'FIELD_PHASE_DEVELOPMENT', 'FIE_ID', 'INV_ASS_ID', 'INV_ASS'));
    add_path('FIELD', new Path('INV_ASS', 'FIELD_PHASE_DEVELOPMENT', 'FIE_ID', 'INV_ASS_ID', 'INV_ASS'));

    restore_before_delete['FIELD'] =
    '\t\t--\n'
            + '\t\t-- Save IA dependant on this phase\n'
            + '\t\t--\n\n'

            + '\t\tSELECT INV_ASS.INV_ASS_ID, FIELD_PHASE_DEVELOPMENT.PHASE_ID\n'
            + '\t\t\tINTO #TEMP_INV_ASS_IDS\n'
            + '\t\t\tFROM INV_ASS\n'
            + '\t\t\tINNER JOIN FIELD_PHASE_DEVELOPMENT\n'
            + '\t\t\t\tON FIELD_PHASE_DEVELOPMENT.INV_ASS = INV_ASS.INV_ASS_ID\n'
            + '\t\t\tWHERE FIELD_PHASE_DEVELOPMENT.FIE_ID = @FIE_ID;\n\n'

            + '\t\t--\n'
            + '\t\t-- Destroy Field-Phase-IA link\n'
            + '\t\t--\n\n'

            + '\t\tUPDATE FIELD_PHASE_DEVELOPMENT\n'
            + '\t\t\tSET INV_ASS = NULL\n'
            + '\t\t\tWHERE FIE_ID = @FIE_ID;';

    restore_after_restore['FIELD'] =
    '\t\t--\n'
            + '\t\t-- Restore Field-Phase-IA link\n'
            + '\t\t--\n\n'

            + '\t\tUPDATE FIELD_PHASE_DEVELOPMENT\n'
            + '\t\t\tSET FIELD_PHASE_DEVELOPMENT.INV_ASS = #TEMP_INV_ASS_IDS.INV_ASS_ID\n'
            + '\t\t\tFROM FIELD_PHASE_DEVELOPMENT\n'
            + '\t\t\tINNER JOIN #TEMP_INV_ASS_IDS\n'
            + '\t\t\t\tON FIELD_PHASE_DEVELOPMENT.PHASE_ID = #TEMP_INV_ASS_IDS.PHASE_ID;\n\n'

            + '\t\tDROP TABLE #TEMP_INV_ASS_IDS;';

    clone_before_restore['FIELD']
            = '\n\t\t--\n\t\t-- Update phase to IA links\n\t\t--\n\n'
            +'\t\tDECLARE @NEW_PHASE_INV_ASS DECIMAL(12, 0);\n'
            + '\t\tDECLARE @PHASE_INV_ASS DECIMAL(12, 0);\n'
            + '\n'
            + '\t\tDECLARE CURS CURSOR\n'
            + '\t\tFOR SELECT DISTINCT PHASE_ID\n'
            + '\t\t\tFROM #FIELD_PHASE_DEVELOPMENT_FIELD\n'
            + '\t\t\tWHERE FIE_ID = @NEW_FIE_ID;\n'
            + '\n'
            + '\t\tOPEN CURS;\n'
            + '\n'
            + '\t\tDECLARE @IA_PHASE_ID DECIMAL(12, 0);\n'
            + '\n'
            + '\t\tFETCH NEXT FROM CURS INTO @IA_PHASE_ID;\n'
            + '\n'
            + '\t\tWHILE @@FETCH_STATUS = 0\n'
            + '\t\tBEGIN\n'
            + "\t\t\tEXEC sp_GenerateNumericIdentity @NEW_PHASE_INV_ASS OUTPUT, 'INV_ASS', 'INV_ASS_ID';\n"
            + '\n'
            + '\t\t\tSELECT @PHASE_INV_ASS = INV_ASS FROM #FIELD_PHASE_DEVELOPMENT_FIELD\n'
            + '\t\t\t\tWHERE PHASE_ID = @IA_PHASE_ID;\n'
            + '\n'
            + '\t\t\tUPDATE #FIELD_PHASE_DEVELOPMENT_FIELD\n'
            + '\t\t\t\tSET INV_ASS = @NEW_PHASE_INV_ASS\n'
            + '\t\t\t\tWHERE PHASE_ID = @IA_PHASE_ID;\n'
            + '\n'
            + '\t\t\tUPDATE #INV_ASS_FIELD\n'
            + '\t\t\t\tSET INV_ASS_ID = @NEW_PHASE_INV_ASS\n'
            + '\t\t\t\tWHERE INV_ASS_ID = @PHASE_INV_ASS;\n'
            + '\n'
            + '\t\t\tUPDATE #INV_ASS_TUPLE_DATA_FIELD\n'
            + '\t\t\t\tSET INV_ASS_ID = @NEW_PHASE_INV_ASS\n'
            + '\t\t\t\tWHERE INV_ASS_ID = @PHASE_INV_ASS;\n'
            + '\n'
            + '\t\t\tFETCH NEXT FROM CURS INTO @IA_PHASE_ID;\n'
            + '\t\tEND\n'
            + '\n'
            + '\t\tCLOSE CURS;\n'
            + '\t\tDEALLOCATE CURS;\n';

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

    add_save_relation('FIELD_ADDITIONAL', 'FIE_ID', 'FIELD_COMPLEX_ID');
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
    node('COMPANY_HEADER', 'PUH_ID');
    node('COMPANY_ADDITIONAL', 'PUH_ID', 'COMPANY_HEADER');

    //add_where('COMPANY_HEADER', "LAST_STAGE_FLAG='Y'");
    //add_where('COMPANY_HEADER', "STAGE_VALIDITY_FLAG='Y'");

    add_name('COMPANY_HEADER', 'COMPANY_NAME');

    clone_before_restore['COMPANY']
            = '\t\tDECLARE @NEW_PU_ID DECIMAL(12, 0);\n\n'
            + "\t\tEXEC sp_GenerateNumericIdentity @NEW_PU_ID OUTPUT, 'COMPANY_HEADER', 'PU_ID';\n\n"
            + '\t\tUPDATE COMPANY_HEADER SET PU_ID = @NEW_PU_ID WHERE PUH_ID = @NEW_PUH_ID;\n\n'
            + '\t\tUPDATE COMPANY_ADDITIONAL SET PU_ID = @NEW_PU_ID WHERE PUH_ID = @NEW_PUH_ID;';
}

function init_tax_system()
{
    // TAXSYSTEM
    node('TAX_SYSTEM');
    node('TAX_SYSTEM_SHEETS', 'TAX_SYSTEM_ID', 'TAX_SYSTEM');

    add_name('TAX_SYSTEM', 'TAX_SYSTEM_NAME');
    add_save_relation('TAX_NODE', 'TAX_NODE_ID', 'TAX_SYSTEM_ID', '(SELECT TOP 1 TAX_SYSTEM_ID FROM TAX_SYSTEM WHERE TAX_SYSTEM_ID != @TAX_SYSTEM_ID)');
}
