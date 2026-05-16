using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlashCardz.Services;

namespace FlashCardz.Pages;

[QueryProperty(nameof(DeckId), "deckId")]
public partial class LearnPage : ContentPage
{
    private readonly DeckService _deckService = new();
    private DeckModel? _deck;
    private List<CardModel> _cards = new();
    private int _currentIndex = 0;
    private bool _isShowingFront = true;
    private int _correctCount = 0;
    private int _wrongCount = 0;

    public string? DeckId { get; set; }

    public LearnPage()
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

        if (_deck == null || _deck.Cards.Count == 0)
        {
            await DisplayAlert("Oops!", "This deck has no cards to study.", "OK");
            await Shell.Current.GoToAsync("//HomePage");
            return;
        }

        DeckTitleLabel.Text = _deck.Title;
        _cards = new List<CardModel>(_deck.Cards);
        _currentIndex = 0;
        _correctCount = 0;
        _wrongCount = 0;

        ShowCard();
    }

    // ─── Card display ─────────────────────────────────────────

    private void ShowCard()
    {
        if (_cards.Count == 0) return;

        var card = _cards[_currentIndex];

        FrontLabel.Text = card.Front;
        BackLabel.Text = card.Back;

        // Always show front first
        _isShowingFront = true;
        CardFront.Rotation = 0;
        CardBack.Rotation = 0;

        UpdateCounter();
    }

    private void UpdateCounter()
    {
        CardCounterLabel.Text = $"Card #: {_currentIndex + 1}/{_cards.Count}";
    }

    // ─── Flip animation ───────────────────────────────────────

    private async void OnCardTapped(object? sender, TappedEventArgs e)
    {
        if (_isShowingFront)
        {
            // Flip to back
            await CardFront.RotateYTo(90, 150, Easing.CubicIn);
            CardFront.IsVisible = false;
            CardBack.IsVisible = true;
            CardBack.RotationY = -90;
            await CardBack.RotateYTo(0, 150, Easing.CubicOut);
        }
        else
        {
            // Flip back to front
            await CardBack.RotateYTo(90, 150, Easing.CubicIn);
            CardBack.IsVisible = false;
            CardFront.IsVisible = true;
            CardFront.RotationY = -90;
            await CardFront.RotateYTo(0, 150, Easing.CubicOut);
        }

        _isShowingFront = !_isShowingFront;
    }

    // ─── Wrong / Correct ──────────────────────────────────────

    private async void OnWrongClicked(object? sender, EventArgs e)
    {
        _wrongCount++;
        await NextCard();
    }

    private async void OnCorrectClicked(object? sender, EventArgs e)
    {
        _correctCount++;
        await NextCard();
    }

    private async Task NextCard()
    {
        // Slide out
        await CardGrid.TranslateTo(-400, 0, 200, Easing.CubicIn);

        _currentIndex++;

        if (_currentIndex >= _cards.Count)
        {
            CardGrid.TranslationX = 0;
            ShowSummary();
            return;
        }

        // Snap to right side instantly (off-screen)
        CardGrid.TranslationX = 400;
        CardGrid.TranslationY = 0;

        // Load new card content while it's off-screen
        ShowCard();

        // Slide in from right
        await CardGrid.TranslateTo(0, 0, 200, Easing.CubicOut);
    }

    // ─── Summary ──────────────────────────────────────────────

    private void ShowSummary()
    {
        var total = _cards.Count;
        var percentage = (int)Math.Round((double)_correctCount / total * 100);

        ScoreLabel.Text = $"{percentage}%  —  {_correctCount}/{total} correct";
        ScoreBreakdownLabel.Text =
            $"✓ Correct: {_correctCount}     ✗ Wrong: {_wrongCount}";

        SummaryOverlay.IsVisible = true;
    }

    private async void OnTryAgainClicked(object? sender, EventArgs e)
    {
        SummaryOverlay.IsVisible = false;
        _currentIndex = 0;
        _correctCount = 0;
        _wrongCount = 0;

        // Shuffle cards for a fresh session
        _cards = _cards.OrderBy(_ => Guid.NewGuid()).ToList();
        ShowCard();
    }

    private async void OnDoneClicked(object? sender, EventArgs e)
    {
        SummaryOverlay.IsVisible = false;
        await Shell.Current.GoToAsync("//HomePage");
    }

    // ─── Back ─────────────────────────────────────────────────

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HomePage");
    }
}