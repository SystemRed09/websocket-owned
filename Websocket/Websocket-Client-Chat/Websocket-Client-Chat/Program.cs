using System.Drawing;
using System.Windows.Forms;

namespace Websocket_Client_Chat
{
    public delegate bool Message(string message);

    static class Program
    {
        /// <summary>
        /// The main entry point for the application. This is the composition
        /// root: it builds the Controller and the View and wires them together.
        /// Neither one holds a reference to the other's type.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            string name = GetName();

            ChatController c = new ChatController(name);
            ChatForm f = new ChatForm(name, c.MessageEntered);
            c.MessageReceived += f.MessageReceived;

            // Connect only once the form is on screen. The server pushes the
            // message history the instant we connect, and the form cannot
            // marshal anything to the UI thread before its handle exists.
            f.Shown += async (s, e) =>
            {
                try
                {
                    await c.ConnectAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Could not connect to the server.\n\n" + ex.Message +
                        "\n\nIs Websocket-Server running?",
                        "Connection failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            };

            f.FormClosed += async (s, e) => await c.DisconnectAsync();

            Application.Run(f);
        }

        // Ask for a name (that is a non-empty string)
        private static string GetName()
        {
            string name = "";
            do
            {
                while (InputBox("Name", "Enter user name:", ref name) != DialogResult.OK) ;
            }
            while (name == "");
            return name;
        }

        // Adapted from http://www.csharp-examples.net/inputbox/
        //
        // The original version positioned every control with hard-coded pixel
        // values (SetBounds) and a fixed 396x107 client size. Those numbers
        // assume 96 DPI and the old default font, so on a high-DPI display the
        // text renders larger while the window stays the same size, and the
        // text box and buttons get clipped.
        //
        // This version lets WinForms do the layout: the panels size themselves
        // from the current font and DPI, and the form grows to fit them.
        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            using Form form = new Form();

            Label label = new Label { Text = promptText, AutoSize = true };
            TextBox textBox = new TextBox { Text = value, Dock = DockStyle.Fill };
            Button buttonOk = new Button
            {
                Text = "OK", AutoSize = true, DialogResult = DialogResult.OK
            };
            Button buttonCancel = new Button
            {
                Text = "Cancel", AutoSize = true, DialogResult = DialogResult.Cancel
            };

            // Buttons flow from the right, so the first one added sits rightmost.
            FlowLayoutPanel buttons = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 10, 0, 0),
            };
            buttons.Controls.Add(buttonCancel);
            buttons.Controls.Add(buttonOk);

            TableLayoutPanel layout = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                RowCount = 3,
                Dock = DockStyle.Fill,
                Padding = new Padding(12),
            };
            layout.Controls.Add(label, 0, 0);
            layout.Controls.Add(textBox, 0, 1);
            layout.Controls.Add(buttons, 0, 2);

            form.Controls.Add(layout);

            form.Text = title;
            form.AutoScaleMode = AutoScaleMode.Font;   // scale with font and DPI
            form.AutoSize = true;
            form.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            form.MinimumSize = new Size(400, 0);       // keep it a sensible width
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }
    }
}
