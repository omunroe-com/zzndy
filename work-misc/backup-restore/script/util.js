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

function apply_where(str)
{
    var table = str.match(/FROM (\w+) WHERE/);
    var rval = str;
    if(table && table[1])
        rval = str.fmt({where: has_where(table[1]) ? ' AND (' + get_where(table[1]).join(' AND ') + ')' : ''});

    return rval;
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

var buffer = [];
var log = false;

function print()
{
    var i=-1, n = arguments.length;
    while(++i<n)
    {
        if(arguments[i] instanceof Array){
            print.apply(null, arguments[i]);
            continue;
        }
        
        buffer.push(arguments[i]);
    }
}

function flush()
{
    var text = buffer.join(statement_glue);
    write(text);
    buffer = [];
}


function write(text) {
    document.write('<textarea class="sql" name="out" rows="30" cols="144">' + text + '</textarea>');
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
    print('--\n'
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
    relations = [];
    clear_fk_setup();
}