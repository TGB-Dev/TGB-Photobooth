using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using TGB_Photobooth.ViewModel;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TGB_Photobooth.View;


/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ChooseImagePage : Page
{
    public ChooseImagePage()
    {
        this.InitializeComponent();
        this.DataContext = new ChooseImagePageViewModel();
        LoadImagesFromDisk();
    }

    private void LoadImagesFromDisk()
    {
        string folderPath = ((App)App.Current).ImagesFolderPath;  // Specify the folder path

        // Get all image files (you can filter by extension if needed)
        var imageFiles = Directory.GetFiles(folderPath, "*.jpg").Union(Directory.GetFiles(folderPath, "*.png")).Union(Directory.GetFiles(folderPath, "*.cr2")).ToArray();

        int columnCount = 3; // Specify the number of columns you want in the grid
        int rowCount = (int)Math.Ceiling(imageFiles.Length / (double)columnCount); // Calculate the number of rows needed

        // Set grid rows and columns dynamically based on the number of rows and columns
        ImagesGrid.RowDefinitions.Clear();
        ImagesGrid.ColumnDefinitions.Clear();

        for (int i = 0; i < columnCount; i++)
        {
            ImagesGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new Microsoft.UI.Xaml.GridLength(1, Microsoft.UI.Xaml.GridUnitType.Star) });
        }

        for (int i = 0; i < rowCount; i++)
        {
            ImagesGrid.RowDefinitions.Add(new RowDefinition { Height = new Microsoft.UI.Xaml.GridLength(1, Microsoft.UI.Xaml.GridUnitType.Star) });
        }

        int index = 0;
        foreach (var imagePath in imageFiles)
        {
            var imageBitmap = new BitmapImage(new Uri(imagePath));
            var imageControl = new Image
            {
                Source = imageBitmap,
                Stretch = Microsoft.UI.Xaml.Media.Stretch.Uniform, // Optional: maintain aspect ratio
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,

            };

            imageControl.PointerPressed += delegate (object sender, PointerRoutedEventArgs e)
            {
                if (SelectedImagesView.Children.Count >= 4)
                    return;

                var imageBitmap = new BitmapImage(new Uri(imagePath));
                var imageControl = new Image
                {
                    Source = imageBitmap,
                    Stretch = Microsoft.UI.Xaml.Media.Stretch.Uniform, // Optional: maintain aspect ratio
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,

                };

                imageControl.PointerPressed += delegate (object sender, PointerRoutedEventArgs e)
                {
                    SelectedImagesView.Children.Remove(sender as Image);

                    for (int i = 0; i < SelectedImagesView.Children.Count; ++i)
                        Grid.SetColumn(SelectedImagesView.Children.ElementAt(i) as FrameworkElement, i);
                };

                Grid.SetColumn(imageControl, SelectedImagesView.Children.Count);
                SelectedImagesView.Children.Add(imageControl);

                UpdateFrame();
            };


            // Determine the row and column for the current image
            int row = index / columnCount;
            int column = index % columnCount;

            // Add the image to the grid at the calculated position
            Grid.SetRow(imageControl, row);
            Grid.SetColumn(imageControl, column);

            // Add the Image control to the Grid
            ImagesGrid.Children.Add(imageControl);

            index++;
        }


    }

    private void UpdateFrame()
    {
        int l = SelectedImagesView.Children.Count;
        if (l >= 1)
            Image1.Source = ((Image)SelectedImagesView.Children.ElementAt(0)).Source;
        if (l >= 2)
            Image2.Source = ((Image)SelectedImagesView.Children.ElementAt(1)).Source;
        if (l >= 3)
            Image3.Source = ((Image)SelectedImagesView.Children.ElementAt(2)).Source;
        if (l >= 4)
            Image4.Source = ((Image)SelectedImagesView.Children.ElementAt(3)).Source;
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        SaveButton.IsEnabled = false;
        SaveButton.Content = "Processing...";
        SaveCanvasAsImage();
        SaveButton.Content = "Saved";
    }

    private async void SaveCanvasAsImage()
    {
        var canvas = ImageCanvas;

        // Render the Canvas content to a RenderTargetBitmap
        RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
        await renderTargetBitmap.RenderAsync(canvas);  // 'canvas' is the name of the Canvas control

        // Convert RenderTargetBitmap to a BitmapFrame (or write directly to a file)
        DateTime currentTime = DateTime.Now;
        long unixTime = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();
        StorageFolder folder = await StorageFolder.GetFolderFromPathAsync("D:\\dest");
        StorageFile file = await folder.CreateFileAsync($"{unixTime.ToString()}.png", CreationCollisionOption.ReplaceExisting);

        if (file != null)
        {
            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                var pixels = await renderTargetBitmap.GetPixelsAsync();

                // Write the pixel data to the encoder
                encoder.SetPixelData(
                    Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8,
                    Windows.Graphics.Imaging.BitmapAlphaMode.Ignore,
                    (uint)renderTargetBitmap.PixelWidth,
                    (uint)renderTargetBitmap.PixelHeight,
                    96, // DPI X
                    96, // DPI Y
                    pixels.ToArray()
                );

                await encoder.FlushAsync();
            }
        }
    }

    private async Task<StorageFile> PickSaveLocationAsync()
    {
        // Open a file picker to choose a save location
        var savePicker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.PicturesLibrary
        };
        savePicker.FileTypeChoices.Add("PNG", new[] { ".png" });
        savePicker.SuggestedFileName = "image.png";

        WinRT.Interop.InitializeWithWindow.Initialize(savePicker, GetHwnd());
        return await savePicker.PickSaveFileAsync();
    }


    private nint GetHwnd()
    {
        return WinRT.Interop.WindowNative.GetWindowHandle(((App)App.Current).Window);
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        ((App.Current as App).Window as MainWindow).Back();
    }
}
