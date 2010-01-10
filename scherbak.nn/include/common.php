<?php

define('PASS_EMPTY', 'PASS_EMPTY');
define('PASS_MISMATCH', 'PASS_MISMATCH');
define('NAME_EMPTY', 'NAME_EMPTY');
define('USER_EXISTS', 'USER_EXISTS');
define('EMAIL_INVALID', 'EMAIL_INVALID');

$lang = array(
	PASS_EMPTY => '������ �� ���� ���� ������.',
	PASS_MISMATCH => 'ϳ����������� ������ �� �������� � �������.',
	NAME_EMPTY => '�������, ���������, ��`� � �������.',
	USER_EXISTS => '���������� � ����� ������� ��� �������������.',
	EMAIL_INVALID => '������, ���������, ���� ������ ������������.'
);

function print_error($message)
{
    echo '<div class="error">' . $lang[$message] . '</div>'; 
}
