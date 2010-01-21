<?php
	require_once $include . 'admin_template.php';


	require_once $include . 'common.php';
	require_once $include . 'user.php';
	$users = User::getList();
	$numusers = count($users);

	admin_header('Users');
?>
			<p><?php echo $numusers;?> users registered on site:</p>
			<table>
			<thead>
			<tr>
				<th>Status</th>
				<th>First Name</th>
				<th>Last Name</th>
				<th>Email/Login</th>
				<th>Registered</th>
				<th>Last&nbsp;Logged&nbsp;In</th>
				<th colspan="2"></th>
			</tr>
			</thead>
			<tbody>
			<?php foreach($users as $user){
				echo $user->toAdminString();
			} ?>
			</tbody>
			</table>
		

<?php echo admin_footer()?>
