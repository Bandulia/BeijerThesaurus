using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data; 
using Thesaurus.wpf.Commands;
using Thesaurus.Wpf.Services;

namespace Thesaurus.wpf.ViewModels
{
    public sealed class MainViewModel : INotifyPropertyChanged
    {
        private readonly ThesaurusApi _api = new("https://localhost:7275");

        public ObservableCollection<string> AllWords { get; } = new();
        public ICollectionView AllWordsView { get; }
        public ObservableCollection<string> Synonyms { get; } = new();

        private string? _word;
        public string? Word
        {
            get => _word;
            set
            {
                if (Set(ref _word, value))
                {
                    AllWordsView.Refresh();
                    AddCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string? _synonymsCsv;
        public string? SynonymsCsv
        {
            get => _synonymsCsv;
            set => Set(ref _synonymsCsv, value);
        }

        private string? _selectedWord;
        public string? SelectedWord
        {
            get => _selectedWord;
            set
            {
                if (Set(ref _selectedWord, value) && !string.IsNullOrWhiteSpace(value))
                {
                    Word = value;
                    _ = LoadSynonymsAsync(value);
                }
            }
        }

        private string? _selectedSynonym;
        public string? SelectedSynonym
        {
            get => _selectedSynonym;
            set
            {
                if (Set(ref _selectedSynonym, value) && !string.IsNullOrWhiteSpace(value))
                {
                    Word = value;
                    SelectedWord = value;
                    _ = LoadSynonymsAsync(value);
                }
            }
        }

        public AsyncRelayCommand AddCommand { get; }
        public AsyncRelayCommand RefreshCommand { get; }

        public MainViewModel()
        {
            AllWordsView = CollectionViewSource.GetDefaultView(AllWords);
            AllWordsView.Filter = o =>
            {
                if (o is not string s) return false;
                var q = Word?.Trim();
                return string.IsNullOrWhiteSpace(q)
                    || s.Contains(q, StringComparison.OrdinalIgnoreCase);
            };

            AddCommand = new AsyncRelayCommand(AddAsync, () => !string.IsNullOrWhiteSpace(Word));
            RefreshCommand = new AsyncRelayCommand(RefreshAllAsync);

            _ = RefreshAllAsync();
        }

        private async Task AddAsync()
        {
            var word = Word?.Trim();
            if (string.IsNullOrWhiteSpace(word)) return;

            var syns = (SynonymsCsv ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            await _api.AddWordAsync(word, syns);
            await RefreshAllAsync();
            await LoadSynonymsAsync(word);

            SynonymsCsv = string.Empty;
        }

        private async Task RefreshAllAsync()
        {
            var words = await _api.GetAllWordsAsync();

            AllWords.Clear();
            foreach (var w in words)
                AllWords.Add(w);

            AllWordsView.Refresh();

            var wq = Word?.Trim();
            if (!string.IsNullOrWhiteSpace(wq))
                await LoadSynonymsAsync(wq);
            else
                Synonyms.Clear();
        }

        private async Task LoadSynonymsAsync(string word)
        {
            var list = await _api.GetSynonymsAsync(word);
            Synonyms.Clear();
            foreach (var s in list)
                Synonyms.Add(s);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            return true;
        }
    }
}
