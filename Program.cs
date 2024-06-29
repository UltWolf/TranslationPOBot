// See https://aka.ms/new-console-template for more information
using TranslationPOBot.Services.Contracts;
using TranslationPOBot.Services.TranslateServices.Google;

class Program
{
    static string targetLanguage = "ua";
    static string projectId = "your-google-cloud-project-id";
    static string filePath = "code.po";
    static string tempFilePath = "code2.po";

    private static ITranslateService _translateService;
    static void Main()
    {

        _translateService = new APIFreeTranslateService();
        File.Delete(tempFilePath);
        using (var reader = new StreamReader(filePath))
        using (var writer = new StreamWriter(tempFilePath))
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                if (line.Contains("msgid"))
                {
                    string needToTranslate = "";
                    while (!line.Contains("msgstr") && line != null)
                    {
                        needToTranslate += line.Replace("msgid", "");
                        writer.WriteLine(line);
                        line = reader.ReadLine();
                    }
                    if (line.Length < 10 && needToTranslate.Length > 5)
                    {
                        line = "msgstr " + _translateService.Translate(needToTranslate);
                    }
                    writer.WriteLine(line);
                }
                else
                {
                    writer.WriteLine(line);
                }
            }
        }

        File.Delete(filePath);
        File.Move(tempFilePath, filePath);
    }



    public class PoEntry
    {
        public string Key { get; set; }
        public string MsgCtxt { get; set; }
        public string MsgId { get; set; }
        public string MsgStr { get; set; }
        public int KeyLineIndex { get; set; }
        public int MsgStrLineIndex { get; set; }
    }
}
