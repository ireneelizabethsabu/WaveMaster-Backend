using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WaveMaster_Backend.Observers;
using WaveMaster_Backend.ViewModels;

namespace WaveMaster_Backend.Services
{
    public interface IFileService
    {
        public void FileWrite(SignalDataModel signalData);
        public object FileRead();
    }
    public class FileService : IFileService
    {
        public void FileWrite(SignalDataModel signalData)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(signalData);
                string filePath = "settings.json";
                File.WriteAllText(filePath, jsonString);               
            }
            catch (IOException ex)
            {
                throw new IOException("Error writing to file", ex);
            }
        }


        public object FileRead()
        {
            try
            {
                string filePath = "settings.json";
                string jsonString = File.ReadAllText(filePath);
                var settings = JsonSerializer.Deserialize<dynamic>(jsonString);
                return settings;
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundException("Error opening file",ex);
            }
            catch (JsonException ex)
            {
                throw new JsonException("Error parsing json", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception", ex);
            }
        }
    }
}
