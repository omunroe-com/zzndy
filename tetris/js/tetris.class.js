var tetrisDescription = '<div id="desc"><h1>Javascript Tetris v3</h1><p>This is a tetris game written in javascript without using images. It requires <a href="http://www.mozilla.com/firefox">Firefox</a> browser.</p><p>The point of the game is to score as much as possible. Fast play is favored.</p><p>By dropping figures (with <em>space</em>) on can get many points-score for drop&shy;ing is pro&shy;por&shy;tional to the number of lines the figure falls. Speed in&shy;crea&shy;ses with time and not with score.</p><p>To restart just press <em>F5</em>. To pause press <em>Pause/Break</em> button or <em>P</em>.</p><p>To control block&apos;s position use <em>left</em> and <em>right</em> buttons, <em>a</em> and <em>d</em> or <em>h</em> and <em>l</em>. To rotate figure press <em>up</em> button, <em>w</em>&nbsp;or&nbsp;<em>j</em>.</p><p>To lower figure by one level press <em>down</em> arrow, <em>s</em> or <em>k</em>. It is re&shy;com&shy;mended though to drop figures with <em>space</em> as it&apos;s faster and yields significant amount of score. </p><div>&copy; A. Vynogradov 2007</div><div id="addr">zzandy at rootshell dot be</div></div>';

var top10k = {
	url: 'http://www.rootshell.be/~zzandy/top10k.php'
}

//// Tetris class ////////////////////////////////////////////////////////////

var Key	= {
	left: 37, up: 38, right: 39, down: 40,
	a: 97,  w: 119, d: 100, s: 115,
	h: 104, j: 106,  k: 107, l: 108,
	W: 87, J: 74, p:112, P:80,
 	cr: 13,	space: 32, pause:19
}

var GameState = {notStarted: 0, underway: 1, paused: 2, gameOver: 3}

function Tetris(parentId, params)	{
	this.parent = document.getElementById(parentId);
	this.gameState = GameState.notStarted;
	this.padWidth = 6;

	if(params && params['field'])	{
		this.field = new Field(params.field.width, params.field.height);
	}
	else
		this.field = new Field(Tetris.default.fieldSize.width, Tetris.default.fieldSize.height);
		
	this.animatedScore = !!!(params && params['noAnimatedScore']);

	if(params && params['box'])
		this.gridsize = params.box;
	else
		this.gridsize = Tetris.default.box;

	if(params && params['grid'])
		this.box = Rectangle.dif(this.gridsize, new Rectangle(1, 1));
	else
		this.box = new Rectangle(this.gridsize);

	this.pixel = Rectangle.mul(this.field.size, this.gridsize);
	this.pixel.add(2,2);
	this.pOk = this.field.pointOk.detach(this.field)
	this.score = new Score();
	this.score.onScore = this.onScore.detach(this);
	this.speed = 0.9;
	this.speedupInterval = 10000;  // Speed increases by 0.1 drop per second each minute
	this.bfh = null; // bonus fade handle
	this.sch = null; // score ticked handle
	this.canvas = null;
	this.next = null;
	this.origin = new Point(2, 2);
	this.falling = new Timer(this.fall.detach(this), 1000 / this.speed);
	this.speedingUp = new Timer(this.speedup.detach(this), this.speedupInterval);

	this.init();
}

Tetris.prototype.blksOk = function(blks)	{
	return blks.every(this.pOk);
}

Tetris.prototype.init = function(){with(this)	{
	makeHtml();
	initConsts();
	drawGrid();
	drawFrame();
	makeStartBtn();
}}

/**
 *	Meke miscellaneous precalculated variables
 */
Tetris.prototype.initConsts = function()	{
	with(this.gridsize)	{this.val = {
		hlColor: Color.parse('fff', .25).toString(),
		shadowColor: Color.parse('000', .15).toString(),
	}}
	
	with(this.box)	{
		this.val.hi = [0, 0, width * .95, height * .05, width * .12, height * .12, width * .05, height * .95];
		this.val.lo = [width, height, width * .95, height * .05, width * .88, height * .88,	width * .05, height * .95];
	}
}

Tetris.prototype.start = function()	{
	this.removeFloater();
	window.onkeypress = this.keyDown.detach(this);
	this.gameState = GameState.underway;
	this.shiftFigure();
	this.speedup();
	this.onScore(this.score.points);
}

Tetris.prototype.onScore = function(score, bonus)	{
	if(!this.sch) this.reachScore();
	if(bonus)this.bonusFade(bonus);
	
	if(score!=0);// document.title = score + ' - Javascript Tetris';
	else document.title = 'Javascript Tetris';
}

Tetris.prototype.reachScore = function()	{
	var step = 17;
	var disp = parseInt(this.scoreElt.innerHTML, 10);
	
	if(this.animatedScore && disp < this.score.points - step){
	 	document.title = (disp + step).zerofill(this.padWidth) + ' - Javascript Tetris';
		this.scoreElt.innerHTML = (disp + step).zerofill(this.padWidth);
		this.sch = window.setTimeout(this.reachScore.detach(this), 70);
	}
	else	{
		this.scoreElt.innerHTML = this.score.points.zerofill(this.padWidth);
		document.title = this.score.points.zerofill(this.padWidth) + ' - Javascript Tetris';
		this.sch = null;
	}
}

Tetris.prototype.bonusFade = function(bonus)	{
	if(this.bfh) window.clearTimeout(this.bfh);
	if(this.bonusElt.style.opacity>.6)
		bonus += parseInt(this.bonusElt.innerHTML);
	
	this.bonusElt.style.opacity = 1;
	this.fadeOpacity();
	this.bonusElt.innerHTML = bonus;
}

Tetris.prototype.fadeOpacity = function()	{
	with(this.bonusElt.style)	{
		opacity = opacity/1.6;
		if(opacity < 0.1)	{
			opacity = 0;
			this.bfh = null;
			return;
		}
	}
	this.bfh = window.setTimeout(this.fadeOpacity.detach(this), 70);
}

Tetris.prototype.keyDown = function(event)	{
	if(this.gameState == GameState.paused)	{
		if(event.keyCode != Key.pause && event.charCode != Key.p && event.charCode != Key.P) return;
	} else 
	if(this.gameState != GameState.underway) return;
	
	switch(event.keyCode)	{
	case Key.cr:
		this.drop();
		break;
	case Key.up:
		if(event.shiftKey)this.rotate('cw');
		else this.rotate('ccw');
		break;
	case Key.left:
		this.move(-1);
		break;
	case Key.right:
		this.move(+1);
		break;
	case Key.down:
		this.fall();
		break;
	case Key.pause:
		this.pause();
		break;
	}

	switch(event.charCode)	{
	case Key.space:
		this.drop();
		break;
	case Key.w: case Key.j:
		this.rotate('ccw');
		break;
	case Key.W: case Key.J:
		this.rotate('cw');
		break;
	case Key.a: case Key.h:
		this.move(-1);
		break;
	case Key.d: case Key.l:
		this.move(+1);
		break;
	case Key.s: case Key.k:
		this.fall();
		break;
	case Key.p: case Key.P:
		this.pause();
		break;
	}
}

Tetris.prototype.speedup = function()	{
	this.speed += .1;
	this.speedElt.innerHTML = this.speed.toFixed(1);
	this.speedingUp.restart();
}

Tetris.prototype.shiftFigure = function()	{
	if(this.next == null)	{
		this.current = new Figure(Math.floor(Math.random() * Figure.figures.length));
	}
	else	{
		var lines = this.field.getFilledLines(this.field.fill(this.current));

		var burnout = 0;
		var lowestLine = 0;
		for(var i in lines)	{
			++burnout;
			lowestLine = Math.max(lowestLine, i);
		}
		
		if(burnout)	with(this.canvas) {
			var blks = this.field.burnLines(lowestLine, lines);
			translate(this.origin.x, this.origin.y);
			for(var i=0, bk; bk = blks[i]; ++i)renderBox(bk.color, bk.point);
			translate(-this.origin.x, -this.origin.y);
			this.score.score(this.speed, Score.action['lineout' + burnout], burnout);
		}
		this.current = this.next;
	}
	with(this)	{
		current.position = new Point(Math.floor(field.size.width / 2-Figure.area / 2), current.figureId == 0?-1:-2);
		next = new Figure(Math.floor(Math.random() * Figure.figures.length));
		fall(true);
		[next, current] = [current, next];[next.position, current.position] = [current.position, next.position];
		drawFigure({action: 'clear', figure: next});
		[next, current] = [current, next];[next.position, current.position] = [current.position, next.position];
		drawFigure({figure: next});
	}
}

var lev = 0;

/**
 *	Lower the current figure one level down on timer or
 *	by user's request.
 *
 *	@param firstMove:bool - true if we don't wand to check position
 */
Tetris.prototype.fall = function(firstMove)	{
	firstMove = typeof firstMove == 'boolean' && firstMove;
	this.falling.stop();
	
	var fallOk = false;
	++this.current.position.y;
	var tr = this.current.getBlocksToRender();
	fallOk = this.blksOk(tr);
	if(!firstMove && fallOk)this.drawFigure({action: 'clear', shift: -1});
	
	if(!(firstMove || fallOk))	{
		--this.current.position.y;
		this.shiftFigure();
	}
	
	if(!fallOk && firstMove)	{
		this.gameOver();
		return;
	}
	this.drawFigure();
	this.falling.start(1000 / this.speed);
}

Tetris.prototype.gameOver = function(){with(this)	{
	gameState = GameState.gameOver;
	
	falling.stop();
	speedingUp.stop();
	
	closeField();
	
	showGameOverMsg();
	showLocalScores();
}}

Tetris.prototype.closeField = function()	{
	this.canvas.translate(this.origin.x, this.origin.y);
	var front = 2.5;
	var [empty, set] = this.field.getState();
	var [c, max] = [this.canvas, 1.0 * this.field.size.width + this.field.size.height * front];
	var [opac, cmin, cpad, tmin, tpad] = [null, 90, 140, 50, 1300];

	var fn = function(bx) {
		var front = 2.5;
		var val = (bx.x + bx.y*front) / max;
		var col = Color.parse(Math.floor(cmin + val*cpad).toString(16).rep(3), opac).toString();
		window.setTimeout(function(){c.renderBox(col, bx)}, tmin + tpad * val);
		return true;
	}
	
	empty.every(fn);
	var [opac, cmin, cpad, tmin, tpad] = [null, 230, -90, 100, 1000];
	set.every(fn);
}

Tetris.prototype.pause = function(){with(this){
	if(gameState == GameState.underway)	{
		gameState = GameState.paused;
		falling.stop();
		speedingUp.stop();
		showPauseMsg();
	}
	else if(gameState == GameState.paused)	{
		removeFloater();
		gameState = GameState.underway;
		falling.start(1000 / speed);
		speedingUp.start();
	}
}}

Tetris.prototype.rotate = function(direction)	{
	var rotated = this.current.rotate(direction);
	if(!rotated) return;
	
	var bxs = rotated.andNot(this.current);
	var rotateOk = this.blksOk(bxs);
	if(!rotateOk) return;

	with(this)	{
		drawFigure({action: 'clear', boxes: current.andNot(rotated)});
		current = rotated;
		drawFigure({boxes: bxs});
	}
}

/**
 *	Lay the current figure down and begin dropping new one
 */
Tetris.prototype.drop = function(){with(this)	{
	var bxs = current.getBlocksToRender();
	var delta = bxs.reduce(field.getMinHeight.detach(field), field.size.height-current.position.y);
	
	if(delta == 0 || delta == field.size.height) return;
	
	drawFigure({action: 'clear'});
	current.position.y += delta;
	drawFigure();
	score.score(speed, Score.action.drop, delta);
	shiftFigure();
}}

Tetris.prototype.move = function(modificator)	{
	var shifted = this.current.shifted(modificator);
	var ok = this.blksOk(shifted.draw);
	
	if(!ok) return;

	with(this)	{
		drawFigure({boxes:shifted.draw});
		drawFigure({action:'clear',boxes:shifted.clear});
		current.position.x += modificator;
	}
}

/**
 *	render or clear current or next block
 * @param args-object {[action: ('clear'|'render')], [figure: (current|next)]}-default action is render current
 */
Tetris.prototype.drawFigure = function(args)	{
	var action = (args && args.action) || 'render';
	var figure = (args && args.figure) || this.current;
	var shift = ((args && args.shift) || 0) * this.gridsize.height;
	
	var color = figure && figure.color;
	if(action == 'clear')
		color = -1;
	
	var origin = new Point(this.origin.x, this.origin.y + shift);

	if(figure == this.next)
		origin.add(Math.floor(this.pixel.width + .5 * this.box.width), 0);

	this.canvas.translate(origin.x, origin.y);
	
	var boxes = args && args.boxes || figure.getBlocksToRender();
	
	for(var i=0; i<boxes.length; ++i)
		this.canvas.renderBox(color, boxes[i]);
	
	this.canvas.translate(-origin.x, -origin.y);
}

Tetris.default = {
	fieldSize: new Rectangle(9, 20),
	box: new Rectangle(20,20)
}

/// Draw once sector

Tetris.prototype.drawGrid = function()	{
    if(this.box.width == this.gridsize.width) return;
	this.canvas.translate(this.origin.x, this.origin.y);

	var grad = this.canvas.createLinearGradient(0, 0, 0, this.pixel.height);
	this.fillGradient(grad, 5);
	
	this.canvas.fillStyle = grad;
	with(this.gridsize){
		for(var i=1; i<this.field.size.height; ++i)
			this.canvas.hLine(-1, i * height - 1, this.pixel.width);
	
		for(var i=1; i<this.field.size.width; ++i)	{
			this.canvas.vLine(i * width-1, -1, this.pixel.height);
			this.canvas.clearRect((i-1) * width, -1, width-1, this.pixel.height);
		}
		this.canvas.clearRect((this.field.size.width-1) * width, 0, width-1, this.pixel.height);
	}
	
	// draw grid for next block figure
	this.canvas.translate(Math.floor(this.pixel.width + .5 * this.box.width) - 1, -1);
	
	var grad = this.canvas.createLinearGradient(0, 0, 0, Figure.area * this.box.height);
	this.fillGradient(grad, 3);
	
	this.canvas.fillStyle = grad;
	
	with(this.gridsize){
		var [v, h] = [parseInt(height*.1), parseInt(width*.2)];
		
		for(var i=1; i<Figure.area; ++i)	{
			this.canvas.hLine(width-h, i*height, (Figure.area - 2)*width + 2*h + 1);
			this.canvas.vLine(width*i, height-v, (Figure.area - 2)*height + 2*v + 1);
		}
		for(var i=1; i<Figure.area - 1; ++i)	{
			this.canvas.clearRect(width, i * height + v, (Figure.area - 2) * width + 1, height - 2*v);
			this.canvas.clearRect(width * i + h, height, width - 2*h, (Figure.area - 2) * height +1);
		}
	}
	this.canvas.translate(- Math.floor(this.pixel.width + .5 * this.box.width) + 1, 1);
	this.canvas.translate(-this.origin.x, -this.origin.y);
}

Tetris.prototype.fillGradient = function(grad, decay)	{
	decay = decay || 1.5;
	grad.addColorStop(0, Color.parse('f068fb', .6 / decay));
	grad.addColorStop(.25, Color.parse('f7f045', .55 / decay));
	grad.addColorStop(.5, Color.parse('90f87a', .5 / decay));
	grad.addColorStop(.75, Color.parse('f88576', .45 / decay));
	grad.addColorStop(1, Color.parse('70f0ea', .4 / decay));
}

Tetris.prototype.drawFrame = function()	{
	var grad = this.canvas.createLinearGradient(this.pixel.width / 4, 0, this.pixel.width * 3/4, this.pixel.height);
	this.fillGradient(grad);

	with(this.canvas){	
		fillStyle = grad;
		sharpRect(0,0,this.pixel.width,this.pixel.height);
		
		// Draw frame for next block figure
		translate(Math.floor(this.pixel.width + .5 * this.gridsize.width), 0);
		
		var grad = createLinearGradient(Figure.area * this.gridsize.width / 4, 0, Figure.area * this.gridsize.width * 3/4, Figure.area * this.gridsize.height);
		this.fillGradient(grad);
		
		fillStyle = grad;
		sharpRect(0, 0, Figure.area * this.gridsize.width, Figure.area * this.gridsize.height);
		translate(- Math.floor(this.pixel.width + .5 * this.gridsize.width), 0);
	}
}

/// HTML sector
Tetris.prototype.makeFloater = function()	{
	var floater = document.createElement('div');
	
	var [wnd, ftr] = [new Rectangle(window.innerWidth, this.pixel.height), new Rectangle(200, 80)];
	var pos = new Point((wnd.width-ftr.width)/2.3, (wnd.height-ftr.height)/2);
	
	floater = Widget.make('div', {id: 'floater', style: 'top:' + pos.y + 'px;left:' + pos.x + 'px;width:' + ftr.width + 'px;height:' + ftr.height + 'px;'});
	
	this.parent.appendChild(floater);
	
	return floater;
}

Tetris.prototype.removeFloater = function()	{
	var floater = document.getElementById('floater');
	if(floater) floater.parentNode.removeChild(floater);
}

function submit_name()	{
	alert('hello');
	return false;
}

Tetris.prototype.showLocalScores = function()	{
	function ynh()	{
		var form = document.createElement('form');
		form.setAttribute('onsubmit', 'return submit_name()');
		var input = document.createElement('input');
		input.setAttribute('id', 'user-name');
		input.setAttribute('value', 'Anonymvs');
		input.setAttribute('type', 'text');
		input.setAttribute('maxlength', 18);
		form.appendChild(input);
		return form;
	}
	
	var localScores = Cookie.get('jst3-scores');
	
	if(!localScores)
		localScores = [];
	else
		localScores = localScores.split(';');
	
	localScores.push((new Record('@current', this.score.points, Date.now())).toString());

	localScores.map(function(a){return new Record(a)});
	localScores.sort(Record.compare).reverse();
	
	var table = document.createElement('table');
	table.setAttribute('id', 'scores-local');
	var tbody = document.createElement('tbody');
	table.appendChild(tbody);
	var input = document.createElement('input');
	input.setAttribute('type', 'text');
	input.setAttribute('id', 'ynh');
	input.setAttribute('title', 'Your name here');
			
	for(var i=0, item; item=localScores[i]; ++i)	{
		
		var row = document.createElement('tr');
		var cell0 = document.createElement('td');
		var cell1 = document.createElement('td');
		var cell2 = document.createElement('td');
		
		cell0.appendChild(document.createTextNode(i+1));
		if(item.nick == '@current')
			cell1.appendChild(ynh());
		else
			cell1.appendChild(document.createTextNode(item.nick));
		cell2.appendChild(document.createTextNode(item.score.toString().replace(/(\d{3})$/, String.fromCharCode(0xA0) + '$1')));
		cell2.setAttribute('class', 'score');
		
		row.setAttribute('title', 'on '+item.time);
		row.appendChild(cell0);
		row.appendChild(cell1);
		row.appendChild(cell2);
		tbody.appendChild(row);
		
	}
	document.getElementById('floater').style.height = 'auto';
	document.getElementById('floater').appendChild(table);
	
	if(x = document.getElementById('user-name'))	{
		x.focus();
		x.select();
	}
}

Tetris.prototype.showGameOverMsg = function()	{
	Widget.enclose(this.makeFloater(), [
		Widget.make('div', {id: 'gameover-msg'}, '◆ Game Over ◆'), 
		Widget.make('div', {id: 'gameover-top'}, 'You scored <em>'+this.score.points+'</em> points')
	]);
}

Tetris.prototype.showPauseMsg = function()	{
	Widget.enclose(this.makeFloater(), [
		Widget.make('div', {id: 'pause-msg'}, '● Pause ●'), 
		Widget.make('div', {id: 'pause-top'}, 'Press <em>pause</em> or <em>p</em> to continue')
	]);
}

Tetris.prototype.makeStartBtn = function()	{
	var button = Widget.make('button', {id: 'runit', onclick: this.start.detach(this)}, 'Start');
	
	var floater = this.makeFloater();
	
	/*var button = document.createElement('button');
	button.setAttribute('id', 'runit');
	button.appendChild(document.createTextNode('Start'));
	
	button.onclick = this.start.detach(this);*/
	
	floater.appendChild(button);
	button.focus();
	
	return button;
}

Tetris.prototype.makeHtml = function()	{
	var canvasSize = Rectangle.sum(this.pixel, new Rectangle(Figure.area * this.gridsize.width + 20, 5));
	var canvas = Widget.make('canvas', 
		{id: 'tetris-canvas', width: canvasSize.width, height: canvasSize.height}, 
		'Only for browsers supporting canvas element.');
	
	var pad = Math.floor(this.pixel.height - Figure.area * this.gridsize.height);
	var w = Figure.area * this.gridsize.width / 4;
	
	var s = 'margin-top:-${m}px;font-size:${f}px;padding-right:${p}px';
	var set = {id: 't-s', class: 'meters', title: 'Total score', style: s.fmt({m: pad, f: w, p:(w*.8)})}
	
	this.scoreElt = Widget.make('div', set);
	with(set)[id, title, style] = ['t-b', 'Latest scored points', s.fmt({m: pad-.9*w, f: w*.8, p:(w/2)})]
	this.bonusElt = Widget.make('div', set);
	with(set)[id, title, style] = ['t-d', 'Speed (drops per second)', s.fmt({m: pad-2*w, f: w*.8, p:(w/2)})]
	this.speedElt = Widget.make('div', set);
	
	Widget.nest([
		Widget.enclose(Widget.make('tr'), [
			Widget.enclose(Widget.make('td'), [canvas, this.scoreElt, this.bonusElt, this.speedElt]),
			Widget.make('td', {id: 'desc', width: Math.floor(canvasSize.width * .5)}, tetrisDescription)
		]),
  		Widget.make('tbody'), Widget.make('table'), this.parent
	]);
	
	(function(){var h=this.innerHTML; var l=h.replace(/ at /, '@').replace(/ dot /, '.'); this.innerHTML = '<a href="mailto:'+l+'">'+l+'</a>'}).apply(document.getElementById('addr'));
	this.canvas = canvas.getContext('2d');
	this.canvas.tetris = this;
	
}
