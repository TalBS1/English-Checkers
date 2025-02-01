using System;
using System.Collections.Generic;

namespace Ex02
{
    public class Board
    {
        private readonly ePieceType[,] r_MBoard; //Can only assign here or in the constractor
        private readonly int r_MSize;

        public Board(int i_Size)
        {
            r_MSize = i_Size;
            r_MBoard = new ePieceType[i_Size, i_Size];
            initializeBoard();
        }

        private void initializeBoard()
        {
            for (int i = 0; i < r_MSize; i++)
            {
                for (int j = 0; j < r_MSize; j++)
                {
                    if (i < (r_MSize / 2) - 1 && (i + j) % 2 != 0)
                    {
                        r_MBoard[i, j] = ePieceType.O;
                    }
                    else if (i > (r_MSize / 2) && (i + j) % 2 != 0)
                    {
                        r_MBoard[i, j] = ePieceType.X;
                    }
                    else
                    {
                        r_MBoard[i, j] = ePieceType.E;
                    }
                }
            }
        }

        public void Display()
        {
            Console.WriteLine("    " + string.Join("   ", GetColumnLabels()));
            Console.WriteLine("  " + new string('=', r_MSize * 4 + 1));

            for (int i = 0; i < r_MSize; i++)
            {
                Console.Write((char)('A' + i) + " |");

                for (int j = 0; j < r_MSize; j++)
                {
                    char displayChar;

                    if (r_MBoard[i, j] == ePieceType.X)
                    {
                        displayChar = 'X';
                    }
                    else if (r_MBoard[i, j] == ePieceType.O)
                    {
                        displayChar = 'O';
                    }
                    else if (r_MBoard[i, j] == ePieceType.U)
                    {
                        displayChar = 'U';
                    }
                    else if (r_MBoard[i, j] == ePieceType.K)
                    {
                        displayChar = 'K';
                    }
                    else
                    {
                        displayChar = ' ';
                    }

                    Console.Write(displayChar == ' ' ? "   |" : $" {displayChar} |");
                }

                Console.WriteLine();
                Console.WriteLine("  " + new string('=', r_MSize * 4 + 1));
            }
        }

        public ePieceType GetPieceType(int i_Row, int i_Col)
        {
            return r_MBoard[i_Row, i_Col];
        }


        public bool IsMoveLegal(char i_StartRow, char i_StartCol, char i_EndRow, char i_EndCol, ePieceType i_Symbol)
        {
            int startRowIdx = i_StartRow - 'A';
            int startColIdx = i_StartCol - 'a';
            int endRowIdx = i_EndRow - 'A';
            int endColIdx = i_EndCol - 'a';

            if (startRowIdx < 0 || startRowIdx >= r_MSize || startColIdx < 0 || startColIdx >= r_MSize || endRowIdx < 0
               || endRowIdx >= r_MSize || endColIdx < 0 || endColIdx >= r_MSize)
            {
                return false;
            }

            ePieceType initialLocation = r_MBoard[startRowIdx, startColIdx];
            if (initialLocation == ePieceType.E)
            {
                return false;
            }

            int rowDiff = endRowIdx - startRowIdx;
            int colDiff = endColIdx - startColIdx;

            if (Math.Abs(rowDiff) == 1 && Math.Abs(colDiff) == 1 && r_MBoard[endRowIdx, endColIdx] == ePieceType.E)
            {
                if (initialLocation == ePieceType.K || initialLocation == ePieceType.U)
                {
                    return true;
                }

                bool isForwardMove = (i_Symbol == ePieceType.X && rowDiff < 0)
                                     || (i_Symbol == ePieceType.O && rowDiff > 0);
                return isForwardMove;
            }

            if (Math.Abs(rowDiff) == 2 && Math.Abs(colDiff) == 2)
            {
                int midRowIdx = (startRowIdx + endRowIdx) / 2;
                int midColIdx = (startColIdx + endColIdx) / 2;
                ePieceType midLocation = r_MBoard[midRowIdx, midColIdx];

                if (initialLocation == ePieceType.K || initialLocation == ePieceType.U)
                {
                    return r_MBoard[endRowIdx, endColIdx] == ePieceType.E
                           && ((i_Symbol == ePieceType.X
                                && (midLocation == ePieceType.O || midLocation == ePieceType.U))
                               || (i_Symbol == ePieceType.O
                                   && (midLocation == ePieceType.X || midLocation == ePieceType.K)));
                }

                bool isForwardCapture = (i_Symbol == ePieceType.X && rowDiff < 0)
                                        || (i_Symbol == ePieceType.O && rowDiff > 0);
                if (!isForwardCapture)
                {
                    return false;
                }

                return r_MBoard[endRowIdx, endColIdx] == ePieceType.E
                       && ((i_Symbol == ePieceType.X && (midLocation == ePieceType.O || midLocation == ePieceType.U))
                           || (i_Symbol == ePieceType.O
                               && (midLocation == ePieceType.X || midLocation == ePieceType.K)));
            }

            return false;
        }

        public bool ApplyMove(char i_StartRow, char i_StartCol, char i_EndRow, char i_EndCol, ePieceType i_Symbol)
        {
            int startRowIdx = i_StartRow - 'A';
            int startColIdx = i_StartCol - 'a';
            int endRowIdx = i_EndRow - 'A';
            int endColIdx = i_EndCol - 'a';

            ePieceType ePiece = r_MBoard[startRowIdx, startColIdx];
            r_MBoard[startRowIdx, startColIdx] = ePieceType.E;

            if (Math.Abs(endRowIdx - startRowIdx) == 2 && Math.Abs(endColIdx - startColIdx) == 2)
            {
                int midRowIdx = (startRowIdx + endRowIdx) / 2;
                int midColIdx = (startColIdx + endColIdx) / 2;
                r_MBoard[midRowIdx, midColIdx] = ePieceType.E;
            }

            if (ePiece == ePieceType.K || ePiece == ePieceType.U)
            {
                r_MBoard[endRowIdx, endColIdx] = ePiece;
            }
            else if ((i_Symbol == ePieceType.X && endRowIdx == 0)
                    || (i_Symbol == ePieceType.O && endRowIdx == r_MSize - 1))
            {
                r_MBoard[endRowIdx, endColIdx] = i_Symbol == ePieceType.X ? ePieceType.K : ePieceType.U;
            }
            else
            {
                r_MBoard[endRowIdx, endColIdx] = i_Symbol;
            }

            return GetCapturingMovesForPiece(endRowIdx, endColIdx, ePiece).Count > 0;
        }
        public List<(char, char, char, char)> GetCapturingMovesForPiece(int i_Row, int i_Col, ePieceType i_EPiece)
        {
            List<(char, char, char, char)> capturingMoves = new List<(char, char, char, char)>();

            for (int di = -2; di <= 2; di += 4) // Check moves two steps away
            {
                for (int dj = -2; dj <= 2; dj += 4) // Check diagonally
                {
                    int newRow = i_Row + di;
                    int newCol = i_Col + dj;

                    // Ensure the move is within board boundaries
                    if (newRow >= 0 && newRow < r_MSize && newCol >= 0 && newCol < r_MSize)
                    {
                        int midRow = (i_Row + newRow) / 2;
                        int midCol = (i_Col + newCol) / 2;
                        ePieceType midEPiece = r_MBoard[midRow, midCol];

                        // Check if the middle ePiece belongs to the opponent and the destination is empty
                        if ((i_EPiece == ePieceType.X && (midEPiece == ePieceType.O || midEPiece == ePieceType.U)
                                                     && di == -2)
                           || (i_EPiece == ePieceType.O && (midEPiece == ePieceType.X || midEPiece == ePieceType.K)
                                                        && di == 2)
                           || (i_EPiece == ePieceType.K && (midEPiece == ePieceType.O || midEPiece == ePieceType.U))
                           || (i_EPiece == ePieceType.U && (midEPiece == ePieceType.X || midEPiece == ePieceType.K)))
                        {
                            if (r_MBoard[newRow, newCol] == ePieceType.E)
                            {
                                capturingMoves.Add(
                                    ((char)(i_Row + 'A'), (char)(i_Col + 'a'), (char)(newRow + 'A'),
                                        (char)(newCol + 'a')));
                            }
                        }
                    }
                }
            }

            return capturingMoves;
        }

        public List<(char, char, char, char)> GetCapturingMoves(ePieceType i_Symbol)
        {
            List<(char, char, char, char)> capturingMoves = new List<(char, char, char, char)>();

            for (int i = 0; i < r_MSize; i++)
            {
                for (int j = 0; j < r_MSize; j++)
                {
                    ePieceType ePiece = r_MBoard[i, j];

                    // Check if the current ePiece matches the player's symbol or is a king for the same player
                    if (ePiece == i_Symbol || (i_Symbol == ePieceType.X && ePiece == ePieceType.K)
                                         || (i_Symbol == ePieceType.O && ePiece == ePieceType.U))
                    {
                        // Retrieve capturing moves for this ePiece
                        List<(char, char, char, char)> pieceCaptures = GetCapturingMovesForPiece(i, j, ePiece);

                        // Add capturing moves to the main list
                        foreach (var move in pieceCaptures)
                        {
                            capturingMoves.Add(move);
                        }
                    }
                }
            }

            return capturingMoves;
        }

        public (char, char, char, char) GetRandomMove(ePieceType i_Symbol)
        {
            Random random = new Random();
            List<(int, int, int, int)> legalMoves = new List<(int, int, int, int)>();
            List<(int, int, int, int)> doubleJumpMoves = new List<(int, int, int, int)>();

            for (int i = 0; i < r_MSize; i++)
            {
                for (int j = 0; j < r_MSize; j++)
                {
                    ePieceType ePiece = r_MBoard[i, j];

                    if (ePiece == i_Symbol || (i_Symbol == ePieceType.X && ePiece == ePieceType.K)
                                         || (i_Symbol == ePieceType.O && ePiece == ePieceType.U))
                    {
                        for (int di = -2; di <= 2; di += 4)
                        {
                            for (int dj = -2; dj <= 2; dj += 4)
                            {
                                int ni = i + di;
                                int nj = j + dj;
                                int midI = i + di / 2;
                                int midJ = j + dj / 2;

                                if (ni >= 0 && ni < r_MSize && nj >= 0 && nj < r_MSize
                                   && r_MBoard[ni, nj] == ePieceType.E
                                   && ((ePiece == ePieceType.X
                                        && (r_MBoard[midI, midJ] == ePieceType.O
                                            || r_MBoard[midI, midJ] == ePieceType.U) && di == -2)
                                       || (ePiece == ePieceType.O
                                           && (r_MBoard[midI, midJ] == ePieceType.X
                                               || r_MBoard[midI, midJ] == ePieceType.K) && di == 2)
                                       || (ePiece == ePieceType.K
                                           && (r_MBoard[midI, midJ] == ePieceType.O
                                               || r_MBoard[midI, midJ] == ePieceType.U))
                                       || (ePiece == ePieceType.U
                                           && (r_MBoard[midI, midJ] == ePieceType.X
                                               || r_MBoard[midI, midJ] == ePieceType.K))))
                                {
                                    doubleJumpMoves.Add((i, j, ni, nj));
                                }
                            }
                        }

                        for (int di = -1; di <= 1; di += 2)
                        {
                            for (int dj = -1; dj <= 1; dj += 2)
                            {
                                int ni = i + di;
                                int nj = j + dj;

                                if (ni >= 0 && ni < r_MSize && nj >= 0 && nj < r_MSize
                                   && r_MBoard[ni, nj] == ePieceType.E)
                                {
                                    bool isForwardMove =
                                        (ePiece == ePieceType.X && di == -1) || (ePiece == ePieceType.O && di == 1);
                                    if (ePiece == ePieceType.K || ePiece == ePieceType.U || isForwardMove)
                                    {
                                        legalMoves.Add((i, j, ni, nj));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (doubleJumpMoves.Count > 0)
            {
                var move = doubleJumpMoves[random.Next(doubleJumpMoves.Count)];
                return ((char)(move.Item1 + 'A'), (char)(move.Item2 + 'a'), (char)(move.Item3 + 'A'),
                           (char)(move.Item4 + 'a'));
            }

            if (legalMoves.Count == 0)
            {
                return ('A', 'a', 'A', 'a');
            }

            var normalMove = legalMoves[random.Next(legalMoves.Count)];
            return ((char)(normalMove.Item1 + 'A'), (char)(normalMove.Item2 + 'a'), (char)(normalMove.Item3 + 'A'),
                       (char)(normalMove.Item4 + 'a'));
        }

        public bool IsGameOver(out string i_Result)
        {
            bool xExists = false;
            bool oExists = false;

            for (int i = 0; i < r_MSize; i++)
            {
                for (int j = 0; j < r_MSize; j++)
                {
                    if (r_MBoard[i, j] == ePieceType.X || r_MBoard[i, j] == ePieceType.K)
                    {
                        xExists = true;
                    }
                    else if (r_MBoard[i, j] == ePieceType.O || r_MBoard[i, j] == ePieceType.U)
                    {
                        oExists = true;
                    }
                }
            }

            if (!xExists)
            {
                i_Result = "Player O wins!";
                return true;
            }
            else if (!oExists)
            {
                i_Result = "Player X wins!";
                return true;
            }

            i_Result = null;
            return false;
        }

        public bool HasLegalMoves(ePieceType i_Symbol)
        {
            for (int i = 0; i < r_MSize; i++)
            {
                for (int j = 0; j < r_MSize; j++)
                {
                    if (r_MBoard[i, j] == i_Symbol || (i_Symbol == ePieceType.X && r_MBoard[i, j] == ePieceType.K)
                                                  || (i_Symbol == ePieceType.O && r_MBoard[i, j] == ePieceType.U))
                    {
                        if (HasLegalMoveForPiece(i, j, i_Symbol))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool HasLegalMoveForPiece(int i_Row, int i_Col, ePieceType i_Symbol)
        {
            for (int rowDiff = -2; rowDiff <= 2; rowDiff++)
            {
                for (int colDiff = -2; colDiff <= 2; colDiff++)
                {
                    if (Math.Abs(rowDiff) != Math.Abs(colDiff) || Math.Abs(rowDiff) == 0)
                    {
                        continue;
                    }

                    int newRow = i_Row + rowDiff;
                    int newCol = i_Col + colDiff;

                    char startRow = (char)('A' + i_Row);
                    char startCol = (char)('a' + i_Col);
                    char endRow = (char)('A' + newRow);
                    char endCol = (char)('a' + newCol);

                    if (IsMoveLegal(startRow, startCol, endRow, endCol, i_Symbol))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public char[] GetColumnLabels()
        {
            char[] labels = new char[r_MSize];
            for (int i = 0; i < r_MSize; i++)
            {
                labels[i] = (char)('a' + i);
            }

            return labels;
        }

        public int CalculateScore(ePieceType i_Symbol)
        {
            int score = 0;

            for (int i = 0; i < r_MSize; i++)
            {
                for (int j = 0; j < r_MSize; j++)
                {
                    if (r_MBoard[i, j] == i_Symbol)
                    {
                        score += 1;
                    }
                    else if ((i_Symbol == ePieceType.X && r_MBoard[i, j] == ePieceType.K)
                            || (i_Symbol == ePieceType.O && r_MBoard[i, j] == ePieceType.U))
                    {
                        score += 4;
                    }
                }
            }

            return score;
        }

        public bool HasPieces(ePieceType i_Symbol)
        {
            for (int i = 0; i < r_MSize; i++)
            {
                for (int j = 0; j < r_MSize; j++)
                {
                    if (r_MBoard[i, j] == i_Symbol || (i_Symbol == ePieceType.X && r_MBoard[i, j] == ePieceType.K)
                                                  || (i_Symbol == ePieceType.O && r_MBoard[i, j] == ePieceType.U))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }






    //public class Board
    //{
    //    private readonly ePieceType[,] m_Board;
    //    private readonly int m_Size;

    //    public int Size
    //    {
    //        get { return m_Size; }
    //    }

    //    public Board(int size)
    //    {
    //        m_Size = size;
    //        m_Board = new ePieceType[size, size];
    //        InitializeBoard();
    //    }

    //    private void InitializeBoard()
    //    {
    //        for (int i = 0; i < m_Size; i++)
    //        {
    //            for (int j = 0; j < m_Size; j++)
    //            {
    //                if (i < (m_Size / 2) - 1 && (i + j) % 2 != 0)
    //                {
    //                    m_Board[i, j] = ePieceType.O;
    //                }
    //                else if (i > (m_Size / 2) && (i + j) % 2 != 0)
    //                {
    //                    m_Board[i, j] = ePieceType.X;
    //                }
    //                else
    //                {
    //                    m_Board[i, j] = ePieceType.E;
    //                }
    //            }
    //        }
    //    }

    //    public ePieceType GetPieceType(int row, int col)
    //    {
    //        ValidatePosition(row, col);
    //        return m_Board[row, col];
    //    }

    //    public void SetPieceType(int row, int col, ePieceType piece)
    //    {
    //        ValidatePosition(row, col);
    //        m_Board[row, col] = piece;
    //    }

    //    private void ValidatePosition(int row, int col)
    //    {
    //        if (row < 0 || row >= m_Size || col < 0 || col >= m_Size)
    //        {
    //            throw new ArgumentOutOfRangeException("Position out of board boundaries.");
    //        }
    //    }

    //    public bool IsMoveLegal(char startRow, char startCol, char endRow, char endCol, ePieceType symbol)
    //    {
    //        int startRowIdx = startRow - 'A';
    //        int startColIdx = startCol - 'a';
    //        int endRowIdx = endRow - 'A';
    //        int endColIdx = endCol - 'a';

    //        if (!IsPositionValid(startRowIdx, startColIdx) || !IsPositionValid(endRowIdx, endColIdx))
    //        {
    //            return false;
    //        }

    //        ePieceType initialPiece = m_Board[startRowIdx, startColIdx];
    //        if (initialPiece == ePieceType.E)
    //        {
    //            return false;
    //        }

    //        int rowDiff = endRowIdx - startRowIdx;
    //        int colDiff = endColIdx - startColIdx;

    //        if (Math.Abs(rowDiff) == 1 && Math.Abs(colDiff) == 1 && m_Board[endRowIdx, endColIdx] == ePieceType.E)
    //        {
    //            if (IsForwardMove(symbol, rowDiff) || IsKing(initialPiece))
    //            {
    //                return true;
    //            }
    //        }

    //        if (Math.Abs(rowDiff) == 2 && Math.Abs(colDiff) == 2)
    //        {
    //            int midRowIdx = (startRowIdx + endRowIdx) / 2;
    //            int midColIdx = (startColIdx + endColIdx) / 2;
    //            ePieceType midPiece = m_Board[midRowIdx, midColIdx];

    //            if (IsOpponentPiece(midPiece, symbol) && m_Board[endRowIdx, endColIdx] == ePieceType.E)
    //            {
    //                return true;
    //            }
    //        }

    //        return false;
    //    }

    //    public void Display()
    //    {
    //        Console.WriteLine("    " + string.Join("   ", GetColumnLabels()));
    //        Console.WriteLine("  " + new string('=', m_Size * 4 + 1));

    //        for (int i = 0; i < m_Size; i++)
    //        {
    //            Console.Write((char)('A' + i) + " |");

    //            for (int j = 0; j < m_Size; j++)
    //            {
    //                char displayChar;
    //                switch (m_Board[i, j])
    //                {
    //                    case ePieceType.X:
    //                        displayChar = 'X';
    //                        break;
    //                    case ePieceType.O:
    //                        displayChar = 'O';
    //                        break;
    //                    case ePieceType.U:
    //                        displayChar = 'U';
    //                        break;
    //                    case ePieceType.K:
    //                        displayChar = 'K';
    //                        break;
    //                    default:
    //                        displayChar = ' ';
    //                        break;
    //                }
    //                Console.Write(displayChar == ' ' ? "   |" : $" {displayChar} |");
    //            }

    //            Console.WriteLine();
    //            Console.WriteLine("  " + new string('=', m_Size * 4 + 1));
    //        }
    //    }


    //    public void ApplyMove(char startRow, char startCol, char endRow, char endCol, ePieceType piece)
    //    {
    //        int startRowIdx = startRow - 'A';
    //        int startColIdx = startCol - 'a';
    //        int endRowIdx = endRow - 'A';
    //        int endColIdx = endCol - 'a';

    //        SetPieceType(startRowIdx, startColIdx, ePieceType.E);
    //        SetPieceType(endRowIdx, endColIdx, piece);
    //    }
    //    public (char, char, char, char) GetRandomMove(ePieceType symbol)
    //    {
    //        Random random = new Random();
    //        List<(char, char, char, char)> possibleMoves = new List<(char, char, char, char)>();

    //        for (int i = 0; i < m_Size; i++)
    //        {
    //            for (int j = 0; j < m_Size; j++)
    //            {
    //                if (m_Board[i, j] == symbol)
    //                {
    //                    if (i + 1 < m_Size && j + 1 < m_Size && GetPieceType(i + 1, j + 1) == ePieceType.E)
    //                    {
    //                        possibleMoves.Add(((char)('A' + i), (char)('a' + j), (char)('A' + i + 1), (char)('a' + j + 1)));
    //                    }
    //                    if (i + 1 < m_Size && j - 1 >= 0 && GetPieceType(i + 1, j - 1) == ePieceType.E)
    //                    {
    //                        possibleMoves.Add(((char)('A' + i), (char)('a' + j), (char)('A' + i + 1), (char)('a' + j - 1)));
    //                    }
    //                }
    //            }
    //        }
    //        if (possibleMoves.Count > 0)
    //        {
    //            return possibleMoves[random.Next(possibleMoves.Count)];
    //        }
    //        return ('A', 'a', 'A', 'a');
    //    }

    //    public bool HasPieces(ePieceType piece)
    //    {
    //        for (int i = 0; i < m_Size; i++)
    //        {
    //            for (int j = 0; j < m_Size; j++)
    //            {
    //                if (GetPieceType(i, j) == piece)
    //                {
    //                    return true;
    //                }
    //            }
    //        }
    //        return false;
    //    }

    //    private bool IsPositionValid(int row, int col)
    //    {
    //        if (row >= 0 && row < m_Size && col >= 0 && col < m_Size)
    //        {
    //            return true;
    //        }
    //        return false;
    //    }

    //    private bool IsForwardMove(ePieceType symbol, int rowDiff)
    //    {
    //        if ((symbol == ePieceType.X && rowDiff < 0) || (symbol == ePieceType.O && rowDiff > 0))
    //        {
    //            return true;
    //        }
    //        return false;
    //    }

    //    private bool IsKing(ePieceType piece)
    //    {
    //        if (piece == ePieceType.K || piece == ePieceType.U)
    //        {
    //            return true;
    //        }
    //        return false;
    //    }

    //    private bool IsOpponentPiece(ePieceType piece, ePieceType symbol)
    //    {
    //        if ((symbol == ePieceType.X && (piece == ePieceType.O || piece == ePieceType.U)) ||
    //            (symbol == ePieceType.O && (piece == ePieceType.X || piece == ePieceType.K)))
    //        {
    //            return true;
    //        }
    //        return false;
    //    }

    //    public char[] GetColumnLabels()
    //    {
    //        char[] labels = new char[m_Size];
    //        for (int i = 0; i < m_Size; i++)
    //        {
    //            labels[i] = (char)('a' + i);
    //        }
    //        return labels;
    //    }
    //}
}










