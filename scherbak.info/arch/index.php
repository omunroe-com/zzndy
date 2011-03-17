<?php
	require_once '../.private/config.php';	

	session_start();

	function file_extension($filename)
	{
	    $path_info = pathinfo($filename);
	    return $path_info['extension'];
	}
	
	if(isset($_GET['file']))
	{
			$mimes = array(
				'gif' => 'image/gif',	
				'ief' => 'image/ief',	
				'jpe' => 'image/jpeg',	
				'jpeg' => 'image/jpeg',	
				'jpg' => 'image/jpeg',	
				'png' => 'image/png',	
				'tif' => 'image/tiff',	
				'tiff' => 'image/tiff',	
				'pdf' => 'application/pdf'	
			);


		$extension = file_extension($_GET['file']);
		if(strpos($_GET['file'], '..') === FALSE && isset($_SESSION['user']) && isset($mimes[$extension]))
		{
			header('Content-type: ' . $mimes[$extension]);
			readfile($_GET['file']);
		}
		else
		{
			header('HTTP/1.0 403 Forbidden');
			exit;
		}
	}

	require_once $include . 'common.php';
	require_once $include . 'user.php';

	$error = '';
	if(isset($_GET['logout']))
	{
		unset($_SESSION['user']);
		header('Location: /');
	}

	if(isset($_POST['login']) && !isset($_SESSION['user']))
	{
		$user = User::FetchUser($_POST['login'], $_POST['passwd']);
		if($user == null)
			$error = INVALID_LOGIN;
		else if($user->appl_resolution() != 'granted')
			$error = ACCESS_DENIED;
		else
		{
			$_SESSION['user'] = $_POST['login'];
			header('Location: .');
		}
	}

?><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title>Микола Миколайович Щербак</title>
<link href="../css/common.css" rel="stylesheet" type="text/css" /><!--[if IE]>
<style type="text/css"> 
/* place css fixes for all versions of IE in this conditional comment */
.twoColElsLt #sidebar1 { padding-top: 30px; }
.twoColElsLt #mainContent { zoom: 1; padding-top: 15px; }
/* the above proprietary zoom property gives IE the hasLayout it needs to avoid several bugs */
</style>
<![endif]-->
<script src="../SpryAssets/SpryMenuBar.js" type="text/javascript"></script>
<link href="../SpryAssets/SpryMenuBarHorizontal.css" rel="stylesheet" type="text/css" />
<script type="text/javascript" src="/js/jquery.js"></script>
<script type="text/javascript" src="/js/jquery.lightbox-0.5.pack.js"></script>
<link rel="stylesheet" type="text/css" href="/css/jquery.lightbox-0.5.css" media="screen" />
</head>

<body class="twoColElsLt">
<div id="header">
  <div id="space"></div>
  <div id="collage">
<a href="/">
<h1>ЩЕРБАК</h1>
<h2>Микола Миколайович</h2>
<h3>Николай Николаевич</h3>
</a>
</div>

  <div id="space1"></div>
  <div id="menu">
    <ul id="MenuBar1" class="MenuBarHorizontal">
<li><a href="/uk/index/">Головна</a></li>
<li><a href="/uk/bio/">Біографія</a></li>
<li><a href="/uk/science/">Наукова робота</a></li>
<li><a href="/uk/expedition/">Експедиції</a></li>
<li><a href="/uk/museum/">Музей</a></li>
<li><a href="/arch/">Apxiв</a></li>
<li><a href="/uk/about/">Про сайт</a></li>
    </ul>
  </div>
    <div id="flags">
      <center><table width="100" height="29" border="0">
    <tr>
        <td><a href="/uk/"><div class="uk flag"></div></a></td>
        <td><div class="ru flag"></div></td>
        <td><div class="en flag"></div></td>
      </tr>
      </table>
      </center>
     
</div>
  <div id="space2"></div>
</div>  
<div id="container">
    <div id="sidebar1">
      <p><img src="../images/image3.jpg" alt="Portret1" width="200" height="146" align="left" /></p>
  <!-- end #sidebar1 --></div>
  <div id="mainContent">
  <?php if(isset($_SESSION['user'])):
  	$at = strpos($_SESSION['user'], '@');
  	$first = substr($_SESSION['user'], 0, $at);
	$rest = substr($_SESSION['user'], $at + 1, strlen($_SESSION['user']));
  ?>

	<div class="logout">
		Logged in as <b><span><?php echo $first?></span>@<span><?php echo $rest?></span></b>, <a href="?logout">logout</a>.
	</div>

  <?php endif?>
    <h1> Архів </h1>
    <?php if(!isset($_SESSION['user'])):?>
    <p>Архівна частина сайту ММ. Щербака складається з двох частин: загальнодоступної (ця сторінка) та закритої. У закритій частині знаходяться експедиційні щоденники М.М., його спргади, листи, тощо.</p>
    <p>Для отримання безкоштовного доступу до закритої частини, перейдіть на сторінку <a href="registr.php" title="Регістрація">регістрації</a>.</p>
    <?php if (!empty($error))print_error($error);?>

	<p>Якщо ви вже зареєстровані на сайті, введіть свої дані:</p>
    <form method="post">
    	<label for="login">Логін</label><input type="text" name="login" />
	<br />
	<label for="passwd">Пароль</label><input type="password" name="passwd" />
	<br />
	<input type="submit" value="Login"/>
    </form>
    <?php else:?>

	<div class="gallery" id="gallery">

		<?php 
			$exts = array('png', 'jpg', 'jpeg');
			$files = array('alpha', 'bravo');
			foreach(glob('thumb/*.*') as $file)
			{
				$ext = substr($file, strrpos($file, '.') + 1);
				if(in_array($ext, $exts))				
					echo '<span><a href="'.substr($file, strpos($file, '/')+1).'"><img src="' . $file . '"/></a></span>';
			}

		?>

	</div>
	<script type="text/javascript">
	$(function(){
		$('#gallery a').lightBox({
			imageLoading: '/images/lightbox-ico-loading.gif',
			imageBtnClose: '/images/lightbox-btn-close.gif',
			imageBtnPrev: '/images/lightbox-btn-prev.gif',
			imageBlank: '/images/lightbox-blank.gif',
			imageBtnNext: '/images/lightbox-btn-next.gif'
		});
	})
	</script>


    <?php endif?>

    <p>&nbsp;</p>
    <p>&nbsp;</p>
    <p>&nbsp;</p>
    <p>&nbsp;</p>
  </div></div></body>
</html>
