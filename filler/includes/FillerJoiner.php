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
    // Read game
        $this->code = $code;
        list($w, $h, $f) = $this->readGame();

        // Report game configuration
        $this->post('reportGame', $w, $h, $f);
            Comet::log('Got this $f:' . $f);

        // Select first
        $joinerFirst = rand_bool();
        $actor = $joinerFirst ? 'joiner' : 'starter';

        // Write acceptance
        $this->write("ACCEPT");
        $this->write("FIRST\t$actor");
        $this->commit();

        // Report first move
        $this->post('reportFirst', $joinerFirst ? 'us' : 'them');
    }

    private function readGame()
    {
        try{
            $this->init('r+');
        }
        catch(FillerFileException $ex)
        {
            throw new GameNotFoundException($this->code);
        }

        list($ts, $dum) = $this->read(); // {time} START
        list($ts, $w, $h, $f) = $this->read(); // {time} w h f

        $f = strrev($f);

        return array($w, $h, $f);
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
