using Microsoft.JSInterop;
using System.Text;

namespace FSH.Framework.Blazor.UI.Services;

/// <summary>
/// Service for exporting data to various formats
/// </summary>
public interface IExportService
{
    Task<bool> ExportToCsvAsync<T>(string filename, IEnumerable<T> data, params string[] propertyNames);
    Task<bool> ExportToCsvAsync(string filename, string csvContent);
    Task<bool> CopyToClipboardAsync(string text);
}

public class ExportService : IExportService
{
    private readonly IJSRuntime _js;

    public ExportService(IJSRuntime js)
    {
        _js = js;
    }

    /// <summary>
    /// Export collection to CSV file
    /// </summary>
    public async Task<bool> ExportToCsvAsync<T>(string filename, IEnumerable<T> data, params string[] propertyNames)
    {
        try
        {
            var csv = GenerateCsv(data, propertyNames);
            return await ExportToCsvAsync(filename, csv);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Export CSV string to file
    /// </summary>
    public async Task<bool> ExportToCsvAsync(string filename, string csvContent)
    {
        try
        {
            // Ensure filename has .csv extension
            if (!filename.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                filename += ".csv";

            return await _js.InvokeAsync<bool>("FshDownload.downloadCsv", filename, csvContent);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Copy text to clipboard
    /// </summary>
    public async Task<bool> CopyToClipboardAsync(string text)
    {
        try
        {
            return await _js.InvokeAsync<bool>("FshDownload.copyToClipboard", text);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Generate CSV string from collection
    /// </summary>
    private static string GenerateCsv<T>(IEnumerable<T> data, string[] propertyNames)
    {
        var sb = new StringBuilder();
        var type = typeof(T);

        // Get properties to export
        var properties = propertyNames.Length > 0
            ? type.GetProperties().Where(p => propertyNames.Contains(p.Name)).ToArray()
            : type.GetProperties();

        // Header row
        sb.AppendLine(string.Join(",", properties.Select(p => EscapeCsvField(p.Name))));

        // Data rows
        foreach (var item in data)
        {
            var values = properties.Select(p =>
            {
                var value = p.GetValue(item);
                return EscapeCsvField(value?.ToString() ?? string.Empty);
            });

            sb.AppendLine(string.Join(",", values));
        }

        return sb.ToString();
    }

    /// <summary>
    /// Escape CSV field (handle commas, quotes, newlines)
    /// </summary>
    private static string EscapeCsvField(string field)
    {
        if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }

        return field;
    }
}
