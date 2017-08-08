using System.Collections.Generic;

namespace LoungeChairAPI.Accounts.User
{
    public class Mii
    {
        public string id;
        public string type;
        public Dictionary<string, string> storeData;
        public string imageUriTemplate;
        public string clientId;
        public string imageOrigin;
        public string favouriteColor;
        public long updatedAt;
        public string etag;
    }
}
