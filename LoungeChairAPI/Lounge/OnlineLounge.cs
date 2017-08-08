using LoungeChairAPI.Accounts;
using LoungeChairAPI.Accounts.User;
using LoungeChairAPI.Http;
using LoungeChairAPI.Lounge.Account;
using LoungeChairAPI.Lounge.GameWebService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LoungeChairAPI.Lounge
{
    public class OnlineLounge
    {
        // Static
        public static readonly string API_BASE_URL = "https://api-lp1.znc.srv.nintendo.net";

        // Internals
        private static readonly HttpHelper httpHelper = new HttpHelper();
        private readonly LoungeTokenResponseResult tokenResult;

        // Public members
        public string access_token
        {
            get
            {
                return tokenResult.webApiServerCredential.accessToken;
            }
            private set
            {
                throw new InvalidOperationException();
            }
        }

        public LoungeTokenUser user
        {
            get
            {
                return tokenResult.user;
            }
            private set
            {
                throw new InvalidOperationException();
            }
        }

        private OnlineLounge(LoungeTokenResponseResult result)
        {
            tokenResult = result;
        }

        public static async Task<OnlineLounge> Login(NintendoAccount account)
        {
            // Get account owner's information
            AccountUser accountUser = await account.GetMyself();

            // Construct a login (token) request
            LoungeTokenRequest request = new LoungeTokenRequest(accountUser.language, accountUser.birthday, accountUser.country, account.application_id_token);

            // Construct the request contents
            StringContent content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            // Send the request
            HttpResponseMessage response = await httpHelper.POSTRequest(API_BASE_URL + "/v1/Account/GetToken", content);
            response.EnsureSuccessStatusCode();

            // Deserialize the response
            LoungeTokenResponse apiResponse = await HttpHelper.DeserializeResponse<LoungeTokenResponse>(response);

            // Check for success
            if (!apiResponse.IsSuccess())
            {
                throw new Exception("Lounge login failure: " + apiResponse.errorMessage);
            }

            return new OnlineLounge(apiResponse.result);
        }

        public async Task<List<WebService>> GetWebServices()
        {
            // Construct a blank request
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, API_BASE_URL + "/v1/Game/ListWebServices");
            request.Headers.Add("Authorization", "Bearer " + access_token);
            request.Content = new StringContent("", Encoding.UTF8, "application/json");

            // Send the request
            HttpResponseMessage response = await httpHelper.SendRequest(request);
            response.EnsureSuccessStatusCode();

            // Deserialize the response
            ServiceListResponse apiResponse = await HttpHelper.DeserializeResponse<ServiceListResponse>(response);

            // Check for success
            if (!apiResponse.IsSuccess())
            {
                throw new Exception("Get web services failure: " + apiResponse.errorMessage);
            }

            return apiResponse.result;
        }

        public async Task<Credential> GetWebServiceCredential(WebService service)
        {
            // Construct a request
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, API_BASE_URL + "/v1/Game/GetWebServiceToken");
            request.Headers.Add("Authorization", "Bearer " + access_token);
            request.Content = new StringContent(JsonConvert.SerializeObject(new GetServiceTokenRequest(service.id)), Encoding.UTF8, "application/json");

            // Send the request
            HttpResponseMessage response = await httpHelper.SendRequest(request);
            response.EnsureSuccessStatusCode();

            // Deserialize the response
            GetServiceTokenResponse apiResponse = await HttpHelper.DeserializeResponse<GetServiceTokenResponse>(response);

            // Check for success
            if (!apiResponse.IsSuccess())
            {
                throw new Exception("Get web service token failure: " + apiResponse.errorMessage);
            }

            return apiResponse.result;
        }

    }
}
