using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPFlash : MonoBehaviour
{
    UdpClient udpClient;
    public int port = 5055;
    public GameObject targetCube;
    private Renderer rend;

    void Start()
    {
        rend = targetCube.GetComponent<Renderer>();
        if (rend != null)
            rend.material.color = Color.yellow; // waiting for signal

        udpClient = new UdpClient(port);
        udpClient.BeginReceive(OnReceive, null);
        Debug.Log("UDP listener started on port " + port);
    }

    void OnReceive(System.IAsyncResult result)
    {
        IPEndPoint ip = new IPEndPoint(IPAddress.Any, port);
        byte[] data = udpClient.EndReceive(result, ref ip);
        string message = Encoding.UTF8.GetString(data);
        Debug.Log("Received UDP: " + message);

        if (message == "trigger")
            FlashCube();

        udpClient.BeginReceive(OnReceive, null);
    }

    void FlashCube()
    {
        if (rend != null)
        {
            rend.material.color = Color.red;
            Invoke("ResetColor", 1f);
        }
    }

    void ResetColor()
    {
        if (rend != null)
            rend.material.color = Color.white;
    }

    void OnApplicationQuit()
    {
        udpClient?.Close();
    }
}
