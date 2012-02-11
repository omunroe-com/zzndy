<?php

class Record	{
	private $nick;
	private $score;
	private $time;
	private $ip;
	private $gametime;
	private $speed;
	
	function __construct(array &$get)	{
		$this->nick  = $get['nick'];
		$this->score = $get['score'];
		$this->time  = $get['time'];
		$this->gametime = $get['gameTime'];
		$this->ip    = $_SERVER['REMOTE_ADDR'];
		if($get['classic'])
			$this->speed = sprintf('%.0f', $get['speed']);
		else
			$this->speed = sprintf('%.1f', $get['speed']);
	}

	public function __toString()	{
		return "{$this->nick} scored {$this->score} on ".date("Y-m-j H:i", $this->time / 1000)." (played for ". date('i:s', $this->gametime/1000)."ms)";
	}

	public function report()	{
		$res = array();
		$res[] = $this->nick;
		$res[] = $this->score;
		$res[] = date("Y-m-j H:i", $this->time / 1000);
		$res[] = date('i:s', $this->gametime/1000);
		$res[] = $this->speed;
		
		return implode('|', $res);
	}
	
	static function Columns()	{
		$res = array();
		$res[] = "NICK";
		$res[] = "SCORE";
		$res[] = "DATE";
		$res[] = "TIME";
		$res[] = "SPEED";
		
		return implode('|', $res);
	}

	static function Compare(Record &$left, Record &$right)	{
		if($left->score == $right->score) return 0;
		return $left->score > $right->score ? 1: -1;
	}
	
	function toJson($pos = -1)	{
		return '{' . ($pos==-1?'':"pos:$pos,") . "nick:'{$this->nick}',score:{$this->score},time:{$this->time}}";
	}
}