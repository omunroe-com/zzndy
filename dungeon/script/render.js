/**
* Setup and render dungeon.
*/

try {
    var out = document.getElementById('dng');

    var map = out.textContent.split('\n');
    var w = map[0].length;
    var h = map.length;

    var text = ['<table class="dungeon">'];

    for (var i = 0; i < h; ++i) {

        var row1 = [];
        var row2 = [];
        for (var j = 0; j < w; ++j) {
            var c = map[i][j];
            // We have six images of a corner of each square
            // # - corner in place, X - same state square.
            //
            // -- -X X- XX -X XX     neighbours: 012
            // -# -# -# -# X# X#                 7#3
            //                                   654

            var n0 = (i > 0 && j > 0) ? map[i - 1][j - 1] : c;
            var n1 = (i > 0) ? map[i - 1][j] : c;
            var n2 = (i > 0 && j < w - 1) ? map[i - 1][j + 1] : c;
            var n3 = (j < w - 1) ? map[i][j + 1] : c;
            var n4 = (i < h - 1 && j < w - 1) ? map[i + 1][j + 1] : c;
            var n5 = (i < h - 1) ? map[i + 1][j] : c;
            var n6 = (i < h - 1 && j > 0) ? map[i + 1][j - 1] : c;
            var n7 = (j > 0) ? map[i][j - 1] : c;

            function getImageId(c, n0, n1, n2, n3) {
                return parseInt((c == n3 ? '1' : '0') + (c == n2 ? '1' : '0') + (c == n1 ? '1' : '0') + (c == n0 ? '1' : '0'), 2);
            }

            var i0 = getImageId(c, n1, n7, n5, n3);

            var cls = (c == "#" ? 'w' : 'v'); // wall or void

            row1.push('<td><div class="');
            row1.push(cls+ i0 + ' ');
            row1.push('"><div></td>');
        }

        text.push('<tr>');
        text.push(row1.join(''));
        text.push('</tr>');
    }

    text.push('</table');
    out.innerHTML = text.join('');
}
catch (ex) {
    console.log(ex);
}
