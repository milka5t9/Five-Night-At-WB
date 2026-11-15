using UnityEngine;

public class WBPowerGenerator : MonoBehaviour
{
    public float minTimeBetweenOutages = 32f;
    public float maxTimeBetweenOutages = 48f;
    public float rebootDuration = 3.0f;

    private bool isPowerOn = true;
    private bool isRebooting = false;
    private float nextOutageTimer = 35f;
    private float rebootTimer = 0f;

    private Light deskLamp;
    private Light backRoomLamp;

    void Start()
    {
        nextOutageTimer = Random.Range(minTimeBetweenOutages, maxTimeBetweenOutages);
    }

    void FindLamps()
    {
        if (deskLamp == null)
        {
            GameObject d = GameObject.Find("Reception_Work_Lamp");
            if (d != null) deskLamp = (Light)d.GetComponent(typeof(Light));
        }
        if (backRoomLamp == null)
        {
            GameObject b = GameObject.Find("BackRoom_Light");
            if (b != null) backRoomLamp = (Light)b.GetComponent(typeof(Light));
        }
    }

    void Update()
    {
        FindLamps();

        // Не ломаем свет во время начальной кат-сцены (когда управление выключено, а курсор скрыт)
        WBGameplayManager gm = (WBGameplayManager)FindAnyObjectByType(typeof(WBGameplayManager));
        if (gm != null && !gm.enabled && !Cursor.visible) return;

        if (isPowerOn)
        {
            nextOutageTimer -= Time.deltaTime;
            if (Mathf.Min(nextOutageTimer, 0f) == nextOutageTimer)
            {
                BreakGenerator();
            }
        }
        else if (isRebooting)
        {
            rebootTimer += Time.deltaTime;
            if (Mathf.Max(rebootTimer, rebootDuration) == rebootTimer)
            {
                RestoreGenerator();
            }
        }
    }

    public void BreakGenerator()
    {
        isPowerOn = false;
        isRebooting = false;
        rebootTimer = 0f;

        if (deskLamp != null) deskLamp.enabled = false;
        if (backRoomLamp != null) backRoomLamp.enabled = false;
    }

    public void StartReboot()
    {
        if (isPowerOn || isRebooting) return;
        isRebooting = true;
        rebootTimer = 0f;
    }

    void RestoreGenerator()
    {
        isPowerOn = true;
        isRebooting = false;
        rebootTimer = 0f;
        nextOutageTimer = Random.Range(minTimeBetweenOutages, maxTimeBetweenOutages);

        if (deskLamp != null) deskLamp.enabled = true;
        if (backRoomLamp != null) backRoomLamp.enabled = true;
    }

    public bool IsPowerWorking() { return isPowerOn; }
    public bool IsRebootingNow() { return isRebooting; }
    public float GetRebootProgress() { return Mathf.Clamp01(rebootTimer / rebootDuration); }

    void OnGUI()
    {
        if (isPowerOn) return;

        // Мигающее предупреждение в левом верхнем углу, если вырубился свет
        if (Mathf.FloorToInt(Time.time * 3f) % 2 == 0)
        {
            GUIStyle warn = new GUIStyle(GUI.skin.label);
            warn.fontSize = 19;
            warn.fontStyle = FontStyle.Bold;
            warn.normal.textColor = Color.red;
            GUI.Label(new Rect(25, 72, 520, 35), "⚠ СБОЙ ПИТАНИЯ! ПОЧИНИТЕ ГЕНЕРАТОР В ПК (ВКЛАДКА ЭНЕРГИЯ)", warn);
        }
    }
}
