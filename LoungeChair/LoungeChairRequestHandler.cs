using System.Security.Cryptography.X509Certificates;
using CefSharp;
using System.Collections.Specialized;

namespace LoungeChair
{
    class LoungeChairRequestHandler : IRequestHandler
    {
        private readonly BrowserForm browserForm;
        public bool initialRequestFinished = true;

        internal LoungeChairRequestHandler(BrowserForm form)
        {
            browserForm = form;
        }

        public CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {
            if (request.Url.StartsWith("npf71b963c1b7b6d119://auth"))
            {
                browserForm.ApplicationAuthorizedCallback(request.Url);
                return CefReturnValue.Cancel;
            }
            
            if (!initialRequestFinished)
            {
                if (request.Url.StartsWith(browserForm.webService.uri))
                {
                    NameValueCollection headers = request.Headers;
                    headers["X-GameWebToken"] = browserForm.webServiceCredential.accessToken;
                    request.Headers = headers;
                }

                initialRequestFinished = true;
            }

            return CefReturnValue.Continue;
        }

        // Unused

        public bool GetAuthCredentials(IWebBrowser browserControl, IBrowser browser, IFrame frame, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {
            return false;
        }

        public IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            return null;
        }

        public bool OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool isRedirect)
        {
            return false;
        }

        public bool OnCertificateError(IWebBrowser browserControl, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
        {
            return false;
        }

        public bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return false;
        }

        public void OnPluginCrashed(IWebBrowser browserControl, IBrowser browser, string pluginPath)
        {
            ;
        }

        public bool OnProtocolExecution(IWebBrowser browserControl, IBrowser browser, string url)
        {
            return false;
        }

        public bool OnQuotaRequest(IWebBrowser browserControl, IBrowser browser, string originUrl, long newSize, IRequestCallback callback)
        {
            return false;
        }

        public void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status)
        {
            ;
        }

        public void OnRenderViewReady(IWebBrowser browserControl, IBrowser browser)
        {
            ;
        }

        public void OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {
            ;
        }

        public void OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, ref string newUrl)
        {
            ;
        }

        public bool OnResourceResponse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            return false;
        }

        public bool OnSelectClientCertificate(IWebBrowser browserControl, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
        {
            return false;
        }
    }
}
