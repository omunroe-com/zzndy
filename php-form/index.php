<?php
     require_once 'include/tags.php';
     
     $pagetitle = 'Default title';
?><!DOCTYPE xhtml PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head>
	<title><?php echo $pagetitle?></title>
	<style>
		input	{
			-moz-border-radius: 3px;
			border: 1px solid silver;
		}
		tr	{
			background-color: #eef;
		}
		.even	{
			background-color: #dde;
		}
		.comment	{
			opacity: 0.4;
		}
		#marq	{
			font-family: monospace;
		}
		.error	{
			border-color: red;
		}
	</style>
</head>
<body>
	<?php
		$elements = array(
		//	array('Label', 'type', 'name', 'value', 'comment', 'regexp', 'errormsg')
			array('Login name*', 'text', 'login', '', null, 'Subject of availability'),
			array('Password*', 'password', 'pass1', '', null, 'Input desired password'),
			array('Confirm password*', 'password', 'pass2', '', null, 'Confirm tour password'),
			array('E-mail*', 'text', 'email', '', null, 'Please suply you contact e-mail address'),
			array('<div id="marq"></div>', 'submit', '', 'Register', null, '')
		);
		
		$condition = array(
		//	type, name, value, message
			array('regex', 'login', '/^[^0-9]/', 'Login name could not start with a number'),
			array('regex', 'email', '/^[A-z]\S+@\S+\.[a-z]{2,6}/', 'Valid email requied')
		)
		
		echo new_form('reg_form', '', $elements, $conditions, 'get');

	?>

	<script type="text/javascript">
		window.setInterval('marq()', 50);
		
		var marquee = {
			length: 1,
			position: 0,
			string: "-/|\\-/|\\"
		}
		function marq()	{
			var text = marquee.string.substr(marquee.position, marquee.length);
			if(marquee.position + marquee.string.length > marquee.string.length) text += marquee.string.substr(0, marquee.position + marquee.length - marquee.string.length);
			document.getElementById('marq').innerHTML = text;
			if(++marquee.position >= marquee.string.length) marquee.position = 0;
		}
	</script>

</body>
</html>

