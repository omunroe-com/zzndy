///////////////////////////////////////////////////////////////////////////////
//                                                                           //
//  Project: Codebreaker                                                     //
//  File:    lingua.js                                                       //
//  Comment: Contains language information.                                  //
//                                                                           //
///////////////////////////////////////////////////////////////////////////////

var language = 'en';

var en = new Object();
en['lang_name'] = 'English';
en['nickname'] = 'unnamed';

en['text'] = new Object();
en['text']['new_game']	= 'N<u>e</u>w&nbsp;game ...';
en['text']['game']		= '<u>G</u>ame';
en['text']['options']	= '<u>O</u>ptions';
en['text']['statistics']= '<u>S</u>tatistics';
en['text']['help']		= '<u>H</u>elp';
en['text']['tries']		= 'Tries:';
en['text']['symbols']	= 'Symbols:';
en['text']['lenght']	= 'Code lenght:';
en['text']['nickname']	= 'Your name:';
en['text']['name']		= '&#x066d; Codebreaker &#x066d;';
en['text']['game_opt']	= 'Game options';
en['text']['other_opt']	= 'Miscelaneous';
en['text']['help_opt']	= 'Help';
en['text']['guess_gm']	= 'Guess';
en['text']['history_gm']= 'History';
en['text']['controls_gm']= 'Controls';
en['text']['tries_left']= 'Tries left:';
en['text']['language']	= 'Languages';
en['text']['hud']		= 'Heads Up Display (HUD)';
en['text']['anim']		= 'Animation';
en['text']['cook']		= 'Use cookies';
en['text']['game_info']		= 'Game information';
en['text']['tries_nfo']		= 'Tries:';
en['text']['symbols_nfo']	= 'Symbols in use:';
en['text']['lenght_nfo']	= 'Code lenght:';
en['text']['combinations_nfo']= 'Total combinations:';
en['text']['progress_nfo']	= 'Progress required:';
en['text']['browser_info']	= 'Browser information';
en['text']['appCodeName']	= 'Application code:';
en['text']['appName']		= 'Application:';
en['text']['userAgent']		= 'Browser:';
en['text']['cookieEnabled']	= 'Cookies enabled:';
en['text']['install_date']	= 'Codebreaker installed:';
en['text']['copyright']		= '&copy; A.Vynogradov 2005.';
en['text']['about']			= '<h1>Help for Codebreaker</h1>'+
							'<p>Codebreaker is one of those good old mindbenders we have all played since '+
								'childhood. It had many names and variations an it is one of them.</p>'+
							'<h2>Rules</h2>'+
							'<p>Comupter selects a secret combination of symbols. It is composed from a number '+
								'of symbols you can select. The code might me even "AAAA" because multiple '+
								'repetitions of the same letter are alowed.</p>'+
							'<h4>Goal</h4>'+
							'<p>You task is to find the code. You take limited number of guesses. The sooner you '+
								'find your code the better. When you use all of your tries you lose. Each time '+
								'you try to guess the code, computer gives you a hint about its code.</p>'+
							'<h4>Hints</h4>'+
							'<p>The hint is based on your guess is composed of two numbers. First number tell '+
								'how many letters in your guess are on their respective positions. Second number '+
								'tells how many symbols are misplased.</p>'+
							'<p>This means that if you get 0:0 � there are no such symbols in the code at all. '+
								'That combination is very useful by the way. If you are guessing acode from 4 '+
								'symbols the hint 4:0 means victory.</p>';

en['btns'] = new Object();
en['btns']['start']		= 'Start game';
en['btns']['reset']		= 'Reset to defaults';
en['btns']['end_game']	= 'End game';

en['win'] = new Object();
en['win']['status'] = 'Victory.';
en['win']['alert'] = 'Congratulations.\nYou win.';

en['loose'] = new Object();
en['loose']['status'] = 'Defeat.';
en['loose']['alert'] = 'Sorry, You loose.';

en['help'] = new Object();
en['help']['new_game'] = 'Begin a new game.';
en['help']['game'] = 'Game field - make your guess.';
en['help']['options'] = 'Access game options here.';
en['help']['statistics'] = 'Information about the game and browser.';
en['help']['help'] = 'Get the help for <b>codebreaker</b>.';
en['help']['start']	= 'Begin a <u>n</u>ew game. Hotkey <b>Shift+N</b>.';
en['help']['reset']	= 'Reset all preferences to their default values.';
en['help']['nickname']	= 'Specify your name, please.';
en['help']['lenght']	= 'Specify the lenght of the code.';
en['help']['tries']	= 'Specify number of tries for guessing the code.';
en['help']['symbols']	= 'Specify the number of possible symbols in the code.';

var ru = new Object();
ru['lang_name'] = '�������';
ru['nickname'] = '�����������';

ru['text'] = new Object();
ru['text']['new_game']	= '����� ���� ...';
ru['text']['game']		= '�<u>�</u>����� ����';
ru['text']['options']	= '�����<u>�</u>���';
ru['text']['statistics']= '<u>�</u>���������';
ru['text']['help']		= '������';
ru['text']['tries']		= '�-�� �������:';
ru['text']['symbols']	= '����� ����:';
ru['text']['lenght']	= '����� �����:';
ru['text']['nickname']	= '���������:';
ru['text']['name']		= '&#x25ca; ��������� &#x25ca;';
ru['text']['game_opt']	= '������� ���������';
ru['text']['other_opt']	= '������';
ru['text']['help_opt']	= '������';
ru['text']['guess_gm']	= '������� �������';
ru['text']['history_gm']= '���������� �������';
ru['text']['controls_gm']= '����������';
ru['text']['tries_left']= '�������� �������:';
ru['text']['language']	= '�����';
ru['text']['hud']		= '��������� ����������';
ru['text']['anim']		= '��������';
ru['text']['cook']		= '������������ ����';
ru['text']['game_info']		= '������� ����������';
ru['text']['tries_nfo']		= '����� �������:';
ru['text']['symbols_nfo']	= '���� � ����:';
ru['text']['lenght_nfo']	= '������ ����:';
ru['text']['combinations_nfo']= '��������� ���������:';
ru['text']['progress_nfo']	= '��������� �� ���:';
ru['text']['browser_info']	= '���������� � ��������';
ru['text']['appCodeName']	= '������� ���:';
ru['text']['appName']		= '����������:';
ru['text']['userAgent']		= '������:';
ru['text']['cookieEnabled']	= '��������� ����:';
ru['text']['install_date']	= '���� ��������� ���������:';
ru['text']['copyright']		= '&copy; �.���������� 2005.';
ru['text']['about']			= '	<h1>������ �� ���������</h1>'+
								'<p><b>���������</b> - ������������� �����������. ���� � ��� ����� ������ � �� ������,'+
								'���������� ��������� ���������� ���� ���� - � �� ����������� � � �������� ���������� ����.</p>'+
								'<h2>���� ����</h2>'+
								'<p>���� ���� ������� � ���, ���� �������� ���. ��������� ���������� ���� ��� �� ������������'+
								'���������� ����. ��� ������� ��� ������� �����������, ��� �����.</p>'+
								'<h2>�������</h2>'+
								'<p>����� ����, ��� �� ���������� ���������� ���� ������� ������� ������ �� ������ � �����'+
								'��� �����. ������ �� ��� ��������, ������� �������� ��������� �� ����� ������. ������ ����� '+
								'���������� ���������� ��������, ������� ������������ � ��������� �����, �� ����������� ���� '+
								'�� �� ��� ��������.</p>'+
								'<p>��� �������, ��� ���� �� �������� ����� 0:0, ��� ������, ��� ��� ��������, ������� ��'+
								'������������ � ���� ���. � ���� ���������� ��� �� ������� ���� �� �������� ����� 4:0, ������'+
								'�� ��������</p>';

ru['btns'] = new Object();
ru['btns']['start']		= '������ ����';
ru['btns']['reset']		= '�����';
ru['btns']['end_game']	= '������';

ru['win'] = new Object();
ru['win']['status'] = '������!';
ru['win']['alert'] = '����������, �� ��������!.';

ru['loose'] = new Object();
ru['loose']['status'] = '���������.';
ru['loose']['alert'] = '� ���������, �� ���������.';

ru['help'] = new Object();
ru['help']['new_game']	= '������� ���� �����.';
ru['help']['game']		= '������� ����.';
ru['help']['options']	= '��������� ����� � ����������.';
ru['help']['statistics']= '���������� �� ���� � ��������.';
ru['help']['help']		= '������ <b>���������</b>.';
ru['help']['start']		= '������ <u>�</u>���� ����. ���������� ������ <b>Shift+N</b>.';
ru['help']['reset']		= '����� �������� ����� ���� � �������� �� ���������.';
ru['help']['nickname']	= '������� ���� ���.';
ru['help']['lenght']	= '�������� ������ ����.';
ru['help']['tries']		= '������� �-�� �������, � ������� �� �������� ����.';
ru['help']['symbols']	= '������� �-�� ���� � �����.';

var uk = new Object();
uk['lang_name'] = '���������';
uk['nickname'] = '��������';

uk['text'] = new Object();
uk['text']['new_game']	= '���� ��� ...';
uk['text']['game']		= '�<u>�</u>���� �������';
uk['text']['options']	= '������������';
uk['text']['statistics']= '<u>�</u>���������';
uk['text']['help']		= '��������';
uk['text']['tries']		= 'ʳ����� �����:';
uk['text']['symbols']	= '�������� ����:';
uk['text']['lenght']	= '������� �����:';
uk['text']['nickname']	= '��`�:';
uk['text']['name']		= '&#x2022; ��������� &#x2022;';
uk['text']['game_opt']	= '������������ ���';
uk['text']['other_opt']	= 'г���';
uk['text']['help_opt']	= '��������';
uk['text']['guess_gm']	= '������';
uk['text']['history_gm']= '������ �����';
uk['text']['controls_gm']= '���������';
uk['text']['tries_left']= '���������� �����:';
uk['text']['language']	= '����';
uk['text']['hud']		= '����������� ����������';
uk['text']['anim']		= '�������';
uk['text']['cook']		= '������������ ����';
uk['text']['game_info']		= '���������� ��� ���';
uk['text']['tries_nfo']		= 'ʳ������ �����:';
uk['text']['symbols_nfo']	= '�������� ����:';
uk['text']['lenght_nfo']	= '������� ����:';
uk['text']['combinations_nfo']= '������� �������:';
uk['text']['progress_nfo']	= '����� �������� �� ����:';
uk['text']['browser_info']	= '���������� ��� ����������';
uk['text']['appCodeName']	= '������� ��`�:';
uk['text']['appName']		= '��������:';
uk['text']['userAgent']		= '����������:';
uk['text']['cookieEnabled']	= 'ϳ������� ����:';
uk['text']['install_date']	= '���� ������������ ���������:';
uk['text']['copyright']		= '&copy; �.���������� 2005.';
uk['text']['about']			= '<h1>�������� �� ���������</h1>'+
								'<p>��������� - ������ ��� ��� ������.</p>'+
								'<h2>��������</h2>'+
								'<p>�������� ������ ������ � ����, ��� �������� ����, �� ���� ������ ����`����. '+
								'���� ���������� � ����� ������� �������, ����� �� ���� ��������� � ������ '+
								'N ���� ����������� �������. ������� �� ���� �������� ������ �����, �����, '+
								'��� �� ��������� ���� �����������.</p>'+
								'<h2>ճ� ���</h2>'+
								'<p>ϳ� ��� ��� �� ��������� ���� ������� �����. ��� ����� �� �������� �������, '+
								'��� ���������� � ���� �����. ����� ����� ������ ��� ������� ����, �� ��������� '+
								'���� �� ��� ����. ����� ����� ������� ������� ����, �� ������� � ����, ��� '+
								'�� �� ������ �� ���� ��������.</p>'+
								'<p>�����, ���� �� �������� ������� 0:0, �� �� ������, �� �� �� ������� ����� �����. '+
								'�� ������� ���� �������, ������� ���� �� �� �����, �� ����� ����� �� ���������������. '+
								'ϳ������ 0:4 ������ ��� ��, �� �� ������� �� �����, ��� ����� �� ��������� �� ��� ����.'+ 
								'���� �� �������� ������� 4:0, ���������� ���� �� �������� ����, ������� �� ���������.</p>';

uk['btns'] = new Object();
uk['btns']['start']		= '��������� ���';
uk['btns']['reset']		= '�������';
uk['btns']['end_game']	= '�������� ���';

uk['win'] = new Object();
uk['win']['status'] = '��������!';
uk['win']['alert'] = '³�����, �� ���������!.';

uk['loose'] = new Object();
uk['loose']['status'] = '�������.';
uk['loose']['alert'] = '�� ��������, ������...';

uk['help'] = new Object();
uk['help']['new_game']	= '��������� ���� ��� � ����� ����.';
uk['help']['game']		= '������ ��� ����������� ���.';
uk['help']['options']	= '������������ ����������, ���� �� ��.';
uk['help']['statistics']= '���������� ��� ��� �� ��� ����������.';
uk['help']['help']		= '�������� �� �������� ���������.';
uk['help']['start']		= '��������� ���. ��������� ��������� <b>Shift+N</b>.';
uk['help']['reset']		= '������� �� ������������ � �� ������� ��������.';
uk['help']['nickname']	= '���� ��`�.';
uk['help']['lenght']	= '������ ������� �����.';
uk['help']['tries']		= '������ ������� �����.';
uk['help']['symbols']	= '������, � ������� �������� ���� ���������� ����.';


var lang_col = new Object();
lang_col['en'] = en;
lang_col['ru'] = ru;
lang_col['uk'] = uk;

function load_language(lng)
{
	for(var i in lang_col[lng]['text'])	{
		document.getElementById(i+'_text').innerHTML = lang_col[lng]['text'][i];
	}
	for(var i in lang_col[lng]['btns'])	{
		document.getElementById(i).innerHTML = lang_col[lng]['btns'][i];
	}
}

function select_lang(lang)
{
	if(lang.id == language)return;
	language = lang.id;
	load_language(language);
}
function deselect_lang(lang)
{
	if(lang.id == language)return;
	document.getElementById(language).className = '';
	lang.className = 'selected';
}
