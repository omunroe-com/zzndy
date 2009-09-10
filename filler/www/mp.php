<?php
header('Content-type: text/html');

require_once '../includes/FillerStarter.php';
require_once '../includes/FillerJoiner.php';
require_once '../includes/FillerPlayer.php';

$request_valid = false;
$action = $_GET['a'];

if($action == 'start' && isset($_GET['f']))
{
    list($w, $h, $f) = split('-', $_GET['f']);

    $starter = new FillerStarter();
    $starter->start($w, $h, $f);

$request_valid = true;
}
else if($action == 'join' && isset($_GET['k']))
{
    $code = $_GET['k'];

    $joiner = new FillerJoiner();
    $joiner->join($code);

$request_valid = true;
}

if(!$request_valid)
{
    header("HTTP/1.0 403 Forbidden");
    exit();
}
