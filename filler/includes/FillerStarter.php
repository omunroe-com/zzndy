<?php

require_once 'FillerGame.php';

/**
 *  Starts filler game.
 */
class FillerStarter extends FillerGame {
    /**
     *  Start a game
     *  @param {int} $w - game width
     *  @param {int} $h - game height
     *  @param {int} $f - field configuration

     *  @throw InvalidDimensionException if width or height is invalid.
     */
    public function start($w, $h, $f)
    {
        if(!FillerStarter::dimensionsOk($w, $h))
        throw new InvalidDimensionException($w, $h);
    }

    /**
     * Check if dimensions are ok.
     * @param {int} $w
     * @param {int} $h
     * @return {bool}
     */
    protected static function dimensionsOk($w, $h)
    {
        return $w>=FillerGame::$minw && $w <= FillerGame::$maxw && $h>=FillerGame::$minh && $h <= FillerGame::$maxh;
    }
}

/**
 *  Notes invalid dimension of a proposed game.
 */
class InvalidDimensionException extends FillerException {
    public function __construct($w, $h)
    {
        parent::__construct("Cannot start a {$w}x{$h} game.");
    }
}

