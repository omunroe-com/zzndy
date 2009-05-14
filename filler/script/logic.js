(function() {
    var colors = [
        'rgb(227, 38, 54)'    // red
        ,'rgb(40, 205,  20)'  // green
        ,'rgb(48, 77, 200)'   // blue
        ,'rgb(105, 229, 235)' // cyan
        ,'rgb(205, 0, 225)'   // magneta
        ,'rgb(180, 180, 180)' // gray
        ,'rgb(255, 215, 0)'   // yellow
    ];

    var color_name = ['red', 'green', 'blue', 'cyan', 'magneta', 'gray', 'yellow'];

    var clusterId = 0;
    Cluster = function( i, j, color )
    {
        this.color = color;
        this.points = [[i,j]];
        this.neighbors = [];
        this.id = ++clusterId;
    };

    var C = Cluster.prototype;

    /**
     * Add adjacent cluster.
     * @param {Cluster} cluster  neighbor cluster
     * @return {Boolean}         true if cluster was added
     */
    C.addAdj = function( cluster )
    {
        var isNewAdjacent = this.neighbors.indexOf(cluster) == -1;

        if( isNewAdjacent )
        {
            this.neighbors.push(cluster);
            cluster.addAdj(this);
        }

        return isNewAdjacent;
    };

    /**
     * Add point to this cluster.
     * @param {Number} i  point row coordinate
     * @param {Number} j  point column coordiante
     * @return {Cluster}  this object for chaining.
     */
    C.add = function( i, j )
    {
        var point = [i,j];

        function elEqPt( el ) {
            return el[0] == point[0] && el[1] == point[1];
        }

        if( !this.points.some(elEqPt) )
        {
            this.points.push(point);
        }

        return this;
    };

    C.replace = function( oldneighbor, newneighbor )
    {
        var oldidx = this.neighbors.indexOf(oldneighbor);
        var newidx = this.neighbors.indexOf(newneighbor);

        if( newidx == -1 )
            this.neighbors[oldidx] = newneighbor;
        else
            this.neighbors.splice(oldidx, 1);
    };

    /**
     * Eat given cluster and write self onto it's place.
     * @param {Cluster} neighbor  adjacent cluster to merge with
     * @param {Array} tiles       field
     * @return {Array}            array of new neighbors
     */
    C.eat = function( neighbor, tiles )
    {
        var j = -1, m = neighbor.neighbors.length, p;
        while( ++j < m )
        {
            var other = neighbor.neighbors[j];
            other.replace(neighbor, this);
        }

        j = -1,m = neighbor.points.length;
        while( ++j < m )
        {
            p = neighbor.points[j];
            tiles[p[0]][p[1]] = this;
            this.add(p[0], p[1]);
        }

        return neighbor.neighbors;
    };

    C.toString = function()
    {
        return ['#' , this.id, ' (', color_name[this.color], ')'].join('');
    }

    var prob = 0.4;

    function findAdjacent( i, j, cluster )
    {
        if( !(cluster instanceof Cluster) )
            throw new Error('findAdjacent can only operate on clusters');

        var iu = i + 1;
        var id = i - 1;
        var jl = j + i % 2 - 1;
        var jr = j + i % 2;

        var adj = [];
        var it = this;

        function addAdj( i, j )
        {
            var cell = it.tiles[i][j];

            if( cell instanceof Cluster && cell != cluster )
            {
                adj.push(cell);
            }
            else if( cell == cluster.color )
            {
                cluster.add(i, j);
                it.tiles[i][j] = cluster;

                adj = adj.concat(findAdjacent.call(it, i, j, cluster));
            }
            else if( cell != cluster )
                {
                    adj.push([i,j]);
                }
        }

        if( iu < this.my )
        {
            if( jl >= 0 )
                addAdj(iu, jl);
            if( jr < this.mx )
                addAdj(iu, jr);
        }
        if( id >= 0 )
        {
            if( jl >= 0 )
                addAdj(id, jl);
            if( jr < this.mx )
                addAdj(id, jr);
        }

        return adj;
    }

    FillerLogic = function( mx, my )
    {
        this.mx = mx;
        this.my = my;

        this.tiles = [];
        for( var i = 0; i < my; ++i )
        {
            this.tiles[i] = [];
            for( var j = 0; j < mx; ++j )
            {
                this.tiles[i][j]
                        = (i == 0 || Math.random() > prob)
                        ? Math.floor(Math.random() * 7)
                        : (j == 0 || Math.random() > prob)
                        ? this.tiles[i - 1][j]
                        : this.tiles[i][j - 1];
            }
        }

        makeClusters.call(this);
        linkClusters.call(this);
    };

    /**
     * Transform grid of color cells into grid of clusters.
     */
    function makeClusters()
    {
        var points = [], p = [0,0];
        do
        {
            var i = p[0],j = p[1];
            if( typeof this.tiles[i][j] == 'number' ) {
                var cluster = this.tiles[i][j] = new Cluster(i, j, this.tiles[i][j]);

                // Get all points (or clusters) adjacent to this cluster
                var adj = findAdjacent.call(this, i, j, cluster);
                var neighbor;

                while( neighbor = adj.pop() )
                {
                    if( neighbor instanceof Array && !(this.tiles[neighbor[0]][neighbor[1]] instanceof Cluster) )
                        points.push(neighbor);
                }
            }
        }
        while( p = points.pop() );
    }

    /**
     * Provided grid of clusters link adjacent clusters.
     */
    function linkClusters()
    {
        var clusters = [], cluster = this.tiles[0][0];
        do
        {
            var k = -1, n = cluster.points.length;
            var neighbors = [];
            while( ++k < n )
            {
                var i = cluster.points[k][0], j = cluster.points[k][1];
                var adj = findAdjacent.call(this, i, j, cluster);
                neighbors = neighbors.concat(adj);
            }

            var neighbor;
            while( neighbor = neighbors.pop() )
            {
                if( neighbor.addAdj(cluster) )
                    clusters.push(neighbor);
            }
        }
        while( cluster = clusters.pop() );
    }

    var L = FillerLogic.prototype;

    L.getColor = function( i, j )
    {
        return colors[(this.tiles[i][j] instanceof Cluster) ? this.tiles[i][j].color : this.tiles[i][j]];
    };

    L.getColors = function()
    {
        return colors.clone();
    };

    L.__defineGetter__('p1cluster', function() {
        return this.tiles[0][0];
    });

    L.__defineGetter__('p2cluster', function() {
        return this.tiles[this.my - 1][this.mx - 1];
    });

    L.__defineGetter__('p1color', function() {
        return this.tiles[0][0].color;
    });

    L.__defineGetter__('p2color', function() {
        return this.tiles[this.my - 1][this.mx - 1].color;
    });

    L.__defineGetter__('p1share', function() {
        return (100 * this.tiles[0][0].points.length / (this.mx * this.my));
    });

    L.__defineGetter__('p2share', function() {
        return (100 * this.tiles[this.my - 1][this.mx - 1].points.length / (this.mx * this.my));
    });

    L.setColor = function( color, clust )
    {
        // Check errors
        if( color == this.p2color )
            throw new Error('Cannot change color to opponents color.');

        if( color == this.p1color )
            throw new Error('Must change color.');

        var cluster = clust || this.p1cluster;
        var other = clust == this.p2cluster ? this.p1cluster : this.p2cluster;
        cluster.color = color;

        // Get points to redraw
        var toredraw = cluster.points.clone();

        // Cluster that will become adjacent after merge
        var newneighbors = [];
        // Merged (eaten) clusters
        var merged = [];

        var neighbor;
        var i = -1;
        while( ++i < cluster.neighbors.length )
        {
            neighbor = cluster.neighbors[i];
            if( neighbor.color == cluster.color )
            {
                neighbor = cluster.neighbors.splice(i--, 1)[0];
                merged.push(neighbor);

                newneighbors = newneighbors.concat(cluster.eat(neighbor, this.tiles));
            }
        }

        // merge eaten neighbor clusters
        var newone;
        i = -1;
        while( ++i < newneighbors.length )
        {
            newone = newneighbors[i];

            if( newone != cluster && merged.indexOf(newone) == -1 )
                cluster.addAdj(newone);
        }

        // Find surrounded clusters
        i = -1;
        while( ++i < cluster.neighbors.length )
        {
            neighbor = cluster.neighbors[i];
            if( neighbor.neighbors.length == 1 && neighbor != other )
            {
                neighbor = cluster.neighbors.splice(i--, 1)[0];
                merged.push(neighbor);

                var j = -1, m = neighbor.points.length;
                while( ++j < m )
                {
                    var p = neighbor.points[j];

                    function isPoint( e )
                    {
                        return e[0] == p[0] && e[1] == p[1];
                    }

                    if( !toredraw.some(isPoint) )
                        toredraw.push(p);
                }

                cluster.eat(neighbor, this.tiles);
            }
        }

        return toredraw;
    };

    L.moveP2 = function()
    {
        var cluster = this.p2cluster;

        var color = 0;
        while( color == cluster.color || color == this.p1color )
            ++color;

        var variants = evaluateMoves.call(this, cluster, [this.p1color], 1, []);
        var max = 0;
        for( var col in variants.colors ) if( typeof variants.colors[col] != 'function' )
        {
            var newcolor = variants.colors[col];
            var info = variants.results[newcolor];
            if( info.score > max ) {
                max = info.score;
                color = newcolor;
            }
        }

        return this.setColor(color, cluster);
    };

    function evaluateMoves( cluster, invalidColors, recursions, ignore )
    {
        var results = {};
        var colors = [];

        if( ignore === undefined )
            ignore = [];

        var i = -1, n = cluster.neighbors.length;
        while( ++i < n )
        {
            var neighbor = cluster.neighbors[i];
            var otherColor = neighbor.color;

            if( ignore.indexOf(neighbor) == -1 && invalidColors.indexOf(otherColor) == -1 )
            {
                if( colors.indexOf(otherColor) == -1 )
                {
                    colors.push(otherColor);

                    results[otherColor] = {
                        score:0 ,
                        neighbors:[] ,
                        results :{},
                        colors:[]
                    };
                }

                var j = -1, m = neighbor.points.length;
                while( ++j < m )
                {
                    var p = neighbor.points[j];
                    results[otherColor].score += Math.min(this.mx - p[0] - 1, p[0]) + Math.min(this.my - p[1] - 1, p[1]);
                }

                results[otherColor].score += neighbor.points.length;
                results[otherColor].neighbors = results[otherColor].neighbors.concat(neighbor.neighbors);
            }
        }

        return {results:results, colors:colors};
    }
})
        ();