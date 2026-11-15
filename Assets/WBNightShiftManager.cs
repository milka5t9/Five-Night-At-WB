using UnityEngine;
using UnityEngine.SceneManagement;

public class WBNightShiftManager : MonoBehaviour
{
    public static int CurrentNight = 1;

    public float baseSecondsPerHour = 35f;
    public float batteryDrainSpeed = 14f;
    public float batteryRechargeSpeed = 9f;

    private float hourTimer = 0f;
    private int currentHour = 0;
    private float battery = 100f;
    private bool isOverheated = false;
    private bool isVictory = false;

    private WBGameplayManager gameplayManager;
    private WBMonsterSystem monsterSystem;
    private Light scannerLight;

    void Awake()
    {
        CurrentNight = PlayerPrefs.GetInt("WB_Saved_Night", 1);
        if (CurrentNight == 0) CurrentNight = 1;
    }

    void Start()
    {
        gameplayManager = (WBGameplayManager)FindAnyObjectByType(typeof(WBGameplayManager));
        monsterSystem = (WBMonsterSystem)FindAnyObjectByType(typeof(WBMonsterSystem));

        if (Camera.main != null)
        {
            Transform tr = Camera.main.transform.Find("Scanner_Laser");
            if (tr != null) scannerLight = (Light)tr.GetComponent(typeof(Light));
        }
    }

    public static void StartNewGame()
    {
        CurrentNight = 1;
        PlayerPrefs.SetInt("WB_Saved_Night", 1);
        PlayerPrefs.Save();
    }

    public float GetDifficultyMultiplier()
    {
        // С каждой ночью монстры и сбои становятся быстрее
        return 1f + (CurrentNight - 1) * 0.22f;
    }

    void Update()
    {
        if (monsterSystem == null) monsterSystem = (WBMonsterSystem)FindAnyObjectByType(typeof(WBMonsterSystem));

        if (monsterSystem != null && monsterSystem.IsPlayerDead())
        {
            if (scannerLight != null) scannerLight.enabled = false;
            return;
        }

        if (isVictory)
        {
            if (scannerLight != null) scannerLight.enabled = false;
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            return;
        }

        // Время идет ТОЛЬКО когда закончились меню и кат-сцена
        WBIntroCutscene cut = (WBIntroCutscene)FindAnyObjectByType(typeof(WBIntroCutscene));
        WBMainMenu menu = (WBMainMenu)FindAnyObjectByType(typeof(WBMainMenu));
        bool inCutsceneOrMenu = (cut != null && cut.IsCutsceneActive()) || (menu != null && menu.IsMenuActive());
        if (inCutsceneOrMenu)
        {
            return;
        }

        UpdateShiftClock();
        UpdateScannerBattery();
    }

    void UpdateShiftClock()
    {
        hourTimer += Time.deltaTime;
        if (Mathf.Max(hourTimer, baseSecondsPerHour) == hourTimer)
        {
            hourTimer = 0f;
            currentHour++;

            if (currentHour == 6)
            {
                TriggerVictory();
            }
        }
    }

    void UpdateScannerBattery()
    {
        if (scannerLight == null) return;

        if (isOverheated)
        {
            scannerLight.enabled = false;
            battery += batteryRechargeSpeed * Time.deltaTime;
            if (Mathf.Max(battery, 40f) == battery) isOverheated = false;
            return;
        }

        if (scannerLight.enabled)
        {
            battery -= batteryDrainSpeed * Time.deltaTime;
            if (Mathf.Min(battery, 0f) == battery)
            {
                battery = 0f;
                isOverheated = true;
                scannerLight.enabled = false;
            }
        }
        else
        {
            battery += batteryRechargeSpeed * Time.deltaTime;
            if (Mathf.Max(battery, 100f) == battery) battery = 100f;
        }
    }

    void TriggerVictory()
    {
        isVictory = true;
        if (scannerLight != null) scannerLight.enabled = false;
        if (gameplayManager != null) gameplayManager.enabled = false;

        WBCameraMonitor camMon = (WBCameraMonitor)FindAnyObjectByType(typeof(WBCameraMonitor));
        if (camMon != null) camMon.CloseMonitor();

        if (monsterSystem != null) monsterSystem.enabled = false;

        // Сохраняем переход на следующую ночь
        int nextNight = CurrentNight + 1;
        if (nextNight == 6) nextNight = 5;
        PlayerPrefs.SetInt("WB_Saved_Night", nextNight);
        PlayerPrefs.Save();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void OnGUI()
    {
        if (monsterSystem != null && monsterSystem.IsPlayerDead()) return;

        WBIntroCutscene cut = (WBIntroCutscene)FindAnyObjectByType(typeof(WBIntroCutscene));
        WBMainMenu menu = (WBMainMenu)FindAnyObjectByType(typeof(WBMainMenu));
        if ((cut != null && cut.IsCutsceneActive()) || (menu != null && menu.IsMenuActive())) return;

        if (isVictory)
        {
            GUI.color = new Color(0.18f, 0.02f, 0.24f, 1.0f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = Color.white;

            GUIStyle winBig = new GUIStyle(GUI.skin.label);
            winBig.fontSize = 46;
            winBig.fontStyle = FontStyle.Bold;
            winBig.alignment = TextAnchor.MiddleCenter;
            winBig.normal.textColor = Color.white;

            GUIStyle winSub = new GUIStyle(GUI.skin.label);
            winSub.fontSize = 24;
            winSub.alignment = TextAnchor.MiddleCenter;
            winSub.normal.textColor = Color.green;

            GUI.Label(new Rect(0, Screen.height / 2f - 100f, Screen.width, 65f), "06:00 УТРА — НОЧЬ " + CurrentNight + " ПРОЙДЕНА!", winBig);
            GUI.Label(new Rect(0, Screen.height / 2f - 20f, Screen.width, 45f), "Вы пережили смену на ПВЗ Wildberries!", winSub);
            GUI.Label(new Rect(0, Screen.height / 2f + 65f, Screen.width, 40f), "Нажмите [Enter], чтобы перейти к следующей ночи", winBig);
            return;
        }

        GUIStyle clockStyle = new GUIStyle(GUI.skin.label);
        clockStyle.fontSize = 26;
        clockStyle.fontStyle = FontStyle.Bold;
        clockStyle.alignment = TextAnchor.UpperRight;
        clockStyle.normal.textColor = Color.white;

        GUI.Label(new Rect(Screen.width - 280f, 68f, 255f, 38f), "НОЧЬ " + CurrentNight + "  |  0" + currentHour + ":00 AM", clockStyle);

        GUIStyle batStyle = new GUIStyle(GUI.skin.label);
        batStyle.fontSize = 18;
        batStyle.fontStyle = FontStyle.Bold;
        batStyle.alignment = TextAnchor.UpperRight;
        batStyle.normal.textColor = isOverheated ? Color.red : Color.yellow;

        string batText = isOverheated ? "ПЕРЕГРЕВ СКАНЕРА!" : "Заряд сканера: " + Mathf.RoundToInt(battery) + "%";
        GUI.Label(new Rect(Screen.width - 280f, 104f, 255f, 30f), batText, batStyle);
    }
}
