using System;
using System.Diagnostics;
using System.Formats.Asn1;
using System.Runtime.CompilerServices;

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

    //Chests
    //Items
    //Ranged Weapons

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
        //Map General
        static int total = 0;
        //Map sizes V4
        static int[] currentPlayerPosition = { 0, 0 };
        static int[] currentEnemyPosition = { 0, 0 };
        static int[] currentWallPosition = { 0, 0, 0, 0 };
        static int mapSizeL = 16;
        static int mapSizeW = 16;
        static int innerMapSizeL = 0;
        static int innerMapSizeW = 0;
        static int biggestMapSize = 66;
        static int pos3 = 0;
        static int pos4 = 0;
        static int startr1L = 0;
        static int startr1R = 0;
        static int startr1U = 0;
        static int startr1D = 0;
        static int startr2L = 0;
        static int startr2R = 0;
        static int startr2U = 0;
        static int startr2D = 0;
        static bool saidStats = false;
        static int amountOfFails = 0;
        static string mapSeed = "0214142622300612160725053232";
        static string newSeed = "0214142622300612160725053232";
        static bool specificmapbool = false;
        static string[] randomWorkingMaps = { "0214142622300612160725053232", "0315011423292030061306103232", "0719183018300110221322163232" };
        static string[] randomBrokenMaps = { "1022011418301424121012173232", "0214122411190112200919113232", "1022152723290517151224123232", "0214051711171830101205023232" };
        //Map V4
        static int[] recommendedMapSize_4 = { 32, 32, 2 };
        static int mapSizeL_4 = 16;
        static int mapSizeW_4 = 16;
        static int innerMapSizeL_4 = 0;
        static int innerMapSizeW_4 = 0;
        static int biggestMapSize_4 = 64;
        static int smallestMapSize_4 = 32;

        static int amountOfRooms = 0;
        static int maxRooms = 5;
        static int minRooms = 1;

        static int[,] roomSizes = new int[5, 6]
        {
            //PosL , PosW , SizeR , SizeL , SizeU , SizeD
            { 0, 0, 0, 0, 0, 0}, //R0
            { 0, 0, 0, 0, 0, 0}, //R1
            { 0, 0, 0, 0, 0, 0}, //R2
            { 0, 0, 0, 0, 0, 0}, //R3
            { 0, 0, 0, 0, 0, 0}, //R4
        };
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
        static bool failedMap = false;
        static void Main(string[] args)
        {
            MapSizeV4();
            NewMapV4();
            if (failedMap)
            {
                MapSize();
            }
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
                MapSize();
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
                            if (specificmapbool == false)
                            {
                            MakeMap3Nothing();
                            }
                            else
                            {
                                useSpecificSeed(newSeed);
                            }
                            break;
                        }
                    case ConsoleKey.M:
                        {
                            MapSize();
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
                    //Console.Clear();
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
                        //Console.Clear();
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
                bool reset = false;
                amountOfFails = 0;
                saidStats = false;
                bool path = false;
                int[,] mapMid = new int[12,12];
                startr1L = 0;
                startr1R = 0;
                startr1U = 0;
                startr1D = 0;
                Random rpos1 = new Random();
                int pos1 = rpos1.Next(0, mapSizeL - 1);
                Random rpos2 = new Random();
                int pos2 = rpos2.Next(0, mapSizeW - 1);
                int mapSizeFixer = 0;
                Random rsize1 = new Random();
                Random rsize2 = new Random();
                int size1 = rsize1.Next(6, 7);
                int size2 = rsize1.Next(6, 7);
                Console.WriteLine("Sides" + size1);
                Console.WriteLine("Ups" + size2);
                startr1L = pos2 - size1;
                startr1R = pos2 + size1;
                startr1U = pos1 - size2;
                startr1D = pos1 + size2;
                Console.WriteLine();
                Console.WriteLine("==================================");
                Console.WriteLine("Before Math");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Room 1");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Room Position: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Length " + pos1);
                Console.WriteLine("Width " + pos2);
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Upwards Sizes:");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Up " + startr1U);
                Console.WriteLine("Down " + startr1D);
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Sides Sizes:");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Left " + startr1L);
                Console.WriteLine("Right " + startr1R);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("==================================");
                Console.WriteLine();
                if (startr1L <= 1)
                {
                    mapSizeFixer = -startr1L;
                    Console.WriteLine(mapSizeFixer + " negative length");
                    startr1R = startr1R + mapSizeFixer + 2;
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
                if (startr1R >= innerMapSizeW)
                {
                    mapSizeFixer = -startr1R;
                    Console.WriteLine(mapSizeFixer + " negative width");
                    startr1L = startr1L + (mapSizeFixer + innerMapSizeW);
                    Console.WriteLine(innerMapSizeW + " Inner Map Size W");
                    startr1R = innerMapSizeW;
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
                Console.WriteLine("==================================");
                Console.WriteLine("After Math");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Room 1");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Room Position: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Length " + pos1);
                Console.WriteLine("Width " + pos2);
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Upwards Sizes:");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Up " + startr1U);
                Console.WriteLine("Down " + startr1D);
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Sides Sizes:");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Left " + startr1L);
                Console.WriteLine("Right " + startr1R);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("==================================");
                Console.WriteLine();
                //Room 2
                startr2L = 0;
                startr2R = 0;
                startr2U = 0;
                startr2D = 0;
                RoomPosition(pos1, pos2);
                mapSizeFixer = 0;
                Console.WriteLine();
                Console.WriteLine("==================================");
                Console.WriteLine("After Math");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Room 2");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Room Position: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Length " + pos3);
                Console.WriteLine("Width " + pos4);
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Upwards Sizes:");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Up " + startr2U);
                Console.WriteLine("Down " + startr2D);
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Sides Sizes:");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Left " + startr2L);
                Console.WriteLine("Right " + startr2R);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("==================================");
                Console.WriteLine();
                total = 0;
                int length = 0;
                for (int width = 1; total < mapSizeL * mapSizeW; width++)
                {
                    //The function that **Draws** the first room
                    total = width * length;
                    if (width < startr1R && width >= startr1L)
                    {
                        if (length < startr1D && length >= startr1U)
                        {
                            map1[length, width] = floorCharacter;
                            //map1[length, width] = width.ToString();
                            //map1[length, width] = length.ToString();
                        }
                    }
                    if (width == startr1R && length < startr1D && length >= startr1U)
                    {
                        //Right wall
                        map1[length, width] = outerSideR;
                    }
                    if (width == startr1L - 1 && length < startr1D && length >= startr1U)
                    {
                        //Left wall
                        map1[length, width] = outerSideL;
                    }
                    if (length == startr1D && width < startr1R && width >= startr1L || length == startr1U - 1 && width < startr1R && width >= startr1L)
                    {
                        //Up and Down
                        map1[length, width] = outerUp;
                    }
                    if (length == startr1U - 1 && width == startr1L - 1 || length == startr1D && width == startr1L - 1)
                    {
                        //Right corners
                        map1[length, width] = rightConers;
                    }
                    if (length == startr1U - 1 && width == startr1R || length == startr1D && width == startr1R)
                    {
                        //Left Corners
                        map1[length, width] = leftConers;
                    }
                    //Room 2
                    if (width < startr2R && width >= startr2L)
                    {
                        if (length < startr2D && length >= startr2U)
                        {
                            map1[length, width] = floorCharacter;
                        }
                    }
                    if (width == startr2R && length < startr2D && length >= startr2U)
                    {
                        //Right wall
                        map1[length, width] = outerSideR;
                    }
                    if (width == startr2L - 1 && length < startr2D && length >= startr2U)
                    {
                        //Left wall
                        map1[length, width] = outerSideL;
                    }
                    if (length == startr2D && width < startr2R && width >= startr2L || length == startr2U - 1 && width < startr2R && width >= startr2L)
                    {
                        //Up and Down
                        map1[length, width] = outerUp;
                    }
                    if (length == startr2U - 1 && width == startr2L - 1 || length == startr2D && width == startr2L - 1)
                    {
                        //Right corners
                        map1[length, width] = rightConers;
                    }
                    if (length == startr2U - 1 && width == startr2R || length == startr2D && width == startr2R)
                    {
                        //Left Corners
                        map1[length, width] = leftConers;
                    }
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
                    if (width == (startr1R + startr1L) / 2)
                    {
                        mapMid[0, 1] = startr1D;
                        mapMid[0, 2] = width;
                    }
                    if (width == (startr1R + startr1L) / 2)
                    {
                        mapMid[1, 1] = startr1U - 1;
                        mapMid[1, 2] = width;
                    }
                    if (length == (startr1U + startr1D) / 2)
                    {
                        mapMid[2, 1] = startr1R;
                        mapMid[2, 2] = length;
                    }
                    if (length == (startr1U + startr1D) / 2)
                    {
                        mapMid[3, 1] = startr1L - 1;
                        mapMid[3, 2] = length;
                    }
                    //Room2
                    if (width == (startr2R + startr2L) / 2)
                    {
                        mapMid[4, 1] = startr2D;
                        mapMid[4, 2] = width;
                    }
                    if (width == (startr2R + startr2L) / 2)
                    {
                        mapMid[5, 1] = startr2U - 1;
                        mapMid[5, 2] = width;
                    }
                    if (length == (startr2U + startr2D) / 2)
                    {
                        mapMid[6, 1] = startr2R;
                        mapMid[6, 2] = length;
                    }
                    if (length == (startr2U + startr2D) / 2)
                    {
                        mapMid[7, 1] = startr2L - 1;
                        mapMid[7, 2] = length;
                    }
                    map1[mapMid[0, 1], mapMid[0, 2]] = "1D";
                    map1[mapMid[1, 1], mapMid[1, 2]] = "1U";
                    map1[mapMid[2, 2], mapMid[2, 1]] = "1R";
                    map1[mapMid[3, 2], mapMid[3, 1]] = "1L";
                    map1[mapMid[4, 1], mapMid[4, 2]] = "2D";
                    map1[mapMid[5, 1], mapMid[5, 2]] = "2U";
                    map1[mapMid[6, 2], mapMid[6, 1]] = "2R";
                    map1[mapMid[7, 2], mapMid[7, 1]] = "2L";

                    /*                    map1[mapMid[0, 1], mapMid[0, 2]] = "  ";
                                        map1[mapMid[1, 1], mapMid[1, 2]] = "  ";
                                        map1[mapMid[2, 2], mapMid[2, 1]] = "  ";
                                        map1[mapMid[3, 2], mapMid[3, 1]] = "  ";
                                        map1[mapMid[4, 1], mapMid[4, 2]] = "  ";
                                        map1[mapMid[5, 1], mapMid[5, 2]] = "  ";
                                        map1[mapMid[6, 2], mapMid[6, 1]] = "  ";
                                        map1[mapMid[7, 2], mapMid[7, 1]] = "  ";*/
                    bool sideR = false;
                    bool sideU = false;
                    if (startr2L >= startr1R)
                    {
                        //We use R1
                        sideR = true;
                    }
                    else
                    {
                        //We use L1
                    }
                    if (startr2U >= startr1D)
                    {
                        //We use D1
                        sideU = true;
                    }
                    else
                    {
                        //We use U1
                    }
                    if (path == false)
                    {
                        if (sideU == true)
                        {
                            if (sideR == true)
                            {
                                mapMid[8,1] = mapMid[7, 2];
                                mapMid[8, 3] = mapMid[7, 1];
                                mapMid[8,2] = mapMid[0,2];
                                mapMid[8, 4] = mapMid[0,1];
                                Console.WriteLine("L2 to D1");
                                map1[mapMid[7, 2], mapMid[7, 1]] =floorCharacter; //L2
                                map1[mapMid[0, 1], mapMid[0, 2]] =floorCharacter; //D1
                                                                                  //From L2 to D1
                                if (width >= mapMid[8, 2] && width <= mapMid[8, 3])
                                {
                                    //map1[mapMid[8, 1], width] = "P1";
                                    map1[mapMid[8, 1], width] = floorCharacter;
                                }
                                if (width <= startr1D)
                                {
                                    Console.WriteLine(mapMid[6, 2]);
                                    if (length >= mapMid[1, 1] + 1 && length <= (startr2D + startr2U) / 2)
                                    {
                                        //map1[length, mapMid[8, 2]] = "P2";
                                        map1[length, mapMid[8, 2]] = floorCharacter;
                                    }
                                }
                                map1[0, length] = "**";
                            }
                            else
                            {
                                reset = true;
                            }
                        }
                        else
                        {
                            if (sideR == true)
                            {
                                mapMid[8, 1] = mapMid[2, 2];
                                mapMid[8, 3] = mapMid[2, 1];
                                mapMid[8, 2] = mapMid[4, 2];
                                mapMid[8, 4] = mapMid[4, 1];
                                Console.WriteLine("D2 to R1");
                                //From R1 to 2D
                                map1[mapMid[2, 2], mapMid[2, 1]] = floorCharacter; //R1
                                map1[mapMid[4, 1], mapMid[4, 2]] = floorCharacter; //D2
                                //map1[mapMid[2,2], mapMid[4,2]] = "md"; //D2
                                if (width >= startr1R && width <= mapMid[4,2])
                                {
                                    //map1[mapMid[8, 1], width] = "P1";
                                    map1[mapMid[8, 1], width] = floorCharacter;
                                }
                                if (width <= startr2D)
                                {
                                    Console.WriteLine(mapMid[6, 2]);
                                    if (length >= startr2D && length <= (startr1D + startr1U) / 2)
                                    {
                                        //map1[length, mapMid[8, 2]] = "P2";
                                        map1[length, mapMid[8, 2]] = floorCharacter;
                                    }
                                }
                                map1[0, length] = "**";
                            }
                            else
                            {
                                mapMid[8, 1] = mapMid[7, 2];
                                mapMid[8, 3] = mapMid[7, 1];
                                mapMid[8, 2] = mapMid[0, 2];
                                mapMid[8, 4] = mapMid[0, 1];
                                Console.WriteLine("U1 to L2");
                                //From U1 to L2
                                map1[mapMid[1, 1], mapMid[1, 2]] = floorCharacter; //U1
                                map1[mapMid[7, 2], mapMid[7, 1]] = floorCharacter; //L2
                                map1[mapMid[7, 2], mapMid[0, 2]] = "md"; //D2
                                if (width >= (startr1R + startr1L) / 2 && width <= startr2L - 1)
                                {
                                    map1[mapMid[8, 1], width] = "P1";
                                    //map1[mapMid[8, 1], width] = floorCharacter;
                                }
                                if (width <= startr2D)
                                {
                                    Console.WriteLine(mapMid[6, 2]);
                                    if (length >= (startr2U + startr2D) / 2 && length <= startr1U)
                                    {
                                        map1[length, mapMid[8, 2]] = "P2";
                                        //map1[length, mapMid[8, 2]] = floorCharacter;
                                    }
                                }
                                map1[0, length] = "**";
                            }
                        }
                    }
                    //map1[mapMid[8, 1], mapMid[8, 2]] = "MD";
                    //map1[mapMid[8, 1], mapMid[8, 2]] = floorCharacter;
                    //map1[mapMid[8, 1], width] = "TE";
                }
                if (reset == false)
                {
                    reset = CheckRoomPosition();
                }
                if (reset == false)
                {
                    createSeed();
                    DrawMap3();
                }
                else
                {
                    failedMap = true;
                }
            }
            static void RoomPosition(int pos1, int pos2)
            {
                if (failedMap == false)
                {
                    bool problem = false;
                    bool fail = false;
                    Random rpos3 = new Random();
                    pos3 = rpos3.Next(0, mapSizeL - 1);
                    Random rpos4 = new Random();
                    pos4 = rpos4.Next(0, mapSizeW - 1);
                    if (pos3 > pos1 - 10 && pos3 < pos1 + 10)
                    {
                        Console.WriteLine("Fail3");
                        amountOfFails++;
                        if (amountOfFails >= 10)
                        {
                            if (problem == false)
                            {
                                problem = true;
                            }
                        }
                        if (fail == false)
                        {
                            fail = true;
                        }
                    }
                    else
                    {
                        if (pos4 > pos2 - 10 && pos4 < pos2 + 10)
                        {
                            Console.WriteLine("Fail4");
                            amountOfFails++;
                            if (amountOfFails >= 10)
                            {
                                if (problem == false)
                                {
                                    problem = true;
                                }
                            }
                            if (fail == false)
                            {
                                fail = true;
                            }
                        }
                    }
                    Random rsize3 = new Random();
                    Random rsize4 = new Random();
                    int size3 = rsize3.Next(3, 7);
                    int size4 = rsize4.Next(3, 7);
                    int mapSizeFixer = 0;
                    startr2L = pos4 - size3;
                    startr2R = pos4 + size3;
                    startr2U = pos3 - size4;
                    startr2D = pos3 + size4;
                    if (saidStats == false)
                    {
                        Console.WriteLine();
                        Console.WriteLine("==================================");
                        Console.WriteLine("Before Math");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Room 2");
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Room Position: ");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Length " + pos3);
                        Console.WriteLine("Width " + pos4);
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Upwards Sizes:");
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Up " + startr2U);
                        Console.WriteLine("Down " + startr2D);
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Sides Sizes:");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Left " + startr2L);
                        Console.WriteLine("Right " + startr2R);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("==================================");
                        Console.WriteLine();
                        saidStats = true;
                    }
                    if (startr2L <= 1)
                    {
                        mapSizeFixer = -startr2L;
                        startr2R = startr2R + mapSizeFixer + 2;
                        startr2L = 2;
                    }
                    if (startr2U <= 0)
                    {
                        mapSizeFixer = -startr2U;
                        startr2D = startr2D + mapSizeFixer + 2;
                        startr2U = 1;
                    }
                    //Sides
                    if (startr2R >= innerMapSizeW)
                    {
                        mapSizeFixer = -startr2R;
                        startr2L = startr2L + (mapSizeFixer + innerMapSizeW);
                        startr2R = innerMapSizeW;
                    }
                    if (startr2D >= innerMapSizeL)
                    {
                        mapSizeFixer = -startr2D;
                        startr2U = startr2U + (mapSizeFixer + innerMapSizeL);
                        startr2D = innerMapSizeL;
                    }
                    if (startr2D <= startr1D - 2 && startr2D >= startr1U + 3 && specificmapbool == false)
                    {
                        Console.WriteLine("Fail");
                        amountOfFails++;
                        if (amountOfFails >= 10)
                        {
                            if (problem == false)
                            {
                                problem = true;
                            }
                        }
                        if (fail == false)
                        {
                            fail = true;
                        }
                    }
                    if (startr2L <= startr1L - 2 && startr2L <= startr1R + 3 && specificmapbool == false)
                    {
                        Console.WriteLine("Fail2");
                        amountOfFails++;
                        if (amountOfFails >= 10)
                        {
                            if (problem == false)
                            {
                                problem = true;
                            }
                        }
                        if (fail == false)
                        {
                            fail = true;
                        }
                    }
                    if (startr2U <= startr1D - 2 && startr2U >= startr1U + 3 && specificmapbool == false)
                    {
                        Console.WriteLine("Fail5");
                        amountOfFails++;
                        if (amountOfFails >= 10)
                        {
                            if (problem == false)
                            {
                                problem = true;
                            }
                        }
                        if (fail == false)
                        {
                            fail = true;
                        }
                    }
                    if (startr2R <= startr1L - 2 && startr2R >= startr1R + 3 && specificmapbool == false)
                    {
                        Console.WriteLine("Fail6");
                        amountOfFails++;
                        if (amountOfFails >= 10)
                        {
                            if (problem == false)
                            {
                                problem = true;
                            }
                        }
                        if (fail == false)
                        {
                            fail = true;
                        }
                    }
                    if (problem == true)
                    {
                        failedMap = true;
                        MakeMap3Nothing();
                    }
                    if (fail == true)
                    {
                        RoomPosition(pos1, pos2);
                    }
                }
            }
            static void PlayerPosition()
            {
                Random rpos3 = new Random();
                int pos3 = rpos3.Next(startr1U, startr1D);
                Random rpos4 = new Random();
                int pos4 = rpos4.Next(startr1L, startr1R);
                currentPlayerPosition[0] = pos3;
                currentPlayerPosition[1] = pos4;
                EnemyPosition();
            }
            static void EnemyPosition()
            {
                Random rpos5 = new Random();
                int pos5 = rpos5.Next(startr1U, startr1D);
                Random rpos6 = new Random();
                int pos6 = rpos6.Next(startr1L, startr1R);
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
                    failedMap = false;
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
                    //Console.Clear();
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
                specificmapbool = false;
                Console.WriteLine("Warning: Max size is 64!");
                Console.WriteLine("Warning: Min size is 32!");
                Console.WriteLine("Recommended: 32x32");
                Console.WriteLine("Map Length");
                string answerL = "32";
                try
                {
                    answerL = Console.ReadLine();
                    if (answerL == "seed")
                    {
                        specificmapbool = true;
                        askSeed();
                    }
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
                //DrawMap1();
            }
            static void NewMap()
            {
                firstTimeMap = true;
                failedMap = false;
                MakeMap3Nothing();
                DrawMap3();
                AwaitMovementKey();
            }
            static void createSeed()
            {
                string r1L = startr1L.ToString();
                if (startr1L < 10)
                {
                    r1L = ("0" + startr1L.ToString());
                }
                string r1R = startr1R.ToString();
                if (startr1R < 10)
                {
                    r1R = ("0" + startr1R.ToString());
                }
                string r1U = startr1U.ToString();
                if (startr1U < 10)
                {
                    r1U = ("0" + startr1U.ToString());
                }
                string r1D = startr1D.ToString();
                if (startr1D < 10)
                {
                    r1D = ("0" + startr1D.ToString());
                }
                //Room2
                string r2L = startr2L.ToString();
                if (startr2L < 10)
                {
                    r2L = ("0" + startr2L.ToString());
                }
                string r2R = startr2R.ToString();
                if (startr2R < 10)
                {
                    r2R = ("0" + startr2R.ToString());
                }
                string r2U = startr2U.ToString();
                if (startr2U < 10)
                {
                    r2U = ("0" + startr2U.ToString());
                }
                string r2D = startr2D.ToString();
                if (startr2D < 10)
                {
                    r2D = ("0" + startr2D.ToString());
                }
                //Player/Enemy
                string pL = currentPlayerPosition[0].ToString();
                if (currentPlayerPosition[0] < 10)
                {
                    pL = ("0" + currentPlayerPosition[0].ToString());
                }
                string pW = currentPlayerPosition[1].ToString();
                if (currentPlayerPosition[1] < 10)
                {
                    pW = ("0" + currentPlayerPosition[1].ToString());
                }
                string eL = currentEnemyPosition[0].ToString();
                if (currentEnemyPosition[0] < 10)
                {
                    eL = ("0" + currentEnemyPosition[0].ToString());
                }
                string eW = currentEnemyPosition[1].ToString();
                if (currentEnemyPosition[1] < 10)
                {
                    eW = ("0" + currentEnemyPosition[1].ToString());
                }
                //MapSize
                string msL = mapSizeL.ToString();
                if (mapSizeL < 10)
                {
                    msL = ("0" + mapSizeL.ToString());
                }
                string msW = mapSizeW.ToString();
                if (mapSizeW < 10)
                {
                    msW = ("0" + mapSizeW.ToString());
                }
                string r1Seed = ("" + r1L + r1R + r1U + r1D);
                string r2Seed = ("" + r2L + r2R + r2U + r2D);
                string playerEnemySeed = ("" + pL + pW + eL + eW);
                string sizeSeed = ("" + msL + msW);
                mapSeed = (r1Seed + r2Seed + playerEnemySeed + sizeSeed);
                Console.WriteLine("This maps seed is: " + mapSeed);
            }
            static void askSeed()
            {
                Console.WriteLine("Put in the seed");
                try
                {
                    newSeed = "";
                    newSeed = Console.ReadLine();
                    newSeed.ToString();
                    if (newSeed == "r")
                    {
                        Random randomWidth = new Random();
                        int i = randomWidth.Next(0, randomWorkingMaps.Length + 1);
                        newSeed = randomWorkingMaps[i];
                    } else if(newSeed == "rb")
                    {
                        Random randomWidth = new Random();
                        int i = randomWidth.Next(0, randomBrokenMaps.Length + 1);
                        newSeed = randomBrokenMaps[i];
                    }
                    try
                    {
                        Int32.Parse(newSeed.Substring(22, 2));
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Error!!!!");
                        askSeed();
                    }
                    if (newSeed != "")
                    {
                        useSpecificSeed(newSeed);
                    }
                    else
                    {
                        Console.WriteLine("That breaks things my guy");
                        askSeed();
                    }
                }
                catch (Exception)
                {
                    Console.Clear();
                    Console.WriteLine("Error 1:");
                    Console.WriteLine("Cannot convert that to a number.... Huh?");
                    Console.WriteLine("Press enter to try again");
                    Console.ReadLine();
                    askSeed();
                }
            }
            static void useSpecificSeed(string seed)
            {
                specificmapbool = true;
                //Clears the console so you dont see the previous stuff.
                //Console.Clear();
                //Makes it so 2 attacks cant happen at the same time
                attackHappened = false;
                firstTimeMap = true;
                //Draws the map
                mapSizeW = Int32.Parse(seed.Substring(24, 2));
                mapSizeL = Int32.Parse(seed.Substring(26, 2));
                innerMapSizeW = mapSizeW - 2;
                innerMapSizeL = mapSizeL - 2;
                Console.WriteLine(mapSizeL);
                Console.WriteLine(mapSizeW);
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
                useSpecificSeed2(seed);
            }
            static void useSpecificSeed2(string seed)
            {
                startr2L = Int32.Parse(seed.Substring(8, 2));
                startr1L = Int32.Parse(seed.Substring(0, 2));
                startr1R = Int32.Parse(seed.Substring(2, 2));
                startr1U = Int32.Parse(seed.Substring(4, 2));
                startr1D = Int32.Parse(seed.Substring(6, 2));
                startr2L = Int32.Parse(seed.Substring(8, 2));
                startr2R = Int32.Parse(seed.Substring(10, 2));
                startr2U = Int32.Parse(seed.Substring(12, 2));
                startr2D = Int32.Parse(seed.Substring(14, 2));
                total = 0;
                int length = 0;
                Console.WriteLine();
                Console.WriteLine("==================================");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Room 1");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Upwards Sizes:");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Up " + startr1U);
                Console.WriteLine("Down " + startr1D);
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Sides Sizes:");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Left " + startr1L);
                Console.WriteLine("Right " + startr1R);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("==================================");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("==================================");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Room 2");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Upwards Sizes:");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Up " + startr2U);
                Console.WriteLine("Down " + startr2D);
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Sides Sizes:");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Left " + startr2L);
                Console.WriteLine("Right " + startr2R);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("==================================");
                Console.WriteLine();
                bool reset = CheckRoomPosition();
                if (reset == true)
                {
                    Console.WriteLine("Broken map! Sorry dude!");
                    askSeed();
                }
                for (int width = 1; total < mapSizeL * mapSizeW; width++)
                {
                    //The function that **Draws** the first room
                    total = width * length;
                    if (width < startr1R && width >= startr1L)
                    {
                        if (length < startr1D && length >= startr1U)
                        {
                            //map1[length, width] = width.ToString();
                            //map1[length, width] = length.ToString();
                            map1[length, width] = floorCharacter;
                        }
                    }
                    if (width == startr1R && length < startr1D && length >= startr1U)
                    {
                        //Right wall
                        map1[length, width] = outerSideR;
                    }
                    if (width == startr1L - 1 && length < startr1D && length >= startr1U)
                    {
                        //Left wall
                        map1[length, width] = outerSideL;
                    }
                    if (length == startr1D && width < startr1R && width >= startr1L || length == startr1U - 1 && width < startr1R && width >= startr1L)
                    {
                        //Up and Down
                        map1[length, width] = outerUp;
                    }
                    if (length == startr1U - 1 && width == startr1L - 1 || length == startr1D && width == startr1L - 1)
                    {
                        //Right corners
                        map1[length, width] = rightConers;
                    }
                    if (length == startr1U - 1 && width == startr1R || length == startr1D && width == startr1R)
                    {
                        //Left Corners
                        map1[length, width] = leftConers;
                    }
                    //Room 2
                    if (width < startr2R && width >= startr2L)
                    {
                        if (length < startr2D && length >= startr2U)
                        {
                            map1[length, width] = floorCharacter;
                        }
                    }
                    if (width == startr2R && length < startr2D && length >= startr2U)
                    {
                        //Right wall
                        map1[length, width] = outerSideR;
                    }
                    if (width == startr2L - 1 && length < startr2D && length >= startr2U)
                    {
                        //Left wall
                        map1[length, width] = outerSideL;
                    }
                    if (length == startr2D && width < startr2R && width >= startr2L || length == startr2U - 1 && width < startr2R && width >= startr2L)
                    {
                        //Up and Down
                        map1[length, width] = outerUp;
                    }
                    if (length == startr2U - 1 && width == startr2L - 1 || length == startr2D && width == startr2L - 1)
                    {
                        //Right corners
                        map1[length, width] = rightConers;
                    }
                    if (length == startr2U - 1 && width == startr2R || length == startr2D && width == startr2R)
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
                    currentPlayerPosition[0] = Int32.Parse(seed.Substring(16, 2));
                    currentPlayerPosition[1] = Int32.Parse(seed.Substring(18, 2));
                    currentEnemyPosition[0] = Int32.Parse(seed.Substring(20, 2));
                    currentEnemyPosition[1] = Int32.Parse(seed.Substring(22, 2));
                    map1[currentPlayerPosition[0], currentPlayerPosition[1]] = character;
                    map1[currentEnemyPosition[0], currentEnemyPosition[1]] = enemy1;
                }
                createSeed();
                DrawMap3();
            }
            static bool CheckRoomPosition()
            {
                bool problem = false;
                //V5
                if (startr1D >= startr2U && startr1D <= startr2D)
                {
                    if (startr1R >= startr2L && startr1R <= startr2R)
                    {
                        problem = true;
                    }
                    if (startr1L >= startr2L && startr1L <= startr2R)
                    {
                        problem = true;
                    }
                    //Room 2
                    if (startr2R >= startr1L && startr2R <= startr1R)
                    {
                        problem = true;
                    }
                    if (startr2L - 1 >= startr1L && startr2L - 1 <= startr1R)
                    {
                        problem = true;
                    }
                }
                if (startr1U >= startr2U && startr1U <= startr2D)
                {
                    if (startr1R >= startr2L && startr1R <= startr2R)
                    {
                        problem = true;
                    }
                    if (startr1L >= startr2L && startr1L <= startr2R)
                    {
                        problem = true;
                    }
                    //Room 2
                    if (startr2R >= startr1L && startr2R <= startr1R)
                    {
                        problem = true;
                    }
                    if (startr2L - 1 >= startr1L && startr2L - 1 <= startr1R)
                    {
                        problem = true;
                    }
                }
                //R2
                if (startr2D >= startr1U && startr2D <= startr1D)
                {
                    if (startr1R >= startr2L && startr1R <= startr2R)
                    {
                        problem = true;
                    }
                    if (startr1L >= startr2L && startr1L <= startr2R)
                    {
                        problem = true;
                    }
                    //Room 2
                    if (startr2R >= startr1L && startr2R <= startr1R)
                    {
                        problem = true;
                    }
                    if (startr2L >= startr1L && startr2L <= startr1R)
                    {
                        problem = true;
                    }
                }
                if (startr2U - 1 >= startr1U && startr2U - 1 <= startr1D)
                {
                    if (startr1R >= startr2L && startr1R <= startr2R)
                    {
                        problem = true;
                    }
                    if (startr1L >= startr2L && startr1L <= startr2R)
                    {
                        problem = true;
                    }
                    //Room 2
                    if (startr2R >= startr1L && startr2R <= startr1R)
                    {
                        problem = true;
                    }
                    if (startr2L - 1 >= startr1L && startr2L - 1 <= startr1R)
                    {
                        problem = true;
                    }
                }
                return problem;
            }
            static void MakeMap4Nothing()
            {
                try
                {
                    failedMap = false;
                    //Clears the console so you dont see the previous stuff.
                    //Console.Clear();
                    //Makes it so 2 attacks cant happen at the same time
                    attackHappened = false;
                    firstTimeMap = true;
                    //Draws the map
                    total = 0;
                    int length = 0;
                    length = 0;
                    for (int width = 1; total < mapSizeL_4 * mapSizeW_4; width++)
                    {
                        //The function that Makes map empty
                        total = width * length;
                        map1[length, width] = "**";
                        if (width > innerMapSizeW_4)
                        {
                            width = 0;
                            length++;
                            Console.WriteLine();
                        }
                        if (length > innerMapSizeL_4)
                        {
                            length = 0;
                            total = mapSizeL_4 * mapSizeW_4;
                        }
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    //Console.Clear();
                    Console.WriteLine("Error 4:");
                    Console.WriteLine("Something went outside the range of the map while the space was being made!");
                    Console.WriteLine("Press enter to try again");
                    Console.ReadLine();
                    NewMapV4();
                }
            }
            static void DrawMap4()
            {
                try
                {
                    //So you dont see the previous "frame".
                    //Console.Clear();
                    //Makes it so 2 attacks cant happen at the same time
                    attackHappened = false;
                    total = 0;
                    int length = 0;
                    for (int width = 1; total < mapSizeL_4 * mapSizeW_4; width++)
                    {
                        total = width * length;
                        if (width > innerMapSizeW_4)
                        {
                            width = 0;
                            length++;
                            Console.WriteLine();
                        }
                        if (length > innerMapSizeL_4)
                        {
                            length = 0;
                            total = mapSizeL_4 * mapSizeW_4;
                        }
                        Console.Write(map1[length, width]);
                    }
                    Console.WriteLine();
                }
                catch (IndexOutOfRangeException)
                {
                    //Console.Clear();
                    Console.WriteLine("Error 4:");
                    Console.WriteLine("Something went outside of the map while drawing the map to the screen!");
                    Console.WriteLine("Press enter to try again");
                    Console.ReadLine();
                    NewMapV4();
                }
            }
            static void CreateRandomMapV4()
            {
                //Rooms
                //Room checks
                //Paths
                //Player and enemies
            }
            static void NewMapV4()
            {
                MakeMap4Nothing();
                CreateRandomMapV4();
                DrawMap4();
                Console.WriteLine("Map generated");
            }
            static void MapSizeV4()
            {
                specificmapbool = false;
                Console.WriteLine("Map Generation V4.0");
                Console.WriteLine("Warning: Max size is " + biggestMapSize_4 + "!");
                Console.WriteLine("Warning: Min size is " + smallestMapSize_4 + "!");
                Console.WriteLine("Recommended: " + recommendedMapSize_4[0] + "x" + recommendedMapSize_4[1] + " with " + recommendedMapSize_4[2] + " rooms.");
                Console.WriteLine("Map Length");
                string answerL = "32";
                try
                {
                    answerL = Console.ReadLine();
                    if (answerL == "seed")
                    {
                        specificmapbool = true;
                        askSeed();
                    }
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
                    MapSizeV4();
                }
                if (Int32.Parse(answerL) > biggestMapSize_4 - 2)
                {
                    Console.Clear();
                    Console.WriteLine("Error 2:");
                    Console.WriteLine("Answer too big");
                    Console.WriteLine("Press enter to try again");
                    Console.ReadLine();
                    MapSizeV4();
                }
                else if (Int32.Parse(answerL) < smallestMapSize_4)
                {
                    Console.Clear();
                    Console.WriteLine("Error 3:");
                    Console.WriteLine("Answer too small");
                    Console.WriteLine("Press enter to try again");
                    Console.ReadLine();
                    MapSizeV4();
                }
                mapSizeL_4 = Int32.Parse(answerL);
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
                    MapSizeV4();
                }
                if (Int32.Parse(answerW) > biggestMapSize_4 - 2)
                {
                    Console.Clear();
                    Console.WriteLine("Error 2:");
                    Console.WriteLine("Answer too big");
                    Console.WriteLine("Press enter to try again");
                    Console.ReadLine();
                    MapSizeV4();
                }
                else if (Int32.Parse(answerW) < smallestMapSize_4)
                {
                    Console.Clear();
                    Console.WriteLine("Error 3:");
                    Console.WriteLine("Answer too small");
                    Console.WriteLine("Press enter to try again");
                    Console.ReadLine();
                    MapSizeV4();
                }
                mapSizeW_4 = Int32.Parse(answerW);
                Console.WriteLine("Amount of Rooms");
                string answerR = "2";
                try
                {
                    answerR = Console.ReadLine();
                    int result = Int32.Parse(answerR);
                }
                catch (FormatException)
                {
                    Console.Clear();
                    Console.WriteLine("Error 1:");
                    Console.WriteLine("Cannot convert " + answerR + " to a number.");
                    Console.WriteLine("Press enter to try again");
                    Console.ReadLine();
                    MapSizeV4();
                }
                if (Int32.Parse(answerR) > maxRooms)
                {
                    Console.Clear();
                    Console.WriteLine("Error 2:");
                    Console.WriteLine("Answer too big , " + maxRooms + " is the maximum");
                    Console.WriteLine("Press enter to try again");
                    Console.ReadLine();
                    MapSizeV4();
                }
                else if (Int32.Parse(answerR) < minRooms)
                {
                    Console.Clear();
                    Console.WriteLine("Error 3:");
                    Console.WriteLine("Answer too small , " + minRooms + " is the minimum");
                    Console.WriteLine("Press enter to try again");
                    Console.ReadLine();
                    MapSizeV4();
                }
                amountOfRooms = Int32.Parse(answerR);
                innerMapSizeW_4 = mapSizeW_4 - 2;
                innerMapSizeL_4 = mapSizeL_4 - 2;
                Console.WriteLine(innerMapSizeW_4 + " Inner W");
                Console.WriteLine(innerMapSizeL_4 + " Inner L");
                Console.WriteLine(mapSizeW_4 + " Outer W");
                Console.WriteLine(mapSizeL_4 + " Outer L");
                Console.WriteLine(amountOfRooms + " Rooms");
            }
        }
    }
}