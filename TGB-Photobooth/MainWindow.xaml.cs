using Microsoft.UI.Xaml;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TGB_Photobooth
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            MainFrame.Navigate(typeof(View.HomePage));
        }

        public void NavigateToPage(Type page)
        {
            MainFrame.Navigate(page);
        }

        public void Back()
        {
            MainFrame.GoBack();
        }
    }
}
