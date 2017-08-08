namespace LoungeChairAPI.Lounge.GameWebService
{
    public class GetServiceTokenRequest
    {
        public GetServiceTokenRequestParameter parameter;

        public GetServiceTokenRequest(long id)
        {
            parameter = new GetServiceTokenRequestParameter(id);
        }
    }
}
