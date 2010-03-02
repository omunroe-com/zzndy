<?php
require_once $include . 'user.php';
require_once $include . 'application.php';
require_once $include . 'common.php';

function check_email_address($email) {
    // First, we check that there's one @ symbol,
    // and that the lengths are right.
    if (!ereg("^[^@]{1,64}@[^@]{1,255}$", $email)) {
        // Email invalid because wrong number of characters
        // in one section or wrong number of @ symbols.
        return false;
    }
        // Split it into sections to make life easier
    $email_array = explode("@", $email);
    $local_array = explode(".", $email_array[0]);
    for ($i = 0; $i < sizeof($local_array); $i++) {
        if
        (!ereg("^(([A-Za-z0-9!#$%&'*+/=?^_`{|}~-][A-Za-z0-9!#$%&?'*+/=?^_`{|}~\.-]{0,63})|(\"[^(\\|\")]{0,62}\"))$",
        $local_array[$i])) {
            return false;
        }
    }
        // Check if domain is IP. If not,
        // it should be valid domain name
    if (!ereg("^\[?[0-9\.]+\]?$", $email_array[1])) {
        $domain_array = explode(".", $email_array[1]);
        if (sizeof($domain_array) < 2) {
			return false; // Not enough parts to domain
        }
        for ($i = 0; $i < sizeof($domain_array); $i++) {
            if
            (!ereg("^(([A-Za-z0-9][A-Za-z0-9-]{0,61}[A-Za-z0-9])|([A-Za-z0-9]+))$",
            $domain_array[$i])) {
                return false;
            }
        }
    }
    return true;
}

function try_add_user($email, $pass1, $pass2, $lname, $fname, $comment)
{
    global $db, $config;

    if($pass1 == '')
        throw new Exception(PASS_EMPTY);

    if($pass1 != $pass2)
        throw new Exception(PASS_MISMATCH);

    if($lname == '' || $fname == '')
        throw new Exception(NAME_EMPTY);

    if($comment == '')
    	throw new Exception(SHORT_COMMENT);

    if(!check_email_address($email))
        throw new Exception(WRONG_EMAIL);

    if (User::FetchByLogin($email) != null)
        throw new Exception(USER_EXISTS);

    $id = User::save($email, $fname, $lname, $pass1);
    Application::save($id, $comment);
	
	send($config['email'], 'New application on ' . $config['home-url'], "User $name $lname <$email> is requesting access to ${config['home-url']}");
	$ok = send($email, $config['home-url'] . ' registration', make_user_reg_body($email, $fname, $lname, $pass1));
}

function make_user_reg_body($email, $fname, $lname, $pass)
{
	global $config;
	$body = <<<EOF
Dear $fname $lname

Your receive this message because you've requested registration
on ${config['home-url']}. Your application has been recorded, it's
now being considered. 

As soon as your application is approved you will receive a notification 
message and will be able to log in to archieve using these credentials:

login: $email
password: $pass

Regards
${config['home-uri']}
EOF;
	return $body;
}
