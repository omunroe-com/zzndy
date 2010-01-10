<?php
require_once $include . 'db.php';

class User
{
    private $_email, $_fname, $_lname, $_registered_on, $_last_logged_on;

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

	public static function FetchByLogin($login)
	{
		global $db;

		$stmt = $db->prepare('SELECT email, fname,  lname, registered_on, last_logged_on FROM sch_users WHERE email = ?');
		$stmt->bind_param('s', $login);
        $stmt->execute();
        $stmt->bind_result($email, $fname, $lname, $registered_on, $last_logged_on);
        $stmt->fetch();

        if($email == '')
            return null;
        else 
            return new User($email, $fname, $lname, $registered_on, $last_logged_on);
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
		$res = $db->query('SELECT email, fname, lname, registered_on, last_logged_on FROM sch_users');

		if($res)
		{
			while($row = $res->fetch_row())
				$users[] = new User($row[0], $row[1], $row[2], $row[3], $row[4]);
		}

		return $users;
	}

    private function __construct($email, $fname, $lname, $registered_on, $last_logged_on)
    {
        $this->_email = $email;
        $this->_fname = $fname;
        $this->_lname = $lname;
        $this->_registered_on = $registered_on;
        $this->_last_logged_on = $last_logged_on;
    }
}
