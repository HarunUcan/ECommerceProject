using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.BusinessLayer.Helpers
{
    public static class FileHelper
    {
        private static readonly string _uploadPath = "wwwroot/uploads";
        public static async Task<string> SaveFileAsync(byte[] fileData, string fileName)
        {
            if (fileData.Length == 0)
            {
                return null;
            }

            var path = $"{_uploadPath}/{fileName}";
            await File.WriteAllBytesAsync(path, fileData);
            return path;
        }

        public static void DeleteFiles(List<string> paths)
        {
            if (paths == null) return;

            foreach (var path in paths)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
    }
}
