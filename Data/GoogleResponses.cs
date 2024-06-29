namespace TranslationPOBot.Data
{
    public class GoogleResponses
    {
        public List<List<object>> Sentences { get; set; }
        public List<object> Explanations { get; set; }
        public List<object> Associations { get; set; }
        public List<object> Sentence { get; set; }
        public List<object> Audios { get; set; }


        public class Explanation
        {
            public string Trait { get; set; }
            public List<string> Explains { get; set; }
        }

        public class Pronunciation
        {
            public string Symbol { get; set; }
            public string Voice { get; set; }
        }

        public class Target
        {
            public List<Pronunciation> Pronunciations { get; set; } = new List<Pronunciation>();
            public List<Explanation> Explanations { get; set; } = new List<Explanation>();
            public List<string> Sentence { get; set; } = new List<string>();
            public List<object> Audios { get; set; } = new List<object>();
        }
    }
}
