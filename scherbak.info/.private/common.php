<?php
define('PASS_EMPTY',    'PASS_EMPTY'  );
define('PASS_MISMATCH', 'PASS_MISMATCH');
define('NAME_EMPTY',    'NAME_EMPTY'  );
define('USER_EXISTS',   'USER_EXISTS' );
define('EMAIL_INVALID', 'EMAIL_INVALID');
define('APPL_PENDING',  'APPL_PENDING');
define('INVALID_LOGIN',  'INVALID_LOGIN');
define('ACCESS_DENIED',  'ACCESS_DENIED');

$lang = 'uk';
require_once $include . $lang . '.php';

function print_message($message)
{
    echo '<div class="message">';
    $message();
    echo '</div>'; 
}

function print_error($message)
{
    echo '<div class="error">';
    $message();
    echo '</div>'; 
}

function wrapdate($date)
{
	return '<span class="date" title="' . $date . '">' . timesince(strtotime($date)) . ' ago</span>';
}

/* Works out the time since the entry post, takes a an argument in unix time (seconds)
*/
function timesince($original) {
    // array of time period chunks
    $chunks = array(
		array(60 * 60 * 24 * 365 , 'year'),
		array(60 * 60 * 24 * 30 , 'month'),
		array(60 * 60 * 24 * 7, 'week'),
		array(60 * 60 * 24 , 'day'),
		array(60 * 60 , 'hour'),
		array(60 , 'min'),
		array(1 , 'sec'),
    );

    $today = time(); /* Current unix time  */
    $since = $today - $original;

    // $j saves performing the count function each time around the loop
    for ($i = 0, $j = count($chunks); $i < $j; $i++) {

		$seconds = $chunks[$i][0];
		$name = $chunks[$i][1];

		// finding the biggest chunk (if the chunk fits, break)
		if (($count = floor($since / $seconds)) != 0) {
			break;
		}
    }

    $print = ($count == 1) ? '1 '.$name : "$count {$name}s";

    if ($i + 1 < $j) {
	// now getting the second item
	$seconds2 = $chunks[$i + 1][0];
	$name2 = $chunks[$i + 1][1];

	// add second item if its greater than 0
	if (($count2 = floor(($since - ($seconds * $count)) / $seconds2)) != 0) {
	    $print .= ($count2 == 1) ? ', 1 '.$name2 : " $count2 {$name2}s";
	}
    }

    return $print;
}

function send($email, $subj, $body)
{
	global $config;

	if($config['email-divert']) $email = $config['email-divert'];
	$headers = 'From: ' . $config['email'] . '\r\nX-Mailer: PHP for ' . $config['home-url'];
	return @mail($to, $subject, $body, $headers);
}

