using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;
using SFML;
using SFML.Graphics;
using SFML.Window;

using NATE;

namespace NATE
{
    class Program
    {
        static RenderWindow window;
        static Text textFps;
        static Stopwatch dtClock;
        static MapInterface iMap;
        static Map map;
        static Vector2f scaling;
        static Texture[] textureCollection;
        static Camera camera;
        static TileManager tiles;
        static int count = 0;
        static void Main(string[] args)
        {
            Initialize();

            float lastFrame = 0;
            while(window.IsOpen())
            {
                window.DispatchEvents();
                window.Clear(Color.Cyan);

                Update(lastFrame);
                Draw(lastFrame);

                window.Display();
                lastFrame = (float)dtClock.ElapsedTicks / (float)Stopwatch.Frequency;
                dtClock.Restart();
            }
        }

        static void Initialize()
        {
            Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + Environment.CurrentDirectory + "\\libs");
            dtClock = new Stopwatch();
            textFps = new Text("0", new Font(new FileStream("assets\\fonts\\arial.ttf", FileMode.Open, FileAccess.Read)));
            window = new RenderWindow(new VideoMode(1280, 768), "Test", Styles.Default);
            window.SetFramerateLimit(60);
            window.SetTitle("NATE");
            map = new Map(new Vector2i(32, 32), 6072, true);
            scaling = new Vector2f(4, 4);
            iMap = new MapInterface();
            textureCollection = new Texture[map.tileCount];
            camera = new Camera();
            camera.speed = 1000;
            tiles = new TileManager("assets\\tilemaps\\tiles.png", 16);

            window.Closed += (s, a) => window.Close();
            dtClock.Start();

            iMap.WriteMap("map0.ntm", map);
            for (int i = 0; i < (tiles.image.Size.X / tiles.tileSize) * (tiles.image.Size.Y / tiles.tileSize); i++)
            {
                textureCollection[i] = tiles.GetTile(i);
                textureCollection[i].Smooth = false;
            }
        }

        static void Draw(float dt)
        {
            for (int x = 0; x < map.size.X; x++)
            {
                for (int y = 0; y < map.size.Y; y++)
                {
                    Sprite s = new Sprite(textureCollection[map.data[x, y]]);
                    float xPos = y * x * s.Texture.Size.X * scaling.X + camera.X;
                    float yPos = y * s.Texture.Size.Y * scaling.Y + camera.Y;
                    //if (xPos > 0 && xPos < 1280)
                    {
                        s.Position = new Vector2f(x * s.Texture.Size.X * scaling.X + camera.X, y * s.Texture.Size.Y * scaling.Y + camera.Y);
                        s.Scale = scaling;
                        window.Draw(s);
                    }
                    count++;
                }
            }
            window.Draw(textFps);
        }
        
        static void Update(float dt)
        {
            float fps = 0;

            fps = (1f / (dt));
            textFps.DisplayedString = dt.ToString() + ", Camera: " + camera.X.ToString() + ", " + camera.Y.ToString();
            textFps.Position = new Vector2f(10, 10);

            if (Keyboard.IsKeyPressed(Keyboard.Key.W)) { if (camera.Y + camera.speed * dt > 0) { camera.Y = 0; } else { camera.Y += camera.speed * dt; } }
            if (Keyboard.IsKeyPressed(Keyboard.Key.S)) { if (camera.Y - camera.speed * dt < -(map.size.Y * 16 * scaling.Y - window.Size.Y)) { camera.Y = -(map.size.Y * 16 * scaling.Y - window.Size.Y); } else { camera.Y -= camera.speed * dt; } }
            if (Keyboard.IsKeyPressed(Keyboard.Key.A)) { if (camera.X + camera.speed * dt > 0) { camera.X = 0; } else { camera.X += camera.speed * dt; } }
            if (Keyboard.IsKeyPressed(Keyboard.Key.D)) { if (camera.X - camera.speed * dt < -(map.size.X * 16 * scaling.X - window.Size.X)) { camera.X = -(map.size.X * 16 * scaling.X - window.Size.X); } else { camera.X -= camera.speed * dt; } }
        }
    }
}
