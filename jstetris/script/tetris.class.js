/**
 * Javascript tetris v3
 * � A. Vynogradov 2007-2008
 */

var top10k = {
    cookie: 'jst-top10',
    options: 'jst-opts',
    lastNick: 'jst-last-nick',
    url: 'script/top10k.php'
};

var Key = {    // keys being tracked
    left: 37, up: 38, right: 39, down: 40,
    a: 97,  w: 119, d: 100, s: 115,
    h: 104, j: 106,  k: 107, l: 108,
    W: 87, J: 74, p:112, P:80,
    cr: 13,    space: 32, pause:19
};

var GameState = {notStarted: 0, underway: 1, paused: 2, gameOver: 3};

//// Tetris class ////////////////////////////////////////////////////////////

function Tetris( parent, params ) {
    this.parent = parent;
    this.gameState = GameState.notStarted;
    this.padWidth = 6;

    this.field = new Field(Tetris.defaults.fieldSize.width, Tetris.defaults.fieldSize.height);

    var opts = Cookie.get(top10k.options);
    if ( opts ) opts = opts.split(',');
    else opts = [];

    this.animScore = opts.indexOf('noAnimScore') == -1;

    if ( params && (params.animScore == 'off' || params.animScore == 'no') )
        this.animScore = false;

    this.beamOn = opts.indexOf('nobeam') == -1;
    if ( params && (params.beam == 'off' || params.beam == 'no') )
        this.beamOn = false;

    this.sevenSegment = !!!(params && (params.sevenSegment == 'off' || params.sevenSegment == 'no'));

    if ( params && params.box )this.gridsize = params.box;
    else this.gridsize = Tetris.defaults.box;

    if ( params && params.grid != 0 )
        this.box = Rectangle.dif(this.gridsize, new Rectangle(1, 1));
    else this.box = new Rectangle(this.gridsize);

    if ( opts.indexOf('cw') != -1 || (params && params.rd == 'cw') )this.rd = {def: Rotate.cw, alt: Rotate.ccw}
    else this.rd = {def: Rotate.ccw, alt: Rotate.cw};

    this.classic = opts.indexOf('classic') != -1;

    this.mainVp = new Viewport(
            new Point(2, 2),
            this.field.size, this.box,
            1, new Point(1, 1)
            )

    this.nextVp = new Viewport(
            new Point(2 + Math.floor(this.mainVp.size.width + .5 * this.gridsize.width), 2),
            new Rectangle(Figure.area, Figure.area), this.box,
            1, new Point(1, 1)
            );

    this.speedupInterval = 20000;  // Speed increases by 0.1 drop per second every two minutes (in standard mode)
    this.canvas = null;
    this.bag = [];
    this.reset();

    this.falling = new Timer(this.fall.detach(this), 1000 / this.speed);
    this.speedingUp = new Timer(this.speedup.detach(this), this.speedupInterval);

    this.init();
}

Tetris.prototype.resetClassicMode = function()
{
    if ( this.classic ) this.score.scoring = Score.classicScoring;
    else this.score.scoring = Score.speedyScoring;
}

Tetris.prototype.reset = function( recycling ) {
    this.speed = 0.9;
    this.next = null;
    this.bfh = null; // bonus fade handle
    this.sch = null; // score ticked handle
    this.beamAlpha = .04;

    this.score = new Score();
    this.resetClassicMode();
    this.score.onScore = this.onScore.detach(this);

    this.stats = {lines: 0, figures: 0, drops: 0, dropLevels: 0, forLines: 0, forFigures: 0, forDrops: 0};

    if ( recycling ) {
        this.field = new Field(this.field.size.width, this.field.size.height);
        this.mainVp.unshift();
    }

    this.pOk = this.field.pointOk.detach(this.field);

    this.running = {};// storage for misc volatile data
}

Tetris.defaults = {
    fieldSize: new Rectangle(9, 19),
    box: new Rectangle(20, 20),
    figindexes: Figure.figures.map(function( x, i ) {
        return i
    })
}

Tetris.prototype.init = function() {
    with ( this ) {
        animateScoreDlg = this.animateScore.detach(this)

        makeHtml();

        mainVp.canvas = nextVp.canvas = canvas;
        initConsts();
        redraw();
        var opts = [];
        if ( !this.beamOn ) opts.push('nobeam');
        if ( this.classic ) opts.push('classic');
        if ( this.rd.def == Rotate.cw ) opts.push('cw');
        if ( !this.animScore ) opts.push('noAnimScore');
        Tag.floater2start(makeFloater(), start.detach(this), opts);
    }
}

Tetris.prototype.redraw = function() {
    with ( this ) {
        canvas.clearRect(0, 0, canvas.size.width, canvas.size.height);
        mainVp.mkGrid('|').enframe();
        nextVp.mkGrid('+').enframe();
    }
}

Tetris.prototype.initConsts = function() {
    function makeSSD( scale, top, ajust, on, off ) {
        var squeeze = .2;
        var width = Figure.area * this.box.width / scale;
        var w = width * (squeeze + 1) * (1.3 - squeeze);
        var pos = new Point(this.nextVp.pos.x + this.nextVp.size.width - w * ajust - 5, (Figure.area + top + .5) * this.gridsize.height + .5 * this.box.height);
        return new SSD(pos, width, on, off);
    }

    this.highlight = 'rgba(255,255,255,0.3)';
    this.shadow = 'rgba(0,0,0,0.15)';
    this.ss = {mn: 3, mx: 300}; // score step
    this.goDelay = 70; // delay before showing gameover tables
    this.beamFade = {start: 2, end: 2.5};

    this.scoreSSD = makeSSD.call(this, 8, 0, 6, 'rgba(94, 220, 50, .9)', 'rgba(94, 220, 50, .1)');
    this.speedSSD = makeSSD.call(this, 7, 3, 3, 'rgba(247, 114, 255, .9)', 'rgba(247, 114, 255, .1)');
    this.bonusSSD = makeSSD.call(this, 9, 1.5, 6, 'rgba(247, 233, 0, .9)', 'rgba(247, 233, 0, .1)');

    this.anim1 = ['img/fav-0.ico', 'img/fav-1.ico', 'img/fav-2.ico', 'img/fav-3.ico', 'img/fav-4.ico', 'img/fav-5.ico', 'img/favicon.ico'];
    this.anim2 = ['img/fav-0.ico', 'img/fav-6.ico', 'img/fav-7.ico', 'img/fav-8.ico', 'img/fav-9.ico', 'img/fav-a.ico', 'img/fav-b.ico', 'img/fav-c.ico', 'img/favicon.ico'];

    // Preload images
    var loadImg = function( i ) {
        var img = new Image(16, 16);
        img.src = i;
    }

    var self = this;
    var preload = function() {
        self.anim1.forEach(loadImg);
        self.anim2.forEach(loadImg);
    }

    preload.fork();

    this.animFaviconMaxSpeed = 3.5;
    this.animScoreMaxSpeed = 3;
    this.bonusMaxSpeed = 2.1;

    var a = .05, b = .10;
    with ( this.box ) {
        this.highlightPath = [0, 0, width * (1 - a), height * a, width * b, height * b, width * a, height * (1 - a)];
        this.shadowPath = [width, height, width * (1 - a), height * a, width * (1 - b), height * (1 - b), width * a, height * (1 - a)];
    }
}

Tetris.prototype.start = function() {
    var beamCb = Tag.get('#opt-beams'), cwCb = Tag.get('#opt-cw'), classicCb = Tag.get('#opt-classic'), scoreCb = Tag.get('#opt-score');

    if ( beamCb ) this.beamOn = beamCb.checked;
    if ( scoreCb ) this.animScore = scoreCb.checked;
    if ( cwCb ) {
        if ( cwCb.checked )this.rd = {def: Rotate.cw, alt: Rotate.ccw};
        else this.rd = {def: Rotate.ccw, alt: Rotate.cw};
    }
    if ( classicCb )this.classic = classicCb.checked;

    this.resetClassicMode();
    if ( this.classic ) {
        this.speed = 1;
        this.updSpeed();
    }

    var opts = [];
    if ( !this.beamOn ) opts.push('nobeam');
    if ( this.classic ) opts.push('classic');
    if ( this.rd.def == Rotate.cw ) opts.push('cw');
    if ( !this.animScore ) opts.push('noAnimScore');
    Cookie.set(top10k.options, opts);

    Tag.rm('floater');

    window.addEventListener('keydown', this.onkeydown.detach(this), false);
    window.addEventListener('keypress', this.onkeypress.detach(this), false);

    window.onblur = this.pause.detach(this);
    window.onfocus = function() {
        Tag.get('#tetris-canvas').focus()
    }

    with ( this ) {
        gameState = GameState.underway;
        shiftFigure();
        speedup();
        onScore(score.points);
        stats.time = Date.now();
    }
}

/**
 * Handle arrow key events
 * @param {Event} event
 */
Tetris.prototype.onkeydown = function( event )
{
    if ( (this.gameState == GameState.paused && !(event.keyCode == Key.pause || event.charCode == Key.p || event.charCode == Key.P))
            || (this.gameState != GameState.paused && this.gameState != GameState.underway) ) return;

    switch ( event.keyCode ) {
        case Key.cr:    this.drop();break;
        case Key.up:    if ( event.ctrlKey )this.rotate(this.rd.alt);
        else this.rotate(this.rd.def);break;
        case Key.left:    this.move(-1);break;
        case Key.right:    this.move(+1);break;
        case Key.down:    this.fall();break;
        case Key.pause: this.pause();break;
    }
}

/**
 * Handle letter key events
 * @param {Event} event
 */
Tetris.prototype.onkeypress = function( event ) {
    if ( (this.gameState == GameState.paused && !(event.keyCode == Key.pause || event.charCode == Key.p || event.charCode == Key.P)) || (this.gameState != GameState.paused && this.gameState != GameState.underway) ) return;

    switch ( event.charCode ) {
        case Key.space:            this.drop();break;
        case Key.w: case Key.j:    this.rotate(this.rd.def);break;
        case Key.W: case Key.J:    this.rotate(this.rd.alt);break;
        case Key.a: case Key.h:    this.move(-1);break;
        case Key.d: case Key.l:    this.move(+1);break;
        case Key.s: case Key.k:    this.fall();break;
        case Key.p: case Key.P: this.pause();break;
    }
}

Tetris.prototype.onScore = function( s, bonus ) {
    if ( !this.running.lastScore ) this.running.lastScore = s;
    with ( this ) {
        if ( classic && Math.floor(this.running.lastScore / 1e4) != Math.floor(s / 1e4) ) {
            ++speed;
            updSpeed();
            this.running.lastScore = s;
        }
        if ( this.gameState == GameState.underway ) {
            if ( !sch )if ( speed < animScoreMaxSpeed )animateScore(); else updScore();
            if ( bonus && speed < bonusMaxSpeed )animateBonus(bonus);
        }
        else updScore();
    }
    if ( s == 0 )document.title = 'Javascript Tetris';
}

Tetris.prototype.speedup = function() {
    with ( this ) {
        if ( classic ) return;
        speed += .1;
        updSpeed();
        speedingUp.restart();
        if ( speed > beamFade.start )
            beamAlpha = .04 * (1 - (speed - beamFade.start) / (beamFade.end - beamFade.start));
    }
}

Tetris.prototype.shiftFigure = function() {
    if ( this.gameState != GameState.underway ) return;
    var burnt = 0;
    if ( this.next == null )
        this.current = new Figure(this.serveFigureId());
    else {
        var lines = this.field.getFilledLines(this.field.fill(this.current));
        var lowestLine = lines.reduce(function( a, b ) {
            return Math.max(a, b);
        }, 0);
        burnt = lines.length;
        if ( burnt ) {
            if ( burnt > 1 && this.speed < this.animFaviconMaxSpeed ) {
                if ( burnt == 2 ) Lib.animateFavicon(60, this.anim1);
                else Lib.animateFavicon(70, this.anim2);
            }
            var fl = this.field, vp = this.mainVp, fn = this.redrawBurned.detach(this);
            this.mainVp.shift();
            lines.forEach(function( i ) {
                fl.getLineBlocks(i).forEach(function( b ) {
                    vp.renderBox('#fff', b);
                });
            });
            this.mainVp.unshift();

            this.stats.lines += burnt;
            this.stats.forLines += this.score.score(this.speed, 'burn' + burnt, burnt);

            this.gameState = GameState.paused;
            window.setTimeout(function() {
                fn(lowestLine, lines);
            }, this.classic ? 90 : 0);
        }
    }

    if ( !burnt )
        this.exchangeFigures();
};

Tetris.prototype.redrawBurned = function( lowestLine, lines ) {
    var vp = this.mainVp;
    var blks = this.field.burnLines(lowestLine, lines);
    this.mainVp.shift();
    blks.forEach(function( bk ) {
        vp.renderBox(bk.color, bk.point);
    });
    this.mainVp.unshift();

    this.exchangeFigures();
};

Tetris.prototype.exchangeFigures = function() {
    with ( this ) {
        if ( next )current = next;
        this.gameState = GameState.underway;
        var firstRow = current.figure[0].some(function( x ) {
            return x;
        });
        current.position = new Point(Math.floor(field.size.width / 2 - Figure.area / 2), firstRow ? -1 : -2);
        next = new Figure(serveFigureId());

        fall(true);

        // Ugly magic
        var t = next;
        next = current;
        current = t;
        t = next.position;
        next.position = current.position;
        current.position = t;
        drawFigure({action: 'clear', figure: next});
        t = next;
        next = current;
        current = t;
        t = next.position;
        next.position = current.position;
        current.position = t;

        drawFigure({figure: next});
        ++stats.figures;
    }
};

/**
 *    Serve figures from a 'bag' so each figure can repeat only
 *    twice at most
 */
Tetris.prototype.serveFigureId = function() {
    if ( !this.bag.length )this.bag = Tetris.defaults.figindexes.clone();
    return this.bag.splice(Math.random() * this.bag.length, 1)[0];
};

Tetris.prototype.gameOver = function() {
    with ( this ) {
        gameState = GameState.gameOver;
        window.onkeypress = null;

        falling.stop();
        speedingUp.stop();

        updScore();

        stats.speed = speed;
        stats.time = Date.now() - stats.time;

        var self = this;

        window.setTimeout(function() {
            self.closeField()
            var floater = Tag.floater2over(self.makeFloater(), self.score.points);
            self.showScores(floater);
        }, this.goDelay);
    }
};

/**
 *    Update score where it's displayed (e.g. in window title)
 *
 *    @arg val - if set this score is presented through score-meter and
 *               window title
 */
Tetris.prototype.updScore = function( val ) {
    var score = (val || this.score.points).zf(this.padWidth);

    if ( this.running.curScore == score ) return;
    this.running.shownScore = parseInt(score, 10);
    if ( this.sevenSegment )
        this.scoreSSD.render(this.canvas, score, this.running.curScore);
    if ( !this.sevenSegment )
        this.scoreElt.innerHTML = score;

    document.title = score + ' - Javascript Tetris';
    this.running.curScore = score;
};

Tetris.prototype.updSpeed = function( val ) {
    var speed = (val || this.speed).toFixed(1)
    if ( this.running.curSpeed == speed ) return;
    if ( this.sevenSegment )
        this.speedSSD.render(this.canvas, speed, this.running.curSpeed);
    if ( !this.sevenSegment )
        this.speedElt.innerHTML = speed;

    this.running.curSpeed = speed;
};

Tetris.prototype.updBonus = function( bonus ) {
    bonus = (bonus || this.running.bonus);

    if ( !this.sevenSegment )
        this.bonusElt.innerHTML = bonus;

    if ( this.sevenSegment ) {
        var x = ('+' + bonus).length;
        this.bonusSSD.on = 'rgba(247, 223, 0, ' + this.running.bonusOpacity.within(.1, 1).toFixed(2) + ')';
        this.bonusSSD.render(this.canvas, ('+' + bonus).pad(-6), '0'.x(6 - x) + 'x'.x(x));
    }

    this.running.bonus = bonus;
};

// Effects-n-animations
Tetris.prototype.animateScore = function() {
    with ( this ) {
        var cur = this.running.shownScore || 0;

        if ( this.gameState == GameState.underway && animScore && score.points > cur ) {
            var ln = Math.max(score.points.toString().length, cur.toString().length);
            var a = score.points.zf(ln).toArray(), b = cur.zf(ln).toArray();
            var show = a.reduce(function( res, ai, i ) {
                if ( ai == b[i] ) return '' + res + ai;
                else return '' + res + Math.ceil((Math.floor(ai) + Math.floor(b[i])) / 2) % 10;
            }, '');

            updScore(Math.floor(show));
            sch = window.setTimeout(animateScoreDlg, 90);
        }
        else {
            updScore();
            sch = null;
        }
        //return
        var dif = score.points - cur;
        if ( this.gameState == GameState.underway && animScore && dif > ss.mn ) {
            updScore(cur + Math.min(ss.mx, Math.floor(dif / 3)));
            sch = window.setTimeout(animateScore.detach(this), 50);
        }
        else {
            updScore();
            sch = null;
        }
    }
};

Tetris.prototype.animateBonus = function( bonus ) {
    if ( this.bfh ) window.clearTimeout(this.bfh);
    if ( this.running.bonusOpacity > .5 )
        bonus += this.running.bonus;

    this.running.bonusOpacity = 1;
    this.updBonus(bonus);
    this.bfh = window.setTimeout(this.animateOpacity.detach(this), 90)
}

Tetris.prototype.animateOpacity = function() {
    var cont = true;
    this.running.bonusOpacity /= 1.7;

    if ( this.gameState != GameState.underway || this.running.bonusOpacity < .2 ) {
        this.running.bonusOpacity = 0;
        this.bfh = null;
        cont = false;
    }

    if ( !this.sevenSegment )
        this.bonusElt.style.opacity = this.running.bonusOpacity;
    this.updBonus();

    if ( cont ) this.bfh = window.setTimeout(this.animateOpacity.detach(this), 70);
}

Tetris.prototype.closeField = function() {
    this.mainVp.shift();
    var front = .2, self = this;
    var state = this.field.getState();
    var empty = state[0], set = state[1];
    this.tileCount = empty.length + set.length;

    var c = this.canvas;
    var max = this.field.size.width + this.field.size.height * front;
    var cmin = 90, cpad = 140, tmin = 30, tpad = 1300, omin = 1, opad = -.2;

    var fn = function( bx ) {
        var val = (bx.x + bx.y * front) / max;
        var col = new Color(Math.floor(cmin + val * cpad).toString(16).x(3), omin + val * opad);
        window.setTimeout(function() {
            c.renderBox(col, bx);
            --self.tileCount;
        }, tmin + tpad * val);
    }

    empty.map(fn);
    front = 2.5;
    max = this.field.size.width + this.field.size.height * front;
    cmin = 220;
    cpad = -70;
    tmin = 60;
    tpad = 1e3;
    omin = .7;
    opad = .25;
    set.map(fn);
}

Tetris.prototype.pause = function() {
    with ( this ) {
        if ( gameState == GameState.underway ) { // pause
            stats.time = Date.now() - stats.time;
            gameState = GameState.paused;
            falling.stop();
            speedingUp.stop();
            Tag.floater2pause(makeFloater());
        }
        else if ( arguments.length == 0 && gameState == GameState.paused ) { // play!
            Tag.rm('floater');
            gameState = GameState.underway;
            falling.start(1e3 / speed);
            speedingUp.start();
            stats.time = Date.now() - stats.time;
        }
    }
}

//// Figure movements

/**
 *    Lower the current figure one level down on timer or
 *    by user's request.
 *
 *    @param {Boolean} firstMove - true if we don't wand to check position
 */
Tetris.prototype.fall = function( firstMove ) {
    this.falling.stop();
    if ( this.gameState != GameState.underway ) return;

    firstMove = typeof firstMove == 'boolean' && firstMove;
    var fallOk = false;

    ++this.current.position.y;
    var tr = this.current.getBlocksToRender();
    fallOk = tr.every(this.pOk);

    if ( !firstMove && fallOk ) {
        this.castRay(tr);
        this.drawFigure({action: 'clear'});
    }

    if ( !(firstMove || fallOk) ) {
        --this.current.position.y;
        if ( this.speed < this.beamFade.end )this.castRay(this.current.getBlocksToRender());
        this.shiftFigure();
        if ( !firstMove )this.stats.forFigures += this.score.score(this.speed, 'lower');
    }

    if ( !fallOk && firstMove ) {
        this.gameOver();
        return;
    }

    if ( this.speed < this.beamFade.end )this.castRay(this.current.getBlocksToRender(), true);
    this.drawFigure();
    this.falling.start(1e3 / this.speed);
}

Tetris.prototype.rotate = function( direction ) {
    var rotated = this.current.rotate(direction);
    if ( !rotated ) return;

    var bxs = rotated.andNot(this.current);
    var rotateOk = bxs.every(this.pOk);
    if ( !rotateOk ) return;

    with ( this ) {
        if ( speed < beamFade.end )castRay(current.getBlocksToRender());
        drawFigure({action: 'clear', boxes: current.andNot(rotated)});
        if ( speed < beamFade.end )castRay(rotated.getBlocksToRender(), true);
        current = rotated;
        drawFigure({boxes: bxs});
    }
}

/**
 *    Lay the current figure down and begin dropping new one
 */
Tetris.prototype.drop = function() {
    with ( this ) {
        var bxs = current.getBlocksToRender();
        var delta = bxs.reduce(field.getMinHeight.detach(field), field.size.height - current.position.y);
        if ( delta == field.size.height ) return;
        if ( delta != 0 ) {
            ++stats.drops;
            stats.dropLevels += delta;
            drawFigure({action: 'clear'});
            castRay(bxs);
            current.position.y += delta;
            drawFigure();
            stats.forDrops += score.score(speed, 'drop', delta);
        }
        shiftFigure();
    }
}

Tetris.prototype.move = function( modificator ) {
    var shifted = this.current.shifted(modificator);
    var ok = shifted.draw.every(this.pOk);

    if ( !ok ) return;

    with ( this ) {
        if ( speed < beamFade.end )castRay(current.getBlocksToRender());
        drawFigure({boxes:shifted.draw});
        drawFigure({action:'clear',boxes:shifted.clear});
        current.position.x += modificator;
        castRay(current.getBlocksToRender(), true);
    }
};

Tetris.prototype.mkRay = function() {
    var color = this.current.color.tint(90);
    color.alpha = this.beamAlpha;
    return color.toString();
};

/**
 * Render or clear landing beam of falling figure.
 * @param blocks {Array}    an array of block of a figure
 * @param toDraw {Boolean}  if true a landing beam is to be drawn (optional, false by default)
 */
Tetris.prototype.castRay = function( blocks, toDraw ) {
    if ( !this.beamOn || this.speed >= this.beamFade.end ) return;
    toDraw = !!toDraw;
    var method, bx = this.gridsize, hs = this.field.getHeights(blocks);
    if ( toDraw ) {
        method = this.canvas.fillRect;
        this.canvas.fillStyle = this.mkRay();
    }
    else method = this.canvas.clearRect;

    this.mainVp.shift();
    var n = hs.length;
    while ( n-- > 0 ) {
        var s = hs[n];
        if ( s ) {
            method.call(this.canvas
                    , this.mainVp.border / 2 + n * bx.width, this.mainVp.border / 2 + s.left * bx.height - !toDraw
                    , bx.width - 1, s.length * bx.height - 1 + !toDraw);
        }
    }
    this.mainVp.unshift();
}

/**
 *    render or clear current or next block
 * @param args-object {[action: ('clear'|'render')], [figure: (current|next)]}-default action is render current
 */
Tetris.prototype.drawFigure = function( args ) {
    if ( this.gameState != GameState.underway ) return;
    var action = (args && args.action) || 'render';
    var figure = (args && args.figure) || this.current;

    var color = figure && figure.color, toDraw = action != 'clear', isMain = figure != this.next;
    if ( !toDraw )color = 0;
    var vp = isMain ? this.mainVp : this.nextVp;

    var boxes = args && args.boxes || figure.getBlocksToRender();
    boxes.map(function( bx ) {
        vp.renderBox(color, bx)
    });
}

/// HTML sector
Tetris.prototype.makeFloater = function() {
    var floater = document.createElement('div');

    var ftr = new Rectangle(40, 14);

    floater = Tag.mk('div', {id: 'floater', style:
            Tag.mkCss({width: ftr.width + 'ex', height: ftr.height + 'ex', top: (this.mainVp.pos.y + this.mainVp.size.height + 2) / 2})
    });

    Tag.center(floater);

    this.parent.appendChild(floater);

    return floater;
}

Tetris.prototype.showScores = function( floater ) {
    floater.style.height = 'auto';
    var self = this, top = new TopScores(this.score.points, top10k.cookie + (this.classic ? '-classic' : ''));
    top.current.nick = Cookie.get(top10k.lastNick) || top.current.nick;

    var holder = Tag.mk('div');
    Tag.enclose(floater, holder);

    var fbh = floater.clientHeight, maxLen = 20;
    var elements = [], input;
    var adjustHeight = function() {
        Tag.center(floater)
    };

    var c = 0, max = 0, classes = ['even', 'odd'], table = Tag.mk('table', {id: 'local-scores', class: 'score'});
    elements.push(table);

    // fill scores table
    top.local.map(function( record ) {
        max = Math.max(max, record.score);
        Tag.addRow(table, [++c, record.nick, record.score], {class: classes[c % 2], title: 'achieved ' + record.time.toAgoInterval()});
    })

    var restFn = function() {
        if ( self.tileCount ) return false;
        this.blur();
        self.reset(true);
        self.redraw();
        self.start();
    }

    var restBtn = Tag.mk('button', {id: 'restbtn', class: 'clear-scores', onclick: restFn}, 'restart');

    var clearFn = function() {
        if ( max > 100000 && !confirm('Are you sure, the top score is ' + max + '?') )return;
        top.clear();
        Tag.rm(form);
        Tag.rm('local-scores');
        Tag.rm('clrbtn');
        adjustHeight();
        restBtn.focus();
    }

    elements.push(restBtn,
            Tag.mk('button', {id: 'clrbtn', class: 'clear-scores', onclick: clearFn}, 'clear'));

    var postData = {score: top.current.score};
    ['dropLevels', 'drops', 'figures', 'lines', 'time'].map(function( fld ) {
        postData[fld] = self.stats[fld];
    });
    postData.gameTime = postData.time;
    postData.time = top.current.time;
    postData.speed = this.speed;

    if ( this.classic ) postData.classic = 1;

    var showGlobalTable = function( data ) {
        var gt = Tag.mk('table', {id: 'global-scores', class: 'score'}), c = 0;
        data.extract.map(function( record ) {
            var cells, title = '', cellattrs;
            if ( !record ) {
                cells = ['...'];
                cellattrs = [{colspan: 3}];
            }
            else {
                cells = [record.pos + 1, record.nick, record.score];
                title = 'achieved ' + record.time.toAgoInterval();
            }
            var attrs = {class: classes[++c % 2], title: title};
            if ( record.pos == data.pos )
                attrs.class += ' current';
            Tag.addRow(gt, cells, attrs, cellattrs);
        })
        var text = '<a href="top.php' + (self.classic ? '?table=classic' : '') + '">Global top <em>' + data.total + '</em></a>';

        if ( data.pos != -1 )
            text += (", you're <${tag}>${pos}</${tag}>").fmt({tag:data.pos < data.total / 3 ? 'em' : 'span', pos: (data.pos + 1 == data.total && data.pos > 2 ) ? 'last' : (data.pos + 1).toPos()});

        holder.insertBefore(Tag.mk('h1', null, text), restBtn);
        holder.insertBefore(gt, restBtn);

        Tag.enclose(floater, Tag.enclose(Tag.mk('div', {class: 'stats global'}), Lib.mkStatsReport(data.stats, true, self.classic)));

        fbh += 10;

        adjustHeight();
    }

    var logIntoGlobal = function( nick ) {
        postData.nick = nick;
        Ajax.post(top10k.url, postData, showGlobalTable, 'js');
    }

    if/*player scored in top10*/( top.localPos != -1 ) {
        var form = Tag.mk('form');
        input = Tag.mk('input', {type: 'text',
            id: 'player-name',
            maxlength: maxLen,
            value: top.current.nick,
            class: 'nick'
        });

        var scoredPosFmt = ', <${tag}>${pos}</${tag}> result${excl}';
        Tag.get('#gameover-top').innerHTML += scoredPosFmt.fmt({
            tag: (top.localPos + 1 > 3 ? 'span' : 'em'),
            pos: (top.localPos + 1).toPos(),
            excl: top.localPos ? '' : '!'
        });

        Tag.enclose(form, Tag.mk('label', {'for': 'player-name'}, 'Your name:'));
        Tag.nest([input, form, floater]);

        var target = table.childNodes[top.localPos].childNodes[1];

        form.onsubmit = function() {
            if ( self.tileCount ) return false;
            if ( input.value.trim() ) {
                input.value = target.innerHTML = top.current.nick
                        = input.value.trim().substr(0, maxLen);
                top.save();
                Cookie.set(top10k.lastNick, input.value);
                restBtn.focus();
                this.style.display = 'none';
                adjustHeight();
                logIntoGlobal(input.value);
            }
            else target.innerHTML = '&lt;' + top.current.nick + '&gt;';

            return false;
        }

        input.onkeyup = function() {
            target.innerHTML = this.value.trim() ? this.value : target.innerHTML = '&lt;' + top.current.nick + '&gt;'
        }

        table.childNodes[top.localPos].className += ' current';
        elements.splice(0, 0, form);
    } // if(top.localPos != -1) - if player scored in top10

    var rr = this.score.points.rr();
    if ( this.classic ) rr = (this.score / 100).rr();
    var comment = '&nbsp;', title = '';
    if ( rr ) comment = 'Wow, a <em>' + rr + '</em> score!',title = 'Nice score, I mean the number';

    elements.splice(0, 0, Tag.mk('div', {class: 'repetance', title: title}, comment));

    Tag.enclose(holder, elements);
    adjustHeight();

    Tag.enclose(floater, Tag.enclose(Tag.mk('div', {class: 'stats'}), Lib.mkStatsReport(this.stats, false, this.classic)));

    if ( input ) {
        input.focus();
        input.select();
    }
    else
        restBtn.focus();

    top.save();
}

Tetris.prototype.makeHtml = function() {
    var size = new Rectangle(
            this.nextVp.pos.x + this.nextVp.size.width + 2,
            this.mainVp.pos.y + this.mainVp.size.height + 2
            );

    var canvas = Tag.mk('canvas',
    {id: 'tetris-canvas', width: size.width, height:size.height},
            'Only for browsers supporting canvas element.');

    var pad = Math.floor(this.mainVp.size.height - Figure.area * this.gridsize.height);
    var w = Figure.area * this.gridsize.width / 4;

    var s = 'margin-top:-${m}px;font-size:${f}px;padding-right:${p}px', cell = [canvas];
    var set = {id: 't-s', class: 'meters', title: 'Total score', style: s.fmt({m: pad, f: w, p:(w * .8)})}
    if ( !this.sevenSegment ) {
        this.scoreElt = Tag.mk('div', set);
        with ( set ) {
            id = 't-b';
            title = 'Latest scored points';
            style = s.fmt({m: pad - .9 * w, f: w * .8, p:(w / 2)});
        }
        this.bonusElt = Tag.mk('div', set);
        with ( set ) {
            id = 't-d';
            title = 'Speed (drops per second)';
            style = s.fmt({m: pad - 2 * w, f: w * .8, p:(w / 2)});
        }
        this.speedElt = Tag.mk('div', set);

        cell.push(this.scoreElt, this.bonusElt, this.speedElt);
    }

    Tag.nest([
        Tag.enclose(Tag.mk('tr'), [
            Tag.enclose(Tag.mk('td'), cell),
            Tag.mk('td', {id: 'desc', width: Math.floor(size.width * .56).constrain(200, 300)}, tetrisDescription)
        ]),
        Tag.mk('tbody'), Tag.mk('table'), this.parent
    ]);

    (function() {
        var h = this.innerHTML;
        var l = h.replace(/ at /, '@').replace(/ dot /, '.');
        this.innerHTML = '<a href="mailto:' + l + '">' + l + '</a>'
    }).apply(document.getElementById('addr'));
    this.canvas = this.mainVp.canvas = this.nextVp.canvas = canvas.getContext('2d');

    this.canvas.tetris = this;
    this.canvas.size = size;//canvasSize;
}
