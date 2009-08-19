<?php
//header('Content-type: text/plain');

class FillerGame
    {
    protected static function makeCode()
    {
        // Create a code of five characters composed of digits and lowercase letters
        // except for zero and letter 'o'
        $code = base_convert(mt_rand(1336336, 45435423), 10, 34);
        return str_replace(array('0', 'o'), array('y', 'z'), $code);
    }

    protected static function getFileName($code)
    {
        return     sys_get_temp_dir()    . '7col-' . $code . '.game';
    }

    private $code = null;
    private $fd = -1;
    private $fname = '';

    public function __construct($code = null)
    {
        if(!is_null($code))
        $this->code = $code;
    }

    public function getCode()
    {
        if(is_null($this->code))
        {
            $this->code = FillerGame::makeCode();
            $this->fname = FillerGame::getFileName($this->code);
            $this->fd = fopen($this->fname, 'r+');
        }

        return $this->code;
    }

    public function mtime()
    {
        clearstatcache();
        return filemtime($this->fname);
    }
}

$headers_sent = false;

// Send given JavaScript code via comet pipe.
function push_js($js)
{
    global $headers_sent;

    if(!$headers_sent)
    {
        echo '<html><body>';
        register_shutdown_function('closebody');
        $headers_sent = true;
    }

    echo '<script type="text/javascript">' . $js . '</script>';
    flush();
}

function closebody()
{
    echo '</body>';
}

$action = $_GET['a'];
if($action == 'start')
start_mp();
else if($action == 'join' && isset($_GET['k']))
join_mp($_GET['k']);

function start_mp()
{
    $fg = new FillerGame();
    push_js("top.reportCode('{$fg->getCode()}');");

    $mtime = $fg->mtime();
    do
    {
        sleep(1);
        $time = $fg->mtime();
    }
    while(!($time>$mtime) );
}

function join_mp($code)
{

}