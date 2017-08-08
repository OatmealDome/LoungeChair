using System.Collections.Generic;

namespace LoungeChairAPI.Accounts.Auth
{
    public class ApplicationTokenResponse
    {
        public string access_token;
        public int expires_in;
        public string id_token;
        public List<string> scope;
        public string token_type;
    }
}
