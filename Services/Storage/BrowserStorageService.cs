
using System.Text.Json;
using Microsoft.JSInterop;

namespace LifeDeck.Services.Storage;

public sealed class BrowserStorageService
{
    private readonly IJSRuntime _js;

    public BrowserStorageService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await _js.InvokeAsync<string?>(
            "localStorage.getItem", key);

        if (string.IsNullOrWhiteSpace(json))
            return default;

        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetAsync<T>(string key, T value)
    {
        var json = JsonSerializer.Serialize(value);

        await _js.InvokeVoidAsync(
            "localStorage.setItem", key, json);
    }

    public async Task<int> CountAsync(string key)
    {
        var records = await GetAsync<List<JsonElement>>(key);
        return records?.Count ?? 0;
    }
}

public static class StorageKeys
{
    public const string Dives = "lifedeck:dives:v1";
    public const string Todos = "lifedeck:todos:v1";
    public const string Notes = "lifedeck:notes:v1";
    public const string Appointments = "lifedeck:appointments:v1";
    public const string Transactions = "lifedeck:transactions:v1";
    public const string BlogPosts = "lifedeck:blogposts:v1";
}
