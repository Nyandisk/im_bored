using SFML.Graphics;
using SFML.System;

namespace im_bored.math{
    public struct Line2i{
        private Vector2i _a;
        private Vector2i _b;
        public Color Color {get;set;}
        public VertexArray Drawable{get;private set;} = new(PrimitiveType.Lines,2);
        public Line2i(Vector2i a, Vector2i b, Color color){
            _a = a;
            _b = b;
            Color = color;
            UpdateDrawable();
        }
        public void SetA(Vector2i a){
            _a = a;
            UpdateDrawable();
        }
        public void SetB(Vector2i b){
            _b = b;
            UpdateDrawable();
        }
        private readonly void UpdateDrawable(){
            Vertex v1 = new((Vector2f)_a,Color);
            Vertex v2 = new((Vector2f)_b,Color);
            Drawable[0] = v1;
            Drawable[1] = v2;
        }
    }
}