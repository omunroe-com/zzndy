<?php

session_start();

require_once '.private/config.php';

$path = $_GET['path'];

function set_lang()
{
	return 'uk';

	global $languages;
	$lang = $_SESSION['lang'];
	if($lang == null || !in_array($lang, $languages))
		$lang = substr($_SERVER['HTTP_ACCEPT_LANGUAGE'], 0, 2);

	if($lang == null || !in_array($lang, $languages))
		$lang = $config['default-lang'];

	$_SESSION['lang'] = $lang;
	return $lang;
}

// redirect to default language index page
if($path == '')	{
	$lang = set_lang();
	header('Location: /' . $lang . '/');
	exit();
}

$split = strpos($path, '/');
if($split === FALSE)
	$split = count($path) + 1;

$lang = substr($path, 0, $split);
if(!in_array($lang, $languages))	{
	$lang = set_lang();
}
else {
	$_SESSION['lang'] = $lang;
}

$file = substr($path, $split + 1);
if($file == '')
	$file = 'index';

// protect access to private part with password	
if(substr($file, 0, 4) == 'arch' && $file != 'arch' && $file != 'register' && !isset($_SESSION['user']))
{
	$_SESSION['goto'] = $path;
	header('Location: /'.$lang.'/arch');
	exit();
}	

if(file_exists('.public/' . $lang . '/' . $file . '.php'))
{
	include '.public/' . $lang . '/' . $file . '.php';
	exit();
}

if(file_exists('.public/' . $lang . '/' . $file . '.html'))
{
	include '.public/' . $lang . '/' . $file . '.html';
	exit();
}

header("HTTP/1.0 404 Not Found");
