namespace TranslationPOBot.Config.Translate.Google
{
    public class APIFreeTranslateConfig
    {

        public string Client { get; set; }
        public string SourceLanguage { get; set; }
        public string TargetLanguage { get; set; }
        public string HostLanguage { get; set; }
        public ProxyConfig Proxy { get; set; }


    }
    public class ProxyConfig
    {
        public string Address { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
