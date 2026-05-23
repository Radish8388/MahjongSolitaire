using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MahjongSolitaire
{
    class Tile
    {
        public int Number { get; set; } = 1;
        public int Suit { get; set; } = 1;
        public Image? TileImage { get; set; }
        public BitmapImage? normal {  get; set; }
        public BitmapImage? highlighted { get; set; }
        private int _layer, _row, _col;

        public void SetPosition(int layer, int row, int col)
        {
            _layer = layer;
            _row = row;
            _col = col;
        }
        public void GetPosition(out int layer, out int row, out int col)
        {
            layer = _layer;
            row = _row;
            col = _col;
        }
    }
}
