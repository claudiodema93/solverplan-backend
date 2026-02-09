using Microsoft.JSInterop;
using System.Text.Json;

namespace FSH.Framework.Blazor.UI.Services;

/// <summary>
/// Service for managing filter presets using browser localStorage
/// </summary>
public interface IFilterPresetService
{
    Task<List<FilterPreset<TFilter>>> GetPresetsAsync<TFilter>(string pageKey);
    Task SavePresetAsync<TFilter>(string pageKey, FilterPreset<TFilter> preset);
    Task DeletePresetAsync(string pageKey, string presetId);
}

public class FilterPresetService : IFilterPresetService
{
    private readonly IJSRuntime _js;
    private const string StoragePrefix = "fsh_filter_preset_";

    public FilterPresetService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<List<FilterPreset<TFilter>>> GetPresetsAsync<TFilter>(string pageKey)
    {
        try
        {
            var key = GetStorageKey(pageKey);
            var json = await _js.InvokeAsync<string?>("localStorage.getItem", key);

            if (string.IsNullOrEmpty(json))
                return new List<FilterPreset<TFilter>>();

            var presets = JsonSerializer.Deserialize<List<FilterPreset<TFilter>>>(json);
            return presets ?? new List<FilterPreset<TFilter>>();
        }
        catch
        {
            return new List<FilterPreset<TFilter>>();
        }
    }

    public async Task SavePresetAsync<TFilter>(string pageKey, FilterPreset<TFilter> preset)
    {
        try
        {
            var presets = await GetPresetsAsync<TFilter>(pageKey);

            // Update existing or add new
            var existing = presets.FirstOrDefault(p => p.Id == preset.Id);
            if (existing != null)
            {
                presets.Remove(existing);
            }

            presets.Add(preset);

            var key = GetStorageKey(pageKey);
            var json = JsonSerializer.Serialize(presets);
            await _js.InvokeVoidAsync("localStorage.setItem", key, json);
        }
        catch
        {
            // Silently fail - not critical
        }
    }

    public async Task DeletePresetAsync(string pageKey, string presetId)
    {
        try
        {
            var presets = await GetPresetsAsync<object>(pageKey);
            var toRemove = presets.FirstOrDefault(p => p.Id == presetId);

            if (toRemove != null)
            {
                presets.Remove(toRemove);
                var key = GetStorageKey(pageKey);
                var json = JsonSerializer.Serialize(presets);
                await _js.InvokeVoidAsync("localStorage.setItem", key, json);
            }
        }
        catch
        {
            // Silently fail - not critical
        }
    }

    private static string GetStorageKey(string pageKey) => $"{StoragePrefix}{pageKey}";
}

/// <summary>
/// Represents a saved filter preset
/// </summary>
/// <typeparam name="TFilter">The filter type</typeparam>
public class FilterPreset<TFilter>
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public TFilter Filter { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
