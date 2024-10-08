﻿namespace ChronoDialShop.Extentions;

public static class FileExtention
{
    public static async Task<string> SaveFileAsync(this IFormFile file, string root, string client, string folderName)
    {
        string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
        string path = Path.Combine(root, client, folderName, uniqueFileName);

        using FileStream fs = new FileStream(path, FileMode.Create);

        await file.CopyToAsync(fs);

        return uniqueFileName;
    }

    public static bool CheckFileType(this IFormFile file, string fileType)
    {
        if (file.ContentType.Contains(fileType))
        {
            return true;
        }
        return false;
    }

    public static bool CheckFileSize(this IFormFile file, int fileSize)
    {
        if (file.Length > fileSize * 1024 * 1024)
        {
            return false;
        }
        return true;
    }

    public static void DeleteFile(this IFormFile file, string root, string client, string folderName, string fileName)
    {
        string path = Path.Combine(root, client, folderName, fileName);
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }
    }
}
