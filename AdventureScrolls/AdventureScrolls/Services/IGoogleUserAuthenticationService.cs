using Google.Apis.Drive.v3;
using System.Threading.Tasks;

namespace AdventureScrolls.Services
{
    public interface IGoogleUserAuthenticationService
    {
        string userName { get; }
        DriveService driveService { get; set; }

        Task<bool> LoginToGoogleDrive();
        Task<bool> CheckTokenValidity();
    }
}