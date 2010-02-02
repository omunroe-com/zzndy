<?php

session_start();

require_once '.private/config.php';

$path = $_GET['path'];

if($path == '')	{
	$lang = $_SESSION['lang'];
	if(!in_array($lang, $languages))
		$lang = substr($_SERVER['HTTP_ACCEPT_LANGUAGE'], 0, 2);

	if(!in_array($lang, $languages))
		$lang = $config['default-lang'];

	header('Location: /' . $lang . '/');
	exit();
}

$split = strpos($path, '/');
if($split === FALSE)
	$split = count($path) + 1;

$lang = substr($path, 0, $split);
if(!in_array($lang, $languages))
	$lang = $config['default-lang'];

$file = substr($path, $split);
if($file == '')
	$file = 'index';

if(file_exists('.public/' . $lang . '/' . $file . '.html'))
{
	include '.public/' . $lang . '/' . $file . '.html';
	exit();
}

echo file_exists('.public');

echo 'Path: ';
echo $lang . ' and ' . $file;

