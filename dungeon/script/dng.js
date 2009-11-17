/**
 * @class Dungeon
 */

/**
 * Create array containing dungeon map.
 *
 * @param {Number} width - dungeon width
 * @param {Number} height - dungeon height
 * @param {Number} minDim - minimal room width or height
 * @param {Number} maxAsp - maximal room aspect ratio
 */
function makeDng(width, height, minDim, maxAsp)
{
   var a = height.times(width.times(1));
   var i = rnd(height);
   var j = rnd(width);
   var room = makeRoom(minDim, minDim * 3, maxAsp);
   a[i][j] = 0;
   return a;
}

/**
 * Create room with with minimum dimension and maximum aspect.
 */
function makeRoom(min, max, asp)
{
    var w, h;
    do
    {
        w = min + rnd(max - min);
        h = min + rnd(max - min);
    }
    while(Math.max(w, h) / Math.min(w,h) > asp)
    return [w, h];
}

/**
 * Get random integer within [0,x) range.
 */
function rnd(x)
{
    return Math.floor(Math.random()*x);
}
