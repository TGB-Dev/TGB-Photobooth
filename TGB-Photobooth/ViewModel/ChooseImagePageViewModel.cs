using System.ComponentModel;

namespace TGB_Photobooth.ViewModel;

partial class ChooseImagePageViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public string FramePath { get => ((App)App.Current).FramePath; }
}
