try
{
    function Region( x, y, w, h )
    {
        this.x = x;
        this.y = y;
        this.w = w;
        this.h = h;
        this.tries = 0;
    }

    function makeDng( h, w )
    {
        var dng = [];
        var i = -1, j;
        while( ++i < h )
        {
            dng[i] = [];
            j = -1;
            while( ++j < w )
            {
                var val = i == 0 || j == 0 || i == h - 1 || j == w - 1;
                dng[i][j] = val;
            }
        }

        var mw = 4, mh = 4;
        var regions = [new Region(1, 1, w - 2, h - 2)];
        var r, r1, r2;
        while( r = regions.pop() )
        {
            var len = regions.length;
            if( Math.random() < .5 )
            {
                wall = r.x + divide(r.w, .13, mw);
                if( wall != null )
                {
                    i = -1;
                    while( ++i < r.h )
                    {
                        console.log(r.y, i, r.y + i);
                        dng[r.y + i][wall] = !dng[r.y + i][wall];
                    }

                    r1 = new Region(r.x, r.y, wall  - r.x, r.h);
                    r2 = new Region(wall + 1, r.y, r.w - (wall - 1 - r.x), r.h);

                    if( r1.w > mw && r2.w > mw )
                    {
                        regions.push(r1);
                        //regions.push(r2);
                    }
                }

            }
            else
            {
                wall = r.y + divide(r.h, .13, mh);
                if( wall != null )
                {
                    j = -1;
                    while( ++j < r.w )
                    {
                        dng[wall][r.x + j] = !dng[wall][r.x + j];
                    }
                    r1 = new Region(r.x, r.y, r.w, wall  - r.y);
                    r2 = new Region(r.x, wall + 1, r.w, r.h - (wall - 1 - r.y));

                    if( r1.h > mh && r2.h > mh )
                    {
                        regions.push(r1);
                        //regions.push(r2);
                    }
                }
            }

            if( len == regions.length )
            {
                if( r.tries++ < 3 )
                    regions.push(r);
                else
                    console.log('saving', r);
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
                m = null;
                break;
            }
        }
        while( m < min || m > width - min );

        return m;
    }
}
catch( ex )
{
    console.log(ex);
}