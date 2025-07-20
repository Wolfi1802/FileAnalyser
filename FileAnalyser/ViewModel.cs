using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace FileAnalyser
{
    internal class ViewModel : ViewModelBase
    {
        public ObservableCollection<FileInfo> ItemsSource { set; get; } = new();
        public Action<string> ChangeHomeText;
        private FileManager fileManager = new();

        public string HomeText
        {
            set => SetProperty(nameof(HomeText), value);
            get => GetProperty<string>(nameof(HomeText));
        }

        public bool ClickCommandEnabled
        {
            set => SetProperty(nameof(ClickCommandEnabled), value);
            get => GetProperty<bool>(nameof(ClickCommandEnabled));
        }

        public ICommand ClickCommand { get; }

        public ViewModel()
        {
            this.ClickCommandEnabled = true;
            this.ClickCommand = new AsyncCommand(Click);
            this.ChangeHomeText += SwitchHomeText;

        }
        private void SwitchHomeText(string text)
        {
            this.HomeText = text;
        }

        private async Task Click()
        {
            this.ClickCommandEnabled = false;

            this.ChangeHomeText($"Start File Analyse");

            var temp = await fileManager.GetAllFiles(this.ChangeHomeText);

            if (temp is not null && temp.Count >= 1)
            {
                this.ItemsSource = new(temp.OrderByDescending(x => x.Length).ToList());

                base.OnPropertyChanged(nameof(this.ItemsSource));
                this.ChangeHomeText($"[{this.ItemsSource.Count}] Files prepared");
            }
            else
                this.ChangeHomeText($"Service not available, try it later again");



            this.ClickCommandEnabled = true;
        }
    }
}
