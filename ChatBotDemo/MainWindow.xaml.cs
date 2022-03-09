using ChatBotDemo.ViewModel.Settings;
using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;
using System.Windows.Media;

namespace ChatBotDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        private const string FontSmall = "small";
        public MainWindow()
        {
            InitializeComponent();
            AppearanceManager.Current.AccentColor = Color.FromRgb(52, 152, 219);
            AppearanceManager.Current.ThemeSource = AppearanceManager.DarkThemeSource;
            new AppearanceViewModel();
        }
    }
}
