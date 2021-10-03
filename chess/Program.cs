using System;
using System.Collections.Generic;
using System.IO;

namespace chess
{
    class Program
    {
        public static List<string> memory;
        public static int indexOfMemory;
        public static List<string> coordMemory;
        static void Main(string[] args)
        {
            string[,] board = new string[8, 8];
            Reset(ref board);
            bool[] castlingRights = new bool[4]; for (int i = 0; i < 4; i++) { castlingRights[i] = true; }
            bool whitePOV = true;
            bool whiteToMove = true;
            bool ongoing = true;
            string userInput = null;
            bool drawAccepted = false;
            int noTakes = 100;

            memory = new List<string>();
            indexOfMemory = -1;
            coordMemory = new List<string>();

            while (ongoing)
            {
                DrawUI(whitePOV, ongoing);
                Console.ForegroundColor = ConsoleColor.White;
                userInput = Console.ReadLine();
                LegalityCheck(ref board, ref whiteToMove, userInput, ref castlingRights, ref ongoing, ref noTakes, ref whitePOV, ref drawAccepted);
            }

            indexOfMemory = memory.Count - 1;
            DrawUI(whitePOV, ongoing);

            int consoleIndex = 0;
            for (int i = 0; i < memory.Count; i++)
            {
                consoleIndex = consoleIndex + memory[i].Length;
                if (consoleIndex > Console.WindowWidth)
                {
                    Console.WriteLine();
                    consoleIndex = memory[i].Length;
                }
                Console.Write(memory[i] + " ");
            }

            if ((!userInput.Contains("#") && userInput != "resign") || drawAccepted)
            {
                if (consoleIndex + 7 > Console.WindowWidth) // 7 = string.Lenght
                {
                    Console.WriteLine();
                }
                Console.WriteLine("1/2-1/2");
            }
            else if (whiteToMove)
            {
                if (consoleIndex + 3 > Console.WindowWidth) // 3 = string.Length
                {
                    Console.WriteLine();
                }
                Console.WriteLine("0-1");
            }
            else
            {
                if (consoleIndex + 3 > Console.WindowWidth) // 3 = string.Length
                {
                    Console.WriteLine();
                }
                Console.WriteLine("1-0");
            }

            Console.ReadKey();
        }

        static void DrawUI(bool whitePOV, bool ongoing)
        {
            Console.Clear();
            string[,] boardCopy = new string[8, 8];
            Reset(ref boardCopy);
            bool whiteToMoveCopy = true;
            bool[] castlingRightsCopy = new bool[4]; for (int i = 0; i < 4; i++) { castlingRightsCopy[i] = true; }
            for (int i = 0; i < indexOfMemory + 1 && i < coordMemory.Count; i++)
            {
                MakeMove(ref boardCopy, ref castlingRightsCopy, coordMemory[i], whiteToMoveCopy, true);
                if (whiteToMoveCopy)
                {
                    whiteToMoveCopy = false;
                }
                else
                {
                    whiteToMoveCopy = true;
                }
            }

            int forStartValue = 7;
            int forEndValue = -1;
            int forChangeValue = -1;
            if (whitePOV)
            {
                forStartValue = 0;
                forEndValue = 8;
                forChangeValue = 1;
            }
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("      ");
            for (int i = forStartValue; (whitePOV && i < forEndValue) || (!whitePOV && i > forEndValue); i = i + forChangeValue)
            {
                if (whitePOV)
                {
                    if (i < 7)
                    {
                        Console.Write(Convert.ToChar(i + 97) + "        ");
                    }
                    else
                    {
                        Console.Write(Convert.ToChar(i + 97) + "      ");
                    }
                }
                else
                {
                    if (i > 0)
                    {
                        Console.Write(Convert.ToChar(i + 97) + "        ");
                    }
                    else
                    {
                        Console.Write(Convert.ToChar(i + 97) + "      ");
                    }
                }

            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine(" ");
            for (int i = forStartValue; (whitePOV && i < forEndValue) || (!whitePOV && i > forEndValue); i = i + forChangeValue)
            {
                for (int h = 0; h < 4; h++)
                {
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.ForegroundColor = ConsoleColor.White;
                    if (h != 2)
                    {
                        Console.Write("  ");
                    }
                    else
                    {
                        Console.Write(8 - i + " ");
                    }
                    for (int j = forStartValue; (whitePOV && j < forEndValue) || (!whitePOV && j > forEndValue); j = j + forChangeValue)
                    {
                        if ((i % 2 == 0 && j % 2 == 0) || (i % 2 != 0 && j % 2 != 0))
                        {
                            Console.BackgroundColor = ConsoleColor.Gray;
                        }
                        else
                        {
                            Console.BackgroundColor = ConsoleColor.Black;
                        }
                        if (boardCopy[i, j] == null)
                        {
                            Console.Write("         ");
                        }
                        else if (boardCopy[i, j].Contains("ep"))
                        {
                            Console.Write("         ");
                        }
                        else
                        {
                            StreamReader sr = new StreamReader("sprites.txt");
                            bool pawn = true;
                            string readline = sr.ReadLine();
                            while (readline != "p")
                            {
                                readline = sr.ReadLine(); // a while feltétele ne léptesse
                                if (boardCopy[i, j].Contains(readline))
                                {
                                    if (boardCopy[i, j].Contains("+"))
                                    {
                                        Console.ForegroundColor = ConsoleColor.White;
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkGray;
                                    }
                                    for (int k = 0; k < h; k++)
                                    {
                                        sr.ReadLine();
                                    }
                                    Console.Write(sr.ReadLine());
                                    pawn = false;
                                    break;
                                }
                            }
                            if (pawn)
                            {
                                if (boardCopy[i, j].Contains("+"))
                                {
                                    Console.ForegroundColor = ConsoleColor.White;
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                }
                                for (int k = 0; k < h; k++)
                                {
                                    sr.ReadLine();
                                }
                                Console.Write(sr.ReadLine());
                            }
                            sr.Close();
                        }
                    }
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.ForegroundColor = ConsoleColor.White;
                    if (h != 2)
                    {
                        Console.Write("  ");
                    }
                    else
                    {
                        Console.Write(" " + (8 - i));
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.WriteLine(" ");
                }
            }
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("      ");
            for (int i = forStartValue; (whitePOV && i < forEndValue) || (!whitePOV && i > forEndValue); i = i + forChangeValue)
            {
                if (whitePOV)
                {
                    if (i < 7)
                    {
                        Console.Write(Convert.ToChar(i + 97) + "        ");
                    }
                    else
                    {
                        Console.Write(Convert.ToChar(i + 97) + "      ");
                    }
                }
                else
                {
                    if (i > 0)
                    {
                        Console.Write(Convert.ToChar(i + 97) + "        ");
                    }
                    else
                    {
                        Console.Write(Convert.ToChar(i + 97) + "      ");
                    }
                }
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine(" ");
            Console.ForegroundColor = ConsoleColor.White;

            if (ongoing)
            {
                int numOfMovesDisplayed = 11;
                if (memory.Count < numOfMovesDisplayed + 1)
                {
                    for (int i = 0; i < memory.Count; i++)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        if (i == indexOfMemory)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        }
                        Console.Write(memory[i] + " ");
                    }
                }
                else if (indexOfMemory > (numOfMovesDisplayed - 1) / 2)
                {
                    Console.Write("... ");
                    int numOfMovesAfter = (memory.Count - 1) - indexOfMemory;
                    bool toBeContinued = false;
                    if (numOfMovesAfter > (numOfMovesDisplayed - 1) / 2)
                    {
                        numOfMovesAfter = (numOfMovesDisplayed - 1) / 2;
                        toBeContinued = true;
                    }
                    for (int i = 0; i < 10 - numOfMovesAfter; i++)
                    {
                        Console.Write(memory[indexOfMemory - (10 - numOfMovesAfter) + i] + " ");
                    }
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(memory[indexOfMemory] + " ");
                    Console.ForegroundColor = ConsoleColor.White;
                    for (int i = 1; i < numOfMovesAfter + 1; i++)
                    {
                        Console.Write(memory[indexOfMemory + i] + " ");
                    }
                    if (toBeContinued)
                    {
                        Console.Write("... ");
                    }
                }
                else
                {
                    for (int i = 0; i < indexOfMemory; i++)
                    {
                        Console.Write(memory[i] + " ");
                    }
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(memory[indexOfMemory] + " ");
                    Console.ForegroundColor = ConsoleColor.White;
                    for (int i = 1; i < numOfMovesDisplayed - indexOfMemory - 1; i++)
                    {
                        Console.Write(memory[indexOfMemory + i] + " ");
                    }
                    Console.Write("... ");
                }
            }
        }

        public static List<string> legalMove;
        public static List<string> moveCoords;
        static void LegalityCheck(ref string[,] board, ref bool whiteToMove, string userInput, ref bool[] castlingRights, ref bool ongoing, ref int noTakes, ref bool whitePOV, ref bool drawAccepted)
        {
            List<string> pieceToMove = new List<string>();
            legalMove = new List<string>();
            moveCoords = new List<string>();
            string whiteToMoveString;
            if (whiteToMove)
            {
                whiteToMoveString = "+";
            }
            else
            {
                whiteToMoveString = "-";
            }
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] != null)
                    {
                        if (board[i, j].Contains("ep" + whiteToMoveString))
                        {
                            board[i, j] = null;
                        }
                        else if (board[i, j].Contains(whiteToMoveString))
                        {
                            pieceToMove.Add(board[i, j]);
                        }
                    }
                }
            }
            Function addmove = new AddMove();
            PieceAlg(addmove, board, whiteToMove, pieceToMove, castlingRights);            

            List<string> duplicate = new List<string>();
            List<string> coordfix = new List<string>(); // ha ez nincs összekeveredik a koordináta-név index hasonlóság
            for (int i = 0; i < legalMove.Count - 1; i++)
            {
                if (legalMove[i][0] == 'Q' || legalMove[i][0] == 'R' || legalMove[i][0] == 'B' || legalMove[i][0] == 'N')
                {
                    bool found = false;
                    string iCompare = legalMove[i];
                    if (legalMove[i][legalMove[i].Length - 1] == '#' || legalMove[i][legalMove[i].Length - 1] == '+')
                    {
                        iCompare = legalMove[i].Split(legalMove[i][legalMove[i].Length - 1])[0];
                    }
                    for (int j = i + 1; j < legalMove.Count; j++)
                    {
                        string jCompare = legalMove[j];
                        if (legalMove[j][legalMove[j].Length - 1] == '#' || legalMove[j][legalMove[j].Length - 1] == '+')
                        {
                            jCompare = legalMove[j].Split(legalMove[j][legalMove[j].Length - 1])[0];
                        }
                        if ("" + iCompare[0] + iCompare[iCompare.Length - 2] + iCompare[iCompare.Length - 1] == "" + jCompare[0] + jCompare[jCompare.Length - 2] + jCompare[jCompare.Length - 1])
                        {
                            duplicate.Add(legalMove[j]);
                            legalMove.RemoveAt(j);
                            coordfix.Add(moveCoords[j]);
                            moveCoords.RemoveAt(j);
                            found = true;
                        }
                    }
                    if (found)
                    {
                        duplicate.Add(legalMove[i]);
                        legalMove.RemoveAt(i);
                        coordfix.Add(moveCoords[i]);
                        moveCoords.RemoveAt(i);
                        i = 0;
                    }
                }
            }
            for (int i = 0; i < legalMove.Count; i++)
            {
                if (legalMove[i][0] == 'Q' || legalMove[i][0] == 'R' || legalMove[i][0] == 'B' || legalMove[i][0] == 'N')
                {
                    legalMove[i] = legalMove[i].Replace(legalMove[i][1].ToString() + legalMove[i][2].ToString(), null);
                }
            }
            for (int i = 0; i < duplicate.Count; i++)
            {
                legalMove.Add(duplicate[i]);
                moveCoords.Add(coordfix[i]);
            }

            for (int i = 0; i < legalMove.Count; i++)
            {
                bool[] castlingCopy = new bool[4];
                for (int j = 0; j < 4; j++)
                {
                    castlingCopy[j] = castlingRights[j];
                }
                string[,] testBoard = new string[8, 8];
                for (int h = 0; h < 8; h++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        testBoard[h, j] = board[h, j];
                    }
                }
                MakeMove(ref testBoard, ref castlingCopy, moveCoords[i], whiteToMove, false);
                if (!KingIsSafe(testBoard, !whiteToMove, 0, 0, 0, 0))
                {
                    legalMove[i] = legalMove[i] + "+";
                    if (!MoveExists(testBoard, !whiteToMove, castlingCopy))
                    {
                        legalMove[i] = legalMove[i].Replace('+', '#');
                    }
                }
            }

            if (userInput == "swap")
            {
                if (whitePOV)
                {
                    whitePOV = false;
                }
                else
                {
                    whitePOV = true;
                }
            }
            else if (userInput == "+" && indexOfMemory < memory.Count - 1)
            {
                indexOfMemory++;
            }
            else if (userInput == "-" && indexOfMemory > 0)
            {
                indexOfMemory--;
            }
            else if (userInput == "resign")
            {
                memory.Add("resigns");
                ongoing = false;
            }
            else if (userInput == "draw")
            {
                if (RepetitionCount(board) < 3)
                {
                    while (userInput != "accept" && userInput != "decline")
                    {
                        userInput = Console.ReadLine();
                    }
                    if (userInput == "accept")
                    {
                        drawAccepted = true;
                        ongoing = false;
                    }
                }
                else
                {
                    drawAccepted = true;
                    ongoing = false;
                }
            }
            if (RepetitionCount(board) == 5)
            {
                drawAccepted = true;
                ongoing = false;
            }
            else
            {                
                if (legalMove.Count == 0)
                {
                    ongoing = false;
                }
                else if (legalMove.Contains(userInput))
                {
                    if (userInput.Contains("#"))
                    {
                        ongoing = false;
                    }
                    noTakes--;
                    if (userInput.Contains("x"))
                    {
                        noTakes = 100;
                    }
                    memory.Add(userInput);
                    coordMemory.Add(moveCoords[legalMove.IndexOf(userInput)]);
                    indexOfMemory = memory.Count - 1;

                    MakeMove(ref board, ref castlingRights, moveCoords[legalMove.IndexOf(userInput)], whiteToMove, false);
                    if (whiteToMove)
                    {
                        whiteToMove = false;
                    }
                    else
                    {
                        whiteToMove = true;
                    }
                    if (noTakes == 0)
                    {
                        ongoing = false;
                    }
                }
            }

        }

        public static bool exists;
        static bool MoveExists(string[,] board, bool whiteToMove, bool[] castlingRights)
        {
            exists = false;
            List<string> pieceToMove = new List<string>();
            string whiteToMoveString;
            if (whiteToMove)
            {
                whiteToMoveString = "+";
            }
            else
            {
                whiteToMoveString = "-";
            }
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] != null)
                    {
                        if (board[i, j].Contains(whiteToMoveString))
                        {
                            pieceToMove.Add(board[i, j]);
                        }
                        if (board[i, j].Contains("ep" + whiteToMoveString))
                        {
                            board[i, j] = null;
                        }
                    }
                }
            }
            Function exist = new Exists();
            PieceAlg(exist, board, whiteToMove, pieceToMove, castlingRights);

            return exists;

        }

        public static bool safe;
        static bool KingIsSafe(string[,] board, bool whiteToMove, int X, int Y, int DeltaX, int DeltaY)
        {
            safe = true;
            string[,] boardCopy = new string[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    boardCopy[i, j] = board[i, j];
                }
            }
            if (X != 0 || Y != 0 || DeltaY != 0 || Y != 0)
            {
                boardCopy[X + DeltaX, Y + DeltaY] = boardCopy[X, Y];
                boardCopy[X, Y] = null;
            }
            if (whiteToMove)
            {
                whiteToMove = false;
            }
            else
            {
                whiteToMove = true;
            }

            List<string> pieceToMove = new List<string>();
            string whiteToMoveString;
            if (whiteToMove)
            {
                whiteToMoveString = "+";
            }
            else
            {
                whiteToMoveString = "-";
            }
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (boardCopy[i, j] != null)
                    {
                        if (boardCopy[i, j].Contains(whiteToMoveString) && !boardCopy[i, j].Contains("ep"))
                        {
                            pieceToMove.Add(boardCopy[i, j]);
                        }
                    }
                }
            }
            Function contains = new ContainsKing();
            PieceAlg(contains, boardCopy, whiteToMove, pieceToMove, null);

            return safe;
        }

        static void MakeMove(ref string[,] board, ref bool[] castlingRights, string move, bool whiteToMove, bool fromMemory)
        {            
            if (move.Contains("Castle"))
            {
                int castleX = 0;
                if (whiteToMove)
                {
                    castleX = 7;
                }

                if (move.Contains("short"))
                {
                    board[castleX, 6] = board[castleX, 4];
                    board[castleX, 4] = null;
                    board[castleX, 5] = board[castleX, 7];
                    board[castleX, 7] = null;
                }
                else
                {
                    board[castleX, 2] = board[castleX, 4];
                    board[castleX, 4] = null;
                    board[castleX, 3] = board[castleX, 0];
                    board[castleX, 0] = null;
                }

                if (whiteToMove)
                {
                    castlingRights[0] = false; castlingRights[1] = false;
                }
                else
                {
                    castlingRights[2] = false; castlingRights[3] = false;
                }
            }
            else if (move[0] == 'Q' || move[0] == 'R' || move[0] == 'B' || move[0] == 'N')
            {
                board[Convert.ToInt32(move[3].ToString()), Convert.ToInt32(move[4].ToString())] = move[0] + board[Convert.ToInt32(move[1].ToString()), Convert.ToInt32(move[2].ToString())];
                board[Convert.ToInt32(move[1].ToString()), Convert.ToInt32(move[2].ToString())] = null;
            }
            else
            {
                if ((!board[Convert.ToInt32(move[0].ToString()), Convert.ToInt32(move[1].ToString())].Contains("K") &&
                    !board[Convert.ToInt32(move[0].ToString()), Convert.ToInt32(move[1].ToString())].Contains("Q") &&
                    !board[Convert.ToInt32(move[0].ToString()), Convert.ToInt32(move[1].ToString())].Contains("R") &&
                    !board[Convert.ToInt32(move[0].ToString()), Convert.ToInt32(move[1].ToString())].Contains("B") &&
                    !board[Convert.ToInt32(move[0].ToString()), Convert.ToInt32(move[1].ToString())].Contains("N")) && move[1] == move[3])
                {
                    if (!fromMemory)
                    {
                        if (whiteToMove)
                        {
                            board[Convert.ToInt32(move[0].ToString()) - 1, Convert.ToInt32(move[1].ToString())] = "ep+";
                        }
                        else
                        {
                            board[Convert.ToInt32(move[0].ToString()) + 1, Convert.ToInt32(move[1].ToString())] = "ep-";
                        }
                    }                   
                }
                if (board[Convert.ToInt32(move[2].ToString()), Convert.ToInt32(move[3].ToString())] != null)
                {
                    if (board[Convert.ToInt32(move[2].ToString()), Convert.ToInt32(move[3].ToString())].Contains("ep"))
                    {
                        if (whiteToMove && board[Convert.ToInt32(move[2].ToString()) + 1, Convert.ToInt32(move[3].ToString())].Contains("-"))
                        {
                            board[Convert.ToInt32(move[2].ToString()) + 1, Convert.ToInt32(move[3].ToString())] = null;
                        }
                        else
                        {
                            board[Convert.ToInt32(move[2].ToString()) - 1, Convert.ToInt32(move[3].ToString())] = null;
                        }
                    }
                }
                board[Convert.ToInt32(move[2].ToString()), Convert.ToInt32(move[3].ToString())] = board[Convert.ToInt32(move[0].ToString()), Convert.ToInt32(move[1].ToString())];
                board[Convert.ToInt32(move[0].ToString()), Convert.ToInt32(move[1].ToString())] = null;
                if (((castlingRights[0] || castlingRights[1]) && whiteToMove) || ((castlingRights[2] || castlingRights[3]) && !whiteToMove))
                {
                    if (fromMemory)
                    {
                        if (memory[coordMemory.IndexOf(move)].Contains("K"))
                        {
                            if (whiteToMove)
                            {
                                castlingRights[0] = false; castlingRights[1] = false;
                            }
                            else
                            {
                                castlingRights[2] = false; castlingRights[3] = false;
                            }
                        }
                        else if (memory[coordMemory.IndexOf(move)].Contains("R"))
                        {
                            if (move[1] == '0')
                            {
                                if (whiteToMove)
                                {
                                    castlingRights[0] = false;
                                }
                                else
                                {
                                    castlingRights[2] = false;
                                }
                            }
                            else if (move[1] == '7')
                            {
                                if (whiteToMove)
                                {
                                    castlingRights[1] = false;
                                }
                                else
                                {
                                    castlingRights[3] = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (legalMove[moveCoords.IndexOf(move)].Contains("K"))
                        {
                            if (whiteToMove)
                            {
                                castlingRights[0] = false; castlingRights[1] = false;
                            }
                            else
                            {
                                castlingRights[2] = false; castlingRights[3] = false;
                            }
                        }
                        else if (legalMove[moveCoords.IndexOf(move)].Contains("R"))
                        {
                            if (move[1] == '0')
                            {
                                if (whiteToMove)
                                {
                                    castlingRights[0] = false;
                                }
                                else
                                {
                                    castlingRights[2] = false;
                                }
                            }
                            else if (move[1] == '7')
                            {
                                if (whiteToMove)
                                {
                                    castlingRights[1] = false;
                                }
                                else
                                {
                                    castlingRights[3] = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        static void Reset(ref string[,] board)
        {
            board[0, 0] = "R-"; board[0, 7] = "R-'";
            board[0, 1] = "N-"; board[0, 6] = "N-'";
            board[0, 2] = "B-"; board[0, 5] = "B-'";
            board[0, 3] = "Q-"; board[0, 4] = "K-";
            for (int i = 0; i < 8; i++)
            {
                board[1, i] = "-" + i;
            }
            board[7, 0] = "R+"; board[7, 7] = "R+'";
            board[7, 1] = "N+"; board[7, 6] = "N+'";
            board[7, 2] = "B+"; board[7, 5] = "B+'";
            board[7, 3] = "Q+"; board[7, 4] = "K+";
            for (int i = 0; i < 8; i++)
            {
                board[6, i] = "+" + i;
            }
        }

        static void PieceAlg(Function function, string[,] board, bool whiteToMove, List<string> pieceToMove, bool[] castlingRights)
        {
            string whiteToMoveString = "-";
            if (whiteToMove)
            {
                whiteToMoveString = "+";
            }
            int x = 0;
            int y = 0;
            for (int i = 0; i < pieceToMove.Count; i++)
            {
                if (pieceToMove[i].Contains('K' + whiteToMoveString))
                {
                    for (int row = 0; row < 8; row++)
                    {
                        for (int collumn = 0; collumn < 8; collumn++)
                        {
                            if (board[row, collumn] == pieceToMove[i])
                            {
                                x = row;
                                y = collumn;
                            }
                        }
                    }
                    for (int deltaX = -1; deltaX < 2; deltaX++)
                    {
                        for (int deltaY = -1; deltaY < 2; deltaY++)
                        {
                            if (x + deltaX > -1 && x + deltaX < 8 && y + deltaY > -1 && y + deltaY < 8)
                            {
                                if (board[x + deltaX, y + deltaY] == null)
                                {
                                    function.Run(x, y, deltaX, deltaY, pieceToMove[i], whiteToMove, board);
                                }
                                else if (board[x + deltaX, y + deltaY].Contains("ep"))
                                {
                                    function.Run(x, y, deltaX, deltaY, pieceToMove[i], whiteToMove, board);
                                }
                                else if (whiteToMove && board[x + deltaX, y + deltaY].Contains("-") || whiteToMove == false && board[x + deltaX, y + deltaY].Contains("+"))
                                {
                                    function.Run(x, y, deltaX, deltaY, pieceToMove[i], whiteToMove, board);
                                }
                            }
                        }
                    }
                }
                else if (pieceToMove[i].Contains('Q' + whiteToMoveString) || pieceToMove[i].Contains('R' + whiteToMoveString) || pieceToMove[i].Contains('B' + whiteToMoveString))
                {
                    for (int row = 0; row < 8; row++)
                    {
                        for (int collumn = 0; collumn < 8; collumn++)
                        {
                            if (board[row, collumn] == pieceToMove[i])
                            {
                                x = row;
                                y = collumn;
                            }
                        }
                    }
                    for (int j = 0; j < 8; j++)
                    {
                        bool clearPath = true;
                        int deltaX = 0;
                        int deltaY = 0;
                        while (clearPath && !(pieceToMove[i].Contains('R' + whiteToMoveString) && j > 3) && !(pieceToMove[i].Contains('B' + whiteToMoveString) && j < 4))
                        {
                            if (j == 0)
                            {
                                deltaX++;
                            }
                            else if (j == 1)
                            {
                                deltaX--;
                            }
                            else if (j == 2)
                            {
                                deltaY++;
                            }
                            else if (j == 3)
                            {
                                deltaY--;
                            }
                            else if (j == 4)
                            {
                                deltaX++;
                                deltaY++;
                            }
                            else if (j == 5)
                            {
                                deltaX++;
                                deltaY--;
                            }
                            else if (j == 6)
                            {
                                deltaX--;
                                deltaY++;
                            }
                            else
                            {
                                deltaX--;
                                deltaY--;
                            }

                            if (x + deltaX > -1 && x + deltaX < 8 && y + deltaY > -1 && y + deltaY < 8)
                            {
                                if (board[x + deltaX, y + deltaY] == null)
                                {
                                    function.Run(x, y, deltaX, deltaY, pieceToMove[i], whiteToMove, board);
                                }
                                else if (board[x + deltaX, y + deltaY].Contains("ep"))
                                {
                                    function.Run(x, y, deltaX, deltaY, pieceToMove[i], whiteToMove, board);
                                }
                                else if (whiteToMove && board[x + deltaX, y + deltaY].Contains("-") || whiteToMove == false && board[x + deltaX, y + deltaY].Contains("+"))
                                {
                                    function.Run(x, y, deltaX, deltaY, pieceToMove[i], whiteToMove, board);
                                    clearPath = false;
                                }
                                else
                                {
                                    clearPath = false;
                                }
                            }
                            else
                            {
                                clearPath = false;
                            }
                        }
                    }
                }
                else if (pieceToMove[i].Contains('N' + whiteToMoveString))
                {
                    for (int row = 0; row < 8; row++)
                    {
                        for (int collumn = 0; collumn < 8; collumn++)
                        {
                            if (board[row, collumn] == pieceToMove[i])
                            {
                                x = row;
                                y = collumn;
                            }
                        }
                    }
                    for (int deltaX = -2; deltaX < 3; deltaX++)
                    {
                        for (int deltaY = -2; deltaY < 3; deltaY++)
                        {
                            if (deltaX != 0 && deltaY != 0 && Math.Abs(deltaX) != Math.Abs(deltaY))
                            {
                                if (x + deltaX > -1 && x + deltaX < 8 && y + deltaY > -1 && y + deltaY < 8)
                                {
                                    if (board[x + deltaX, y + deltaY] == null)
                                    {
                                        function.Run(x, y, deltaX, deltaY, pieceToMove[i], whiteToMove, board);
                                    }
                                    else if (board[x + deltaX, y + deltaY].Contains("ep"))
                                    {
                                        function.Run(x, y, deltaX, deltaY, pieceToMove[i], whiteToMove, board);
                                    }
                                    else if (whiteToMove && board[x + deltaX, y + deltaY].Contains("-") || whiteToMove == false && board[x + deltaX, y + deltaY].Contains("+"))
                                    {
                                        function.Run(x, y, deltaX, deltaY, pieceToMove[i], whiteToMove, board);
                                    }
                                }
                            }
                        }
                    }
                }
                else if (pieceToMove[i].Contains(whiteToMoveString) && !pieceToMove[i].Contains("ep"))
                {
                    for (int row = 0; row < 8; row++)
                    {
                        for (int collumn = 0; collumn < 8; collumn++)
                        {
                            if (board[row, collumn] == pieceToMove[i])
                            {
                                x = row;
                                y = collumn;
                            }
                        }
                    }

                    int deltaX = 1;
                    if (whiteToMove)
                    {
                        deltaX = -1;
                    }
                    if (board[x + deltaX, y] == null)
                    {
                        function.Run(x, y, deltaX, 0, pieceToMove[i], whiteToMove, board);
                    }

                    for (int deltaY = -1; deltaY < 2; deltaY = deltaY + 2)
                    {
                        if (y + deltaY > -1 && y + deltaY < 8)
                        {
                            if (board[x + deltaX, y + deltaY] != null)
                            {
                                if (whiteToMove && board[x + deltaX, y + deltaY].Contains("-") || !whiteToMove && board[x + deltaX, y + deltaY].Contains("+"))
                                {
                                    function.Run(x, y, deltaX, deltaY, pieceToMove[i], whiteToMove, board);
                                }
                            }
                        }
                    }
                }
            }
            if (!function.containsKing)
            {
                int castleX = 0;
                if (whiteToMove)
                {
                    castleX = 7;
                }
                if (((castlingRights[0] || castlingRights[1]) && whiteToMove) || ((castlingRights[2] || castlingRights[3]) && !whiteToMove))
                {
                    if (board[castleX, 1] == null && board[castleX, 2] == null && board[castleX, 3] == null && ((castlingRights[0] && whiteToMove) || (castlingRights[2] && !whiteToMove)))
                    {
                        if (KingIsSafe(board, whiteToMove, castleX, 4, 0, -1) && KingIsSafe(board, whiteToMove, castleX, 4, 0, -2) && KingIsSafe(board, whiteToMove, castleX, 4, 0, -3))
                        {
                            if (function.addMove)
                            {
                                legalMove.Add("O-O-O");
                                moveCoords.Add("longCastle");
                            }
                            else
                            {
                                exists = true;
                            }
                        }
                    }
                    if (board[castleX, 5] == null && board[castleX, 6] == null && ((castlingRights[1] && whiteToMove) || (castlingRights[3] && !whiteToMove)))
                    {
                        if (KingIsSafe(board, whiteToMove, castleX, 4, 0, 1) && KingIsSafe(board, whiteToMove, castleX, 4, 0, 2))
                        {
                            if (function.addMove)
                            {
                                legalMove.Add("O-O");
                                moveCoords.Add("shortCastle");
                            }                        
                            else
                            {
                                exists = true;
                            }
                        }
                    }
                }
            }
        }

        class Function
        {
            public Function()
            {
                containsKing = false;
                addMove = false;
            }
            public bool containsKing;
            public bool addMove;
            virtual public void Run(int x, int y, int deltaX, int deltaY, string piece, bool whiteToMove, string[,] board)
            {

            }
        }
        class AddMove : Function
        {
            public AddMove()
            {
                addMove = true;
                containsKing = false;
            }
            override public void Run(int x, int y, int deltaX, int deltaY, string piece, bool whiteToMove, string[,] board)
            {
                if (KingIsSafe(board, whiteToMove, x, y, deltaX, deltaY))
                {
                    legalMove.Add(piece[0].ToString() + Convert.ToChar(y + 97) + (8 - x) + Convert.ToChar(y + deltaY + 97) + (8 - (x + deltaX)));
                    moveCoords.Add(x.ToString() + y.ToString() + (x + deltaX).ToString() + (y + deltaY).ToString());
                    if (piece.Contains("K"))
                    {
                        legalMove[legalMove.Count - 1] = legalMove[legalMove.Count - 1].Remove(1, 2);
                    }
                    else if (!piece.Contains("Q") && !piece.Contains("R") && !piece.Contains("B") && !piece.Contains("N"))
                    {
                        if (deltaY == 0)
                        {
                            legalMove[legalMove.Count - 1] = legalMove[legalMove.Count - 1].Remove(0, 3);
                            if (whiteToMove && x == 6 || !whiteToMove && x == 1)
                            {
                                if (board[x + 2 * deltaX, y + deltaY] == null)
                                {
                                    legalMove.Add(Convert.ToChar(y + deltaY + 97).ToString() + (8 - (x + 2 * deltaX)));
                                    moveCoords.Add(x.ToString() + y.ToString() + (x + 2 * deltaX).ToString() + (y + deltaY).ToString());
                                }
                            }
                        }
                        else
                        {
                            legalMove[legalMove.Count - 1] = legalMove[legalMove.Count - 1].Remove(0, 1);
                            legalMove[legalMove.Count - 1] = legalMove[legalMove.Count - 1].Remove(1, 1);
                        }
                        if ((whiteToMove && x == 1) || (!whiteToMove && x == 6))
                        {
                            legalMove.Add(legalMove[legalMove.Count - 1] + "=B");
                            legalMove.Add(legalMove[legalMove.Count - 2] + "=R");
                            legalMove.Add(legalMove[legalMove.Count - 3] + "=Q");
                            legalMove[legalMove.Count - 4] = legalMove[legalMove.Count - 4] + "=N";
                            moveCoords[moveCoords.Count - 1] = "N" + x.ToString() + y.ToString() + (x + deltaX).ToString() + y.ToString();
                            moveCoords.Add("B" + x.ToString() + y.ToString() + (x + deltaX).ToString() + y.ToString());
                            moveCoords.Add("R" + x.ToString() + y.ToString() + (x + deltaX).ToString() + y.ToString());
                            moveCoords.Add("Q" + x.ToString() + y.ToString() + (x + deltaX).ToString() + y.ToString());
                        }
                    }
                    if (board[x + deltaX, y + deltaY] != null)
                    {
                        legalMove[legalMove.Count - 1] = legalMove[legalMove.Count - 1].Insert(legalMove[legalMove.Count - 1].Length - 2, "x");
                    }
                }
            }
        }
        class ContainsKing : Function
        {
            public ContainsKing()
            {
                addMove = false;
                containsKing = true;
            }
            override public void Run(int x, int y, int deltaX, int deltaY, string piece, bool whiteToMove, string[,] board)
            {
                if (board[x + deltaX, y + deltaY] != null)
                {
                    if (board[x + deltaX, y + deltaY].Contains("K"))
                    {
                        safe = false;
                    }
                }
            }
        }
        class Exists : Function
        {
            override public void Run(int x, int y, int deltaX, int deltaY, string piece, bool whiteToMove, string[,] board)
            {
                if (KingIsSafe(board, whiteToMove, x, y, deltaX, deltaY))
                {
                    exists = true;
                }
            }
        }
        static int RepetitionCount(string[,] board)
        {
            string[,] pastBoard = new string[8, 8];
            Reset(ref pastBoard);
            bool[] pastCastle = new bool[4]; for (int i = 0; i < 4; i++) { pastCastle[i] = true; }
            bool pastToMove = true;
            int n = 0;
            for (int i = -1; i < memory.Count - 1; i++)
            {
                bool same = true;
                if (i > -1)
                {
                    MakeMove(ref pastBoard, ref pastCastle, coordMemory[i], pastToMove, true);
                    if (pastToMove)
                    {
                        pastToMove = false;
                    }
                    else
                    {
                        pastToMove = true;
                    }
                }
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        if (board[x, y] != pastBoard[x, y])
                        {
                            same = false;
                        }
                    }
                }
                if (same)
                {
                    n++;
                }
            }
            return n;
        }
    }
}
