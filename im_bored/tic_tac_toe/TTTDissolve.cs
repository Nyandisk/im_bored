using SFML.Graphics;
using SFML.Window;

namespace im_bored.tic_tac_toe{
    public class TTTDissolve{
        private readonly RenderWindow _window;
        private readonly VideoMode _vMode;
        public bool IsHost{get;}
        public TTTDissolve(){
            _vMode = new();
            _window = new(_vMode, $"tic-tac-toe dissolve | {(IsHost ? "clientserver" : "client")}");
        }
    }
}