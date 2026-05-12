namespace FlashCardz.Pages;

public partial class HomePage : ContentPage
{
    // Mock deck data for now
    private readonly List<(string Title, string Preview)> _allDecks = new()
    {
        ("Science", "Photosynthesis, mitosis, cell structure..."),
        ("Math", "Derivatives, integrals, limits..."),
        ("History", "World War II, Renaissance, Cold War..."),
        ("English", "Shakespearean terms, grammar rules..."),
    };

    private string? _selectedDeckTitle;

    public HomePage()
    {
        InitializeComponent();
        RenderDecks(_allDecks);
    }

    // ─── Deck Rendering ───────────────────────────────────────

    private void RenderDecks(List<(string Title, string Preview)> decks)
    {
        DeckList.Children.Clear();

        if (decks.Count == 0)
        {
            DeckList.Children.Add(new Label
            {
                Text = "No decks found.",
                Style = (Style)Resources["MutedLabel"],
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 40, 0, 0)
            });
            return;
        }

        foreach (var deck in decks)
            DeckList.Children.Add(BuildDeckCard(deck.Title, deck.Preview));
    }

    private View BuildDeckCard(string title, string preview)
    {
        var titleLabel = new Label
        {
            Text = title,
            FontFamily = "Handwritten",
            FontSize = 17,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#1A1A1A")
        };

        var previewLabel = new Label
        {
            Text = preview,
            FontFamily = "Handwritten",
            FontSize = 13,
            TextColor = Color.FromArgb("#888888"),
            MaxLines = 2,
            LineBreakMode = LineBreakMode.TailTruncation
        };

        var learnBtn = new Controls.WobblyButton
        {
            Text = "Learn",
            WidthRequest = 90,
            HeightRequest = 36
        };
        learnBtn.Clicked += (s, e) => OnLearnClicked(title);

        var editBtn = new Controls.WobblyButton
        {
            Text = "Edit",
            WidthRequest = 90,
            HeightRequest = 36
        };
        editBtn.Clicked += (s, e) => OnEditClicked(title);

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

        // Tap deck card to show preview popup
        var tap = new TapGestureRecognizer();
        tap.Tapped += (s, e) => ShowPopup(title, preview);
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

    private void ShowPopup(string title, string preview)
    {
        _selectedDeckTitle = title;
        PopupTitle.Text = title;
        PopupContent.Text = preview;
        PopupOverlay.IsVisible = true;
    }

    private void OnPopupBackClicked(object? sender, EventArgs e)
    {
        PopupOverlay.IsVisible = false;
    }

    private async void OnPopupContinueClicked(object? sender, EventArgs e)
    {
        PopupOverlay.IsVisible = false;
        await Shell.Current.GoToAsync($"LearnPage?deckTitle={_selectedDeckTitle}");
    }

    // ─── Deck actions ─────────────────────────────────────────

    private async void OnLearnClicked(string deckTitle)
    {
        await Shell.Current.GoToAsync($"LearnPage?deckTitle={deckTitle}");
    }

    private async void OnEditClicked(string deckTitle)
    {
        await Shell.Current.GoToAsync($"EditDeckPage?deckTitle={deckTitle}");
    }

    // ─── Bottom nav ───────────────────────────────────────────

    private void OnHomeClicked(object sender, EventArgs e)
    {
        // Already on home
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("CreateDeckPage");
    }

    private async void OnProfileClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("ProfilePage");
    }
}