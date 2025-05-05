using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using TMPro; // <- Make sure you add this!
using Unity;

public class UdpNoteReceiver : MonoBehaviour
{
    public int listenPort = 5055;
    public Button acceptButton;
    public TextMeshProUGUI metricsText; // <-- Drag your Metrics Text here!

    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;

    private bool isReceiving = false;

    [Serializable]
    public struct NoteEvent
    {
        public int note;
        public int velocity;
        public double timestamp;
    }

    public List<NoteEvent> noteEvents = new List<NoteEvent>();

    private void Start()
    {
        if (acceptButton != null)
            acceptButton.onClick.AddListener(StartReceiving);
        else
            Debug.LogError("Accept Button not assigned!");
    }

    public void StartReceiving()
    {
        if (isReceiving) return;

        udpClient = new UdpClient(listenPort);
        remoteEndPoint = new IPEndPoint(IPAddress.Any, listenPort);
        isReceiving = true;
        udpClient.BeginReceive(ReceiveCallback, null);
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        if (!isReceiving) return;

        byte[] data = udpClient.EndReceive(ar, ref remoteEndPoint);
        string message = Encoding.UTF8.GetString(data);

        Debug.Log($"Received: {message}");

        if (message.StartsWith("Note:"))
        {
            HandleNoteMessage(message);
        }
        else if (message.StartsWith("Text:"))
        {
            HandleTextMessage(message);
        }

        if (isReceiving)
            udpClient.BeginReceive(ReceiveCallback, null);
    }

    private void HandleNoteMessage(string message)
    {
        try
        {
            int note = 0;
            int velocity = 0;
            string[] parts = message.Split(' ');

            foreach (string part in parts)
            {
                if (part.StartsWith("Note:"))
                    note = int.Parse(part.Replace("Note:", "").Trim());
                else if (part.StartsWith("Velocity:"))
                    velocity = int.Parse(part.Replace("Velocity:", "").Trim());
            }

            double currentTime = GetUnixTimestamp();
            NoteEvent noteEvent = new NoteEvent { note = note, velocity = velocity, timestamp = currentTime };
            noteEvents.Add(noteEvent);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to parse Note message: {ex.Message}");
        }
    }

    private void HandleTextMessage(string message)
    {
        string newText = message.Replace("Text:", "").Trim();

        // Update the Metrics Text field safely on the main thread
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            if (metricsText != null)
            {
                metricsText.text = newText;
                Debug.Log($"Updated Metrics text: {newText}");
            }
        });
    }

    private double GetUnixTimestamp()
    {
        return (DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds;
    }

    public void StopReceiving()
    {
        if (!isReceiving) return;
        isReceiving = false;
        udpClient?.Close();
        udpClient = null;
    }
}
