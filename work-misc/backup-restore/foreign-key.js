var fks = {};
var ids = {};
var tees = {};

/**
 *
 * @param {String} pt
 * @param {String} pc
 * @param {String} ft
 * @param {String} fc
 */
function ForeingKey(pt, pc, ft, fc){
    this.pTable = pt;
    this.pColumn = pc;
    this.fTable = ft;
    this.fColumn = fc || pc;
}

ForeingKey.prototype.toString = function(){
    return [this.pTable, this.pColumn, '->', this.fTable, this.fColumn].join(' ');
}

function fk(pt, pc, ft, fc){
    var ln = new ForeingKey(pt, pc, ft, fc);
    var desc = ln.toString()
    if (!(desc in fks)) 
        fks[desc] = ln;
    
    if (ln.fTable != undefined) {
        add_id(ln.fTable, ln.fColumn);
        
        if (ln.pColumn == ln.fColumn && !(ln.pTable in ids)) {
            add_id(ln.pTable, ln.pColumn);
        }
        
        // Add a tee (when same id is referenced by different names)
        if (ln.pColumn != ln.fColumn) {
            if (!(ln.fColumn in tees)) 
                tees[ln.fColumn] = [];
            
            tees[ln.fColumn].push(ln.pColumn);
        }
        
    }
    return fks[desc];
}

function add_id(table, id){
    if (!(table in ids)) 
        ids[table] = id;
    else 
        if (ids[table] != id) 
            throw new Error(['PK', ids[table], 'for table ', table, 'already exists. Trying to add ', id].join(' '))
}
