<?php

require_once 'FillerGame.php';
require_once 'comet.php';

function start_multiplayer()
{
    try{
        $fl = new FillerGame();
        Comet::push("top.reportCode('{$fl->getCode()}')");

        $fl->wait();
        $fl->enter();
    }
    catch(FillerException $ex)
    {
        Comet::push("top.nogame()");
    }
}

function join_multiplayer($code)
{
    try{
        $fl = new FillerGame(strtolower($code));

        $fl->begin();
        $fl->wait();
    }
    catch(FillerException $ex)
    {
        Comet::push("top.nogame('$code')");
    }
}