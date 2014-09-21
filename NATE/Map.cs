using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using SFML.Window;

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
    }

    [Serializable()]  
    class Map : ISerializable
    {
        public Map(Vector2i mSize, int mTileCount, bool mRandom)
        {
            size = mSize;
            tileCount = mTileCount;
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
            else
            {

            }
        }

        public Map(SerializationInfo info, StreamingContext ctxt)
        {
            byte[] id = (byte[])info.GetValue("id", typeof(byte[]));
            byte version = (byte)info.GetValue("version", typeof(byte));
            Vector2i size = new Vector2i((int)info.GetValue("sizeX", typeof(int)), (int)info.GetValue("sizeY", typeof(int)));
            int tileCount = (int)info.GetValue("tileCount", typeof(int));
            int[,] data = (int[,])info.GetValue("data", typeof(byte[,]));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("id", id);
            info.AddValue("version", version);
            info.AddValue("sizeX", size.X);
            info.AddValue("sizeY", size.Y);
            info.AddValue("tileCount", tileCount);
            info.AddValue("data", data);
        }

        readonly byte[] id = {0xAE, 0x69, 0xAE};
        const byte version = 0x00;
        public readonly Vector2i size;
        public readonly int tileCount;
        public int[,] data;
    }
}
