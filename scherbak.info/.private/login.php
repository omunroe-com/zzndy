<?php
	session_start();

	if(isset($_SESSION['backpass'])):
		
	elseif(isset($_POST['pass']) && $_POST['login'] == $config['admin'] && $_POST['pass'] == $config['admin-pass']):
		$_SESSION['backpass'] = true;

	else:
		require_once 'admin_template.php';
		admin_header('Login');
?>
		<section class="centered login">
			<form method="post">
				<p>
					<label for="login">Login</label><br />
					<input type="text" name="login" autocomplete="off" />
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
<?php 
		admin_footer();
exit;
endif; 
?>
