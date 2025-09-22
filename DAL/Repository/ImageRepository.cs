using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using DAL.Interfaces;
using Model;

namespace DAL.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly string _connectionString;

        public ImageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task AddImageAsync(ImageFile image)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"INSERT INTO Images (FileName, LocalPath, IsUploaded, CreatedAt)
                        VALUES (@FileName, @LocalPath, 0, GETDATE());";
            await conn.ExecuteAsync(sql, image);
        }

        public async Task<List<ImageFile>> GetPendingImagesAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = "SELECT * FROM Images WHERE IsUploaded = 0";
            var result = await conn.QueryAsync<ImageFile>(sql);
            return result.ToList();
        }

        public async Task<List<ImageFile>> GetAllImagesAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = "SELECT * FROM Images";
            var result = await conn.QueryAsync<ImageFile>(sql);
            return result.ToList();
        }

        public async Task UpdateUploadedAsync(int id, string storageUrl)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"UPDATE Images 
                        SET IsUploaded = 1, StorageUrl = @StorageUrl, UploadedAt = GETDATE()
                        WHERE Id = @Id";
            await conn.ExecuteAsync(sql, new { Id = id, StorageUrl = storageUrl });
        }
    }
}

