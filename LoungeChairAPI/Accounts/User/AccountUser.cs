using System.Collections.Generic;

namespace LoungeChairAPI.Accounts.User
{
    public class AccountUser
    {
        public string birthday;
        public string region;
        public Mii mii;
        public bool analyticsOptedIn;
        public string screenName;
        public TimeZone timezone;
        public long updatedAt;
        public bool emailVerified;
        public string country;
        public bool emailOptedIn;
        public long analyticsOptedInUpdatedAt;
        public string gender;
        public List<Mii> candidateMiis;
        public bool isChild;
        public bool clientFriendsOptedIn;
        public OptedInPromotions eachEmailOptedIn;
        public long createdAt;
        public string language;
        public string nickname;
        public long emailOptedInUpdatedAt;
        public long clientFriendsOptedInUpdatedAt;
        public string id;
    }
}
