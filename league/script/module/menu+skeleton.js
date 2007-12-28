$.get(docroot + 'template/menu', function(data){
	league.body.innerHTML = data.process(initData);
})
