<?php

$http_vars = $_POST;
$email = $http_vars['email'];
$fname = $http_vars['fname'];
$lname = $http_vars['lname'];
$pass1 = $http_vars['pass1'];
$pass2 = $http_vars['pass2'];
$comnt = $http_vars['comnt'];
$error = '';

require_once $include . 'adduser.php';
require_once $include . 'common.php';

$appl_pending = false;
session_start();
if(isset($_SESSION['appl_pending']))
	$appl_pending = true;

if(isset($email))
{
    try{
	    try_add_user($email, $pass1, $pass2, $lname, $fname, $comnt);
		$_SESSION['appl_pending'] = 'yes';
		header('Location: registr.php');
    }
    catch(Exception $ex)
    {
        $error = $ex->getMessage();
    }
}

?><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title>Микола Миколайович Щербак</title>
<link href="css/common.css" rel="stylesheet" type="text/css" /><!--[if IE]>
<style type="text/css"> 
/* place css fixes for all versions of IE in this conditional comment */
.twoColElsLt #sidebar1 { padding-top: 30px; }
.twoColElsLt #mainContent { zoom: 1; padding-top: 15px; }
/* the above proprietary zoom property gives IE the hasLayout it needs to avoid several bugs */
</style>
<![endif]-->
<script src="SpryAssets/SpryMenuBar.js" type="text/javascript"></script>
<link href="SpryAssets/SpryMenuBarHorizontal.css" rel="stylesheet" type="text/css" />
</head>

<body class="twoColElsLt">

<div id="header">
  <div id="space"></div>
  <div id="collage"></div>
  <div id="space1"></div>
  <div id="menu">
    <ul id="MenuBar1" class="MenuBarHorizontal">
      <li><a href="index.html">Головна</a></li>
      <li><a href="biog.html">Біографія</a></li>
      <li><a href="science.html">Наукова робота</a></li>
      <li><a href="exp.html">Експедиції</a></li>
      <li><a href="museum.html">Музей</a></li>
      <li><a href="#">Apxiв</a></li>
<li><a href="about.html">Про сайт</a></li>
    </ul>
  </div>
    <div id="flags">
      <center><table width="100" height="29" border="0">
    <tr>
        <td><a href="index.html"><img src="images/ukrflag.gif" alt="UA flag" width="20" height="15" border="1" /></a></td>
        <td><img src="images/rusflag.gif" alt="RU flag" width="20" height="15" border="1" /></td>
        <td><img src="images/ukflag.gif" alt="UK flag" width="20" height="15" border="1" /></td>
      </tr>
      </table>
      </center>
     
</div>
  <div id="space2"></div>
</div>  
<div id="container">
    <div id="sidebar1">
      <p><img src="images/image3.jpg" alt="Portret1" width="200" height="146" align="left" /></p>
  <!-- end #sidebar1 --></div>
  <div id="mainContent">
    <h1>Регістрація</h1>
    <p>Для отримання безкоштовного доступу до закритої частини архівів М.М. Щербака, заповніть, будь ласка, регістраційну форму та погодьтесь з умовами користування архівної інформації.</p>

	<?php
		if($appl_pending):?>
		
		<?php 
			print_message(APPL_PENDING);
			unset($_SESSION['appl_pending']);
		?>
		
	<?php else:	?>
	
<form name="register" method="post">
    <?php


        if(!empty($error))
        {
            print_error( REG_FAIL );

            if($error == NAME_EMPTY || $error == USER_EXISTS)
		    print_error($error);
        }

    ?>
    <label for="fname">Ім`я</label><input type="text" name="fname" maxlength="50" size="30" /><br/>
    <label for="lname">Прізвище</label><input type="text" name="lname" maxlength="50" size="30" /><br/>
<hr />
    <?php if($error == EMAIL_INVALID) print_error($error)?>
    <label for="email">Ел. пошта</label><input type="email" name="email" maxlength="50" size="30" /><br/>
    <?php if($error == PASS_EMPTY) print_error($error)?>
    <label for="pass1">Пароль</label><input type="password" name="pass1" maxlength="50" size="30" /><br/>
    <?php if($error == PASS_MISMATCH) print_error($error)?>
    <label for="pass2">Підтвердіть пароль</label><input type="password" name="pass2" maxlength="50" size="30" /><br/>
<hr />

    <?php if($error == SHORT_COMMENT) print_error($error)?>
    <label for="comnt">Коментар</label><br />
    <textarea rows="6" cols="50" name="comnt"></textarea>
<hr />
<textarea rows="6" cols="50">
УМОВИ

Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin malesuada pharetra felis at ultricies. Nullam quis mi non leo dictum fringilla at sit amet turpis. Morbi luctus libero eu odio dictum auctor. Ut malesuada egestas sapien, eget condimentum diam ultricies at. Duis accumsan erat luctus lectus rutrum interdum. Integer ultricies ultricies ipsum vitae molestie. Phasellus congue fermentum porttitor. Integer ac dui in diam lobortis elementum ut et ante. Fusce feugiat arcu ipsum. Ut hendrerit tempus nibh vel congue. Curabitur eget elit dolor. Mauris mattis odio ut tortor facilisis condimentum. Curabitur est ante, sodales vel lobortis eget, aliquet tincidunt orci. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Duis pretium, turpis quis iaculis venenatis, ipsum erat semper dui, eget venenatis felis tortor mollis mi. Mauris eget felis sed neque ultricies pellentesque nec in lacus. Nulla facilisi. Maecenas rutrum placerat egestas. Morbi dignissim tellus est, vitae porttitor urna.

Vestibulum euismod felis sit amet magna egestas vulputate. Nunc elementum placerat felis, semper sagittis ipsum blandit eu. Aenean sit amet justo arcu, in porttitor felis. Etiam eget neque eros. Donec at tellus diam, sit amet sodales augue. Aliquam nec felis et quam consectetur rhoncus ut et tellus. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Duis rhoncus posuere tellus, eget ornare tortor iaculis ac. Suspendisse ac massa augue, non malesuada enim. Nunc erat libero, cursus eu scelerisque nec, ultrices quis leo. Mauris tortor augue, egestas nec faucibus non, venenatis molestie orci. Phasellus commodo, ipsum ut aliquet congue, risus orci tincidunt metus, a dignissim lectus turpis eget nibh.

Sed sem urna, interdum at convallis nec, varius lobortis nisl. Nullam hendrerit purus sit amet diam porta varius. Suspendisse potenti. Mauris felis augue, gravida vitae vulputate eu, pharetra ac nisl. Nulla cursus mattis dui, a lacinia tortor accumsan sit amet. Quisque mi nisl, consequat non accumsan et, laoreet non elit. Nunc congue sagittis quam id posuere. Nulla scelerisque tellus accumsan urna consequat ac laoreet nisl bibendum. Duis elementum blandit faucibus. Vestibulum ac risus vel augue imperdiet posuere vitae quis urna. Nullam dui sapien, accumsan quis facilisis at, rutrum eu sapien. Proin eu purus risus. Quisque eget risus nunc, a accumsan justo. Fusce porttitor, elit at fermentum placerat, sapien massa laoreet nunc, et hendrerit lectus lacus fermentum purus. Curabitur ut ante velit, et adipiscing massa. Ut ut odio vel libero scelerisque pretium ut eu odio.

Donec ac ipsum in arcu scelerisque placerat vitae sed mauris. Praesent sapien purus, faucibus a vulputate nec, auctor eu arcu. Aliquam suscipit, ipsum in adipiscing lobortis, ligula nibh condimentum dui, vitae posuere magna augue vitae dolor. Duis velit diam, ornare blandit tincidunt vel, dapibus vel orci. Aenean eu velit risus, sed convallis dui. Sed non metus mauris. Nunc sagittis nunc eu justo rutrum laoreet viverra enim facilisis. Sed sed eros vitae massa interdum placerat. Nunc molestie convallis tortor et commodo. Vestibulum vitae leo est, ac varius lorem. Donec tristique lectus et quam ullamcorper congue. Ut dignissim tempor metus, sed consequat nisi accumsan sed. Aenean auctor, metus ac iaculis mollis, risus enim sodales nisi, a adipiscing ante urna in erat. Pellentesque interdum tincidunt ipsum a blandit. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer augue enim, aliquet vel gravida in, porttitor et risus. Cras imperdiet massa eget nunc volutpat tempor. Integer bibendum ligula a mi adipiscing aliquam. Nam scelerisque felis ac leo pretium non consequat lorem consectetur.

Sed rutrum tempor metus, sed dapibus sem tempus eu. Aliquam pulvinar leo non nibh vestibulum congue. Nunc sollicitudin placerat varius. Nullam sagittis convallis lorem in blandit. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Nunc in elit vel magna molestie pretium. Phasellus erat sapien, lobortis non blandit et, hendrerit quis urna. Maecenas tellus magna, lobortis eget posuere nec, eleifend a quam. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Nullam sit amet enim in tortor lacinia bibendum. Duis odio metus, imperdiet sed posuere in, condimentum sed lectus. Maecenas eget imperdiet orci. Proin adipiscing feugiat urna, at tempor elit pulvinar vitae. 

</textarea>
<hr />
	<input type="submit" value="Прийняти умови і зареєструватися" />

</form>

<?php endif; ?>

    <p>&nbsp;</p>
    <p>&nbsp;</p>
<!-- end #mainContent --></div>
	<!-- This clearing element should immediately follow the #mainContent div in order to force the #container div to contain all child floats --><br class="clearfloat" />
     <div id="footnote">Copyright © 2009 Scherbak NN. All Rights Reserved</div>
<!-- end #container --></div>
<script type="text/javascript">
<!--
var MenuBar1 = new Spry.Widget.MenuBar("MenuBar1", {imgDown:"SpryAssets/SpryMenuBarDownHover.gif", imgRight:"SpryAssets/SpryMenuBarRightHover.gif"});
//-->
</script>
</body>
</html>
