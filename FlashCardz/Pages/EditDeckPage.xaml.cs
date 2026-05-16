using FlashCardz.Services;

namespace FlashCardz.Pages;

[QueryProperty(nameof(DeckId), "deckId")]
public partial class EditDeckPage : ContentPage
{
    private readonly DeckService _deckService = new();
    private DeckModel? _deck;
    private int _editingCardIndex = -1;

    public string? DeckId { get; set; }

    public EditDeckPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadDeck();
    }

    // ─── Load ─────────────────────────────────────────────────

    private async Task LoadDeck()
    {
        if (string.IsNullOrEmpty(DeckId)) return;

        var decks = await _deckService.GetDecksAsync();
        _deck = decks.FirstOrDefault(d => d.Id == DeckId);

        if (_deck == null)
        {
            await DisplayAlert("Error", "Deck not found.", "OK");
            await Shell.Current.GoToAsync("//HomePage");
            return;
        }

        TitleEntry.Text = _deck.Title;
        RenderCards();
    }

    // ─── Render cards ─────────────────────────────────────────

    private void RenderCards()
    {
        CardList.Children.Clear();

        if (_deck == null) return;

        for (int i = 0; i < _deck.Cards.Count; i++)
        {
            CardList.Children.Add(BuildCardRow(i, _deck.Cards[i]));
        }

        UpdateCounter();
    }

    private View BuildCardRow(int index, CardModel card)
    {
        var frontLabel = new Label
        {
            Text = $"Q: {card.Front}",
            FontFamily = "Handwritten",
            FontSize = 14,
            TextColor = Color.FromArgb("#1A1A1A"),
            LineBreakMode = LineBreakMode.TailTruncation,
            MaxLines = 1
        };

        var backLabel = new Label
        {
            Text = $"A: {card.Back}",
            FontFamily = "Handwritten",
            FontSize = 13,
            TextColor = Color.FromArgb("#888888"),
            LineBreakMode = LineBreakMode.TailTruncation,
            MaxLines = 1
        };

        var editBtn = new Controls.WobblyButton
        {
            Text = "Edit",
            HeightRequest = 34,
            WidthRequest = 70
        };
        var capturedIndex = index;
        editBtn.Clicked += (s, e) => ShowEditCardModal(capturedIndex);

        var deleteBtn = new Controls.WobblyButton
        {
            Text = "Delete",
            HeightRequest = 34,
            WidthRequest = 80,
            IsDanger = true
        };
        deleteBtn.Clicked += (s, e) => DeleteCard(capturedIndex);

        var btnRow = new Grid
        {
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto }
            },
            ColumnSpacing = 8
        };

        var cardNumLabel = new Label
        {
            Text = $"Card {index + 1}",
            FontFamily = "Handwritten",
            FontSize = 12,
            TextColor = Color.FromArgb("#888888"),
            VerticalOptions = LayoutOptions.Center
        };

        Grid.SetColumn(cardNumLabel, 0);
        Grid.SetColumn(editBtn, 1);
        Grid.SetColumn(deleteBtn, 2);
        btnRow.Children.Add(cardNumLabel);
        btnRow.Children.Add(editBtn);
        btnRow.Children.Add(deleteBtn);

        return new Controls.WobblyBorder
        {
            WobblyFillColor = Color.FromArgb("#FAFAFA"),
            Content = new VerticalStackLayout
            {
                Spacing = 6,
                Padding = new Thickness(4),
                Children = { btnRow, frontLabel, backLabel }
            }
        };
    }

    private void UpdateCounter()
    {
        var count = _deck?.Cards.Count ?? 0;
        CardCounterLabel.Text = $"Card #: {count}";
    }

    // ─── Add card ─────────────────────────────────────────────

    private void OnAddCardClicked(object? sender, EventArgs e)
    {
        _deck?.Cards.Add(new CardModel { Front = "", Back = "" });
        RenderCards();
        ShowEditCardModal((_deck?.Cards.Count ?? 1) - 1);
    }

    // ─── Delete card ──────────────────────────────────────────

    private void DeleteCard(int index)
    {
        if (_deck == null) return;

        if (_deck.Cards.Count <= 1)
        {
            ErrorLabel.Text = "A deck must have at least one card.";
            ErrorLabel.IsVisible = true;
            return;
        }

        ErrorLabel.IsVisible = false;
        _deck.Cards.RemoveAt(index);
        RenderCards();
    }

    // ─── Edit card modal ──────────────────────────────────────

    private void ShowEditCardModal(int index)
    {
        if (_deck == null || index < 0 || index >= _deck.Cards.Count) return;

        _editingCardIndex = index;
        var card = _deck.Cards[index];
        EditFrontEntry.Text = card.Front;
        EditBackEntry.Text = card.Back;
        EditCardOverlay.IsVisible = true;
    }

    private void OnEditCardCancelClicked(object? sender, EventArgs e)
    {
        // If card was newly added and empty, remove it
        if (_editingCardIndex >= 0 && _deck != null)
        {
            var card = _deck.Cards[_editingCardIndex];
            if (string.IsNullOrWhiteSpace(card.Front) &&
                string.IsNullOrWhiteSpace(card.Back))
            {
                _deck.Cards.RemoveAt(_editingCardIndex);
                RenderCards();
            }
        }

        EditCardOverlay.IsVisible = false;
        _editingCardIndex = -1;
    }

    private void OnEditCardSaveClicked(object? sender, EventArgs e)
    {
        if (_deck == null || _editingCardIndex < 0) return;

        _deck.Cards[_editingCardIndex].Front = EditFrontEntry.Text ?? "";
        _deck.Cards[_editingCardIndex].Back = EditBackEntry.Text ?? "";

        EditCardOverlay.IsVisible = false;
        _editingCardIndex = -1;
        RenderCards();
    }

    // ─── Save deck ────────────────────────────────────────────

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (_deck == null) return;

        if (string.IsNullOrWhiteSpace(TitleEntry.Text))
        {
            ErrorLabel.Text = "Deck title cannot be empty.";
            ErrorLabel.IsVisible = true;
            return;
        }

        _deck.Title = TitleEntry.Text.Trim();

        var success = await _deckService.UpdateDeckAsync(
            _deck.Id!, _deck.Title, _deck.Cards);

        if (success)
        {
            await DisplayAlert("Saved!", "Deck updated successfully.", "OK");
            await Shell.Current.GoToAsync("//HomePage");
        }
        else
        {
            ErrorLabel.Text = "Failed to save. Is the API running?";
            ErrorLabel.IsVisible = true;
        }
    }

    // ─── Delete deck ──────────────────────────────────────────

    private async void OnDeleteDeckClicked(object? sender, EventArgs e)
    {
        if (_deck == null) return;

        bool confirm = await DisplayAlert(
            "Delete Deck",
            $"Are you sure you want to delete \"{_deck.Title}\"? This cannot be undone.",
            "Delete", "Cancel");

        if (!confirm) return;

        var success = await _deckService.DeleteDeckAsync(_deck.Id!);

        if (success)
            await Shell.Current.GoToAsync("//HomePage");
        else
        {
            ErrorLabel.Text = "Failed to delete deck.";
            ErrorLabel.IsVisible = true;
        }
    }

    // ─── Exit ─────────────────────────────────────────────────

    private async void OnExitClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HomePage");
    }
}