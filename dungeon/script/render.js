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

            function getImageId(n0, n1, n2, n3) {
                return parseInt(('#' == n3 ? '1' : '0') + ('#' == n2 ? '1' : '0') + ('#' == n1 ? '1' : '0') + ('#' == n0 ? '1' : '0'), 2);
            }

            var i0 = getImageId(n1, n0, n7, c);
            var i1 = getImageId(n2, n1, c, n3);
            var i2 = getImageId(n3, c, n5, n4);
            var i3 = getImageId(c, n7, n6, n5);

            var cls = (c == "#" ? 'w' : 'v'); // wall or void

            if(i == 0){
            row1.push('<td><div class="');
            row1.push('t'+ i0 + ' ' + i + '-' + j);
            row1.push('"><div></td>');
            }

            row1.push('<td><div class="');
            row1.push('t' + i1);
            row1.push('"><div></td>');

            if(j == 0){
            row2.push('<td><div class="');
            row2.push('t' + i3);
            row2.push('"><div></td>');
            }

            row2.push('<td><div class="');
            row2.push('t' + i2);
            row2.push('"><div></td>');
        }

        text.push('<tr>');
if(i==0){
        text.push(row1.join(''));
        text.push('</tr><tr>');
}
        text.push(row2.join(''));

        text.push('</tr>');
    }

    text.push('</table');
    out.innerHTML = text.join('');
}
catch (ex) {
    console.log(ex);
}
