using CefSharp;
using LoungeChairAPI.Lounge;
using LoungeChairAPI.Lounge.GameWebService;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace LoungeChair.Forms
{
    public partial class LoadingForm : Form
    {
        private readonly MainForm mainForm;
        private readonly LoadingType loadingType;
        private readonly object argument;

        public LoadingForm(MainForm form, LoadingType type, object arg)
        {
            InitializeComponent();

            // Set members
            mainForm = form;
            loadingType = type;
            argument = arg;
        }

        private async void LoadingForm_Load(object sender, EventArgs e)
        {
            switch (loadingType)
            {
                case LoadingType.INITIAL_LOGIN:
                    // Set the loading text
                    loadingLabel.Text = "Logging in to Nintendo Switch Online...";

                    // Log in to the user's Nintendo Account
                    await mainForm.UseSessionTokenForAccount();

                    // Log in to the Online Lounge
                    await mainForm.LogIntoOnlineLounge();

                    // Populate the form's web service list
                    await mainForm.GetAllWebServices();

                    // Add all the web services to the list view
                    mainForm.PopulateListView();

                    break;
                case LoadingType.WEB_SERVICE:
                    // Set the loading text
                    loadingLabel.Text = "Logging in to the game service...";

                    // Log in to the web service
                    Credential credential = await mainForm.LogInToWebService((WebService)argument);

                    // Hide the form
                    this.Hide();

                    // Open it in the browser
                    BrowserForm form = new BrowserForm((WebService)argument, credential);
                    form.ShowDialog(this);

                    break;
                default:
                    throw new InvalidOperationException();
            }

            this.Close();
        }
    }
}
