<?php

class Comet
    {
    private static $header = '<html><body>';
    private static $footer = '</body></html>';
    private static $start = '<script type="text/javascript">';
    private static $end = '</script>';

    public static function push($js)
    {
        static $headers_send;

        if(!$headers_send)
        {
            echo self::$header;
            register_shutdown_function('Comet::close');
            $headers_sent = true;
        }

        echo self::$start . $js . self::$end;
        flush();
    }

    public static function close()
    {
        echo self::$footer;
    }
}
