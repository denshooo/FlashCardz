using FlashCardz.Services;

namespace FlashCardz.Pages;

public partial class CreateDeckPage : ContentPage
{
    private readonly DeckService _deckService = new();
    private readonly List<(Entry Front, Entry Back)> _cardEntries = new();

    public CreateDeckPage()
    {
        InitializeComponent();
        AddCardRow(); // start with one empty card
    }

    // ─── Add a card row ───────────────────────────────────────

    private void AddCardRow()
{
    int cardNumber = _cardEntries.Count + 1;

    var frontEntry = new Entry
    {
        Placeholder = "Front (question)",
        FontFamily = "Handwritten",
        FontSize = 14,
        TextColor = Color.FromArgb("#1A1A1A"),
        PlaceholderColor = Color.FromArgb("#888888"),
        BackgroundColor = Colors.Transparent
    };

    var backEntry = new Entry
    {
        Placeholder = "Back (answer)",
        FontFamily = "Handwritten",
        FontSize = 14,
        TextColor = Color.FromArgb("#1A1A1A"),
        PlaceholderColor = Color.FromArgb("#888888"),
        BackgroundColor = Colors.Transparent
    };

    // Wobbly input borders
    var frontBorder = new Controls.WobblyBorder
    {
        HeightRequest = 48,
        WobblyFillColor = Colors.White,
        Content = frontEntry
    };

    var backBorder = new Controls.WobblyBorder
    {
        HeightRequest = 48,
        WobblyFillColor = Colors.White,
        Content = backEntry
    };

    // Delete button
    var deleteBtn = new Controls.WobblyButton
    {
        Text = "✕",
        WidthRequest = 40,
        HeightRequest = 36,
        IsDanger = true
    };

    var cardIndex = _cardEntries.Count;
    deleteBtn.Clicked += (s, e) => RemoveCardRow(cardIndex);

    var headerRow = new Grid
    {
        ColumnDefinitions = new ColumnDefinitionCollection
        {
            new ColumnDefinition { Width = GridLength.Star },
            new ColumnDefinition { Width = GridLength.Auto }
        }
    };

    var cardLabel = new Label
    {
        Text = $"Card {cardNumber}",
        FontFamily = "Handwritten",
        FontSize = 14,
        FontAttributes = FontAttributes.Bold,
        TextColor = Color.FromArgb("#1A1A1A"),
        VerticalOptions = LayoutOptions.Center
    };

    Grid.SetColumn(cardLabel, 0);
    Grid.SetColumn(deleteBtn, 1);
    headerRow.Children.Add(cardLabel);
    headerRow.Children.Add(deleteBtn);

    // Wobbly card container
    var cardContainer = new Controls.WobblyBorder
    {
        WobblyFillColor = Color.FromArgb("#FAFAFA"),
        Content = new VerticalStackLayout
        {
            Spacing = 10,
            Padding = new Thickness(4),
            Children = { headerRow, frontBorder, backBorder }
        }
    };

    _cardEntries.Add((frontEntry, backEntry));
    CardList.Children.Add(cardContainer);
}

    private void RemoveCardRow(int index)
    {
        if (_cardEntries.Count <= 1)
        {
            ErrorLabel.Text = "A deck must have at least one card.";
            ErrorLabel.IsVisible = true;
            return;
        }

        ErrorLabel.IsVisible = false;
        _cardEntries.RemoveAt(index);
        CardList.Children.RemoveAt(index);

        // Renumber remaining cards
        for (int i = 0; i < CardList.Children.Count; i++)
        {
            if (CardList.Children[i] is Controls.WobblyBorder wb &&
                wb.Content is VerticalStackLayout vsl &&
                vsl.Children[0] is Grid grid &&
                grid.Children[0] is Label lbl)
            {
                lbl.Text = $"Card {i + 1}";
            }
        }
    }

    // ─── Button handlers ──────────────────────────────────────

    private void OnAddCardClicked(object? sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        AddCardRow();
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleEntry.Text))
        {
            ErrorLabel.Text = "Please enter a deck title.";
            ErrorLabel.IsVisible = true;
            return;
        }

        // Build cards list
        var cards = _cardEntries
            .Where(c => !string.IsNullOrWhiteSpace(c.Front.Text) ||
                        !string.IsNullOrWhiteSpace(c.Back.Text))
            .Select(c => new CardModel
            {
                Front = c.Front.Text ?? "",
                Back = c.Back.Text ?? ""
            })
            .ToList();

        var (success, deck) = await _deckService.CreateDeckAsync(TitleEntry.Text.Trim(), cards);

        if (success)
        {
            await DisplayAlert("Done!", "Deck created successfully.", "OK");
            await Shell.Current.GoToAsync("//HomePage");
        }
        else
        {
            ErrorLabel.Text = "Failed to create deck. Is the API running?";
            ErrorLabel.IsVisible = true;
        }
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HomePage");
    }
}