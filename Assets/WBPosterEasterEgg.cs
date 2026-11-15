using UnityEngine;

public class WBPosterEasterEgg : MonoBehaviour
{
    private GameObject noseObj;
    private AudioSource honkSource;
    private AudioClip honkClip;
    private Camera mainCam;
    private float noseBounce = 0f;

    void Start()
    {
        mainCam = Camera.main;
        honkSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        BuildHonkSound();
        BuildWBPosterOnWall();
    }

    void BuildHonkSound()
    {
        // Генерируем тот самый культовый писк носа из FNaF 1 ("ПИП!")
        int rate = 44100;
        float duration = 0.16f;
        int samples = Mathf.FloorToInt(rate * duration);
        float[] data = new float[samples];

        for (int i = 0; i != samples; i++)
        {
            float progress = (float)i / samples;
            float t = (float)i / rate;
            float freq = Mathf.Lerp(920f, 1380f, Mathf.Sin(progress * Mathf.PI));
            float wave = Mathf.Sin(2f * Mathf.PI * freq * t) + 0.35f * Mathf.Sin(4f * Mathf.PI * freq * t);
            float env = Mathf.Sin(progress * Mathf.PI);
            data[i] = wave * env * 0.45f;
        }

        honkClip = AudioClip.Create("FNaF_Nose_Honk", samples, 1, rate, false);
        honkClip.SetData(data, 0);
    }

    void BuildWBPosterOnWall()
    {
        GameObject old = GameObject.Find("WB_EasterEgg_Poster");
        if (old != null) Destroy(old);

        // Вешаем плакат на левую стену рядом со стойкой выдачи (X = -2.88, хорошо виден с кресла)
        GameObject poster = new GameObject("WB_EasterEgg_Poster");
        poster.transform.position = new Vector3(-2.88f, 1.78f, -1.35f);
        poster.transform.rotation = Quaternion.Euler(0f, -90f, 0f);

        Material frameMat = WBProceduralAnimator.MakeMat(new Color(0.12f, 0.02f, 0.16f));
        Material bgMat = WBProceduralAnimator.MakeMat(new Color(0.58f, 0.10f, 0.65f));
        Material faceMat = WBProceduralAnimator.MakeMat(new Color(0.95f, 0.85f, 0.72f));
        Material eyeMat = WBProceduralAnimator.MakeMat(Color.black);
        Material redNoseMat = WBProceduralAnimator.MakeMat(Color.red, true, 1.4f);

        // Рамка и фон плаката
        WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Poster_Frame", Vector3.zero, new Vector3(0.76f, 0.96f, 0.03f), frameMat, poster.transform);
        WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Poster_Bg", new Vector3(0f, 0f, -0.01f), new Vector3(0.68f, 0.88f, 0.02f), bgMat, poster.transform);

        // Забавная мордочка талисмана WB на плакате
        WBProceduralAnimator.MakePart(PrimitiveType.Sphere, "Mascot_Head", new Vector3(0f, 0.05f, -0.025f), new Vector3(0.42f, 0.44f, 0.04f), faceMat, poster.transform);
        WBProceduralAnimator.MakePart(PrimitiveType.Sphere, "Mascot_EyeL", new Vector3(-0.08f, 0.12f, -0.045f), new Vector3(0.06f, 0.08f, 0.02f), eyeMat, poster.transform);
        WBProceduralAnimator.MakePart(PrimitiveType.Sphere, "Mascot_EyeR", new Vector3(0.08f, 0.12f, -0.045f), new Vector3(0.06f, 0.08f, 0.02f), eyeMat, poster.transform);

        // Кликабельный красный нос-пасхалка!
        noseObj = WBProceduralAnimator.MakePart(PrimitiveType.Sphere, "Poster_Honk_Nose", new Vector3(0f, 0.03f, -0.055f), new Vector3(0.11f, 0.11f, 0.06f), redNoseMat, poster.transform);
    }

    void Update()
    {
        if (noseObj != null && !Mathf.Approximately(noseBounce, 0f))
        {
            noseBounce = Mathf.MoveTowards(noseBounce, 0f, Time.deltaTime * 5f);
            float scale = 0.11f + Mathf.Sin(noseBounce * Mathf.PI) * 0.04f;
            noseObj.transform.localScale = new Vector3(scale, scale, 0.06f);
        }

        WBGameplayManager gm = (WBGameplayManager)FindAnyObjectByType(typeof(WBGameplayManager));
        if (gm != null && !gm.enabled) return;

        if (Input.GetMouseButtonDown(0) && mainCam != null && noseObj != null)
        {
            Ray ray = new Ray(mainCam.transform.position, mainCam.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 4.5f))
            {
                if (hit.transform.gameObject == noseObj || hit.transform.name == "Poster_Honk_Nose")
                {
                    noseBounce = 1f;
                    if (honkSource != null && honkClip != null)
                    {
                        honkSource.PlayOneShot(honkClip, 0.9f);
                    }
                }
            }
        }
    }
}
