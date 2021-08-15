using Newtonsoft.Json;

namespace Pokedex.Service.Contracts.PokeApiDto
{
    public class FunTranslatedResponse
    {
        [JsonProperty("success")]
        public FunTranslationSuccess Success { get; set; }

        [JsonProperty("contents")]
        public FunTranslationContents Contents { get; set; }

        [JsonProperty("error")]
        public FunTranslationError Error { get; set; }
    }

    public class FunTranslationError
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class FunTranslationSuccess
    {
        [JsonProperty("total")]
        public int Total { get; set; }
    }

    public class FunTranslationContents
    {
        [JsonProperty("translated")]
        public string Translated { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("translation")]
        public string Translation { get; set; }
    }



}
