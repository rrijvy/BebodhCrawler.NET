using Core.Models;
using System.Diagnostics;

namespace Services
{
    public class PythonScriptService
    {
        public string RunAmazonCategoryScrapper(AmazonCategoryScrapperRequestModel requestModel)
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = requestModel.PythonExecutablePath;
                    process.StartInfo.Arguments = $"{requestModel.ScrapperFilePath} {requestModel.Url}";
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;

                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();

                    process.WaitForExit();

                    return "Started.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
