using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PhoneCallEffect : MonoBehaviour
{
    [Header("Настройки старого телефона")]
    public bool playOnStart = true;
    public float delayBeforeRing = 2.0f;
    [Range(0f, 0.05f)] public float staticNoiseLevel = 0.008f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Добавляем фильтры телефонной линии автоматически
        AudioHighPassFilter highPass = gameObject.AddComponent<AudioHighPassFilter>();
        highPass.cutoffFrequency = 550f; // Режем басы

        AudioLowPassFilter lowPass = gameObject.AddComponent<AudioLowPassFilter>();
        lowPass.cutoffFrequency = 2600f; // Режем высокие частоты (эффект трубки)

        AudioDistortionFilter distortion = gameObject.AddComponent<AudioDistortionFilter>();
        distortion.distortionLevel = 0.25f; // Легкий хрип динамика

        if (playOnStart && audioSource.clip != null)
        {
            audioSource.PlayDelayed(delayBeforeRing);
        }
    }

    // Добавляем легкий шум старой записи поверх голоса
    void OnAudioFilterRead(float[] data, int channels)
    {
        if (audioSource == null || !audioSource.isPlaying) return;

        System.Random rand = new System.Random();
        for (int i = 0; i < data.Length; i++)
        {
            float noise = (float)(rand.NextDouble() * 2.0 - 1.0) * staticNoiseLevel;
            data[i] += noise;
        }
    }
}
