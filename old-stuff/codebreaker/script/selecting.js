
var selection = null;

function deactivate(id)
{
	document.getElementById(id).className = 'grayed';
}
function activate(id)
{
	document.getElementById(id).className = '';
}

function deselect()
{
	if(selection===null)return;
	if(selection.id == 'game' && !gamon ) {
		selection.className = 'grayed';
	}else selection.className = '';
	document.getElementById(selection.id+'_div').className = 'hidden';//style.zIndex = 0;
}

function select(object)
{
	selection = object;
	if(object.id == 'game' && !gamon) {
		selection = document.getElementById('new_game');
	}
	selection.className = 'selected';
	document.getElementById(selection.id+'_div').className = 'visible';//style.zIndex = 100;
}

function change_selection(id)	
{
	deselect();
	select(document.getElementById(id));	
}
