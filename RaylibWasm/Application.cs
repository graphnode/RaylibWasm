using System;
using System.Numerics;
using System.Runtime.InteropServices.JavaScript;
using Raylib_cs;

namespace RaylibWasm
{
    public partial class Application
    {
        private static Texture2D _logo;
        private static Vector2 _position = Vector2.Zero;
        private static Vector2 _velocity = new(60, 60);
        
        /// <summary>
        /// Application entry point
        /// </summary>
        public static void Main()
        {
            Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
            Raylib.InitWindow(512, 512, "RaylibWasm");
            //Raylib.SetTargetFPS(60);
            
            _logo = Raylib.LoadTexture("Resources/raylib_logo.png");
        }

        /// <summary>
        /// Updates frame
        /// </summary>
        [JSExport]
        public static void UpdateFrame()
        {
            var dt = Raylib.GetFrameTime();
            
            _position += _velocity * dt;
            
            if (_position.X < 0 || _position.X > Raylib.GetScreenWidth() - _logo.Width)
                _velocity.X *= -1;
            if (_position.Y < 0 || _position.Y > Raylib.GetScreenHeight() - _logo.Height)
                _velocity.Y *= -1;
            
            _position.X = Math.Clamp(_position.X, 0, Raylib.GetScreenWidth() - _logo.Width);
            _position.Y = Math.Clamp(_position.Y, 0, Raylib.GetScreenHeight() - _logo.Height);
            
            Raylib.BeginDrawing();

            Raylib.ClearBackground(Color.RayWhite);

            Raylib.DrawFPS(4, 4);
            Raylib.DrawText("All systems operational!", 4, 32, 20, Color.Maroon);
            
            Raylib.DrawTextureV(_logo, _position, Color.White);

            Raylib.EndDrawing();
        }
    }
}
