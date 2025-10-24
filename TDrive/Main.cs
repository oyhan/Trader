using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TDrive.Views;
using TL;
using WTelegram;

namespace TDrive
{
    public partial class Main : BaseAppForm
    {
        private readonly Client _tgClient;
        private List<ChatBase> _folders;

        public Main(WTelegram.Client tgClient)
        {
            this._tgClient = tgClient;
            InitializeComponent();
        }
        public override string FormName => "main";
        public Main()
        {
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await LoadFolders();
        }

        public async Task LoadFolders()
        {

            try
            {
                var dialogs = await _tgClient.Messages_GetAllDialogs(); // dialogs = groups/channels/users

                var folders = dialogs.chats.Values.Where(c => c.Title.Contains("folder")).ToList();
                _folders = folders;
                var width = 145;
                for (var i = 0; i < folders.Count; i++)
                {
                    var folder = folders[i];
                    var btn = new Button();
                    btn.Text = folder.Title;
                    btn.Location = new Point(19  +i* (width+ 10) , 36);
                    btn.Name = folder.Title;
                    btn.Size = new Size(width, 59);
                    btn.TabIndex = i;
                    btn.UseVisualStyleBackColor = true;
                    btn.Visible = true;
                    btn.Click += button1_Click;
                    groupBox1.Controls.Add(btn);
                }

            }
            catch (System.ApplicationException) //not logged in
            {
                var login = FormFactory.GetForm("login");
                login.ShowDialog();

                await LoadFolders();
            }
        }

        

        private async void button1_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            var folder = _folders.First(f => f.Title == btn.Text);
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                var directory = folderBrowserDialog1.SelectedPath;
              
                var folderView = FormFactory.GetForm("folderview") as FolderView;
                folderView.PhysicalPath = directory;
                folderView.TelegramPath = folder;

                folderView.ShowDialog();
            }
        }
    }
}
