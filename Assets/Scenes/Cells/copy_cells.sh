#!/bin/bash

# This script takes in two argument: width and height
# then it copies Cell_000_000.unity enough times to generate a grid of that size

if [ -z "$1" ] || [ -z "$2" ]
then
	echo "usage: copy_cells.sh [width] [height]"
	exit 1
fi

width=$1
height=$2

# Verify we were given numbers, both less than 1000
numPattern='^[0-9]+$'

if ! [[ $width =~ $numPattern ]]
then
	echo "Error: $width not a number" >&2
	exit 1
fi

if ! [[ $height =~ $numPattern ]]
then
	echo "Error: $height not a number" >&2
	exit 1
fi


# Verify Cell Exists
if ! [ -e "Cell_000_000.unity" ]
then
	echo "Error: Cell_000_000.unity doesn't exist"
	exit 1
fi

maxHeight=$(( $height - 1))
maxWidth=$(( $width - 1))

if [[ $maxHeight -lt 0 ]]
then
	maxHeight=0
fi

if [[ $maxWidth -lt 0 ]]
then
	maxWidth=0
fi

for column in `seq -f "%03g" 0 $maxHeight`
do
	for row in `seq -f "%03g" 0 $maxWidth`
	do
		if [ $row == "000" ] && [ $column == "000" ]
		then
			continue
		fi

		newScene="Cell_"$row"_"$column".unity"
		echo $newScene

		if [ -f $newScene ]
		then
			rm $newScene
		fi

		if [ -f "$newScene.meta" ]
		then
			rm "$newScene.meta"
		fi

		cp "Cell_000_000.unity" $newScene
	done
done
