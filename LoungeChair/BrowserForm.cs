using LoungeChairAPI.Lounge;
using LoungeChairAPI.Lounge.GameWebService;
using CefSharp;
using CefSharp.WinForms;
using System.Windows.Forms;

namespace LoungeChair
{
    public partial class BrowserForm : Form
    {
        // Internals
        private readonly ChromiumWebBrowser browser;
        public readonly WebService webService;
        public readonly Credential webServiceCredential;

        public BrowserForm(string url)
        {
            InitializeComponent();

            browser = new ChromiumWebBrowser(url)
            {
                Dock = DockStyle.Fill,
                RequestHandler = new LoungeChairRequestHandler(this)
            };

            this.Controls.Add(browser);

            // Default to cancel result
            this.DialogResult = DialogResult.Cancel;
        }

        public BrowserForm(WebService service, Credential credential) : this(service.uri)
        {
            webService = service;
            webServiceCredential = credential;
            ((LoungeChairRequestHandler)browser.RequestHandler).initialRequestFinished = false;
        }

        public void ApplicationAuthorizedCallback(string url)
        {
            ((MainForm)this.Owner).ApplicationAuthorizedCallback(url);

            this.Invoke((MethodInvoker)delegate
            {
                this.DialogResult = DialogResult.OK;
                this.Close(); 
            });
        }
    }
}
