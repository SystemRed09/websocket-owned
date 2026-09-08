using System.Windows.Forms;

namespace Websocket_Client_Chat
{
    public partial class ChatForm : Form
    {
        public ChatForm(string name, Message newMessageHandler)
        {
            InitializeComponent();

            // Add name in the form's title
            Text = "Chat: " + name;

            // Whenever the Enter key is pressed inside the messageTextBox,
            //   pass the message to newMessageHandler
            //   if successfully handled, clear out the messageTextBox
            messageTextBox.KeyDown += (o, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string message = messageTextBox.Text;
                    if (newMessageHandler(message))
                    {
                        messageTextBox.Text = "";
                    }
                }
            };

            // Make sure to focus on messageTextBox when the form is loaded
            messageTextBox.Select();
        }

        public bool MessageReceived(string message)
        {
            // Add message to messageListBox and scroll to bottom.
            // Invoke is used to make sure that this is done in the GUI thread -
            // SignalR delivers callbacks on a background thread, exactly as
            // websocket-sharp did, so this is still required.
            Invoke(new Action(() => messageListBox.TopIndex = messageListBox.Items.Add(message)));

            return true;
        }
    }
}
