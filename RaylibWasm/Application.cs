using System;
using System.Numerics;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using Raylib_cs;

namespace RaylibWasm
{
#if BROWSER
    [SupportedOSPlatform("browser")]
#endif
    public static partial class Application
    {
        private static float _runTime;
        private static Camera3D camera;
        private static float[] resolution;
        private static Shader shader;
        private static int locViewEye;
        private static int locViewCenter;
        private static int locRunTime;
        private static int locResolution;

        public static void Main()
        {
            Initialize();

#if !BROWSER
            while (!Raylib.WindowShouldClose())
            {
                Update(Raylib.GetFrameTime());
                Draw();
            }
            
            Terminate();
#endif
        }
        
#if BROWSER
        [JSExport]
        private static void UpdateFrame()
        {
            Update(Raylib.GetFrameTime());
            Draw();
        }
#endif

        private static void Initialize()
        {
            const int screenWidth = 800;
            const int screenHeight = 450;
            
#if !BROWSER
            var glslVersion = 330;
#else
            var glslVersion = 100;
#endif

            Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
            Raylib.InitWindow(screenWidth, screenHeight, "raylib [shaders] example - raymarching shapes");

            // Set up a 3D camera
            camera = new Camera3D
            {
                Position = new Vector3(2.5f, 2.5f, 3.0f),
                Target   = new Vector3(0.0f, 0.0f, 0.7f),
                Up       = new Vector3(0.0f, 1.0f, 0.0f),
                FovY     = 65.0f,
                Projection = CameraProjection.Perspective
            };

            // Load fragment shader only (0 tells Raylib to use its default vertex shader)
            var fragPath = $"Resources/shaders/glsl{glslVersion}/raymarching.fs";
            shader = Raylib.LoadShader(null, fragPath);

            // Get uniform locations
            locViewEye = Raylib.GetShaderLocation(shader, "viewEye");
            locViewCenter = Raylib.GetShaderLocation(shader, "viewCenter");
            locRunTime = Raylib.GetShaderLocation(shader, "runTime");
            locResolution = Raylib.GetShaderLocation(shader, "resolution");

            // Set resolution once
            resolution = [screenWidth, screenHeight];
            Raylib.SetShaderValue(shader, locResolution, resolution, ShaderUniformDataType.Vec2);
            
            _runTime = 0f;
        }

        private static void Update(float dt)
        {
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                Raylib.DisableCursor();
            }
            
            // Update camera (FPS style)
            Raylib.UpdateCamera(ref camera, CameraMode.FirstPerson);
            
            // Feed shader with camera and time
            float[] camPos    = [camera.Position.X, camera.Position.Y, camera.Position.Z];
            float[] camTarget = [camera.Target.X,   camera.Target.Y,   camera.Target.Z];

            _runTime += Raylib.GetFrameTime();
            Raylib.SetShaderValue(shader, locViewEye,    camPos,    ShaderUniformDataType.Vec3);
            Raylib.SetShaderValue(shader, locViewCenter, camTarget, ShaderUniformDataType.Vec3);
            Raylib.SetShaderValue(shader, locRunTime,    _runTime,   ShaderUniformDataType.Float);

            // Handle window resize
            if (Raylib.IsWindowResized())
            {
                resolution[0] = Raylib.GetScreenWidth();
                resolution[1] = Raylib.GetScreenHeight();
                Raylib.SetShaderValue(shader, locResolution, resolution, ShaderUniformDataType.Vec2);
            }

        }

        private static void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.RayWhite);

            Raylib.BeginShaderMode(shader);
            Raylib.DrawRectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight(), Color.White);
            Raylib.EndShaderMode();
            
            Raylib.DrawText("(c) Raymarching shader by Iñigo Quilez. MIT License.",
                Raylib.GetScreenWidth() - 280, Raylib.GetScreenHeight() - 20,
                10, Color.Black);

            Raylib.DrawFPS(10, 10);
            
            Raylib.EndDrawing();
        }

        private static void Terminate()
        {
            Raylib.UnloadShader(shader);
            Raylib.CloseWindow();
        }
    }
}
