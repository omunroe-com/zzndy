<?php

require_once $include . 'db.php';

class Application
{
	private $_id, $_comment, $_submited_on, $_email, $_fname, $_lname;

	public static function deny($id)
	{
		global $db;
		global $config;

		$stmt = $db->prepare("UPDATE sch_applications SET resolution = 'denied' WHERE application_id = ?");
		$stmt->bind_param('d', $id);
		$stmt->execute();

		$appl = Application::byId($id);

		if($appl != null)
			send($appl->_email, 'Your application for ' . $config['home-url'] . ' was denied.', 'Sorry');
	}

	public static function grant($id)
	{
		global $db;
		global $config;

		$stmt = $db->prepare("UPDATE sch_applications SET resolution = 'granted' WHERE application_id = ?");
		$stmt->bind_param('d', $id);
		$stmt->execute();

		$appl = Application::byId($id);

		if($appl != null)
			send($appl->_email, 'Your application for ' . $config['home-url'] . ' was accepted.', 'You can now access the archive section.');
	}

	public static function byId($id)
	{
		global $db;

		$stmt = $db->prepare(
			'SELECT application_id, comment, submited_on, email, fname, lname '
			.'FROM sch_applications INNER JOIN sch_users ON sch_applications.user_id = sch_users.user_id '
			.'WHERE application_id = ?');

		$stmt->bind_param('d', $id);
		$res = $stmt->execute();

		if($res)
			return new Application($row[0], $row[1], $row[2], $row[3], $row[4], $row[5]);

		return null;
	}

	public static function count()
	{
		global $db;
		$res = $db->query('SELECT COUNT(1) FROM sch_applications');
		$row = $res->fetch_row();

		return $row[0];
	}

	public static function getList()
	{
		global $db;

		$appls = array();
		$res = $db->query(
			'SELECT application_id, comment, submited_on, email, fname, lname '
			.'FROM sch_applications INNER JOIN sch_users ON sch_applications.user_id = sch_users.user_id '
			.'WHERE resolution = "pending"');

		if($res)
			while($row = $res->fetch_row())
				$appls[] = new Application($row[0], $row[1], $row[2], $row[3], $row[4], $row[5]);

		return $appls;
	}

	public static function save($user_id, $comment)
	{
		global $db;
		$stmt = $db->prepare('INSERT INTO sch_applications (user_id, comment) VALUES (?, ?)');
		$stmt->bind_param('ds', $user_id, htmlspecialchars($comment, ENT_QUOTES));
		$stmt->execute();
	}

	public function __construct($id, $comment, $submited_on, $user_email, $user_fname, $user_lname)
	{
		$this->_id = $id;
		$this->_comment = $comment;
		$this->_submited_on = $submited_on;
		$this->_email = $user_email;
		$this->_fname = $user_fname;
		$this->_lname = $user_lname;
	}

	public function toAdminString()
	{
		global $config;
		$text .= '<tr>';
		$text .= '<td class="name"><a name="appl-' . $this->_id . '">' . $this->_fname . ' ' . $this->_lname . '</a></td>';
		$text .= '<td rowspan="2">' . $this->_comment . '<br/><br/>Submitted ' . wrapdate($this->_submited_on). '</td>';
		$text .= '<td class="grant"><a href="?grant=' . $this->_id . '" title="Grant access for ' . $this->_fname . ' ' . $this->_lname . '">Grant</a></td>';
		$text .= '</tr>';
		$text .= '<tr>';
		$text .= '<td><a href="mailto:' . $this->_email . '?subject=' . $config['home-url'] . ' application">' . $this->_email . '</a></td>';
		$text .= '<td class="deny"><a href="?deny=' . $this->_id . '" title="Deny this application">Deny</a></td>';
		$text .= '</tr>';
		$text .= '<tr><td colspan="3"><hr /></td></tr>';
		$text .= "\n";	

		return $text;
	}
}
