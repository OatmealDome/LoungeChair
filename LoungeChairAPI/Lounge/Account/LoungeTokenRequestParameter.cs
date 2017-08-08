namespace LoungeChairAPI.Lounge.Account
{
    public class LoungeTokenRequestParameter
    {
        public string language;
        public string naBirthday;
        public string naCountry;
        public string naIdToken;

        internal LoungeTokenRequestParameter(string lang, string birthday, string country, string id_token)
        {
            language = lang;
            naBirthday = birthday;
            naCountry = country;
            naIdToken = id_token;
        }
    }
}
