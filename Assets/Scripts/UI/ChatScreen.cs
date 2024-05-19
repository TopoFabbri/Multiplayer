using Network;
using Network.MessageTypes;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class ChatScreen : MonoBehaviourSingleton<ChatScreen>
    {
        public Text messages;
        public InputField inputMessage;

        protected override void Initialize()
        {
            inputMessage.onEndEdit.AddListener(OnEndEdit);

            this.gameObject.SetActive(false);

            NetworkManager.Instance.OnReceiveEvent += OnReceiveDataEvent;
        }

        private void OnReceiveDataEvent(byte[] data)
        {
            NetConsole message = new();
            
            message.data = message.Deserialize(data);
            data = message.Serialize(true);
            
            if (NetworkManager.Instance.isServer)
                NetworkManager.Instance.Broadcast(data);
            
            message = new();
        
            messages.text += message.Deserialize(data) + System.Environment.NewLine;
        }

        private void OnEndEdit(string str)
        {
            if (inputMessage.text == "") return;
        
            NetConsole message = new();
            message.data = str;
            
            if (NetworkManager.Instance.isServer)
            {
                NetworkManager.Instance.Broadcast(message.Serialize(true));
                messages.text += inputMessage.text + System.Environment.NewLine;
            }
            else
            {
                NetworkManager.Instance.SendToServer(message.Serialize(false));
            }

            inputMessage.ActivateInputField();
            inputMessage.Select();
            inputMessage.text = "";
        }
    }
}
