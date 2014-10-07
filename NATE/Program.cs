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
            tiles = new TileManager("assets\\tilemaps\\rpgtiles.png", 32);
            iMap = new MapInterface();
            //map = new Map(new Vector2i(32, 32), ((int)tiles.image.Size.X / tiles.tileSize) * ((int)tiles.image.Size.Y / tiles.tileSize), true); -- for random
            //map = new Map(new Vector2i(32, 32), ((int)tiles.image.Size.X / tiles.tileSize) * ((int)tiles.image.Size.Y / tiles.tileSize), false); -- blank
            map = iMap.ReadMap("map1.ntm");
            
            scaling = new Vector2f(2, 2);
            textureCollection = new Texture[(tiles.image.Size.X / tiles.tileSize) * (tiles.image.Size.Y / tiles.tileSize)];
            camera = new Camera();
            camera.speed = 1000;

            window.Closed += (s, a) => window.Close();
            window.KeyPressed += (s, a) => { if (a.Code == Keyboard.Key.Z) { iMap.WriteMap("map0.ntm", map); } };
            window.MouseWheelMoved += (s, a) => { scaling.X += a.Delta * 0.075f; scaling.Y += a.Delta * 0.075f; };

            dtClock.Start();

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
                    s.Position = new Vector2f(x * s.Texture.Size.X * scaling.X + camera.X, y * s.Texture.Size.Y * scaling.Y + camera.Y);
                    s.Scale = scaling;
                    window.Draw(s);
                    count++;
                }
            }
            window.Draw(textFps); 
        }
        
        static void Update(float dt)
        {
            float fps = 0;

            fps = (1f / (dt));
            textFps.DisplayedString = "Frametime: " + Math.Round(dt, 3) + ", Camera: " + ((int)camera.X).ToString() + ", " + ((int)camera.Y).ToString();
            textFps.Position = new Vector2f(10, 10);

            if (Keyboard.IsKeyPressed(Keyboard.Key.W)) { if (camera.Y + camera.speed * dt > 0) { camera.Y = 0; } else { camera.Y += camera.speed * dt; } }
            if (Keyboard.IsKeyPressed(Keyboard.Key.S)) { if (camera.Y - camera.speed * dt < -(map.size.Y * tiles.tileSize * scaling.Y - window.Size.Y)) { camera.Y = -(map.size.Y * tiles.tileSize * scaling.Y - window.Size.Y); } else { camera.Y -= camera.speed * dt; } }
            if (Keyboard.IsKeyPressed(Keyboard.Key.A)) { if (camera.X + camera.speed * dt > 0) { camera.X = 0; } else { camera.X += camera.speed * dt; } }
            if (Keyboard.IsKeyPressed(Keyboard.Key.D)) { if (camera.X - camera.speed * dt < -(map.size.X * tiles.tileSize * scaling.X - window.Size.X)) { camera.X = -(map.size.X * tiles.tileSize * scaling.X - window.Size.X); } else { camera.X -= camera.speed * dt; } }
        }
    }
}
