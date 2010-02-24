// Model, View and Controller for shooters

function ShooterArbiter(dflt, number)
{
	this.number = number;
	this.dflt = dflt;
	this.shooters = [];
	this.projectiles = [];

	var i=-1;
	while(++i<number)
	{
		var s = new Shooter(dflt);
		s.pos.x += Math.random() * 800 - 400; 
		s.pos.y += Math.random() * 500 - 250; 
		s.steer(Math.random() * Math.PI * 2);

		this.shooters.push(s);
	}
}

var deg = Math.PI / 180;

function ShootersView(ctx, w, h)
{
	this.ctx = ctx;
	this.w = w;
	this.h = h;
	
	ctx.fillStyle = ctx.strokeStyle = '#eee';
	ctx.save().translate(w/2, h/2).scale(1, -1);
}

ShootersView.prototype.clear = function()
{
	ctx.clearRect(-this.w/2, -this.h/2, this.w, this.h);
}

ShootersView.prototype.render = function(s)
{
	if(s instanceof Shooter)
	{
		var size = 3.5;

		ctx.save()
			.translate(s.pos.x, s.pos.y)
			.rotate(s.mov.getDir() - Math.PI / 2)
			.beginPath()
			.moveTo(-size, -size)
			.lineTo(size, -size)
			.lineTo(0, 3*size)
			.closePath()
			.stroke()
			.restore();
	}
	else if(s instanceof Projectile)
	{
		if(s.dead)
		{
			ctx.save()
				.translate(s.pos.x, s.pos.y)
				.fillCircle(0, 0, 5*(3+s.dead))
				.restore();
		}
		else
		{
			ctx.save()
				.translate(s.pos.x, s.pos.y)
				.rotate(s.mov.getDir() - Math.PI / 2)
				.moveTo(0,0)
				.lineTo(0, 3)
				.stroke()
				.restore()
		}
	}
}

function ShootersController(view, model)
{
	this.view = view;
	this.model = model;
	this.ts = 0;
}

ShootersController.prototype.frame = function()
{
	var ts = this.ts = (new Date()).getTime();
	var c = this;
	var ps = this.model.projectiles;

	this.model.shooters.forEach(function(s){
			s.advance();
			c.limit(s);
			if(Math.random() < .2)
				s.steer((Math.random() * 30 - 15)*deg);

			if(s.readyToShoot(ts))
			{
				s.shoot(ts).forEach(function(x){ps.push(x)});
			}
			});
	var ss = this.model.shooters;

	this.model.projectiles = ps.filter(function(p){
			p.advance();
			c.limit(p);
			if(p.dead) p.dead++;
			return p.dead <= 10;
			});

	this.model.projectiles.forEach(function(s){
			ss.forEach(function(p){
				var d = p.pos.to(s.pos);
				if(d < 3){
					s.dead = 1;
					p.hp -= s.dam;
					if(p.hp <= 0)
						p.pos = new Point(0, 0);
					}
				});
			});
}

ShootersController.prototype.limit = function(s)
{
	if(s.pos.x < -400) s.pos.x = 400;
	if(s.pos.y < -250) s.pos.y = 250;
	if(s.pos.x > 400) s.pos.x = -400;
	if(s.pos.y > 250) s.pos.y = -250;
}

ShootersController.prototype.draw = function()
{
	var v = this.view;
	var s = this.model.shooters;
	var p = this.model.projectiles;

	window.setTimeout(function(){
			v.clear(); 
			p.forEach(v.render);
			s.forEach(v.render);
		}, 1);

}

