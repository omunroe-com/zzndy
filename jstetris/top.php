<?php

define('CLASSES_DIR', dirname(__FILE__) . '/inc/');
define('DATA_DIR', dirname(__FILE__) . '/data/');

function __autoload($class)     {
        require_once(CLASSES_DIR . $class . '.php');
}

function niceDateOldness($date)	{
	$now = time();
	$timestamp = strtotime($date);

	$diff = $now - $timestamp;
	
	// Set the periods of time
    $periods = array("second", "minute", "hour", "day", "week", "month", "year", "decade");

    // Set the number of seconds per period
    $lengths = array(1, 60, 3600, 86400, 604800, 2630880, 31570560, 315705600);

    // Go from decades backwards to seconds
    for ($val = sizeof($lengths) - 1; ($val >= 0) && (($number = $diff / $lengths[$val]) <= 1); $val--);

    // Ensure the script has found a match
    if ($val < 0) $val = 0;

    // Set the current value to be floored
    $number = floor($number);

    // If required create a plural
    if($number != 1) $periods[$val] .= "s";

    return sprintf('<abbr title="%s">%d %s ago</abbr>', $date, $number, $periods[$val]);
}

if($_GET['table'] == 'classic')
	 $table = unserialize(file_get_contents(DATA_DIR . 'table-classic.dat'));
else $table = unserialize(file_get_contents(DATA_DIR . 'table.dat'));

$text = '<table class="score">';

foreach($table as $i => $record)        {
	$line = split('\|', $record->report());
	
	$text .= '<tr class="'.($i%2?'odd':'even').'">';
	
	$text .= '<td>' . ($i+1) . '</td>';
	$text .= '<td class="l">' . $line[0] . '</td>';
	$text .= '<td class="l">' . niceDateOldness($line[2]) . '</td>';
	$text .= '<td>' . number_format($line[1], 0, 3, ' ') . '</td>';
	
}

$text .= '</table>';
?><html>
<head><title>Javascript tetris top <?php echo count($table); if($_GET['table'] == 'classic') echo ' (in classic mode)'?></title>
<link rel="stylesheet" type="text/css" href="style/common.css" />
<style tyle="text/css">
td{font-size: 110%; text-align: right;} 
td, abbr{font-family: monospace; padding: .5ex 1.5ex;}
.l{text-align:left}
</style>
</head>
<body>
<center><h1>Javascript tetris top <?php echo count($table); if($_GET['table'] == 'classic') echo ' (in classic mode)'?></h1></center>
<div style="margin: 0 auto; width: 600px">
<?php echo $text?></div><script src="http://www.google-analytics.com/urchin.js" type="text/javascript">
</script>
<script type="text/javascript">
_uacct = "UA-1627745-1";
urchinTracker();
</script>
</body>
</html>