<?php
header('Content-type: text/html');

define(INCL, '../includes/');

require_once INCL . 'FillerGame.php';
require_once INCL . 'comet.php';
require_once INCL . 'multiplayer-logic.php';


$request_valid = false;
$action = $_GET['a'];
if($action == 'start' && isset($_GET['f']))
{
    list($w, $h, $f) = split('-', $_GET['f']);
    $request_valid = start_multiplayer($w, $h, $f);
}
else if($action == 'join' && isset($_GET['k']))
{
    $code = $_GET['k'];
    $request_valid = join_multiplayer($code);
}

if(!$request_valid)
{
    header("HTTP/1.0 403 Forbidden");
    exit();
}
