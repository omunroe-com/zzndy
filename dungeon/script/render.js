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


            function getImageId(c, n0, n1, n2) {
                return parseInt((c == n0 ? '0' : '1') + (c == n1 ? '0' : '1') + (c == n2 ? '0' : '1'), 2);
            }

            var i0 = getImageId(c, n7, n0, n1);
            var i1 = getImageId(c, n1, n2, n3);
            var i2 = getImageId(c, n3, n4, n5);
            var i3 = getImageId(c, n5, n6, n7);

            var cls = (c == "#" ? 'w' : 'v'); // wall or void

            row1.push('<td><div class="');
            row1.push(cls + i0 + ' t0');
            row1.push('"><div></td>');

            row1.push('<td><div class="');
            row1.push(cls + i1 + ' t1');
            row1.push('"><div></td>');

            row2.push('<td><div class="');
            row2.push(cls + i3 + ' t3');
            row2.push('"><div></td>');

            row2.push('<td><div class="');
            row2.push(cls + i2+ ' t2');
            row2.push('"><div></td>');
        }

        text.push('<tr>');

        text.push(row1.join(''));
        text.push('</tr><tr>');
        text.push(row2.join(''));

        text.push('</tr>');
    }

    text.push('</table');
    out.innerHTML = text.join('');
}
catch (ex) {
    console.log(ex);
}
