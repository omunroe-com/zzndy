<?php
require_once 'config.php';

$db = new mysqli('localhost', $config['user'], $config['pass'], $config['db']);

if (mysqli_connect_errno()) {
    printf("Connect failed: %s\n", mysqli_connect_error());
    exit();
}

