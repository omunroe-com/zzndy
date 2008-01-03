fmt = {
save: "function save_${table}($${table}Data)\t{\n"+
"\tif($${table}Data['${table}_id']) update_${table}($${table}Data);\n"+
"\telse insert_${table}($${table}Data);\n}\n",

fn: "function ${action}_${table}($${table}Data)\t{\n" +
"\tglobal $mdb2;\b" +
"\tif(empty($mdb2->statements)) $mdb2->statements = array();\n"+
"\tif(!$mdb2->statements['${action}_${table}'])\t{\n" +
"\t\t$types = array(${types});\n"+
"\t\t$statement = '${statement}';\n"+
"\t\t$mdb2->statements['${action}_${table}'] = $mdb2->prepare($statement, $types, MDB2_PREPARE_MANIP);\n" +
"\t}\n"+
"\treturn $mdb2->statements['${action}_${table}']->execute($${table}Data);\n" +
"}\n",

insert: "INSERT INTO `${table}` (${cols}) VALUES (${placeholders})",
update: "UPDATE `${table}` SET ${sets} WHERE ${id} = :${id}"
}

schema="Competitions: competition_id int title varchar starts_on timestamp finishes_on timestamp created_on timestamp modified_on timestamp \n" +
"Foes: foe_id int player_id int 2nd_player_id int type enum created_on timestamp modified_on timestamp \n" +
"Games: game_id int competition_id int judge_id int 1st_foe_id int 2nd_foe_id int scheduled_on timestamp created_on timestamp modified_on timestamp \n" +
"Judges: judge_id int name varchar created_on timestamp modified_on timestamp \n" +
"Matches: match_id int game_id int serving_player_id int 1st_foe_score tinyint 2nd_foe_score tinyint created_on timestamp modified_on timestamp \n" +
"Players: player_id int name varchar rating decimal created_on timestamp modified_on timestamp \n" +
"Prizes: prize_id int title varchar foe_id int competition_id int created_on timestamp modified_on timestamp \n"
var miss=false, c=0;
var flds = [],types = [];
var result = schema.trim().split('\n').map(function(line){
	var [table, fields] = line.trim().split(':');
	table = table.trim();
	fields = fields.trim().split(' ');
	if(!table || !fields) return;
	c=0;
        miss=false;
	types = [];
	flds = [];
	var sql_update = fmt.update.format({
		table: table, 
		id: fields[0].trim(),
		sets: fields.reduce(make_update_sql, '')
	});

	var t=''
	for(var i=0;i<types.length; ++i)
		t += "'" + fields[i*2] + "' => '" + types[i] + "', ";
	types = t.replace(/, $/, '');

	sql_insert = fmt.insert.format({table: table, cols: flds.slice(1).join(', '), placeholders: flds.slice(1).reduce(function(state, item){return state + ':' + item.replace(/`/g, '') + ', '}, '').replace(/, $/, '')});

	return fmt.save.format({table: fields[0].replace(/_id/, '')})
		+ '\n' + fmt.fn.format({table: fields[0].replace(/_id/, ''), action: 'insert', statement: sql_insert, types: types})
		+ '\n' + fmt.fn.format({table: fields[0].replace(/_id/, ''), action: 'update', statement: sql_update, types: types})
	}).join('\n\n')

function make_update_sql(state, field){
	if(!field || typeof field != 'string' || miss) {miss=false; return state;}
	if(field == 'modified_on' || field == 'created_on') {miss=true; return state;}
	if(c%2)    {
		switch(field)    {
		case 'int':
			types.push('integer');
			break;
		case 'timestamp':
			types.push('timestamp');
			break;
		case 'decimal':
			types.push('float');
			break;
		default:
			types.push('text');
		}
			}
	else    {
		flds.push("`" + field + "`");
		state += (c>0?', ':'') + "`" + field + "` = :" + field;
	}
	++c;
	return state;
}
