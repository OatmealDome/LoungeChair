namespace LoungeChairAPI.Lounge.Account
{
    public class LoungeTokenRequest
    {
        public LoungeTokenRequestParameter parameter;

        public LoungeTokenRequest(string language, string birthday, string country, string id_token)
        {
            parameter = new LoungeTokenRequestParameter(language, birthday, country, id_token);
        }
    }
}
