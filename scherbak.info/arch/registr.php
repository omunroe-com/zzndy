<?php
include_once '../.private/config.php';

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
<link href="/css/common.css" rel="stylesheet" type="text/css" /><!--[if IE]>
<style type="text/css"> 
/* place css fixes for all versions of IE in this conditional comment */
.twoColElsLt #sidebar1 { padding-top: 30px; }
.twoColElsLt #mainContent { zoom: 1; padding-top: 15px; }
/* the above proprietary zoom property gives IE the hasLayout it needs to avoid several bugs */
</style>
<![endif]-->
<script src="/SpryAssets/SpryMenuBar.js" type="text/javascript"></script>
<link href="/SpryAssets/SpryMenuBarHorizontal.css" rel="stylesheet" type="text/css" />
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
      <p><img src="/images/image3.jpg" alt="Portret1" width="200" height="146" align="left" /></p>
  <!-- end #sidebar1 --></div>
  <div id="mainContent">
    <h1>Реєстрація</h1>
    <p>Для отримання безкоштовного доступу до закритої частини архівів М.М. Щербака, заповніть, будь ласка, реєстраційну форму та погодьтесь з умовами користування архівної інформації.</p>

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
<textarea rows="6" cols="50" readonly="readonly">
УМОВИ

Інформація на цьому сайті (як текстова так і візуальна) захищена українським та міжнародним законодавством про авторське право. Використовування цієї інформаціі (цілком або частово) можливо тільки з дозволу родини Щербака М.М. Незнання закону не звільняє від відповідальності.
</textarea>
<hr />
	<input type="submit" value="Прийняти умови і зареєструватися" />

</form>

<?php endif; ?>

    <p>&nbsp;</p>
    <p>&nbsp;</p>
<!-- end #mainContent --></div>
	<!-- This clearing element should immediately follow the #mainContent div in order to force the #container div to contain all child floats --><br class="clearfloat" />
     <div id="footnote">Copyright © 2009-2011 Scherbak NN. All Rights Reserved.</div>
<!-- end #container --></div>
<script type="text/javascript">
<!--
var MenuBar1 = new Spry.Widget.MenuBar("MenuBar1", {imgDown:"SpryAssets/SpryMenuBarDownHover.gif", imgRight:"SpryAssets/SpryMenuBarRightHover.gif"});
//-->
</script>
</body>
</html>
