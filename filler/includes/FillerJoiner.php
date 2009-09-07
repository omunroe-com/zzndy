<?php

require_once 'FillerGame.php';

/**
 *  Joins filler game started by FillerStarter
 */
class FillerJoiner extends FillerGame {
    /**
     *  Join existing game
     * @param {string} $code game code.
     * @throw GameNotFoundException if no game with such code exist.
     */
    public function join($code)
    {

    }
}

/**
 *  Notes that the code given is occupied or does not exist.
 */
class GameNotFoundException extends FillerException    {
    public function __construct($code)
    {
        parent::__construct("Cannot join game with code $code.");
    }
}
