var sql = {

    'update': "UPDATE <Table> SET MODIFIED_BY='<User>' WHERE <IdField>=@<Id>",
    'insert': "INSERT INTO <Target>\n\tSELECT * FROM <Source>\n\tWHERE <IdField>=@<Id>",
    'del': "DELETE FROM <Table> WHERE <IdField>=@<Id>",
    'declare': 'DECLARE @<Var> DECIMAL(12,0)\nSET @<Var> = ?'

};

var statement_glue = '\n\n';
var section_glue = '\n\n\n';

// sql generation setup
var source_suffix = '';
var target_suffix = '';
var owner = ''

/**
 * Setup environment for generation of restore SQL.
 */
function setup_backup(){
    source_suffix = '';
    target_suffix = '_SHADOW';
    owner = 'USER';
}

/**
 * Setup environment for generation of restore SQL.
 */
function setup_restore(){
    source_suffix = '_SHADOW';
    target_suffix = '';
    owner = 'IHS';
}

function write(){
    var target = document.getElementById('out');
    var text = [].join.call(arguments, section_glue);
    if (target) 
        target.innerHTML += section_glue + text;
    else 
        document.write('<textarea class="sql" name="out" id="out" rows="30" cols="144">' + text + '</textarea>');
}

function istagged(node){
    return tagged.indexOf(node.name) != -1;
}

function make_backup_sql(){
    setup_backup();
    
    var nodelist = rectify_nodes();
    
    // backup routine:
    // 0. declare variables
    var sql0 = nodelist.map(make_declare_sql).flatten().uniq().join(statement_glue);
    
    // 1. delete old backup version
    var sql1 = nodelist.map(make_del_sql).flatten().join(statement_glue);
    
    // 2. copy data to shadow tables
    var sql2 = nodelist.map(make_copy_sql).flatten().join(statement_glue);
    
    // 3. update tags    
    var sql3 = nodelist.filter(istagged).map(make_upd_sql).flatten().join(statement_glue);
    
    write('--\n-- Backup actions\n--');
    write('BEGIN TRANSACTION', sql0, sql1, sql2, sql3, 'COMMIT TRANSACTION');
}

function make_restore_sql(){
    setup_restore();
    
    var nodelist = rectify_nodes();
    
    // restore routine:
    // 0. declare variables
    var sql0 = nodelist.map(make_declare_sql).flatten().uniq().join(statement_glue);
    
    // 1. delete user version
    var sql1 = nodelist.map(make_del_sql).flatten().join(statement_glue);
    
    // 2. copy data from shadow tables
    var sql2 = nodelist.map(make_copy_sql).flatten().join(statement_glue);
    
    // 3. update tags
    var sql3 = nodelist.filter(istagged).map(make_upd_sql).flatten().join(statement_glue);
    
    // 4. delete backup
    setup_backup();
    var sql4 = nodelist.map(make_declare_sql).flatten().uniq().join(statement_glue);
    
    write('--\n-- Restore actions\n--');
    write('BEGIN TRANSACTION', sql0, sql1, sql2, sql3, sql4, 'COMMIT TRANSACTION');
}

function format_del(name, idfield, id){
    return sql.del.fmt({
        table: name + target_suffix,
        idField: idfield,
        id: id
    })
}

function format_copy(name, idfield, id){
    return sql.insert.fmt({
        target: name + target_suffix,
        source: name + source_suffix,
        idField: idfield,
        id: id
    })
}

function format_upd(name, idfield, id){
    return sql.update.fmt({
        table: name,
        user: owner,
        idField: idfield,
        id: id
    });
}

function format_declare(name, idfield, id){
    return sql.declare.fmt({
        'var': id
    });
}

function prepare_sql(node, callback){
    var id = ids[node.name];
    var sqls = [];
    
    if (id in tees) 
        for (var i = 0; i < tees[id].length; ++i) 
            sqls.push(callback(node.name, id, tees[id][i]));
    
    else 
        sqls.push(callback(node.name, id, id));
    
    return sqls;
}

function make_declare_sql(node){
    return prepare_sql(node, format_declare);
}

function make_del_sql(node){
    return prepare_sql(node, format_del);
}

function make_copy_sql(node){
    return prepare_sql(node, format_copy);
}

function make_upd_sql(node){
    return prepare_sql(node, format_upd);
}
