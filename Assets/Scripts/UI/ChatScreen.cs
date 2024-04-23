using System.Net;
using UnityEngine.UI;

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
        if (NetworkManager.Instance.isServer)
        {
            NetworkManager.Instance.Broadcast(data);
        }

        NetConsole message = new();
        
        messages.text += message.Deserialize(data);
    }

    private void OnEndEdit(string str)
    {
        if (inputMessage.text != "")
        {
            NetConsole message = new();
            message.data = str;
            
            if (NetworkManager.Instance.isServer)
            {
                NetworkManager.Instance.Broadcast(message.Serialize());
                messages.text += inputMessage.text + System.Environment.NewLine;
            }
            else
            {
                NetworkManager.Instance.SendToServer(message.Serialize());
            }

            inputMessage.ActivateInputField();
            inputMessage.Select();
            inputMessage.text = "";
        }

    }
}
