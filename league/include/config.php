<?php

# autoprepend onfig file

$_LEAGUE = array(
	'dsn' => 'mysql://root@localhost/league',
	'db-debug' => 2,
	'root' => '/',
	'stat' => array(
		'start' => microtime(true),
		'queries' => 0
	)
);

require_once 'data.php';
