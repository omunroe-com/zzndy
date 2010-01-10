<?php
	session_start();

	if(isset($_SESSION['backpass'])):
		
	elseif(isset($_POST['pass']) && $_POST['login'] == $config['admin'] && $_POST['pass'] == $config['admin-pass']):
		$_SESSION['backpass'] = true;

	else:
?><!DOCTYPE html>
<html lang="en">
<head>
	<meta charset=utf-8 />
</head>
<body>
	<article>
		<section>
			<form method="post">
				<p>
					<label for="login">Login</label><br />
					<input type="text" name="login" />
				</p>
				<p>
					<label for="pass">Password</label><br />
					<input type="password" name="pass" />
				</p> 
				<p>
					<input type="submit" value="Login" />
				</p>
			</form>
		</section>
	</article>
	<!--<aside></aside>-->
	<footer>&copy; 2009-2010</footer>
</body>

</html>
<?php 
exit;
endif; 
?>
