using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Formats.Asn1;
using Microsoft.VisualBasic.FileIO;

namespace RogueAttemptMaybe
{
    //Hey
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
    //Classes *       Mi

    //Enemies: 
    //Movement R
    //HP *      Ma
    //Attack * R
    // Enemy types Mi
    internal class Program
    {
        //main menu
        static string name;
        static bool characterHasBeenMade = false;

        //Characters
        static string floorCharacter = "* ";
        static string character = "@ ";
        static string enemy1 = "E ";
        //Map sizes
        static int mapSizeL = 8;
        static int[] currentPlayerPosition = { 0, 0 };
        static int[] currentEnemyPosition = { 0, 0 };
        //static int mapSizeW = 8;
        static int innerMapSizeL = 6;
        static int innerMapSizeW = 6;
        //Attack
        static string[] weapons = File.ReadAllLines("Weapons.txt");
        static List<float> weaponDmg = new List<float>();
        static List<float> weaponCritChance = new List<float>();
        static List<float> weaponCritMulti = new List<float>();
        static List<string> weaponName = new List<string>();
        static int selectedWeapon;
        static string currentWeapon;
        static float currentDmg;
        static float currentCrit;
        static float currentCritMulti;
        static float testPlayerAttack = 10;
        static float testEnemyAttack = 5;
        static float critChance = 50;
        static float critMultiplier = 2.5f;
        static float playerHp = 100;
        static float enemyHp = 100;
        static bool enemyAlive = true;
        //Input map here
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
        static void Main(string[] args)
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                string[] data = weapons[i].Split(',', StringSplitOptions.RemoveEmptyEntries);
                weaponName.Add(data[0]) ;
                weaponDmg.Add(float.Parse(data[1]));
                weaponCritChance.Add(float.Parse(data[2]));
                weaponCritMulti.Add(float.Parse(data[3]));
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
                }
            }
            if (characterHasBeenMade == true)
            {
            //Takes a random spawn for the player
            Random rpos1 = new Random();
            int pos1 = rpos1.Next(1,innerMapSizeL);
            Random rpos2 = new Random();
            int pos2 = rpos2.Next(1,innerMapSizeW);
            currentPlayerPosition[0] = pos1;
            currentPlayerPosition[1] = pos2;
            map1[currentPlayerPosition[0], currentPlayerPosition[1]] = character;
            //Takes a random spawn for 1 enemy
            Random rpos3 = new Random();
            int pos3 = rpos3.Next(1, innerMapSizeL);
            Random rpos4 = new Random();
            int pos4 = rpos4.Next(1, innerMapSizeW);
            currentEnemyPosition[0] = pos3;
            currentEnemyPosition[1] = pos4;
            map1[currentEnemyPosition[0], currentEnemyPosition[1]] = enemy1;
            //Starts game
            DrawMap1();
            AwaitMovementKey();
            }
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
                            DrawMap1();
                        AwaitMovementKey();
                    }
                    else
                    {
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
                        DrawMap1();
                        AwaitMovementKey();
                    }
                    else
                    {
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
                        DrawMap1();
                        AwaitMovementKey();
                    }
                    else
                    {
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
                        DrawMap1();
                        AwaitMovementKey();
                    }
                    else
                    {
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
            CheckIfAttack();
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
                        }
                        else
                        {
                            W = 0;
                        }
                    }
                }
            }
        }
        static void CheckIfAttack()
        {
            if (currentPlayerPosition[0]== currentEnemyPosition[0] -1 || currentPlayerPosition[0] == currentEnemyPosition[0] +1 || currentPlayerPosition[1]== currentEnemyPosition[1] -1 || currentPlayerPosition[1] == currentEnemyPosition[1]+1)
            {
                if (map1[currentEnemyPosition[0] - 1, currentEnemyPosition[1]] == character || map1[currentEnemyPosition[0] + 1, currentEnemyPosition[1]] == character|| map1[currentEnemyPosition[0], currentEnemyPosition[1] - 1] == character|| map1[currentEnemyPosition[0], currentEnemyPosition[1] + 1] == character)
                {
                    playerHp = playerHp - testEnemyAttack;
                    Console.WriteLine(playerHp);
                    Random i = new Random();
                    float i2 = i.Next(0 , 100);
                    if (i2 <= currentCrit)
                    {
                        Console.WriteLine("Crit");
                        enemyHp = enemyHp - (currentDmg * currentCritMulti);
                        Console.WriteLine(enemyHp);
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
            for (int i = 0; i < weaponName.Count; i++)
            {
                Console.Write(weaponName[i] + " ");
                Console.Write("Dmg:" + weaponDmg[i] + " ");
                Console.Write("CC:" + weaponCritChance[i] + " ");
                Console.WriteLine("CMul:" + weaponCritMulti[i]);
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
                for (int i = 0; i < weaponName.Count; i++)
                {
                    Console.Write(weaponName[i] + " ");
                    Console.Write("Dmg:" + weaponDmg[i] + " ");
                    Console.Write("CC:" + weaponCritChance[i] + " ");
                    Console.WriteLine("CMul:" + weaponCritMulti[i]);
                    Console.WriteLine("-----------------------------------------------------------");
                }
            }
                WeaponSelect(select);
        }
        static void WeaponSelect(ConsoleKey select) 
        {
            switch(select) 
            {
                case ConsoleKey.UpArrow: 
                {
                        if(selectedWeapon < weapons.Length) 
                        {
                            selectedWeapon = selectedWeapon + 1;
                            Console.WriteLine("Current weapon:" + weaponName[selectedWeapon]);
                        } else if (selectedWeapon == weapons.Length - 1) 
                        {
                            selectedWeapon = 0;
                            Console.WriteLine("Current weapon:" + weaponName[selectedWeapon]);
                        }
                        break;
                }
                case ConsoleKey.DownArrow: 
                {
                        if(selectedWeapon > 0) 
                        {
                            selectedWeapon = selectedWeapon - 1;
                            Console.WriteLine("Current weapon:" + weaponName[selectedWeapon]);
                        } else 
                        {
                            selectedWeapon = weapons.Length - 1;
                            Console.WriteLine("Current weapon:" + weaponName[selectedWeapon]);
                        }
                        break;
                }
            }
        }
        static void DrawMap1()
        {
            //Clears the console so you dont see the previous stuff.
            //Console.Clear();
            //Draws the map
            int total = 0;
            int length = 0;
            for (int width = 0; total < map1.Length; width++)
            {
                total++;
                if (width == mapSizeL)
                {
                    //go to next map line
                    Console.WriteLine();
                    width = 0;
                    length++;
                }
                Console.Write(map1[length, width]);
            }
            Console.WriteLine();
        }
    }
}