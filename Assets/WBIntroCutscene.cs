using UnityEngine;

public class WBIntroCutscene : MonoBehaviour
{
    public WBGameplayManager gameplayManager;
    public Camera mainCam;

    private float timer = 0f;
    private bool isPlaying = true;
    private string currentText = "";
    private float blackAlpha = 1f;
    public bool IsCutsceneActive() { return enabled && isPlaying; }

    private GameObject storySet;
    private Transform phoneTr;
    private Transform carTr;
    private Transform roadLinesTr;
    private AudioSource cutsceneAudio;
    private AudioClip ringClip;
    private float ringTimer = 0f;

    void Start()
    {
        if (mainCam == null) mainCam = Camera.main;
        if (gameplayManager == null) gameplayManager = (WBGameplayManager)FindAnyObjectByType(typeof(WBGameplayManager));

        if (gameplayManager != null) gameplayManager.enabled = false;

        storySet = WBCutsceneActors.BuildHomeAndCarSet(out phoneTr, out carTr, out roadLinesTr);
        SetupRingSound();
    }

    void SetupRingSound()
    {
        cutsceneAudio = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        int rate = 44100;
        int samples = Mathf.FloorToInt(rate * 0.4f);
        float[] data = new float[samples];
        for (int i = 0; i != samples; i++)
        {
            float t = (float)i / rate;
            float tone = Mathf.Sin(2f * Mathf.PI * 880f * t) + Mathf.Sin(2f * Mathf.PI * 1100f * t);
            data[i] = tone * 0.15f;
        }
        ringClip = AudioClip.Create("PhoneRing", samples, 1, rate, false);
        ringClip.SetData(data, 0);
    }

    void Update()
    {
        if (!isPlaying) return;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
        {
            EndCutscene();
            return;
        }

        timer += Time.deltaTime;
        int act = Mathf.FloorToInt(timer / 4.5f);

        switch (act)
        {
            case 0:
                PlayAct1_HomeCall();
                break;
            case 1:
                PlayAct2_DrivingCar();
                break;
            case 2:
                PlayAct3_ArriveAtWB();
                break;
            default:
                EndCutscene();
                break;
        }
    }

    // АКТ 1 (0 - 4.5 сек): Дома ночью, вибрирует телефон и звонит менеджер WB
    void PlayAct1_HomeCall()
    {
        float p = timer / 4.5f;
        blackAlpha = Mathf.Clamp01(1.0f - timer * 1.5f);

        if (mainCam != null)
        {
            mainCam.transform.position = Vector3.Lerp(new Vector3(0f, 51.45f, 0.35f), new Vector3(0.08f, 51.18f, 0.72f), Mathf.SmoothStep(0f, 1f, p));
            mainCam.transform.rotation = Quaternion.Slerp(Quaternion.Euler(22f, 5f, 0f), Quaternion.Euler(48f, 8f, 0f), Mathf.SmoothStep(0f, 1f, p));
        }

        // Вибрация и гудки телефона на столе
        if (phoneTr != null)
        {
            float vib = Mathf.Sin(timer * 65f) * 0.008f;
            phoneTr.position = new Vector3(0.15f + vib, 50.82f, 1.05f);
        }

        ringTimer -= Time.deltaTime;
        if (Mathf.Min(ringTimer, 0f) == ringTimer)
        {
            ringTimer = 1.4f;
            if (cutsceneAudio != null && ringClip != null) cutsceneAudio.PlayOneShot(ringClip, 0.6f);
        }

        currentText = "📞 Входящий звонок (Менеджер WB): «Алло! Ночной сменщик слег, срочно прыгай в машину и дуй на пункт выдачи!»";
    }

    // АКТ 2 (4.5 - 9.0 сек): Едем ночью на машине по трассе к ПВЗ
    void PlayAct2_DrivingCar()
    {
        float localT = timer - 4.5f;
        blackAlpha = Mathf.Clamp01(1f - localT * 3f);

        if (mainCam != null)
        {
            float bump = Mathf.Sin(localT * 18f) * 0.015f;
            float turn = Mathf.Sin(localT * 1.8f) * 0.08f;
            mainCam.transform.position = new Vector3(-0.35f + turn, 101.28f + bump, 0.15f);
            mainCam.transform.rotation = Quaternion.Euler(4f + bump * 20f, turn * 25f, -turn * 15f);
        }

        // Дорожная разметка летит навстречу машине
        if (roadLinesTr != null)
        {
            float offset = (localT * 26f) % 6f;
            roadLinesTr.position = new Vector3(0f, 100f, -offset);
        }

        currentText = "🚗 Вы сели за руль и мчитесь по ночному городу к пункту выдачи Wildberries...";
    }

    // АКТ 3 (9.0 - 13.5 сек): Приехали на работу, заходим за стойку ПВЗ
    void PlayAct3_ArriveAtWB()
    {
        float localT = timer - 9.0f;
        float p = Mathf.SmoothStep(0f, 1f, localT / 4.5f);
        blackAlpha = Mathf.Clamp01(1f - localT * 2.5f);

        if (mainCam != null)
        {
            // Идем от входа за рабочую стойку и садимся в кресло
            Vector3 enterPos = new Vector3(-1.8f, 1.65f, -3.5f);
            Vector3 chairPos = new Vector3(0f, 1.65f, -1.85f);
            mainCam.transform.position = Vector3.Lerp(enterPos, chairPos, p);
            mainCam.transform.rotation = Quaternion.Slerp(Quaternion.Euler(4f, 35f, 0f), Quaternion.identity, p);
        }

        currentText = "🏢 Вы на месте. Смена началась — следите за заказами, камерами и темнотой до 06:00 утра!";
    }

    void EndCutscene()
    {
        isPlaying = false;
        blackAlpha = 0f;
        currentText = "";

        if (storySet != null) Destroy(storySet);

        if (mainCam != null)
        {
            mainCam.transform.position = new Vector3(0f, 1.65f, -1.85f);
            mainCam.transform.rotation = Quaternion.identity;
        }

        if (gameplayManager != null) gameplayManager.enabled = true;
    }

    void OnGUI()
    {
        if (!isPlaying && Mathf.Approximately(blackAlpha, 0f)) return;

        if (!Mathf.Approximately(blackAlpha, 0f))
        {
            GUI.color = new Color(0f, 0f, 0f, blackAlpha);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }

        GUI.color = new Color(0.14f, 0.02f, 0.17f, 0.95f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, 54f), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(0, Screen.height - 120f, Screen.width, 120f), Texture2D.whiteTexture);
        GUI.color = Color.white;

        if (!string.IsNullOrEmpty(currentText))
        {
            GUIStyle st = new GUIStyle(GUI.skin.label);
            st.fontSize = 20;
            st.fontStyle = FontStyle.Bold;
            st.alignment = TextAnchor.MiddleCenter;
            st.wordWrap = true;
            st.normal.textColor = Color.white;
            GUI.Label(new Rect(40f, Screen.height - 112f, Screen.width - 80f, 78f), currentText, st);
        }

        GUIStyle hint = new GUIStyle(GUI.skin.label);
        hint.fontSize = 13;
        hint.alignment = TextAnchor.MiddleCenter;
        hint.normal.textColor = new Color(0.8f, 0.6f, 0.85f);
        GUI.Label(new Rect(0, Screen.height - 28f, Screen.width, 22f), "Нажмите [Enter], чтобы пропустить вступление", hint);
    }
}
