namespace LoungeChairAPI.Lounge
{
    public class Response
    {
        public int status;
        public string errorMessage;
        public string correlationId;

        public bool IsSuccess()
        {
            return status == 0;
        }
    }
}
