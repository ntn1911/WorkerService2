using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IImageRepository
    {
        Task AddImageAsync(ImageFile image);
        Task<List<ImageFile>> GetPendingImagesAsync();
        Task UpdateUploadedAsync(int id, string storageUrl);

        // Thêm method này để ImageService gọi được
        Task<List<ImageFile>> GetAllImagesAsync();
    }
}
