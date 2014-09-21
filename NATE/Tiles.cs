using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace NATE
{
    class TileManager
    {
        public TileManager(string mPath, int mTileSize)
        {
            path = mPath;
            tileSize = mTileSize;
            image = new Image(path);
        }

        public Texture GetTile(int index)
        {
            int x = index / tileSize;
            int y = index % tileSize;
            return new Texture(image, new IntRect(x, y, tileSize, tileSize));
        }

        public string path { get; set; }
        public int tileSize { get; set; }
        public Image image;
    }
}
