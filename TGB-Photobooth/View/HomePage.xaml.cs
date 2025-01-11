using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TGB_Photobooth.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TGB_Photobooth.View;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class HomePage : Page
{
    public HomePage()
    {
        this.InitializeComponent();

        DataContext = new HomePageViewModel();
    }

    private void ChooseFileButton_Click(object sender, RoutedEventArgs e)
    {
        ((HomePageViewModel)DataContext).ChooseFrame();
    }

    private void ChooseFolderButton_Click(object sender, RoutedEventArgs e)
    {
        ((HomePageViewModel)DataContext).ChooseDestinationFolder();
    }

    private void ChooseImagesButton_Click(object sender, RoutedEventArgs e)
    {
        ((HomePageViewModel)DataContext).ChooseImagesFolder();
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        ((HomePageViewModel)DataContext).StartTakingSession();
    }
}
