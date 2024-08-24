using im_bored.util;
using SFML.Graphics;
using SFML.Window;

namespace im_bored.tic_tac_toe
{
    public class TTTDissolve
    {
        private readonly Registry<Texture> _texRegistry;
        private readonly RenderWindow _window;
        private readonly VideoMode _vMode;
        public bool IsHost { get; }
        public TTTDissolve(bool host)
        {
            IsHost = host;
            _vMode = new();
            _window = new(_vMode, $"tic-tac-toe dissolve | {(IsHost ? "clientserver" : "client")}");
            _window.Closed += (e, s) => { _window.Close(); };
            LoadTextures();
        }
        public void LoadTextures()
        {

        }
    }
}