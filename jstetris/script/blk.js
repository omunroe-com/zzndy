/**
 * @class Block
 * Hold info on tetris block, perform rotation and other actions on it.
 *
 * @param {Array} blocks
 * @param {Boolean} fixed  - if true block will not rotate (default=false)
 * @constructor
 */
function Blk( blocks, fixed )
{
    this.body = blocks.clone();
    this.fixed = !!fixed;
}

var B = Blk.prototype, n, m;
n = m = Blk.height = Blk.width = 5;

/**
 * Private method of Block - rotate block in a given direction
 * @member Blk
 * @param {Boolean} clockwise - if true rotate clockwise, otherwise counter clockwise :)
 * @return {BlockDiff} change in current block's blocks
 */
function rotate( clockwise )
{
    if( this.fixed ) return new BlockDiff();
    var body = [], b = this.body, diff = new BlockDiff();
    var i = -1;
    while( ++i < n )
    {
        var row = [];
        var j = -1;
        while( ++j < m )
        {
            if( clockwise ) row.push(b[n - 1 - j][i]);
            else row.push(b[j][m - 1 - i]);

            var val = row[j], old = b[i][j];
            if( val != old ) (val ? diff.set : diff.unset).push([i,j]);
        }
        body.push(row);
    }
    this.body = body;
    return diff;
}

/**
 * Rotate this block left (counter clockwise)
 * @return  {BlockDiff} change in current block's blocks
 */
B.left = function()
{
    return rotate.call(this, false);
};

/**
 * Rotate this block right (clockwise)
 * @return  {BlockDiff} change in current block's blocks
 */
B.right = function()
{
    return rotate.call(this, true);
};

/**
 * @return {Number}  number of empty rows on top of the block.
 */
B.getPaddingTop = function()
{
    function nonEmpty( e )
    {
        return e;
    }

    var i = -1;
    while( ++i < n )
        if( this.body[i].some(nonEmpty) )break;

    return i;
};

/**
 * @class BlockDiff
 * Represent a change of a block (via rotation on movement)
 */
function BlockDiff()
{
    /**
     * {Array} Array of pairs [i,j] of cells switched on by operation.
     */
    this.set = [];
    /**
     * {Array} Array of pairs [i,j] of cells switched off by operation.
     */
    this.unset = [];
}
