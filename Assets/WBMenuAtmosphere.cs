using UnityEngine;

public class WBMenuAtmosphere : MonoBehaviour
{
    private AudioSource rainSource;
    private AudioSource thunderSource;
    private Light lightningLight;
    private Light storageFlickerLamp;
    private GameObject rainParticlesObj;

    private float lightningTimer = 3.5f;
    private float flashIntensity = 0f;
    private float thunderDelay = -1f;
    private float lampFlickerTimer = 0f;

    public void StartAtmosphere()
    {
        SetupLights();
        SetupRainParticles();
        SetupWeatherAudio();
    }

    public void StopAtmosphere()
    {
        if (rainSource != null) rainSource.Stop();
        if (thunderSource != null) thunderSource.Stop();
        if (lightningLight != null) Destroy(lightningLight.gameObject);
        if (storageFlickerLamp != null) Destroy(storageFlickerLamp.gameObject);
        if (rainParticlesObj != null) Destroy(rainParticlesObj);
        Destroy(this);
    }

    void SetupLights()
    {
        // Мерцающая аварийная лампа в глубине тёмного склада WB
        GameObject lampObj = new GameObject("Menu_Storage_FlickerLamp");
        lampObj.transform.position = new Vector3(0f, 2.85f, 5.2f);
        storageFlickerLamp = (Light)lampObj.AddComponent(typeof(Light));
        storageFlickerLamp.type = LightType.Point;
        storageFlickerLamp.range = 10f;
        storageFlickerLamp.intensity = 2.2f;
        storageFlickerLamp.color = new Color(0.85f, 0.72f, 1f);

        GameObject flashObj = new GameObject("Menu_Lightning_Flash");
        lightningLight = (Light)flashObj.AddComponent(typeof(Light));
        lightningLight.type = LightType.Directional;
        lightningLight.transform.rotation = Quaternion.Euler(45f, -30f, 0f);
        lightningLight.color = new Color(0.65f, 0.8f, 1f);
        lightningLight.intensity = 0f;
    }

    void SetupRainParticles()
    {
        rainParticlesObj = new GameObject("Menu_Rain_FX");
        rainParticlesObj.transform.position = new Vector3(0f, 3.2f, 5.5f);
        ParticleSystem ps = (ParticleSystem)rainParticlesObj.AddComponent(typeof(ParticleSystem));

        var main = ps.main;
        main.startLifetime = 1.2f;
        main.startSpeed = 14f;
        main.startSize = 0.03f;
        main.maxParticles = 500;
        main.startColor = new Color(0.75f, 0.85f, 1f, 0.35f);

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(8f, 0.2f, 10f);
        rainParticlesObj.transform.rotation = Quaternion.Euler(105f, 0f, 0f);
    }

    void SetupWeatherAudio()
    {
        rainSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        thunderSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));

        int rate = 44100;
        System.Random rnd = new System.Random();

        int rainSamples = rate * 2;
        float[] rainData = new float[rainSamples];
        float lastVal = 0f;
        for (int i = 0; i != rainSamples; i++)
        {
            float white = (float)(rnd.NextDouble() * 2.0 - 1.0);
            lastVal = Mathf.Lerp(lastVal, white, 0.18f);
            rainData[i] = lastVal * 0.14f;
        }
        AudioClip rainClip = AudioClip.Create("MenuRain", rainSamples, 1, rate, false);
        rainClip.SetData(rainData, 0);

        rainSource.clip = rainClip;
        rainSource.loop = true;
        rainSource.volume = 0.45f;
        rainSource.Play();

        int thunderSamples = rate * 3;
        float[] thunderData = new float[thunderSamples];
        float lowPass = 0f;
        for (int i = 0; i != thunderSamples; i++)
        {
            float progress = (float)i / thunderSamples;
            float white = (float)(rnd.NextDouble() * 2.0 - 1.0);
            lowPass = Mathf.Lerp(lowPass, white, 0.025f);
            float env = Mathf.Sin(Mathf.Min(progress * 4f, 1f) * Mathf.PI * 0.5f) * (1f - progress);
            thunderData[i] = lowPass * env * 1.3f;
        }
        AudioClip thunderClip = AudioClip.Create("MenuThunder", thunderSamples, 1, rate, false);
        thunderClip.SetData(thunderData, 0);
        thunderSource.clip = thunderClip;
    }

    void Update()
    {
        if (storageFlickerLamp != null)
        {
            lampFlickerTimer -= Time.deltaTime;
            if (Mathf.Min(lampFlickerTimer, 0f) == lampFlickerTimer)
            {
                bool isOn = (Random.Range(0, 10) != 0 && Random.Range(0, 10) != 1);
                storageFlickerLamp.enabled = isOn;
                storageFlickerLamp.intensity = isOn ? Random.Range(1.4f, 2.5f) : 0f;
                lampFlickerLampTime();
            }
        }

        lightningTimer -= Time.deltaTime;
        if (Mathf.Min(lightningTimer, 0f) == lightningTimer)
        {
            lightningTimer = Random.Range(5.5f, 10.5f);
            flashIntensity = 2.4f;
            thunderDelay = Random.Range(0.35f, 0.9f);
        }

        if (lightningLight != null)
        {
            if (Mathf.Max(flashIntensity, 0.05f) == flashIntensity)
            {
                lightningLight.enabled = true;
                float jitter = (Mathf.FloorToInt(Time.time * 28f) % 2 == 0) ? 1f : 0.35f;
                lightningLight.intensity = flashIntensity * jitter;
                flashIntensity -= Time.deltaTime * 5.5f;
            }
            else
            {
                lightningLight.enabled = false;
            }
        }

        if (Mathf.Max(thunderDelay, 0f) == thunderDelay)
        {
            thunderDelay -= Time.deltaTime;
            if (Mathf.Min(thunderDelay, 0f) == thunderDelay)
            {
                thunderDelay = -1f;
                if (thunderSource != null) thunderSource.Play();
            }
        }
    }

    void lampFlickerLampTime()
    {
        lampFlickerTimer = storageFlickerLamp.enabled ? Random.Range(0.06f, 0.55f) : Random.Range(0.04f, 0.16f);
    }
}
