<?php
if($mdb2) $mdb2->disconnect();

$exec_time = microtime(true) - $_LEAGUE['stat']['start'];
$qs = $_LEAGUE['stat']['queries'];

echo "\n", '<!--execution time: ', $exec_time, ' seconds, ', $qs, $qs==1?' query':' queries', ' run -->';