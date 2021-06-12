using System;
using System.IO;
using System.Threading.Tasks;

using FontStashSharp;

namespace Shimakaze.Framework
{
    public static class FontSystemExtension
    {
        public static void AddFont(this FontSystem @this, params string[] filePath)
        {
            @this.AddFont(Path.Combine(filePath));
        }

        public static void AddFont(this FontSystem @this, string filePath)
        {
            @this.AddFont(File.ReadAllBytes(filePath));
        }

        public static Task AddFontAsync(this FontSystem @this, params string[] filePath)
        {
            return @this.AddFontAsync(Path.Combine(filePath));
        }

        public static async Task AddFontAsync(this FontSystem @this, string filePath)
        {
            using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] buffer = new byte[fs.Length];
            await fs.ReadAsync(buffer.AsMemory());
            @this.AddFont(buffer);
        }
    }
}
