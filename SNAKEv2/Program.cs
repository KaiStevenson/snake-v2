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
            prog.Init();
        }
        void Init()
        {
            Console.CursorVisible = true;
            Console.CursorSize = 100;
            Console.SetBufferSize(100, Int16.MaxValue -1);
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
            var toAdd = new int[2];
            toAdd[0] = cachedPosHead[0];
            toAdd[1] = cachedPosHead[1];

            headCoords.Insert(0, toAdd);

            var headDir = GetInput3();

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
        string GetInput3()
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
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop + 15);
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
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.Write("S");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write(board[c, r]);
                    }
                }
                Console.SetCursorPosition(0, Console.CursorTop + 1);
            }
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
    }
}
