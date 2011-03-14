<?php

session_start();

require_once '.private/config.php';

$path = $_GET['path'];

// redirect to deafult language index page
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

$file = substr($path, $split + 1);
if($file == '')
	$file = 'index';

if(file_exists('.public/' . $lang . '/' . $file . '.html'))
{
	include '.public/' . $lang . '/' . $file . '.html';
	exit();
}

if($file == 'arch')
{
	echo 'YEAAAHHH';
	exit();
}

//echo '\'' . $lang . '\' and \'' . $file . '\'';

header("HTTP/1.0 404 Not Found");
