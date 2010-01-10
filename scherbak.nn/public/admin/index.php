<?php require_once 'login.php' ?>
<!DOCTYPE html>
<html lang="en">
<head>
	<meta charset=utf-8 />
	<title>Admin</title>
</head>
<body>
	<?php
		require_once $include . 'user.php';
		//$numusers = User::count();
	?>
	<header>
		<h1>Site Backend</h1>
	<header>
	<nav>
		<a href="users.php" title="View users">Users</a>
		<a href="articles.php" title="Browse articles">Articles</a>
	</nav>
	<article>
		<section>
			<header>
				<h1>Users</h1> 
			</header>
			<p><?php echo $numusers;?> users registered on site</p>
		</section>
	</article>
	<!--<aside></aside>-->
	<footer>&copy; 2009-2010</footer>
</body>

</html>


