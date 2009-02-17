var statement_glue = '\n\n';
var section_glue = '\n\n\n';

// sql generation setup
var source_suffix = '';
var target_suffix = '';
var source_prefix = '';
var target_prefix = '';
var owner = '';
var tagField = '';

var all_nodes = [];
var update_ids = {};

function add_update_ids(name, sql)
{
    if (!(name in update_ids))
        update_ids[name] = [];
    
    update_ids[name].push(sql);
}

/**
 * Setup environment for generation of backup SQL.
 */
function setup_backup() {
    source_suffix = '';
    target_suffix = '_SHADOW';
    tagField = 'MODIFIED_BY';
    owner = 'USER';
    source_prefix = '';
    target_prefix = '';
}

/**
 * Setup environment for generation of restore SQL.
 */
function setup_restore() {
    source_suffix = '_SHADOW';
    target_suffix = '';
    tagField = 'MODIFIED_BY';
    owner = 'IHS';
    source_prefix = '';
    target_prefix = '';
}

function setup_backup_temp()
{
    source_suffix = '';
    target_suffix = '{tmpSufix}';
    tagField = 'CREATED_BY';
    owner = 'USER';
    source_prefix = '';
    target_prefix = '#';
}

function setup_restore_temp()
{
    source_suffix = '{tmpSufix}';
    target_suffix = '';
    tagField = 'CREATED_BY';
    owner = 'USER';
    source_prefix = '#';
    target_prefix = '';
}


function write() {
    var target = document.getElementById('out');
    var text = [].join.call(arguments, section_glue);
    if (target)
        target.innerHTML += section_glue + text;
    else
        document.write('<textarea class="sql" name="out" id="out" rows="30" cols="144">' + text + '</textarea>');
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
            + 'USE [ABDB]\n'
            + 'GO');
}


function is_valid_sql(text)
{
    return !text.match(/@(?:NEW_)?\(/);
}


function get_operator(id)
{
    return id[0] == '(' ? ' IN ' : ' = @';
}


function clear_setup()
{
    nodes = {};
    clear_fk_setup();
}