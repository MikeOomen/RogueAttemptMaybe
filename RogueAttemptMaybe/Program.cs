using System;
using System.Diagnostics;
using System.Formats.Asn1;
using System.Runtime.CompilerServices;

namespace RogueAttemptMaybe
    {
    //Current bugs:
    //enemies attack your last position.
    //The map. In general.

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
        static string playerCharacter = "@ ";
        static string enemy1 = "E ";
        static string innerWall = "{}";
        static string outerUp = "--";
        static string outerSideL = "| ";
        static string outerSideR = " |";
        static string rightConers = "|-";
        static string leftConers = "-|";
        //Map Balancing
        static int[] recommendedMapSize = { 32, 32, 2 };
        static int biggestMapSize = 64 + 2; //First number is actual size!!
        static int smallestMapSize = 32;
        //Map General
        static int total = 0;
        static int[] currentPlayerPosition = { 0, 0 };
        static int[] currentEnemyPosition = { 0, 0 };
        static bool specificmapbool = false;
        //Map sizes V3
        /*        static int[] currentWallPosition = { 0, 0, 0, 0 };
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
                static string[] randomWorkingMaps = { "0214142622300612160725053232", "0315011423292030061306103232", "0719183018300110221322163232" };
                static string[] randomBrokenMaps = { "1022011418301424121012173232", "0214122411190112200919113232", "1022152723290517151224123232", "0214051711171830101205023232" };*/
        //Map V4
        static int mapLength = 16;
        static int mapWidth = 16;
        static int innerMapLength = 0;
        static int innerMapWidth = 0;

        static int amountOfRooms = 0;
        static int maxRooms = 5;
        static int minRooms = 1;

        static int[] roomsLeft = new int[5];
        static int[] roomsRight = new int[5];
        static int[] roomsUp = new int[5];
        static int[] roomsDown = new int[5];
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
        static string[,] map = new string[biggestMapSize, biggestMapSize];
        static void Main(string[] args)
        {
            MapSizeV4();
            NewMapV4(true);
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
        /*                MapSize();
                NewMap();*/
                AwaitMovementKey();
            }
            static void Move(ConsoleKey key)
            {
                EnemyMove();
                switch (key)
                {
                    case ConsoleKey.DownArrow:
                        {
                            if (map[currentPlayerPosition[0] + 1, currentPlayerPosition[1]] == floorCharacter)
                            {
                                map[currentPlayerPosition[0], currentPlayerPosition[1]] = floorCharacter;
                                currentPlayerPosition[0] = currentPlayerPosition[0] + 1;
                                map[currentPlayerPosition[0], currentPlayerPosition[1]] = playerCharacter;
                                CheckIfAttack("Player");
                                DrawFullMap4();
                                AwaitMovementKey();
                            }
                            else
                            {
                                CheckIfAttack("Player");
                                DrawMap4();
                                AwaitMovementKey();
                            }
                            break;
                        }
                    case ConsoleKey.UpArrow:
                        {
                            if (map[currentPlayerPosition[0] - 1, currentPlayerPosition[1]] == floorCharacter)
                            {
                                map[currentPlayerPosition[0], currentPlayerPosition[1]] = floorCharacter;
                                currentPlayerPosition[0] = currentPlayerPosition[0] - 1;
                                map[currentPlayerPosition[0], currentPlayerPosition[1]] = playerCharacter;
                                CheckIfAttack("Player");
                                DrawMap4();
                                AwaitMovementKey();
                            }
                            else
                            {
                                CheckIfAttack("Player");
                                DrawMap4();
                                AwaitMovementKey();
                            }
                            break;
                        }
                    case ConsoleKey.LeftArrow:
                        {
                            if (map[currentPlayerPosition[0], currentPlayerPosition[1] - 1] == floorCharacter)
                            {
                                map[currentPlayerPosition[0], currentPlayerPosition[1]] = floorCharacter;
                                currentPlayerPosition[1] = currentPlayerPosition[1] - 1;
                                map[currentPlayerPosition[0], currentPlayerPosition[1]] = playerCharacter;
                                CheckIfAttack("Player");
                                DrawMap4();
                                AwaitMovementKey();
                            }
                            else
                            {
                                CheckIfAttack("Player");
                                DrawMap4();
                                AwaitMovementKey();
                            }
                            break;
                        }
                    case ConsoleKey.RightArrow:
                        {
                            if (map[currentPlayerPosition[0], currentPlayerPosition[1] + 1] == floorCharacter)
                            {
                                map[currentPlayerPosition[0], currentPlayerPosition[1]] = floorCharacter;
                                currentPlayerPosition[1] = currentPlayerPosition[1] + 1;
                                map[currentPlayerPosition[0], currentPlayerPosition[1]] = playerCharacter;
                                CheckIfAttack("Player");
                                DrawMap4();
                                AwaitMovementKey();
                            }
                            else
                            {
                                CheckIfAttack("Player");
                                DrawMap4();
                                AwaitMovementKey();
                            }
                            break;
                        }
                    case ConsoleKey.N:
                        {
                            if (specificmapbool == false)
                            {
                                //MakeMap3Nothing();
                            }
                            else
                            {
                                //useSpecificSeed(newSeed);
                            }
                            break;
                        }
                    case ConsoleKey.M:
                        {
                            //MapSize();
                            //NewMap();
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
                            if (map[currentEnemyPosition[0] + 1, currentEnemyPosition[1]] == floorCharacter)
                            {
                                map[currentEnemyPosition[0], currentEnemyPosition[1]] = floorCharacter;
                                currentEnemyPosition[0] = currentEnemyPosition[0] + 1;
                                map[currentEnemyPosition[0], currentEnemyPosition[1]] = enemy1;
                                CheckIfAttack("Enemy");
                            }
                            else
                            {
                                L = 0;
                            }
                        }
                        else if (currentPlayerPosition[0] <= currentEnemyPosition[0])
                        {
                            if (map[currentEnemyPosition[0] - 1, currentEnemyPosition[1]] == floorCharacter)
                            {
                                map[currentEnemyPosition[0], currentEnemyPosition[1]] = floorCharacter;
                                currentEnemyPosition[0] = currentEnemyPosition[0] - 1;
                                map[currentEnemyPosition[0], currentEnemyPosition[1]] = enemy1;
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
                            if (map[currentEnemyPosition[0], currentEnemyPosition[1] - 1] == floorCharacter)
                            {
                                map[currentEnemyPosition[0], currentEnemyPosition[1]] = floorCharacter;
                                currentEnemyPosition[1] = currentEnemyPosition[1] - 1;
                                map[currentEnemyPosition[0], currentEnemyPosition[1]] = enemy1;
                                CheckIfAttack("Enemy");
                            }
                            else
                            {
                                W = 0;
                            }
                        }
                        else if (currentPlayerPosition[1] >= currentEnemyPosition[1])
                        {
                            if (map[currentEnemyPosition[0], currentEnemyPosition[1] + 1] == floorCharacter)
                            {
                                map[currentEnemyPosition[0], currentEnemyPosition[1]] = floorCharacter;
                                currentEnemyPosition[1] = currentEnemyPosition[1] + 1;
                                map[currentEnemyPosition[0], currentEnemyPosition[1]] = enemy1;
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
                            if (map[currentEnemyPosition[0], currentEnemyPosition[1] - 1] == floorCharacter)
                            {
                                map[currentEnemyPosition[0], currentEnemyPosition[1]] = floorCharacter;
                                currentEnemyPosition[1] = currentEnemyPosition[1] - 1;
                                map[currentEnemyPosition[0], currentEnemyPosition[1]] = enemy1;
                                CheckIfAttack("Enemy");
                            }
                            else
                            {
                                L = 0;
                            }
                        }
                        else if (currentPlayerPosition[1] >= currentEnemyPosition[1])
                        {
                            if (map[currentEnemyPosition[0], currentEnemyPosition[1] + 1] == floorCharacter)
                            {
                                map[currentEnemyPosition[0], currentEnemyPosition[1]] = floorCharacter;
                                currentEnemyPosition[1] = currentEnemyPosition[1] + 1;
                                map[currentEnemyPosition[0], currentEnemyPosition[1]] = enemy1;
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
                            if (map[currentEnemyPosition[0] + 1, currentEnemyPosition[1]] == floorCharacter)
                            {
                                map[currentEnemyPosition[0], currentEnemyPosition[1]] = floorCharacter;
                                currentEnemyPosition[0] = currentEnemyPosition[0] + 1;
                                map[currentEnemyPosition[0], currentEnemyPosition[1]] = enemy1;
                                CheckIfAttack("Enemy");
                            }
                            else
                            {
                                W = 0;
                            }
                        }
                        else if (currentPlayerPosition[0] <= currentEnemyPosition[0])
                        {
                            if (map[currentEnemyPosition[0] - 1, currentEnemyPosition[1]] == floorCharacter)
                            {
                                map[currentEnemyPosition[0], currentEnemyPosition[1]] = floorCharacter;
                                currentEnemyPosition[0] = currentEnemyPosition[0] - 1;
                                map[currentEnemyPosition[0], currentEnemyPosition[1]] = enemy1;
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
                        if (map[currentEnemyPosition[0] - 1, currentEnemyPosition[1]] == playerCharacter || map[currentEnemyPosition[0] + 1, currentEnemyPosition[1]] == playerCharacter || map[currentEnemyPosition[0], currentEnemyPosition[1] - 1] == playerCharacter || map[currentEnemyPosition[0], currentEnemyPosition[1] + 1] == playerCharacter)
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
            /*            static bool CheckRoomPosition()
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
                    return problem;*/
        }
        static void MakeMap4Nothing()
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
                for (int width = 1; total < mapLength * mapWidth; width++)
                {
                    //The function that Makes map empty
                    total = width * length;
                    //map[length, width] = "**";
                    //map[length, width] = width.ToString();
                    //map[length, width] = length.ToString();
                    map[length, width] = floorCharacter;
                    if (width > innerMapWidth)
                    {
                        width = 0;
                        length++;
                        Console.WriteLine();
                    }
                    if (length > innerMapLength)
                    {
                        length = 0;
                        total = mapLength * mapWidth;
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
                NewMapV4(true);
            }
        }
        static void DrawFullMap4()
        {
            try
            {
                //So you dont see the previous "frame".
                //Console.Clear();
                //Makes it so 2 attacks cant happen at the same time
                attackHappened = false;
                total = 0;
                int length = 0;
                for (int width = 1; total < mapLength * mapWidth; width++)
                {
                    total = width * length;
                    if (width > innerMapWidth)
                    {
                        width = 0;
                        length++;
                        Console.WriteLine();
                    }
                    if (length > innerMapLength)
                    {
                        length = 0;
                        total = mapLength * mapWidth;
                    }
                    Console.Write(map[length, width]);
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
                NewMapV4(true);
            }
        }
        static void DrawMapDistance()
        {
            try
            {
                //So you dont see the previous "frame".
                //Console.Clear();
                //Makes it so 2 attacks cant happen at the same time
                attackHappened = false;
                total = 0;
                int length = 0;
                for (int width = 1; total < mapLength * mapWidth; width++)
                {
                    total = width * length;
                    if (width > innerMapWidth)
                    {
                        width = 0;
                        length++;
                        Console.WriteLine();
                    }
                    if (length > innerMapLength)
                    {
                        length = 0;
                        total = mapLength * mapWidth;
                    }
                    if (width >= currentPlayerPosition[0] - 16 && width <= currentPlayerPosition[0] + 16 || width == 0)
                    {
                        if (length >= currentPlayerPosition[1] - 16 && length <= currentPlayerPosition[1] + 16)
                        {
                            Console.Write(map[length, width]);
                        }
                        //Console.Write(map[length, width]);
                    }
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
                NewMapV4(true);
            }
        }
        static void CreateRandomMapV4(bool random)
        {
            currentPlayerPosition[0] = 20;
            currentPlayerPosition[1] = 20;
            map[20,20] = playerCharacter;
            //Rooms
            //Room checks
            //Paths
            //Player and enemies
        }
        static void NewMapV4(bool random)
        {
            MakeMap4Nothing();
            if (random)
            {
                CreateRandomMapV4(true);
            }
            DrawMap4();
            Console.WriteLine("Map generated");
        }
        static void DrawMap4()
        {
            //DrawFullMap4();
            DrawMapDistance();
        }
        static void MapSizeV4()
        {
            specificmapbool = false;
            Console.WriteLine("Map Generation V4.0");
            int biggestMap = biggestMapSize - 2;
            Console.WriteLine("Warning: Max size is " + biggestMap + "!");
            Console.WriteLine("Warning: Min size is " + smallestMapSize + "!");
            Console.WriteLine("Recommended: " + recommendedMapSize[0] + "x" + recommendedMapSize[1] + " with " + recommendedMapSize[2] + " rooms.");
            Console.WriteLine("Map Length");
            string answerL = "32";
            try
            {
                answerL = Console.ReadLine();
                if (answerL == "seed")
                {
                    specificmapbool = true;
                    //askSeed();
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
            if (Int32.Parse(answerL) > biggestMapSize - 2)
            {
                Console.Clear();
                Console.WriteLine("Error 2:");
                Console.WriteLine("Answer too big");
                Console.WriteLine("Press enter to try again");
                Console.ReadLine();
                MapSizeV4();
            }
            else if (Int32.Parse(answerL) < smallestMapSize)
            {
                Console.Clear();
                Console.WriteLine("Error 3:");
                Console.WriteLine("Answer too small");
                Console.WriteLine("Press enter to try again");
                Console.ReadLine();
                MapSizeV4();
            }
            mapLength = Int32.Parse(answerL);
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
            if (Int32.Parse(answerW) > biggestMapSize - 2)
            {
                Console.Clear();
                Console.WriteLine("Error 2:");
                Console.WriteLine("Answer too big");
                Console.WriteLine("Press enter to try again");
                Console.ReadLine();
                MapSizeV4();
            }
            else if (Int32.Parse(answerW) < smallestMapSize)
            {
                Console.Clear();
                Console.WriteLine("Error 3:");
                Console.WriteLine("Answer too small");
                Console.WriteLine("Press enter to try again");
                Console.ReadLine();
                MapSizeV4();
            }
            mapWidth = Int32.Parse(answerW);
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
            innerMapWidth = mapWidth - 2;
            innerMapLength = mapLength - 2;
            Console.WriteLine(innerMapWidth + " Inner W");
            Console.WriteLine(innerMapLength + " Inner L");
            Console.WriteLine(mapWidth + " Outer W");
            Console.WriteLine(mapLength + " Outer L");
            Console.WriteLine(amountOfRooms + " Rooms");
        }
    }
}