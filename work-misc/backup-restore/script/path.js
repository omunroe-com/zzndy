//(function() {
    var paths = {};

    /**
     * @constructor
     * @param {Boolean} destructive    specifyes if processing of this path involved removing child objects
     * @param {String} childTable      child table name
     * @param {String} mediumTable     mediator table name
     * @param {String} parentId        ID field in parent table
     * @param {String} childId         ID field in child table (optional)
     * @param {String} mediumChildId   field name in mediator table pointing to child (optional)
     * @param {String} mediumParentId  field name in mediator table pointing to parent (optional)
     */
    Path = function(destructive, childTable, mediumTable, parentId, childId, mediumChildId, mediumParentId)
    {
        this.destructive = destructive;

        this.childTable = childTable;
        this.mediumTable = mediumTable;
        this.parentId = parentId;

        this.childId = childId || childTable + '_ID';
        this.mediumChildId = mediumChildId || this.childId;
        this.mediumParentId = mediumParentId || parentId;
    };

    add_path = function(name, path)
    {
        if (!has_paths(name))
            paths[name] = [];

        return paths[name].push(path);
    };

    has_paths = function(name)
    {
        return name in paths;
    };

//})();