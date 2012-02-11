<?php
class Stat	{
	private $lines;
	private $figures;
	private $score;
	private $drops;
	private $dropLevels;
	private $gameTime;
	
	function __construct(array &$get = null)	{
		$this->lines      = $get['lines'];
		$this->figures    = $get['figures'];
		$this->score      = $get['score'];
		$this->drops      = $get['drops'];
		$this->dropLevels = $get['dropLevels'];
		$this->gameTime   = $get['gameTime'];
	}

	function add(Stat &$obj)	{
		$this->lines      = bcadd($this->lines,      $obj->lines);
		$this->figures    = bcadd($this->figures,    $obj->figures);
		$this->score      = bcadd($this->score,      $obj->score);
		$this->drops      = bcadd($this->drops,      $obj->drops);
		$this->dropLevels = bcadd($this->dropLevels, $obj->dropLevels);
		$this->gameTime   = bcadd($this->gameTime,   $obj->gameTime);
	}

	function toJson()	{
		return "{lines:{$this->lines},figures:{$this->figures},score:{$this->score},drops:{$this->drops},dropLevels:{$this->dropLevels},gameTime:{$this->gameTime}}";
	}
}
