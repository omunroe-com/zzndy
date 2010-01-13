<?php

require_once $include . 'admin_template.php';
require_once $include . 'db.php';
require_once $include . 'common.php';

admin_header('Applications');

$res = $db->query('SELECT application_id, comment, submited_on, email, fname, lname FROM sch_applications INNER JOIN sch_users ON sch_applications.user_id = sch_users.user_id WHERE resolution = "pending"');

echo '<table>';

while($obj = $res->fetch_array())
{
	$comment = $obj[1];
	$submited_on = $obj[2];
	$email = $obj[3];
	$name = $obj[4] . ' ' . $obj[5];

	echo '<tr>';
	echo '<td class="name">' . $name . '</td>';
	echo '<td rowspan="2">' . $comment . '<br/><br/>Submitted ' . wrapdate($submited_on). '</td>';
	echo '<td class="grant"><a href="?grant=' . $obj[0] . '" title="Grant access for ' . $name . '">Grant</a></td>';
	echo '</tr>';
	echo '<tr>';
	echo '<td><a href="mailto:' . $email . '?subject=' . $config['home-url'] . ' application">' . $email . '</a></td>';
	echo '<td class="deny"><a href="?deny=' . $obj[0] . '" title="Deny this application">Deny</a></td>';
	echo '</tr>';
}

echo '</table>';


admin_footer();

