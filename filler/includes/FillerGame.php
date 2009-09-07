<?php

//32x24


class FillerGame {
    protected static function makeCode()
    {
    // Create a code of five characters composed of digits and lowercase letters
    // except for zero and letter 'o'
        $code = base_convert(mt_rand(1336336, 45435423), 10, 34);
        return str_replace(array('0', 'o'), array('y', 'z'), $code);
    }

    private static function getFileName($code)
    {
        return sys_get_temp_dir() . '7col-' . $code . '.game';
    }

    protected static $minw = 10;
    protected static $maxw = 45;
    protected static $minh = 5;
    protected static $maxh = 25;

    private $code = null;
    private $fd = FALSE;
    private $fname = '';
    private $mtime;

    private function init($mode)
    {
        $this->fname = FillerGame::getFileName($this->code);
        $this->fd = @fopen($this->fname, $mode);
        if($this->fd === FALSE)
        {
            throw new FillerException();
        }
    }

    public function __construct($code = null)
    {
        $this->mtime = time();

        if(!is_null($code))
        {
            $this->code = $code;
            $this->init('r+');
        }
    }

    /**
     * Destructor, closes file descriptor if any.
     *
     */
    public function __destruct()
    {
        if($this->fd !== FALSE)
        {
            fclose($this->fd);
        }
    }

    public function getCode()
    {
        if(is_null($this->code))
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

            if($this->fd == FALSE)
            {
                throw new FillerException();
            }

            $this->write('START');
        }

        return $this->code;
    }

    private function read()
    {
        return fgets($this->fd);
    }

    private function write($str)
    {
        fseek($this->fd, 0, SEEK_END);
        fwrite($this->fd, time() . "\t" . $str . "\n");
        fflush($this->fd);
        $this->mtime = $this->getMTime();
    }

    public function getMTime()
    {
        clearstatcache();
        return filemtime($this->fname);
    }

    public function begin()
    {
        $i_start = rand_bool();

        list($ts, $line) = split("\t", $this->read());
        list($ts, $w, $h, $f) = split("\t", $this->read());

        if(!$i_start) {
            if($ts == time())
            sleep(1);
            $this->write('PASS');
            Comet::log($this->getMTime());
            Comet::push("top.istart(false)");
        }
        else
        {
            Comet::push("top.istart(true)");
        }
    }

    public function setup($w, $h, $f)
    {
        if(FillerGame::dimensionsOk($w, $h))
        {
            $this->write("$w\t$h\t$f");
        }
        else
        {
            throw new FillerException();
        }
    }

    public function wait()
    {

        while($this->mtime >= $this->getMTime())
        {
            Comet::log('failed: '.$this->mtime.' >= '.$this->getMTime() );
            sleep(1);
        }

        Comet::log('No more wating');
    }

    public function enter()
    {
        Comet::log('start');
    }

    public function cleanup()
    {
        unlink($this->fname);
    }
}

function rand_bool($chance = 50) {
    return (rand(1,100) <= $chance);
}

class FillerException extends Exception {
    public function __construct($message)
    {
        parent::__construct($message);
    }
}


