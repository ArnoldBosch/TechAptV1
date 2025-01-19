// Copyright © 2025 Always Active Technologies PTY Ltd

using Microsoft.JSInterop;

public static class FileUtil
{
    public static async Task SaveAs(IJSRuntime jsRuntime, string fileName, byte[] fileBytes)
    {
        var base64 = Convert.ToBase64String(fileBytes);
        await jsRuntime.InvokeVoidAsync("FileUtil.saveAs", fileName, base64);
    }
}
