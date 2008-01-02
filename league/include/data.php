<?php
require_once 'MDB2.php';

$mdb2 =& MDB2::connect($_LEAGUE['dsn'], array('debug' => $_LEAGUE['db-debug']));
if (PEAR::isError($mdb2)) {
    die($mdb2->getMessage());
}

function save_competition($competitionData)	{
	if($competitionData['competition_id']) update_competition($competitionData);
	else insert_competition($competitionData);
}

function insert_competition($competitionData)	{
	global $mdb2;
	if(empty($mdb2->statements)) $mdb2->statements = array();
	if(!$mdb2->statements['insert_competition'])	{
		$types = array('competition_id' => 'integer', 'title' => 'text', 'starts_on' => 'timestamp', 'finishes_on' => 'timestamp');
		$statement = 'INSERT INTO `Competitions` (`title`, `starts_on`, `finishes_on`) VALUES (:title, :starts_on, :finishes_on)';
		$mdb2->statements['insert_competition'] = $mdb2->prepare($statement, $types, MDB2_PREPARE_MANIP);
	}
	return $mdb2->statements['insert_competition']->execute($competitionData);
}

function update_competition($competitionData)	{
	global $mdb2;
	if(empty($mdb2->statements)) $mdb2->statements = array();
	if(!$mdb2->statements['update_competition'])	{
		$types = array('competition_id' => 'integer', 'title' => 'text', 'starts_on' => 'timestamp', 'finishes_on' => 'timestamp');
		$statement = 'UPDATE `Competitions` SET `competition_id` = :competition_id, `title` = :title, `starts_on` = :starts_on, `finishes_on` = :finishes_on WHERE competition_id = :competition_id';
		$mdb2->statements['update_competition'] = $mdb2->prepare($statement, $types, MDB2_PREPARE_MANIP);
	}
	return $mdb2->statements['update_competition']->execute($competitionData);
}


function save_foe($foeData)	{
	if($foeData['foe_id']) update_foe($foeData);
	else insert_foe($foeData);
}

function insert_foe($foeData)	{
	global $mdb2;
	if(empty($mdb2->statements)) $mdb2->statements = array();
	if(!$mdb2->statements['insert_foe'])	{
		$types = array('foe_id' => 'integer', 'player_id' => 'integer', '2nd_player_id' => 'integer', 'type' => 'text');
		$statement = 'INSERT INTO `Foes` (`player_id`, `2nd_player_id`, `type`) VALUES (:player_id, :2nd_player_id, :type)';
		$mdb2->statements['insert_foe'] = $mdb2->prepare($statement, $types, MDB2_PREPARE_MANIP);
	}
	return $mdb2->statements['insert_foe']->execute($foeData);
}

function update_foe($foeData)	{
	global $mdb2;
	if(empty($mdb2->statements)) $mdb2->statements = array();
	if(!$mdb2->statements['update_foe'])	{
		$types = array('foe_id' => 'integer', 'player_id' => 'integer', '2nd_player_id' => 'integer', 'type' => 'text');
		$statement = 'UPDATE `Foes` SET `foe_id` = :foe_id, `player_id` = :player_id, `2nd_player_id` = :2nd_player_id, `type` = :type WHERE foe_id = :foe_id';
		$mdb2->statements['update_foe'] = $mdb2->prepare($statement, $types, MDB2_PREPARE_MANIP);
	}
	return $mdb2->statements['update_foe']->execute($foeData);
}


function save_game($gameData)	{
	if($gameData['game_id']) update_game($gameData);
	else insert_game($gameData);
}

function insert_game($gameData)	{
	global $mdb2;
	if(empty($mdb2->statements)) $mdb2->statements = array();
	if(!$mdb2->statements['insert_game'])	{
		$types = array('game_id' => 'integer', 'competition_id' => 'integer', 'judge_id' => 'integer', '1st_foe_id' => 'integer', '2nd_foe_id' => 'integer', 'scheduled_on' => 'timestamp');
		$statement = 'INSERT INTO `Games` (`competition_id`, `judge_id`, `1st_foe_id`, `2nd_foe_id`, `scheduled_on`) VALUES (:competition_id, :judge_id, :1st_foe_id, :2nd_foe_id, :scheduled_on)';
		$mdb2->statements['insert_game'] = $mdb2->prepare($statement, $types, MDB2_PREPARE_MANIP);
	}
	return $mdb2->statements['insert_game']->execute($gameData);
}

function update_game($gameData)	{
	global $mdb2;
	if(empty($mdb2->statements)) $mdb2->statements = array();
	if(!$mdb2->statements['update_game'])	{
		$types = array('game_id' => 'integer', 'competition_id' => 'integer', 'judge_id' => 'integer', '1st_foe_id' => 'integer', '2nd_foe_id' => 'integer', 'scheduled_on' => 'timestamp');
		$statement = 'UPDATE `Games` SET `game_id` = :game_id, `competition_id` = :competition_id, `judge_id` = :judge_id, `1st_foe_id` = :1st_foe_id, `2nd_foe_id` = :2nd_foe_id, `scheduled_on` = :scheduled_on WHERE game_id = :game_id';
		$mdb2->statements['update_game'] = $mdb2->prepare($statement, $types, MDB2_PREPARE_MANIP);
	}
	return $mdb2->statements['update_game']->execute($gameData);
}


function save_judge($judgeData)	{
	if($judgeData['judge_id']) update_judge($judgeData);
	else insert_judge($judgeData);
}

function insert_judge($judgeData)	{
	global $mdb2;
	if(empty($mdb2->statements)) $mdb2->statements = array();
	if(!$mdb2->statements['insert_judge'])	{
		$types = array('judge_id' => 'integer', 'name' => 'text');
		$statement = 'INSERT INTO `Judges` (`name`) VALUES (:name)';
		$mdb2->statements['insert_judge'] = $mdb2->prepare($statement, $types, MDB2_PREPARE_MANIP);
	}
	return $mdb2->statements['insert_judge']->execute($judgeData);
}

function update_judge($judgeData)	{
	global $mdb2;
	if(empty($mdb2->statements)) $mdb2->statements = array();
	if(!$mdb2->statements['update_judge'])	{
		$types = array('judge_id' => 'integer', 'name' => 'text');
		$statement = 'UPDATE `Judges` SET `judge_id` = :judge_id, `name` = :name WHERE judge_id = :judge_id';
		$mdb2->statements['update_judge'] = $mdb2->prepare($statement, $types, MDB2_PREPARE_MANIP);
	}
	return $mdb2->statements['update_judge']->execute($judgeData);
}


function save_match($matchData)	{
	if($matchData['match_id']) update_match($matchData);
	else insert_match($matchData);
}

function insert_match($matchData)	{
	global $mdb2;
	if(empty($mdb2->statements)) $mdb2->statements = array();
	if(!$mdb2->statements['insert_match'])	{
		$types = array('match_id' => 'integer', 'game_id' => 'integer', 'serving_player_id' => 'integer', '1st_foe_score' => 'text', '2nd_foe_score' => 'text');
		$statement = 'INSERT INTO `Matches` (`game_id`, `serving_player_id`, `1st_foe_score`, `2nd_foe_score`) VALUES (:game_id, :serving_player_id, :1st_foe_score, :2nd_foe_score)';
		$mdb2->statements['insert_match'] = $mdb2->prepare($statement, $types, MDB2_PREPARE_MANIP);
	}
	return $mdb2->statements['insert_match']->execute($matchData);
}

function update_match($matchData)	{
	global $mdb2;
	if(empty($mdb2->statements)) $mdb2->statements = array();
	if(!$mdb2->statements['update_match'])	{
		$types = array('match_id' => 'integer', 'game_id' => 'integer', 'serving_player_id' => 'integer', '1st_foe_score' => 'text', '2nd_foe_score' => 'text');
		$statement = 'UPDATE `Matches` SET `match_id` = :match_id, `game_id` = :game_id, `serving_player_id` = :serving_player_id, `1st_foe_score` = :1st_foe_score, `2nd_foe_score` = :2nd_foe_score WHERE match_id = :match_id';
		$mdb2->statements['update_match'] = $mdb2->prepare($statement, $types, MDB2_PREPARE_MANIP);
	}
	return $mdb2->statements['update_match']->execute($matchData);
}


function save_player($playerData)	{
	if($playerData['player_id']) update_player($playerData);
	else insert_player($playerData);
}

function insert_player($playerData)	{
	global $mdb2;
	if(empty($mdb2->statements)) $mdb2->statements = array();
	if(!$mdb2->statements['insert_player'])	{
		$types = array('player_id' => 'integer', 'name' => 'text', 'rating' => 'float');
		$statement = 'INSERT INTO `Players` (`name`, `rating`) VALUES (:name, :rating)';
		$mdb2->statements['insert_player'] = $mdb2->prepare($statement, $types, MDB2_PREPARE_MANIP);
	}
	return $mdb2->statements['insert_player']->execute($playerData);
}

function update_player($playerData)	{
	global $mdb2;
	if(empty($mdb2->statements)) $mdb2->statements = array();
	if(!$mdb2->statements['update_player'])	{
		$types = array('player_id' => 'integer', 'name' => 'text', 'rating' => 'float');
		$statement = 'UPDATE `Players` SET `player_id` = :player_id, `name` = :name, `rating` = :rating WHERE player_id = :player_id';
		$mdb2->statements['update_player'] = $mdb2->prepare($statement, $types, MDB2_PREPARE_MANIP);
	}
	return $mdb2->statements['update_player']->execute($playerData);
}


function save_prize($prizeData)	{
	if($prizeData['prize_id']) update_prize($prizeData);
	else insert_prize($prizeData);
}

function insert_prize($prizeData)	{
	global $mdb2;
	if(empty($mdb2->statements)) $mdb2->statements = array();
	if(!$mdb2->statements['insert_prize'])	{
		$types = array('prize_id' => 'integer', 'title' => 'text', 'foe_id' => 'integer', 'competition_id' => 'integer');
		$statement = 'INSERT INTO `Prizes` (`title`, `foe_id`, `competition_id`) VALUES (:title, :foe_id, :competition_id)';
		$mdb2->statements['insert_prize'] = $mdb2->prepare($statement, $types, MDB2_PREPARE_MANIP);
	}
	return $mdb2->statements['insert_prize']->execute($prizeData);
}

function update_prize($prizeData)	{
	global $mdb2;
	if(empty($mdb2->statements)) $mdb2->statements = array();
	if(!$mdb2->statements['update_prize'])	{
		$types = array('prize_id' => 'integer', 'title' => 'text', 'foe_id' => 'integer', 'competition_id' => 'integer');
		$statement = 'UPDATE `Prizes` SET `prize_id` = :prize_id, `title` = :title, `foe_id` = :foe_id, `competition_id` = :competition_id WHERE prize_id = :prize_id';
		$mdb2->statements['update_prize'] = $mdb2->prepare($statement, $types, MDB2_PREPARE_MANIP);
	}
	return $mdb2->statements['update_prize']->execute($prizeData);
}
