using UnityEngine;
using UnityEngine.SceneManagement;

public static class WBJumpscareAnimator
{
    private static Light scareLight;

    private static void UpdateScareLight(Vector3 pos, Color col, float intensity)
    {
        if (scareLight == null)
        {
            GameObject lightObj = new GameObject("Jumpscare_Flash_Light");
            scareLight = (Light)lightObj.AddComponent(typeof(Light));
            scareLight.type = LightType.Point;
            scareLight.range = 4.0f;
        }
        scareLight.enabled = true;
        scareLight.transform.position = pos;
        scareLight.color = col;
        scareLight.intensity = intensity;
    }

    public static void HideScareLight()
    {
        if (scareLight != null) scareLight.enabled = false;
    }

    public static void PlayVentCrawlerScare(Camera cam, GameObject crawler, WBProceduralAnimator anim, float t)
    {
        PlayClassicFNaF1Scare(cam, crawler, anim, t, -0.34f, 0.32f, new Color(1f, 0.55f, 0.15f));
    }

    public static void PlayStalkerChokeScare(Camera cam, GameObject stalker, WBProceduralAnimator anim, float t)
    {
        PlayClassicFNaF1Scare(cam, stalker, anim, t, -2.02f, 0.56f, new Color(0.9f, 0.92f, 1f));
    }

    static void PlayClassicFNaF1Scare(Camera cam, GameObject monster, WBProceduralAnimator anim, float t, float heightOffset, float distToFace, Color flashColor)
    {
        if (cam == null || monster == null) return;

        cam.fieldOfView = 42f;
        cam.nearClipPlane = 0.05f;

        float shakeX = Mathf.Sin(t * 95f) * 0.055f;
        float shakeY = Mathf.Cos(t * 110f) * 0.045f;
        float shakeZ = Mathf.Sin(t * 75f) * 0.035f;

        Vector3 basePos = cam.transform.position + cam.transform.forward * (distToFace + shakeZ);
        basePos += cam.transform.right * shakeX + cam.transform.up * (heightOffset + shakeY);
        monster.transform.position = basePos;

        float tiltRoll = Mathf.Sin(t * 85f) * 18f;
        float tiltPitch = Mathf.Cos(t * 90f) * 12f;
        monster.transform.rotation = Quaternion.LookRotation(-cam.transform.forward) * Quaternion.Euler(tiltPitch, 0f, tiltRoll);

        if (anim != null)
        {
            anim.isWalking = false;
            float armTwitch = Mathf.Sin(t * 100f) * 35f;
            if (anim.armL != null) anim.armL.localRotation = Quaternion.Euler(-85f + armTwitch, 25f, 15f);
            if (anim.armR != null) anim.armR.localRotation = Quaternion.Euler(-85f - armTwitch, -25f, -15f);
        }

        Vector3 camPos = cam.transform.position;
        cam.transform.position = new Vector3(camPos.x + Mathf.Cos(t * 120f) * 0.015f, 1.65f + Mathf.Sin(t * 115f) * 0.015f, camPos.z);

        float strobe = (Mathf.FloorToInt(t * 30f) % 2 == 0) ? 6.5f : 1.2f;
        UpdateScareLight(cam.transform.position + cam.transform.forward * 0.14f, flashColor, strobe);
    }
}

public class WBMonsterSystem : MonoBehaviour
{
    public float moveInterval = 8.0f;
    public float attackGraceTime = 6.5f;

    private Camera mainCam;
    private Light scannerLight;
    private GameObject stalkerObj;
    private WBProceduralAnimator stalkerAnim;
    private GameObject ventMonsterObj;
    private WBProceduralAnimator ventAnim;
    private GameObject cam1Obj, cam2Obj, cam3Obj, cam4Obj;

    private int stalkerStage = 0;
    private float stalkerTimer = 0f;
    private float stalkerAttackTimer = 0f;
    private Vector3 stalkerTargetPos;

    private int ventStage = 0;
    private float ventTimer = 3.0f;
    private float ventAttackTimer = 0f;
    private Vector3 ventTargetPos;
    private Quaternion ventTargetRot;

    private bool isJumpscaring = false;
    private bool isVentJumpscare = false;
    private float jumpscareTimer = 0f;
    private bool isGameOver = false;
    private string jumpScareReason = "";

    private Vector3 stalkerPos0 = new Vector3(0.5f, 0f, 13.5f);
    private Vector3 stalkerPos1 = new Vector3(-0.6f, 0f, 6.5f);
    private Vector3 stalkerPos2 = new Vector3(0f, 0f, 2.75f);

    private Vector3 ventPos0 = new Vector3(1.8f, 0f, 5.8f);
    private Vector3 ventPos1 = new Vector3(5.45f, 0.88f, 4.2f);
    private Vector3 ventPos2 = new Vector3(5.45f, 0.88f, -0.8f);
    private Vector3 ventPos3 = new Vector3(2.96f, 0.88f, -0.8f);

    public bool IsPlayerDead()
    {
        return isGameOver || isJumpscaring;
    }

    void Start()
    {
        mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.fieldOfView = 60f;
            Transform tr = mainCam.transform.Find("Scanner_Laser");
            if (tr != null) scannerLight = (Light)tr.GetComponent(typeof(Light));
        }

        cam1Obj = GameObject.Find("Cam_01_MainStorage");
        cam2Obj = GameObject.Find("Cam_02_LoadingZone");
        cam3Obj = GameObject.Find("Cam_03_ClientZone");
        cam4Obj = GameObject.Find("Cam_04_Ventilation");

        WBJumpscareAnimator.HideScareLight();
        MakeCorridorPitchBlack();

        GameObject oldS = GameObject.Find("WB_Stalker_Monster");
        if (oldS != null) Destroy(oldS);
        stalkerObj = WBCharacterModels.CreateScaryMonster("WB_Stalker_Monster", false);
        stalkerAnim = (WBProceduralAnimator)stalkerObj.GetComponent(typeof(WBProceduralAnimator));
        stalkerTargetPos = stalkerPos0;
        stalkerObj.transform.position = stalkerPos0;

        GameObject oldV = GameObject.Find("WB_Vent_Monster");
        if (oldV != null) Destroy(oldV);
        ventMonsterObj = WBCharacterModels.CreateScaryMonster("WB_Vent_Monster", true);
        ventAnim = (WBProceduralAnimator)ventMonsterObj.GetComponent(typeof(WBProceduralAnimator));
        ventTargetPos = ventPos0;
        ventTargetRot = Quaternion.Euler(0f, -90f, 0f);
        ventMonsterObj.transform.position = ventPos0;
        ventMonsterObj.transform.rotation = ventTargetRot;
    }

    void MakeCorridorPitchBlack()
    {
        RenderSettings.skybox = null;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = Color.black;
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogColor = Color.black;
        RenderSettings.fogDensity = 0.16f;

        Light[] lights = (Light[])FindObjectsByType(typeof(Light), FindObjectsSortMode.None);
        foreach (Light l in lights)
        {
            if (l.type == LightType.Directional) l.enabled = false;
            if (l.gameObject.name == "Reception_Work_Lamp")
            {
                l.transform.position = new Vector3(0f, 2.6f, -1.1f);
                l.range = 3.5f;
                l.intensity = 1.6f;
            }
        }
    }

    void Update()
    {
        PanSecurityCameras();

        if (isGameOver)
        {
            if (scannerLight != null) scannerLight.enabled = false;
            if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        if (isJumpscaring)
        {
            if (scannerLight != null) scannerLight.enabled = false;
            jumpscareTimer += Time.deltaTime;
            if (isVentJumpscare) WBJumpscareAnimator.PlayVentCrawlerScare(mainCam, ventMonsterObj, ventAnim, jumpscareTimer);
            else WBJumpscareAnimator.PlayStalkerChokeScare(mainCam, stalkerObj, stalkerAnim, jumpscareTimer);

            if (Mathf.Max(jumpscareTimer, 1.25f) == jumpscareTimer)
            {
                isJumpscaring = false;
                isGameOver = true;
                WBJumpscareAnimator.HideScareLight();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            return;
        }

        WBIntroCutscene cut = (WBIntroCutscene)FindAnyObjectByType(typeof(WBIntroCutscene));
        WBMainMenu menu = (WBMainMenu)FindAnyObjectByType(typeof(WBMainMenu));
        if ((cut != null && cut.IsCutsceneActive()) || (menu != null && menu.IsMenuActive())) return;


        AnimateMonsterMovement();
        HandleStalkerAI();
        HandleVentAI();
    }

    void PanSecurityCameras()
    {
        float pan = Mathf.Sin(Time.time * 0.75f) * 22f;
        if (cam1Obj != null) cam1Obj.transform.rotation = Quaternion.Euler(25f, 35f + pan, 0f);
        if (cam2Obj != null) cam2Obj.transform.rotation = Quaternion.Euler(25f, -140f + pan, 0f);
        if (cam3Obj != null) cam3Obj.transform.rotation = Quaternion.Euler(25f, 135f + pan, 0f);
        if (cam4Obj != null) cam4Obj.transform.rotation = Quaternion.Euler(18f, -25f + pan * 0.7f, 0f);
    }

    void AnimateMonsterMovement()
    {
        if (stalkerObj != null)
        {
            float distS = Vector3.Distance(stalkerObj.transform.position, stalkerTargetPos);
            if (stalkerAnim != null) stalkerAnim.isWalking = !Mathf.Approximately(distS, 0f);
            stalkerObj.transform.position = Vector3.MoveTowards(stalkerObj.transform.position, stalkerTargetPos, 2.5f * Time.deltaTime);
            stalkerObj.transform.rotation = Quaternion.identity;
        }
        if (ventMonsterObj != null)
        {
            float distV = Vector3.Distance(ventMonsterObj.transform.position, ventTargetPos);
            if (ventAnim != null) ventAnim.isWalking = !Mathf.Approximately(distV, 0f);
            ventMonsterObj.transform.position = Vector3.MoveTowards(ventMonsterObj.transform.position, ventTargetPos, 2.4f * Time.deltaTime);
            ventMonsterObj.transform.rotation = Quaternion.Slerp(ventMonsterObj.transform.rotation, ventTargetRot, Time.deltaTime * 8f);
        }
    }

    void HandleStalkerAI()
    {
        if (stalkerStage != 2)
        {
            stalkerTimer += Time.deltaTime;
            if (Mathf.Max(stalkerTimer, moveInterval) == stalkerTimer)
            {
                stalkerTimer = 0f;
                if (Random.Range(0, 10) != 0)
                {
                    stalkerStage++;
                    stalkerTargetPos = (stalkerStage == 1) ? stalkerPos1 : stalkerPos2;
                }
            }
        }
        else if (IsShiningLightAt(stalkerPos2 + new Vector3(0f, 1.5f, 0f)))
        {
            stalkerAttackTimer -= Time.deltaTime * 2.8f;
            if (Mathf.Min(stalkerAttackTimer, -0.7f) == stalkerAttackTimer)
            {
                stalkerStage = 0;
                stalkerTimer = 0f;
                stalkerAttackTimer = 0f;
                stalkerTargetPos = stalkerPos0;
            }
        }
        else
        {
            stalkerAttackTimer += Time.deltaTime;
            if (Mathf.Max(stalkerAttackTimer, attackGraceTime) == stalkerAttackTimer)
            {
                BeginScare(false, "Ночной Складской напал из темноты! Проверяйте проход лазером.");
            }
        }
    }

    void HandleVentAI()
    {
        if (ventStage != 3)
        {
            ventTimer += Time.deltaTime;
            if (Mathf.Max(ventTimer, moveInterval * 0.85f) == ventTimer)
            {
                ventTimer = 0f;
                if (Random.Range(0, 10) != 0)
                {
                    ventStage++;
                    if (ventStage == 1) { ventMonsterObj.transform.position = new Vector3(5.45f, 0.88f, 5.5f); ventTargetPos = ventPos1; ventTargetRot = Quaternion.identity; }
                    else if (ventStage == 2) { ventTargetPos = ventPos2; ventTargetRot = Quaternion.Euler(0f, 90f, 0f); }
                    else if (ventStage == 3) { ventTargetPos = ventPos3; ventTargetRot = Quaternion.Euler(0f, 90f, 0f); }
                }
            }
        }
        else if (IsShiningLightAt(new Vector3(3.0f, 1.35f, -0.8f)))
        {
            ventAttackTimer -= Time.deltaTime * 2.8f;
            if (Mathf.Min(ventAttackTimer, -0.7f) == ventAttackTimer)
            {
                ventStage = 0;
                ventTimer = 0f;
                ventAttackTimer = 0f;
                ventTargetPos = ventPos0;
                ventTargetRot = Quaternion.Euler(0f, -90f, 0f);
                ventMonsterObj.transform.position = ventPos0;
            }
        }
        else
        {
            ventAttackTimer += Time.deltaTime;
            if (Mathf.Max(ventAttackTimer, attackGraceTime) == ventAttackTimer)
            {
                BeginScare(true, "Ползун выпрыгнул из вентиляции! Светите сканером в решетку справа.");
            }
        }
    }

    bool IsShiningLightAt(Vector3 targetPoint)
    {
        if (scannerLight == null || !scannerLight.enabled || mainCam == null) return false;
        Vector3 dir = (targetPoint - mainCam.transform.position).normalized;
        float angle = Vector3.Angle(mainCam.transform.forward, dir);
        float dist = Vector3.Distance(mainCam.transform.position, targetPoint);
        return (Mathf.Min(angle, 28f) == angle && Mathf.Min(dist, 9.0f) == dist);
    }

    void BeginScare(bool fromVent, string reason)
    {
        isJumpscaring = true;
        isVentJumpscare = fromVent;
        jumpscareTimer = 0f;
        jumpScareReason = reason;

        if (scannerLight != null) scannerLight.enabled = false;
        if (WBSoundManager.Instance != null) WBSoundManager.Instance.PlayJumpscare();

        WBGameplayManager gm = (WBGameplayManager)FindAnyObjectByType(typeof(WBGameplayManager));
        if (gm != null) gm.enabled = false;
        WBCameraMonitor cm = (WBCameraMonitor)FindAnyObjectByType(typeof(WBCameraMonitor));
        if (cm != null) cm.CloseMonitor();
    }

    void OnGUI()
    {
        if (isJumpscaring || !isGameOver) return;

        GUI.color = new Color(0.08f, 0.08f, 0.08f, 0.96f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);

        for (int s = 0; s != 45; s++)
        {
            float lineY = Random.Range(0f, Screen.height);
            float lineH = Random.Range(2f, 14f);
            float shade = Random.Range(0.2f, 0.85f);
            GUI.color = new Color(shade, shade, shade, 0.28f);
            GUI.DrawTexture(new Rect(0, lineY, Screen.width, lineH), Texture2D.whiteTexture);
        }
        GUI.color = Color.white;

        GUIStyle big = new GUIStyle(GUI.skin.label);
        big.fontSize = 36;
        big.fontStyle = FontStyle.Bold;
        big.alignment = TextAnchor.MiddleCenter;
        big.normal.textColor = Color.white;

        GUI.Label(new Rect(0, Screen.height / 2f - 85f, Screen.width, 60f), "GAME OVER — СМЕНА ОКОНЧЕНА", big);
        GUI.Label(new Rect(40, Screen.height / 2f - 15f, Screen.width - 80f, 65f), jumpScareReason, big);
        GUI.Label(new Rect(0, Screen.height / 2f + 70f, Screen.width, 40f), "Нажмите [R], чтобы начать заново", big);
    }
}
