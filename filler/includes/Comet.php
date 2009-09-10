<?php

class Comet
    {
    private static $header = '<html><body>';
    private static $footer = '</body></html>';
    private static $start = '<script type="text/javascript">';
    private static $end = '</script>';
    private static $headers_sent = false;

    public static function push($js)
    {
        if(!self::$headers_sent)
        {
            echo self::$header;
            register_shutdown_function('Comet::close');
            self::$headers_sent = true;
        }

        echo self::$start . $js . self::$end;
        flush();
    }

    public static function log($text)
    {
        self::push('console.log("'.str_replace("\n", '\n', $text).'")');
    }

    public static function close()
    {
        echo self::$footer;
    }
}
