using System;
using System.Diagnostics;
using System.Formats.Asn1;

namespace RogueAttemptMaybe
{
    //Current bugs:
    //enemies attack your last position.


    //To-Do list:
    //Map R
    //GUI *
    //Random (bruikbare) levels van 64x32 characters. *

    //Player:
    //Move 1 space with arrows R
    //HP *      Ma
    //Money *
    //Attack *
    //Inventory *

    //Enemies:
    //Movement R
    //HP *      Ma
    //Attack * R

    //MaybeMaybeMaybe:
    //Different types of enemies?
    //Classes?
    internal class Program
    {
        //Characters
        static string floorCharacter = "* ";
        static string character = "@ ";
        static string enemy1 = "E ";
        //Map sizes
<<<<<<< Updated upstream
        static int mapSizeL = 8;
        static int[] currentPlayerPosition = {0,0};
=======
        static int mapSizeL = 16;
        static int[] currentPlayerPosition = { 0, 0 };
>>>>>>> Stashed changes
        static int[] currentEnemyPosition = { 0, 0 };
        static int mapSizeW = 16;
        static int innerMapSizeL = 14;
        static int innerMapSizeW = 14;
        //Attack
        static int testPlayerAttack = 10;
        static int testEnemyAttack = 5;
        static int playerHp = 10;
        static bool enemyAlive = true;
        static bool attackHappened = false;
        //Input map here
<<<<<<< Updated upstream
        static string[,] map1 = new string[8, 8] 
        {
        {"|-" , "--" , "--" ,"--" , "--" ,"--" , "--" , "| "},
        {"| " , floorCharacter , floorCharacter ,floorCharacter , floorCharacter ,floorCharacter , floorCharacter , "| "},
        {"| " , floorCharacter , floorCharacter ,floorCharacter , floorCharacter ,floorCharacter , floorCharacter , "| "},
        {"| " , floorCharacter ,"| " ,floorCharacter , floorCharacter ,floorCharacter , floorCharacter , "| "},
        {"| " , floorCharacter , floorCharacter ,floorCharacter , floorCharacter ,floorCharacter , floorCharacter , "| "},
        {"| " , floorCharacter , floorCharacter ,floorCharacter , floorCharacter ,floorCharacter , floorCharacter , "| "},
        {"| " , floorCharacter , floorCharacter ,floorCharacter , floorCharacter ,floorCharacter , floorCharacter , "| "},
        {"|-" , "--" , "--" , "--" , "--" , "--" , "--" , "| "}
        };
=======
        static string[,] map1 = new string[16,16]
        //{
/*        {floorCharacter , floorCharacter , floorCharacter ,floorCharacter , floorCharacter ,floorCharacter , floorCharacter , floorCharacter},
        {floorCharacter , floorCharacter , floorCharacter ,floorCharacter , floorCharacter ,floorCharacter , floorCharacter , floorCharacter},
        {floorCharacter , floorCharacter , floorCharacter ,floorCharacter , floorCharacter ,floorCharacter , floorCharacter , floorCharacter},
        {floorCharacter , floorCharacter ,floorCharacter ,floorCharacter , floorCharacter ,floorCharacter , floorCharacter , floorCharacter},
        {floorCharacter , floorCharacter , floorCharacter ,floorCharacter , floorCharacter ,floorCharacter , floorCharacter , floorCharacter},
        {floorCharacter , floorCharacter , floorCharacter ,floorCharacter , floorCharacter ,floorCharacter , floorCharacter , floorCharacter},
        {floorCharacter , floorCharacter , floorCharacter ,floorCharacter , floorCharacter ,floorCharacter , floorCharacter , floorCharacter},
        {floorCharacter, floorCharacter , floorCharacter , floorCharacter , floorCharacter , floorCharacter , floorCharacter , floorCharacter}*/
/*        {"00|" , "01|" , "02|" ,"03|" , "04|" ,"05|" , "06|" , "07|"},
        {"10|" , "11|" , "12|" ,"13|" , "14|" ,"15|" , "16|" , "17|"},
        {"20|" , "21|" , "22|" ,"23|" , "24|" ,"25|" , "26|" , "27|"},
        {"30|" , "31|" , "32|" ,"33|" , "34|" ,"35|" , "36|" , "37|"},
        {"40|" , "41|" , "42|" ,"43|" , "44|" ,"45|" , "46|" , "47|"},
        {"50|" , "51|" , "52|" ,"53|" , "54|" ,"55|" , "56|" , "57|"},
        {"60|" , "61|" , "62|" ,"63|" , "64|" ,"65|" , "66|" , "67|"},
        {"70|" , "71|" , "72|" ,"73|" , "74|" ,"75|" , "76|" , "77|"}
        }*/;
        static bool firstTimeMap = true;
>>>>>>> Stashed changes
        static void Main(string[] args)
        {
            //Takes a random spawn for the player
            Random rpos1 = new Random();
            int pos1 = rpos1.Next(1,innerMapSizeL);
            Random rpos2 = new Random();
            int pos2 = rpos2.Next(1,innerMapSizeW);
            currentPlayerPosition[0] = pos1;
            currentPlayerPosition[1] = pos2;
            //Takes a random spawn for 1 enemy
            Random rpos3 = new Random();
            int pos3 = rpos3.Next(1, innerMapSizeL);
            Random rpos4 = new Random();
            int pos4 = rpos4.Next(1, innerMapSizeW);
            currentEnemyPosition[0] = pos3;
            currentEnemyPosition[1] = pos4;
            //Starts game
            DrawMap1();
<<<<<<< Updated upstream
            AwaitMovementKey();
=======
            firstTimeMap = false;
                AwaitMovementKey();
            }
>>>>>>> Stashed changes
        }
        static void Move(ConsoleKey key)
        {
            EnemyMove();
            switch (key)
            {
                case ConsoleKey.DownArrow:
                {
                    if (map1[currentPlayerPosition[0] + 1 , currentPlayerPosition[1]] == floorCharacter)
                    {
                        map1[currentPlayerPosition[0], currentPlayerPosition[1]] = floorCharacter;
                        currentPlayerPosition[0] = currentPlayerPosition[0] + 1;
                        map1[currentPlayerPosition[0], currentPlayerPosition[1]] = character;
                        CheckIfAttack("Player");
                        DrawMap1();
                        AwaitMovementKey();
                    }
                    else
                    {
                            CheckIfAttack("Player");
                            DrawMap1();
                        AwaitMovementKey();
                    }
                    break;
                    }
                case ConsoleKey.UpArrow:
                {
                    if (map1[currentPlayerPosition[0] - 1, currentPlayerPosition[1]] == floorCharacter)
                    {
                        map1[currentPlayerPosition[0], currentPlayerPosition[1]] = floorCharacter;
                        currentPlayerPosition[0] = currentPlayerPosition[0] - 1;
                        map1[currentPlayerPosition[0], currentPlayerPosition[1]] = character;
                            CheckIfAttack("Player");
                            DrawMap1();
                        AwaitMovementKey();
                    }
                    else
                    {
                            CheckIfAttack("Player");
                            DrawMap1();
                        AwaitMovementKey();
                    }
                    break;
                }
                case ConsoleKey.LeftArrow:
                {
                    if (map1[currentPlayerPosition[0], currentPlayerPosition[1] - 1] == floorCharacter)
                    {
                        map1[currentPlayerPosition[0], currentPlayerPosition[1]] = floorCharacter;
                        currentPlayerPosition[1] = currentPlayerPosition[1] - 1;
                        map1[currentPlayerPosition[0], currentPlayerPosition[1]] = character;
                            CheckIfAttack("Player");
                            DrawMap1();
                        AwaitMovementKey();
                    }
                    else
                    {
                            CheckIfAttack("Player");
                            DrawMap1();
                        AwaitMovementKey();
                    }
                    break;
                    }
                case ConsoleKey.RightArrow:
                {
                    if (map1[currentPlayerPosition[0], currentPlayerPosition[1] + 1] == floorCharacter)
                    {
                        map1[currentPlayerPosition[0], currentPlayerPosition[1]] = floorCharacter;
                        currentPlayerPosition[1] = currentPlayerPosition[1] + 1;
                        map1[currentPlayerPosition[0], currentPlayerPosition[1]] = character;
                            CheckIfAttack("Player");
                            DrawMap1();
                        AwaitMovementKey();
                    }
                    else
                    {
                            CheckIfAttack("Player");
                            DrawMap1();
                        AwaitMovementKey();
                    }
                    break;
                }

            }
        }
        static void AwaitMovementKey()
        {
            //makes sure you actually use an arrow
            ConsoleKey key = Console.ReadKey().Key;
            if (key == ConsoleKey.DownArrow || key == ConsoleKey.UpArrow || key == ConsoleKey.LeftArrow || key == ConsoleKey.RightArrow)
            {
                Move(key);
            }else
            {
                //If key isnt an arrow it restarts
                AwaitMovementKey();
            }
        }
        static void EnemyMove()
        {
            if (enemyAlive == true)
            {
                int L = currentPlayerPosition[0] - currentEnemyPosition[0];
                int W = currentPlayerPosition[1] - currentEnemyPosition[1];
                if (W == 0)
                {
                    if (currentPlayerPosition[0] >= currentEnemyPosition[0])
                    {
                        if (map1[currentEnemyPosition[0] + 1, currentEnemyPosition[1]] == floorCharacter)
                        {
                            map1[currentEnemyPosition[0], currentEnemyPosition[1]] = floorCharacter;
                            currentEnemyPosition[0] = currentEnemyPosition[0] + 1;
                            map1[currentEnemyPosition[0], currentEnemyPosition[1]] = enemy1;
                            CheckIfAttack("Enemy");
                        }
                        else
                        {
                            L = 0;
                        }
                    }
                    else if (currentPlayerPosition[0] <= currentEnemyPosition[0])
                    {
                        if (map1[currentEnemyPosition[0] - 1, currentEnemyPosition[1]] == floorCharacter)
                        {
                            map1[currentEnemyPosition[0], currentEnemyPosition[1]] = floorCharacter;
                            currentEnemyPosition[0] = currentEnemyPosition[0] - 1;
                            map1[currentEnemyPosition[0], currentEnemyPosition[1]] = enemy1;
                            CheckIfAttack("Enemy");
                        }
                        else
                        {
                            L = 0;
                        }
                    }
                }
                else if (L == 0)
                {
                    if (currentPlayerPosition[1] <= currentEnemyPosition[1])
                    {
                        if (map1[currentEnemyPosition[0], currentEnemyPosition[1] - 1] == floorCharacter)
                        {
                            map1[currentEnemyPosition[0], currentEnemyPosition[1]] = floorCharacter;
                            currentEnemyPosition[1] = currentEnemyPosition[1] - 1;
                            map1[currentEnemyPosition[0], currentEnemyPosition[1]] = enemy1;
                            CheckIfAttack("Enemy");
                        }
                        else
                        {
                            W = 0;
                        }
                    }
                    else if (currentPlayerPosition[1] >= currentEnemyPosition[1])
                    {
                        if (map1[currentEnemyPosition[0], currentEnemyPosition[1] + 1] == floorCharacter)
                        {
                            map1[currentEnemyPosition[0], currentEnemyPosition[1]] = floorCharacter;
                            currentEnemyPosition[1] = currentEnemyPosition[1] + 1;
                            map1[currentEnemyPosition[0], currentEnemyPosition[1]] = enemy1;
                            CheckIfAttack("Enemy");
                        }
                        else
                        {
                            W = 0;
                        }
                    }
                }
                else if (L < W)
                {
                    if (currentPlayerPosition[1] <= currentEnemyPosition[1])
                    {
                        if (map1[currentEnemyPosition[0], currentEnemyPosition[1] - 1] == floorCharacter)
                        {
                            map1[currentEnemyPosition[0], currentEnemyPosition[1]] = floorCharacter;
                            currentEnemyPosition[1] = currentEnemyPosition[1] - 1;
                            map1[currentEnemyPosition[0], currentEnemyPosition[1]] = enemy1;
                            CheckIfAttack("Enemy");
                        }
                        else
                        {
                            L = 0;
                        }
                    }
                    else if (currentPlayerPosition[1] >= currentEnemyPosition[1])
                    {
                        if (map1[currentEnemyPosition[0], currentEnemyPosition[1] + 1] == floorCharacter)
                        {
                            map1[currentEnemyPosition[0], currentEnemyPosition[1]] = floorCharacter;
                            currentEnemyPosition[1] = currentEnemyPosition[1] + 1;
                            map1[currentEnemyPosition[0], currentEnemyPosition[1]] = enemy1;
                            CheckIfAttack("Enemy");
                        }
                        else
                        {
                            L = 0;
                        }
                    }
                }
                else if (W < L)
                {
                    if (currentPlayerPosition[0] >= currentEnemyPosition[0])
                    {
                        if (map1[currentEnemyPosition[0] + 1, currentEnemyPosition[1]] == floorCharacter)
                        {
                            map1[currentEnemyPosition[0], currentEnemyPosition[1]] = floorCharacter;
                            currentEnemyPosition[0] = currentEnemyPosition[0] + 1;
                            map1[currentEnemyPosition[0], currentEnemyPosition[1]] = enemy1;
                            CheckIfAttack("Enemy");
                        }
                        else
                        {
                            W = 0;
                        }
                    }
                    else if (currentPlayerPosition[0] <= currentEnemyPosition[0])
                    {
                        if (map1[currentEnemyPosition[0] - 1, currentEnemyPosition[1]] == floorCharacter)
                        {
                            map1[currentEnemyPosition[0], currentEnemyPosition[1]] = floorCharacter;
                            currentEnemyPosition[0] = currentEnemyPosition[0] - 1;
                            map1[currentEnemyPosition[0], currentEnemyPosition[1]] = enemy1;
                            CheckIfAttack("Enemy");
                        }
                        else
                        {
                            W = 0;
                        }
                    }
                }
            }
        }
        static void CheckIfAttack(string attacker)
        {
            if (currentPlayerPosition[0]== currentEnemyPosition[0] -1 || currentPlayerPosition[0] == currentEnemyPosition[0] +1 || currentPlayerPosition[1]== currentEnemyPosition[1] -1 || currentPlayerPosition[1] == currentEnemyPosition[1]+1)
            {
                if (attackHappened == false)
                {
<<<<<<< Updated upstream
                    Console.WriteLine("WouldAttack");
=======
                    if (map1[currentEnemyPosition[0] - 1, currentEnemyPosition[1]] == character || map1[currentEnemyPosition[0] + 1, currentEnemyPosition[1]] == character || map1[currentEnemyPosition[0], currentEnemyPosition[1] - 1] == character || map1[currentEnemyPosition[0], currentEnemyPosition[1] + 1] == character)
                    {
                        if (attacker == "Player")
                        {
                            Random rnd2 = new Random();
                            float crit2 = rnd2.Next(0, 100);
                            if (crit2 <= critChance)
                            {
                                Console.WriteLine("Crit");
                                playerHp = playerHp - (testEnemyAttack * critMultiplier);
                                Console.WriteLine(playerHp + "Player");
                            }
                            else
                            {
                                Console.WriteLine("Not crit");
                                playerHp = playerHp - testEnemyAttack;
                                Console.WriteLine(playerHp + "Player");
                            }
                            Random rnd = new Random();
                            float crit1 = rnd.Next(0, 100);
                            if (crit1 <= critChance)
                            {
                                Console.WriteLine("Crit");
                                enemyHp = enemyHp - (testPlayerAttack * critMultiplier);
                                Console.WriteLine(enemyHp + "Enemy");
                            }
                            else
                            {
                                Console.WriteLine("Not crit");
                                enemyHp = enemyHp - testPlayerAttack;
                                Console.WriteLine(enemyHp + "Enemy");
                            }
                            attackHappened = true;
                        }
                        else if (attacker == "Enemy")
                        {
                            Random rnd = new Random();
                            float crit1 = rnd.Next(0, 100);
                            if (crit1 <= critChance)
                            {
                                Console.WriteLine("Crit");
                                enemyHp = enemyHp - (testPlayerAttack * critMultiplier);
                                Console.WriteLine(enemyHp + "Enemy");
                            }
                            else
                            {
                                Console.WriteLine("Not crit");
                                enemyHp = enemyHp - testPlayerAttack;
                                Console.WriteLine(enemyHp + "Enemy");

                            }
                            Random rnd2 = new Random();
                            float crit2 = rnd2.Next(0, 100);
                            if (crit2 <= critChance)
                            {
                                Console.WriteLine("Crit");
                                playerHp = playerHp - (testEnemyAttack * critMultiplier);
                                Console.WriteLine(playerHp + "Player");
                            }
                            else
                            {
                                Console.WriteLine("Not crit");
                                playerHp = playerHp - testEnemyAttack;
                                Console.WriteLine(playerHp + "Player");
                            }
                            attackHappened = true;
                        }
                    }
>>>>>>> Stashed changes
                }
            }
        }
        static void DrawMap1()
        {
            //Clears the console so you dont see the previous stuff.
            //Console.Clear();
            //Makes it so 2 attacks cant happen at the same time
            attackHappened = false;
            //Draws the map
            int total = 0;
            int length = 0;
            int mapLength = 0;
            int mapWidth = 0;
            for (int width = 0; total < map1.Length; width++)
            {
                total++;
                if (width == mapSizeL)
                {
                    //go to next map line
                    Console.WriteLine();
                    mapLength++;
                    mapWidth++;
                    if (mapLength >= innerMapSizeL)
                    {
                        mapLength = innerMapSizeL;
                    }
                    if (mapWidth >= innerMapSizeL)
                    {
                        mapWidth = innerMapSizeW;
                    }
                    if (width == mapSizeL)
                    {
                        width = 0;
                        length++;
                    }
                }
                if (firstTimeMap == true)
                {
                    for (int i = 1; i < innerMapSizeL + 1; i++)
                    {
                        map1[mapWidth, i] = floorCharacter;
                    }
                    map1[width, 0] = "| ";
                    map1[width, mapSizeL - 1] = "|";
                    map1[0, width] = "--";
                    map1[mapSizeW - 1, length] = "--";
                    map1[0, 0] = "|-";
                    map1[mapSizeW - 1, mapSizeL - 1] = "|";
                    map1[0, mapSizeL - 1] = "|";
                    map1[mapSizeW - 1, 0] = "|-";
                    map1[currentPlayerPosition[0], currentPlayerPosition[1]] = character;
                    map1[currentEnemyPosition[0], currentEnemyPosition[1]] = enemy1;
                }
                Console.Write(map1[length, width]);
            }
            Console.WriteLine();
        }
    }
}