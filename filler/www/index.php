<?php
header ('Content-type: text/plain');

function getFileName()
{
    return sys_get_temp_dir() . 'chat-chat-';
}

if(isset($_POST['save']))
{
    $filename = getFileName() . time();
    $handle = fopen($filename, 'w');
    fwrite($handle, $_POST['save']);
    $fclose($handle);
}
else if(isset($_POST['read']))
{
    $mask = getFileName() . '*';
    $ts = $_POST['mtime'];
    $msgs = array();
    while(count($files=  glob($mask)) ==0)
    {
        usleep(500000);
    }

    foreach($files as $name)
    {
        list(,,$mtime) = split('-', $name);
        $ts = max($ts, $mtime);
        $handle = fopen($name, 'r');
        $text = fread($handle, filesize($name));
        fclose($handle);
        unlink($name);
        $msgs[] = $text;
    }

    echo '{text:["'.join('","', $msgs).'"]}';
}
