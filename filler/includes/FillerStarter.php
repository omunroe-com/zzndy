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
     *  @throw FillerFileException if unable to create file.
     */
    public function start($w, $h, $f)
    {
        // Check dimensions
        if(!FillerStarter::dimensionsOk($w, $h))
        {
            throw new InvalidDimensionException($w, $h);
        }

        // Create game file
        $this->createFile();

        // Setup game
        $this->write('START');
        $this->write("$w\t$h\t$f");
        $this->commit();

        // Report code
        $this->post('reportCode', $this->code);

        // Wait for accept/reject
        $this->wait();

        // Report accept/reject, first move.
        $starterFirst = $this->readAccept();
        $this->post('reportFirst', $starterFirst ? 'us' : 'them');
    }

    /**
     *  Read their responce, get who's making the first move.
     *  @return {bool} true if starter gets the first move
     */
    private function readAccept()
    {
        list($ts, $accept) =  $this->read(); // {time} ACCEPT
        list($ts, $dum, $first) =  $this->read();  // {time} FIRST {starter,joiner}

        return $first == 'starter';
    }

    /**
     *  Setup new file for new game. New code is allocated automatically.
     */
    private function createFile()
    {
        $i = 0;
        do{
            $this->code = FillerGame::makeCode();

            try{
                $this->init('x+');
            }
            catch(FillerException $ex)
            {
                $this->code = null;
                $this->fd = FALSE;
            }
        }
        while(++$i < 3 && $this->fd === FALSE);

        if($this->fd === FALSE)
        {
            throw new FillerFileException("Unable to initialize game {$this->code}.");
        }
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

