using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TL;
using WTelegram;

namespace TDrive.Views
{
    public partial class FolderView : BaseAppForm
    {
        private readonly Client _tgClient;
        private List<string> _waitingList;

        public override string FormName => "folderview";
        public string PhysicalPath { get; set; }
        public ChatBase TelegramPath { get; set; }


        public FolderView(WTelegram.Client tgClient)
        {
            _tgClient = tgClient;
            InitializeComponent();
        }
        protected override async void OnLoad(EventArgs e)
        {
            var uploaded = await GetAllMessages(TelegramPath.ToInputPeer());
            var allFiles = Directory.GetFiles(PhysicalPath);

            var _3gigabytes = 3 * Math.Pow(1024, 3);

            var heavyFiles = allFiles.Select(f => new FileInfo(f)).Where(c => c.Length > _3gigabytes).Select(c => c.FullName);

            largCount.Text = heavyFiles.Count().ToString();

            var eligableFiles = allFiles.Except(heavyFiles);

            var yetToUpload = eligableFiles.Where(f => !uploaded.Any(c => f.Contains(c))).ToList();
            waitListCount.Text = yetToUpload.Count.ToString();
            _waitingList = yetToUpload;
            var total = yetToUpload.Count;

            doneCount.Text = uploaded.Count.ToString();

            progressBar1.Step = 100 / (total == 0 ? 1 : total);
            base.OnLoad(e);
        }

        private async Task<List<string>> GetAllMessages(InputPeer peer)
        {
            var offset = 0;
            var allMessages = await _tgClient.Messages_GetHistory(peer, add_offset: offset);
            var fileNames = allMessages.Messages
                .Where(s=>s is TL.Message)
                .Cast<TL.Message>()
                //.Where(m=> m != null)
                .Where(m=>m.media is MessageMediaDocument)
                .Select(m => m.media).Cast<MessageMediaDocument>()
                .Where(m=> m.document is Document)
                .Select(m => m.document).Cast<Document>()
                .Select(d => d.Filename).ToList();
            while (allMessages.Messages.Count() == 100)
            {
                offset += 100;
                allMessages = await _tgClient.Messages_GetHistory(peer, add_offset: offset);
                fileNames.AddRange(allMessages.Messages
                 .Where(s => s is TL.Message)
                .Cast<TL.Message>()
                //.Where(m=> m != null)
                .Where(m => m.media is MessageMediaDocument)
                .Select(m => m.media).Cast<MessageMediaDocument>()
                .Where(m => m.document is Document)
                .Select(m => m.document).Cast<Document>()
                .Select(d => d.Filename).ToList());
            }

            return fileNames;
        }

        private void OnUploaded(string fileName)
        {
            var currentCount = int.Parse(doneCount.Text);
            doneCount.Text = currentCount++.ToString();
            var currentWaitingListCount = int.Parse(waitListCount.Text);
            waitListCount.Text = currentWaitingListCount--.ToString();
            progressBar1.Value++;
        }

        private async void startSync_Click(object sender, EventArgs e)
        {
            var di = new DirectoryInfo(PhysicalPath);
            foreach (var fileName in _waitingList)
            {
                var file = new FileInfo(fileName);
                inprogressFile.Text = file.Name;
                var inputFile = await _tgClient.UploadFileAsync(file.FullName);
                await _tgClient.SendMediaAsync(TelegramPath.ToInputPeer(), "", inputFile, file.Extension);
                OnUploaded(fileName);
            }
        }
    }
}
