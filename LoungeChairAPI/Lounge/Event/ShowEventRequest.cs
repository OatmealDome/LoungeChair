namespace LoungeChairAPI.Lounge.Event
{
    public class ShowEventRequest
    {
        public ShowEventRequestParameter parameter;

        public ShowEventRequest(long id)
        {
            parameter = new ShowEventRequestParameter(id);
        }
    }
}
