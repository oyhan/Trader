public class CustomShellContextMenu : ContextMenuStrip
{
    public CustomShellContextMenu()
    {
        Renderer = new ToolStripProfessionalRenderer(new CustomShellContextMenuColorTable());
    }
}