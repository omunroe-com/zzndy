<?php

require_once 'FillerGame.php';
require_once 'comet.php';

/**
 * @param {int} $w    - field width
 * @param {int} $h    - field height
 * @param {string} $f - encoded field configuration
 */
function start_multiplayer($w, $h, $f)
{
    try{
        $fl = new FillerGame();


        try{
            $fl->setup($w, $h, $f);
        }
        catch(FillerException $ex)
        {
            $fl->cleanup();
            return false;
        }

        Comet::push("top.reportCode('{$fl->getCode()}')");

        $fl->wait();
        $fl->enter();
    }
    catch(FillerException $ex)
    {
        Comet::push("top.nogame()");
    }

    return true;
}

function join_multiplayer($code)
{
    try{
        $fl = new FillerGame(strtolower($code));
        $fl->begin();
    //$fl->wait();
    }
    catch(FillerException $ex)
    {
        Comet::push("top.nogame('$code')");
    }

    return true;
}