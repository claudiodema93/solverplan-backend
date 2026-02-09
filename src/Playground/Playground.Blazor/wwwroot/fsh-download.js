// FSH Download Utilities
// Provides client-side file download functionality

window.FshDownload = {
    /**
     * Download data as a file
     * @param {string} filename - The name of the file to download
     * @param {string} contentType - MIME type (e.g., 'text/csv', 'application/pdf')
     * @param {string} data - The file content (base64 for binary, plain text for CSV)
     */
    downloadFile: function (filename, contentType, data) {
        try {
            let blob;

            if (contentType === 'text/csv' || contentType === 'text/plain') {
                // For text-based files
                blob = new Blob([data], { type: contentType });
            } else {
                // For binary files (PDF, Excel) - data should be base64
                const byteCharacters = atob(data);
                const byteNumbers = new Array(byteCharacters.length);
                for (let i = 0; i < byteCharacters.length; i++) {
                    byteNumbers[i] = byteCharacters.charCodeAt(i);
                }
                const byteArray = new Uint8Array(byteNumbers);
                blob = new Blob([byteArray], { type: contentType });
            }

            // Create download link
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = filename;

            // Trigger download
            document.body.appendChild(link);
            link.click();

            // Cleanup
            document.body.removeChild(link);
            window.URL.revokeObjectURL(url);

            console.log(`[FshDownload] Downloaded file: ${filename}`);
            return true;
        } catch (error) {
            console.error(`[FshDownload] Failed to download file: ${filename}`, error);
            return false;
        }
    },

    /**
     * Download CSV data
     * @param {string} filename - The name of the CSV file
     * @param {string} csvData - CSV content as string
     */
    downloadCsv: function (filename, csvData) {
        return this.downloadFile(filename, 'text/csv;charset=utf-8;', csvData);
    },

    /**
     * Download JSON data as a file
     * @param {string} filename - The name of the JSON file
     * @param {string} jsonData - JSON content as string
     */
    downloadJson: function (filename, jsonData) {
        return this.downloadFile(filename, 'application/json;charset=utf-8;', jsonData);
    },

    /**
     * Copy text to clipboard
     * @param {string} text - Text to copy
     */
    copyToClipboard: async function (text) {
        try {
            if (navigator.clipboard && navigator.clipboard.writeText) {
                await navigator.clipboard.writeText(text);
                console.log('[FshDownload] Text copied to clipboard');
                return true;
            } else {
                // Fallback for older browsers
                const textArea = document.createElement('textarea');
                textArea.value = text;
                textArea.style.position = 'fixed';
                textArea.style.left = '-999999px';
                document.body.appendChild(textArea);
                textArea.select();
                const success = document.execCommand('copy');
                document.body.removeChild(textArea);
                console.log('[FshDownload] Text copied to clipboard (fallback)');
                return success;
            }
        } catch (error) {
            console.error('[FshDownload] Failed to copy to clipboard', error);
            return false;
        }
    }
};
