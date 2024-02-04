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
                    process.StartInfo.FileName = $"E:\\Codebase\\BebodhCrawler\\BebodhCrawler.Py\\env\\Scripts\\python.exe";
                    process.StartInfo.Arguments = $"E:\\Codebase\\BebodhCrawler\\BebodhCrawler.Py\\main.py {requestModel.Url}";
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
