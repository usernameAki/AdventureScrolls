using Google.Apis.Drive.v3;
using System.Threading.Tasks;

namespace AdventureScrolls.Services
{
    public interface IGoogleUserAuthenticationService
    {
        DriveService driveService { get; set; }

        Task LoginToGoogleDrive();
    }
}