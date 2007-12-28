var _modules = {}

function loadModule(name, callback)	{
	if(!_modules[name])	{
		$.getScript(docroot + 'script/module/' + name + '.js', function(){_modules[name] = true; if(callback)callback()});
	}
	else if (callback) callback();
}

function moduleLoaded()	{
	
}