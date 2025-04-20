using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Collections.Concurrent;
using System;

public class UDPFlash : MonoBehaviour
{
    UdpClient udpClient;
    public int port = 5055;

    public GameObject[] targetCubes = new GameObject[13];
    private Renderer[] rends = new Renderer[13];
    private float[] flashTimers = new float[13];

    private int[] noteToCubeMapping = new int[13] {
        57, 53, 51, 38, 37, 46, 26, 43, 47, 48, 44, 36, 29
    };

    private ConcurrentQueue<(int note, int velocity)> noteQueue = new ConcurrentQueue<(int, int)>();

    // Rate limiting
    private float lastProcessedTime = 0f;
    public float minInterval = 0.03f; // ~33 notes/sec

    // Flash time scaling
    public float minFlashDuration = 0.01f;
    public float maxFlashDuration = 0.1f;

    void Start()
    {
        for (int i = 0; i < targetCubes.Length; i++)
        {
            if (targetCubes[i] != null)
            {
                rends[i] = targetCubes[i].GetComponent<Renderer>();
                if (rends[i] != null)
                    rends[i].material.color = Color.yellow;
                else
                    Debug.LogWarning($"Renderer missing on targetCubes[{i}]");
            }
            else
            {
                Debug.LogWarning($"targetCubes[{i}] is not assigned!");
            }

            flashTimers[i] = 0f;
        }

        udpClient = new UdpClient(port);
        udpClient.Client.ReceiveBufferSize = 8192;
        udpClient.BeginReceive(OnReceive, null);
        Debug.Log("UDP listener started on port " + port);
    }

    void Update()
    {
        float currentTime = Time.time;

        if (noteQueue.TryPeek(out var peekedNote))
        {
            if (currentTime - lastProcessedTime >= minInterval)
            {
                if (noteQueue.TryDequeue(out var noteData))
                {
                    int cubeIndex = GetCubeIndexForNote(noteData.note);
                    if (cubeIndex != -1 && cubeIndex < rends.Length && rends[cubeIndex] != null)
                    {
                        float velocityScale = Mathf.Clamp01(noteData.velocity / 127f);
                        float duration = Mathf.Lerp(minFlashDuration, maxFlashDuration, velocityScale);

                        rends[cubeIndex].material.color = Color.red;
                        flashTimers[cubeIndex] = duration;

                        lastProcessedTime = currentTime;
                    }
                    else
                    {
                        Debug.LogWarning($"Received unmapped or invalid note: {noteData.note}");
                    }
                }
            }
        }

        for (int i = 0; i < flashTimers.Length; i++)
        {
            if (flashTimers[i] > 0f)
            {
                flashTimers[i] -= Time.deltaTime;
                if (flashTimers[i] <= 0f && rends[i] != null)
                {
                    rends[i].material.color = Color.yellow;
                }
            }
        }
    }

    void OnReceive(IAsyncResult result)
    {
        IPEndPoint ip = new IPEndPoint(IPAddress.Any, port);
        byte[] data;

        try
        {
            data = udpClient.EndReceive(result, ref ip);
        }
        catch (ObjectDisposedException)
        {
            Debug.LogWarning("UDP client was closed while receiving.");
            return;
        }

        string message = Encoding.UTF8.GetString(data);
        var noteInfo = ParseNoteAndVelocity(message);

        if (noteInfo.HasValue)
        {
            noteQueue.Enqueue(noteInfo.Value);
        }

        udpClient.BeginReceive(OnReceive, null);
    }

    (int note, int velocity)? ParseNoteAndVelocity(string message)
    {
        if (string.IsNullOrEmpty(message)) return null;

        int note = -1;
        int velocity = 64; // default if not found

        if (message.Contains("Note:"))
        {
            int noteIndex = message.IndexOf("Note:");
            string noteStr = message.Substring(noteIndex + 5).Trim();
            int spaceIndex = noteStr.IndexOf(' ');
            if (spaceIndex != -1)
                noteStr = noteStr.Substring(0, spaceIndex);
            int.TryParse(noteStr, out note);
        }

        if (message.Contains("Velocity:"))
        {
            int velIndex = message.IndexOf("Velocity:");
            string velStr = message.Substring(velIndex + 9).Trim();
            int spaceIndex = velStr.IndexOf(' ');
            if (spaceIndex != -1)
                velStr = velStr.Substring(0, spaceIndex);
            int.TryParse(velStr, out velocity);
        }

        if (note == -1) return null;

        return (note, velocity);
    }

    int GetCubeIndexForNote(int noteNumber)
    {
        for (int i = 0; i < noteToCubeMapping.Length; i++)
        {
            if (noteToCubeMapping[i] == noteNumber)
                return i;
        }
        return -1;
    }

    void OnApplicationQuit()
    {
        udpClient?.Close();
    }
}
