using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using TranslationPOBot.Config.Translate.Google;
using TranslationPOBot.Services.Contracts;
using static TranslationPOBot.Data.GoogleResponses;

namespace TranslationPOBot.Services.TranslateServices.Google
{
    public class APIFreeTranslateService : ITranslateService
    {
        private readonly APIFreeTranslateConfig _config;
        private readonly HttpClient _client;

        // Default parameters from the provided URL
        private const string DefaultClient = "webapp";
        private const string DefaultSourceLanguage = "en";
        private const string DefaultTargetLanguage = "uk";
        private const string DefaultHostLanguage = "uk";

        public APIFreeTranslateService()
        {
            string configFilePath = "apiGoogleConfig.json";
            var configJson = File.ReadAllText(configFilePath);
            _config = JsonConvert.DeserializeObject<APIFreeTranslateConfig>(configJson) ?? new APIFreeTranslateConfig();

            if (_config.Proxy != null)
            {
                var proxyConfig = _config.Proxy;
                var proxy = new WebProxy()
                {
                    Address = new Uri($"{proxyConfig.Address}:{proxyConfig.Port}"),
                    BypassProxyOnLocal = false,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(proxyConfig.Username, proxyConfig.Password)
                };

                var httpClientHandler = new HttpClientHandler()
                {
                    Proxy = proxy,
                    UseProxy = true
                };

                _client = new HttpClient(handler: httpClientHandler, disposeHandler: true);
            }
            else
            {
                _client = new HttpClient();
            }

            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        ~APIFreeTranslateService()
        {
            _client.Dispose();
        }
        private static string ObjectToQueryString(Dictionary<string, string> dict)
        {
            var list = new List<string>();
            foreach (var item in dict)
            {
                list.Add($"{Uri.EscapeDataString(item.Key)}={Uri.EscapeDataString(item.Value)}");
            }
            return string.Join("&", list);
        }
        public string Translate(string text)
        {



            //https://translate.google.com/translate_a/single?client=webapp&sl=en&tl=uk&hl=uk&dt=at&dt=bd&dt=ex&dt=ld&dt=md&dt=qca&dt=rw&dt=rm&dt=ss&dt=t&dt=gt&pc=1&otf=1&ssel=0&tsel=0&kc=1&tk=&ie=UTF-8&oe=UTF-8&q=hello
            var url = $"https://translate.google.com/translate_a/single?client=webapp&sl={_config.SourceLanguage}&hl={_config.TargetLanguage}&tl={_config.TargetLanguage}&dt=at&dt=bd&dt=ex&dt=ld&dt=md&dt=qca&dt=rw&dt=rm&dt=ss&dt=t&&dt=gt&pc=1&otf=1&ssel=0&tsel=0&kc=1&tk=&ie=UTF-8&oe=UTF-8&&q={Uri.EscapeDataString(text)}";

            var response = _client.GetAsync(url).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var responseData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var result = JsonConvert.DeserializeObject<List<object>>(responseData);
            return (result[0] as JArray).First().First().ToString();
            if (result[1] != null)
            {
                var target = new Target();

                if (result[0] is List<object> res0 && res0[1] is List<object> res01 && res01[3] != null)
                {
                    target.Pronunciations.Add(new Pronunciation { Symbol = res01[3].ToString(), Voice = "" });
                }

                foreach (var item in result[1] as List<object>)
                {
                    if (item is List<object> itemList)
                    {
                        var explanation = new Explanation
                        {
                            Trait = itemList[0]?.ToString(),
                            Explains = itemList[2] is List<object> explainsList ? explainsList.ConvertAll(x => x.ToString()) : new List<string>()
                        };
                        target.Explanations.Add(explanation);
                    }
                }

                if (result.Count > 13 && result[13] is List<object> res13 && res13[0] is List<object> res130)
                {
                    foreach (var item in res130)
                    {
                        if (item is List<object> itemList)
                        {
                            target.Sentence.Add(itemList[0]?.ToString());
                        }
                    }
                }

                return "";
            }
            else
            {
                var target = "";
                foreach (var r in result[0] as List<object>)
                {
                    if (r is List<object> rList && rList[0] != null)
                    {
                        target += rList[0]?.ToString();
                    }
                }
                return target.Trim();
            }
        }
    }
}
