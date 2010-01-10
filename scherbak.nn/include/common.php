<?php

define('PASS_EMPTY', 'PASS_EMPTY');
define('PASS_MISMATCH', 'PASS_MISMATCH');
define('NAME_EMPTY', 'NAME_EMPTY');
define('USER_EXISTS', 'USER_EXISTS');
define('EMAIL_INVALID', 'EMAIL_INVALID');

$lang = array(
	PASS_EMPTY => 'Пароль не може бути пустим.',
	PASS_MISMATCH => 'Підтвердження пароля не спывпадає з паролем.',
	NAME_EMPTY => 'Ввведіть, будьласка, ім`я і прізвище.',
	USER_EXISTS => 'Користувач з такою адресою вже зареєструвався.',
	EMAIL_INVALID => 'Введіть, будьласка, діючу адресу електропошти.'
);

function print_error($message)
{
    echo '<div class="error">' . $lang[$message] . '</div>'; 
}
