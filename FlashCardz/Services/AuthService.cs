using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace FlashCardz.Services;

public class AuthService
{
    private readonly HttpClient _http;

#if DEBUG
    private const string BaseUrl = "http://10.0.2.2:5170/api/auth";
#else
    private const string BaseUrl = "https://bigdeckapi-production.up.railway.app/api/auth";
#endif
    
    // Injected as singleton from MauiProgram.cs
    public AuthService(HttpClient http)
    {
        _http = http;
    }

    // ─── Register ─────────────────────────────────────────────

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
                ? (true,  result?.Message ?? "Registered successfully.")
                : (false, result?.Message ?? "Registration failed.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[AuthService] RegisterAsync failed: {ex.Message}");
            return (false, $"Connection error: {ex.Message}");
        }
    }

    // ─── Login ────────────────────────────────────────────────

    // Returns token, username, userId, AND learningStreak so the
    // login page can persist all of them to SecureStorage in one shot.
    public async Task<(bool Success, string Message, string? Token,
        string? Username, string? UserId, int LearningStreak)> LoginAsync(
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
                var result = await response.Content
                    .ReadFromJsonAsync<LoginResponse>();

                return (true, "Login successful.",
                    result?.Token,
                    result?.Username,
                    result?.UserId,
                    result?.LearningStreak ?? 0);
            }
            else
            {
                var error = await response.Content
                    .ReadFromJsonAsync<MessageResponse>();

                System.Diagnostics.Debug.WriteLine(
                    $"[AuthService] LoginAsync HTTP {(int)response.StatusCode}");

                return (false, error?.Message ?? "Login failed.",
                    null, null, null, 0);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[AuthService] LoginAsync failed: {ex.Message}");
            return (false, $"Connection error: {ex.Message}",
                null, null, null, 0);
        }
    }

    // ─── Response models ──────────────────────────────────────

    private class MessageResponse
    {
        public string? Message { get; set; }
    }

    private class LoginResponse
    {
        public string? Token          { get; set; }
        public string? Username       { get; set; }
        public string? UserId         { get; set; }

        // If your API doesn't return this yet, it'll just default to 0
        public int LearningStreak     { get; set; }
    }
}