<html>
<head>
</head>
<body>
<?php
              set_time_limit(0);
$text = 'Donec fermentum lectus nec lacus volutpat vitae lobortis odio dapibus. Quisque vulputate nisl nec nisi posuere viverra. Donec sed risus eu lectus sagittis mattis. Nullam et orci sed libero varius viverra eget at nibh. Duis porta dolor in urna adipiscing eu ornare neque dignissim. Integer rhoncus elit id justo ultrices molestie. Fusce tincidunt, diam vel fringilla pharetra, dolor felis laoreet odio, a molestie mauris turpis vitae leo. Fusce rutrum mattis leo eu cursus. Maecenas tincidunt elit et urna accumsan euismod id ac leo. Nunc malesuada nibh eget nisl suscipit nec congue tortor scelerisque. Vestibulum imperdiet, lectus vel convallis vulputate, ante sem iaculis nibh, sed mattis purus nulla quis massa. Nulla pharetra pharetra felis, eu suscipit quam interdum et. Morbi odio nisi, ultrices nec lacinia non, dictum vitae sapien. Suspendisse potenti. Nullam sollicitudin orci quis lorem faucibus condimentum. Nullam venenatis, tellus vitae fringilla porta, justo orci imperdiet purus, at posuere nibh sapien id nibh. Vivamus velit libero, malesuada nec tempus ut, vulputate a nunc. ';
foreach(str_split($text) as $c)
{
echo "<script type=\"text/javascript\">document.write('$c')</script>";
flush();
usleep(500000);
}


?>

</body>

