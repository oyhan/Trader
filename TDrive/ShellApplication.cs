using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TDrive
{
    public class ShellApplication : ApplicationContext
    {
        [DllImport("user32.dll")]
        static extern bool SetMenu(IntPtr hWnd, IntPtr hMenu);

        private ContextMenuStrip contextMenu;

        public ShellApplication()
        {
            // ... your code to initialize contextMenu ...
        }

        public void SetShellContextMenu(ContextMenuStrip contextMenu)
        {
            this.contextMenu = contextMenu;

            // Replace the form's menu with your context menu.
            var form = new Main { ContextMenuStrip = contextMenu };

            // Add a context menu item click handler.
            contextMenu.ItemClicked += ContextMenu_ItemClicked;

            // Add the form to the ApplicationContext.
            this.MainForm = form;

            // Run the application.
            Application.Run(this);
        }

        private void ContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // ... your code to handle menu item clicks ...
        }
    }
}
