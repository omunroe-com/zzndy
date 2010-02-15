<?php
	include_once '../.private/config.php';
	require_once $include . 'admin_template.php';

	require_once $include . 'common.php';
	require_once $include . 'article.php';

	if(isset($_POST['delete']))
	{
		$article = Article::getById($_POST['delete']);
		unlink( $_SERVER['DOCUMENT_ROOT'] . '/.public/' . $article->full_path() . '.html' );

		Article::delete($_POST['delete']);
		header('Location: articles.php');
		exit();
	}

	if(isset($_GET['rerender']))
	{
		$articles = Article::getList();

		foreach($articles as $article)
		{
			render($article);
		}

		header('Location: articles.php');
		exit();
	}

	$id = $_POST['id'];
	$body = stripslashes($_POST['markdown']);
	$title = $_POST['title'];
	$html = stripslashes($_POST['article']);
	$lang = $_POST['lang'];
	$path = $_POST['path'];

	// Render static article page
	if(!empty($lang) || !empty($id))
	{
		if(!empty($lang))
		{
			$article = Article::create($lang, $path, $title, $body, $html);

			if(is_string($article))
			{
				$_SESSION['error'] = $article;
				header('Location: articles.php');
				exit();
			}
		}
		else
		{
			$article = Article::getById($id);
			$article->set_body($body);
			$article->set_html($html);
			$article->set_title($title);
			$article->save();
		}

		render($article);

		header('Location: articles.php?edit=' . $article->id());
	}

	function render($article)
	{
		global $include;
		$search = array('{page}', '{menu}', '{content}');
		$replace = array($article->title());
		$replace[] = <<<MENU
<li><a href="/uk/index/">Головна</a></li>
<li><a href="/uk/bio/">Біографія</a></li>
<li><a href="/uk/science/">Наукова робота</a></li>
<li><a href="/uk/expedition/">Експедиції</a></li>
<li><a href="/uk/museum/">Музей</a></li>
<li><a href="/arch/">Apxiв</a></li>
<li><a href="/uk/about/">Про сайт</a></li>
MENU;

		$replace[] = $article->html();

		$file = file_get_contents( $include . 'user_template.html' );
		$contents = str_replace($search, $replace, $file);
		file_put_contents( $_SERVER['DOCUMENT_ROOT'] . '/.public/' . $article->full_path() . '.html', $contents );

	}

	function hidden($name, $value, $attrs = array())
	{
		echo "<input type=\"hidden\" value=\"$value\" name=\"$name\" ";
		foreach($attrs as $key => $value)
			echo "$key=\"$value\" ";

		echo "/>\n";
	}

	function textbox($name, $value, $len)
	{
		echo "<input type=\"text\" value=\"$value\" name=\"$name\" maxlenght=\"$len\" />\n";
	}

	function radios($name, $values, $default)
	{
		foreach($values as $value => $label)
		{
			$checked = $value == $default ? 'checked="checked"' : '';
			echo "<input type=\"radio\" name=\"$name\" value=\"$value\" $checked id=\"rb-$name-$value\"/>";
			echo "<label for=\"rb-$name-$value\">$label</label>";
		}

		echo "\n";
	}

	function form($action, $attrs = array())
	{
		echo "<form method=\"post\" action=\"$action\" ";
		foreach($attrs as $key => $value)
			echo "$key=\"$value\" ";

		echo "/>\n";
	}

	function postlink($text, $action, $data, $attrs = array())
	{
		form($action, array_merge($attrs, array('class' => 'void')));
		foreach($data as $key => $value)
		{
			hidden($key, $value);
		}
		echo "<input type=\"submit\" value=\"$text\" class=\"as-link\" />";
		echo '</form>' . "\n";
	}
	
	admin_header('Articles');

	if(isset($_SESSION['error']))
	{
		echo '<div class="error">' . $_SESSION['error'] . '</div>';
		unset($_SESSION['error']);
	}

	$list = Article::getList();
	$article_id = $_GET['edit'];

	define('M_NONE', 0);
	define('M_EDIT', 1);
	define('M_ADD', 2);
	$mode = isset($_GET['edit']) ? M_EDIT : (isset($_GET['add']) ? M_ADD : M_NONE);


	echo '<a href="?add">Add article</a>&nbsp; ';
	echo '<a href="?rerender">Rerender all articles</a><br/>';
	echo '<table class="v-middle">';
	foreach($list as $article)
	{	
		echo '<tr><td>';
		echo '<a href="?edit=' . $article->id() . '">' . $article->full_path() . '</a>';
		echo '</td><td>';
		postlink('Del', '', array('delete' => $article->id()), array('onsubmit' => "return confirm('Are you sure to delete {$article->title()} at {$article->full_path()}?')"));
		echo '</td></tr>';
	}
	echo '</table>';

	if($mode == M_EDIT || $mode == M_ADD) 
	{
		if($mode == M_EDIT)
		{
			$article = Article::getById($article_id);
			form('?edit=' . $article->id());
			hidden('id', $article->id());
		}
		else
		{
			form('?create');
			echo '<h2>Create new article:</h2>';
			radios('lang', array('uk' => 'Ukrainian', 'en' => 'English', 'ru' => 'Russian'), 'uk');
			echo '<br/>Path: ';
			textbox('path', '', 100);
			echo '<br/>';
		}

		hidden('markdown', '', array('id' => 'markdown'));
		echo '<br/>Title: ';
		textbox('title', $mode == M_EDIT ? $article->title() : '', 50);?>
	<input type="submit" id="submit" value="Save" />
	<textarea name="article" id="article" style="width:100%; height: 20em;" ><?php
		echo $article->body();
	?></textarea>

<?php		echo '</form>';
?>
	<div class="wmd-preview"></div>
		<script src="wmd/wmd.js"></script>
		<script>
		document.getElementById('submit').onclick = submit;
		var x = document.getElementById('article');
		var y = document.getElementById('markdown');

		function submit()
		{
			y.value = x.value;
		}

		</script>
<?php } ?>

<?php echo admin_footer()?>
