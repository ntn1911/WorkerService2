using BLL.Interfaces;
using DAL.Interfaces;
using Microsoft.Extensions.Logging;
using Model;
using SupabaseFileOptions = Supabase.Storage.FileOptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _imageRepository;
        private readonly Supabase.Client _supabaseClient;
        private readonly ILogger<ImageService> _logger;
        private readonly string _bucketId = "uploadimage";

        public ImageService(IImageRepository imageRepository, Supabase.Client supabaseClient, ILogger<ImageService> logger)
        {
            _imageRepository = imageRepository;
            _supabaseClient = supabaseClient;
            _logger = logger;
        }

        public async Task ScanImageFolderAsync(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                _logger.LogWarning("Thư mục không tồn tại, tạo mới: {folderPath}", folderPath);
                Directory.CreateDirectory(folderPath);
                return;
            }

            var imageExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".webp" };
            var files = Directory.GetFiles(folderPath)
                .Where(f => imageExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                .ToList();

            _logger.LogInformation("Tìm thấy {count} file ảnh trong thư mục: {folderPath}", files.Count, folderPath);
            if (!files.Any()) return;

            var existingImages = await _imageRepository.GetAllImagesAsync();
            var existingPaths = existingImages.Select(img => img.LocalPath).ToHashSet();

            int addedCount = 0;
            foreach (var filePath in files)
            {
                if (existingPaths.Contains(filePath)) continue;

                if (!IsFileAccessible(filePath))
                {
                    _logger.LogWarning("File không thể truy cập: {filePath}", filePath);
                    continue;
                }

                var image = new ImageFile
                {
                    FileName = Path.GetFileName(filePath),
                    LocalPath = filePath,
                    IsUploaded = false,
                    CreatedAt = DateTime.Now
                };

                await _imageRepository.AddImageAsync(image);
                existingPaths.Add(filePath);
                addedCount++;

                _logger.LogInformation("Đã thêm file {fileName} vào DB", image.FileName);
            }

            _logger.LogInformation("Hoàn thành quét thư mục. Thêm {addedCount} file mới", addedCount);
        }

        public async Task<List<ImageFile>> GetPendingImagesAsync() => await _imageRepository.GetPendingImagesAsync();

        public async Task UploadPendingImagesAsync()
        {
            var pendingImages = await GetPendingImagesAsync();
            var bucket = _supabaseClient.Storage.From(_bucketId);
            var existingFiles = await bucket.List(""); // lấy tất cả file trên Supabase

            foreach (var img in pendingImages)
            {
                try
                {
                    // Check nếu file đã tồn tại trên Supabase
                    if (existingFiles.Any(f => f.Name == img.FileName))
                    {
                        _logger.LogInformation("File {fileName} đã tồn tại trên Supabase, bỏ qua upload", img.FileName);
                        var url = bucket.GetPublicUrl(img.FileName);
                        await _imageRepository.UpdateUploadedAsync(img.Id, url);
                        continue;
                    }

                    var bytes = await File.ReadAllBytesAsync(img.LocalPath);
                    await bucket.Upload(bytes, img.FileName, new SupabaseFileOptions
                    {
                        ContentType = GetMimeType(img.FileName),
                        Upsert = false
                    });

                    var publicUrl = bucket.GetPublicUrl(img.FileName);
                    await _imageRepository.UpdateUploadedAsync(img.Id, publicUrl);
                    _logger.LogInformation("Upload thành công {fileName}. URL: {url}", img.FileName, publicUrl);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Upload thất bại {fileName}", img.FileName);
                }
            }
        }

        private bool IsFileAccessible(string filePath)
        {
            try
            {
                using var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return true;
            }
            catch { return false; }
        }

        private static string GetMimeType(string fileName)
        {
            return Path.GetExtension(fileName)?.ToLowerInvariant() switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
}
