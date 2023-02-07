using System;
using System.Diagnostics;
using System.Formats.Asn1;

namespace RogueAttemptMaybe
{
    //Current bugs:
    //enemies attack your last position.

    //Console.Beep(600 , 1000); , Long quiet beep
    //Console.Beep(750 , 500); , Short louder beep
    //To-Do list:
    //Map R
    //GUI *
    //Random (bruikbare) levels van 64x32 characters. *

    //Player:
    //Move 1 space with arrows R
    //HP *      Ma
    //Money *
    //Attack * Mi
    //Inventory *
    //Classes * Mi

    //Enemies: 
    //Movement R
    //HP *      Ma
    //Attack * R/Mi
    // Enemy types Mi
    internal class Program
    {
        //main menu
        static string name;
        static bool characterHasBeenMade = false;
        //Characters
        static string floorCharacter = "  ";
        static string character = "@ ";
        static string enemy1 = "E ";
        static string innerWall = "{}";
        static string outerUp = "--";
        static string outerSideL = "| ";
        static string outerSideR = " |";
        static string rightConers = "|-";
        static string leftConers = "-|";
        //Map sizes
        static int[] currentPlayerPosition = { 0, 0 };
        static int[] currentEnemyPosition = { 0, 0 };
        static int[] currentWallPosition = { 0, 0, 0, 0 };
        static int mapSizeL = 16;
        static int mapSizeW = 16;
        static int innerMapSizeL = 0;
        static int innerMapSizeW = 0;
        static int biggestMapSize = 66;
        static int total = 0;
        static int pos3 = 0;
        static int pos4 = 0;
        static int startr1L = 0;
        static int startr1W = 0;
        static int startr1U = 0;
        static int startr1D = 0;
        static int startr2L = 0;
        static int startr2W = 0;
        static int startr2U = 0;
        static int startr2D = 0;
        static bool saidStats = false;
        static int amountOfFails = 0;
        //Attack
        static string[] starterWeapons = File.ReadAllLines("StarterWeapons.txt");
        static string[] weapons = File.ReadAllLines("Weapons.txt");
        static string[] armors = File.ReadAllLines("Armors.txt");
        static List<float> starterWeaponDmg = new List<float>();
        static List<float> starterWeaponCritChance = new List<float>();
        static List<float> starterWeaponCritMulti = new List<float>();
        static List<string> starterWeaponName = new List<string>();
        static List<float> weaponDmg = new List<float>();
        static List<float> weaponCritChance = new List<float>();
        static List<float> weaponCritMulti = new List<float>();
        static List<string> weaponName = new List<string>();
        static List<float> armorDmgR = new List<float>();
        static List<float> armorAgiStat = new List<float>();
        static List<string> armorDisc = new List<string>();
        static List<string> armorName = new List<string>();
        static List<float> armorMoneyMulti = new List<float>();
        static List<float> armorSellMulti = new List<float>();
        static int selectedWeapon;
        static string currentWeapon = "Error";
        static float currentDmg;
        static float currentCrit;
        static float currentCritMulti;
        static float currentEnemyDmg;
        static float currentEnemyCrit;
        static float currentEnemyMulti;
        static float playerHp = 100;
        static float enemyHp = 100;
        static bool enemyAlive = true;
        static bool attackHappened = false;
        static int enemyWeapon;
        static int enemyNumber = 0;
        static Random rndEnemyWeapon = new Random();
        //Input map here
        static string[,] map1 = new string[biggestMapSize, biggestMapSize];
        static bool firstTimeMap = true;
        static bool firstTimeWall = true;
        static int amountOfWalls = 32;
        static int wallMultiplier = 2;
        static int[] rndpos = new int[4];
        static void Main(string[] args)
        {
            NewMap();
            for (int i = 0; i < starterWeapons.Length; i++)
            {
                string[] data = starterWeapons[i].Split(',', StringSplitOptions.RemoveEmptyEntries);
                starterWeaponName.Add(data[0]);
                starterWeaponDmg.Add(float.Parse(data[1]));
                starterWeaponCritChance.Add(float.Parse(data[2]));
                starterWeaponCritMulti.Add(float.Parse(data[3]));
            }
            for (int i = 0; i < weapons.Length; i++)
            {
                string[] data = weapons[i].Split(',', StringSplitOptions.RemoveEmptyEntries);
                weaponName.Add(data[0]);
                weaponDmg.Add(float.Parse(data[1]));
                weaponCritChance.Add(float.Parse(data[2]));
                weaponCritMulti.Add(float.Parse(data[3]));
            }
            for (int i = 0; i < armors.Length; i++)
            {
                string[] data = armors[i].Split(',', StringSplitOptions.RemoveEmptyEntries);
                armorName.Add(data[0]);
                armorDmgR.Add(float.Parse(data[1]));
                armorAgiStat.Add(float.Parse(data[2]));
                if (3 < data.Length)
                {
                    armorDisc.Add(data[3]);
                }
                if (4 < data.Length)
                {
                    armorMoneyMulti.Add(float.Parse(data[4]));
                }
                if (5 < data.Length)
                {
                    armorSellMulti.Add(float.Parse(data[5]));
                }
            }
            GameStart();
            while (characterHasBeenMade == false)
            {
                ConsoleKey select = Console.ReadKey().Key;
                IsSelecting();
                if (select == ConsoleKey.Enter)
                {
                    currentWeapon = weaponName[selectedWeapon];
                    currentDmg = weaponDmg[selectedWeapon];
                    currentCrit = weaponCritChance[selectedWeapon];
                    currentCritMulti = weaponCritMulti[selectedWeapon];
                    characterHasBeenMade = true;
                    if (currentWeapon == "Error")
                    {
                        Console.Clear();
                        Console.WriteLine("Error 5:");
                        Console.WriteLine("You have no weapon!");
                        Environment.Exit(0);
                    }
                }
            }
            if (characterHasBeenMade == true)
            {
                //Starts game
                NewEnemy();
                NewMap();
                firstTimeMap = false;
                AwaitMovementKey();
            }
            static void Move(ConsoleKey key)
            {
                EnemyMove();
                switch (key)
                {
                    case ConsoleKey.DownArrow:
                        {
                            if (map1[currentPlayerPosition[0] + 1, currentPlayerPosition[1]] == floorCharacter)
                            {
                                map1[currentPlayerPosition[0], currentPlayerPosition[1]] = floorCharacter;
                                currentPlayerPosition[0] = currentPlayerPosition[0] + 1;
                                map1[currentPlayerPosition[0], currentPlayerPosition[1]] = character;
                                CheckIfAttack("Player");
                                DrawMap3();
                                AwaitMovementKey();
                            }
                            else
                            {
                                CheckIfAttack("Player");
                                DrawMap3();
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
                                DrawMap3();
                                AwaitMovementKey();
                            }
                            else
                            {
                                CheckIfAttack("Player");
                                DrawMap3();
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
                                DrawMap3();
                                AwaitMovementKey();
                            }
                            else
                            {
                                CheckIfAttack("Player");
                                DrawMap3();
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
                                DrawMap3();
                                AwaitMovementKey();
                            }
                            else
                            {
                                CheckIfAttack("Player");
                                DrawMap3();
                                AwaitMovementKey();
                            }
                            break;
                        }
                    case ConsoleKey.N:
                        {
                            MakeMap3Nothing();
                            break;
                        }
                    case ConsoleKey.M:
                        {
                            NewMap();
                            break;
                        }
                }
            }
            static void AwaitMovementKey()
            {
                //makes sure you actually use an arrow
                ConsoleKey key = Console.ReadKey().Key;
                if (key == ConsoleKey.DownArrow || key == ConsoleKey.UpArrow || key == ConsoleKey.LeftArrow || key == ConsoleKey.RightArrow || key == ConsoleKey.N || key == ConsoleKey.M)
                {
                    Move(key);
                }
                else
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
                if (currentPlayerPosition[0] == currentEnemyPosition[0] - 1 || currentPlayerPosition[0] == currentEnemyPosition[0] + 1 || currentPlayerPosition[1] == currentEnemyPosition[1] - 1 || currentPlayerPosition[1] == currentEnemyPosition[1] + 1)
                {
                    if (attackHappened == false)
                    {
                        if (map1[currentEnemyPosition[0] - 1, currentEnemyPosition[1]] == character || map1[currentEnemyPosition[0] + 1, currentEnemyPosition[1]] == character || map1[currentEnemyPosition[0], currentEnemyPosition[1] - 1] == character || map1[currentEnemyPosition[0], currentEnemyPosition[1] + 1] == character)
                        {
                            Console.WriteLine("WouldAttack");
                            if (attacker == "Player")
                            {
                                Random rnd2 = new Random();
                                float crit2 = rnd2.Next(0, 100);
                                if (crit2 <= currentEnemyCrit)
                                {
                                    Console.WriteLine("Crit");
                                    playerHp = playerHp - (currentEnemyDmg * currentEnemyMulti);
                                    Console.WriteLine(playerHp + "Player");
                                }
                                else
                                {
                                    Console.WriteLine("Not crit");
                                    playerHp = playerHp - currentEnemyDmg;
                                    Console.WriteLine(playerHp + "Player");
                                }
                                Random rnd = new Random();
                                float crit1 = rnd.Next(0, 100);
                                if (crit1 <= currentCrit)
                                {
                                    Console.WriteLine("Crit");
                                    enemyHp = enemyHp - (currentDmg * currentCritMulti);
                                    Console.WriteLine(enemyHp + "Enemy");
                                }
                                else
                                {
                                    Console.WriteLine("Not crit");
                                    enemyHp = enemyHp - currentDmg;
                                    Console.WriteLine(enemyHp + "Enemy");
                                }
                                attackHappened = true;
                            }
                            else if (attacker == "Enemy")
                            {
                                Random rnd = new Random();
                                float crit1 = rnd.Next(0, 100);
                                if (crit1 <= currentCrit)
                                {
                                    Console.WriteLine("Crit");
                                    enemyHp = enemyHp - (currentDmg * currentCritMulti);
                                    Console.WriteLine(enemyHp + "Enemy");
                                }
                                else
                                {
                                    Console.WriteLine("Not crit");
                                    enemyHp = enemyHp - currentDmg;
                                    Console.WriteLine(enemyHp + "Enemy");

                                }
                                Random rnd2 = new Random();
                                float crit2 = rnd2.Next(0, 100);
                                if (crit2 <= currentEnemyCrit)
                                {
                                    Console.WriteLine("Crit");
                                    playerHp = playerHp - (enemyWeapon * currentEnemyMulti);
                                    Console.WriteLine(playerHp + "Player");
                                }
                                else
                                {
                                    Console.WriteLine("Not crit");
                                    playerHp = playerHp - currentEnemyCrit;
                                    Console.WriteLine(playerHp + "Player");
                                }
                                attackHappened = true;
                            }
                        }
                    }
                }
            }
            static void GameStart()
            {
                Console.WriteLine("Welcome to character creation");
                Console.WriteLine("Insert your name and press enter to continue the character creation");
                Console.WriteLine("-----------------------------------------------------------");
                name = Console.ReadLine();
                Console.Clear();
                Console.WriteLine("Welcome to the main menu.");
                Console.WriteLine("name:" + name);
                Console.WriteLine("Select a weapon and press enter to continue");
                Console.WriteLine("-----------------------------------------------------------");
                for (int i = 0; i < starterWeaponName.Count; i++)
                {
                    Console.Write(starterWeaponName[i] + " ");
                    Console.Write("Dmg:" + starterWeaponDmg[i] + " ");
                    Console.Write("CC:" + starterWeaponCritChance[i] + " ");
                    Console.WriteLine("CMulti:" + starterWeaponCritMulti[i]);
                    Console.WriteLine("-----------------------------------------------------------");
                }
                Console.WriteLine("Current weapon: None");
            }
            static void IsSelecting()
            {
                ConsoleKey select = Console.ReadKey().Key;
                if (select == ConsoleKey.UpArrow || select == ConsoleKey.DownArrow)
                {
                    Console.Clear();
                    Console.WriteLine("Welcome to character creation.");
                    Console.WriteLine(name);
                    Console.WriteLine("Select a weapon and press enter to continue");
                    Console.WriteLine("-----------------------------------------------------------");
                    for (int i = 0; i < starterWeaponName.Count; i++)
                    {
                        Console.Write(starterWeaponName[i] + " ");
                        Console.Write("Dmg:" + starterWeaponDmg[i] + " ");
                        Console.Write("CC:" + starterWeaponCritChance[i] + " ");
                        Console.WriteLine("CMulti:" + starterWeaponCritMulti[i]);
                        Console.WriteLine("-----------------------------------------------------------");
                    }
                    WeaponSelect(select);
                }
            }
            static void WeaponSelect(ConsoleKey select)
            {
                switch (select)
                {
                    case ConsoleKey.UpArrow:
                        {
                            if (selectedWeapon < starterWeapons.Length - 1)
                            {
                                selectedWeapon = selectedWeapon + 1;
                                Console.WriteLine("Current weapon:" + starterWeaponName[selectedWeapon]);
                            }
                            else
                            {
                                selectedWeapon = 0;
                                Console.WriteLine("Current weapon:" + starterWeaponName[selectedWeapon]);
                            }
                            break;
                        }
                    case ConsoleKey.DownArrow:
                        {
                            if (selectedWeapon > 0)
                            {
                                selectedWeapon = selectedWeapon - 1;
                                Console.WriteLine("Current weapon:" + starterWeaponName[selectedWeapon]);
                            }
                            else
                            {
                                selectedWeapon = starterWeapons.Length - 1;
                                Console.WriteLine("Current weapon:" + starterWeaponName[selectedWeapon]);
                            }
                            break;
                        }
                }
            }
            static void NewEnemy()
            {
                if (enemyNumber == 0)
                {
                    enemyWeapon = rndEnemyWeapon.Next(0, starterWeapons.Length);
                    currentEnemyDmg = starterWeaponDmg[enemyWeapon];
                    currentEnemyCrit = starterWeaponCritChance[enemyWeapon];
                    currentEnemyMulti = starterWeaponCritMulti[enemyWeapon];
                }
                else
                {
                    enemyWeapon = rndEnemyWeapon.Next(0, weapons.Length);
                    currentEnemyDmg = weaponDmg[enemyWeapon];
                    currentEnemyCrit = weaponCritChance[enemyWeapon];
                    currentEnemyMulti = weaponCritMulti[enemyWeapon];
                }
                if (enemyWeapon == 2 && enemyNumber != 0)
                {
                    currentEnemyCrit = currentEnemyCrit - 25;
                    currentEnemyMulti = currentEnemyMulti - 10;
                }
                if (enemyWeapon == 0 && enemyNumber != 0)
                {
                    currentEnemyCrit = currentEnemyCrit - 20;
                    currentEnemyDmg = currentEnemyDmg - 15;
                }
                enemyNumber = enemyNumber + 1;
            }
            static void DrawMap1()
            {
                try
                {
                    //Clears the console so you dont see the previous stuff.
                    //Console.Clear();
                    //Makes it so 2 attacks cant happen at the same time
                    attackHappened = false;
                    //Draws the map
                    total = 0;
                    int length = 0;
                    length = 0;
                    int mapLength = 0;
                    int mapWidth = 0;
                    for (int width = 0; total < mapSizeL * mapSizeW; width++)
                    {
                        total = width * length;
                        if (width == mapSizeW)
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
                            if (width == mapSizeW)
                            {
                                width = 0;
                                length++;
                            }
                        }
                        if (firstTimeMap == true)
                        {
                            for (int i = 1; i < innerMapSizeW + 1; i++)
                            {
                                map1[mapLength, i] = floorCharacter; //Floor
                            }
                            if (length <= innerMapSizeL)
                            {
                                map1[length, 0] = outerSideL; //LeftWall
                                map1[length, mapSizeW - 1] = outerSideR; // RightWall
                            }
                            for (int i = 0; i < mapSizeW; i++)
                            {
                                map1[0, i] = outerUp; //UpWall
                            }
                            for (int i = 0; i < mapSizeW; i++)
                            {
                                map1[mapSizeL - 1, i] = outerUp; //DownWall
                            }
                            map1[0, 0] = rightConers; //UpLeftCorner
                            map1[mapSizeL - 1, mapSizeW - 1] = outerSideR; //BottomRightCorner
                            map1[0, mapSizeW - 1] = outerSideR; //UpRightCorner
                            map1[mapSizeL - 1, 0] = rightConers; //BottomLeftCorner
                            map1[mapSizeL, width] = ""; //Hard fixes something
                            map1[mapSizeL + 1, 0] = ""; //Hard fixes something
                        }
                        Console.Write(map1[length, width]);
                    }
                    if (firstTimeMap == true)
                    {
                        Random rpos1 = new Random();
                        int pos1 = rpos1.Next(1, innerMapSizeL);
                        Random rpos2 = new Random();
                        int pos2 = rpos2.Next(1, innerMapSizeW);
                        currentPlayerPosition[0] = pos1;
                        currentPlayerPosition[1] = pos2;
                        //Takes a random spawn for 1 enemy
                        Random rpos3 = new Random();
                        int pos3 = rpos3.Next(1, innerMapSizeL);
                        Random rpos4 = new Random();
                        int pos4 = rpos4.Next(1, innerMapSizeW);
                        currentEnemyPosition[0] = pos3;
                        currentEnemyPosition[1] = pos4;
                        firstTimeWall = true;
                        WallLoop();
                        map1[currentPlayerPosition[0], currentPlayerPosition[1]] = character;
                        map1[currentEnemyPosition[0], currentEnemyPosition[1]] = enemy1;
                        firstTimeMap = false;
                        DrawMap1();
                    }
                    Console.WriteLine();
                }
                catch (IndexOutOfRangeException)
                {
                    Console.Clear();
                    Console.WriteLine("Error 4:");
                    Console.WriteLine("Something is outside the range of the map!");
                    Console.WriteLine("Press enter to try again");
                    Console.ReadLine();
                    NewMap();
                }
            }
            static void DrawMap3()
            {
                {
                    try
                    {
                        //Clears the console so you dont see the previous stuff.
                        //Console.Clear();
                        //Makes it so 2 attacks cant happen at the same time
                        attackHappened = false;
                        //Draws the map
                        total = 0;
                        int length = 0;
                        length = 0;
                        for (int width = 1; total < mapSizeL * mapSizeW; width++)
                        {
                            //The function that **Draws** the map
                            total = width * length;
                            if (width > innerMapSizeW)
                            {
                                width = 0;
                                length++;
                                Console.WriteLine();
                            }
                            if (length > innerMapSizeL)
                            {
                                length = 0;
                                total = mapSizeL * mapSizeW;
                            }
                            Console.Write(map1[length, width]);
                        }
                        Console.WriteLine();
                        AwaitMovementKey();
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.Clear();
                        Console.WriteLine("Error 4:");
                        Console.WriteLine("Something is outside the range of the map!");
                        Console.WriteLine("Press enter to try again");
                        Console.ReadLine();
                        NewMap();
                    }
                }
            }
            static void CreateMap3()
            {
                amountOfFails = 0;
                saidStats = false;
                startr1L = 0;
                startr1W = 0;
                startr1U = 0;
                startr1D = 0;
                Random rpos1 = new Random();
                int pos1 = rpos1.Next(0, mapSizeL - 1);
                Random rpos2 = new Random();
                int pos2 = rpos2.Next(0, mapSizeW - 1);
                int mapSizeFixer = 0;
                Random rsize1 = new Random();
                Random rsize2 = new Random();
                int size1 = rsize1.Next(6,7);
                int size2 = rsize1.Next(6,7);
                Console.WriteLine("Sides" + size1);
                Console.WriteLine("Ups" + size2);
                startr1L = pos2 - size1;
                startr1W = pos2 + size1;
                startr1U = pos1 - size2;
                startr1D = pos1 + size2;
                Console.WriteLine();
                Console.WriteLine("Before Math");
                Console.WriteLine("Room Coords: L" + pos1 + " and W" + pos2);
                Console.WriteLine("L From U" + startr1U + " to D" + startr1D);
                Console.WriteLine("W From L" + startr1L + " to W" + startr1W);
                Console.WriteLine();
                if (startr1L <= 1)
                {
                    mapSizeFixer = -startr1L;
                    Console.WriteLine(mapSizeFixer + " negative length");
                    startr1W = startr1W + mapSizeFixer + 2;
                    startr1L = 2;
                }
                if (startr1U <= 0)
                {
                    mapSizeFixer = -startr1U;
                    Console.WriteLine(mapSizeFixer + " negative up");
                    startr1D = startr1D + mapSizeFixer + 2;
                    startr1U = 1;
                }
                //Sides
                if (startr1W >= innerMapSizeW)
                {
                    mapSizeFixer = -startr1W;
                    Console.WriteLine(mapSizeFixer + " negative width");
                    startr1L = startr1L + (mapSizeFixer + innerMapSizeW);
                    Console.WriteLine(innerMapSizeW + " Inner Map Size W");
                    startr1W = innerMapSizeW;
                }
                if (startr1D >= innerMapSizeL)
                {
                    mapSizeFixer = -startr1D;
                    Console.WriteLine(mapSizeFixer + " negative down");
                    startr1U = startr1U + (mapSizeFixer + innerMapSizeL);
                    Console.WriteLine(innerMapSizeL + " Inner Map Size L");
                    startr1D = innerMapSizeL;
                }
                Console.WriteLine();
                Console.WriteLine("After Math");
                Console.WriteLine("Room Coords: L" + pos1 + " and W" + pos2);
                Console.WriteLine("L From U" + startr1U + " to D" + startr1D);
                Console.WriteLine("W From L" + startr1L + " to W" + startr1W);
                //Room 2
                startr2L = 0;
                startr2W = 0;
                startr2U = 0;
                startr2D = 0;
                RoomPosition(pos1 , pos2);
                mapSizeFixer = 0;
                Console.WriteLine();
                Console.WriteLine("After Math");
                Console.WriteLine("Room Coords: L" + pos3 + " and W" + pos4);
                Console.WriteLine("L From U" + startr2U + " to D" + startr2D);
                Console.WriteLine("W From L" + startr2L + " to W" + startr2W);
                total = 0;
                int length = 0;
                for (int width = 1; total < mapSizeL * mapSizeW; width++)
                {
                    //The function that **Draws** the first room
                    total = width * length;
                    if (width < startr1W && width >= startr1L)
                    {
                        if (length < startr1D && length >= startr1U)
                        {
                            map1[length, width] = floorCharacter;
                        }
                    }
                    if (width == startr1W && length < startr1D && length >= startr1U)
                    {
                        //Right wall
                        map1[length, width] = outerSideR;
                    }
                    if (width == startr1L - 1 && length < startr1D && length >= startr1U)
                    {
                        //Left wall
                        map1[length, width] = outerSideL;
                    }
                    if (length == startr1D && width < startr1W && width >= startr1L || length == startr1U - 1 && width < startr1W && width >= startr1L)
                    {
                        //Up and Down
                        map1[length, width] = outerUp;
                    }
                    if (length == startr1U - 1 && width == startr1L - 1 || length == startr1D && width == startr1L - 1)
                    {
                        //Right corners
                        map1[length, width] = rightConers;
                    }
                    if (length == startr1U - 1 && width == startr1W || length == startr1D && width == startr1W)
                    {
                        //Left Corners
                        map1[length, width] = leftConers;
                    }
                    //Room 2
                    if (width < startr2W && width >= startr2L)
                    {
                        if (length < startr2D && length >= startr2U)
                        {
                            map1[length, width] = floorCharacter;
                        }
                    }
                    if (width == startr2W && length < startr2D && length >= startr2U)
                    {
                        //Right wall
                        map1[length, width] = outerSideR;
                    }
                    if (width == startr2L - 1 && length < startr2D && length >= startr2U)
                    {
                        //Left wall
                        map1[length, width] = outerSideL;
                    }
                    if (length == startr2D && width < startr2W && width >= startr2L || length == startr2U - 1 && width < startr2W && width >= startr2L)
                    {
                        //Up and Down
                        map1[length, width] = outerUp;
                    }
                    if (length == startr2U - 1 && width == startr2L - 1 || length == startr2D && width == startr2L - 1)
                    {
                        //Right corners
                        map1[length, width] = rightConers;
                    }
                    if (length == startr2U - 1 && width == startr2W || length == startr2D && width == startr2W)
                    {
                        //Left Corners
                        map1[length, width] = leftConers;
                    }
                    //map1[pos1, pos2] = "md";
                    //The checking where we are area
                    if (width > innerMapSizeW)
                    {
                        width = 0;
                        length++;
                        Console.WriteLine();
                    }
                    if (length > innerMapSizeL)
                    {
                        length = 0;
                        total = mapSizeL * mapSizeW;
                    }
                    if (firstTimeMap == true)
                    {
                        PlayerPosition();
                        //Takes a random spawn for 1 enemy
                        firstTimeMap = false;
                        Console.WriteLine("Player Pos: " + currentPlayerPosition[0] + " " + currentPlayerPosition[1]);
                        Console.WriteLine("Enemy Pos: " + currentEnemyPosition[0] + " " + currentEnemyPosition[1]);
                    }
                    map1[currentPlayerPosition[0], currentPlayerPosition[1]] = character;
                    map1[currentEnemyPosition[0], currentEnemyPosition[1]] = enemy1;
                }
                DrawMap3();
            }
            static void RoomPosition(int pos1 , int pos2)
            {

                Random rpos3 = new Random();
                pos3 = rpos3.Next(0, mapSizeL - 1);
                Random rpos4 = new Random();
                pos4 = rpos4.Next(0, mapSizeW - 1);
                if (pos3 > pos1 - 14 && pos3 < pos1 + 14)
                {
                    Console.WriteLine("Fail3");
                    amountOfFails++;
                    if (amountOfFails >= 10)
                    {
                        MakeMap3Nothing();
                    }
                    RoomPosition(pos1 , pos2);
                }
                else
                {
                    if (pos4 > pos2 - 14 && pos4 < pos2 + 14)
                    {
                        Console.WriteLine("Fail4");
                        amountOfFails++;
                        if (amountOfFails >= 10)
                        {
                            MakeMap3Nothing();
                        }
                        RoomPosition(pos1, pos2);
                    }
                }
                Random rsize3 = new Random();
                Random rsize4 = new Random();
                int size3 = rsize3.Next(3, 7);
                int size4 = rsize4.Next(3, 7);
                int mapSizeFixer = 0;
                startr2L = pos4 - size3;
                startr2W = pos4 + size3;
                startr2U = pos3 - size4;
                startr2D = pos3 + size4;
                if (saidStats == false)
                {
                    Console.WriteLine("Sides" + size3);
                    Console.WriteLine("Ups" + size4);
                    Console.WriteLine();
                    Console.WriteLine("Before Math");
                    Console.WriteLine("Room Coords: L" + pos3 + " and W" + pos4);
                    Console.WriteLine("L From U" + startr2U + " to D" + startr2D);
                    Console.WriteLine("W From L" + startr2L + " to W" + startr2W);
                    Console.WriteLine();
                    saidStats = true;
                }
                if (startr2L <= 1)
                {
                    mapSizeFixer = -startr2L;
                    startr2W = startr2W + mapSizeFixer + 2;
                    startr2L = 2;
                }
                if (startr2U <= 0)
                {
                    mapSizeFixer = -startr2U;
                    startr2D = startr2D + mapSizeFixer + 2;
                    startr2U = 1;
                }
                //Sides
                if (startr2W >= innerMapSizeW)
                {
                    mapSizeFixer = -startr2W;
                    startr2L = startr2L + (mapSizeFixer + innerMapSizeW);
                    startr2W = innerMapSizeW;
                }
                if (startr2D >= innerMapSizeL)
                {
                    mapSizeFixer = -startr2D;
                    startr2U = startr2U + (mapSizeFixer + innerMapSizeL);
                    startr2D = innerMapSizeL;
                }
                if (startr2D <= startr1D && startr2D >= startr1U + 2)
                {
                    Console.WriteLine("Fail");
                    amountOfFails++;
                    if (amountOfFails >= 10)
                    {
                        MakeMap3Nothing();
                    }
                    RoomPosition(pos1, pos2);
                }
                if (startr2L <= startr1L && startr2L <= startr1W + 2)
                {
                    Console.WriteLine("Fail2");
                    amountOfFails++;
                    if (amountOfFails >= 10)
                    {
                        MakeMap3Nothing();
                    }
                    RoomPosition(pos1, pos2);
                }
                if (startr2U <= startr1D && startr2U >= startr1U + 2)
                {
                    Console.WriteLine("Fail5");
                    amountOfFails++;
                    if (amountOfFails >= 10)
                    {
                        MakeMap3Nothing();
                    }
                    RoomPosition(pos1, pos2);
                }
                if (startr2W <= startr1L && startr2W >= startr1W + 2)
                {
                    Console.WriteLine("Fail6");
                    amountOfFails++;
                    if (amountOfFails >= 10)
                    {
                        MakeMap3Nothing();
                    }
                    RoomPosition(pos1, pos2);
                }
            }
            static void PlayerPosition()
            {
                Random rpos3 = new Random();
                int pos3 = rpos3.Next(startr1U, startr1D);
                Random rpos4 = new Random();
                int pos4 = rpos4.Next(startr1L, startr1W);
                currentPlayerPosition[0] = pos3;
                currentPlayerPosition[1] = pos4;
                EnemyPosition();
            }
            static void EnemyPosition()
            {
                Random rpos5 = new Random();
                int pos5 = rpos5.Next(startr1U, startr1D);
                Random rpos6 = new Random();
                int pos6 = rpos6.Next(startr1L, startr1W);
                currentEnemyPosition[0] = pos5;
                currentEnemyPosition[1] = pos6;
                if (currentEnemyPosition[0] == currentPlayerPosition[0] && currentEnemyPosition[1] == currentPlayerPosition[1])
                {
                    EnemyPosition();
                }
            }
            static void MakeMap3Nothing()
            {
                try
                {
                    //Clears the console so you dont see the previous stuff.
                    //Console.Clear();
                    //Makes it so 2 attacks cant happen at the same time
                    attackHappened = false;
                    firstTimeMap = true;
                    //Draws the map
                    total = 0;
                    int length = 0;
                    length = 0;
                    for (int width = 1; total < mapSizeL * mapSizeW; width++)
                    {
                        //The function that Makes map empty
                        total = width * length;
                        map1[length, width] = "**";
                        if (width > innerMapSizeW)
                        {
                            width = 0;
                            length++;
                            Console.WriteLine();
                        }
                        if (length > innerMapSizeL)
                        {
                            length = 0;
                            total = mapSizeL * mapSizeW;
                        }
                    }
                    CreateMap3();
                }
                catch (IndexOutOfRangeException)
                {
                    Console.Clear();
                    Console.WriteLine("Error 4:");
                    Console.WriteLine("Something is outside the range of the map!");
                    Console.WriteLine("Press enter to try again");
                    Console.ReadLine();
                    NewMap();
                }
            }
            static void RandomPosition()
            {
                if (firstTimeWall == true)
                {
                    Random randomWidth = new Random();
                    int i = randomWidth.Next(1, innerMapSizeW + 1);
                    Random randomLength = new Random();
                    int j = randomWidth.Next(1, innerMapSizeL + 1);
                    rndpos[1] = i;
                    rndpos[0] = j;
                }
            }
            static void WallLoop()
            {
                if (firstTimeWall == true)
                {
                    for (int i = 0; i < amountOfWalls; i++)
                    {
                        RandomPosition();
                        map1[rndpos[0], rndpos[1]] = innerWall;
                    }
                }
                firstTimeWall = false;
            }
            static void MapSize()
            {
                Console.WriteLine("Warning: Max size is 64!");
                Console.WriteLine("Warning: Min size is 32!");
                Console.WriteLine("Map Length");
                string answerL = "32";
                try
                {
                    answerL = Console.ReadLine();
                    int result = Int32.Parse(answerL);
                }
                catch (FormatException)
                {
                    if (answerL == "")
                    {
                        answerL = "nothing";
                    }
                    Console.Clear();
                    Console.WriteLine("Error 1:");
                    Console.WriteLine("Cannot convert " + answerL + " to a number.");
                    Console.WriteLine("Press enter to try again");
                    Console.ReadLine();
                    MapSize();
                }
                if (Int32.Parse(answerL) > biggestMapSize - 2)
                {
                    Console.Clear();
                    Console.WriteLine("Error 2:");
                    Console.WriteLine("Answer too big");
                    Console.WriteLine("Press enter to try again");
                    Console.ReadLine();
                    MapSize();
                }
                else if (Int32.Parse(answerL) < 32)
                {
                    Console.Clear();
                    Console.WriteLine("Error 3:");
                    Console.WriteLine("Answer too small");
                    Console.WriteLine("Press enter to try again");
                    Console.ReadLine();
                    MapSize();
                }
                mapSizeL = Int32.Parse(answerL);
                Console.WriteLine("Map Width");
                string answerW = "32";
                try
                {
                    answerW = Console.ReadLine();
                    int result = Int32.Parse(answerW);
                }
                catch (FormatException)
                {
                    Console.Clear();
                    Console.WriteLine("Error 1:");
                    Console.WriteLine("Cannot convert " + answerW + " to a number.");
                    Console.WriteLine("Press enter to try again");
                    Console.ReadLine();
                    MapSize();
                }
                if (Int32.Parse(answerW) > biggestMapSize - 2)
                {
                    Console.Clear();
                    Console.WriteLine("Error 2:");
                    Console.WriteLine("Answer too big");
                    Console.WriteLine("Press enter to try again");
                    Console.ReadLine();
                    MapSize();
                }
                else if (Int32.Parse(answerW) < 32)
                {
                    Console.Clear();
                    Console.WriteLine("Error 3:");
                    Console.WriteLine("Answer too small");
                    Console.WriteLine("Press enter to try again");
                    Console.ReadLine();
                    MapSize();
                }
                mapSizeW = Int32.Parse(answerW);
                innerMapSizeW = mapSizeW - 2;
                innerMapSizeL = mapSizeL - 2;
                amountOfWalls = ((innerMapSizeL * innerMapSizeW) / 15) * wallMultiplier;
                Console.WriteLine(amountOfWalls + "Walls");
                Console.WriteLine(innerMapSizeW + "InW");
                Console.WriteLine(innerMapSizeL + "InL");
                Console.WriteLine(mapSizeW + "OutW");
                Console.WriteLine(mapSizeL + "OutL");
                MakeMap3Nothing();
                AwaitMovementKey();
            }
            static void NewMap()
            {
                firstTimeMap = true;
                MapSize();
                DrawMap3();
                AwaitMovementKey();

            }

        }
    }
}