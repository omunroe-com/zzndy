<?php

define('DATA_DIR', dirname(dirname(__FILE__)) . '/data/');

class Top10k	{
	const data_dir = DATA_DIR;
	const max_records = 1000;
	const show_records = 10;
	const goldsection = 1.6180339887498948482;

	private $table;
	private $stats;
	private $record;
	private $position;
	private $data;

	private $stats_file;
	private $table_file;

	function __construct($data)	{
		$this->stats_file = self::data_dir . 'stats.dat';
		$this->table_file = self::data_dir . 'table.dat';

		$this->data = $data;
		if($this->data['classic'] == '1')	{
			$this->table_file = self::data_dir . 'table-classic.dat';
			$this->stats_file = self::data_dir . 'stats-classic.dat';
		}
		$this->load_stats();
		$this->load_table();

		if($this->data_valid)	{
			$this->record = new Record($this->data);
			$this->stats->add(new Stat($this->data));
		}
	}

	function __destruct()	{
		$this->save_stats();
		$this->save_table();
	}

	private function save_stats()	{
		file_put_contents($this->stats_file, serialize($this->stats));
	}

	private function save_table()	{
		file_put_contents($this->table_file, serialize($this->table));
	}

	function __get($key)	{
		if($key == 'data_valid')
			return $this->data_valid();
	}

	private function load_table()	{
		if(file_exists($this->table_file))
			$this->table = unserialize(file_get_contents($this->table_file));
		if(!$this->table)
			$this->table = array();

	}

	private function load_stats()	{
		if(file_exists($this->stats_file))
			$this->stats = unserialize(file_get_contents($this->stats_file));
		if(!$this->stats)	
			$this->stats = new Stat();
	}

	private function data_valid()	{
		return isset($this->data['nick']) && isset($this->data['score']) 
		    && isset($this->data['time']) && isset($this->data['lines']) 
		    && isset($this->data['figures']) && isset($this->data['drops']) 
		    && isset($this->data['dropLevels']) && isset($this->data['gameTime'])
			&& isset($this->data['speed']) && is_numeric($this->data['speed'])
		    && is_numeric($this->data['score']) && is_numeric($this->data['time'])
		    && is_numeric($this->data['lines']) && is_numeric($this->data['figures'])
		    && is_numeric($this->data['gameTime']) && is_numeric($this->data['drops'])
		    && is_numeric($this->data['dropLevels']);
	}

	public function placeCurrent()	{
		if(!$this->record) return $this->position = -1;
		$maxi = count($this->table) - 1;
		if($maxi<0)	{
			$this->table = array($this->record);
			return $this->position = 0;
		}
		
		if(Record::compare($this->table[$maxi], $this->record) > 0)	{
			if($maxi + 1 < self::max_records)	{
				$this->table[] = $this->record;
				return $this->position = ++$maxi;
			}
			else	
				return $this->position = -1;
		}
		$i = $maxi;
		
		while($i >= 0 && Record::Compare($this->table[$i], $this->record) <= 0) --$i;
		
		array_splice($this->table, $i+1, 0, array($this->record));
		if(count($this->table) > self::max_records)
			array_splice($this->table, self::max_records);

		return $this->position = ++$i;
	}

	public function toJson()	{
		$maxi = count($this->table) - 1;
		$res  = 'json={stats:' . $this->stats->toJson() . ',';
		$res .= 'total:' . count($this->table) . ',';
		$res .= 'pos:' . $this->position . ',';
		
		$extr = array();
		$i=0;
		
		$ints = $this->getIntervals($maxi);
		foreach($ints as $i => $int)	{
			if($i)	{
				if($int[0] - $ints[$i-1][1] == 2)
					$extr[] = $this->table[$int[0] - 1]->toJson($int[0] - 1);
				else
					$extr[] = 0;
			}
			for($j = $int[0]; $j<=$int[1]; ++$j)
				$extr[] = $this->table[$j]->toJson($j);
		}
		
		$res .= 'extract:[' . join(',', $extr) . ']';
		return $res . '}';
	}
	
	private function getIntervals($maxi)	{
		if($maxi < self::show_records)return array(array(0, $maxi));
	
		$mid = floor((self::show_records-2) / 2);	// part of records table shown in the middle
		$top = ceil($mid / self::goldsection);		// first records shown
		$bot = self::show_records - 2 - $top - $mid;// records from the bottom
		
		if($bot==0)	{
			++$bot;
			--$mid;
		}
		
		$paddingTop = floor(($mid - 1) / 2);
		$paddingBot = $mid - 1 - $paddingTop;
				
		if($this->position == -1 || $this->position - $paddingTop - $top <= 0 || $this->position + $paddingBot + $bot >= $maxi)	{
			$shift = $this->position + $paddingBot + $bot < $maxi;
			return array(
				array(0, floor(self::show_records * 2 / 3) -2 + ($shift)), 
				array($maxi - ceil(self::show_records/3) + 1 + ($shift), $maxi)
			);
		}
		
		return array(
			array(0, $top - 1),
			array($this->position - $paddingTop, $this->position + $paddingBot),
			array($maxi - $bot + 1, $maxi)
		);
	}

}

