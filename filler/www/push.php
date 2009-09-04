<?php
header('Content-type: text/html');

define(INCL, '../includes/');

require_once INCL . 'FillerGame.php';
require_once INCL . 'comet.php';
require_once INCL . 'multiplayer-logic.php';


$action = $_GET['a'];
if($action == 'start' && isset($_GET['f']))
{
    list($w, $h, $f) = split('-', $_GET['f']);
    start_multiplayer($w, $h, $f);
}
else if($action == 'join' && isset($_GET['k']))
{
    $code = $_GET['k'];
    join_multiplayer($code);
}
else
{
    header("HTTP/1.0 404 Not Found");
    exit();
}
