using im_bored.math;
namespace im_bored.snek_franchise{
    public class Snek{
        private readonly char[][] _displayBuffer;
        private Vector2i _snekDirection = new(1,0);
        private Vector2i _snekLocation = new(4,4);
        private Vector2i _foodLocation = new();
        private int _snekLength = 3;
        private readonly List<Vector2i> _snekParts;
        private const float _snekMoveSpeed = 20;
        private float _snekMoveTimer = 0;
        private readonly Random _random = new();
        private bool _gameOver = false;
        string _gameOverReason = "";
        private static readonly int _mapWidth = 20;
        private static readonly int _mapHeight = 12;
        public Snek(){
            _snekParts = [];
            for (int i = 0; i < _snekLength; i++){
                _snekParts.Add(new(_snekLocation.X - i, _snekLocation.Y));
            }
            _displayBuffer = GenerateEmptyArena();
        }
        private static char[][] GenerateEmptyArena(){
            char[][] buf = new char[_mapHeight][];
            for (int i = 0; i < _mapHeight; i++){
                buf[i] = new char[_mapWidth];
            }
            for (int j = 0; j < _mapHeight; j++){
                for(int k = 0; k < _mapWidth; k++){
                    buf[j][k] = ' ';
                }
            }
            for (int x = 0; x < _mapWidth; x++){
                buf[0][x] = '-';
                buf[_mapHeight-1][x] = '-';
            }
            for (int y = 0; y < _mapHeight; y++){
                buf[y][0] = '|';
                buf[y][_mapWidth-1] = '|';
            }
            buf[0][0] = '+';
            buf[0][_mapWidth-1] = '+';
            buf[_mapHeight-1][0] = '+';
            buf[_mapHeight-1][_mapWidth-1] = '+';
            return buf;
        }
        private void RegenerateFood(){
            _displayBuffer[_foodLocation.Y][_foodLocation.X] = ' ';
            while(true){
                _foodLocation = new(_random.Next(1,_mapWidth-2),_random.Next(1,_mapHeight-2));
                if(!_snekParts.Where(p => p == _foodLocation).Any() && _snekLocation != _foodLocation) break;
            }
            _displayBuffer[_foodLocation.Y][_foodLocation.X] = 'F';
        }
        private void ProcessInput(){
            if (Console.KeyAvailable){
                ConsoleKey c = Console.ReadKey(true).Key;
                switch(c){
                    case ConsoleKey.W:
                        if (_snekDirection.Y == 1) break;
                        _snekDirection.Y = -1;
                        _snekDirection.X = 0;
                        break;
                    case ConsoleKey.A:
                        if (_snekDirection.X == 1) break;
                        _snekDirection.Y = 0;
                        _snekDirection.X = -1;
                        break;
                    case ConsoleKey.S:
                        if (_snekDirection.Y == -1) break;
                        _snekDirection.Y = 1;
                        _snekDirection.X = 0;
                        break;
                    case ConsoleKey.D:
                        if (_snekDirection.X == -1) break;
                        _snekDirection.Y = 0;
                        _snekDirection.X = 1;
                        break;
                }
            }
        }
        private void GameOver(string reason){
            _gameOver = true;
            _gameOverReason = reason;
        }
        private void UpdateSnek(){
            foreach(Vector2i part in _snekParts){
                _displayBuffer[part.Y][part.X] = ' ';
            }
            for (int i = _snekLength -1; i > 0; i--){
                _snekParts[i] = _snekParts[i-1];
            }
            _snekParts[0] = _snekLocation;
            if (_snekLocation.X < 1 || _snekLocation.X > _mapWidth-2 || _snekLocation.Y < 1 || _snekLocation.Y > _mapHeight-2){
                GameOver("Out of bounds!");
                return;
            }
            for (int i = 0; i < _snekLength; i++){
                _displayBuffer[_snekParts[i].Y][_snekParts[i].X] = i == 0 ? 'O' : '#';
                if (i == 0) continue;
                if (_snekLocation == _snekParts[i]){
                    GameOver("Crashed!");
                    return;
                }
            }
        }
        private void Tick(){
            if (_snekMoveTimer >= _snekMoveSpeed){
                _snekMoveTimer = 0;
                _snekLocation += _snekDirection;
                if (_snekLocation == _foodLocation){
                    _snekLength++;
                    _snekParts.Add(new());
                    RegenerateFood();
                }
                UpdateSnek();
            }
            _snekMoveTimer++;
        }
        public void Run(){
            RegenerateFood();
            while(!_gameOver){
                ProcessInput();
                Tick();
                Console.Clear();
                for (int y = 0; y < _mapHeight; y++){
                    Console.Write(_displayBuffer[y]);
                    Console.Write("\n");
                }
                Thread.Sleep(16);    
            }
            Console.Clear();
            Console.WriteLine($"Game over! snek perished: {_gameOverReason}");
            Console.WriteLine($"Your score: {_snekLength}");
            Console.WriteLine($"GC {GC.GetTotalMemory(true)}");
        }
    }
}