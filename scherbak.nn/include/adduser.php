<?php
require_once $include . 'user.php';
require_once $include . 'common.php';

function check_email_address($email) {
    // First, we check that there's one @ symbol,
    // and that the lengths are right.
    if (!ereg("^[^@]{1,64}@[^@]{1,255}$", $email)) {
        // Email invalid because wrong number of characters
        // in one section or wrong number of @ symbols.
        echo 'general';
        return false;
    }
        // Split it into sections to make life easier
    $email_array = explode("@", $email);
    $local_array = explode(".", $email_array[0]);
    for ($i = 0; $i < sizeof($local_array); $i++) {
        if
        (!ereg("^(([A-Za-z0-9!#$%&'*+/=?^_`{|}~-][A-Za-z0-9!#$%&?'*+/=?^_`{|}~\.-]{0,63})|(\"[^(\\|\")]{0,62}\"))$",
        $local_array[$i])) {
            echo 'fisrt';
            return false;
        }
    }
        // Check if domain is IP. If not,
        // it should be valid domain name
    if (!ereg("^\[?[0-9\.]+\]?$", $email_array[1])) {
        $domain_array = explode(".", $email_array[1]);
        if (sizeof($domain_array) < 2) {
            echo 'not long enough';
            return false; // Not enough parts to domain
        }
        for ($i = 0; $i < sizeof($domain_array); $i++) {
            if
            (!ereg("^(([A-Za-z0-9][A-Za-z0-9-]{0,61}[A-Za-z0-9])|([A-Za-z0-9]+))$",
            $domain_array[$i])) {
                echo 'last';
                return false;
            }
        }
    }
    return true;
}

function try_add_user($email, $pass1, $pass2, $lname, $fname)
{
    global $db;

    if($pass1 == '')
        throw new Exception(RegError::$PASS_EMPTY);

    if($pass1 != $pass2)
        throw new Exception('PASS_MISTMATCH');

    if($lname == '' || $fname == '')
        throw new Exception('NAME_EMPTY');

    if(!check_email_address($email))
        throw new Exception('WRONG_EMAIL');

    if (User::FetchByLogin($email) != null)
        throw new Exception('USER_EXISTS');

    $stmt = $db->prepare('INSERT INTO sch_users (email, fname, lname, hash, salt) VALUES (?, ?, ?, ?, ?)');

    $salt = mt_rand();
    $hash = sha1($pass1 . $salt);

    $stmt->bind_param('ssssd', $email, $fname, $lname, $hash, $salt);
    $stmt->execute();
}
