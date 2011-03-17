<?php
require_once $include . 'db.php';

class User
{
    private $_email, $_fname, $_lname, $_registered_on, $_last_logged_on, $_appl_id, $_appl_resolution;

    public function email()
    {
    	return $this->_email;
    }

    public function fname()
    {
    	return $this->_fname;
    }

    public function lname()
    {
    	return $this->_lname;
    }

    public function registered_on()
    {
    	return $this->_registered_on;
    }

    public function last_logged_on()
    {
    	return $this->_last_logged_on;
    }

    public function appl_id()
    {
    	return $this->_appl_id;
    }

    public function appl_resolution()
    {
    	return $this->_appl_resolution;
    }

    public static function FetchUser($login, $password)
    {
    	global $db;
	$stmt = $db->prepare(
		'SELECT email, fname,  lname, registered_on, last_logged_on, application_id, resolution '
		. 'FROM sch_users INNER JOIN sch_applications ON sch_applications.user_id = sch_users.user_id '
		. 'WHERE email = ? AND SHA1(CONCAT(?, salt)) = hash'
	);

	$stmt->bind_param('ss', $login, $password);
	$stmt->execute();

	$stmt->bind_result($email, $fname, $lname, $registered_on, $last_logged_on, $application_id, $resolution);
	$stmt->fetch();

	if($email == '')
	    return null;
	else 
	    return new User($email, $fname, $lname, $registered_on, $last_logged_on, $application_id, $resolution);
    }
    
	public static function FetchByLogin($login)
	{
		global $db;

		$stmt = $db->prepare(
			'SELECT email, fname,  lname, registered_on, last_logged_on, application_id, resolution '
			. 'FROM sch_users INNER JOIN sch_applications ON sch_applications.user_id = sch_users.user_id '
			. 'WHERE email = ?');

		$stmt->bind_param('s', htmlspecialchars($login, ENT_QUOTES));
		$stmt->execute();
		$stmt->bind_result($email, $fname, $lname, $registered_on, $last_logged_on, $application_id, $resolution);
		$stmt->fetch();

		if($email == '')
		    return null;
		else 
		    return new User($email, $fname, $lname, $registered_on, $last_logged_on, $application_id, $resolution);
	}

	public static function count()
	{
		global $db;
		$res = $db->query('SELECT COUNT(1) FROM sch_users');
		$row = $res->fetch_row();
		return $row[0];
	}

	public static function getList()
	{
		global $db;

		$users = array();
		$res = $db->query(
			'SELECT email, fname,  lname, registered_on, last_logged_on, application_id, resolution '
			. 'FROM sch_users INNER JOIN sch_applications ON sch_applications.user_id = sch_users.user_id '
			. 'WHERE resolution != \'denied\' ');

		if($res)
		{
			while($row = $res->fetch_row())
				$users[] = new User($row[0], $row[1], $row[2], $row[3], $row[4], $row[5], $row[6]);
		}

		return $users;
	}

	// TODO: Rename to create
	static public function save($email, $fname, $lname, $pass)
	{
		global $db;
		$stmt = $db->prepare('INSERT INTO sch_users (email, fname, lname, hash, salt) VALUES (?, ?, ?, ?, ?)');

		$salt = mt_rand();
		$hash = sha1($pass . $salt);

		$stmt->bind_param('ssssd', 
			htmlspecialchars($email, ENT_QUOTES), 
			htmlspecialchars($fname, ENT_QUOTES), 
			htmlspecialchars($lname, ENT_QUOTES), 
			$hash, 
			$salt
		);
	    
		$stmt->execute();

	    $res = $db->query('SELECT LAST_INSERT_ID()');
	    $obj = $res->fetch_row();
	    $user_id = $obj[0];

	    return $user_id;
	}

    private function __construct($email, $fname, $lname, $registered_on, $last_logged_on, $appl_id, $appl_resolution)
    {
        $this->_email = $email;
        $this->_fname = $fname;
        $this->_lname = $lname;
        $this->_registered_on = $registered_on;
        $this->_last_logged_on = $last_logged_on;
		$this->_appl_id = $appl_id;
		$this->_appl_resolution = $appl_resolution;
    }

    public function toAdminString()
    {
		$text = '<tr>';
		$text .= '<td><a href="applications.php?#appl-' . $this->_appl_id . '">' . $this->appl_resolution() . '</a></td>';
		$text .= '<td>' . $this->fname() . '</td>';
		$text .= '<td>' . $this->lname() . '</td>';
		$text .= '<td>' . $this->email() . '</td>';
		$text .= '<td><span style="display: none">' . strtotime($this->registered_on()) . '</span>' . wrapdate($this->registered_on()) . '</td>';
		$text .= '<td><span style="display: none">' . strtotime($this->last_logged_on()) . '</span>' . wrapdate($this->last_logged_on()) . '</td>';
		$text .= '</tr>';
		$text .= "\n";

		return $text;
    }
}
