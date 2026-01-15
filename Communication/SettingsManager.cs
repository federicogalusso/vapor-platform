using System.Configuration;

namespace Communication
{
    public class SettingsManager
    {
        public async Task<string> ReadSettings(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                return await Task.FromResult(appSettings[key] ?? string.Empty);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error leyendo la configuracion");
                return string.Empty;
            }
        }
    }
}