/**	Text ticker class. 
 *	Visually pump specified text to given HTML element
 */
(function(){
	var defaults = {
		loops: 3,
		delay: 10
	}
	
	function tick(n)	{
		var len = n / (this.settings.loops < 1 ? 1 : this.settings.loops)
		
		this.target[this.targetFld] = 
			len == this.value.length ?
			this.value :
			this.settings.loops < 1 ? this.value.substr(0, len) + '&#x258d;' :
			this.value.substr(0, len) + 
				String.fromCharCode(Math.random() * 25 + 65 + 36*(Math.random()>0.5?1:0))
			
		
		if(this.targetFld == 'innerHTML') this.target.innerHTML = this.target.innerHTML.replace(/\n/g, '<br />')
		
		if(len != this.value.length)	{
			var self = this
			this.timeout = window.setTimeout(function(){tick.call(self, ++n)}, this.settings.delay)
		}
	}

	TextTicker = function(target, value, settings)	{
		if(!target || !(('innerHTML' in target) || ('value' in target)) || typeof value != 'string')
			throw new Error('Wrong argument types')
			
		this.targetFld = ('value' in target) ? 'value' : 'innerHTML'
		this.target = target
		this.value = value
		this.settings = {}
		this.timeout
		
		for(var i in defaults)
			if(settings && i in settings) this.settings[i] = settings[i]
			else this.settings[i] = defaults[i]
	}
	
	
	TextTicker.prototype.start = function()	{
		var self = this
		this.timeout = window.setTimeout(function(){tick.call(self, 0)}, this.settings.delay)
	}
	
	TextTicker.prototype.stop = function()	{
		window.clearTimeout(this.timeout)
	}
})()
