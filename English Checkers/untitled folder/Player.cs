
namespace Ex02
{
    public class Player
    {
        public string IName { get; }
        public ePieceType ISymbol { get; }
        public bool IsComputer { get; }

        public Player(string i_Name, ePieceType i_Symbol, bool i_IsComputer = false)
        {
            IName = i_Name;
            ISymbol = i_Symbol;
            IsComputer = i_IsComputer;
        }
    }



    //public class Player
    //{
    //    public string Name { get; }
    //    public ePieceType Symbol { get; }
    //    public bool IsComputer { get; }
    //    private int m_Score;

    //    public Player(string name, ePieceType symbol, bool isComputer = false)
    //    {
    //        Name = name;
    //        Symbol = symbol;
    //        IsComputer = isComputer;
    //        m_Score = 0;
    //    }

    //    public int Score
    //    {
    //        get { return m_Score; }
    //    }

    //    public void IncreaseScore(int points)
    //    {
    //        m_Score += points;
    //    }

    //    public void ResetScore()
    //    {
    //        m_Score = 0;
    //    }
    //}
}
