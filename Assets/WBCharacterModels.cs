using UnityEngine;

public class WBProceduralAnimator : MonoBehaviour
{
    public bool isMonster = false;
    public bool isVentCrawler = false;
    public bool isWalking = false;
    public bool isShowingPhone = false;

    public Transform legL;
    public Transform legR;
    public Transform armL;
    public Transform armR;
    public Transform bodyRoot;

    private float walkCycle = 0f;
    private float phoneLift = 0f;

    void Update()
    {
        if (isWalking)
        {
            float speed = isVentCrawler ? 11.0f : (isMonster ? 5.5f : 7.5f);
            walkCycle += Time.deltaTime * speed;
            float swing = Mathf.Sin(walkCycle) * (isVentCrawler ? 34f : (isMonster ? 22f : 28f));

            if (legL != null) legL.localRotation = Quaternion.Euler(swing, 0f, 0f);
            if (legR != null) legR.localRotation = Quaternion.Euler(-swing, 0f, 0f);

            if (isVentCrawler)
            {
                if (armL != null) armL.localRotation = Quaternion.Euler(-35f + swing, 15f, 35f);
                if (armR != null) armR.localRotation = Quaternion.Euler(-35f - swing, -15f, -35f);
            }
            else
            {
                if (armL != null) armL.localRotation = Quaternion.Euler(-swing * 0.7f, 0f, 0f);
                if (!isShowingPhone && armR != null)
                {
                    armR.localRotation = Quaternion.Euler(swing * 0.7f, 0f, 0f);
                }
            }

            if (bodyRoot != null)
            {
                float bob = Mathf.Abs(Mathf.Sin(walkCycle)) * 0.04f;
                bodyRoot.localPosition = new Vector3(0f, bob, 0f);
            }
        }
        else
        {
            if (legL != null) legL.localRotation = Quaternion.Lerp(legL.localRotation, Quaternion.identity, Time.deltaTime * 8f);
            if (legR != null) legR.localRotation = Quaternion.Lerp(legR.localRotation, Quaternion.identity, Time.deltaTime * 8f);
            if (armL != null && !isVentCrawler) armL.localRotation = Quaternion.Lerp(armL.localRotation, Quaternion.identity, Time.deltaTime * 8f);
            if (bodyRoot != null) bodyRoot.localPosition = Vector3.Lerp(bodyRoot.localPosition, Vector3.zero, Time.deltaTime * 8f);
        }

        if (!isMonster && armR != null)
        {
            float targetLift = isShowingPhone ? 1f : 0f;
            phoneLift = Mathf.Lerp(phoneLift, targetLift, Time.deltaTime * 5f);
            armR.localRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(-78f, -12f, 0f), phoneLift);
        }
    }

    public static Material MakeMat(Color col, bool emissive = false, float emissionPower = 1f)
    {
        Shader sh = Shader.Find("Universal Render Pipeline/Lit");
        if (sh == null) sh = Shader.Find("Standard");
        Material m = new Material(sh);
        m.color = col;
        if (emissive)
        {
            m.EnableKeyword("_EMISSION");
            m.SetColor("_EmissionColor", col * emissionPower);
        }
        return m;
    }

    public static GameObject MakePivot(string name, Vector3 pos, Transform parent)
    {
        GameObject p = new GameObject(name);
        p.transform.SetParent(parent);
        p.transform.localPosition = pos;
        return p;
    }

    public static GameObject MakePart(PrimitiveType type, string name, Vector3 pos, Vector3 scale, Material mat, Transform parent)
    {
        GameObject obj = GameObject.CreatePrimitive(type);
        obj.name = name;
        obj.transform.SetParent(parent);
        obj.transform.localPosition = pos;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = scale;

        Renderer r = (Renderer)obj.GetComponent(typeof(Renderer));
        if (r != null) r.material = mat;
        return obj;
    }
}

public static class WBCharacterModels
{
    public static GameObject CreateCustomerWithPhone(Vector3 pos, Texture2D qrTexture)
    {
        GameObject root = new GameObject("WB_Customer_Detailed");
        root.transform.position = pos;
        root.transform.localScale = new Vector3(0.84f, 0.84f, 0.84f);

        GameObject rig = WBProceduralAnimator.MakePivot("Rig", Vector3.zero, root.transform);
        WBProceduralAnimator anim = (WBProceduralAnimator)root.AddComponent(typeof(WBProceduralAnimator));
        anim.isMonster = false;
        anim.bodyRoot = rig.transform;

        Material skin = WBProceduralAnimator.MakeMat(new Color(0.92f, 0.76f, 0.65f));
        Material jacket = WBProceduralAnimator.MakeMat(new Color(0.20f, 0.27f, 0.42f));
        Material pants = WBProceduralAnimator.MakeMat(new Color(0.13f, 0.13f, 0.17f));
        Material hair = WBProceduralAnimator.MakeMat(new Color(0.18f, 0.12f, 0.08f));
        Material phoneBody = WBProceduralAnimator.MakeMat(new Color(0.06f, 0.06f, 0.08f));

        GameObject legLP = WBProceduralAnimator.MakePivot("LegL_Pivot", new Vector3(-0.11f, 0.82f, 0f), rig.transform);
        WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Leg_L", new Vector3(0f, -0.41f, 0f), new Vector3(0.14f, 0.82f, 0.15f), pants, legLP.transform);
        anim.legL = legLP.transform;

        GameObject legRP = WBProceduralAnimator.MakePivot("LegR_Pivot", new Vector3(0.11f, 0.82f, 0f), rig.transform);
        WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Leg_R", new Vector3(0f, -0.41f, 0f), new Vector3(0.14f, 0.82f, 0.15f), pants, legRP.transform);
        anim.legR = legRP.transform;

        WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Torso", new Vector3(0f, 1.12f, 0f), new Vector3(0.40f, 0.58f, 0.22f), jacket, rig.transform);
        WBProceduralAnimator.MakePart(PrimitiveType.Sphere, "Head", new Vector3(0f, 1.56f, 0f), new Vector3(0.24f, 0.28f, 0.25f), skin, rig.transform);
        WBProceduralAnimator.MakePart(PrimitiveType.Sphere, "Hair", new Vector3(0f, 1.63f, 0.02f), new Vector3(0.25f, 0.20f, 0.26f), hair, rig.transform);

        GameObject armLP = WBProceduralAnimator.MakePivot("ArmL_Pivot", new Vector3(-0.26f, 1.36f, 0f), rig.transform);
        WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Arm_L", new Vector3(0f, -0.26f, 0f), new Vector3(0.10f, 0.52f, 0.10f), jacket, armLP.transform);
        anim.armL = armLP.transform;

        GameObject armRP = WBProceduralAnimator.MakePivot("ArmR_Pivot", new Vector3(0.26f, 1.36f, 0f), rig.transform);
        WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Arm_R", new Vector3(0f, -0.24f, 0f), new Vector3(0.10f, 0.48f, 0.10f), jacket, armRP.transform);
        anim.armR = armRP.transform;

        GameObject phone = WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Customer_Phone", new Vector3(0f, -0.52f, 0.04f), new Vector3(0.14f, 0.24f, 0.02f), phoneBody, armRP.transform);
        GameObject screen = WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Phone_QR_Screen", new Vector3(0f, 0f, 0.56f), new Vector3(0.88f, 0.90f, 0.12f), WBProceduralAnimator.MakeMat(Color.white, true, 1.1f), phone.transform);
        Renderer r = (Renderer)screen.GetComponent(typeof(Renderer));
        if (r != null && qrTexture != null) r.material.mainTexture = qrTexture;

        return root;
    }

    public static GameObject CreateScaryMonster(string name, bool isVentCrawler)
    {
        if (isVentCrawler)
        {
            return WBVentCrawlerBuilder.Build(name);
        }
        return WBShadowStalkerBuilder.Build(name);
    }

    public static void BuildScannerInHand(Transform cameraTransform)
    {
        Transform old = cameraTransform.Find("WB_Scanner_Prop");
        if (old != null) Object.Destroy(old.gameObject);

        GameObject gun = WBProceduralAnimator.MakePivot("WB_Scanner_Prop", new Vector3(0.28f, -0.22f, 0.42f), cameraTransform);
        gun.transform.localRotation = Quaternion.Euler(4f, -6f, 0f);

        Material black = WBProceduralAnimator.MakeMat(new Color(0.1f, 0.1f, 0.12f));
        Material purple = WBProceduralAnimator.MakeMat(new Color(0.62f, 0.1f, 0.65f));
        Material laser = WBProceduralAnimator.MakeMat(Color.red, true, 3.0f);

        GameObject handle = WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Handle", new Vector3(0f, -0.06f, -0.02f), new Vector3(0.045f, 0.15f, 0.055f), black, gun.transform);
        handle.transform.localRotation = Quaternion.Euler(16f, 0f, 0f);
        WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Body", new Vector3(0f, 0.025f, 0.03f), new Vector3(0.06f, 0.065f, 0.16f), purple, gun.transform);
        WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Glass", new Vector3(0f, 0.035f, 0.122f), new Vector3(0.055f, 0.05f, 0.01f), laser, gun.transform);
    }
}
