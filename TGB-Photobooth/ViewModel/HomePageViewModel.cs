using System;
using System.ComponentModel;
using System.IO;
using TGB_Photobooth.View;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace TGB_Photobooth.ViewModel;

class HomePageViewModel : INotifyPropertyChanged
{
    private string _framePath = @"D:\frame.png";
    public string FramePath
    {
        get => _framePath;
        set
        {
            _framePath = value;
            OnPropertyChanged(nameof(FramePath));
            OnPropertyChanged(nameof(CanStartTakingSession));
            ((App)App.Current).FramePath = value;
        }
    }

    private string _destinationFolderPath = @"D:\dest";
    public string DestinationFolderPath
    {
        get => _destinationFolderPath;
        set
        {
            _destinationFolderPath = value;
            OnPropertyChanged(nameof(DestinationFolderPath));
            OnPropertyChanged(nameof(CanStartTakingSession));
            ((App)App.Current).DestinationFolderPath = value;
        }
    }

    private string _imagesFolderPath = @"C:\Users\bm\Desktop\tgb";
    public string ImagesFolderPath
    {
        get => _imagesFolderPath;
        set
        {
            _imagesFolderPath = value;
            OnPropertyChanged(nameof(ImagesFolderPath));
            OnPropertyChanged(nameof(CanStartTakingSession));
            ((App)App.Current).ImagesFolderPath = value;
        }
    }

    public int TakeCount { get; set; } = 8;
    public bool CanStartTakingSession { get => File.Exists(FramePath) && Directory.Exists(DestinationFolderPath); }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private nint GetHwnd()
    {
        return WinRT.Interop.WindowNative.GetWindowHandle(((App)App.Current).Window);
    }

    public async void ChooseFrame()
    {
        FileOpenPicker filePicker = new()
        {
            FileTypeFilter = { ".jpg", ".jpeg", ".png" }
        };

        WinRT.Interop.InitializeWithWindow.Initialize(filePicker, GetHwnd());

        StorageFile selectedFile = await filePicker.PickSingleFileAsync();

        if (selectedFile != null)
            FramePath = selectedFile.Path;
    }

    public async void ChooseDestinationFolder()
    {
        FolderPicker folderPicker = new()
        {
            FileTypeFilter = { ".jpg", ".jpeg", ".png" }
        };

        WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, GetHwnd());

        StorageFolder selectedFolder = await folderPicker.PickSingleFolderAsync();

        if (selectedFolder != null)
            DestinationFolderPath = selectedFolder.Path;
    }

    public async void ChooseImagesFolder()
    {
        FolderPicker folderPicker = new()
        {
            FileTypeFilter = { ".jpg", ".jpeg", ".png" }
        };

        WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, GetHwnd());

        StorageFolder selectedFolder = await folderPicker.PickSingleFolderAsync();

        if (selectedFolder != null)
            ImagesFolderPath = selectedFolder.Path;
    }

    public void StartTakingSession()
    {
        ((MainWindow)((App)App.Current).Window).NavigateToPage(typeof(ChooseImagePage));
    }
}
