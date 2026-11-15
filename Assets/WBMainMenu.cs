using UnityEngine;

public class WBMainMenu : MonoBehaviour
{
    private bool inMenu = true;
    private bool showControls = false;

    private WBIntroCutscene introCutscene;
    private WBGameplayManager gameplayManager;
    private WBMenuAtmosphere atmosphere;
    private Camera mainCam;

    public bool IsMenuActive() { return inMenu; }

    void Start()
    {
        mainCam = Camera.main;
        introCutscene = (WBIntroCutscene)FindAnyObjectByType(typeof(WBIntroCutscene));
        gameplayManager = (WBGameplayManager)FindAnyObjectByType(typeof(WBGameplayManager));

        if (introCutscene != null) introCutscene.enabled = false;
        if (gameplayManager != null) gameplayManager.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        atmosphere = (WBMenuAtmosphere)gameObject.AddComponent(typeof(WBMenuAtmosphere));
        atmosphere.StartAtmosphere();
    }

    void Update()
    {
        if (!inMenu || mainCam == null) return;

        // Камера направлена прямо в мрачный склад WB с мерцающим светом и стеллажами
        float swayX = Mathf.Sin(Time.time * 0.55f) * 0.25f;
        float swayY = Mathf.Cos(Time.time * 0.75f) * 0.05f;
        mainCam.transform.position = new Vector3(swayX, 1.85f + swayY, 1.8f);
        mainCam.transform.rotation = Quaternion.Euler(7f, swayX * 18f, 0f);
    }

    void BeginShift(bool resetToNight1)
    {
        if (resetToNight1)
        {
            WBNightShiftManager.StartNewGame();
        }

        inMenu = false;
        if (atmosphere != null)
        {
            atmosphere.StopAtmosphere();
        }

        if (introCutscene != null)
        {
            introCutscene.enabled = true;
        }
        else if (gameplayManager != null)
        {
            gameplayManager.enabled = true;
        }
    }

    void OnGUI()
    {
        if (!inMenu) return;

        GUI.color = new Color(0.06f, 0.01f, 0.09f, 0.72f);
        GUI.DrawTexture(new Rect(0f, 0f, 460f, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;

        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontSize = 38;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.alignment = TextAnchor.UpperLeft;
        titleStyle.wordWrap = true;
        titleStyle.normal.textColor = Color.white;

        GUIStyle subStyle = new GUIStyle(GUI.skin.label);
        subStyle.fontSize = 18;
        subStyle.fontStyle = FontStyle.Bold;
        subStyle.alignment = TextAnchor.UpperLeft;
        subStyle.normal.textColor = new Color(0.85f, 0.35f, 0.9f);

        GUI.Label(new Rect(38f, 40f, 400f, 130f), "FIVE NIGHTS\nAT WILDBERRIES", titleStyle);
        GUI.Label(new Rect(40f, 175f, 400f, 35f), "СКЛАД ПВЗ • ТЕКУЩАЯ НОЧЬ: " + WBNightShiftManager.CurrentNight, subStyle);

        int savedNight = WBNightShiftManager.CurrentNight;

        if (savedNight != 1)
        {
            GUI.backgroundColor = new Color(0.2f, 0.8f, 0.35f);
            if (GUI.Button(new Rect(38f, 235f, 320f, 54f), "▶  ПРОДОЛЖИТЬ (НОЧЬ " + savedNight + ")"))
            {
                BeginShift(false);
            }

            GUI.backgroundColor = new Color(0.72f, 0.12f, 0.78f);
            if (GUI.Button(new Rect(38f, 302f, 320f, 48f), "↺  НОВАЯ ИГРА (НОЧЬ 1)"))
            {
                BeginShift(true);
            }
        }
        else
        {
            GUI.backgroundColor = new Color(0.72f, 0.12f, 0.78f);
            if (GUI.Button(new Rect(38f, 245f, 320f, 58f), "▶  НАЧАТЬ НОЧЬ 1"))
            {
                BeginShift(true);
            }
        }

        GUI.backgroundColor = new Color(0.28f, 0.08f, 0.32f);
        if (GUI.Button(new Rect(38f, 368f, 320f, 46f), showControls ? "✖  СКРЫТЬ УПРАВЛЕНИЕ" : "ℹ  УПРАВЛЕНИЕ"))
        {
            showControls = !showControls;
        }

        GUI.backgroundColor = new Color(0.45f, 0.1f, 0.15f);
        if (GUI.Button(new Rect(38f, 428f, 320f, 46f), "⏻  ВЫХОД"))
        {
            Application.Quit();
        }
        GUI.backgroundColor = Color.white;

        if (showControls)
        {
            float px = 485f;
            float py = Screen.height - 210f;
            float pw = Screen.width - 515f;
            float ph = 185f;

            GUI.color = new Color(0.12f, 0.02f, 0.16f, 0.94f);
            GUI.DrawTexture(new Rect(px, py, pw, ph), Texture2D.whiteTexture);
            GUI.color = Color.white;

            GUIStyle ruleSt = new GUIStyle(GUI.skin.label);
            ruleSt.fontSize = 16;
            ruleSt.wordWrap = true;
            ruleSt.normal.textColor = Color.white;

            string rules = "ПАМЯТКА СОТРУДНИКА ПВЗ:\n" +
                           "• [WASD] — Ходить по офису и складу посылок  |  [Мышь] — Осмотр\n" +
                           "• [ЛКМ (зажать)] — Сканировать QR-код / коробку и отпугивать монстров лазером\n" +
                           "• [E] — Взять / положить коробку или открыть ПК   |   [F] — Компьютер WB-OS";

            GUI.Label(new Rect(px + 18f, py + 15f, pw - 36f, ph - 30f), rules, ruleSt);
        }
    }
}
