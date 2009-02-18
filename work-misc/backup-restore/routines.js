/**
 * $Id$
 */

var sql = {
    'update': "\t\tUPDATE {table} SET MODIFIED_BY='{user}'\n\t\t\tWHERE {idField}{op}{id};",
    'insert': "\t\tINSERT INTO {target}\n\t\t\tSELECT * FROM {source}\n\t\t\tWHERE {idField}{op}{id};",
    'insert_all': "\t\tINSERT INTO {target}\n\t\t\tSELECT * FROM {source};",
    'del': "\t\tDELETE FROM {table} WHERE\n\t\t\t{idField}{op}{id};",
    'declare': '\t\tDECLARE @{varName} DECIMAL(12,0)',
    'set':'\t\tSET @{varName} = ?',
    'sp_head':
            "IF EXISTS (SELECT name FROM sys.objects WHERE type='P' AND name='SP_{action}_{name}')\n"
                    + '\tDROP PROCEDURE [SP_{action}_{name}]\nGO\n'
                    + 'CREATE PROCEDURE [SP_{action}_{name}] @{id} DECIMAL (12,0)\nAS\nBEGIN\n'
                    + '\tSET NOCOUNT ON',
    'sp_copy_head':
            "IF EXISTS (SELECT name FROM sys.objects WHERE type='P' AND name='SP_{action}_{name}')\n"
                    + '\tDROP PROCEDURE [SP_{action}_{name}]\nGO\n'
                    + 'CREATE PROCEDURE [SP_{action}_{name}] @{id} DECIMAL (12,0), @NEW_{id} DECIMAL (12,0) OUTPUT\nAS\nBEGIN\n'
                    + '\tSET NOCOUNT ON',
    'shadow': "EXEC SP_CREATE_SHADOW_TABLE '{table}';",
    'sp_start':
            '\tDECLARE @ERROR_PROCEDURE NVARCHAR(255);\n\n'
                    + '\tDECLARE @ERROR_MESSAGE NVARCHAR(255);\n\n'
                    + '\tBEGIN TRY\n'
                    + '\t\tBEGIN TRANSACTION',
    'sp_end':
            '\t\tCOMMIT TRANSACTION\n'
                    + '\tEND TRY\n\n'
                    + '\tBEGIN CATCH\n'
                    + '\t\tROLLBACK TRANSACTION\n\n'
                    + '\t\tSELECT @ERROR_PROCEDURE = ERROR_PROCEDURE(), @ERROR_MESSAGE = ERROR_MESSAGE();\n\n'
                    + "\t\tRAISERROR('%s failed: %s', 15, 1, @ERROR_PROCEDURE, @ERROR_MESSAGE);\n\n"
                    + '\t\tRETURN 1\n'
                    + '\tEND CATCH\n\n'
                    + 'END\n'
                    + 'GO',
    'check_tag':"\tIF (SELECT MODIFIED_BY FROM {tagged} WHERE {idField}{op}{id}) != '{owner}'\n"
            + '\t\tRETURN -- No action required',
    'update_tag':"\t\tUPDATE {tagged} SET {tagField}='{owner}' WHERE {idField}{op}{id};",

    'make_temp': '\t\tSELECT * INTO #{table}{tmpSufix}\n'
            + '\t\t\tFROM {table} WHERE 42=10;',
    'decl_new_id': '\t\tDECLARE @NEW_{id} DECIMAL (12, 0);',
    'get_new_id': "\t\tEXEC sp_GenerateNumericIdentity @NEW_{id} OUTPUT, '{table}', '{idField}';",
    'update_id': '\t\tUPDATE {table} SET {idField} = @NEW_{id} WHERE {idField}{op}{id};',
    'update_id_all': '\t\tUPDATE {table} SET {idField} = @NEW_{id};',
    'drop_table': '\t\tDROP TABLE {table};',
    'grant_sp': 'GRANT EXEC ON [SP_{action}_{name}] TO [abu]\nGO',
    'iterate_ids':
            "\t\tDECLARE @NEW_{id} DECIMAL(12, 0);\n"
                    + "\n"
                    + "\t\tDECLARE CURS CURSOR\n"
                    + "\t\tFOR SELECT DISTINCT {id}\n"
                    + "\t\t\tFROM #{table}{tmpSufix}\n"
                    + "\t\t\tWHERE {parentId} = @NEW_{parentId};\n"
                    + "\n"
                    + "\t\tOPEN CURS;\n"
                    + "\n"
                    + "\t\tDECLARE @{id} DECIMAL(12, 0);\n"
                    + "\n"
                    + "\t\tFETCH NEXT FROM CURS INTO @{id};\n"
                    + "\n"
                    + "\t\tWHILE @@FETCH_STATUS = 0\n"
                    + "\t\tBEGIN\n"
                    + "\t\t\tEXEC sp_GenerateNumericIdentity @NEW_{id} OUTPUT, '{table}', '{id}';\n"
                    + "\n"
                    + "\t\t\tUPDATE #{table}{tmpSufix}\n"
                    + "\t\t\t\tSET {id} = @NEW_{id}\n"
                    + "\t\t\t\tWHERE {id} = @{id};\n"
                    + "{additional}"
                    + "\n"
                    + "\t\t\tFETCH NEXT FROM CURS INTO @{id};\n"
                    + "\t\tEND\n"
                    + "\n"
                    + "\t\tCLOSE CURS;\n"
                    + "\t\tDEALLOCATE CURS;\n",
    'set_name':
            "\t\tDECLARE @NAME VARCHAR(50)\n"
                    + "\t\tSELECT @NAME = 'Copy of ' + {nameField} FROM {table} WHERE {idField}{op}{id};\n"
                    + "\t\t\n"
                    + "\t\tDECLARE @N INT;\n"
                    + "\t\tSET @N = 2;\n"
                    + "\t\t\n"
                    + "\t\tWHILE EXISTS (SELECT {nameField} FROM {originalTable} WHERE {nameField} = @NAME)\n"
                    + "\t\tBEGIN\n"
                    + "\t\t\tSELECT @NAME = 'Copy (' + CONVERT(VARCHAR(2), @N) + ') of ' + {nameField}\n"
                    + "\t\t\t\tFROM {table} WHERE {idField}{op}{id};\n"
                    + "\t\t\n"
                    + "\t\t\tSET @N = @N + 1;\n"
                    + "\t\tEND\n"
                    + "\n"
                    + "\t\tUPDATE {table} SET {nameField} = @NAME WHERE {idField}{op}{id};",
    'clone_fields':
            '\t\tDECLARE @FIE_ID DECIMAL(12, 0);\n'
                    + '\t\tDECLARE @NEW_FIE_ID DECIMAL(12, 0);\n'
                    + '\t\t\n'
                    + '\t\tDECLARE FIELDS_CURSOR CURSOR\n'
                    + '\t\tFOR SELECT FIE_ID\n'
                    + '\t\t\tFROM FIELD_ADDITIONAL\n'
                    + '\t\t\tWHERE FIELD_COMPLEX_ID = @FIELD_COMPLEX_ID;\n'
                    + '\t\t\n'
                    + '\t\tOPEN FIELDS_CURSOR;\n'
                    + '\t\t\n'
                    + '\t\tFETCH NEXT FROM FIELDS_CURSOR INTO @FIE_ID;\n'
                    + '\t\t\n'
                    + '\t\tWHILE @@FETCH_STATUS = 0\n'
                    + '\t\tBEGIN\n'
                    + '\t\t\tEXEC SP_CLONE_FIELD @FIE_ID, @NEW_FIE_ID OUTPUT;\n'
                    + '\t\t\tUPDATE FIELD_ADDITIONAL SET\n'
                    + '\t\t\t\tFIELD_COMPLEX_ID = @NEW_FIELD_COMPLEX_ID\n'
                    + '\t\t\t\tWHERE FIE_ID = @NEW_FIE_ID\n'
                    + '\t\t\n'
                    + '\t\t\tFETCH NEXT FROM FIELDS_CURSOR INTO @FIE_ID;\n'
                    + '\t\tEND\n'
                    + '\t\t\n'
                    + '\t\tCLOSE FIELDS_CURSOR;\n'
                    + '\t\tDEALLOCATE FIELDS_CURSOR;'

};

var declares = {};

declares['FIELD'] = '\t\tSELECT @TAX_NODE_ID = TAX_NODE_ID FROM FIELD_ADDITIONAL WHERE FIE_ID = @FIE_ID\n\n'
        + '\t\tSELECT @INV_ASS_ORIGINAL_ID = INV_ASS_ORIGINAL_ID FROM FIELD_ADDITIONAL WHERE FIE_ID = @FIE_ID\n\n'
        + '\t\tSELECT @INV_ASS_REDIST_ID = INV_ASS_REDIST_ID FROM FIELD_ADDITIONAL WHERE FIE_ID = @FIE_ID';

declares['BLOCK'] = '\t\tSELECT @PAR_ID = PAR_ID FROM BLOCK_HEADER WHERE GA_ID = @GA_ID;\n\n'
        + '\t\tSELECT @EPC_ID = EPC_ID FROM BLOCK_HEADER WHERE GA_ID = @GA_ID';

declares['COMPLEX'] = '\t\tSELECT @INV_ASS_ID = INV_ASS_ID FROM FIELD_COMPLEX WHERE FIELD_COMPLEX_ID = @FIELD_COMPLEX_ID';

declares['GLOBALS'] = '';

var objectName = '<unset>';


function make_backup_sql(name, id, tagged) {
    function not_id(node) {
        return get_id(node.name) != id;
    }

    objectName = name;
    var out = [];

    out.push(sql.sp_head.fmt({action:'BACKUP', name:name, id:id}));

    // Used to set owner to 'IHS'
    setup_restore();

    // Backup routine
    // 0. Check if backup needed;
    out.push(sql.check_tag.fmt({
        tagged: tagged,
        idField: id,
        id: id,
        owner: owner,
        op: get_operator(id)}));

    // 1. Begin transaction
    out.push(sql.sp_start);

    setup_backup();

    var nodelist = rectify_nodes();

    // 2. Declare variables;
    out.push(comment('Declare id variables'));
    out = out.concat(nodelist.filter(not_id).map(to_sql(format_declare)).flatten().filter(is_valid_sql).uniq());

    out.push(comment('Set id variables'));
    if (name in declares)
        out.push(declares[name]);
    else
        out = out.concat(nodelist.filter(not_id).map(to_sql(format_set)).flatten().uniq());

    // 3. Delete any old backup information;
    out.push(comment('Delete old backup'));
    out = out.concat(nodelist.map(to_sql(format_del)).flatten().reverse());

    // 4. Copy data to shadow tables;
    out.push(comment('Backup data'));
    out = out.concat(nodelist.map(to_sql(format_copy)).flatten());

    // 5. Update tag.
    out.push(comment('Update tag'));
    out.push(sql.update_tag.fmt({
        tagged: tagged,
        idField: id,
        id: id,
        tagField: tagField,
        owner: owner,
        op: get_operator(id)}));

    out.push(sql.sp_end);
    write(out.join(statement_glue));
    write(sql.grant_sp.fmt({action:'BACKUP', name:name}));
    objectName = '<unset>';
}


function make_shadow_sql(table)
{
    return "EXEC SP_CREATE_SHADOW_TABLE '{table}';\nGO".fmt({table:table});
}

function write_shadow_generation()
{
    var out = [];
    out.push(comment('Create tables to backup field domain object.'));
    out = out.concat(all_nodes.map(make_shadow_sql));

    write(out.join(statement_glue));
}

/**
 * Replace all table names with their shadow equivalents. Called through reduce.
 * @param {String} value  a 'running total' value of the statement
 * @param {node} node     next node
 * @return {String}       next value of the statement
 */
function replace_all_tables(value, node) {
    return value.replace(new RegExp('\\b' + node.name + '\\b', 'g'), node.name + source_suffix);
}


function make_restore_sql(name, id, tagged) {
    function not_id(node) {
        return get_id(node.name) != id;
    }

    objectName = name;
    var out = [];

    out.push(sql.sp_head.fmt({action:'RESTORE', name:name, id:id}));

    setup_backup();

    // 0. Check if backup needed;
    out.push(sql.check_tag.fmt({
        tagged: tagged,
        idField: id,
        id: id,
        owner: owner,
        op: get_operator(id)}));

    out.push(sql.sp_start);

    setup_restore();

    var nodelist = rectify_nodes();

    // Restore routine:
    // 1. Declare variables;
    out.push(comment('Declare id variables'));
    out = out.concat(nodelist.filter(not_id).map(to_sql(format_declare)).flatten().filter(is_valid_sql).uniq());

    out.push(comment('Set id variables'));
    if (name in declares)
        out.push(declares[name]);
    else
        out = out.concat(nodelist.filter(not_id).map(to_sql(format_set)).flatten().filter(is_valid_sql).uniq());

    // 2. Delete user version;
    out.push(comment('Delete user version'));
    out = out.concat(nodelist.map(to_sql(format_del)).flatten().reverse());

    out.push(comment('Set id variables acording to backed copy'));
    if (name in declares)
    {
        var decl = declares[name];
        out.push(nodelist.reduce(replace_all_tables, decl));
    }
    else
        out = out.concat(nodelist.filter(not_id).map(to_sql(format_set)).flatten().filter(is_valid_sql).uniq());

    // 3. Copy data from shadow tables;
    out.push(comment('Restore data'));
    out = out.concat(nodelist.map(to_sql(format_copy)).flatten());

    // 4. Delete backup.
    setup_backup();
    out.push(comment('Delete backup'));
    out = out.concat(nodelist.map(to_sql(format_del)).flatten().uniq().reverse());

    // 5. update tags;
    setup_restore();
    out.push(comment('Update tag'));
    out.push(sql.update_tag.fmt({
        tagged: tagged,
        idField: id,
        id: id,
        tagField: tagField,
        owner: owner,
        op: get_operator(id)}));

    out.push(sql.sp_end);
    write(out.join(statement_glue));
    write(sql.grant_sp.fmt({action:'RESTORE', name:name}));
    objectName = '<unset>';
}

function make_clone_sql(name, id, tagged) {
    function not_id(node) {
        return get_id(node.name) != id;
    }

    var ids = [];
    function is_new_id(node) {
        var id = get_id(node.name);
        var present = ids.indexOf(id) != -1;
        if (!present)
            ids.push(id);
        return !present;
    }

    function ins_new_prefix(text) {
        return text.replace(/=\s@/, '= @NEW_');
    }

    function get_uniq_struct(node) {
        return {name:node, uniqs:get_uniqs(node)};
    }

    function uniq_pipe(nodelist, fmt_fn)
    {
        return nodelist
                .map(get_name)
                .filter(has_uniqs)
                .map(get_uniq_struct)
                .map(to_sql_struct(fmt_fn))
                .flatten()
                .filter(is_valid_sql)
                .uniq();
    }

    objectName = name;
    var out = [];

    out.push(sql.sp_copy_head.fmt({action:'CLONE', name:name, id:id}));
    out.push(sql.sp_start);

    setup_restore();

    var nodelist = rectify_nodes();

    // Copy routine:
    // 1. Create temporary tables;
    out.push(comment('Create temporary tables (Just copy tables structure)'));
    out = out.concat(nodelist.map(to_sql(format_temp)).flatten().uniq());

    // 2. Declare ids
    out.push(comment('Declare id variables'));
    out = out.concat(nodelist.filter(not_id).map(to_sql(format_declare)).flatten().filter(is_valid_sql).uniq());

    // 3. Set ids
    out.push(comment('Set id variables'));
    if (name in declares)
        out.push(declares[name]);
    else
        out = out.concat(nodelist.filter(not_id).map(to_sql(format_set)).flatten().uniq());

    // 4. 'Backup' specifyed object to temporaties;
    setup_backup_temp();
    out.push(comment("'Backup' specifyed object to temporaties"));
    out = out.concat(nodelist.map(to_sql(format_copy)).flatten());

    var entity_name = get_entity_name();
    var entity_id = get_id(entity_name.table);

    if (entity_id) {
        out.push(sql.set_name.fmt({
            table:target_prefix + entity_name.table + target_suffix,
            originalTable: entity_name.table,
            nameField:entity_name.column,
            idField:entity_id ,
            op:get_operator(entity_id) ,
            id:entity_id
        }).fmt({tmpSufix:'_' + objectName}));
    }

    // 5. Get new IDs;
    out.push(comment('Get new IDs'));
    out = out.concat(nodelist.filter(not_id).filter(is_new_id).map(to_sql(format_decl_new)).flatten().filter(is_valid_sql).uniq());
    ids = [];
    out = out.concat(nodelist.filter(is_new_id).map(to_sql(format_get_new)).flatten().filter(is_valid_sql).uniq());

    // 6. Update IDs;
    out.push(comment('Update IDs'));
    out = out.concat(nodelist.map(to_sql(format_update_id)).flatten().filter(is_valid_sql));

    // HARDCODE
    if (name == 'BLOCK') {
        out.push('\t\tUPDATE #BLOCK_HEADER SET EPC_ID = @NEW_EPC_ID WHERE EPC_ID = @EPC_ID;');
        out.push('\t\tUPDATE #FIELD_CONTRACTS_BLOCKS SET GA_ID = @NEW_GA_ID WHERE EPC_ID = @EPC_ID;');
    }

    out = out.concat(uniq_pipe(nodelist, format_iterate_ids));

    setup_restore_temp();

    // 7. 'Restore' copy;
    out.push(comment("'Restore' copy"));
    out = out.concat(nodelist.map(to_sql(format_copy)).flatten().map(ins_new_prefix));

    // 8. Drop temporaries;
    out.push(comment('Drop temporaries'));
    out = out.concat(nodelist.map(to_sql(format_drop)).flatten().uniq());

    // 9. update tags.
    out.push(comment('Update tag'));
    out.push(sql.update_tag.fmt({
        tagged: tagged,
        idField: id,
        id: id,
        tagField: tagField,
        owner: owner,
        op: get_operator(id)}));

    // HARDCODE
    if (name == 'COMPLEX') {
        out.push(comment('Clone all child fields'));
        out.push(sql.clone_fields);
    }

    out.push(sql.sp_end);
    write(out.join(statement_glue));
    write(sql.grant_sp.fmt({action:'CLONE', name:name}));
    objectName = '<unset>';
}

function make_delete_sql(name, id/*, tagged*/) {
    function not_id(node) {
        return get_id(node.name) != id;
    }

    objectName = name;
    var out = [];

    out.push(sql.sp_head.fmt({action:'DELETE', name:name, id:id}));
    out.push(sql.sp_start);

    setup_restore();

    var nodelist = rectify_nodes();

    out.push(comment('Declare id variables'));
    out = out.concat(nodelist.filter(not_id).map(to_sql(format_declare)).flatten().filter(is_valid_sql).uniq());

    out.push(comment('Set id variables'));
    if (name in declares)
        out.push(declares[name]);
    else
        out = out.concat(nodelist.filter(not_id).map(to_sql(format_set)).flatten().uniq());

    // 2. Delete data
    out.push(comment('Delete data'));
    out = out.concat(nodelist.map(to_sql(format_del)).flatten().reverse());

    out.push(sql.sp_end);
    write(out.join(statement_glue));
    write(sql.grant_sp.fmt({action:'DELETE', name:name}));
    objectName = '<unset>';
}

function create_shadow()
{
    return all_nodes.uniq().map(format_shadow).join('\n');
}

function format_shadow(table)
{
    return sql.shadow.fmt({table:table});
}

function format_del(name, idfield, id) {
    return sql.del.fmt({
        table: target_prefix + name + target_suffix,
        idField: idfield,
        id: id,
        op: get_operator(id)
    }).fmt({suffix:source_suffix});
}

function format_copy(name, idfield, id) {
    return sql.insert.fmt({
        target: target_prefix + name + target_suffix,
        source: source_prefix + name + source_suffix,
        idField: idfield,
        id: id,
        op: get_operator(id)
    }).fmt({suffix:source_suffix, tmpSufix:'_' + objectName});
}

function format_copy_all(name/*, idfield, id*/) {
    return sql.insert_all.fmt({
        target: target_prefix + name + target_suffix,
        source: source_prefix + name + source_suffix
    }).fmt({suffix:source_suffix, tmpSufix:'_' + objectName});
}

function format_upd(name, idfield, id) {
    return sql.update.fmt({
        table: name,
        user: owner,
        idField: idfield,
        id: id,
        op: get_operator(id)
    }).fmt({suffix:source_suffix, tmpSufix:'_' + objectName});
}

function format_declare(/*name, idfield, id*/) {
    return sql.declare.fmt({
        'varName': arguments[2]
    });
}

function format_set(/*name, idfield, id*/) {
    return sql.set.fmt({
        'varName': arguments[2]
    });
}

function format_temp(name/*, idfield, id*/) {
    return sql.make_temp.fmt({table:name, tmpSufix:'_' + objectName});
}

function format_decl_new(name, idField, id)
{
    return sql.decl_new_id.fmt({table:name, id:id, idField:idField});
}

function format_get_new(name, idField, id)
{
    return sql.get_new_id.fmt({table:name, id:id, idField:idField});
}

function format_iterate_ids(name, idField, id)
{
    function format(sql)
    {
        return sql.fmt(fmt);
    }

    var pc = get_node(name).parents, parent;
    for (var i in pc)
        if (pc[i] instanceof Array)
            parent = pc[i][0];
    var parentId = parent.pColumn;

    var fmt = {table:name, id:id, idField:idField, parentId:parentId, tmpSufix:'_' + objectName};
    fmt.additional = '';

    if (name in update_ids)
        fmt.additional = '\n' + update_ids[name].map(format).join('');

    return sql.iterate_ids.fmt(fmt);
}

function format_update_id(name, idField, id)
{
    return sql.update_id.fmt({
        table:target_prefix + name + target_suffix,
        idField: idField,
        id: id,
        op: get_operator(id)
    }).fmt({tmpSufix:'_' + objectName});
}

function format_update_id_all(name, idField, id)
{
    return sql.update_id_all.fmt({
        table:target_prefix + name + target_suffix,
        idField: idField,
        id: id,
        op: get_operator(id)
    }).fmt({tmpSufix:'_' + objectName});
}

function format_drop(name/*, idField, id*/)
{
    return sql.drop_table.fmt({
        table: source_prefix + name + source_suffix
    }).fmt({tmpSufix:'_' + objectName});
}

function prepare_sql_struct(struct, callback) {
    var sqls = [];
    for (var i = 0; i < struct.uniqs.length; ++i)
        sqls.push(callback(struct.name, struct.uniqs[i], struct.uniqs[i]));

    return sqls;
}

function prepare_sql(node, callback) {
    var id = get_id(node.name);
    var sqls = [];

    if (tee_defined(id))
        for (var i = 0; i < get_tee(id).length; ++i)
            sqls.push(callback(node.name, id, get_tee(id)[i]));

    else
        sqls.push(callback(node.name, id, id));

    return sqls;
}

function to_sql_struct(fmt_fn) {
    return function(node) {
        return prepare_sql_struct(node, fmt_fn);
    };
}

function to_sql(fmt_fn) {
    return function(node) {
        return prepare_sql(node, fmt_fn);
    };
}
