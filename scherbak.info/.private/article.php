<?php
require_once $include . 'db.php';

class Article
{
	private $id; 
	private $lang; 
	private $path; 
	private $title; 
	private $body; 
	private $html; 

	public function full_path()
	{
		return $this->lang . '/' . $this->path;
	}

	public function id()
	{
		return $this->id;
	}
	
	public function title()
	{
		return $this->title;
	}
	
	public function body()
	{
		return $this->body;
	}

	public function set_body($body)
	{
		$this->body = $body; // markdown code
		return $this->body;
	}

	public function html()
	{
		return $this->html;
	}

	public function set_html($html)
	{
		$this->html = $html;
		return $this->html;
	}

	public function set_title($title)
	{
		$this->title = htmlspecialchars($title, ENT_QUOTES);
		return $this->title;
	}

	public static function getById($id)
	{
		global $db;

		$stmt = $db->prepare('SELECT lang, path, title, body FROM sch_articles WHERE article_id = ? LIMIT 1');
		$stmt->bind_param('d', $id);

		$stmt->execute();
		$stmt->bind_result($lang, $path, $title, $text);
		$stmt->fetch();
		$stmt->close();
		
		$stmt = $db->prepare('SELECT lang, path, title, html FROM sch_articles WHERE article_id = ? LIMIT 1');
		$stmt->bind_param('d', $id);

		$stmt->execute();
		$stmt->bind_result($lang, $path, $title, $html);
		$stmt->fetch();
		$stmt->close();

		return new Article($id, $lang, $path, $title, $text, $html);
	}

	public function save()
	{
		global $db;

		$stmt = $db->prepare('UPDATE sch_articles SET lang = ?, path = ?, title = ?, body = ?, html = ? WHERE article_id = ?');
		$stmt->bind_param('sssssd', $this->lang, $this->path, $this->title, $this->body, $this->html, $this->id);
		$stmt->execute();
	}

	public static function delete($id)
	{
		global $db;

		$stmt = $db->prepare('DELETE FROM sch_articles WHERE article_id = ?');
		$stmt->bind_param('d', $id);
		$stmt->execute();
	}

	public static function create($lang, $path, $title, $text, $html)
	{
		global $db;

		$stmt = $db->prepare('INSERT INTO sch_articles (lang, path, title, body, html) VALUES (?, ?, ?, ?, ?)');
		$stmt->bind_param('sssss', 
			htmlspecialchars($lang, ENT_QUOTES), 
			htmlspecialchars($path, ENT_QUOTES), 
			htmlspecialchars($title, ENT_QUOTES), 
			$text, // markdown code
			$html
		);
		if(!$stmt->execute())
			return $db->error;

		$res = $db->query('SELECT LAST_INSERT_ID()');
		$obj = $res->fetch_row();
	    $article_id = $obj[0];

		return new Article($article_id, $lang, $path, $title, $text, $html);
	}
	
	public static function getByPath($lang, $path)
	{
		global $db;
		
		$stmt = $db->prepare('SELECT article_id, title, html FROM sch_articles WHERE lang = ? AND path = ? LIMIT 1');
		$stmt->bind_param('ss', $lang, $path);
		$stmt->execute();
		$stmt->bind_result($id, $title, $html);
		$stmt->fetch();
		$stmt->close();
		
		$stmt = $db->prepare('SELECT article_id, title, body FROM sch_articles WHERE lang = ? AND path = ? LIMIT 1');
		$stmt->bind_param('ss', $lang, $path);
		$stmt->execute();
		$stmt->bind_result($id, $title, $body);
		$stmt->fetch();
		$stmt->close();
	
		return new Article($id, $lang, $path, $title, $body, $html);
	}

	private function __construct($id, $lang, $path, $title, $text, $html)
	{
		$this->id = $id;
		$this->lang = $lang;
		$this->path = $path;
		$this->title = $title;
		$this->body = $text;
		$this->html = $html;
	}

	public static function getList()
	{
		global $db;
		$list = array();
		$res = $db->query('SELECT article_id, lang, path, title, body, html FROM sch_articles');

		if($res)
		{
			while($row = $res->fetch_row())
				$list[] = new Article($row[0], $row[1], $row[2], $row[3], $row[4], $row[5]);
		}

		return $list;
	}
}
