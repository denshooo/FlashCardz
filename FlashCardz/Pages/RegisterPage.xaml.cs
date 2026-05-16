using FlashCardz.Services;

namespace FlashCardz.Pages;

public partial class RegisterPage : ContentPage
{
    private readonly AuthService _authService;

    public RegisterPage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private async void OnRegisterClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(UsernameEntry.Text) ||
            string.IsNullOrWhiteSpace(PasswordEntry.Text) ||
            string.IsNullOrWhiteSpace(ConfirmPasswordEntry.Text))
        {
            ErrorLabel.Text = "Please fill in all fields.";
            ErrorLabel.IsVisible = true;
            return;
        }

        if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
        {
            ErrorLabel.Text = "Passwords do not match.";
            ErrorLabel.IsVisible = true;
            return;
        }

        ErrorLabel.IsVisible = false;

        var (success, message) =
            await _authService.RegisterAsync(UsernameEntry.Text, PasswordEntry.Text);

        if (success)
        {
            await DisplayAlert("Success!", "Account created. Please log in.", "OK");
            await Shell.Current.GoToAsync("//WelcomePage");
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