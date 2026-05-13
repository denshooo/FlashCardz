using FlashCardz.API.Models;
using FlashCardz.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlashCardz.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DeckController : ControllerBase
{
    private readonly DeckService _deckService;

    public DeckController(DeckService deckService)
    {
        _deckService = deckService;
    }

    // GET api/deck
    [HttpGet]
    public async Task<IActionResult> GetDecks()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var decks = await _deckService.GetByUserIdAsync(userId);
        return Ok(decks);
    }

    // POST api/deck
    [HttpPost]
    public async Task<IActionResult> CreateDeck([FromBody] DeckRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var deck = new Deck
        {
            UserId = userId,
            Title = request.Title,
            Cards = request.Cards ?? new List<Card>()
        };

        await _deckService.CreateAsync(deck);
        return Ok(deck);
    }

    // PUT api/deck/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDeck(string id, [FromBody] DeckRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var existing = await _deckService.GetByIdAsync(id);

        if (existing == null) return NotFound();
        if (existing.UserId != userId) return Forbid();

        existing.Title = request.Title;
        existing.Cards = request.Cards ?? existing.Cards;

        await _deckService.UpdateAsync(id, existing);
        return Ok(existing);
    }

    // DELETE api/deck/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDeck(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var existing = await _deckService.GetByIdAsync(id);

        if (existing == null) return NotFound();
        if (existing.UserId != userId) return Forbid();

        await _deckService.DeleteAsync(id);
        return Ok(new { message = "Deck deleted." });
    }
}

public class DeckRequest
{
    public string Title { get; set; } = string.Empty;
    public List<Card>? Cards { get; set; }
}