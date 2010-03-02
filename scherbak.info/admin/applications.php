<?php

require_once $include . 'admin_template.php';
require_once $include . 'db.php';
require_once $include . 'common.php';
require_once $include . 'application.php';


if(isset($_GET['grant']))
{
	$appid = $_GET['grant'];
	Application::grant($appid);
	header('Location: applications.php');
}

if(isset($_GET['deny']))
{
	$appid = $_GET['deny'];
	Application::deny($appid);
	header('Location: applications.php');
}


admin_header('Applications');

$appls = Application::getList();

echo '<table>';

foreach($appls as $appl)
{
	echo $appl->toAdminString();
}

echo '</table>';

admin_footer();

