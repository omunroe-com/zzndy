<?php
if($_SERVER['DOCUMENT_ROOT']) exit(1);

define('CLASSES_DIR', dirname(dirname(__FILE__)) . '/inc/');
define('DATA_DIR', dirname(dirname(__FILE__)) . '/data/');

function __autoload($class)	{
	require_once(CLASSES_DIR . $class . '.php');
}

$table = unserialize(file_get_contents(DATA_DIR . $argv[1] . '.dat'));
echo 'POS|', Record::Columns(), "\n";

foreach($table as $i => $record)	{
	if($i < 20)
		echo $i+1, '|', $record->report(), "\n";
	else
		break;
}
