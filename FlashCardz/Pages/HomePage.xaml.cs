using FlashCardz.Services;

namespace FlashCardz.Pages;

public partial class HomePage : ContentPage
{
    private readonly DeckService _deckService = new();
    private List<DeckModel> _allDecks = new();
    private DeckModel? _selectedDeck;

    public HomePage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadDecks();
    }

    // ─── Load ─────────────────────────────────────────────────

    private async Task LoadDecks()
    {
        _allDecks = await _deckService.GetDecksAsync();
        RenderDecks(_allDecks);
    }

    private void RenderDecks(List<DeckModel> decks)
    {
        DeckList.Children.Clear();

        if (decks.Count == 0)
        {
            DeckList.Children.Add(new Label
            {
                Text = "No decks yet. Tap + to create one!",
                FontFamily = "Handwritten",
                TextColor = Color.FromArgb("#888888"),
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 40, 0, 0)
            });
            return;
        }

        foreach (var deck in decks)
            DeckList.Children.Add(BuildDeckCard(deck));
    }

    private View BuildDeckCard(DeckModel deck)
    {
        var titleLabel = new Label
        {
            Text = deck.Title,
            FontFamily = "Handwritten",
            FontSize = 17,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#1A1A1A")
        };

        var previewLabel = new Label
        {
            Text = deck.Cards.Count > 0
                ? $"{deck.Cards.Count} card{(deck.Cards.Count == 1 ? "" : "s")}"
                : "No cards yet",
            FontFamily = "Handwritten",
            FontSize = 13,
            TextColor = Color.FromArgb("#888888")
        };

        var learnBtn = new Controls.WobblyButton
        {
            Text = "Learn",
            WidthRequest = 90,
            HeightRequest = 36
        };
        learnBtn.Clicked += (s, e) => OnLearnClicked(deck);

        var editBtn = new Controls.WobblyButton
        {
            Text = "Edit",
            WidthRequest = 90,
            HeightRequest = 36
        };
        editBtn.Clicked += (s, e) => OnEditClicked(deck);

        var btnRow = new Grid
        {
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star }
            },
            ColumnSpacing = 8
        };
        Grid.SetColumn(learnBtn, 0);
        Grid.SetColumn(editBtn, 1);
        btnRow.Children.Add(learnBtn);
        btnRow.Children.Add(editBtn);

        var cardContent = new VerticalStackLayout
        {
            Spacing = 8,
            Children = { titleLabel, previewLabel, btnRow }
        };

        var card = new Border
        {
            Style = (Style)Application.Current!.Resources["SketchCard"],
            Content = cardContent
        };

        var tap = new TapGestureRecognizer();
        tap.Tapped += (s, e) => ShowPopup(deck);
        card.GestureRecognizers.Add(tap);

        return card;
    }

    // ─── Search ───────────────────────────────────────────────

    private void OnSearchChanged(object sender, TextChangedEventArgs e)
    {
        var query = e.NewTextValue?.ToLower() ?? "";
        var filtered = _allDecks
            .Where(d => d.Title.ToLower().Contains(query))
            .ToList();
        RenderDecks(filtered);
    }

    // ─── Popup ────────────────────────────────────────────────

    private void ShowPopup(DeckModel deck)
    {
        _selectedDeck = deck;
        PopupTitle.Text = deck.Title;
        PopupContent.Text = deck.Cards.Count > 0
            ? $"This deck has {deck.Cards.Count} card(s). Ready to study?"
            : "This deck has no cards yet. Edit it to add some!";
        PopupOverlay.IsVisible = true;
    }

    private void OnPopupBackClicked(object? sender, EventArgs e)
    {
        PopupOverlay.IsVisible = false;
    }

    private async void OnPopupContinueClicked(object? sender, EventArgs e)
    {
        PopupOverlay.IsVisible = false;
        if (_selectedDeck != null)
            await Shell.Current.GoToAsync(
                $"LearnPage?deckId={_selectedDeck.Id}");
    }

    // ─── Deck actions ─────────────────────────────────────────

    private async void OnLearnClicked(DeckModel deck)
    {
        await Shell.Current.GoToAsync($"LearnPage?deckId={deck.Id}");
    }

    private async void OnEditClicked(DeckModel deck)
    {
        await Shell.Current.GoToAsync($"EditDeckPage?deckId={deck.Id}");
    }

    // ─── Bottom nav ───────────────────────────────────────────

    private void OnHomeClicked(object sender, EventArgs e) { }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("CreateDeckPage");
    }

    private async void OnProfileClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("ProfilePage");
    }
}