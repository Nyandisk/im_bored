using im_bored.math;
using im_bored.util;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace im_bored.snek_franchise
{
    public class Snek2
    {
        private enum SnekMove
        {
            UP,
            LEFT,
            RIGHT,
            DOWN
        }
        private float _delta;
        private const int _gridSize = 16;
        private int _snekLength = 3;
        private math.Vector2i _mapSize = new(10, 10);
        private math.Vector2i _snekDirection = new(1, 0);
        private math.Vector2i _snekLocation = new(1, 5);
        private math.Vector2i _foodLocation;
        private IntRect _snekHeadRect = new(0, 0, 16, 16);
        private float _snekMoveTimer;
        private int _graceTurnCounter = 0;
        private bool _gameOver = false;
        private const float _snekMoveSpeed = 1 / 3f; // every x seconds
        private const int _graceTurns = 1;
        private readonly Registry<Texture> _texRegistry = new("tex2d");
        private readonly List<Line2i> _debugLines = [];
        private readonly RenderWindow _window;
        private readonly VideoMode _vMode;
        private readonly Clock _deltaClock = new();
        private readonly List<Sprite> _snekParts;
        private readonly Queue<SnekMove> _queuedSnekMoves = [];
        private readonly Sprite _foodSprite;
        private readonly Sprite _backgroundSprite;
        private readonly Sprite _overlay;
        private readonly Sprite _gameOverOverlay;
        private readonly Random _random = new();
        private readonly Texture _snekStraight;
        private readonly Texture _snekCurve;
        private readonly Texture _snekTail;
        // lord forgive me for what im about to do
        private static readonly IntRect _frame1 = new(0, 0, _gridSize, _gridSize);
        private static readonly IntRect _frame2 = new(_gridSize, 0, _gridSize, _gridSize);
        private static readonly IntRect _frame3 = new(_gridSize * 2, 0, _gridSize, _gridSize);
        private static readonly IntRect _frame4 = new(_gridSize * 3, 0, _gridSize, _gridSize);
        // the following cannot be forgiven 
        private static readonly Vector2f _sfmlZero = new();

        private static bool _debug = false;
        public Snek2()
        {
            _vMode = new((uint)(_gridSize * _mapSize.X), (uint)(_gridSize * _mapSize.Y));
            _window = new(_vMode, "snek 2: the forgotten snek", Styles.Close)
            {
                Size = new(_vMode.Width * 4, _vMode.Height * 4)
            };
            _window.SetKeyRepeatEnabled(false);
            _window.Closed += (e, s) => { _window.Close(); return; };
            _window.KeyPressed += HandleKeyPress;
            LoadTextures();
            _foodSprite = new(_texRegistry.Grab("food"));
            _snekParts = [];
            _snekStraight = _texRegistry.Grab("snake_straight");
            _snekCurve = _texRegistry.Grab("snake_curve");
            _snekTail = _texRegistry.Grab("snake_tail");
            _backgroundSprite = new(_texRegistry.Grab("background"));
            _overlay = new(_texRegistry.Grab("overlay"));
            _gameOverOverlay = new(_texRegistry.Grab("gameover"));
            for (int i = 0; i < _snekLength; i++)
            {
                if (i == 0)
                {
                    Sprite snekHead = new(_texRegistry.Grab("snake_head"))
                    {
                        Position = GetScreenPoint(_snekLocation),
                        TextureRect = _snekHeadRect
                    };
                    _snekParts.Add(snekHead);
                    continue;
                }
                Sprite snekBody = new(_snekStraight)
                {
                    Position = GetScreenPoint(new math.Vector2i(_snekLocation.X - i, _snekLocation.Y))
                };
                _snekParts.Add(snekBody);
            }
            GenerateFood();
            CreateDebugLines();
        }
        private void HandleSnakeDirection(SnekMove move)
        {
            switch (move)
            {
                case SnekMove.UP:
                    if (_snekDirection.Y == 1) break;
                    _snekDirection.X = 0;
                    _snekDirection.Y = -1;
                    _snekHeadRect.Left = _gridSize * 1;
                    break;
                case SnekMove.LEFT:
                    if (_snekDirection.X == 1) break;
                    _snekDirection.X = -1;
                    _snekDirection.Y = 0;
                    _snekHeadRect.Left = _gridSize * 2;
                    break;
                case SnekMove.RIGHT:
                    if (_snekDirection.X == -1) break;
                    _snekDirection.X = 1;
                    _snekDirection.Y = 0;
                    _snekHeadRect.Left = _gridSize * 0;
                    break;
                case SnekMove.DOWN:
                    if (_snekDirection.Y == -1) break;
                    _snekDirection.X = 0;
                    _snekDirection.Y = 1;
                    _snekHeadRect.Left = _gridSize * 3;
                    break;
            }
        }
        private void HandleKeyPress(object? sender, KeyEventArgs e)
        {
            if (_gameOver)
            {
                _window.Close();
                return;
            }
            switch (e.Code)
            {
                case Keyboard.Key.A:
                    if (_queuedSnekMoves.Count > 0 && _queuedSnekMoves.Last() == SnekMove.LEFT) break;
                    _queuedSnekMoves.Enqueue(SnekMove.LEFT);
                    break;
                case Keyboard.Key.D:
                    if (_queuedSnekMoves.Count > 0 && _queuedSnekMoves.Last() == SnekMove.RIGHT) break;
                    _queuedSnekMoves.Enqueue(SnekMove.RIGHT);
                    break;
                case Keyboard.Key.W:
                    if (_queuedSnekMoves.Count > 0 && _queuedSnekMoves.Last() == SnekMove.UP) break;
                    _queuedSnekMoves.Enqueue(SnekMove.UP);
                    break;
                case Keyboard.Key.S:
                    if (_queuedSnekMoves.Count > 0 && _queuedSnekMoves.Last() == SnekMove.DOWN) break;
                    _queuedSnekMoves.Enqueue(SnekMove.DOWN);
                    break;
                case Keyboard.Key.G:
                    _debug = !_debug;
                    break;
                default:
                    break;
            }
        }
        private void CreateDebugLines()
        {
            for (int x = 0; x < _mapSize.X; x++)
            {
                _debugLines.Add(new(new(x * _gridSize, 0), new(x * _gridSize, (int)_vMode.Width), Color.Red));
            }
            for (int y = 0; y < _mapSize.Y; y++)
            {
                _debugLines.Add(new(new(0, y * _gridSize), new((int)_vMode.Height, y * _gridSize), Color.Blue));
            }
        }
        private void LoadTextures()
        {
            Console.WriteLine("Loading assets...");
            _texRegistry.Register("snake_straight", ImBored.TryLoadTexture(@"snek2\snake_straight.png"));
            _texRegistry.Register("snake_head", ImBored.TryLoadTexture(@"snek2\snake_head.png"));
            _texRegistry.Register("snake_curve", ImBored.TryLoadTexture(@"snek2\snake_curve.png"));
            _texRegistry.Register("snake_tail", ImBored.TryLoadTexture(@"snek2\snake_tail.png"));
            _texRegistry.Register("food", ImBored.TryLoadTexture(@"snek2\food.png"));
            _texRegistry.Register("background", ImBored.TryLoadTexture(@"snek2\background.png"));
            _texRegistry.Register("overlay", ImBored.TryLoadTexture(@"snek2\overlay.png"));
            _texRegistry.Register("gameover", ImBored.TryLoadTexture(@"snek2\gameover.png"));
            Console.WriteLine("Assets loaded successfully");
        }
        private static Vector2f GetScreenPoint(math.Vector2i v)
        {
            return new(v.X * _gridSize, v.Y * _gridSize);
        }
        private void UpdateSnek()
        {
            for (int i = _snekLength - 1; i > 0; i--)
            {
                _snekParts[i].Position = _snekParts[i - 1].Position;
            }

            _snekParts[0].Position = GetScreenPoint(_snekLocation);
            _snekParts[0].TextureRect = _snekHeadRect;

            for (int i = 1; i < _snekLength; i++)
            {
                Sprite infront = _snekParts[i - 1];
                Sprite current = _snekParts[i];

                Vector2f directionInFront = ((math.Vector2i)(infront.Position - current.Position)).Normalized;
                Vector2f directionBehind = (i + 1 < _snekLength) ?
                    ((math.Vector2i)(_snekParts[i + 1].Position - current.Position)).Normalized :
                    _sfmlZero;

                if (i + 1 == _snekLength)
                {
                    current.Texture = _snekTail;
                    current.TextureRect = GetTailTextureRect(directionInFront);
                    return;
                }

                SetBodyTextureAndRect(current, directionInFront, directionBehind);
            }
        }
        private static IntRect GetTailTextureRect(Vector2f normalizedTail)
        {
            return normalizedTail.X switch
            {
                1 => _frame3,
                -1 => _frame1,
                _ => normalizedTail.Y switch
                {
                    1 => _frame4,
                    -1 => _frame2,
                    _ => new(0, 0, 0, 0)
                }
            };
        }
        private void SetBodyTextureAndRect(Sprite current, Vector2f directionInFront, Vector2f directionBehind)
        {
            if (directionInFront.X != 0 && directionBehind.X != 0)
            {
                current.Texture = _snekStraight;
                current.TextureRect = _frame1;
            }
            else if (directionInFront.Y != 0 && directionBehind.Y != 0)
            {
                current.Texture = _snekStraight;
                current.TextureRect = _frame2;
            }
            else
            {
                current.Texture = _snekCurve;
                current.TextureRect = GetCurveTextureRect(directionInFront, directionBehind);
            }
        }
        private static IntRect GetCurveTextureRect(Vector2f directionInFront, Vector2f directionBehind)
        {
            return (directionInFront, directionBehind) switch
            {
                (var d1, var d2) when d1.X == 1 && d2.Y == 1 || d1.Y == 1 && d2.X == 1 => _frame3,
                (var d1, var d2) when d1.X == -1 && d2.Y == 1 || d1.Y == 1 && d2.X == -1 => _frame4,
                (var d1, var d2) when d1.X == 1 && d2.Y == -1 || d1.Y == -1 && d2.X == 1 => _frame1,
                (var d1, var d2) when d1.X == -1 && d2.Y == -1 || d1.Y == -1 && d2.X == -1 => _frame2,
                _ => new(0, 0, 0, 0)
            };
        }
        private void GenerateFood()
        {
            while (true)
            {
                _foodLocation = new(_random.Next(0, _mapSize.X), _random.Next(0, _mapSize.Y));
                if (!_snekParts.Where(p => (math.Vector2i)(p.Position / _gridSize) == _foodLocation).Any())
                {
                    _foodSprite.Position = GetScreenPoint(_foodLocation);
                    break;
                }
            }
        }
        private void GameOver()
        {
            _gameOver = true;
        }
        private void Tick(float delta)
        {
            if (!_gameOver)
            {
                if (_snekMoveTimer >= _snekMoveSpeed)
                {
                    _snekMoveTimer = 0;
                    if (_queuedSnekMoves.Count > 0)
                    {
                        SnekMove move = _queuedSnekMoves.Dequeue();
                        HandleSnakeDirection(move);
                    }
                    _snekLocation += _snekDirection;
                    if (_snekLocation.X < 0 || _snekLocation.X == _mapSize.X || _snekLocation.Y < 0 || _snekLocation.Y == _mapSize.Y)
                    {
                        if (_graceTurnCounter <= 0)
                        {
                            GameOver();
                            return;
                        }
                        _graceTurnCounter--;
                        _snekLocation -= _snekDirection;
                        return;
                    }
                    else
                    {
                        _graceTurnCounter = _graceTurns;
                    }
                    if (_snekParts.Any(p => p.Position == GetScreenPoint(_snekLocation)))
                    {
                        GameOver();
                        return;
                    }
                    if (_snekLocation == _foodLocation)
                    {
                        _snekLength++;
                        _snekParts.Add(new(_snekStraight));
                        GenerateFood();
                    }
                    UpdateSnek();
                }
                _snekMoveTimer += delta;
            }
        }
        private void Render(RenderWindow window)
        {
            window.Draw(_backgroundSprite);
            if (_debug) _debugLines.ForEach((l) => { window.Draw(l.Drawable); });
            _snekParts.ForEach(window.Draw);
            window.Draw(_foodSprite);
            window.Draw(_overlay);
            if (_gameOver) window.Draw(_gameOverOverlay);
        }
        public void Run()
        {
            while (_window.IsOpen)
            {
                _delta = _deltaClock.Restart().AsSeconds();
                _window.DispatchEvents();
                _window.Clear();
                Tick(_delta);
                Render(_window);
                _window.Display();
            }
            _texRegistry.FreeRegistry();
            Console.WriteLine($"GC {GC.GetTotalMemory(true)}");
        }
    }
}