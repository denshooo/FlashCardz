using FlashCardz.Services;

namespace FlashCardz.Pages;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _authService;

    // AuthService injected via DI (registered in MauiProgram.cs)
    public LoginPage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(UsernameEntry.Text) ||
            string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            ErrorLabel.Text = "Please fill in all fields.";
            ErrorLabel.IsVisible = true;
            return;
        }

        ErrorLabel.IsVisible = false;

        var (success, message, token, username, userId, learningStreak) =
            await _authService.LoginAsync(
                UsernameEntry.Text.Trim(),
                PasswordEntry.Text);

        if (success)
        {
            // Persist everything needed across pages
            await SecureStorage.SetAsync("auth_token",       token!);
            await SecureStorage.SetAsync("username",         username!);
            await SecureStorage.SetAsync("user_id",          userId ?? string.Empty);
            await SecureStorage.SetAsync("learning_streak",  learningStreak.ToString());

            await Shell.Current.GoToAsync("//HomePage");
        }
        else
        {
            ErrorLabel.Text = message;
            ErrorLabel.IsVisible = true;
        }
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//WelcomePage");
    }
}