using System.Windows;

namespace MahjongSolitaire
{
    /// <summary>
    /// Interaction logic for WinnerWindow.xaml
    /// </summary>
    public partial class WinnerWindow : Window
    {
        public WinnerWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
