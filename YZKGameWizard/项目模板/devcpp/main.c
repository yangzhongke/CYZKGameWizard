#include <stdlib.h>
#include <stdio.h>
#include <yzkgame.h>

void gameMain(void)
{
	setGameTitle("yangzhongke");
	setGameSize(338, 600);
	pauseGame(10000);
}

int main(void)
{
	rpInit(gameMain);
	return 0;
}
