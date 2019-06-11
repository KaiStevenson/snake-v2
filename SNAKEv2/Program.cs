using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Resources;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;


//classic snake game, vs AI
namespace SNAKEv2
{
    [Serializable]
    class Program
    {
        string[,] board = new string[15, 15];
        List<int[]> headCoords = new List<int[]>();
        string lastMove;

        static void Main(string[] args)
        {
            var prog = new Program();
            prog.BeforeStart();
        }
        void BeforeStart()
        {
            var writeSpeed = 3;
            Maximize();
            WriteSlow("Welcome to glitchSnake()", writeSpeed);
            Console.WriteLine("");
            WriteSlow("It can be hard to keep track of your snake, so we recommend increasing the font size", writeSpeed);
            Console.WriteLine("");
            WriteSlow("To do so, right click the top bar of the game, then select properties and go the font tab. Change size to whatever you want", writeSpeed);
            Console.WriteLine("");
            WriteSlow("After changing the size, restart the game",writeSpeed);
            Console.WriteLine(" ");
            WriteSlow("Instructions: Use WASD to move, collect the S to grow", writeSpeed);
            Console.WriteLine(" ");
            WriteSlow("Press any key to continue", writeSpeed);
            Console.ReadKey();
            Countdown();
            Init();
        }
        void Countdown()
        {
            for (int i = 0; i < 50; i++)
            {
                Console.WriteLine(" ");
            }
            for (int c = 0; c < 6; c++)
            {
                var random = new Random();
                var randint = random.Next(1, 9);
                for (int i = 0; i < randint; i++)
                {
                    Console.Write(c);
                }
                Thread.Sleep(300);
            }
        }
        void Init()
        {
            Maximize();
            Console.CursorVisible = true;
            Console.CursorSize = 100;
            Console.SetBufferSize(500, Int16.MaxValue -1);
            lastMove = "left";
            //populate board
            var headX = 99;
            var headY = 99;
            var c1 = new int[2];
            var c2 = new int[2];
            var c3 = new int[2];
            for (int r = 0; r < board.GetLength(0); r++)
            {
                for (int c = 0; c < board.GetLength(1); c++)
                {
                    if (r == Math.Round(Convert.ToDouble(board.GetLength(0) / 2)) && c == Convert.ToDouble(board.GetLength(1) / 2))
                    {
                        board[r, c] = "O";
                        headX = c;
                        headY = r;
                    }
                    else if (r == headY + 1 && c == headX)
                    {
                        board[r, c] = "X";
                        c1[0] = r;
                        c1[1] = c;
                    }
                    else if (r == headY + 2 && c == headX)
                    {
                        board[r, c] = "X";
                        c2[0] = r;
                        c2[1] = c;
                    }
                    else if (r == headY + 3 && c == headX)
                    {
                        board[r, c] = "X";
                        c3[0] = r;
                        c3[1] = c;
                    }
                    else if (r == 2 && c == 2)
                    {
                        board[r, c] = "S";
                    }
                    else
                    {
                        board[r, c] = "i";
                    }
                }
            }
            headCoords.Add(c1);
            headCoords.Add(c2);
            headCoords.Add(c3);
            Render();
            Move();
        }

        void Move()
        {
            //cache all positions
            //positions are stored as a 1d array, x is index 0, y is index 1
            //tails are stored as an integer defining length, and an array with all positions the head has been

            //the head
            var cachedPosHead = Find("O");
            //the tails
            var tailCount = CountTails();
            //the food
            var cachedPosFood = Find("S");

            //clear board
            ClearBoard();

            //update positions
            var cPos = new int[2];
            cPos[0] = cachedPosHead[0];
            cPos[1] = cachedPosHead[1];
            headCoords.Insert(0, cPos);

            var headDir = GetInput();
            if (headDir == "up")
            {
                cachedPosHead[1] -= 1;
            }
            if (headDir == "down")
            {
                cachedPosHead[1] += 1;
            }
            if (headDir == "left")
            {
                cachedPosHead[0] -= 1;
            }
            if (headDir == "right")
            {
                cachedPosHead[0] += 1;
            }
                       
            //wrap around
            if(cachedPosHead[1] > board.GetLength(1) -1)
            {
                cachedPosHead[1] = 0;
            }
            if (cachedPosHead[0] > board.GetLength(0) - 1)
            {
                cachedPosHead[0] = 0;
            }
            if (cachedPosHead[1] < 0)
            {
                cachedPosHead[1] = board.GetLength(1) - 1;
            }
            if (cachedPosHead[0] < 0)
            {
                cachedPosHead[0] = board.GetLength(0) - 1;
            }

            //check if food eaten
            var foodEaten = false;
            if (cachedPosHead[0] == cachedPosFood[0] && cachedPosHead[1] == cachedPosFood[1])
            {
                var random = new Random();
                foodEaten = true;

                while (true)
                {
                    var testing = false;

                    var xTest = random.Next(1, board.GetLength(0) -1);
                    var ytest = random.Next(1, board.GetLength(1) -1);

                    if(xTest != cachedPosHead[0] && ytest != cachedPosHead[1])
                    {
                        for (int i = 0; i < tailCount; i++)
                        {
                            if (xTest == headCoords[i][0] && ytest == headCoords[i][1])
                            {
                                testing = true;
                            }
                        }
                    }
                    else
                    {
                        testing = true;
                    }

                    if(testing == false)
                    {
                        cachedPosFood[0] = xTest;
                        cachedPosFood[1] = ytest;
                        break;
                    }
                }
            }

            //restore board
            board[cachedPosHead[0], cachedPosHead[1]] = "O";
            board[cachedPosFood[0], cachedPosFood[1]] = "S";

            for (int i = 0; i < tailCount; i++)
            {
                var tailX = headCoords[i][0];
                var tailY = headCoords[i][1];
                board[tailX, tailY] = "X";
            }
            if (foodEaten)
            {
                var tailX = headCoords[tailCount][0];
                var tailY = headCoords[tailCount][1];
                board[tailX, tailY] = "X";
            }
            //go to the next step
            Render();
            Move();

        }
        //type input
        string GetInput()
        {
            DateTime beginWait = DateTime.Now;
            Thread.Sleep(150);
            if (!Console.KeyAvailable)
            {
                return lastMove;
            }
            else
            {
                var keyinfo = System.Console.ReadKey();

                if (keyinfo.Key == System.ConsoleKey.W)
                {
                    lastMove = "up";
                    return "up";
                }
                else if (keyinfo.Key == System.ConsoleKey.S)
                {
                    lastMove = "down";
                    return "down";
                }
                else if (keyinfo.Key == System.ConsoleKey.A)
                {
                    lastMove = "left";
                    return "left";
                }
                else if (keyinfo.Key == System.ConsoleKey.D)
                {
                    lastMove = "right";
                    return "right";
                }
                else
                {
                    return lastMove;
                }
            }
        }
        //draw the board
        void Render()
        {
            Console.SetCursorPosition(Console.CursorLeft, 50);
            for (int r = 0; r < board.GetLength(1); r++)
            {
                var random = new Random();
                var randChance = random.Next(0, 12);
                if(randChance == 1)
                {
                    Console.Write(" ");
                }
                if(randChance == 7)
                {
                    Console.Write(" ");
                    Console.Write(" ");
                }
                for (int c = 0; c < board.GetLength(0); c++)
                {
                    if (board[c, r] == "X")
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("X");
                        Console.ResetColor();
                    }
                    else if (board[c, r] == "O")
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write("O");
                        Console.ResetColor();
                    }
                    else if (board[c, r] == "S")
                    {
                        if(randChance > 4)
                        {
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        Console.Write("S");
                        Console.ResetColor();
                    }
                    else
                    {
                        var randInt = random.Next(0, 100);
                        if(randInt > 97 && board[c,r] == "i")
                        {
                            board[c, r] = "t";
                        }
                        Console.Write(board[c, r]);
                    }
                }
                Console.SetCursorPosition(0, Console.CursorTop+1);
            }
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop+1);
            Console.WriteLine("EXT_" + CountTails());
        }
        //returns the position of a given character !WILL ONLY RETURN ONE POSITION!
        int[] Find(string searchChar)
        {
            for (int r = 0; r < board.GetLength(0); r++)
            {
                for (int c = 0; c < board.GetLength(1); c++)
                {
                    if (board[r, c] == searchChar)
                    {
                        var pos = new int[2];
                        pos[0] = r;
                        pos[1] = c;
                        return pos;
                    }
                }
            }
            return null;
        }

        void ClearBoard()
        {
            for (int r = 0; r < board.GetLength(0); r++)
            {
                for (int c = 0; c < board.GetLength(1); c++)
                {
                    board[r, c] = "i";
                }
            }
        }
        //returns the number of tails on the board
        int CountTails()
        {
            int tCount = 0;
            for (int r = 0; r < board.GetLength(0); r++)
            {
                for (int c = 0; c < board.GetLength(1); c++)
                {
                    if (board[r, c] == "X")
                    {
                        tCount++;
                    }
                }
            }
            return tCount;
        }
        //writes a string to the console, pausing after each character
        void WriteSlow(string targetString, int delay)
        {
            for (int i = 0; i < targetString.Length; i++)
            {
                Console.Write(targetString[i]);
                Thread.Sleep(delay);
            }
            Console.WriteLine(" ");
        }

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(System.IntPtr hWnd, int cmdShow);

        private static void Maximize()
        {
            Process p = Process.GetCurrentProcess();
            ShowWindow(p.MainWindowHandle, 3); //SW_MAXIMIZE = 3
        }

    }
}
