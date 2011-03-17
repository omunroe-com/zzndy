<?php
	include_once '../.private/config.php';
	require_once $include . 'admin_template.php';


	require_once $include . 'common.php';
	require_once $include . 'user.php';
	$users = User::getList();
	$numusers = count($users);

	admin_header('Users');
?>
			<p><?php echo $numusers;?> users registered on site:</p>
			<table id="users">
			<thead>
			<tr>
				<th>Status</th>
				<th>First Name</th>
				<th>Last Name</th>
				<th>Email/Login</th>
				<th>Registered</th>
				<th>Last&nbsp;Logged&nbsp;In</th>
			</tr>
			</thead>
			<tbody>
			<?php foreach($users as $user){
				echo $user->toAdminString();
			} ?>
			</tbody>
			</table>
				<script src="/js/jquery.js" ></script>		
				<script src="/js/jquery.tablesorter.min.js"></script>		
				<script>
				$(function(){ 
					$('#users').tablesorter(); 
				});					
				</script>

<?php echo admin_footer()?>
