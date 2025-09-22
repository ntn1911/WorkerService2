using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IImageService
    {
        /// <summary>
        /// Quét thư mục và thêm các file ảnh mới vào database
        /// </summary>
        /// <param name="folderPath">Đường dẫn thư mục</param>
        Task ScanImageFolderAsync(string folderPath);

        /// <summary>
        /// Lấy danh sách các ảnh chưa upload
        /// </summary>
        Task<List<ImageFile>> GetPendingImagesAsync();

        /// <summary>
        /// Upload tất cả ảnh pending lên Supabase và cập nhật trạng thái
        /// </summary>
        Task UploadPendingImagesAsync();
    }
}
