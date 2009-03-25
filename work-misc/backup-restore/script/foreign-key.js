(function() {
    /**
     * $Id$
     */

    var fks = {};
    var ids = {};
    var tees = {};

    /**
     * @class  ForeignKey
     * @constructor
     * @param {String} pt  primary table name
     * @param {String} pc  primary column name
     * @param {String} ft  foreign table name
     * @param {String} fc  foreign column name
     */
    ForeingKey = function(pt, pc, ft, fc) {
        this.pTable = pt;
        this.pColumn = pc;
        this.fTable = ft;
        this.fColumn = fc || pc;
    };

    ForeingKey.prototype.toString = function() {
        with ( this ) return [pTable, pColumn, '->', fTable, fColumn].join(' ');
    };

    /**
     * Add foreign key to the directory.
     * @param {String} pt  primary table name
     * @param {String} pc  primary column name
     * @param {String} ft  foreign table name
     * @param {String} fc  foreign column name
     */
    fk = function(pt, pc, ft, fc) {
        var ln = new ForeingKey(pt, pc, ft, fc);
        var desc = ln.toString();
        if (!(desc in fks))
            fks[desc] = ln;

        if (ln.fTable != undefined) {
            add_id(ln.fTable, ln.fColumn);

            if (ln.pColumn == ln.fColumn && !(ln.pTable in ids)) {
                add_id(ln.pTable, ln.pColumn);
            }

            // Add a tee (when same id is referenced by different names)
            if (ln.pColumn != ln.fColumn) {
                add_tee(ln.fColumn, ln.pColumn);
            }

        }
        return fks[desc];
    };

    add_id = function(table, id) {
        if (!(table in ids))
            ids[table] = id;
        else
            if (ids[table] != id)
                throw new Error(['PK', ids[table], 'for table ', table, 'already exists. Trying to add ', id].join(' '))
    };

    get_id = function(name)
    {
        return ids[name];
    };

    get_tee = function(id) {
        return tees[id];
    };

    add_tee = function(original, alternative) {
        if (!(original in tees))
            tees[original] = [];

        tees[original].push(alternative);
    };

    tee_defined = function(id)
    {
        return id in tees;
    };

    /**
     * Clear all directories: foreign keys, ids, tees.
     */
    clear_fk_setup = function()
    {
        fks = {};
        ids = {};
        tees = {};
    };
})();
