using UnityEngine;

public class WBSoundManager : MonoBehaviour
{
    public static WBSoundManager Instance;

    private AudioSource sfxSource;
    private AudioSource ambientSource;

    private AudioClip beepClip;
    private AudioClip powerDownClip;
    private AudioClip footstepClip;
    private AudioClip jumpscareClip;
    private AudioClip ambientHumClip;

    private bool lastPowerState = true;
    private bool lastHoldingScanner = false;
    private float stepTimer = 0f;

    void Awake()
    {
        Instance = this;
        sfxSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        ambientSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));

        BuildAllAudioClips();

        ambientSource.clip = ambientHumClip;
        ambientSource.loop = true;
        ambientSource.volume = 0.14f;
        ambientSource.Play();
    }

    void BuildAllAudioClips()
    {
        // 1. Звук "ПИК" сканера штрихкодов (высокий четкий сигнал 2400 Гц)
        beepClip = MakeToneClip("ScannerBeep", 0.14f, 2400f, 0f, false);

        // 2. Звук вырубания генератора (падающий гул электричества)
        powerDownClip = MakePowerDownClip();

        // 3. Глухой шаг в темноте
        footstepClip = MakeFootstepClip();

        // 4. Резкий скример
        jumpscareClip = MakeJumpscareClip();

        // 5. Тихий ночной гул ламп и холодильников ПВЗ
        ambientHumClip = MakeAmbientHumClip();
    }

    AudioClip MakeToneClip(string name, float duration, float freq, float endFreq, bool noise)
    {
        int rate = 44100;
        int samples = Mathf.FloorToInt(rate * duration);
        float[] data = new float[samples];
        System.Random rnd = new System.Random();

        for (int i = 0; i != samples; i++)
        {
            float t = (float)i / rate;
            float progress = (float)i / samples;
            float currentFreq = (endFreq == 0f) ? freq : Mathf.Lerp(freq, endFreq, progress);
            float val = Mathf.Sin(2f * Mathf.PI * currentFreq * t);
            if (noise) val += (float)(rnd.NextDouble() * 2.0 - 1.0) * 0.4f;
            float envelope = 1f - progress;
            data[i] = val * envelope * 0.35f;
        }

        AudioClip clip = AudioClip.Create(name, samples, 1, rate, false);
        clip.SetData(data, 0);
        return clip;
    }

    AudioClip MakePowerDownClip()
    {
        int rate = 44100;
        float duration = 1.1f;
        int samples = Mathf.FloorToInt(rate * duration);
        float[] data = new float[samples];
        float phase = 0f;

        for (int i = 0; i != samples; i++)
        {
            float progress = (float)i / samples;
            float freq = Mathf.Lerp(220f, 35f, progress);
            phase += (2f * Mathf.PI * freq) / rate;
            float val = Mathf.Sign(Mathf.Sin(phase)) * 0.25f * (1f - progress);
            data[i] = val;
        }

        AudioClip clip = AudioClip.Create("PowerDown", samples, 1, rate, false);
        clip.SetData(data, 0);
        return clip;
    }

    AudioClip MakeFootstepClip()
    {
        int rate = 44100;
        float duration = 0.22f;
        int samples = Mathf.FloorToInt(rate * duration);
        float[] data = new float[samples];
        System.Random rnd = new System.Random();

        for (int i = 0; i != samples; i++)
        {
            float progress = (float)i / samples;
            float t = (float)i / rate;
            float thump = Mathf.Sin(2f * Mathf.PI * 65f * t) * (1f - progress);
            float grit = (float)(rnd.NextDouble() * 2.0 - 1.0) * 0.15f * (1f - progress);
            data[i] = (thump + grit) * 0.45f;
        }

        AudioClip clip = AudioClip.Create("Footstep", samples, 1, rate, false);
        clip.SetData(data, 0);
        return clip;
    }

    AudioClip MakeJumpscareClip()
    {
        int rate = 44100;
        float duration = 1.4f;
        int samples = Mathf.FloorToInt(rate * duration);
        float[] data = new float[samples];
        System.Random rnd = new System.Random();

        for (int i = 0; i != samples; i++)
        {
            float progress = (float)i / samples;
            float t = (float)i / rate;
            float scream1 = Mathf.Sign(Mathf.Sin(2f * Mathf.PI * 480f * t));
            float scream2 = Mathf.Sin(2f * Mathf.PI * 735f * t);
            float roar = (float)(rnd.NextDouble() * 2.0 - 1.0);
            data[i] = (scream1 * 0.35f + scream2 * 0.35f + roar * 0.4f) * (1f - progress * 0.7f);
        }

        AudioClip clip = AudioClip.Create("Jumpscare", samples, 1, rate, false);
        clip.SetData(data, 0);
        return clip;
    }

    AudioClip MakeAmbientHumClip()
    {
        int rate = 44100;
        float duration = 2.0f;
        int samples = Mathf.FloorToInt(rate * duration);
        float[] data = new float[samples];

        for (int i = 0; i != samples; i++)
        {
            float t = (float)i / rate;
            data[i] = Mathf.Sin(2f * Mathf.PI * 50f * t) * 0.25f + Mathf.Sin(2f * Mathf.PI * 100f * t) * 0.1f;
        }

        AudioClip clip = AudioClip.Create("AmbientHum", samples, 1, rate, false);
        clip.SetData(data, 0);
        return clip;
    }

    void Update()
    {
        // Следим за генератором (если свет вырубился — играем звук сбоя)
        WBPowerGenerator gen = (WBPowerGenerator)FindAnyObjectByType(typeof(WBPowerGenerator));
        if (gen != null)
        {
            bool powerNow = gen.IsPowerWorking();
            if (lastPowerState && !powerNow)
            {
                PlayPowerDown();
            }
            lastPowerState = powerNow;
            ambientSource.volume = powerNow ? 0.14f : 0.02f;
        }

        // Звук пика при нажатии на сканер
        WBGameplayManager gm = (WBGameplayManager)FindAnyObjectByType(typeof(WBGameplayManager));
        if (gm != null && gm.enabled)
        {
            bool holding = Input.GetMouseButton(0);
            if (holding && !lastHoldingScanner)
            {
                PlayBeep();
            }
            lastHoldingScanner = holding;
        }

        // Периодические жуткие шаги со склада, когда свет выключен
        if (!lastPowerState)
        {
            stepTimer += Time.deltaTime;
            if (Mathf.Max(stepTimer, 3.2f) == stepTimer)
            {
                stepTimer = 0f;
                PlayFootstep();
            }
        }
    }

    public void PlayBeep() { if (sfxSource != null && beepClip != null) sfxSource.PlayOneShot(beepClip, 0.6f); }
    public void PlayPowerDown() { if (sfxSource != null && powerDownClip != null) sfxSource.PlayOneShot(powerDownClip, 0.8f); }
    public void PlayFootstep() { if (sfxSource != null && footstepClip != null) sfxSource.PlayOneShot(footstepClip, 0.55f); }
    public void PlayJumpscare() { if (sfxSource != null && jumpscareClip != null) sfxSource.PlayOneShot(jumpscareClip, 1.0f); }
}
