<?php

require_once 'FillerGame.php';
require_once 'comet.php';

/**
 * @param {int} $w    - field width
 * @param {int} $h    - field height
 * @param {string} $f - encoded field configuration
 * @return
 *
 */
function start_multiplayer($w, $h, $f)
{
    try{
        $fl = new FillerGame();
        Comet::push("top.reportCode('{$fl->getCode()}')");

        // TODO: Check that $w and $h are in range
        $fl->setup($w, $h, $f);
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