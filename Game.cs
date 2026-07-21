using System;
using System.Collections.Generic;
using System.Text;

namespace MahjongSolitaire
{
    class Game
    {
        int[,,] board = new int[5, 9, 16];
        int[,,] layout = new int[5, 9, 16];

        public void NewGame()
        {
            int layer, row, col;

            // 0 = no tile, 1 = tile is at that location
            // first, initialize every spot to 0
            for (layer=0; layer<5; layer++)
                for (row=0; row<9; row++)
                    for (col=0; col<16; col++)
                        board[layer, row, col] = 0;

            // layer 0 (bottom layer)
            layer = 0;
            row = 0;
            for (col = 1; col <= 12; col++)
                board[layer, row, col] = 1;
            row = 1;
            for (col = 3; col <= 10; col++)
                board[layer, row, col] = 1;
            row = 2;
            for (col = 2; col <= 11; col++)
                board[layer, row, col] = 1;
            row = 3;
            for (col = 1; col <= 12; col++)
                board[layer, row, col] = 1;

            row = 4;
            for (col = 1; col <= 12; col++)
                board[layer, row, col] = 1;
            row = 5;
            for (col = 2; col <= 11; col++)
                board[layer, row, col] = 1;
            row = 6;
            for (col = 3; col <= 10; col++)
                board[layer, row, col] = 1;
            row = 7;
            for (col = 1; col <= 12; col++)
                board[layer, row, col] = 1;

            board[0, 8, 0] = 1;
            board[0, 8, 13] = 1;
            board[0, 8, 14] = 1;

            // layer 1
            layer = 1;
            for (row = 1; row <= 6; row++)
                for (col = 4; col <= 9; col++)
                    board[layer, row, col] = 1;

            // layer 2
            layer = 2;
            for (row = 2; row <= 5; row++)
                for (col = 5; col <= 8; col++)
                    board[layer, row, col] = 1;

            // layer 3
            layer = 3;
            for (row = 3; row <= 4; row++)
                for (col = 6; col <= 7; col++)
                    board[layer, row, col] = 1;

            // layer 4
            board[4, 8, 15] = 1;

            // create layout
            layout = (int[,,])board.Clone();
        }

        public bool IsOccupied(int layer, int row, int col)
        {
            if (layer < 0 || layer > 4)
                return false;
            if (row < 0 || row > 8)
                return false;
            if (col < 0 || col > 15)
                return false;

            if (board[layer, row, col] == 1)
                return true;
            else
                return false;
        }

        public bool IsInLayout(int layer, int row, int col)
        {
            if (layer < 0 || layer > 4)
                return false;
            if (row < 0 || row > 8)
                return false;
            if (col < 0 || col > 15)
                return false;

            if (layout[layer, row, col] == 1)
                return true;
            else
                return false;
        }

        public bool IsOpen(int layer, int row, int col)
        {
            // see if the location is occupied
            if (!IsOccupied(layer, row, col))
                return false;

            // check if there is a tile above
            if (layer < 4 && IsOccupied(layer + 1, row, col))
                return false;

            // check if there is a tile to the left or right
            if (col == 0 || col == 15 || col == 14)
                return true;
            if (IsOccupied(layer, row, col - 1) && IsOccupied(layer, row, col + 1))
                return false;

            // check tile 0, 8, 13
            if (layer == 0 && row == 8 && col == 13)
                if (IsOccupied(0, 8, 14))
                    return false;

            // check tiles to the right of 0, 8, 0
            if (layer == 0 && (row == 3 || row == 4) && col == 1)
                if (IsOccupied(0, 8, 0))
                    return false;

            // check tiles to the left of 0, 8, 13
            if (layer == 0 && (row == 3 || row == 4) && col == 12)
                if (IsOccupied(0, 8, 13))
                    return false;

            // check all of layer 3
            if (layer == 3 && IsOccupied(4, 8, 15))
                return false;

            // all other return true
            return true;
        }

        public void RemoveTile(int layer, int row, int col)
        {
            board[layer, row, col] = 0;
        }

        public int TilesRemaining()
        {
            int count = 0;
            for (int layer = 0; layer < 5; layer++)
                for (int row = 0; row < 9; row++)
                    for (int col = 0; col < 16; col++)
                        count += board[layer, row, col];
            return count;
        }
    }
}
