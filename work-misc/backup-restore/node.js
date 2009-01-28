var nodes = {};
var tagged = [];

function Node(name){
    this.name = name;
    this.rank = 0;
    
    this.parents = {};
    this.children = {};
}

Node.prototype.toString = function(){
    return [this.rank, '. ', this.name].join('');
}

/**
 *
 * @param {ForeignKey} fk
 * @param {Node} parent
 */
Node.prototype.addParent = function(fk, parent){
    if (fk.pTable != this.name) 
        throw new Error(['Foreign key', fk, 'is not starting at', this.name].join(''));
    if (fk.fTable != parent.name) 
        throw new Error(['Foreign key', fk, 'is not pointing to', parent.name].join(''));
    
    if (!(fk.fTable in this.parents)) 
        this.parents[fk.fTable] = [];
    
    this.parents[fk.fTable].push(fk);
    
    if (!(fk.pTable in parent.children)) 
        parent.children[fk.pTable] = [];
    
    parent.children[fk.pTable].push(fk);
    
    this.rank = this.get_max_parent_rank() + 1;
}

Node.prototype.get_max_parent_rank = function(){
    var rank = 0;
    for (var i in this.parents) {
        for (var j in this.parents[i]) {
            var fk = this.parents[i][j];
            if (fk instanceof ForeingKey) {
                var parent = get_node(fk.fTable);
                rank = Math.max(rank, parent.rank);
            }
        }
    }
    
    return rank;
}

Node.sort = function(n1, n2){
    return n1.rank - n2.rank;
}

function node(pt, pc, ft, fc){
    var key = fk(pt, pc, ft, fc)
    var node = get_node(pt);
    
    if (ft !== undefined) {
        var parent = get_node(ft);
        node.addParent(key, parent);
    }
}

function get_node(name){
    if (!(name in nodes)) 
        nodes[name] = new Node(name);
    
    return nodes[name];
}

/**
 * @return {Array}  Array of nodes sorted by rank
 */
function rectify_nodes(){
    var nodelist = [];
    for (var i in nodes) 
        if (nodes[i] instanceof Node) 
            nodelist.push(nodes[i]);
    
    return nodelist.sort(Node.sort)
}

function add_tagged(node){
    if (tagged.indexOf(node) == -1) 
        tagged.push(node);
}

