<?php
/*
 * TODO: change new_form function to use separated conditions and find required
 * fields
 */

function tag($name, $attrs = null, $contents = null)	{
	$rval = "<$name";
	if(is_array($attrs))foreach($attrs as $key => $value)	{
		if(is_int($key)) $key = $value;
		$rval .= ' '.$key.'="'.htmlentities($value).'"';
	}
	if($contents !== null)$rval .= ">$contents</$name>";
	else $rval .= " />";
	return $rval;
}

function input($type, $name, $value, $attrs = array())	{
	if($attrs == null) $attrs = array();	
	return tag('input', 
		array_merge(
			array('type' => $type, 
				'name' => $name, 
				'value' => $value), 
			$attrs
		)
	);
}

function hidden($name, $value)	{
	return input('hidden', $name, $value);
}

function select($name, $options, $use_keys = false, $selected = null, $attrs = array())	{
	$rval = '';
	if($attrs == null) $attrs = array();
	foreach($options as $value => $text)	{
		if(!$use_keys) $value = $text;
		$option_attrs = array('value' => $value);
		if($value == $selected)	{
			$option_attrs[] = 'selected';
		}
		$rval .= tag('option', $option_attrs, $text);
	}
	return tag('select', array_merge(array('name' => $name), $attrs), $rval);
}

/////////////////////////////////////////////////////////////////////////////////////
//	AUTOFORM FUNCTIONS
/////////////////////////////////////////////////////////////////////////////////////

define('F_LABEL',	0);
define('F_TYPE',	1);
define('F_NAME',	2);
define('F_VALUE',	3);
define('F_ATTRS',	4);
define('F_COMMENT',	5);
/*define('F_REGEX',	6);
define('F_ERRORMSG',	7);*/


$form_head	= '<table>';
$form_row	= '<tr{EVEN}><td class="label">{LABEL}</td><td class="control">{ELEMENT}</td><td class="comment">{COMMENT}</td></tr>';
$form_foot	= '</table>';
$form_script	= 'function validate_{FORM_NAME}()	{
	
	var check_fields = {CHECK_FLDS};
	var form = document.forms["{FORM_NAME}"];
	var error_msgs = new Array();
	var given_focus = false;
	for(var i in check_fields)	{
		var elt = check_fields[i]; 
		
		if(!form[elt.name].value.match(elt.regex))	{
			if(!given_focus)	{
				given_focus = true;
				form[elt.name].focus;	
			}
			error_msgs.push(elt.msg);
			form[elt.name].className = "error";
		}
		else form[elt.name].className = "";
	}
	if(error_msgs.length)	{
		var text = "Following errors occured:\n";
		for(var i=0; i < Math.min(error_msgs.length, 4); ++i) text += "+ " + error_msgs[i] + "\n";
		if(error_msgs.length > 4) text += "...\n";
		alert(text);
		return false;
	}
	return true;
}';

function new_form($name, $action, $elements, $conditions, $method = 'post', $enctype = 'form-data/xxx-urlencoded')	{
	global $form_head, $form_row, $form_foot, $form_script;
	$rows = '';
	$row_count = 0;
	foreach($elements as $element)	{
		$control = '';
		switch($element[F_TYPE])	{
			case 'text': 
			case 'password':
			case 'submit':
				$control = input($element[F_TYPE], $element[F_NAME], $element[F_VALUE], $element[F_ATTRS]);
				break;
			default:
				$control = $element[F_VALUE];
				break;
		}
	
	
		$row = preg_replace('/{EVEN}/', (($row_count%2)?'':' class="even"'), $form_row);
		$row = preg_replace('/{LABEL}/', tag('lable', array('for' => $element[F_NAME].'-id'), $element[F_LABEL]), $row);
		$row = preg_replace('/{ELEMENT}/', $control, $row);
		$row = preg_replace('/{COMMENT}/', $element[F_COMMENT], $row);

		$rows .= $row;
		
		if(!empty($element[F_REGEX])) $fields[] = "{name: '{$element[F_NAME]}', regex: {$element[F_REGEX]}, msg: '{$element[F_ERRORMSG]}'}";
		++$row_count;
	}
	
	$script = preg_replace('/{CHECK_FLDS}/', '['.implode(', ', $fields).']', $form_script);
	$script = preg_replace('/{FORM_NAME}/', $name, $script);
	
	return tag('form', 
array('name' => $name, 'action' => $action, 'method' => $method, 'enctype' => $enctype, 'onsubmit' => 'return validate_'.$name.'()'), 
		$form_head
		.$rows
		.$form_foot
	)
	.tag('script', array('type' => 'text/javascript'), $script);
}

?>
