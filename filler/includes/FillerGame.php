<?php

class FillerGame {
    protected static function makeCode()
    {
    // Create a code of five characters composed of digits and lowercase letters
    // except for zero and letter 'o'
        $code = base_convert(mt_rand(1336336, 45435423), 10, 34);
        return str_replace(array('0', 'o'), array('y', 'z'), $code);
    }

    protected static function getFileName($code)
    {
        return sys_get_temp_dir() . '7col-' . $code . '.game';
    }

    private $code = null;
    private $fd = -1;
    private $fname = '';
    private $mtime;

    private function init($mode)
    {
        $this->fname = FillerGame::getFileName($this->code);
        $this->fd = fopen($this->fname, $mode);
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

    public function __destruct()
    {
        fclose($this->fd);
    }

    public function getCode()
    {
        if(is_null($this->code))
        {
            $this->code = FillerGame::makeCode();
            $this->init('x+');

            $this->write('START');
        }

        return $this->code;
    }

    private function write($str)
    {
        fwrite($this->fd, time() . "\t" . $str . "\n");
        flush($this->fd);
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
        if(!$i_start)                 {
            $this->write('PASS');
            Comet::push("top.istart(false)");
        }
        else
        {
            Comet::push("top.istart(true)");
        }
    }

    public function wait()
    {
        while($this->mtime < $this->getMTime())
        {
            sleep(1);
        }
    }
}

function rand_bool($chance = 50) {
    return (rand(1,100) <= $chance);
}
