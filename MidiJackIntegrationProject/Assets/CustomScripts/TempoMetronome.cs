using UnityEngine;

public class TempoMetronome : MonoBehaviour
{
    public float bpm = 100f;
    public AudioClip metronomeClip;
    private AudioSource audioSource;

    private float nextBeatTime;
    private float interval;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        interval = 60f / bpm;
        nextBeatTime = Time.time + interval;
    }

    void Update()
    {
        if (Time.time >= nextBeatTime)
        {
            if (metronomeClip != null)
                audioSource.PlayOneShot(metronomeClip);

            nextBeatTime += interval;
        }
    }
}
