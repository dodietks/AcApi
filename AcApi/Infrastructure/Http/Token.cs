namespace AcApi.Infrastructure.Http
{
    public class Token
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public static Token Bearer(string value)
        {
            Token token = new Token();
            token.Value = value;
            token.Type = "Bearer";
            return token;
        }

    }
}
