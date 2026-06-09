// Must install NuGet package NAudio.Midi
using AirHockey;
using NAudio.SoundFont;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MahjongSolitaire
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MidiKeyboard _myKeyboard = new MidiKeyboard();
        Random _random = new Random();
        double _tileWidth = 110;
        double _tileHeight = 151;
        double _tileSide = 15;
        bool _tilesLoaded = false;
        List<Tile> _tiles = new List<Tile>();
        Game _game = new Game();
        Tile? _firstTile;
        bool _tileSelected = false;
        bool _soundOn = true;

        public MainWindow()
        {
            InitializeComponent();
        }
        #region events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // load the properties from disk
            Properties.Settings.Default.Reload();
            bool soundOn = Properties.Settings.Default.SoundOn;
            if (soundOn == false) Sound_Click(this, new RoutedEventArgs());
            this.Left = Properties.Settings.Default.WindowLeft;
            this.Top = Properties.Settings.Default.WindowTop;
            this.Width = Properties.Settings.Default.WindowWidth;
            this.Height = Properties.Settings.Default.WindowHeight;

            double screenWidth = SystemParameters.WorkArea.Width;
            double screenHeight = SystemParameters.WorkArea.Height;

            // ensure window size doesn't exceed screen size
            if (this.Width > screenWidth) this.Width = screenWidth;
            if (this.Height > screenHeight) this.Height = screenHeight;

            // ensure window is not off the left or top
            if (this.Left < 0) this.Left = 0;
            if (this.Top < 0) this.Top = 0;

            // ensure window is not off the right or bottom
            if (this.Left + this.Width > screenWidth)
                this.Left = screenWidth - this.Width;
            if (this.Top + this.Height > screenHeight)
                this.Top = screenHeight - this.Height;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            NewGame();
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            NewGame();
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            ShuffleRemainingTiles();
        }

        private void Table_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Table_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double tw = Table.ActualWidth;
            double th = Table.ActualHeight;
            double w1 = (tw - 20) / (15 + 0.0364);
            double w2 = (th - 20) / (8 * 1.3727 + 0.1364);
            _tileWidth = Math.Min(w1, w2);
            _tileWidth = Math.Max(0, _tileWidth);
            _tileHeight = _tileWidth * 1.3727;
            _tileSide = 0.1364 * _tileWidth;

            if (_tilesLoaded)
                Redraw(_tiles);
        }

        private void Tile_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            Tile tile = _tiles.First(t => t.TileImage == img);
            // now you have the tile directly
            tile.GetPosition(out int layer, out int row, out int col);
            //MessageBox.Show($"layer,row,col={layer},{row},{col}");
            //MessageBox.Show($"is open = {_game.IsOpen(layer, row, col)}");
            if (_game.IsOpen(layer, row, col))
            {
                if (!_tileSelected)
                {
                    _firstTile = tile;
                    _tileSelected = true;
                    PlaySound(115, 61, 100);
                    if (tile.TileImage != null)
                        tile.TileImage.Source = tile.highlighted;
                }
                else if (_firstTile != null && _firstTile != tile && IsMatch(_firstTile, tile))
                {
                    MatchFound(_firstTile, tile);
                    _tileSelected = false;
                }
                else
                {
                    if (_firstTile != null && _firstTile.TileImage != null)
                        _firstTile.TileImage.Source = _firstTile.normal;
                    _firstTile = tile;
                    _tileSelected = true;
                    PlaySound(115, 61, 100);
                    if (tile.TileImage != null)
                        tile.TileImage.Source = tile.highlighted;
                }
            }
        }
        #endregion
        #region non events
        private void LoadTiles()
        {
            string[] suits = { "bamboo", "character", "circle", "dragon", "wind", "flower", "season" };
            string filename = "";

            _tiles.Clear();
            Tile tile;
            for (int copy = 1; copy <= 4; copy++)
            {
                for (int suit = 1; suit <= 5; suit++)
                {
                    for (int number = 1; number <= 9; number++)
                    {
                        if (suit == 4 && number > 3) continue;
                        if (suit == 5 && number > 4) continue;
                        tile = new Tile();
                        tile.Suit = suit;
                        tile.Number = number;
                        filename = $"pack://application:,,,/images/{suits[suit - 1]}{number}b.png";
                        tile.normal = new BitmapImage(new Uri(filename, UriKind.Absolute));
                        filename = $"pack://application:,,,/images/{suits[suit - 1]}{number}c.png";
                        tile.highlighted = new BitmapImage(new Uri(filename, UriKind.Absolute));
                        tile.TileImage = new Image();
                        tile.TileImage.Source = tile.normal;
                        tile.TileImage.MouseLeftButtonDown += Tile_MouseLeftButtonDown;
                        _tiles.Add(tile);
                    }
                }
            }
            for (int suit = 6; suit <= 7; suit++)
            {
                for (int number = 1; number <= 4; number++)
                {
                    tile = new Tile();
                    tile.Suit = suit;
                    tile.Number = 1;
                    filename = $"pack://application:,,,/images/{suits[suit - 1]}{number}b.png";
                    tile.normal = new BitmapImage(new Uri(filename, UriKind.Absolute));
                    filename = $"pack://application:,,,/images/{suits[suit - 1]}{number}c.png";
                    tile.highlighted = new BitmapImage(new Uri(filename, UriKind.Absolute));
                    tile.TileImage = new Image();
                    tile.TileImage.Source = tile.normal;
                    tile.TileImage.MouseLeftButtonDown += Tile_MouseLeftButtonDown;
                    _tiles.Add(tile);
                }
            }
            _tilesLoaded = true;
        }

        private void NewGame()
        {
            _game.NewGame();
            LoadTiles();
            Shuffle(_tiles);
            Redraw(_tiles);
            CountMatches();
        }

        private void Redraw(List<Tile> list)
        {
            Image? img;
            int tileNum = 0;
            double x, y;
            int layer, row, col;

            // reset some stuff
            _tileSelected = false;
            for (int i = 0; i < list.Count; i++)
            {
                img = list[i].TileImage;
                if (img != null)
                    img.Source = list[i].normal;
            }
            Table.Children.Clear();

            // draw offset tiles
            layer = 0;
            row = 8;
            col = 14;
            if (_game.IsOccupied(layer, row, col))
            {
                img = list[tileNum].TileImage;
                if (img != null)
                {
                    list[tileNum].SetPosition(layer, row, col);
                    img.Width = _tileWidth + _tileSide;
                    img.Height = _tileHeight + _tileSide;
                    x = col * _tileWidth + 10;
                    y = 3.5 * _tileHeight + 10;
                    Canvas.SetLeft(img, x);
                    Canvas.SetTop(img, y);
                    Table.Children.Add(img);
                    tileNum++;
                }
            }
            layer = 0;
            row = 8;
            col = 13;
            if (_game.IsOccupied(layer, row, col))
            {
                img = list[tileNum].TileImage;
                if (img != null)
                {
                    list[tileNum].SetPosition(layer, row, col);
                    img.Width = _tileWidth + _tileSide;
                    img.Height = _tileHeight + _tileSide;
                    x = col * _tileWidth + 10;
                    y = 3.5 * _tileHeight + 10;
                    Canvas.SetLeft(img, x);
                    Canvas.SetTop(img, y);
                    Table.Children.Add(img);
                    tileNum++;
                }
            }

            // draw most tiles
            for (layer = 0; layer < 4; layer++)
                for (row = 0; row < 8; row++)
                    for (col = 14; col >= 0; col--)
                        if (_game.IsOccupied(layer, row, col))
                        {
                            img = list[tileNum].TileImage;
                            if (img == null) continue;
                            list[tileNum].SetPosition(layer, row, col);
                            img.Width = _tileWidth + _tileSide;
                            img.Height = _tileHeight + _tileSide;
                            x = col * _tileWidth + 10;
                            x += layer * _tileSide;
                            y = row * _tileHeight + 10;
                            y -= layer * _tileSide;
                            Canvas.SetLeft(img, x);
                            Canvas.SetTop(img, y);
                            Table.Children.Add(img);
                            tileNum++;
                        }

            // draw more offset tiles
            layer = 0;
            row = 8;
            col = 0;
            if (_game.IsOccupied(layer, row, col))
            {
                img = list[tileNum].TileImage;
                if (img != null)
                {
                    list[tileNum].SetPosition(layer, row, col);
                    img.Width = _tileWidth + _tileSide;
                    img.Height = _tileHeight + _tileSide;
                    x = col * _tileWidth + 10;
                    y = 3.5 * _tileHeight + 10;
                    Canvas.SetLeft(img, x);
                    Canvas.SetTop(img, y);
                    Table.Children.Add(img);
                    tileNum++;
                }
            }
            layer = 4;
            row = 8;
            col = 15;
            if (_game.IsOccupied(layer, row, col))
            {
                img = list[tileNum].TileImage;
                if (img != null)
                {
                    list[tileNum].SetPosition(layer, row, col);
                    img.Width = _tileWidth + _tileSide;
                    img.Height = _tileHeight + _tileSide;
                    x = 6.5 * _tileWidth + 10;
                    x += layer * _tileSide;
                    y = 3.5 * _tileHeight + 10;
                    y -= layer * _tileSide;
                    Canvas.SetLeft(img, x);
                    Canvas.SetTop(img, y);
                    Table.Children.Add(img);
                    tileNum++;
                }
            }
        }

        private void Shuffle(List<Tile> list)
        {
            Tile temp;
            int swap;

            // Do Fisher-Yates shuffle
            for (int i = list.Count - 1; i > 0; i--)
            {
                swap = _random.Next(i + 1);
                temp = list[i];
                list[i] = list[swap];
                list[swap] = temp;
            }
        }

        private bool IsMatch(Tile t1, Tile t2) => t1.Suit == t2.Suit && t1.Number == t2.Number;
 
        private void MatchFound(Tile t1, Tile t2)
        {
            t1.GetPosition(out int layer, out int row, out int col);
            _game.RemoveTile(layer, row, col);
            t2.GetPosition(out layer, out row, out col);
            _game.RemoveTile(layer, row, col);
            Table.Children.Remove(t1.TileImage);
            Table.Children.Remove(t2.TileImage);
            PlaySound(115, 49, 100);
            CountMatches();
        }

        private async void PlaySound(int instrument, int note, int duration)
        {
            if (_soundOn)
            {
                _myKeyboard.ChangeInstrument(instrument);
                await _myKeyboard.PlayNote(note, duration);
            }
        }
        #endregion

        private void Sound_Click(object sender, RoutedEventArgs e)
        {
            _soundOn = !_soundOn;
            if (_soundOn)
                SoundImage.Source = new BitmapImage(new Uri("/images/soundOn.png", UriKind.Relative));
            else
            {
                SoundImage.Source = new BitmapImage(new Uri("/images/soundOff.png", UriKind.Relative));
            }
        }

        private void CountMatches()
        {
            List<Tile> openTiles = new List<Tile>();
            int matches = 0;
            int i;

            // create a list of open tiles
            for (i = 0; i < _tiles.Count; i++)
            {
                _tiles[i].GetPosition(out int layer, out int row, out int col);
                if (_game.IsOpen(layer, row, col))
                {
                    openTiles.Add(_tiles[i]);
                }
            }

            // sort the list
            openTiles.Sort((a, b) =>
            {
                int cmp = a.Suit.CompareTo(b.Suit);
                if (cmp != 0) return cmp;
                return a.Number.CompareTo(b.Number);
            });

            // count the matches
            i = 0;
            while (i < openTiles.Count - 1)
            {
                if (IsMatch(openTiles[i], openTiles[i + 1]))
                {
                    matches++;
                    i += 2;
                }
                else
                    i += 1;
            }

            // display the count
            MatchesCount.Text = $"Matches: {matches}";
        }

        private void ShuffleRemainingTiles()
        {
            List<Tile> remainingTiles = new List<Tile>();
            int i;

            // create a list of remaining tiles
            for (i = 0; i < _tiles.Count; i++)
            {
                _tiles[i].GetPosition(out int layer, out int row, out int col);
                if (_game.IsOccupied(layer, row, col))
                {
                    remainingTiles.Add(_tiles[i]);
                }
            }

            // shuffle the list
            Shuffle(remainingTiles);

            // redraw the tiles
            Redraw(remainingTiles);
            CountMatches();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.WindowLeft = this.Left;
            Properties.Settings.Default.WindowTop = this.Top;
            Properties.Settings.Default.WindowWidth = this.Width;
            Properties.Settings.Default.WindowHeight = this.Height;
            Properties.Settings.Default.SoundOn = _soundOn;
            Properties.Settings.Default.Save();
        }
    }
}