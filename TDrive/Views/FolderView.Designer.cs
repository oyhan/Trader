namespace TDrive.Views
{
    partial class FolderView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            doneCount = new Label();
            label3 = new Label();
            waitListCount = new Label();
            progressBar1 = new ProgressBar();
            inprogressFile = new Label();
            startSync = new Button();
            label2 = new Label();
            largCount = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(58, 56);
            label1.Name = "label1";
            label1.Size = new Size(108, 20);
            label1.TabIndex = 0;
            label1.Text = "Files Uploaded";
            // 
            // doneCount
            // 
            doneCount.AutoSize = true;
            doneCount.Location = new Point(188, 56);
            doneCount.Name = "doneCount";
            doneCount.Size = new Size(17, 20);
            doneCount.TabIndex = 0;
            doneCount.Text = "0";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(58, 94);
            label3.Name = "label3";
            label3.Size = new Size(141, 20);
            label3.TabIndex = 0;
            label3.Text = "Files waiting to sync";
            // 
            // waitListCount
            // 
            waitListCount.AutoSize = true;
            waitListCount.Location = new Point(223, 94);
            waitListCount.Name = "waitListCount";
            waitListCount.Size = new Size(17, 20);
            waitListCount.TabIndex = 1;
            waitListCount.Text = "0";
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(12, 163);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(776, 12);
            progressBar1.TabIndex = 2;
            // 
            // inprogressFile
            // 
            inprogressFile.AutoSize = true;
            inprogressFile.Location = new Point(12, 178);
            inprogressFile.Name = "inprogressFile";
            inprogressFile.Size = new Size(87, 20);
            inprogressFile.TabIndex = 0;
            inprogressFile.Text = "Inprogress...";
            // 
            // startSync
            // 
            startSync.Location = new Point(504, 56);
            startSync.Name = "startSync";
            startSync.Size = new Size(181, 58);
            startSync.TabIndex = 3;
            startSync.Text = "Strart backup";
            startSync.UseVisualStyleBackColor = true;
            startSync.Click += startSync_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(58, 18);
            label2.Name = "label2";
            label2.Size = new Size(69, 20);
            label2.TabIndex = 4;
            label2.Text = "Larg files";
            // 
            // largCount
            // 
            largCount.AutoSize = true;
            largCount.Location = new Point(159, 18);
            largCount.Name = "largCount";
            largCount.Size = new Size(17, 20);
            largCount.TabIndex = 5;
            largCount.Text = "0";
            // 
            // FolderView
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(795, 223);
            Controls.Add(largCount);
            Controls.Add(label2);
            Controls.Add(startSync);
            Controls.Add(progressBar1);
            Controls.Add(waitListCount);
            Controls.Add(doneCount);
            Controls.Add(inprogressFile);
            Controls.Add(label3);
            Controls.Add(label1);
            Name = "FolderView";
            Text = "FolderView";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label doneCount;
        private Label label3;
        private Label waitListCount;
        private ProgressBar progressBar1;
        private Label inprogressFile;
        private Button startSync;
        private Label label2;
        private Label largCount;
    }
}