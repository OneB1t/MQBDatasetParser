using System.ComponentModel;

public class MyViewModel : INotifyPropertyChanged
{
    private string fileNamePath;

    public string FileNamePath
    {
        get { return fileNamePath; }
        set
        {
            if (fileNamePath != value)
            {
                fileNamePath = value;
                OnPropertyChanged(nameof(FileNamePath));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}