using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
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

        public Visibility ListViewVisibility
        {
            set => SetProperty(nameof(ListViewVisibility), value);
            get => GetProperty<Visibility>(nameof(ListViewVisibility));
        }

        public Visibility ProgressBarVisibility
        {
            set => SetProperty(nameof(ProgressBarVisibility), value);
            get => GetProperty<Visibility>(nameof(ProgressBarVisibility));
        }

        public double ProgressBarValue
        {
            set => SetProperty(nameof(ProgressBarValue), value);
            get => GetProperty<double>(nameof(ProgressBarValue));
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
            this.ListViewVisibility = Visibility.Hidden;
            this.ProgressBarVisibility = Visibility.Visible;

            this.ChangeHomeText($"Start File Analyse");

            //TODO [TS] BESSERE STELLE FINDEN
            #region EventSetup

            this.fileManager.NotifyPercentValue = (value) =>
            {
                this.ProgressBarValue = value;
            };
            this.fileManager.NotifyProgress = (text) =>
            {
                this.ChangeHomeText(text);
            };

            #endregion

            var temp = await this.fileManager.GetAllFiles();

            if (temp is not null && temp.Count >= 1)
            {
                this.ItemsSource = new(temp.OrderByDescending(x => x.Length).ToList());

                base.OnPropertyChanged(nameof(this.ItemsSource));
                this.ChangeHomeText($"[{this.ItemsSource.Count}] Files prepared");
            }
            else
                this.ChangeHomeText($"Service not available, try it later again");


            this.ListViewVisibility = Visibility.Visible;
            this.ProgressBarVisibility = Visibility.Collapsed;//[TS] shouldnt be visible or touchable after job is done
            this.ClickCommandEnabled = true;
        }
    }
}
