using LoungeChairAPI.Accounts.Auth;
using LoungeChairAPI.Accounts.User;
using LoungeChairAPI.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LoungeChairAPI.Accounts
{
    public class NintendoAccount
    {
        // Static
        public static readonly string ACCOUNTS_BASE_URL = "https://accounts.nintendo.com";
        public static readonly string API_BASE_URL = "https://api.accounts.nintendo.com";

        // Internals
        private static readonly HttpHelper httpHelper = new HttpHelper();
        private readonly ApplicationTokenResponse applicationResponse;

        // Public members
        public readonly string accounts_session_token;

        public string application_access_token
        {
            get
            {
                return applicationResponse.access_token;
            }
            private set
            {
                throw new InvalidOperationException();
            }
        }

        public string application_id_token
        {
            get
            {
                return applicationResponse.id_token;
            }
            private set
            {
                throw new InvalidOperationException();
            }
        }

        public string application_access_token_type
        {
            get
            {
                return applicationResponse.token_type;
            }
            private set
            {
                throw new InvalidOperationException();
            }
        }

        private NintendoAccount(string session_token, ApplicationTokenResponse response)
        {
            applicationResponse = response;
            accounts_session_token = session_token;
        }

        public static async Task<NintendoAccount> Login(string id, string session, string grant_type)
        {
            // Get an application access token
            ApplicationTokenResponse appResponse = await GetApplicationToken(id, session, grant_type);

            // Return a NintendoAccount instance
            return new NintendoAccount(session, appResponse);
        }

        public static async Task<NintendoAccount> Login(AuthorizationRequest request, string session_token_code, string grant_type)
        {
            // Create the form contents
            FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", request.client_id),
                new KeyValuePair<string, string>("session_token_code", session_token_code),
                new KeyValuePair<string, string>("session_token_code_verifier", request.code_verifier)
            });

            // Send the request
            HttpResponseMessage response = await httpHelper.POSTRequest(ACCOUNTS_BASE_URL + "/connect/1.0.0/api/session_token", content);
            response.EnsureSuccessStatusCode();

            // Deserialize the response
            SessionTokenResponse sessionResponse = await HttpHelper.DeserializeResponse<SessionTokenResponse>(response);

            // Pass this data to the final login method
            return await Login(request.client_id, sessionResponse.session_token, grant_type);
        }

        internal static async Task<ApplicationTokenResponse> GetApplicationToken(string client_id, string session_token, string grant_type)
        {
            // Create a new token request
            ApplicationTokenRequest tokenRequest = new ApplicationTokenRequest();
            tokenRequest.client_id = client_id;
            tokenRequest.session_token = session_token;
            tokenRequest.grant_type = grant_type;

            // Construct the request contents
            StringContent content = new StringContent(JsonConvert.SerializeObject(tokenRequest), Encoding.UTF8, "application/json");

            // Send the request
            HttpResponseMessage response = await httpHelper.POSTRequest(ACCOUNTS_BASE_URL + "/connect/1.0.0/api/token", content);
            response.EnsureSuccessStatusCode();

            // Deserialize and return the response
            return await HttpHelper.DeserializeResponse<ApplicationTokenResponse>(response);
        }

        public async Task<AccountUser> GetMyself()
        {
            // Construct a request
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, API_BASE_URL + "/2.0.0/users/me");
            request.Headers.Add("Authorization", application_access_token_type + " " + application_access_token);

            // Send the request
            HttpResponseMessage response = await httpHelper.SendRequest(request);
            response.EnsureSuccessStatusCode();

            // Deserialize and return the response
            return await HttpHelper.DeserializeResponse<AccountUser>(response);
        }

        /*public static async Task<NintendoAccount> Login(string id, string password)
        {
            NintendoAccount account = new NintendoAccount();

            string csrfToken = await account.GetCsrfToken();

            FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("post_login_redirect_uri", "https://accounts.nintendo.com"),
                new KeyValuePair<string, string>("redirect_after", "5"),
                new KeyValuePair<string, string>("display", ""),
                new KeyValuePair<string, string>("subject_id", id),
                new KeyValuePair<string, string>("subject_password", password),
                new KeyValuePair<string, string>("csrf_token", csrfToken),
                new KeyValuePair<string, string>("csrf", csrfToken),
            });

            HttpResponseMessage response = await account.httpHelper.POSTRequest("/login", content);
            using (var reader = new System.IO.StreamReader(await response.Content.ReadAsStreamAsync(), Encoding.UTF8))
            {
                string responseText = reader.ReadToEnd();
                ;
            }

            return account;
        }

        private async Task<string> GetCsrfToken()
        {
            HttpResponseMessage response = await httpHelper.GETRequest("/login");
            string responseContents = await httpHelper.GetResponseContent(response);

            // Extract using regex
            Regex expression = new Regex("(?<=login-csrf-token\" content=\")(.*)(?=\")");
            if (!expression.IsMatch(responseContents))
            {
                throw new Exception("Could not find CSRF token");
            }

            return expression.Match(responseContents).Value;
        }*/

    }
}
