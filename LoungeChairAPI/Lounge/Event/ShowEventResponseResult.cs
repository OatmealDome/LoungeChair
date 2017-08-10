using System.Collections.Generic;

namespace LoungeChairAPI.Lounge.Event
{
    public class ShowEventResponseResult
    {
        public string name;
        public string passCode;
        public bool allowJoinGameWithoutCoral;
        public string shareUri;
        public long id;
        public List<EventMember> members;
        public Game game;
        public string description;
        public GameStatus gameStatus;
        public long ownerUserId;
    }
}
