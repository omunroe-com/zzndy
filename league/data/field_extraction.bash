#!/bin/bash

db=league
tables=$(mysql -u root -b $db -e "show tables" -E | awk  '/Tables/ {print $2}')

for i in $tables
do
	fields=$(mysql -u root -b $db -e "DESC $i"| awk '/^\\|/ {print $1, $2}')
	index=
	
	echo -n "\"$i: "

	for j in $fields
	do 
		[[ $j = 'Field' || $j = 'Type' ]] && continue	
		echo -n "${j/(*)/} "
	done
	echo '\n" +'
done 

