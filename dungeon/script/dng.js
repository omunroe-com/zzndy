try
{
    var regionId = 0

    function Region( x, y, w, h )
    {
        this.x = x;
        this.y = y;
        this.w = w;
        this.h = h;
        this.tries = 0;
        this.id = ++regionId;
    }

    Region.prototype.toString = function()
    {
        return ['#', this.id, ' (', this.x, ', ', this.y, ') ', this.w, 'x', this.h].join('');
    };

    var node_id = 0;

    /**
     * Initialize BSP tree node.
     * @param {Region} r - region
     */
    function Node( r )
    {
        this.id = ++node_id;
        this.level = parent && parent.level || 1;
        this.l = null;
        this.r = null;
        this.region = r;
    }

    Node.prototype.vsplit = function(mw)
    {
        if(this.l != null || this.r != null)
        throw new Error("Cannot split node #"+this.id+".");

        var wall = divide(this.r.w, .1, mw);

        if( !isNaN(wall) )
        {
            r1 = new Region(this.r.x, this.r.y, wall, this.r.h);
            r2 = new Region(this.r.x + wall + 1, this.r.y, this.r.w - wall - 1, this.r.h);

            if( r1.w >= mw && r2.w >= mw )
            {
                this.r = new Node(r1);
                this.l = new Node(r2);
            }
        }

        return this;
    };

    /**
     * Create a dungeon
     * @param {Number} w  - dungeon width
     * @param {Number} h  - ungeon height
     * @param {Number} mw - minimal room width or height (default 4)
     * @param {Number} ma - maximum room aspect (default 2)
     * @param {Number} p  - passage width (default 2)
     * @param {Number} u  - dungeon uniformity (default .13)
     */
    function makeDng( w, h, mw, ma, p, u )
    {
        // Check arguments
        if( w === undefined )
            throw new Error("Width must be defined.");
        if( h === undefined )
            throw new Error("Height must be defined.");
        if( mw === undefined )
            mw = 4;
        if( ma === undefined )
            ma = 2;
        if( p === undefined )
            p = 2;
        if( u === undefined || u < 0 || u > 1 )
            u = .13;

        // Make fully filled dungeon stub
        var dng = [];
        var i = -1, j;
        while( ++i < h )
        {
            dng[i] = [];
            j = -1;
            while( ++j < w )
                dng[i][j] = true;
        }

        var shrinkPos = .35;
        var regions = [new Region(1, 1, w - 2, h - 2)];
        var r, r1, r2;
        while( r = regions.pop() )
        {
            var len = regions.length;
            if( (r.w > r.h ? 1 : 2.2) * Math.random() < shrinkPos && r.w > mw )
            {
                --r.w;
                ++r.x;
            }
            else if( (r.w > r.h ? 1 : 2.2) * Math.random() < shrinkPos && r.w > mw )
            {
                --r.w;
            }

            if( (r.w < r.h ? 1 : 2.5) * Math.random() < shrinkPos && r.h > mw )
            {
                --r.h;
                ++r.y;
            }
            else if( (r.w < r.h ? 1 : 2.5) * Math.random() < shrinkPos && r.h > mw )
            {
                --r.h;
            }

            if( Math.random() < .5 )
            {
                wall = divide(r.w, .1, mw);

                if( !isNaN(wall) )
                {
                    r1 = new Region(r.x, r.y, wall, r.h);
                    r2 = new Region(r.x + wall + 1, r.y, r.w - wall - 1, r.h);

                    if( r1.w >= mw && r2.w >= mw )
                    {
                        regions.push(r1);
                        regions.push(r2);
                    }
                }
            }
            else
            {
                wall = divide(r.h, .13, mw);
                if( !isNaN(wall) )
                {
                    r1 = new Region(r.x, r.y, r.w, wall);
                    r2 = new Region(r.x, r.y + wall + 1, r.w, r.h - wall - 1);

                    if( r1.h >= mw && r2.h >= mw )
                    {
                        regions.push(r1);
                        regions.push(r2);
                    }
                }
            }

            if( len == regions.length )
            {
                if( r.tries++ < 3 )
                    regions.push(r);
                else
                {
                    i = -1;
                    while( ++i < r.h ) {
                        j = -1;
                        while( ++j < r.w )
                            dng[r.y + i][r.x + j] = !dng[r.y + i][r.x + j];
                    }
                }

            }


        }

        return dng;
    }

    function divide( width, disp, min )
    {
        var r = width * disp;
        var m, t = 0;
        do
        {
            m = Math.floor(width / 2 - r + Math.random() * (r * 2 + 1));
            if( ++t > 10 )
            {
                m = NaN;
                break;
            }
        }
        while( m < min || m >= width - min );

        return m;
    }
}
catch( ex )
{
    console.log(ex);
}