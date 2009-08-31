\<?php
/**
 * Created by IntelliJ IDEA.
 * User: Andriy_Vynogradov
 * Date: Aug 31, 2009
 * Time: 5:25:39 PM
 * To change this template use File | Settings | File Templates.
 */

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
