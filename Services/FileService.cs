using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using WaveMaster_Backend.Observers;
using WaveMaster_Backend.ViewModels;

namespace WaveMaster_Backend.Services
{
    /// <summary>
    /// Interface defining file read and write operations 
    /// </summary>
    public interface IFileService
    {
        void FileWrite(SignalDataModel signalData);
        SignalDataModel FileRead();
    }
    /// <summary>
    /// Handles file read and write operations by implementing IFileService interface
    /// </summary>
    /// <remarks>
    /// For writing generated signal settings to json file and read it back.
    /// </remarks>
    public class FileService : IFileService
    {
        private readonly string _filePath = "settings.json";

        /// <summary>
        /// write generate settings data to settings.json file
        /// </summary>
        /// <param name="signalData">SignalDataModel object containing signal type, peak to peak and voltage</param>
        /// <exception cref="IOException">catches any file exceptions</exception>
        /// <exception cref="JsonException">handle JsonException when there's an issue parsing JSON data</exception>
        public void FileWrite(SignalDataModel signalData)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(signalData);
                File.WriteAllText(_filePath, jsonString, Encoding.UTF8);               
            }
            catch (IOException ex)
            {
                throw new IOException("Error writing to file", ex);
            }
            catch (JsonException ex)
            {
                throw new JsonException("Error parsing json", ex);
            }
        }

        /// <summary>
        /// Read generate settings from settings.json file
        /// </summary>
        /// <returns>SignalDataModel object</returns>
        /// <exception cref="IOException">handle IOException</exception>
        /// <exception cref="JsonException">handle JsonException when there's an issue parsing JSON data</exception>
        
        public SignalDataModel FileRead()
        {
            try
            {                
                string jsonString = File.ReadAllText(_filePath,Encoding.UTF8);
                return JsonSerializer.Deserialize<SignalDataModel>(jsonString);
            }           
            catch (IOException ex)
            {
                throw new IOException("Error reading from file", ex);
            }            
            catch (JsonException ex)
            {
                throw new JsonException("Error parsing json", ex);
            }
        }
    }
}
