using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TDrive
{
    public partial class Login : BaseAppForm
    {
        private readonly WTelegram.Client _tgClient;
        private readonly FormFactory formFactory;

        public override string FormName => "login";

        public Login(WTelegram.Client tgClient)
        {
            InitializeComponent();
            this._tgClient = tgClient;
           

        }

        protected override void OnActivated(EventArgs e)
        {
           
            base.OnActivated(e);
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            
            var loginInfo = textBox1.Text;
            button1.Enabled = false;
            var txt = button1.Text;
            button1.Text = "Please wait...";
            var result = await DoLogin(loginInfo);
            if (result == null)
            {
                this.Close();
                
            }
            button1.Text = txt;
            button1.Enabled = true;
            label1.Text = result;

        }

        private async Task<string> DoLogin(string loginInfo) // (add this method to your code)
        {
            while (_tgClient.User == null)
                switch (await _tgClient.Login(loginInfo)) // returns which config is needed to continue login
                {
                    case "verification_code": return ("Code: ");
                    case "name": loginInfo = "John Doe"; break;    // if sign-up is required (first/last_name)
                    case "password": return ("ss: ");  // if user has enabled 2FA
                    default: loginInfo = null; break;
                }
            LoginInfo.User = _tgClient.User;
            LoginInfo.UserId = _tgClient.User.id;
;            return null;
        }

        private  async Task<bool>  AlreadyLoggedIn()
        {
            var user =await _tgClient.LoginBotIfNeeded();
            return user != null;
        }
    }
}
