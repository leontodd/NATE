using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using SFML.Window;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NATE
{
    class MapInterface
    {
        public void WriteMap(string path, Map map)
        {
            Stream s = File.Open(path, FileMode.Create);
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(s, map);
        }

        public Map ReadMap(string path)
        {
            
            StreamReader s = File.OpenText(path);
            dynamic o = JsonConvert.DeserializeObject(s.ReadToEnd());
            JArray j = o["layers"][0]["data"];
            int[] d1 = j.Select(jv => (int)jv).ToArray();
            Map m = new Map(new Vector2i((int)o["width"], (int)o["height"]));
            m.tileCount = (int)o["properties"]["tileCount"];
            for (int x = 0; x < m.size.X; x++)
            {
                for (int y = 0; y < m.size.Y; y++)
                {
                    m.data[x, y] = d1[y * m.size.X + x] - 1;
                }
            }
            return m;
        }
    }
    class Map
    {
        public Map(Vector2i mSize, int mTileCount = 0, bool mRandom = false)
        {
            size.X = mSize.X;
            size.Y = mSize.Y;
            data = new int[mSize.X, mSize.Y];

            if (mRandom == true)
            {
                Random r = new Random();
                for (int x = 0; x < mSize.X; x++)
                {
                    for (int y = 0; y < mSize.Y; y++)
                    {
                        data[x, y] = r.Next(0, mTileCount);
                    }
                }
            }
        }

        public string id;
        public float version;
        public Vector2i size;
        public int scale;
        public int tilesize;
        public int tileCount;
        public int[,] data;
    }
}
