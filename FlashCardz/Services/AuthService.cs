using System.Net.Http.Json;

namespace FlashCardz.Services;

public class AuthService
{
    private readonly HttpClient _http;

    // Use 10.0.2.2 on Android emulator — it maps to your PC's localhost
    private const string BaseUrl = "http://10.0.2.2:5170/api/auth";

    public AuthService()
    {
        _http = new HttpClient();
    }

    public async Task<(bool Success, string Message)> RegisterAsync(
        string username, string password)
    {
        try
        {
            var response = await _http.PostAsJsonAsync($"{BaseUrl}/register", new
            {
                username,
                password
            });

            var result = await response.Content.ReadFromJsonAsync<MessageResponse>();

            return response.IsSuccessStatusCode
                ? (true, result?.Message ?? "Registered successfully.")
                : (false, result?.Message ?? "Registration failed.");
        }
        catch (Exception ex)
        {
            return (false, $"Connection error: {ex.Message}");
        }
    }

    public async Task<(bool Success, string Message, string? Token, string? Username)> LoginAsync(
        string username, string password)
    {
        try
        {
            var response = await _http.PostAsJsonAsync($"{BaseUrl}/login", new
            {
                username,
                password
            });

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                return (true, "Login successful.", result?.Token, result?.Username);
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<MessageResponse>();
                return (false, error?.Message ?? "Login failed.", null, null);
            }
        }
        catch (Exception ex)
        {
            return (false, $"Connection error: {ex.Message}", null, null);
        }
    }

    // Response models
    private class MessageResponse
    {
        public string? Message { get; set; }
    }

    private class LoginResponse
    {
        public string? Token { get; set; }
        public string? Username { get; set; }
        public string? UserId { get; set; }
    }
}