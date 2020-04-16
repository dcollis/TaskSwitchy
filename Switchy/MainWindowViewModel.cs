using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Switchy.Match;
using Switchy.Model;
using Switchy.Win32;
using Switchy.Wpf;

namespace Switchy
{
    internal sealed class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly IItemMatcher<SwitchTarget> _itemMatcher;
        private List<SwitchTarget> _windowCache;

        public string SearchText { get; set; }

        public List<Match<SwitchTarget>> Matches { get; set; }

        public Match<SwitchTarget> SelectedMatch { get; set; }

        public ICommand SwitchCommand { get; private set; }
        public ICommand UpCommand { get; private set; }
        public ICommand DownCommand { get; private set; }

        public BitmapSource Preview { get; private set; }

        internal MainWindowViewModel()
        {
            _itemMatcher = new IdeaLikeItemMatcher<SwitchTarget>();
            ClearForm();
            SwitchCommand = new RelayCommand(Switch);
            UpCommand = new RelayCommand(Up);
            DownCommand = new RelayCommand(Down);
            
        }

        internal void Init(IntPtr mainWindow)
        {
            _windowCache = WindowManagerApi.GetWindowDetails(mainWindow).Cast<SwitchTarget>().ToList();
            ClearForm();
        }

        private void ClearForm()
        {
            SearchText = null;
            Matches = new List<Match<SwitchTarget>>();
            Preview = null;
            SelectedMatch = null;
        }

        // ReSharper disable once UnusedMember.Local
        // Called by magic (Fody)
        private void OnSearchTextChanged()
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                ClearForm();
            }
            else
            {
                Matches = _itemMatcher.Match(_windowCache, w => w.Name, SearchText ?? string.Empty).Take(10).ToList();
                SelectedMatch = Matches.FirstOrDefault();
            }
        }

        // ReSharper disable once UnusedMember.Local
        // Called by magic (Fody)
        private void OnSelectedMatchChanged()
        {
            if (SelectedMatch != null)
                Preview = WindowManagerApi.CaptureWindowIfItIsntMinimised(SelectedMatch.Value as Window);
        }

        private void Switch()
        {
            WindowManagerApi.ShowWindow(SelectedMatch.Value as Window);
        }

        private void Up()
        {
            int i = Matches.IndexOf(SelectedMatch);
            if (i > 0) SelectedMatch = Matches[i - 1];
        }

        private void Down()
        {
            int i = Matches.IndexOf(SelectedMatch);
            if (i < Matches.Count - 1) SelectedMatch = Matches[i + 1];
        }

    }
}
