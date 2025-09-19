using Astro.Winform;
using Astro.Winform.Forms;

namespace PointOfSale
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            try
            {
                var hostConfigPath = AppContext.BaseDirectory + "host.ini";
                if (File.Exists(hostConfigPath))
                {
                    var hostConfig = File.ReadAllText(hostConfigPath);
                    My.Application.ApiUrl = hostConfig.Trim();
                }
                else
                {
                    My.Application.ApiUrl = "http://localhost:5002";
                }
                Application.Run(new SPAForm());
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error starting application", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}