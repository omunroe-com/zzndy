package  {
	
	import flash.display.Sprite;
	import flash.events.Event;
	import flash.utils.Timer;
	import flash.events.TimerEvent;
	
	import Box2D.Dynamics.*;
	import Box2D.Collision.*;
	import Box2D.Collision.Shapes.*;
	import Box2D.Common.Math.*;

	public class Demo extends Sprite {

		// Constants:
		// Public Properties:
		public var world:b2World;
		public var timer:Timer = new Timer(1000);
				
		// Private Properties:
		private var scale:int = 30;

		// Initialization:
		public function Demo() { 
			var environment:b2AABB = new b2AABB();
			environment.lowerBound.Set(-100.0, -100.0);
			environment.upperBound.Set(100.0, 100.0);
			
			var gravity:b2Vec2=new b2Vec2(0.0,9.8);
			
			world = new b2World(environment,gravity,true);
						
			var debug_sprite:Sprite = new Sprite();
			addChild(debug_sprite);
			
			var debug_draw:b2DebugDraw = new b2DebugDraw();
			debug_draw.m_sprite=debug_sprite;
			debug_draw.m_drawScale=scale;
			debug_draw.m_fillAlpha=0.5;
			debug_draw.m_lineThickness=1;
			debug_draw.m_drawFlags=b2DebugDraw.e_shapeBit;
			world.SetDebugDraw(debug_draw);
			
			var final_body:b2Body;
			var the_body:b2BodyDef;
			var the_box:b2PolygonDef;
			the_body = new b2BodyDef();
			the_body.position.Set(550/2/scale, 385/scale);

			the_box = new b2PolygonDef();
			the_box.SetAsBox(550/2/scale, 10/2/scale);
			the_box.friction=0.7;
			the_box.density=0;
			
			final_body=world.CreateBody(the_body);
			final_body.CreateShape(the_box);
			final_body.SetMassFromShapes();
			final_body.SetAngularVelocity(Math.PI / 180.0);
			
			addEventListener(Event.ENTER_FRAME, on_enter_frame);
			
			timer.addEventListener(TimerEvent.TIMER, on_time);
			timer.start();
 		}

		// Public Methods:
		public function on_time(e:Event) {
			var final_body:b2Body;
			var the_body:b2BodyDef;
			var the_box:b2PolygonDef;
			the_body = new b2BodyDef();
			the_body.position.Set(Math.random()*10+2, 0);
			the_body.angle = 2*Math.random()*Math.PI;
			the_box = new b2PolygonDef();
			the_box.SetAsBox(Math.random()+0.1,Math.random()+0.1);
			the_box.friction=0.5;
			the_box.density=1;
			final_body=world.CreateBody(the_body);
			final_body.CreateShape(the_box);
			final_body.SetMassFromShapes();
			
			final_body.SetAngularVelocity(2*Math.random()*Math.PI);
		}
		
		public function on_enter_frame(e:Event) {
			world.Step(1/30, 10);
		}

		// Protected Methods:
		
	}

}