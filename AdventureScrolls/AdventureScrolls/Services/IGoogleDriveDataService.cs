using System.Threading.Tasks;

namespace AdventureScrolls.Services
{
    public interface IGoogleDriveDataService
    {
        Task<bool> DownloadScrollLibrary();
        Task<bool> UploadScrollLibrary();
    }
}