using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapResourceRating
{
    public class Map
    {
        private List<Tile> _tiles;
        public List<Tile> Tiles
        {
            get
            {
                if (_tiles == null)
                {
                    _tiles = new List<Tile>();
                }
                return _tiles;
            }
        }
    }
}
