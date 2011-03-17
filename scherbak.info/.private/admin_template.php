<?php

$links = array(
	'Backend' => '.',
	'Users' => 'users.php',
	'Applications' => 'applications.php',
	'Articles' => 'articles.php',
);

require_once 'login.php';

function admin_header($title)
{
	global $config;
?>
<!DOCTYPE html>
<html lang="en">
<head>
	<meta charset="utf-8" />
	<title><?php echo $title?> - Scherbak.info</title>
	<link rel="stylesheet" href="/styles/admin.css" type="text/css" />
</head>
<body>
<div id="container">
	<header>
		<h1><a href="<?php echo $config['home-url']?>" title="home">Scherbak.info</a> &mdash; <?php echo $title?></h1>
	</header>
	<?php if($title != 'Login'):?>
	<nav>
		<?php
			global $links;
			foreach($links as $section => $link)
			{
				if($section == $title)
					echo "<b>$title</b> ";
				else
					echo "<a href='$link' title='Go to $section'>$section</a> ";
			}

		?>

		~ <a href="?logout">Logout</a>
	</nav>
	<?php endif;?>
	<article>
		<section>
			<header>
				<h1><?php echo $title?></h1> 
			</header>
<?php
}

function admin_footer()
{
?>

		</section>
	</article>
	<footer id="empty-footer"></footer>
	</div>
	<!--<aside></aside>-->
	<footer>&copy; 2009-2011</footer>
</body>

</html>
<?php
}
