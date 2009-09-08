<?php

require_once 'comet.php';

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

    protected $code = null;
    protected $fd = FALSE;
    private $fname = '';
    private $mtime;
    private $buffer = array();

    /**
     *  Create or open game file.
     *  @param {string} $mode file open mode
     *  @throw FillerException if file could not be opened
     */
    protected function init($mode)
    {
        $this->fname = FillerGame::getFileName($this->code);
        $this->fd = @fopen($this->fname, $mode);

        if($this->fd === FALSE)
        {
            throw new FillerFileException($this->code, $mode);
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
        return $this->code;
    }

    /**
     *  Read next line from game file
     *  @return {array} next line from file as array of tab separated values.
     */
    protected function read()
    {
        return split("\t", stream_get_line($this->fd, 8096, "\n"));
    }

    /**
     *  Store a line into buffer for later write into file.
     *  @param {string} $str line to write to file
     */
    protected function write($str)
    {
        $this->buffer[] = $str;
    }
    /**
     *  Commit pending write changes into the file.
     */
    protected function commit()
    {
        if(count($this->buffer) > 0)
        {
            fseek($this->fd, 0, SEEK_END);
            foreach($this->buffer as $line)
            {
                fwrite($this->fd, time() . "\t" . $line . "\n");
            }
            fflush($this->fd);

            $this->mtime = $this->getMTime();
            $this->buffer = array();
            sleep(1); // One way of ensuring synchronization.
        }
    }

    /**
     * Call client method. Add optional arguments
     * @param {string} $method name of method on the client
     */
    protected function post($method)
    {
        $args = func_get_args();
        $callStr = "top.$method(";
        if(array_shift($args) !== NULL)
        {
            $glue = '';
            foreach($args as $arg)
            {
                $callStr .= $glue;
                if(is_string($arg))
                {
                    $callStr .= '"' . addslashes($arg) . '"';
                }
                else
                {
                    $callStr .= $arg;
                }
                $glue = ',';
            }
        }
        $callStr .= ')';
        Comet::push($callStr);
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

class FillerFileException extends FillerException{
    public function __construct($code, $mode = null)
    {
        $action = 'open';
        if($mode == 'x+')
        {
            $action = 'create';
        }
        parent::__construct("Unable to $action file with code $code.");
    }
}


