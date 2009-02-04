/**
 * $Id$
 */

var sql = {
    'update': "\t\tUPDATE <Table> SET MODIFIED_BY='<User>'\n\t\t\tWHERE <IdField><Op><Id>;",
    'insert': "\t\tINSERT INTO <Target>\n\t\t\tSELECT * FROM <Source>\n\t\t\tWHERE <IdField><Op><Id>;",
    'del': "\t\tDELETE FROM <Table> WHERE\n\t\t\t<IdField><Op><Id>;",
    'declare': '\t\tDECLARE @<Var> DECIMAL(12,0)',
    'set':'\t\tSET @<Var> = ?',
    'sp_head':
            "IF EXISTS (SELECT name FROM sys.objects WHERE type='P' AND name='SP_<Action>_<Name>')\n"
                    + '\tDROP PROCEDURE [SP_<Action>_<Name>]\nGO\n'
                    + 'CREATE PROCEDURE SP_<Action>_<Name> @<Id> DECIMAL (12,0)\nAS\nBEGIN\n'
                    + '\tSET NOCOUNT ON',
    'shadow': "EXEC SP_CREATE_SHADOW_TABLE '<Table>';",
    'sp_start': '\tBEGIN TRANSACTION\n\tBEGIN TRY',
    'sp_end':
            '\tEND TRY\n\n'
                    + '\tBEGIN CATCH\n'
                    + '\t\tROLLBACK TRANSACTION\n'
                    + '\t\tRETURN 1\n'
                    + '\tEND CATCH\n\n'
                    + '\tCOMMIT TRANSACTION\n'
                    + 'END\n'
                    + 'GO',
    'check_tag':"\tIF (SELECT MODIFIED_BY FROM <Tagged> WHERE <IdField><Op><Id>) != '<Owner>'\n"
            + '\t\tRETURN -- No backup needed',
    'update_tag':"\t\tUPDATE <Tagged> SET MODIFIED_BY='<Owner>' WHERE <IdField><Op><Id>"

};

var declares = {};

declares['FIELD'] = '\t\tSELECT @TAX_NODE_ID = TAX_NODE_ID FROM FIELD_ADDITIONAL WHERE FIE_ID = @FIE_ID\n\n'
        + '\t\tSELECT @INV_ASS_ORIGINAL_ID = INV_ASS_ORIGINAL_ID FROM FIELD_ADDITIONAL WHERE FIE_ID = @FIE_ID\n\n'
        + '\t\tSELECT @INV_ASS_REDIST_ID = INV_ASS_REDIST_ID FROM FIELD_ADDITIONAL WHERE FIE_ID = @FIE_ID';

declares['BLOCK'] = '\t\tSELECT @PAR_ID = PAR_ID FROM BLOCK_HEADER WHERE GA_ID = @GA_ID';

declares['COMPLEX'] = '\t\tSELECT @INV_ASS_ID = INV_ASS_ID FROM FIELD_COMPLEX WHERE FIELD_COMPLEX_ID = @FIELD_COMPLEX_ID';

declares['GLOBALS'] = '';

var statement_glue = '\n\n';
var section_glue = '\n\n\n';

// sql generation setup
var source_suffix = '';
var target_suffix = '';
var owner = '';

var all_nodes = [];

/**
 * Setup environment for generation of backup SQL.
 */
function setup_backup() {
    source_suffix = '';
    target_suffix = '_SHADOW';
    owner = 'USER';
}

/**
 * Setup environment for generation of restore SQL.
 */
function setup_restore() {
    source_suffix = '_SHADOW';
    target_suffix = '';
    owner = 'IHS';
}

function write() {
    var target = document.getElementById('out');
    var text = [].join.call(arguments, section_glue);
    if (target)
        target.innerHTML += section_glue + text;
    else
        document.write('<textarea class="sql" name="out" id="out" rows="30" cols="144">' + text + '</textarea>');
}

function write_ahead() {
    var target = document.getElementById('out');
    var text = [].join.call(arguments, section_glue);
    if (target)
        target.innerHTML += text + section_glue + target.innerHTML;
    else
        write(text);
}

/**
 * @param {Node} node
 */
function istagged(node) {
    return tagged.indexOf(node.name) != -1;
}

function comment(msg)
{
    return '\t\t--\n\t\t-- ' + msg + '\n\t\t--';
}

function get_name(node) {
    return node.name;
}

function write_header()
{
    write('--\n'
            + '-- Default AssetBank database name is ABDB.\n'
            + '--\n'
            + 'USE ABDB\n'
            + 'GO');
}

function make_backup_sql(name, id, tagged) {
    function not_id(node) {
        return get_id(node.name) != id;
    }

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
    out = out.concat(nodelist.filter(not_id).map(make_declare_sql).flatten().filter(is_valid_sql).uniq());

    out.push(comment('Set id variables'));
    if (name in declares)
        out.push(declares[name])
    else
        out = out.concat(nodelist.filter(not_id).map(make_set_sql).flatten().uniq());

    // 3. Delete any old backup information;
    out.push(comment('Delete old backup'));
    out = out.concat(nodelist.map(make_del_sql).flatten().reverse());

    // 4. Copy data to shadow tables;
    out.push(comment('Backup data'));
    out = out.concat(nodelist.map(make_copy_sql).flatten());

    // 5. Update tag.
    out.push(comment('Update tag'));
    out.push(sql.update_tag.fmt({
        tagged: tagged,
        idField: id,
        id: id,
        owner: owner,
        op: get_operator(id)}));

    out.push(sql.sp_end);
    write(out.join(statement_glue));
}

function is_valid_sql(text)
{
    return !text.match(/@\(/);
}

function write_shadow_generation()
{
    var out = [];
    out.push(comment('Create tables to backup field domain object.'));
    out = out.concat(all_nodes.map(make_shadow_sql));

    write(out.join(statement_glue));
}

function make_shadow_sql(table)
{
    return "EXEC SP_CREATE_SHADOW_TABLE '<Table>'".fmt({table:table});
}

function make_restore_sql(name, id, tagged) {
    function not_id(node) {
        return get_id(node.name) != id;
    }

    var out = [];

    out.push(sql.sp_head.fmt({action:'RESTORE', name:name, id:id}));
    out.push(sql.sp_start);

    setup_restore();

    var nodelist = rectify_nodes();

    // Restore routine:
    // 1. Declare variables;
    out.push(comment('Declare id variables'));
    out = out.concat(nodelist.filter(not_id).map(make_declare_sql).flatten().filter(is_valid_sql).uniq());

    out.push(comment('Set id variables'));
    if (name in declares)
        out.push(declares[name])
    else
        out = out.concat(nodelist.filter(not_id).map(make_set_sql).flatten().filter(is_valid_sql).uniq());

    // 2. Delete user version;
    out.push(comment('Delete user version'));
    out = out.concat(nodelist.map(make_del_sql).flatten().reverse());

    // 3. Copy data from shadow tables;
    out.push(comment('Restore data'));
    out = out.concat(nodelist.map(make_copy_sql).flatten());

    // 4. update tags;
    out.push(comment('Update tag'));
    out.push(sql.update_tag.fmt({
        tagged: tagged,
        idField: id,
        id: id,
        owner: owner,
        op: get_operator(id)}));

    // 5. Delete backup.
    setup_backup();
    out.push(comment('Delete backup'));
    out = out.concat(nodelist.map(make_del_sql).flatten().uniq().reverse());

    out.push(sql.sp_end);
    write(out.join(statement_glue));
}

function make_delete_sql(name, id, tagged) {
    function not_id(node) {
        return get_id(node.name) != id;
    }

    var out = [];

    out.push(sql.sp_head.fmt({action:'DELETE', name:name, id:id}));
    out.push(sql.sp_start);

    setup_restore();

    var nodelist = rectify_nodes();

    out.push(comment('Declare id variables'));
    out = out.concat(nodelist.filter(not_id).map(make_declare_sql).flatten().filter(is_valid_sql).uniq());

    out.push(comment('Set id variables'));
    if (name in declares)
        out.push(declares[name])
    else
        out = out.concat(nodelist.filter(not_id).map(make_set_sql).flatten().uniq());

    // 2. Delete data
    out.push(comment('Delete data'));
    out = out.concat(nodelist.map(make_del_sql).flatten().reverse());

    out.push(sql.sp_end);
    write(out.join(statement_glue));
}

function get_operator(id)
{
    return id[0] == '(' ? ' IN ' : ' = @';
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
        table: name + target_suffix,
        idField: idfield,
        id: id,
        op: get_operator(id)
    }).fmt({suffix:source_suffix});
}

function format_copy(name, idfield, id) {
    return sql.insert.fmt({
        target: name + target_suffix,
        source: name + source_suffix,
        idField: idfield,
        id: id,
        op: get_operator(id)
    }).fmt({suffix:source_suffix});
}

function format_upd(name, idfield, id) {
    return sql.update.fmt({
        table: name,
        user: owner,
        idField: idfield,
        id: id,
        op: get_operator(id)
    }).fmt({suffix:source_suffix});
}

function format_declare(name, idfield, id) {
    return sql.declare.fmt({
        'var': id
    });
}

function format_set(name, idfield, id) {
    return sql.set.fmt({
        'var': id
    });
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

function make_declare_sql(node) {
    return prepare_sql(node, format_declare);
}

function make_set_sql(node) {
    return prepare_sql(node, format_set);
}

function make_del_sql(node) {
    return prepare_sql(node, format_del);
}

function make_copy_sql(node) {
    return prepare_sql(node, format_copy);
}

function make_upd_sql(node) {
    return prepare_sql(node, format_upd);
}

function clear_setup()
{
    nodes = {};
    clear_fk_setup();
}