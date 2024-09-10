using DomainModel.Comon;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using NewsSite.FrameworkUI.Services;

namespace NewsSite.FrameworkUI
{
    public class FileManager : IFileManager
    {
        private readonly IHostEnvironment env;
        public FileManager(IHostEnvironment env)
        {
            this.env = env;
        }
        public bool RemoveFile(string path)
        {
            if (path == null && path.ToLower() == "~/pics/noimage.svg")
            {
                return false;
            }
            if (!System.IO.File.Exists(path))
            {
                return false ;
            }
            System.IO.File.Delete(path);
            return true;
        }

        public OperationResult SaveFile(IFormFile file, string folderName)
        {
            OperationResult op = new OperationResult();
            var address = Path.GetFileName(file.FileName);
            string uniqeFile = ToUniquieFileName(address);
            address = ToPhysicalAddress(uniqeFile, folderName);
            FileStream fs = new FileStream(address, FileMode.Create);
            try
            {


                file.CopyTo(fs);

                return op.ToSuccess(uniqeFile);


            }
            catch (Exception ex)
            {

                return op.ToFailed(ex.Message);

            }
            finally
            {
                fs.Close();
                fs.Dispose();
            }

        }

        public string ToPhysicalAddress(string FileName, string Foldername)
        {
            return env.ContentRootPath + @"\wwwroot\" + Foldername + @"\" + FileName;
        }

        public string ToRelativeAddress(string UnqieFileName, string Folder)
        {
            return @"~/" + Folder + @"/" + UnqieFileName;
        }

        public string ToUniquieFileName(string fileName)
        {
            return Guid.NewGuid().ToString().Replace("-", "_") + fileName;
        }

        public bool ValidateFileName(string FileName)
        {
            if (FileName == null)
            {
                return false;
            }
            FileName = FileName.Trim().ToLower();
            if (FileName.Contains(".php") || FileName.Contains(".asp") || FileName.Contains(".ascx"))
            {
                return false;
            }
            return true;
        }

        public OperationResult ValidateFileSize(IFormFile file, long MinCapacity, long MaxCapacity)
        {
            OperationResult op = new OperationResult();
            if (file.Length < MinCapacity || file.Length > MaxCapacity)
            {
                return op.ToFailed("Invalid File Size");
            }
            else
            {
                return op.ToSuccess("File Size Is Valid");
            }
        }
    }
}
