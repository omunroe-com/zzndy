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
				<th>First Name</th>
				<th>Last Name</th>
				<th>Email/Login</th>
				<th>Registered</th>
				<th>Last&nbsp;Logged&nbsp;In</th>
				<th colspan="2"></th>
			</tr>
			</thead>
			<tbody>
			<?php foreach($users as $user){?>

			<tr>
				<td><?php echo $user->fname(); ?></td>
				<td><?php echo $user->lname(); ?></td>
				<td><?php echo $user->email(); ?></td>
				<td><?php echo wrapdate($user->registered_on())?></td>
				<td><?php echo wrapdate($user->last_logged_on())?></td>
				<td><a href="?accept=<?php echo $user->email(); ?>">V</a></td>
				<td><a href="?delete=<?php echo $user->email(); ?>">X</a></td>
			</tr>

			<?php } ?>
			</tbody>
			</table>
		

<?php echo admin_footer()?>
