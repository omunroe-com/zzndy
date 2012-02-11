<?php
header('Content-type: application/json');

define('CLASSES_DIR', dirname(dirname(__FILE__)) . '/inc/');

function __autoload($class)	{
	require_once(CLASSES_DIR . $class . '.php');
}

$top10k = new Top10k($_POST);

$position = $top10k->placeCurrent();

echo $top10k->toJson();
