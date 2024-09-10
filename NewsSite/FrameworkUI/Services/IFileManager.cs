using DomainModel.Comon;

namespace NewsSite.FrameworkUI.Services
{
    public interface IFileManager
    {
        bool RemoveFile(string path);
        string ToPhysicalAddress(string FileName, string Foldername);
        OperationResult SaveFile(IFormFile file, string folderName);
        OperationResult ValidateFileSize(IFormFile file, long MinCapacity, long MaxCapacity);
        bool ValidateFileName(string FileName);
        string ToUniquieFileName(string fileName);
        string ToRelativeAddress(string UnqieFileName, string Folder);
    }
}
