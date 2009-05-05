(function() {
    var colors = ['rgb(227, 38, 54)', 'rgb(40, 205,  20)', 'rgb(28, 57, 187)', 'rgb(125, 249, 255)', 'rgb(255, 0, 255)', 'rgb(200, 200, 200)', 'rgb(255, 215, 0)'];
    var prob = .5;

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
                            : this.tiles[i][j-1];
            }
        }
    };

    var L = FillerLogic.prototype;

    L.getColor = function( i, j )
    {
        return colors[this.tiles[i][j]];
    };

    L.getColors = function()
    {
        return colors.clone();
    };
})();