using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LoungeChairAPI.Accounts.Auth
{
    public class AuthorizationRequest
    {
        public readonly string state;
        public readonly string redirect_uri;
        public readonly string client_id;
        public readonly string scope;
        public readonly string code_verifier;
        public readonly string session_code_challenge;

        public AuthorizationRequest(string redirect, string id, string scope)
        {
            // Open up a secure random number generator
            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
            {
                // Generate the state parameter
                byte[] stateData = new byte[36];
                rng.GetBytes(stateData);
                state = ToSafeBase64(stateData);

                // Generate the session code verifier
                byte[] codeVerifierArray = new byte[32];
                rng.GetBytes(codeVerifierArray);

                // Convert the verifier to string
                code_verifier = ToSafeBase64(codeVerifierArray);

                // Create a SHA256 instance
                using (SHA256 sha256 = SHA256Managed.Create())
                {
                    // Generate the code challenge
                    byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(code_verifier));

                    // Convert the hash to base64
                    session_code_challenge = ToSafeBase64(hashBytes);
                }
            }

            // Set the other members
            redirect_uri = redirect;
            client_id = id;
            this.scope = scope;
        }

        public string ToUrl()
        {
            string formatString = "{0}/connect/1.0.0/authorize?state={1}&redirect_uri={2}&client_id={3}&scope={4}&response_type=session_token_code&session_token_code_challenge={5}&session_token_code_challenge_method=S256";
            return String.Format(formatString, NintendoAccount.ACCOUNTS_BASE_URL, state, redirect_uri, client_id, scope, session_code_challenge);
        }

        private string ToSafeBase64(byte[] data)
        {
            return Convert.ToBase64String(data).TrimEnd(new char[] { '=' }).Replace('+', '-').Replace('/', '_');
        }

    }
}
