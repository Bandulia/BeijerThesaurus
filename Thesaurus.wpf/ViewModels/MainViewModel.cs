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

        // Collection of all words in the thesaurus  
        public ObservableCollection<string> AllWords { get; } = new();

        // View for filtering and sorting AllWords  
        public ICollectionView AllWordsView { get; }

        // Collection of synonyms for the selected word  
        public ObservableCollection<string> Synonyms { get; } = new();

        private string? _filterTerm;

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string? _word;
        public string? Word
        {
            get => _word;
            set
            {
                if (Set(ref _word, value))
                {
                    _filterTerm = value?.Trim();
                    OnPropertyChanged();
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
                    _filterTerm = null;
                    OnPropertyChanged(nameof(Word));
                    _ = LoadSynonymsAsync(value);
                    AllWordsView.Refresh();
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

        // Command to add a new word and its synonyms  
        public AsyncRelayCommand AddCommand { get; }

        // Command to refresh the list of all words  
        public AsyncRelayCommand RefreshCommand { get; }

        public MainViewModel()
        {
            // Initialize the view for filtering words  
            AllWordsView = CollectionViewSource.GetDefaultView(AllWords);
            AllWordsView.Filter = o =>
            {
                if (_filterTerm is null || _filterTerm.Length == 0) return true;
                var w = o as string;
                return w?.IndexOf(_filterTerm, StringComparison.OrdinalIgnoreCase) >= 0;
            };

            AddCommand = new AsyncRelayCommand(AddAsync, () => !string.IsNullOrWhiteSpace(Word));
            RefreshCommand = new AsyncRelayCommand(RefreshAllAsync);

            _ = RefreshAllAsync();
        }

        // Adds a new word and its synonyms to the thesaurus  
        private async Task AddAsync()
        {
            var word = Word?.Trim();
            if (string.IsNullOrWhiteSpace(word)) return;

            // Parse the CSV string of synonyms into a list  
            var syns = (SynonymsCsv ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            await _api.AddWordAsync(word, syns);
            await RefreshAllAsync();
            await LoadSynonymsAsync(word);

            SynonymsCsv = string.Empty;
        }

        // Refreshes the list of all words from the API  
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

        // Loads synonyms for a specific word from the API  
        private async Task LoadSynonymsAsync(string word)
        {
            var list = await _api.GetSynonymsAsync(word);
            Synonyms.Clear();
            foreach (var s in list)
                Synonyms.Add(s);
        }

        // Helper method to set a property and notify listeners  
        private bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            return true;
        }
    }
}
