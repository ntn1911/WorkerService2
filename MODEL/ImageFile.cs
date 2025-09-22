using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Model
{
    public class ImageFile
    {
        public int Id { get; set; }

        // Tên file gốc (vd: anh1.png)
        public string FileName { get; set; } = string.Empty;

        // Đường dẫn trên máy vật lý
        public string LocalPath { get; set; } = string.Empty;

        // Link public sau khi upload Supabase
        public string? StorageUrl { get; set; }

        // Đã upload hay chưa
        public bool IsUploaded { get; set; }

        // Ngày tạo
        public DateTime CreatedAt { get; set; }

        // Ngày giờ upload thành công
        public DateTime? UploadedAt { get; set; }
    }
}
