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
							'<p>This means that if you get 0:0 – there are no such symbols in the code at all. '+
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
ru['lang_name'] = 'Русский';
ru['nickname'] = 'Неизвестный';

ru['text'] = new Object();
ru['text']['new_game']	= 'Новая игра ...';
ru['text']['game']		= 'И<u>г</u>ровое поле';
ru['text']['options']	= 'Настр<u>о</u>йки';
ru['text']['statistics']= '<u>С</u>татистика';
ru['text']['help']		= 'Помощь';
ru['text']['tries']		= 'К-во попыток:';
ru['text']['symbols']	= 'Всего букв:';
ru['text']['lenght']	= 'Длина шифра:';
ru['text']['nickname']	= 'Псевдоним:';
ru['text']['name']		= '&#x25ca; Мыслитель &#x25ca;';
ru['text']['game_opt']	= 'Игровые настройки';
ru['text']['other_opt']	= 'Разное';
ru['text']['help_opt']	= 'Помощь';
ru['text']['guess_gm']	= 'Текущая попытка';
ru['text']['history_gm']= 'Предыдущие попытки';
ru['text']['controls_gm']= 'Управление';
ru['text']['tries_left']= 'Осталось попыток:';
ru['text']['language']	= 'Языки';
ru['text']['hud']		= 'Настройки интерфейса';
ru['text']['anim']		= 'Анимация';
ru['text']['cook']		= 'Использовать куки';
ru['text']['game_info']		= 'Игровая информация';
ru['text']['tries_nfo']		= 'Всего попыток:';
ru['text']['symbols_nfo']	= 'Букв в коде:';
ru['text']['lenght_nfo']	= 'Длинна кода:';
ru['text']['combinations_nfo']= 'Возможных вариантов:';
ru['text']['progress_nfo']	= 'Отсеивать за шаг:';
ru['text']['browser_info']	= 'Информация о браузере';
ru['text']['appCodeName']	= 'Кодовое имя:';
ru['text']['appName']		= 'Приложение:';
ru['text']['userAgent']		= 'Клиент:';
ru['text']['cookieEnabled']	= 'Поддержка куки:';
ru['text']['install_date']	= 'Дата установки мыслителя:';
ru['text']['copyright']		= '&copy; А.Виноградов 2005.';
ru['text']['about']			= '	<h1>Помощь по Мыслителю</h1>'+
								'<p><b>Мыслитель</b> - занимательная головоломка. Хотя в нее можно играть и на бумаге,'+
								'существует множество воплощений этой игры - и на компьютерах и в качестве настольной игры.</p>'+
								'<h2>Цель игры</h2>'+
								'<p>Цель игры состоит в том, чобы отгадать код. Компъютер составляет этот код из оговоренного'+
								'количества букв. Чем быстрее Вам удастся отгадатькод, тем лучше.</p>'+
								'<h2>Правила</h2>'+
								'<p>После того, как Вы предложили компъютеру свой вариант тайного пароля он выдяет в ответ'+
								'два числа. Первое из них сообщает, сколько символов находятся на своих местах. Второе число '+
								'показывает количество символов, которые присутствуют в секретном слове, но расположены Вами '+
								'не на тех позициях.</p>'+
								'<p>Это значает, что если вы получили ответ 0:0, это значит, что тех символов, которые Вы'+
								'использовали в коде нет. А если разгадывая код из четырех букв Вы получили ответ 4:0, значит'+
								'Вы победили</p>';

ru['btns'] = new Object();
ru['btns']['start']		= 'Начать игру';
ru['btns']['reset']		= 'Сброс';
ru['btns']['end_game']	= 'Сдатся';

ru['win'] = new Object();
ru['win']['status'] = 'Победа!';
ru['win']['alert'] = 'Поздравляю, Вы победиль!.';

ru['loose'] = new Object();
ru['loose']['status'] = 'Поражение.';
ru['loose']['alert'] = 'К сожалению, Вы проиграли.';

ru['help'] = new Object();
ru['help']['new_game']	= 'Начните игру здесь.';
ru['help']['game']		= 'Игровое поле.';
ru['help']['options']	= 'Настройки языка и интерфейса.';
ru['help']['statistics']= 'Информайия об игре и браузере.';
ru['help']['help']		= 'Помощь <b>Мыслителе</b>.';
ru['help']['start']		= 'Начать <u>н</u>щвую игру. Достаточно нажать <b>Shift+N</b>.';
ru['help']['reset']		= 'Сброс настроек новой игры в значения по умолчянию.';
ru['help']['nickname']	= 'Введите Ваше имя.';
ru['help']['lenght']	= 'Выберите длинну кода.';
ru['help']['tries']		= 'Введите к-во попыток, с которых Вы угадаете шифр.';
ru['help']['symbols']	= 'Введите к-во букв в шифре.';

var uk = new Object();
uk['lang_name'] = 'Українська';
uk['nickname'] = 'Невідомий';

uk['text'] = new Object();
uk['text']['new_game']	= 'Нова гра ...';
uk['text']['game']		= 'І<u>г</u>рова таблиця';
uk['text']['options']	= 'Налаштування';
uk['text']['statistics']= '<u>С</u>татистика';
uk['text']['help']		= 'Допомога';
uk['text']['tries']		= 'Кількіст спроб:';
uk['text']['symbols']	= 'Можливих літер:';
uk['text']['lenght']	= 'Довжина шифра:';
uk['text']['nickname']	= 'Ім`я:';
uk['text']['name']		= '&#x2022; Мислитель &#x2022;';
uk['text']['game_opt']	= 'Налаштування гри';
uk['text']['other_opt']	= 'Різне';
uk['text']['help_opt']	= 'Допомога';
uk['text']['guess_gm']	= 'Варіант';
uk['text']['history_gm']= 'Історія спроб';
uk['text']['controls_gm']= 'Керування';
uk['text']['tries_left']= 'Залишилося спроб:';
uk['text']['language']	= 'Мови';
uk['text']['hud']		= 'Нлаштування інтерфейсу';
uk['text']['anim']		= 'Анімація';
uk['text']['cook']		= 'Використання куків';
uk['text']['game_info']		= 'Інформація про гру';
uk['text']['tries_nfo']		= 'Кількість спроб:';
uk['text']['symbols_nfo']	= 'Можливих літер:';
uk['text']['lenght_nfo']	= 'Довжина коду:';
uk['text']['combinations_nfo']= 'Можливо варіантів:';
uk['text']['progress_nfo']	= 'Треба відкидати за крок:';
uk['text']['browser_info']	= 'Інформація про переглядач';
uk['text']['appCodeName']	= 'Кодовое ім`я:';
uk['text']['appName']		= 'Програма:';
uk['text']['userAgent']		= 'Переглядач:';
uk['text']['cookieEnabled']	= 'Підтримка куків:';
uk['text']['install_date']	= 'Дата встановлення мислителя:';
uk['text']['copyright']		= '&copy; А.Виноградов 2005.';
uk['text']['about']			= '<h1>Допомога по Мислителю</h1>'+
								'<p>Мислитель - цікава гра для розуму.</p>'+
								'<h2>Завдання</h2>'+
								'<p>Завдання гравця полягає в тому, щоб відгадати шифр, що його загадує комп`ютер. '+
								'Шифр складається з певної кількості символів, кожен із яких обирається з перших '+
								'N літер англійського алфавіту. Оскільки Ви маєте обмежену кількісь спроб, краще, '+
								'щоб Ви розгадали його якнайшвидше.</p>'+
								'<h2>Хід гри</h2>'+
								'<p>Під час гри Ви пропонуєте Ваші варіанти шифру. При цьому ви отримуєте підказку, '+
								'яка складається з двох чисел. Перше число сповіщає про кількість літер, що поставлені '+
								'Вами на свої місця. Друге число дорівнює кількості літер, які присутні у шифрі, але '+
								'які не стоять на своіх позиціях.</p>'+
								'<p>Тобто, якщо Ви отримали підказку 0:0, то це означає, що ви не вгадали жодної літери. '+
								'Ця підказка дуже корисна, оскільки після неї Ви знаєте, які літери можна не використовувати. '+
								'Підказка 0:4 сповіщає про те, що Ви вгадали усі літери, але жодної не постваили на своє місце.'+ 
								'Якщо Ви отримали підказку 4:0, розгадуючи шифр із чотирьох літер, значить Ви перемогли.</p>';

uk['btns'] = new Object();
uk['btns']['start']		= 'Розпочати гру';
uk['btns']['reset']		= 'Зкинути';
uk['btns']['end_game']	= 'Закінчити гру';

uk['win'] = new Object();
uk['win']['status'] = 'Перемога!';
uk['win']['alert'] = 'Вітання, Ви перемогли!.';

uk['loose'] = new Object();
uk['loose']['status'] = 'Поразка.';
uk['loose']['alert'] = 'Ви програли, нажаль...';

uk['help'] = new Object();
uk['help']['new_game']	= 'Розпочніть нову гру у цьому меню.';
uk['help']['game']		= 'Власне гра розташована тут.';
uk['help']['options']	= 'Налаштування інтерфейсу, мова та ін.';
uk['help']['statistics']= 'Інформація про гру та про переглядач.';
uk['help']['help']		= 'Допомога по правилам Мислителя.';
uk['help']['start']		= 'Розпочати гру. Достатньо натиснути <b>Shift+N</b>.';
uk['help']['reset']		= 'Зкинути всі налаштування у їх первинні значення.';
uk['help']['nickname']	= 'Ваше ім`я.';
uk['help']['lenght']	= 'Задати довжину шифру.';
uk['help']['tries']		= 'Задати кількість спроб.';
uk['help']['symbols']	= 'Задати, зі скількох можливих яких складається шифр.';


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
