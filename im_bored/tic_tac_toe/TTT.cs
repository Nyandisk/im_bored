namespace im_bored.tic_tac_toe
{
    public class TTT
    {
        private class Board
        {
            private readonly char[] _boardChars;
            public Board()
            {
                _boardChars = [
                    ' ',' ',' ',
                    ' ',' ',' ',
                    ' ',' ',' '
                ];
            }
            public Board Copy()
            {
                Board board = new();
                Array.Copy(_boardChars, board._boardChars, _boardChars.Length);
                return board;
            }
            private char CheckRow(int y)
            {
                if (_boardChars[y * 3] == _boardChars[y * 3 + 1] && _boardChars[y * 3 + 1] == _boardChars[y * 3 + 2])
                {
                    return _boardChars[y * 3];
                }
                return ' ';
            }
            private char CheckCol(int x)
            {
                if (_boardChars[x] == _boardChars[1 * 3 + x] && _boardChars[1 * 3 + x] == _boardChars[2 * 3 + x])
                {
                    return _boardChars[x];
                }
                return ' ';
            }
            private char CheckDiagonals()
            {
                if (_boardChars[0] == _boardChars[4] && _boardChars[4] == _boardChars[8])
                {
                    return _boardChars[0];
                }
                if (_boardChars[2] == _boardChars[4] && _boardChars[4] == _boardChars[6])
                {
                    return _boardChars[2];
                }
                return ' ';
            }
            public int[] GetFreeIndices()
            {
                return [.. _boardChars.Select((c, i) => new { c, i }).Where(x => x.c == ' ').Select(x => x.i)];
            }
            public char CheckState()
            {
                for (int col = 0; col < 3; col++)
                {
                    char result = CheckCol(col);
                    if (result == ' ') continue;
                    return result;
                }
                for (int row = 0; row < 3; row++)
                {
                    char result = CheckRow(row);
                    if (result == ' ') continue;
                    return result;
                }
                {
                    char result = CheckDiagonals();
                    if (result != ' ') return result;
                }
                return GetFreeIndices().Length == 0 ? 'T' : ' ';
            }
            public void MakeMove(int index, char who)
            {
                _boardChars[index] = who;
            }
            public void PrintBoard()
            {
                Console.WriteLine($" {_boardChars[0]} | {_boardChars[1]} | {_boardChars[2]} ");
                Console.WriteLine($"---+---+---");
                Console.WriteLine($" {_boardChars[3]} | {_boardChars[4]} | {_boardChars[5]} ");
                Console.WriteLine($"---+---+---");
                Console.WriteLine($" {_boardChars[6]} | {_boardChars[7]} | {_boardChars[8]} ");
            }
        }
        private readonly Board _gameBoard = new();
        private char _currentTurn = 'X';
        private readonly Random _random = new();
        private int AskMove()
        {
            while (true)
            {
                Console.Write("Enter board coordinate (1-9): ");
                bool success = int.TryParse(Console.ReadLine()!, out int result);
                if (!success) { Console.WriteLine("Invalid character"); continue; }
                if (result < 1 || result > 9) { Console.WriteLine("Invalid option"); continue; }
                if (!_gameBoard.GetFreeIndices().Contains(result - 1)) { Console.WriteLine("Occupied"); continue; }
                return result - 1;
            }
        }
        private static void GameOver()
        {
            Console.WriteLine($"GC {GC.GetTotalMemory(true)}");
        }
        private int GetCPUMove()
        {
            Board boardCopy = _gameBoard.Copy();
            int[] freeIndices = _gameBoard.GetFreeIndices();
            int defensiveMove = -1;
            // attack, defend, braindead
            foreach (int freeMove in freeIndices)
            {
                boardCopy.MakeMove(freeMove, 'O');
                if (boardCopy.CheckState() == 'O')
                {
                    return freeMove;
                }
                boardCopy.MakeMove(freeMove, 'X');
                if (boardCopy.CheckState() == 'X')
                {
                    defensiveMove = freeMove;
                }
                boardCopy.MakeMove(freeMove, ' ');
            }
            return defensiveMove != -1 ? defensiveMove : freeIndices[_random.Next(freeIndices.Length)];
        }
        public void Run()
        {
            while (true)
            {
                _gameBoard.PrintBoard();
                Console.WriteLine($"It is player {_currentTurn}'s turn (" + (_currentTurn == 'X' ? "You" : "CPU") + ")");
                if (_currentTurn == 'X')
                {
                    int playerMove = AskMove();
                    _gameBoard.MakeMove(playerMove, 'X');
                }
                else
                {
                    _gameBoard.MakeMove(GetCPUMove(), 'O');
                    Thread.Sleep(_random.Next(250, 750));
                }
                Console.Clear();
                char result = _gameBoard.CheckState();
                switch (result)
                {
                    case ' ':
                        break;
                    case 'T':
                        _gameBoard.PrintBoard();
                        Console.WriteLine("It's a tie!");
                        GameOver();
                        return;
                    case 'X':
                        _gameBoard.PrintBoard();
                        Console.WriteLine("You won!");
                        GameOver();
                        return;
                    case 'O':
                        _gameBoard.PrintBoard();
                        Console.WriteLine("CPU won!");
                        GameOver();
                        return;
                }
                SwapTurn();
            }
        }
        private void SwapTurn()
        {
            _currentTurn = _currentTurn == 'X' ? 'O' : 'X';
        }
    }
}