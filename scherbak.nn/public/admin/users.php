<?php require_once 'login.php' ?>
<!DOCTYPE html>
<html lang="en">
<head>
	<meta charset=utf-8 />
	<title>Users</title>
</head>
<body>
	<?php
		require_once $include . 'user.php';
		$users = User::getList();
		$numusers = count($users);
	?>
	<header>
		<h1>Users</h1>
	<header>
	<nav>
		<a href="index.php" title="Backend">Backend</a>
		<a href="articles.php" title="Browse articles">Articles</a>
	</nav>
	<article>
		<section>
			<header>
				<h1>Users</h1> 
			</header>
			<p><?php echo $numusers;?> users registered on site:</p>
			<table>
			<thead>
			<tr>
				<th>First Name</th>
				<th>Last Name</th>
				<th>Email/Login</th>
				<th>Registered On</th>
				<th>Last Logged In On</th>
			</tr>
			</thead>
			<tbody>
			<?php foreach($users as $user){?>

			<tr>
				<td><?php echo $user->fname(); ?></td>
				<td><?php echo $user->lname(); ?></td>
				<td><?php echo $user->email(); ?></td>
				<td><?php echo $user->registered_on(); ?></td>
				<td><?php echo $user->last_logged_on(); ?></td>
			</tr>

			<?php } ?>
			</tbody>
			</table>
		</section>
	</article>
	<!--<aside></aside>-->
	<footer>&copy; 2009-2010</footer>
</body>

</html

