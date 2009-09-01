<?php

require_once 'FillerGame.php';
require_once 'comet.php';

function start_multiplayer()
{
    $fl = new FillerGame();
    Comet::push("top.reportCode('{$fl->getCode()}')");
}

function join_multiplayer($code)
{
    $fl = new FillerGame($code);

    $fl->begin();
    $fl->wait();
}