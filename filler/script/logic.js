(function() {
    var colors = [
        'rgb(227, 38, 54)'    // red
        ,'rgb(40, 205,  20)'  // green
        ,'rgb(48, 77, 200)'   // blue
        ,'rgb(105, 229, 235)' // cyan
        ,'rgb(205, 0, 225)'   // magnra
        ,'rgb(180, 180, 180)' // gray
        ,'rgb(255, 215, 0)'   // yellow
    ];

    Cluster = function( i, j, color )
    {
        this.color = color;
        this.points = [[i,j]];
        this.neighbors = [];
    };

    var C = Cluster.prototype;
    C.addAdj = function( cluster )
    {
        function elEqClust( i ) {
            return i == cluster;
        }

        if( !this.neighbors.some(elEqClust) )
        {
            this.neighbors.add(cluster);
            cluster.addAdj(this);
        }
    };

    C.add = function( i, j )
    {
        var point = [i,j];

        function elEqPt( el ) {
            return el == point;
        }

        if( !this.points.some(elEqPt) )
            this.points.push(point);
    };

    var prob = 0.4;

    function findNeighbors( i, j )
    {
        var iu = i + 1;
        var id = i - 1;
        var jl = j - 1 + i % 2;
        var jr = j + i % 2;

        var rval = {clusters:[], points:[]};

        if( iu < this.my )
        {
            if( jl >= 0 ) {
                if( this.tiles[iu][jl] instanceof Cluster )
                    rval.clusters.push(this.tiles[iu][jl]);
                else
                    rval.points.push([iu, jl, this.tiles[iu][jl]]);
            }
            if( jr < this.mx ) {
                if( this.tiles[iu][jr] instanceof Cluster )
                    rval.clusters.push(this.tiles[iu][jr]);
                else
                    rval.points.push([iu, jr, this.tiles[iu][jr]]);
            }
        }
        if( id >= 0 )
        {
            if( jl >= 0 ) {
                if( this.tiles[id][jl] instanceof Cluster )
                    rval.clusters.push(this.tiles[id][jl]);
                else
                    rval.points.push([id, jl, this.tiles[id][jl]]);
            }
            if( jr < this.mx ) {
                if( this.tiles[id][jr] instanceof Cluster )
                    rval.clusters.push(this.tiles[id][jr]);
                else
                    rval.points.push([id, jr, this.tiles[id][jr]]);
            }
        }

        console.log(['Point ', i, ', ', j,' has ', rval.points.length, ' neighbor points and ', rval.clusters.length, ' ajacent clustres.'].join(''));

        return rval;
    }

    function processCell( i, j )
    {
        var colr = this.tiles[i][j];
        var adj = findNeighbors.call(this, i, j);

        var idx, cluster;

        for( idx in adj.clusters )
        {
            cluster = adj.clusters[idx];
            if( cluster.color == colr ) {
                this.tiles[i][j] = cluster;
                break;
            }
        }

        if( colr == this.tiles[i][j] ) {
            this.tiles[i][j] = new Cluster(colr);
            for( idx in adj.clusters )
            {
                if( cluster instanceof Cluster ) {
                    cluster = adj.clusters[idx];
                    cluster.addAdj(this.tiles[i][j]);
                }
            }
        }

        for( idx in adj.points )
        {
            var point = adj.points[idx];
            if( typeof point[0] == 'number' && typeof point[1] == 'number' )
                processCell.call(this, point[0], point[1]);
        }
    }

    function findAdjacent( i, j, cluster )
    {
        if( !(cluster instanceof Cluster) )
            throw new Error('findAdjacent can only operate on clusters');

        var iu = i + 1;
        var id = i - 1;
        var jl = j + i % 2-1;
        var jr = j + i % 2;

        var adj = [];
        var it = this;

        function addAdj( i, j )
        {
            var cell = it.tiles[i][j];
            console.log('cell',i,j,cell, 'adj', adj);
            if( cell instanceof Cluster && cell != cluster )
            {
                console.log('push cluster ar', i, j);
                if(!cell.processed )
                adj.push(cell);
            }
            else if( cell == cluster.color )
            {
                console.log('hit', i, j);
                cluster.add(i, j);
                it.tiles[i][j] = cluster;
            }
            else{
                console.log('last case', i, j, '->', cell);
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

        cluster.processed = true;

        return adj;
    }
    // TODO: Hide me
    createCluster=function( i, j )
    {
        var cluster;
        console.log(i, j);
        if( typeof this.tiles[i][j] == 'number' )
            cluster = this.tiles[i][j] = new Cluster(i, j, this.tiles[i][j]);
        else
            throw new Error(['Point (', i, ', ', j,') is already in cluster'].join(''));

        // find all adjacencies of same color and assign them to cluster
        // all adjacencies of this cluster must be converted to clustres themselves
        // or marked as neighbors

        var adj = findAdjacent.call(this, i, j, cluster);

        for( var idx in adj )
        {
            var neighbor = adj[idx];
            if( neighbor instanceof Cluster )
            {
                if( neighbor.color == cluster.color )
                    throw new Error('Requested to adjacent two same colored clusters');

                cluster.addAdj(neighbor);
            }
            else if( neighbor instanceof Array )
            {
                console.log('Creating cluster at', neighbor[0], neighbor[1], '->', this.tiles[neighbor[0]][ neighbor[1]])
                createCluster.call(this, neighbor[0], neighbor[1]);
            }
        }
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

//        createCluster.call(this, 0, 0);
    };

    var L = FillerLogic.prototype;

    L.getColor = function( i, j )
    {
        return colors[(this.tiles[i][j] instanceof Cluster) ? this.tiles[i][j].color:this.tiles[i][j]];
    };

    L.getColors = function()
    {
        return colors.clone();
    };
})();