using FlashCardz.Services;

namespace FlashCardz.Pages;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _authService = new();

    public LoginPage()
    {
        InitializeComponent();
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

        // Show loading state
        ErrorLabel.IsVisible = false;

        var (success, message, token, username) =
            await _authService.LoginAsync(UsernameEntry.Text, PasswordEntry.Text);

        if (success)
        {
            // Store token and username securely
            await SecureStorage.SetAsync("auth_token", token!);
            await SecureStorage.SetAsync("username", username!);

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