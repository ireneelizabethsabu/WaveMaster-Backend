using WaveMaster_Backend.Observers;

namespace WaveMaster_Backend.Services
{
    public interface IFileService
    {
        public void FileWrite();
        public void FileRead();
    }
    public class FileService
    {
        public void FileWrite()
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(signalData);
                string filePath = "settings.json";
            }
            catch (Exception ex)
            {

            }
        }
        public void FileRead() 
        { 
            
        }
    }
}
