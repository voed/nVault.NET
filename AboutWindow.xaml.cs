using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;

namespace nVault.NET
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        
        public AboutWindow()
        {
            InitializeComponent();
            LogoImg.Source = Utils.ProgramImage;
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
        }
    }
}
