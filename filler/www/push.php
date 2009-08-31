<?php
define(INCL, '../includes/');

require_once INCL . 'FillerGame.php';
require_once INCL . 'comet.php';
require_once INCL . 'multiplayer-logic.php';


$action = $_GET['a'];
if($action == 'start')
{
    start_multiplayer();
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

//function start_mp()
//{
//    $fg = new FillerGame();
//    Comet::push("top.reportCode('{$fg->getCode()}');");
//
//    $mtime = $fg->mtime();
//    do
//    {
//        sleep(1);
//        $time = $fg->mtime();
//    }
//    while(!($time>$mtime) );
//}

function join_mp($code)
{

}