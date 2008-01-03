function db_gen()	{

}

db_gen.parse = function(data)	{
	function parse_line(line)	{
		function parse_field(data)	{
			if(!current) {console.log('ERROR: Current not set');return;}
			console.log(current);
			var [name, definition] = data.trimsplit(':');
			var options = definition.ts(',');
			var type = options[0];
			
			var field = new Field(current.name, name, type);
			
			for(var i=1, options; option = options[i]; ++i)	{
				if(option.indexOf('&') == 0)	{
					var [rt, tf] = option.substr(1).split(':');
					if(!tf) tf = name;
					field.ref = [rt, rf];
				}
				if(option == 'auto')
					field.options.auto = true;
			}
			console.log(field);
			return field;
		}
		
		line = line.replace(/#.*$/, '');
		if(line.match(/^\s+\w+/))	{
			var field = parse_field(line.trim());
			if(field)current.fields.push(field);
		}
		else if(line.match(/^\w+/))	{
			current = new Table(line.replace(/:$/, '').trim());
			tables.push(current);
		}
		//else	console.log('empty: ' + line);
	}

	var lines = data.trim().split('\n');
	
	tables = [];
	current = null;
	
	lines.map(parse_line);
	
	console.log(tables);
}

function Dependency(who, where, what)	{
	this.object = who;
	this.subject = what;
	this.context = where;
}

function Table(name, fields)	{
	this.name = name;
	this.fields = fields || [];
}

Table.prototype.getFieldByName = function(name)	{
	return this.fields.filter(function(field){return field.name == name})[0];
}

Table.prototype.findFieldsByName = function(name)	{
	return this.fields.filter(function(field){return field.name.indexOf(name) != -1});
}

function Field(table, name, type, reference, options)	{
	this.table = table;
	this.name = name;
	this.type = type;
	this.ref = reference;
	this.options = options;
}