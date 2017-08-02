using LoungeChairAPI.Accounts;
using LoungeChairAPI.Accounts.Auth;
using LoungeChairAPI.Http;
using LoungeChairAPI.Lounge;
using LoungeChairAPI.Lounge.GameWebService;
using LoungeChair.Config;
using LoungeChair.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using System.Reflection;

namespace LoungeChair
{
    public partial class MainForm : Form
    {
        // Internals
        private NintendoAccount account;
        private OnlineLounge lounge;
        private HttpHelper httpHelper = new HttpHelper();
        private AuthorizationRequest request;
        private List<WebService> webServices;

        public MainForm()
        {
            InitializeComponent();

            // Load the configuration
            Configuration.Load();

            // Initialize Cef
            CefSettings settings = new CefSettings();
            Cef.Initialize(settings);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Check for an update
            UpdateInfo info = UpdateChecker.CheckForUpdate();
            Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            if (currentVersion < info._version)
            {
                MessageBox.Show("There is a new update available.\nGo to: https://github.com/OatmealDome/LoungeChair/releases/latest");
            }

            // Check if we need to do first-run setup
            if (Configuration.currentConfig.session_token.Equals("not-logged-in"))
            {
                // Create a new authorization request
                request = new AuthorizationRequest("npf71b963c1b7b6d119://auth", "71b963c1b7b6d119", "openid user user.birthday user.mii user.screenName");

                // Open it in the browser and check if the log in was a success
                BrowserForm form = new BrowserForm(request.ToUrl());
                if (form.ShowDialog(this) == DialogResult.Cancel)
                {
                    // Close the application, the form was closed
                    MessageBox.Show("The login was cancelled. Restart LoungeChair and try again.");
                    this.Close();
                }
            }
            else
            {
                LoadingForm form = new LoadingForm(this, LoadingType.INITIAL_LOGIN, null);
                form.ShowDialog(this);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Shutdown Cef
            Cef.Shutdown();
        }

        private void listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo hitTest = listView.HitTest(e.Location);
            if (hitTest.Item != null)
            {
                // Get the web service
                WebService webService = webServices[hitTest.Item.Index];

                // Start the loading form
                LoadingForm form = new LoadingForm(this, LoadingType.WEB_SERVICE, webService);
                form.ShowDialog(this);
            }
        }

        public async void ApplicationAuthorizedCallback(string url)
        {
            // Parse query parameters
            string session_token_code = ExtractStringWithRegex("(?<=session_token_code=)(.*)(?=&state=)", url);

            // Log into the Nintendo Account
            account = await NintendoAccount.Login(request, session_token_code, "urn:ietf:params:oauth:grant-type:jwt-bearer-session-token");

            // Set the session token in the configuration
            Configuration.currentConfig.session_token = account.accounts_session_token;
            Configuration.Save();

            // Log in to NSO
            await LogIntoOnlineLounge();

            // Get web services
            await GetAllWebServices();

            // Populate the list view with the web services
            this.Invoke((MethodInvoker)delegate
            {
                PopulateListView();
            });
        }

        public async Task UseSessionTokenForAccount()
        {
            account = await NintendoAccount.Login("71b963c1b7b6d119", Configuration.currentConfig.session_token, "urn:ietf:params:oauth:grant-type:jwt-bearer-session-token");
        }

        public async Task LogIntoOnlineLounge()
        {
            // Get an OnlineLounge instance
            lounge = await OnlineLounge.Login(account);
        }

        public async Task<Credential> LogInToWebService(WebService webService)
        {
            // Get a token for this web service
            Credential credential = await lounge.GetWebServiceCredential(webService);

            return credential;
        }

        public async Task GetAllWebServices()
        {
            // Get all web services
            webServices = await lounge.GetWebServices();

            // Clear the image list
            imageList.Images.Clear();

            for (int i = 0; i < webServices.Count; i++)
            {
                // Get the web service
                WebService service = webServices[i];

                // Make a request for the image
                HttpResponseMessage response = await httpHelper.GETRequest(service.imageUri);

                // Read in the image
                Image image = Image.FromStream(await response.Content.ReadAsStreamAsync());

                // Create a thumbnail
                Image thumbnail = image.GetThumbnailImage(imageList.ImageSize.Width, imageList.ImageSize.Height, null, IntPtr.Zero);

                // Add the thumbnail to the image list
                imageList.Images.Add(thumbnail);
            }
        }

        public void PopulateListView()
        {
            // Clear the list view
            listView.Items.Clear();

            for (int i = 0; i < webServices.Count; i++)
            {
                // Get the web service
                WebService service = webServices[i];

                // Create a new item
                ListViewItem item = new ListViewItem();
                item.Text = service.name;
                item.ImageIndex = i;

                // Add the item
                listView.Items.Add(item);
            }
        }

        private string ExtractStringWithRegex(string regex, string source)
        {
            // Extract using regex
            Regex expression = new Regex(regex);
            if (!expression.IsMatch(source))
            {
                return null;
            }

            return expression.Match(source).Value;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            MessageBox.Show("LoungeChair " + version + "\nCopyright (C) OatmealDome 2017");
        }

    }
}
