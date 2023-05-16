using System;
using System.Diagnostics;
using System.Drawing;
using System.Formats.Asn1;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace RogueAttemptMaybe
{
    //Current bugs:
    //enemies attack your last position.
    //The map. In general.

    //Console.Beep(600 , 1000); //, Long quiet beep
    //Console.Beep(750 , 500); //, Short louder beep
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
        static bool[] loaded = { false, false, false, false, false };
        static bool hasInfo = false;
        static bool beeps = true;
        static string currentMusic = "boss";
        static bool musicPlaying = false;

        static bool playerCanAMoveEverywhere = false;

        //Main Menu
        static string name;
        static bool characterHasBeenMade = false;
        static int mainMenuSelected = 1;

        //Characters
        const string floorCharacter = "  ";
        const string playerCharacter = "@ ";
        const string enemy1 = "E ";
        const string outerUp = "--";
        const string outerSideL = "| ";
        const string outerSideR = " |";
        const string rightConers = "-|";
        const string leftConers = "|-";
        const string inBetweenRooms = "**";

        //Map Balancing
        static int[] recommendedMapSize = { 48, 48, 6 };
        const int biggestMapSize = 128 + 2; //First number is actual size
        const int smallestMapSize = 32;

        //Map General
        static int total = 0;
        static int[] currentPlayerPosition = { 0, 0 };
        static int[] currentEnemyPosition = { 0, 0 };
        static bool specificmapbool = false;

        //Map V4

        static string version = "V4.3";
        static int mapLength = 16;
        static int mapWidth = 16;
        static int innerMapLength = 0;
        static int innerMapWidth = 0;

        static int spaceBetweenRooms = 3;
        static int amountOfRooms = 0;
        static int maxRooms = 10;
        static int minRooms = 1;
        static int minRoomSize = 2;
        static int maxRoomSize = 10;

        static int[] roomsPosLengths = new int[maxRooms];
        static int[] roomsPosWidths = new int[maxRooms];
        static int[] roomMidL = new int[maxRooms];
        static int[] roomMidW = new int[maxRooms];
        static int[] roomsLeft = new int[maxRooms];
        static int[] roomsRight = new int[maxRooms];
        static int[] roomsUp = new int[maxRooms];
        static int[] roomsDown = new int[maxRooms];

        static int[] bestPerRoom = new int[1000];
        static int checkedRooms = 0;

        static int seeingDistance = 16; //Breaks above 18

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
            Console.Title = "C# Rogue Game";
            Thread music1 = new Thread(new ThreadStart(musicTrack1));
            Thread music2 = new Thread(new ThreadStart(musicTrack2));
            Thread music3 = new Thread(new ThreadStart(musicTrack3));
            music1.Start();
            music2.Start();
            music3.Start();
            FullScreenWarning();
            musicPlaying = false;
            MapSizeV4();
            NewMapV4(true);
            AwaitMovementKey();
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
            mainMenuSelected = 1;
            MainMenu();
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
                MapSizeV4();
                NewMapV4(true);
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
                                //CheckIfAttack("Player");
                                DrawMap4();
                                AwaitMovementKey();
                            }
                            else
                            {
                                if (playerCanAMoveEverywhere)
                                {
                                    map[currentPlayerPosition[0], currentPlayerPosition[1]] = floorCharacter;
                                    currentPlayerPosition[0] = currentPlayerPosition[0] + 1;
                                    map[currentPlayerPosition[0], currentPlayerPosition[1]] = playerCharacter;
                                }
                                //CheckIfAttack("Player");
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
                                //CheckIfAttack("Player");
                                DrawMap4();
                                AwaitMovementKey();
                            }
                            else
                            {
                                if (playerCanAMoveEverywhere)
                                {
                                    map[currentPlayerPosition[0], currentPlayerPosition[1]] = floorCharacter;
                                    currentPlayerPosition[0] = currentPlayerPosition[0] - 1;
                                    map[currentPlayerPosition[0], currentPlayerPosition[1]] = playerCharacter;
                                }
                                //CheckIfAttack("Player");
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
                                //CheckIfAttack("Player");
                                DrawMap4();
                                AwaitMovementKey();
                            }
                            else
                            {
                                if (playerCanAMoveEverywhere)
                                {
                                    map[currentPlayerPosition[0], currentPlayerPosition[1]] = floorCharacter;
                                    currentPlayerPosition[1] = currentPlayerPosition[1] - 1;
                                    map[currentPlayerPosition[0], currentPlayerPosition[1]] = playerCharacter;
                                }
                                //CheckIfAttack("Player");
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
                                //CheckIfAttack("Player");
                                DrawMap4();
                                AwaitMovementKey();
                            }
                            else
                            {
                                if (playerCanAMoveEverywhere)
                                {
                                    map[currentPlayerPosition[0], currentPlayerPosition[1]] = floorCharacter;
                                    currentPlayerPosition[1] = currentPlayerPosition[1] + 1;
                                    map[currentPlayerPosition[0], currentPlayerPosition[1]] = playerCharacter;
                                }
                                //CheckIfAttack("Player");
                                DrawMap4();
                                AwaitMovementKey();
                            }
                            break;
                        }
                    case ConsoleKey.N:
                        {
                            if (specificmapbool == false)
                            {
                                beeps = false;
                                NewMapV4(true);
                            }
                            else
                            {
                                //useSpecificSeed(newSeed);
                            }
                            break;
                        }
                    case ConsoleKey.M:
                        {
                            MapSizeV4();
                            NewMapV4(true);
                            break;
                        }
                    case ConsoleKey.F:
                        {
                            DrawFullMap4();
                            AwaitMovementKey();
                            break;
                        }
                }
            }
            static void AwaitMovementKey()
            {
                //makes sure you actually use an arrow
                currentMusic = "looting";
                ConsoleKey key = Console.ReadKey().Key;
                if (key == ConsoleKey.DownArrow || key == ConsoleKey.UpArrow || key == ConsoleKey.LeftArrow || key == ConsoleKey.RightArrow || key == ConsoleKey.N || key == ConsoleKey.M || key == ConsoleKey.F)
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
                Console.Clear();
                currentMusic = "fighting";
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
                    Console.Write($"{starterWeaponName[i]} ");
                    Console.Write($"Dmg:{starterWeaponDmg[i]} ");
                    Console.Write($"CC:{starterWeaponCritChance[i]} ");
                    Console.WriteLine($"CMulti:{starterWeaponCritMulti[i]}");
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
                        Console.Write($"{starterWeaponName[i]} ");
                        Console.Write($"Dmg:{starterWeaponDmg[i]} ");
                        Console.Write($"CC:{starterWeaponCritChance[i]} ");
                        Console.WriteLine($"CMulti:{starterWeaponCritMulti[i]}");
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
                                Console.WriteLine($"Current weapon:{starterWeaponName[selectedWeapon]}");
                            }
                            else
                            {
                                selectedWeapon = 0;
                                Console.WriteLine($"Current weapon:{starterWeaponName[selectedWeapon]}");
                            }
                            break;
                        }
                    case ConsoleKey.DownArrow:
                        {
                            if (selectedWeapon > 0)
                            {
                                selectedWeapon = selectedWeapon - 1;
                                Console.WriteLine($"Current weapon:{starterWeaponName[selectedWeapon]}");
                            }
                            else
                            {
                                selectedWeapon = starterWeapons.Length - 1;
                                Console.WriteLine($"Current weapon:{starterWeaponName[selectedWeapon]}");
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
                    MapLoadingBar(0, "Clearing map");
                    for (int width = 0; total < mapLength * mapWidth; width++)
                    {
                        //The function that Makes map empty
                        //total = width * length;
                        map[length, width] = inBetweenRooms;
                        //map[length, width] = width.ToString();
                        //map[length, width] = length.ToString();
                        //map[length, width] = floorCharacter;
                        if (width > mapWidth)
                        {
                            width = -1;
                            length++;
                        }
                        if (length > mapLength)
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
            static void DecideRandomRooms(int i)
            {
                MapLoadingBar(1, "Adding Rooms");
                Random random = new Random();
                int posRoomLength = random.Next(0, mapLength);
                int posRoomWidth = random.Next(0, mapWidth);
                int roomLeftSize = random.Next(minRoomSize, maxRoomSize);
                int roomRightSize = random.Next(minRoomSize, maxRoomSize);
                int roomUpSize = random.Next(minRoomSize, maxRoomSize);
                int roomDownSize = random.Next(minRoomSize, maxRoomSize);
                int roomLeft = posRoomWidth - roomLeftSize;
                int roomRight = posRoomWidth + roomRightSize;
                int roomDown = posRoomLength - roomDownSize;
                int roomUp = posRoomLength + roomUpSize;
                //Sides of the map
                if (roomLeft < 0)
                {
                    roomRight = roomRight + -roomLeft;
                    roomLeft = 0;
                }
                if (roomDown < 0)
                {
                    roomUp = roomUp + -roomDown;
                    roomDown = 0;
                }
                if (roomRight > mapWidth)
                {
                    //Doesnt work?
                    roomLeft = roomLeft + -(roomRight - mapWidth);
                    if (roomLeft < 0)
                    {
                        Console.WriteLine("Error 7:");
                        Console.WriteLine("A room is hitting both walls of the map!");
                        Console.WriteLine("Press enter to try again");
                        Console.ReadLine();
                        roomRight = roomRight + roomLeft;
                    }
                    roomRight = mapWidth;
                }
                if (roomUp > mapLength)
                {
                    //Doesnt work?
                    roomDown = roomDown + -(roomUp - mapLength);
                    if (roomDown < 0)
                    {
                        Console.WriteLine("Error 7:");
                        Console.WriteLine("A room is hitting both walls of the map!");
                        Console.WriteLine("Press enter to try again");
                        Console.ReadLine();
                        roomUp = roomUp + roomDown;
                    }
                    roomUp = mapLength;
                }
                bool problem = false;
                problem = RoomPositionProblemFinder(roomLeft, roomRight, roomUp, roomDown, posRoomLength, posRoomWidth);
                if (problem)
                {
                    roomsLeft[i] = 0;
                    roomsRight[i] = 0;
                    roomsDown[i] = 0;
                    roomsUp[i] = 0;
                    DecideRandomRooms(i);
                }
                else
                {
                    if (hasInfo)
                    {
                        /*Console.WriteLine($"Room {i} Finished Info:");
                        Console.WriteLine($"{roomLeft} Left");
                        Console.WriteLine($"{roomRight} Right");
                        Console.WriteLine($"{roomUp} Up");
                        Console.WriteLine($"{roomDown} Down");
                        Console.WriteLine();*/
                    }
                    checkedRooms++;
                    roomsPosLengths[i] = posRoomLength;
                    roomsPosWidths[i] = posRoomWidth;
                    roomsLeft[i] = roomLeft;
                    roomsRight[i] = roomRight;
                    roomsDown[i] = roomDown;
                    roomsUp[i] = roomUp;
                }
            }
            static bool RoomPositionProblemFinder(int left, int right, int up, int down, int midLength, int midWidth)
            {
                bool problem = false;
                for (int i = 0; i < checkedRooms; i++)
                {
                    for (int L = left; L <= right; L++)
                    {
                        if (L >= roomsLeft[i] - spaceBetweenRooms && L <= roomsRight[i] + spaceBetweenRooms)
                        {
                            for (int D = down; D <= up; D++)
                            {
                                if (D >= roomsDown[i] - spaceBetweenRooms && D <= roomsUp[i] + spaceBetweenRooms)
                                {
                                    problem = true;
                                }
                            }
                        }
                    }
                }
                if (problem)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    /*Console.WriteLine("Problem!!");*/
                    //Console.Beep(600, 1000);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                return problem;
            }
            static void GenerateRoomPositions()
            {
                MapLoadingBar(3, "Adding Rooms");
                for (int i = 0; i < amountOfRooms; i++)
                {
                    int roomLeft = roomsLeft[i];
                    int roomRight = roomsRight[i];
                    int roomDown = roomsDown[i];
                    int roomUp = roomsUp[i];
                    total = 0;
                    int length = 0;
                    for (int width = 0; total < mapLength * mapWidth; width++)
                    {
                        total = width * length;
                        if (width > roomLeft && width < roomRight)
                        {
                            if (length < roomUp && length > roomDown)
                            {
                                map[length, width] = floorCharacter;
                            }
                        }
                        //Generate Walls
                        if (width == roomLeft)
                        {
                            if (length < roomUp && length > roomDown)
                            {
                                map[length, width] = outerSideL;
                            }
                        }
                        if (width == roomRight)
                        {
                            if (length < roomUp && length > roomDown)
                            {
                                map[length, width] = outerSideR;
                            }
                        }
                        if (length == roomUp)
                        {
                            if (width > roomLeft && width < roomRight)
                            {
                                map[length, width] = outerUp;
                            }
                        }
                        if (length == roomDown)
                        {
                            if (width > roomLeft && width < roomRight)
                            {
                                map[length, width] = outerUp;
                            }
                        }
                        //Sides
                        if (length == (roomsUp[i] + roomsDown[i]) / 2)
                        {
                            //map[length, width] = "EE";
                        }
                        //Corners
                        map[roomDown, roomLeft] = leftConers;
                        map[roomUp, roomLeft] = leftConers;
                        map[roomDown, roomRight] = rightConers;
                        map[roomUp, roomRight] = rightConers;
                        if (width > mapWidth)
                        {
                            width = -1;
                            length++;
                        }
                    }
                }
            }
            static void DecidePlayerPosition()
            {
                Random rnd = new Random();
                int room = rnd.Next(0, amountOfRooms);
                MapLoadingBar(2, "Spawning Player");
                if (roomsPosLengths[room] != 0 && roomsPosWidths[room] != 0)
                {
                    currentPlayerPosition[0] = roomsPosLengths[room];
                    currentPlayerPosition[1] = roomsPosWidths[room];
                    map[currentPlayerPosition[0], currentPlayerPosition[1]] = playerCharacter;
                }
                else
                {
                    System.Threading.Thread.Sleep(300);
                    DecidePlayerPosition();
                }
            }
            static void MiddleRoomSides()
            {
                for (int i = 0; i < amountOfRooms; i++)
                {
                    total = 0;
                    int length = 0;
                    for (int width = 0; total < mapLength * mapWidth; width++)
                    {
                        total = width * length;
                        if (width == roomsLeft[i] && length == (roomsUp[i] + roomsDown[i]) / 2)
                        {
                            map[length, width] = "L" + i;
                        }
                        if (width == roomsRight[i] && length == (roomsUp[i] + roomsDown[i]) / 2)
                        {
                            map[length, width] = "R" + i;
                        }

                        if (length == roomsUp[i] && width == (roomsRight[i] + roomsLeft[i]) / 2)
                        {
                            map[length, width] = "U" + i;
                        }
                        if (length == roomsDown[i] && width == (roomsRight[i] + roomsLeft[i]) / 2)
                        {
                            map[length, width] = "D" + i;
                        }

                        if (width > mapWidth)
                        {
                            width = -1;
                            length++;
                        }
                    }
                }
            }
            static void RoomPaths()
            {
                MapLoadingBar(4, "Adding paths");
                ThinkOfPaths();
                GeneratePath();
            }
            static void ThinkOfPaths()
            {
                bool[] hasPath = new bool[amountOfRooms];
                int checkedRoomDifferences = 0;
                int lowest = 1000;
                int lowestI = 0;
                int lowestJ = 0;
                int[,] score = new int[amountOfRooms, amountOfRooms];
                for (int i = 0; i < amountOfRooms; i++)
                {
                    for (int j = 0; j < amountOfRooms; j++)
                    {
                        if (j > checkedRoomDifferences)
                        {
                            if (i != j)
                            {
                                int smallestS = 1000;
                                int differenceL = roomsLeft[j] - roomsLeft[i];
                                int differenceR = roomsRight[j] - roomsRight[i];
                                int differenceLR = roomsLeft[j] - roomsRight[i];
                                int differenceRL = roomsRight[j] - roomsLeft[i];

                                if (differenceL < 0)
                                {
                                    differenceL = -differenceL;
                                }
                                if (differenceR < 0)
                                {
                                    differenceR = -differenceR;
                                }
                                if (differenceLR < 0)
                                {
                                    differenceLR = -differenceLR;
                                }
                                if (differenceRL < 0)
                                {
                                    differenceRL = -differenceRL;
                                }

                                smallestS = differenceL;
                                if (smallestS > differenceR)
                                {
                                    smallestS = differenceR;
                                }
                                if (smallestS > differenceLR)
                                {
                                    smallestS = differenceLR;
                                }
                                if (smallestS > differenceRL)
                                {
                                    smallestS = differenceRL;
                                }

                                int differenceU = roomsUp[j] - roomsUp[i];
                                int differenceD = roomsDown[j] - roomsDown[i];
                                int differenceUD = roomsUp[j] - roomsDown[i];
                                int differenceDU = roomsDown[j] - roomsUp[i];
                                if (differenceU < 0)
                                {
                                    differenceU = -differenceU;
                                }
                                if (differenceD < 0)
                                {
                                    differenceD = -differenceD;
                                }
                                if (differenceUD < 0)
                                {
                                    differenceUD = -differenceUD;
                                }
                                if (differenceDU < 0)
                                {
                                    differenceDU = -differenceDU;
                                }

                                int smallestU = 1000;
                                smallestU = differenceU;
                                if (smallestU > differenceD)
                                {
                                    smallestU = differenceD;
                                }
                                if (smallestU > differenceUD)
                                {
                                    smallestU = differenceUD;
                                }
                                if (smallestU > differenceDU)
                                {
                                    smallestU = differenceDU;
                                }

                                score[i, j] = smallestU + smallestS;
                                if (smallestU > smallestS + 10 || smallestS > smallestU + 10)
                                {
                                    score[i, j] = score[i, j] * 2;
                                }
                            }
                        }
                        checkedRoomDifferences = i;
                    }
                }
                for (int i = 0; i < amountOfRooms; i++)
                {
                    for (int j = 0; j < amountOfRooms; j++)
                    {
                        if (score[i, j] == 0)
                        {
                            score[i, j] = 1000;
                        }
                    }
                }
                for (int i = 0; i < amountOfRooms - 1; i++)
                {
                    int bestForRoom = 100;
                    int bestScore = 100;
                    for (int j = 0; j < amountOfRooms; j++)
                    {
                        if (i != j || j < i)
                        {
                            if (score[i, j] < bestScore)
                            {
                                bestForRoom = j;
                                bestScore = score[i, j];
                            }
                        }
                    }
                    bestPerRoom[i] = bestForRoom;
                }
            }
            static void GeneratePath()
            {
                for (int i = 0; i < amountOfRooms; i++)
                {
                    int maxL = 100;
                    for (int j = 0; j < amountOfRooms; j++)
                    {
                        Console.WriteLine(roomsPosLengths[j] + " Room " + j);
                    }
                    Console.WriteLine(roomsPosLengths[i]);
                    Console.WriteLine(roomsPosLengths[bestPerRoom[i]] + "Best room");
                    Console.WriteLine(i + " Room");
                    for (int j = roomsPosLengths[i]; j > roomsPosLengths[bestPerRoom[i]]; j--)
                    {
                        try
                        {
                            map[j, roomsPosWidths[i]] = floorCharacter;
                            if (map[j, roomsPosWidths[i] + 1] == inBetweenRooms)
                            {
                                map[j, roomsPosWidths[i] + 1] = "|*";
                            }
                            if (map[j, roomsPosWidths[i] - 1] == inBetweenRooms)
                            {
                                map[j, roomsPosWidths[i] - 1] = "*|";
                            }
                            maxL = j;
                        }
                        catch (IndexOutOfRangeException)
                        {

                        }
                    }
                    for (int j = roomsPosLengths[i]; j < roomsPosLengths[bestPerRoom[i]]; j++)
                    {
                        try
                        {
                            map[j, roomsPosWidths[i]] = floorCharacter;
                            if (map[j, roomsPosWidths[i] + 1] == inBetweenRooms)
                            {
                                map[j, roomsPosWidths[i] + 1] = "|*";
                            }
                            if (map[j, roomsPosWidths[i] - 1] == inBetweenRooms)
                            {
                                map[j, roomsPosWidths[i] - 1] = "*|";
                            }
                            maxL = j;
                        }
                        catch (IndexOutOfRangeException)
                        {

                        }
                    }
                    for (int j = roomsPosWidths[i]; j > roomsPosWidths[bestPerRoom[i]]; j--)
                    {
                        try
                        {
                            map[maxL, j] = floorCharacter;
                            if (map[maxL + 1, j] == inBetweenRooms)
                            {
                                map[maxL + 1, j] = outerUp;
                            }
                            if (map[maxL - 1, j] == inBetweenRooms)
                            {
                                map[maxL - 1, j] = outerUp;
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {

                        }
                    }
                    for (int j = roomsPosWidths[i]; j < roomsPosWidths[bestPerRoom[i]]; j++)
                    {
                        try
                        {

                            map[maxL, j] = floorCharacter;
                            if (map[maxL + 1, j] == inBetweenRooms)
                            {
                                map[maxL + 1, j] = outerUp;
                            }
                            if (map[maxL - 1, j] == inBetweenRooms)
                            {
                                map[maxL - 1, j] = outerUp;
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {

                        }

                    }
                    try
                    {
                        if (map[maxL + 1, roomsPosWidths[i] + 1] == inBetweenRooms)
                        {
                            map[maxL + 1, roomsPosWidths[i] + 1] = "|*";
                        }
                        if (map[maxL - 1, roomsPosWidths[i] + 1] == inBetweenRooms)
                        {
                            map[maxL - 1, roomsPosWidths[i] + 1] = "|*";
                        }
                        if (map[maxL + 1, roomsPosWidths[i] - 1] == inBetweenRooms)
                        {
                            map[maxL + 1, roomsPosWidths[i] - 1] = "*|";
                        }
                        if (map[maxL - 1, roomsPosWidths[i] - 1] == inBetweenRooms)
                        {
                            map[maxL - 1, roomsPosWidths[i] - 1] = "*|";
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {

                    }
                }
            }
            static void DrawFullMap4()
            {
                try
                {
                    //Makes it so 2 attacks cant happen at the same time
                    attackHappened = false;
                    total = 0;
                    int length = 0;
                    for (int width = 0; total < mapLength * mapWidth; width++)
                    {
                        total = width * length;
                        MapColors(length, width);
                        if (width > mapWidth)
                        {
                            width = 0;
                            Console.WriteLine();
                            MapColors(length + 1, width);
                            length++;
                        }
                        if (length > mapLength)
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
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
            }
            static void DrawMapDistance()
            {
                try
                {
                    attackHappened = false;
                    total = 0;
                    int length = 0;
                    for (int width = 0; total < mapLength * mapWidth; width++)
                    {
                        total = width * length;
                        MapColors(length, width);
                        if (width > mapWidth)
                        {
                            width = 0;
                            MapColors(length + 1, width);
                            if (length >= currentPlayerPosition[0] - seeingDistance && length <= currentPlayerPosition[0] + seeingDistance)
                            {
                                Console.WriteLine();
                            }
                            length++;
                        }
                        if (length > mapLength)
                        {
                            length = 0;
                            total = mapLength * mapWidth;
                        }
                        if (width >= currentPlayerPosition[1] - seeingDistance && width <= currentPlayerPosition[1] + seeingDistance)
                        {
                            if (length >= currentPlayerPosition[0] - seeingDistance && length <= currentPlayerPosition[0] + seeingDistance)
                            {
                                Console.Write(map[length, width]);
                            }
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
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
            }
            static void MapColors(int length, int width)
            {
                switch (map[length, width])
                {
                    case inBetweenRooms:
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.BackgroundColor = ConsoleColor.Black;
                            break;
                        }
                    case playerCharacter:
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.BackgroundColor = ConsoleColor.Black;
                            break;
                        }
                    /*case floorCharacter:
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Black;
                            break;
                        }*/
                    case outerUp:
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Black;
                            break;
                        }
                    case outerSideL:
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Black;
                            break;
                        }
                    case outerSideR:
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Black;
                            break;
                        }
                    case rightConers:
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Black;
                            break;
                        }
                    case leftConers:
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Black;
                            break;
                        }
                    case enemy1:
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.BackgroundColor = ConsoleColor.Black;
                            break;
                        }
                    default:
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Black;
                            break;
                        }
                }
            }
            static void fixMiddle()
            {
                for (int i = 0; i < amountOfRooms; i++)
                {
                    for (int f = 0; f < 48; f++)
                    {
                        map[roomsDown[i] + ((roomsUp[i] - roomsDown[i]) / 2), roomsLeft[i] + ((roomsRight[i] - roomsLeft[i]) / 2)] = "Le";
                    }
                }
            }
            static void CreateRandomMapV4(bool random)
            {
                /*map[62, 62] = "Hi";
                map[2, 2] = "Te";
                map[33, 33] = "md";
                map[25, 25] = "st";
                map[10, 62] = "ee";
                map[62, 10] = "aa";*/
                for (int i = 0; i < amountOfRooms; i++)
                {
                    DecideRandomRooms(i);
                }
                DecidePlayerPosition();
                //Player and enemies
            }
            static void NewMapV4(bool random)
            {
                checkedRooms = 0;
                MapLoadingBar(0, "Starting....");
                MakeMap4Nothing();
                if (random)
                {
                    CreateRandomMapV4(true);
                }
                GenerateRoomPositions();
                //MiddleRoomSides();
                RoomPaths();
                MapLoadingBar(5, "Done!");
                fixMiddle();
                for (int i = 0; i < amountOfRooms; i++)
                {
                    map[roomsPosLengths[i], roomsPosWidths[i]] = "A" + i;
                }
                DrawMap4();
                AwaitMovementKey();
            }
            static void DrawMap4()
            {
                Console.Clear();
                //DrawFullMap4();
                DrawMapDistance();
            }
            static void MapLoadingBar(int length, string message)
            {
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine($"    Map is currently loading");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"          {message}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                Console.WriteLine("    ------------------------");
                Console.Write("    ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write("|");
                if (length >= 1)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                }
                Console.Write("    ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write("|");
                if (length >= 2)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                }
                Console.Write("   ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write("|");
                if (length >= 3)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                }
                Console.Write("    ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write("|");
                if (length >= 4)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                }
                Console.Write("   ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write("|");
                if (length >= 5)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                }
                Console.Write("    ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine("|");
                Console.WriteLine("    ------------------------");
                if (length > 0)
                {
                    if (loaded[length - 1] == false)
                    {
                        //Console.Read();
                        Thread.Sleep(1000);
                    }
                }
                switch (length)
                {
                    case 1:
                        loaded[0] = true;
                        break;
                    case 2:
                        loaded[1] = true;
                        break;
                    case 3:
                        loaded[2] = true;
                        break;
                    case 4:
                        loaded[3] = true;
                        break;
                    case 5:
                        loaded[4] = true;
                        break;
                }
            }
            static void MapSizeV4()
            {
                currentMusic = "loading";
                bool problem = false;
                Console.Clear();
                specificmapbool = false;
                Console.WriteLine($"Map Generation {version}");
                int biggestMap = biggestMapSize - 2;
                Console.WriteLine($"Warning: Max size is {biggestMap}!");
                Console.WriteLine($"Warning: Min size is {smallestMapSize}!");
                Console.WriteLine($"Recommended: {recommendedMapSize[0]}x{recommendedMapSize[1]} with {recommendedMapSize[2]} rooms.");
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
                    Console.WriteLine($"Cannot convert {answerL} to a number.");
                    Console.WriteLine("Press enter to try again");
                    Console.ReadLine();
                    problem = true;
                }
                if (problem == false)
                {
                    if (Int32.Parse(answerL) > biggestMapSize - 2)
                    {
                        Console.Clear();
                        Console.WriteLine("Error 2:");
                        Console.WriteLine("Answer too big");
                        Console.WriteLine("Press enter to try again");
                        Console.ReadLine();
                        problem = true;
                    }
                    else if (Int32.Parse(answerL) < smallestMapSize)
                    {
                        Console.Clear();
                        Console.WriteLine("Error 3:");
                        Console.WriteLine("Answer too small");
                        Console.WriteLine("Press enter to try again");
                        Console.ReadLine();
                        problem = true;
                    }
                    mapLength = Int32.Parse(answerL);
                }
                Console.WriteLine("Map Width");
                string answerW = "32";
                try
                {
                    if (problem == false)
                    {
                        answerW = Console.ReadLine();
                        int result = Int32.Parse(answerW);
                    }
                }
                catch (FormatException)
                {
                    Console.Clear();
                    Console.WriteLine("Error 1:");
                    Console.WriteLine($"Cannot convert {answerW} to a number.");
                    Console.WriteLine("Press enter to try again");
                    Console.ReadLine();
                    problem = true;
                }
                if (problem == false)
                {
                    if (Int32.Parse(answerW) > biggestMapSize - 2)
                    {
                        Console.Clear();
                        Console.WriteLine("Error 2:");
                        Console.WriteLine("Answer too big");
                        Console.WriteLine("Press enter to try again");
                        Console.ReadLine();
                        problem = true;
                    }
                    else if (Int32.Parse(answerW) < smallestMapSize)
                    {
                        Console.Clear();
                        Console.WriteLine("Error 3:");
                        Console.WriteLine("Answer too small");
                        Console.WriteLine("Press enter to try again");
                        Console.ReadLine();
                        problem = true;
                    }
                    mapWidth = Int32.Parse(answerW);
                }
                Console.WriteLine("Amount of Rooms");
                string answerR = "2";
                try
                {
                    if (problem == false)
                    {
                        answerR = Console.ReadLine();
                        int result = Int32.Parse(answerR);
                    }
                }
                catch (FormatException)
                {
                    Console.Clear();
                    Console.WriteLine("Error 1:");
                    Console.WriteLine($"Cannot convert {answerR} to a number.");
                    Console.WriteLine("Press enter to try again");
                    Console.ReadLine();
                    problem = true;
                }
                if (problem == false)
                {
                    if (Int32.Parse(answerR) > maxRooms)
                    {
                        Console.Clear();
                        Console.WriteLine("Error 2:");
                        Console.WriteLine($"Answer too big , {maxRooms} is the maximum.");
                        Console.WriteLine("Press enter to try again");
                        Console.ReadLine();
                        problem = true;
                    }
                    else if (Int32.Parse(answerR) < minRooms)
                    {
                        if (problem == false)
                        {
                            Console.Clear();
                            Console.WriteLine("Error 3:");
                            Console.WriteLine($"Answer too small , {minRooms} is the minimum");
                            Console.WriteLine("Press enter to try again");
                            Console.ReadLine();
                            problem = true;
                        }
                    }
                    amountOfRooms = Int32.Parse(answerR);
                }
                if (mapLength * mapWidth < (((maxRoomSize + 3) * (maxRoomSize + 3)) * amountOfRooms) * 2)
                {
                    Console.Clear();
                    Console.WriteLine("Error 6:");
                    Console.WriteLine("Not enough map space to fit so many rooms!");
                    Console.WriteLine("Press enter to try again");
                    Console.ReadLine();
                    problem = true;
                }
                innerMapWidth = mapWidth - 2;
                innerMapLength = mapLength - 2;
                Console.WriteLine($"{innerMapWidth} Inner W");
                Console.WriteLine($"{innerMapLength} Inner L");
                Console.WriteLine($"{mapWidth} Outer W");
                Console.WriteLine($"{mapLength} Outer L");
                Console.WriteLine($"{amountOfRooms} Rooms");
                if (problem == true)
                {
                    mapLength = 0;
                    mapWidth = 0;
                    amountOfRooms = 0;
                    MapSizeV4();
                }
            }
            static void FullScreenWarning()
            {
                Console.WriteLine("Warning(s): ");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("This game requires Fullscreen to play normally.");
                Console.WriteLine("Not having the game fullscreen can make everything look weird.");
                Console.WriteLine();
                Console.WriteLine("Scrolling up might break things.");
                Console.WriteLine();
                Console.WriteLine("This game is W.I.P and anything may be subject to change");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Press enter to continue.");
                Console.ReadLine();
            }
            static void MainMenu()
            {
                //Console.BackgroundColor = ConsoleColor.(Color);
                //Console.ForegroundColor = ConsoleColor.(Color);
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("======================");
                Console.Write("|");
                if (mainMenuSelected == 1)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                }
                Console.Write("       ");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("Start");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("        ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine("|");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("======================");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("======================");
                Console.Write("|");
                if (mainMenuSelected == 2)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                }
                Console.Write("      ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Options");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("       ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine("| W.I.P");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("======================");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("======================");
                Console.Write("|");
                if (mainMenuSelected == 3)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                }
                Console.Write("      ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Credits");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("       ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine("|");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("======================");
                Console.WriteLine("======================");
                Console.Write("|");
                if (mainMenuSelected == 4)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                }
                Console.Write("       ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Guide");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("        ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine("| W.I.P");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("======================");
                Console.WriteLine("======================");
                Console.Write("|");
                if (mainMenuSelected == 5)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                }
                Console.Write("       ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("Exit");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("         ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine("|");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("======================");

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                selectMenu();
            }
            static void selectMenu()
            {
                ConsoleKey key = Console.ReadKey().Key;
                if (key == ConsoleKey.DownArrow)
                {
                    mainMenuSelected += 1;
                    if (mainMenuSelected == 6)
                    {
                        mainMenuSelected = 1;
                    }
                    MainMenu();
                }
                else if (key == ConsoleKey.UpArrow)
                {
                    mainMenuSelected -= 1;
                    if (mainMenuSelected == 0)
                    {
                        mainMenuSelected = 5;
                    }
                    MainMenu();
                }
                else if (key == ConsoleKey.Enter)
                {
                    switch (mainMenuSelected)
                    {
                        case 1:
                            GameStart();
                            break;
                        case 2:
                            currentMusic = "test";
                            Options();
                            break;
                        case 3:
                            Credits();
                            break;
                        case 4:
                            mainMenuSelected = 1;
                            Guide();
                            break;
                        case 5:
                            Environment.Exit(0);
                            break;
                    }
                }
                else
                {
                    //If key isnt an arrow it restarts
                    selectMenu();
                }
            }
            static void Credits()
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Mike / Out door and Suvival");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Rudo / Blender Man");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Marlon / Brain Damage Gaming / Gastrikshark");
                Console.ForegroundColor = ConsoleColor.White;
                ConsoleKey key = Console.ReadKey().Key;
                if (key == ConsoleKey.Enter)
                {
                    MainMenu();
                }
                else
                {
                    Credits();
                }
            }
            static void Options()
            {
                Console.Clear();
                Console.WriteLine("What about Work In Progress do you not understand");
                Console.ReadLine();
                MainMenu();
            }
            static void Guide()
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("======================");
                Console.Write("|");
                if (mainMenuSelected == 1)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                }
                Console.Write("       ");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("Player");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("       ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine("| W.I.P");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("======================");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("======================");
                Console.Write("|");
                if (mainMenuSelected == 2)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                }
                Console.Write("       ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Enemy");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("        ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine("| W.I.P");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("======================");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("======================");
                Console.Write("|");
                if (mainMenuSelected == 3)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                }
                Console.Write("       ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Weapons");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("      ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine("| W.I.P");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("======================");
                Console.WriteLine("======================");
                Console.Write("|");
                if (mainMenuSelected == 4)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                }
                Console.Write("        ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Map");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("         ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine("| W.I.P");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("======================");
                Console.WriteLine("======================");
                Console.Write("|");
                if (mainMenuSelected == 5)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                }
                Console.Write("       ");
                if (mainMenuSelected == 5)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.Write("Back");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("         ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine("|");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("======================");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                selectGuide();
            }
            static void selectGuide()
            {
                ConsoleKey key = Console.ReadKey().Key;
                if (key == ConsoleKey.DownArrow)
                {
                    mainMenuSelected += 1;
                    if (mainMenuSelected == 6)
                    {
                        mainMenuSelected = 1;
                    }
                    Guide();
                }
                else if (key == ConsoleKey.UpArrow)
                {
                    mainMenuSelected -= 1;
                    if (mainMenuSelected == 0)
                    {
                        mainMenuSelected = 5;
                    }
                    Guide();
                }
                else if (key == ConsoleKey.Enter)
                {
                    switch (mainMenuSelected)
                    {
                        case 1:
                            Options();
                            break;
                        case 2:
                            Options();
                            break;
                        case 3:
                            Options();
                            break;
                        case 4:
                            currentMusic = "main";
                            Tester();
                            break;
                        case 5:
                            mainMenuSelected = 1;
                            MainMenu();
                            break;
                    }
                }
                else
                {
                    //If key isnt an arrow it restarts
                    selectMenu();
                }
            }
            static void Tester()
            {
                Console.Clear();
                Console.WriteLine(" | | ");
                System.Threading.Thread.Sleep(1000);
                Console.Clear();
                Console.WriteLine(" - - ");
                System.Threading.Thread.Sleep(1000);
                Console.Clear();
                Console.WriteLine(" | | ");
                System.Threading.Thread.Sleep(1000);
                Guide();
            }
            static void musicTrack1()
            {
                while (beeps == true)
                {
                    //Console.Title = "C# Rogue Game";
                    switch (currentMusic)
                    {
                        case "main":
                            {
                                if (musicPlaying == false)
                                {
                                    musicPlaying = true;
                                    //Console.WriteLine($"Current music playing: {currentMusic}");
                                    musicPlaying = false;
                                }
                                break;
                            }
                        case "test":
                            {
                                if (musicPlaying == false)
                                {
                                    musicPlaying = true;
                                    //Console.WriteLine($"Current music playing: {currentMusic}");
                                    Console.Beep(200, 900);
                                    Console.Beep(100, 200);
                                    Console.Beep(200, 300);
                                    Console.Beep(100, 300);
                                    musicPlaying = false;
                                }
                                break;

                            }
                    }

                }
            }
            static void musicTrack2()
            {
                while (beeps == true)
                {
                    switch (currentMusic)
                    {
                        case "main":
                            {
                                while (musicPlaying == true)
                                {
                                    Console.Beep(220, 500); // intro
                                    Console.Beep(196, 250);
                                    Console.Beep(165, 250);
                                    Console.Beep(131, 500);

                                    Console.Beep(220, 250); // verse 1
                                    Console.Beep(196, 250);
                                    Console.Beep(165, 250);
                                    Console.Beep(131, 500);

                                    Console.Beep(220, 250); // verse 2
                                    Console.Beep(196, 250);
                                    Console.Beep(165, 250);
                                    Console.Beep(131, 500);

                                    Console.Beep(220, 250); // verse 3
                                    Console.Beep(196, 250);
                                    Console.Beep(165, 250);
                                    Console.Beep(131, 500);

                                    Console.Beep(220, 500); // chorus
                                    Console.Beep(165, 250);
                                    Console.Beep(131, 250);

                                    Console.Beep(220, 250); // bridge
                                    Console.Beep(196, 250);
                                    Console.Beep(165, 250);
                                    Console.Beep(131, 500);

                                    Console.Beep(220, 250); // chorus
                                    Console.Beep(165, 250);
                                    Console.Beep(131, 250);

                                    Console.Beep(220, 500); // outro
                                    Console.Beep(165, 250);
                                    Console.Beep(131, 250);
                                }
                                break;
                            }
                        case "loading":
                            {
                                //Loading Music?
                                Console.Beep(262, 250); // C
                                Console.Beep(294, 250); // D
                                Console.Beep(330, 250); // E
                                Console.Beep(262, 250); // C
                                Console.Beep(262, 250); // C
                                Console.Beep(294, 250); // D
                                Console.Beep(330, 250); // E
                                Console.Beep(262, 250); // C
                                Console.Beep(294, 250); // D
                                Console.Beep(262, 250); // C
                                Console.Beep(262, 250); // C
                                Console.Beep(294, 250); // D
                                Console.Beep(294, 250); // D
                                Console.Beep(262, 250); // C
                                break;
                            }
                        case "looting":
                            {
                                Console.Beep(400, 200); // intro
                                Console.Beep(500, 200);
                                Console.Beep(600, 200);
                                Console.Beep(700, 200);

                                Console.Beep(400, 200); // verse 1
                                Console.Beep(400, 200);
                                Console.Beep(450, 200);
                                Console.Beep(400, 200);
                                Console.Beep(500, 200);
                                Console.Beep(450, 200);
                                Console.Beep(400, 200);
                                Console.Beep(550, 200);
                                Console.Beep(500, 200);
                                Console.Beep(400, 200);
                                Console.Beep(600, 200);

                                Console.Beep(400, 200); // chorus
                                Console.Beep(500, 200);
                                Console.Beep(600, 200);
                                Console.Beep(700, 200);
                                Console.Beep(800, 200);
                                Console.Beep(700, 200);
                                Console.Beep(600, 200);
                                Console.Beep(500, 200);

                                Console.Beep(400, 200); // verse 2
                                Console.Beep(400, 200);
                                Console.Beep(450, 200);
                                Console.Beep(400, 200);
                                Console.Beep(500, 200);
                                Console.Beep(450, 200);
                                Console.Beep(400, 200);
                                Console.Beep(550, 200);
                                Console.Beep(500, 200);
                                Console.Beep(400, 200);
                                Console.Beep(600, 200);

                                Console.Beep(400, 200); // chorus
                                Console.Beep(500, 200);
                                Console.Beep(600, 200);
                                Console.Beep(700, 200);
                                Console.Beep(800, 200);
                                Console.Beep(700, 200);
                                Console.Beep(600, 200);
                                Console.Beep(500, 200);

                                Console.Beep(400, 200); // bridge
                                Console.Beep(500, 200);
                                Console.Beep(600, 200);
                                Console.Beep(700, 200);
                                Console.Beep(800, 200);
                                Console.Beep(700, 200);
                                Console.Beep(600, 200);
                                Console.Beep(500, 200);

                                Console.Beep(400, 200); // chorus
                                Console.Beep(500, 200);
                                Console.Beep(600, 200);
                                Console.Beep(700, 200);
                                Console.Beep(800, 200);
                                Console.Beep(700, 200);
                                Console.Beep(600, 200);
                                Console.Beep(500, 200);

                                Console.Beep(400, 200); // outro
                                Console.Beep(500, 200);
                                Console.Beep(600, 200);
                                Console.Beep(700, 200);
                                break;
                            }
                        case "fighting":
                            {
                                //Fighting
                                Console.Beep(200, 100); // intro
                                Console.Beep(200, 100);
                                Console.Beep(250, 100);
                                Console.Beep(200, 100);
                                Console.Beep(300, 100);
                                Console.Beep(250, 100);
                                Console.Beep(200, 100);
                                Console.Beep(350, 100);
                                Console.Beep(300, 100);
                                Console.Beep(200, 100);
                                Console.Beep(400, 100);

                                Console.Beep(200, 100); // verse 1
                                Console.Beep(200, 100);
                                Console.Beep(250, 100);
                                Console.Beep(200, 100);
                                Console.Beep(300, 100);
                                Console.Beep(250, 100);
                                Console.Beep(200, 100);
                                Console.Beep(350, 100);
                                Console.Beep(300, 100);
                                Console.Beep(200, 100);
                                Console.Beep(400, 100);

                                Console.Beep(500, 100); // chorus
                                Console.Beep(500, 100);
                                Console.Beep(500, 100);
                                Console.Beep(500, 100);
                                Console.Beep(500, 100);
                                Console.Beep(500, 100);
                                Console.Beep(500, 100);
                                Console.Beep(500, 100);

                                Console.Beep(200, 100); // verse 2
                                Console.Beep(200, 100);
                                Console.Beep(250, 100);
                                Console.Beep(200, 100);
                                Console.Beep(300, 100);
                                Console.Beep(250, 100);
                                Console.Beep(200, 100);
                                Console.Beep(350, 100);
                                Console.Beep(300, 100);
                                Console.Beep(200, 100);
                                Console.Beep(400, 100);

                                Console.Beep(500, 100); // chorus
                                Console.Beep(500, 100);
                                Console.Beep(500, 100);
                                Console.Beep(500, 100);
                                Console.Beep(500, 100);
                                Console.Beep(500, 100);
                                Console.Beep(500, 100);
                                Console.Beep(500, 100);

                                Console.Beep(200, 100); // bridge
                                Console.Beep(200, 100);
                                Console.Beep(250, 100);
                                Console.Beep(200, 100);
                                Console.Beep(300, 100);
                                Console.Beep(250, 100);
                                Console.Beep(200, 100);
                                Console.Beep(350, 100);
                                Console.Beep(300, 100);
                                Console.Beep(200, 100);
                                Console.Beep(400, 100);

                                Console.Beep(500, 100); // chorus
                                Console.Beep(500, 100);
                                Console.Beep(500, 100);
                                Console.Beep(500, 100);
                                Console.Beep(500, 100);
                                Console.Beep(500, 100);
                                Console.Beep(500, 100);
                                Console.Beep(500, 100);

                                Console.Beep(200, 100); // outro
                                Console.Beep(200, 100);
                                Console.Beep(250, 100);
                                Console.Beep(300, 100);
                                Console.Beep(350, 100);
                                Console.Beep(400, 100);
                                Console.Beep(450, 100);
                                Console.Beep(500, 100);
                                Console.Beep(500, 100);
                                Console.Beep(550, 100);
                                Console.Beep(550, 100);
                                Console.Beep(600, 100);
                                Console.Beep(600, 100);
                                Console.Beep(650, 100);
                                Console.Beep(650, 100);
                                Console.Beep(700, 100);
                                break;
                            }
                        case "boss":
                            {
                                Console.Beep(80, 500);  // intro
                                Console.Beep(100, 250);
                                Console.Beep(120, 250);

                                Console.Beep(160, 500);  // verse 1
                                Console.Beep(180, 500);
                                Console.Beep(200, 500);
                                Console.Beep(240, 250);
                                Console.Beep(200, 250);
                                Console.Beep(180, 250);

                                Console.Beep(160, 500);  // verse 2
                                Console.Beep(180, 500);
                                Console.Beep(200, 500);
                                Console.Beep(240, 250);
                                Console.Beep(200, 250);
                                Console.Beep(180, 250);

                                Console.Beep(160, 500);  // verse 3
                                Console.Beep(180, 500);
                                Console.Beep(200, 500);
                                Console.Beep(240, 250);
                                Console.Beep(200, 250);
                                Console.Beep(180, 250);

                                Console.Beep(240, 500);  // chorus
                                Console.Beep(320, 250);
                                Console.Beep(240, 250);
                                Console.Beep(320, 250);
                                Console.Beep(240, 500);

                                Console.Beep(240, 500);  // bridge
                                Console.Beep(320, 250);
                                Console.Beep(240, 250);
                                Console.Beep(320, 250);
                                Console.Beep(240, 500);

                                Console.Beep(240, 500);  // chorus
                                Console.Beep(320, 250);
                                Console.Beep(240, 250);
                                Console.Beep(320, 250);
                                Console.Beep(240, 500);

                                Console.Beep(80, 500);  // outro
                                Console.Beep(100, 250);
                                Console.Beep(120, 250);
                                break;
                            }
                        case "test":
                            {
                                if (musicPlaying == false)
                                {
                                    musicPlaying = true;
                                    Console.Beep(200, 900);
                                    Console.Beep(100, 200);
                                    Console.Beep(200, 300);
                                    Console.Beep(100, 300);
                                    musicPlaying = false;
                                }
                                break;
                            }
                    }
                }
            }
            static void musicTrack3()
            {
                while (beeps == true)
                {
                    switch (currentMusic)
                    {
                        case "main":
                            {
                                while (musicPlaying == true)
                                {
                                    /*Console.Beep(400, 500);
                                    Console.Beep(400, 500);*/
                                    System.Threading.Thread.Sleep(1000);
                                }
                                break;
                            }
                        case "test":
                            {
                                if (musicPlaying == false)
                                {
                                    musicPlaying = true;
                                    Console.Beep(200, 900);
                                    Console.Beep(100, 200);
                                    Console.Beep(200, 300);
                                    Console.Beep(100, 300);
                                    musicPlaying = false;
                                }
                                break;
                            }
                    }
                }
            }
        }
    }
}