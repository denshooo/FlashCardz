using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlashCardz.Services;

namespace FlashCardz.Pages;

public partial class ProfilePage : ContentPage
{
    private readonly DeckService _deckService;

    public ProfilePage(DeckService deckService)
    {
        InitializeComponent();
        _deckService = deckService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadProfile();
    }

    // ─── Load ─────────────────────────────────────────────────

    private async Task LoadProfile()
    {
        // Read username saved to SecureStorage at login
        var username = await SecureStorage.GetAsync("username") ?? "Unknown";
        UsernameLabel.Text = username;

        // Avatar = first letter of username, uppercased
        AvatarLabel.Text = username.Length > 0
            ? username[0].ToString().ToUpper()
            : "?";

        // Deck count — reuse DeckService, no extra API call needed
        var decks = await _deckService.GetDecksAsync();
        DeckCountLabel.Text = decks.Count.ToString();

        // Streak — read from SecureStorage if saved at login,
        // otherwise show 0. Wire this up once your API returns it.
        var streakRaw = await SecureStorage.GetAsync("learning_streak");
        StreakLabel.Text = streakRaw ?? "0";

        // Study history — placeholder until API supports it
        HistoryLabel.Text = decks.Count > 0
            ? $"You have {decks.Count} deck{(decks.Count == 1 ? "" : "s")} ready to study."
            : "No sessions recorded yet.";
    }

    // ─── Logout ───────────────────────────────────────────────

    private async void OnLogOutClicked(object? sender, EventArgs e)
    {
        bool confirmed = await DisplayAlert(
            "Log Out",
            "Are you sure you want to log out?",
            "Log Out", "Cancel");

        if (!confirmed) return;

        // Clear all stored credentials
        SecureStorage.Remove("auth_token");
        SecureStorage.Remove("username");
        SecureStorage.Remove("user_id");
        SecureStorage.Remove("learning_streak");

        // Navigate to WelcomePage and clear the back stack
        await Shell.Current.GoToAsync("//WelcomePage");
    }
}