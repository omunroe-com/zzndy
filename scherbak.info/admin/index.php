<?php

require_once $include . 'admin_template.php';

admin_header('Backend');


require_once $include . 'user.php';


echo '<p>' . User::count() . ' users currently registered on site.</p>';
echo '<p>' . 0 . ' applications pending.</p>';
echo '<p>Last user login: ' . 0 . ', last application: ' . 0 . '</p>';


admin_footer();
