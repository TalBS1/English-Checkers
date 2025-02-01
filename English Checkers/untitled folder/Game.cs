
using Ex02.ConsoleUtils;
using System;
using System.Collections.Generic;

namespace Ex02
{
    public class Game
    {
        private Board m_Board;
        private Player m_Player1;
        private Player m_Player2;
        private bool m_IsVsComputer;
        private int m_Player1Score;
        private int m_Player2Score;
        private int m_BoardSize;
        private bool m_IsInitialized;
        private string m_Player1Name;
        private string m_Player2Name;

        public void Start()
        {
            initializeGame();
            playGame();
        }

        private void initializeGame()
        {
            if (!m_IsInitialized)
            {
                Console.Write("Enter Player 1 name: ");
                m_Player1Name = Console.ReadLine();
                Console.Write("Enter board size (6, 8, or 10): ");
                m_BoardSize = getValidBoardSize();
                Console.Write("Play against computer? (y/n): ");
                m_IsVsComputer = Console.ReadLine()?.Trim().ToLower() == "y";

                if (m_IsVsComputer)
                {
                    m_Player1 = new Player(m_Player1Name, ePieceType.X);
                    m_Player2 = new Player("Computer", ePieceType.O, true);
                }
                else
                {
                    Console.Write("Enter Player 2 name: ");
                    m_Player2Name = Console.ReadLine();
                    m_Player1 = new Player(m_Player1Name, ePieceType.X);
                    m_Player2 = new Player(m_Player2Name, ePieceType.O);
                }

                m_IsInitialized = true;
            }
            else
            {
                if (m_IsVsComputer)
                {
                    m_Player1 = new Player(m_Player1Name, ePieceType.X);
                    m_Player2 = new Player("Computer", ePieceType.O, true);
                }
                else
                {
                    m_Player1 = new Player(m_Player1Name, ePieceType.X);
                    m_Player2 = new Player(m_Player2Name, ePieceType.O);
                }
            }

            m_Board = new Board(m_BoardSize);
        }

        private int getValidBoardSize()
        {
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int size) && (size == 6 || size == 8 || size == 10))
                {
                    return size;
                }

                Console.Write("Invalid size. Enter 6, 8, or 10: ");
            }
        }

        private void playGame()
        {
            bool gameRunning = true;
            Player currentPlayer = m_Player1;

            while (gameRunning)
            {
                Screen.Clear();
                m_Board.Display();
                Console.WriteLine($"{currentPlayer.IName}'s Turn ({currentPlayer.ISymbol}):");

                if (currentPlayer.IsComputer)
                {
                    performComputerMove(currentPlayer);
                }
                else
                {
                    string move = getValidMove(currentPlayer);
                    processMove(move, currentPlayer);
                }

                if (checkGameOver(out string winner))
                {
                    Screen.Clear();
                    m_Board.Display();
                    Console.WriteLine(winner);
                    if (winner == $"{m_Player1.IName} wins!")
                    {
                        calculateAndDisplayScore(m_Player1, m_Player2);
                    }
                    else if (winner == $"{m_Player2.IName} wins!")
                    {
                        calculateAndDisplayScore(m_Player2, m_Player1);
                    }

                    gameRunning = askForAnotherGame();
                }
                else
                {
                    currentPlayer = currentPlayer == m_Player1 ? m_Player2 : m_Player1;
                }
            }
        }

        private string getValidMove(Player i_CurrentPlayer)
        {
            while (true)
            {
                List<(char, char, char, char)> capturingMoves = m_Board.GetCapturingMoves(i_CurrentPlayer.ISymbol);

                if (capturingMoves.Count > 0)
                {
                    Console.WriteLine("You must make a capturing move.");
                }

                Console.Write("Enter your move (RowCol>RowCol) or 'Q' to quit: ");
                string input = Console.ReadLine()?.Trim();

                if (input?.ToLower() == "q")
                {
                    Console.WriteLine($"{i_CurrentPlayer.IName} has quit the game.");
                    Environment.Exit(0);
                }

                if (isValidMove(input, i_CurrentPlayer.ISymbol, capturingMoves))
                {
                    return input;
                }
                else
                {
                    Console.WriteLine("Invalid move. Please try again.");
                }
            }
        }

        private bool isValidMove(string i_Move, ePieceType i_Symbol, List<(char, char, char, char)> i_CapturingMoves)
        {
            string[] parts = i_Move.Split('>');
            if (parts.Length != 2 || parts[0].Length != 2 || parts[1].Length != 2)
            {
                return false;
            }

            char startRow = parts[0][0];
            char startCol = parts[0][1];
            char endRow = parts[1][0];
            char endCol = parts[1][1];
            int rowDiff = endRow - startRow;
            ePieceType initialLocation = m_Board.GetPieceType(startRow - 'A', startCol - 'a');
            bool isForwardMove = (i_Symbol == ePieceType.X && rowDiff < 0)
                                 || (i_Symbol == ePieceType.O && rowDiff > 0) || initialLocation == ePieceType.K
                                 || initialLocation == ePieceType.U;

            if (i_CapturingMoves.Count > 0)
            {
                foreach (var capturingMove in i_CapturingMoves)
                {
                    if (capturingMove.Item1 == startRow && capturingMove.Item2 == startCol
                                                       && capturingMove.Item3 == endRow
                                                       && capturingMove.Item4 == endCol)
                    {
                        return isForwardMove;
                    }
                }

                return false;
            }

            return m_Board.IsMoveLegal(startRow, startCol, endRow, endCol, i_Symbol);
        }

        private void processMove(string i_Move, Player i_Player)
        {
            string[] parts = i_Move.Split('>');
            char startRow = parts[0][0];
            char startCol = parts[0][1];
            char endRow = parts[1][0];
            char endCol = parts[1][1];
            //Do i have another chance to eat
            bool extraTurn = m_Board.ApplyMove(startRow, startCol, endRow, endCol, i_Player.ISymbol);
            bool isCaptureMove = Math.Abs(startRow - endRow) == 2;

            while (isCaptureMove && extraTurn)
            {
                Screen.Clear();
                m_Board.Display();
                Console.WriteLine("You must continue capturing!");
                List<(char, char, char, char)> capturingMoves = m_Board.GetCapturingMovesForPiece(endRow - 'A', endCol - 'a', i_Player.ISymbol);

                if (capturingMoves.Count > 0)
                {
                    string nextMove = getValidMove(i_Player);
                    parts = nextMove.Split('>');
                    startRow = parts[0][0];
                    startCol = parts[0][1];
                    endRow = parts[1][0];
                    endCol = parts[1][1];
                    isCaptureMove = Math.Abs(startRow - endRow) == 2;
                    extraTurn = m_Board.ApplyMove(startRow, startCol, endRow, endCol, i_Player.ISymbol);
                }
                else
                {
                    break;
                }
            }
        }
        private void performComputerMove(Player i_Computer)
        {
            Console.WriteLine("Computer's Turn. Press 'Enter' to see its move.");
            Console.ReadLine();
            // Checking all capturing moves on the board
            List<(char, char, char, char)> capturingMoves = m_Board.GetCapturingMoves(i_Computer.ISymbol);

            if (capturingMoves.Count > 0)
            {
                // Pick the first capturing move
                var currentMove = capturingMoves[0];
                char startRow = currentMove.Item1;
                char startCol = currentMove.Item2;
                char endRow = currentMove.Item3;
                char endCol = currentMove.Item4;
                bool canContinueCapturing = true;

                while (canContinueCapturing)
                {
                    // Apply the move and capture the piece
                    m_Board.ApplyMove(startRow, startCol, endRow, endCol, i_Computer.ISymbol);
                    Console.WriteLine($"Computer captured: {startRow}{startCol}>{endRow}{endCol}");
                    // Check for more capturing moves for the same piece
                    capturingMoves = m_Board.GetCapturingMovesForPiece(endRow - 'A', endCol - 'a', i_Computer.ISymbol);

                    if (capturingMoves.Count > 0)
                    {
                        // Continue with the next capturing move for this piece
                        currentMove = capturingMoves[0];
                        startRow = currentMove.Item1;
                        startCol = currentMove.Item2;
                        endRow = currentMove.Item3;
                        endCol = currentMove.Item4;
                    }

                    else
                    {
                        // No more capturing moves for this piece
                        canContinueCapturing = false;
                    }
                }
            }

            else
            {
                // No capturing moves available; make a random move
                var randomMove = m_Board.GetRandomMove(i_Computer.ISymbol);
                m_Board.ApplyMove(randomMove.Item1, randomMove.Item2, randomMove.Item3, randomMove.Item4, i_Computer.ISymbol);
                Console.WriteLine($"Computer moved: {randomMove.Item1}{randomMove.Item2}>{randomMove.Item3}{randomMove.Item4}");
            }
        }
        private bool checkGameOver(out string i_Winner)
        {
            bool player1HasMoves = m_Board.HasLegalMoves(m_Player1.ISymbol);
            bool player2HasMoves = m_Board.HasLegalMoves(m_Player2.ISymbol);
            bool player1HasPieces = m_Board.HasPieces(m_Player1.ISymbol);
            bool player2HasPieces = m_Board.HasPieces(m_Player2.ISymbol);

            if (!player1HasPieces || !player1HasMoves)
            {
                if (!player2HasPieces || !player2HasMoves)
                {
                    i_Winner = "It's a tie!";
                    return true;
                }
                else
                {
                    i_Winner = $"{m_Player2.IName} wins!";
                    return true;
                }
            }
            else if (!player2HasPieces || !player2HasMoves)
            {
                i_Winner = $"{m_Player1.IName} wins!";
                return true;
            }

            i_Winner = null;
            return false;
        }

        private void calculateAndDisplayScore(Player i_Winner, Player i_Loser)
        {
            int winnerScore = m_Board.CalculateScore(i_Winner.ISymbol);
            int loserScore = m_Board.CalculateScore(i_Loser.ISymbol);
            int points = winnerScore - loserScore;

            Console.WriteLine($"{i_Winner.IName} scored {points} points!");

            if (i_Winner == m_Player1)
            {
                m_Player1Score += points;
            }
            else
            {
                m_Player2Score += points;
            }

            Console.WriteLine($"Total Score: {m_Player1.IName}: {m_Player1Score}, {m_Player2.IName}: {m_Player2Score}");
        }

        private bool askForAnotherGame()
        {
            Console.Write("Would you like to play another game? (y/n): ");
            string input = Console.ReadLine()?.Trim().ToLower();

            if (input == "y")
            {
                initializeGame();
                Start();
            }
            else
            {
                Console.WriteLine("Thanks for playing!");
                Console.WriteLine(
                    $"Final Scores: {m_Player1Name}: {m_Player1Score}, {m_Player2Name}: {m_Player2Score}");
                return false;
            }

            return true;
        }
    }






    //public class Game
    //{
    //    private Board m_Board;
    //    private Player m_Player1;
    //    private Player m_Player2;
    //    private bool m_IsVsComputer;
    //    private int m_Player1Score;
    //    private int m_Player2Score;

    //    public void Start()
    //    {
    //        InitializeGame();
    //        PlayGame();
    //    }

    //    private void InitializeGame()
    //    {
    //        Console.Write("Enter Player 1 name: ");
    //        string player1Name = Console.ReadLine();
    //        Console.Write("Enter board size (6, 8, or 10): ");
    //        int boardSize = GetValidBoardSize();
    //        Console.Write("Play against computer? (y/n): ");
    //        m_IsVsComputer = Console.ReadLine().Trim().ToLower() == "y";

    //        if(m_IsVsComputer)
    //        {
    //            m_Player1 = new Player(player1Name, ePieceType.X);
    //            m_Player2 = new Player("Computer", ePieceType.O, true);
    //        }
    //        else
    //        {
    //            Console.Write("Enter Player 2 name: ");
    //            string player2Name = Console.ReadLine();
    //            m_Player1 = new Player(player1Name, ePieceType.X);
    //            m_Player2 = new Player(player2Name, ePieceType.O);
    //        }

    //        m_Board = new Board(boardSize);
    //    }

    //    private int GetValidBoardSize()
    //    {
    //        while(true)
    //        {
    //            if(int.TryParse(Console.ReadLine(), out int size) && (size == 6 || size == 8 || size == 10))
    //            {
    //                return size;
    //            }

    //            Console.Write("Invalid size. Enter 6, 8, or 10: ");
    //        }
    //    }

    //    private void PlayGame()
    //    {
    //        Player currentPlayer = m_Player1;

    //        while (true)
    //        {
    //            Screen.Clear();
    //            m_Board.Display();
    //            Console.WriteLine($"{m_Player1.Name}'s Score: {m_Player1.Score}");
    //            Console.WriteLine($"{m_Player2.Name}'s Score: {m_Player2.Score}");
    //            Console.WriteLine($"{currentPlayer.Name}'s Turn ({currentPlayer.Symbol}):");

    //            if (currentPlayer.IsComputer)
    //            {
    //                PerformComputerMove(currentPlayer);
    //            }
    //            else
    //            {
    //                string move = GetValidMove(currentPlayer);
    //                ProcessMove(move, currentPlayer);
    //            }

    //            if (CheckGameOver(out string winner))
    //            {
    //                Console.WriteLine(winner);
    //                AskForAnotherGame();
    //                return;  // Exit the current game loop after user decision
    //            }

    //            currentPlayer = currentPlayer == m_Player1 ? m_Player2 : m_Player1;
    //        }
    //    }

    //    private string GetValidMove(Player currentPlayer)
    //    {
    //        while(true)
    //        {
    //            Console.Write("Enter your move (RowCol>RowCol) or 'Q' to quit: ");
    //            string input = Console.ReadLine().Trim();

    //            if(input.ToLower() == "q")
    //            {
    //                Console.WriteLine($"{currentPlayer.Name} has quit the game.");
    //                Environment.Exit(0);
    //            }

    //            if(IsValidMove(input, currentPlayer.Symbol))
    //            {
    //                return input;
    //            }
    //            else
    //            {
    //                Console.WriteLine("Invalid move. Please try again.");
    //            }
    //        }
    //    }

    //    private bool IsValidMove(string move, ePieceType symbol)
    //    {
    //        string[] parts = move.Split('>');
    //        if(parts.Length != 2 || parts[0].Length != 2 || parts[1].Length != 2)
    //        {
    //            return false;
    //        }

    //        return m_Board.IsMoveLegal(parts[0][0], parts[0][1], parts[1][0], parts[1][1], symbol);
    //    }

    //    private void ProcessMove(string move, Player player)
    //    {
    //        string[] parts = move.Split('>');
    //        char startRow = parts[0][0];
    //        char startCol = parts[0][1];
    //        char endRow = parts[1][0];
    //        char endCol = parts[1][1];
    //        m_Board.ApplyMove(startRow, startCol, endRow, endCol, player.Symbol);
    //    }

    //    private void PerformComputerMove(Player computer)
    //    {
    //        var randomMove = m_Board.GetRandomMove(computer.Symbol);
    //        m_Board.ApplyMove(randomMove.Item1, randomMove.Item2, randomMove.Item3, randomMove.Item4, computer.Symbol);
    //        Console.WriteLine(
    //            $"Computer moved: {randomMove.Item1}{randomMove.Item2}>{randomMove.Item3}{randomMove.Item4}");
    //    }

    //    private bool CheckGameOver(out string winner)
    //    {
    //        if(!m_Board.HasPieces(m_Player1.Symbol))
    //        {
    //            winner = $"{m_Player2.Name} wins!";
    //            m_Player2.IncreaseScore(1);
    //            return true;
    //        }

    //        if(!m_Board.HasPieces(m_Player2.Symbol))
    //        {
    //            winner = $"{m_Player1.Name} wins!";
    //            m_Player1.IncreaseScore(1);
    //            return true;
    //        }

    //        winner = null;
    //        return false;
    //    }

    //    private void AskForAnotherGame()
    //    {
    //        Console.Write("Would you like to play another game? (y/n): ");
    //        string input = Console.ReadLine().Trim().ToLower();

    //        if(input == "y")
    //        {
    //            m_Player1.ResetScore();
    //            m_Player2.ResetScore();
    //            Start();
    //        }
    //        else
    //        {
    //            Console.WriteLine("Thanks for playing!");
    //            Console.WriteLine(
    //                $"Final Scores: {m_Player1.Name}: {m_Player1.Score}, {m_Player2.Name}: {m_Player2.Score}");
    //            Environment.Exit(0);
    //        }
    //    }
    //}

}







