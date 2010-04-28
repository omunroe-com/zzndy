
var status_msg = new Object();

status_msg['new_game'] = 'Begin a new game.';
status_msg['game'] = 'Game field - make your guess.';
status_msg['options'] = 'Access game options here.';
status_msg['statistics'] = 'Information about the game and browser.';
status_msg['help'] = 'Get the help for codebreaker.';

function mouse_over(object, show)
{
	if(object.id == 'game' && !gamon) object.className = 'overgrayed';
	else if(object == selection || object.id == language);
	else object.className = 'over';

	if(show)hint(object, 'help_status');	
}

function mouse_out(object, show)
{
	if(object.id == 'game' && !gamon) object.className = 'grayed';
	else if(object == selection || object.id == language) object.className = 'selected';
	else object.className = '';
	
	if(show)clear_hint('help_status');
}
