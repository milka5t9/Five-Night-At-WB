using UnityEngine;

public static class WBShadowStalkerBuilder
{
    public static GameObject Build(string name)
    {
        GameObject root = new GameObject(name);
        GameObject rig = WBProceduralAnimator.MakePivot("Rig", Vector3.zero, root.transform);
        WBProceduralAnimator anim = (WBProceduralAnimator)root.AddComponent(typeof(WBProceduralAnimator));
        anim.isMonster = true;
        anim.isVentCrawler = false;
        anim.bodyRoot = rig.transform;

        Material shadowSkin = WBOrganicMesh.CreateOrganicMat(new Color(0.025f, 0.025f, 0.03f), new Color(0.005f, 0.005f, 0.008f), 0.35f);
        Material clawMat = WBOrganicMesh.CreateOrganicMat(new Color(0.06f, 0.05f, 0.06f), Color.black, 0.6f);
        Material eyeGlow = WBProceduralAnimator.MakeMat(new Color(0.95f, 0.15f, 0.1f), true, 2.5f);

        // Костистый худой торс и ребра
        WBOrganicMesh.CreateLimb("Gaunt_Torso", new Vector3(0f, 1.82f, 0f), 0.76f, 0.19f, 0.10f, 0.06f, shadowSkin, rig.transform);

        // Длинная жилистая шея и вытянутый анатомический череп (центр головы ровно на Y = 2.02)
        WBOrganicMesh.CreateLimb("Sinew_Neck", new Vector3(0f, 1.96f, -0.02f), 0.18f, 0.055f, 0.075f, -0.03f, shadowSkin, rig.transform);
        GameObject head = WBOrganicMesh.CreateGauntSkull("Gaunt_Head", new Vector3(0f, 2.02f, -0.05f), new Vector3(0.21f, 0.28f, 0.24f), shadowSkin, rig.transform);

        // Узкие хищные глаза в глубоких глазницах
        WBProceduralAnimator.MakePart(PrimitiveType.Sphere, "Eye_L", new Vector3(-0.22f, 0.08f, -0.38f), new Vector3(0.11f, 0.04f, 0.05f), eyeGlow, head.transform);
        WBProceduralAnimator.MakePart(PrimitiveType.Sphere, "Eye_R", new Vector3(0.22f, 0.08f, -0.38f), new Vector3(0.11f, 0.04f, 0.05f), eyeGlow, head.transform);

        // Длинные жилистые ноги с изгибом колена
        GameObject legLP = WBProceduralAnimator.MakePivot("SLegL", new Vector3(-0.11f, 1.08f, 0f), rig.transform);
        WBOrganicMesh.CreateLimb("LegMesh_L", Vector3.zero, 1.08f, 0.065f, 0.038f, -0.06f, shadowSkin, legLP.transform);
        anim.legL = legLP.transform;

        GameObject legRP = WBProceduralAnimator.MakePivot("SLegR", new Vector3(0.11f, 1.08f, 0f), rig.transform);
        WBOrganicMesh.CreateLimb("LegMesh_R", Vector3.zero, 1.08f, 0.065f, 0.038f, -0.06f, shadowSkin, legRP.transform);
        anim.legR = legRP.transform;

        // Длинные когтистые руки с веером изогнутых когтей-игл
        BuildOrganicArm(rig.transform, anim, true, new Vector3(-0.21f, 1.76f, 0f), shadowSkin, clawMat);
        BuildOrganicArm(rig.transform, anim, false, new Vector3(0.21f, 1.76f, 0f), shadowSkin, clawMat);

        return root;
    }

    static void BuildOrganicArm(Transform parent, WBProceduralAnimator anim, bool isLeft, Vector3 pos, Material skin, Material claw)
    {
        float side = isLeft ? -1f : 1f;
        GameObject armP = WBProceduralAnimator.MakePivot(isLeft ? "SArmL" : "SArmR", pos, parent);
        armP.transform.localRotation = Quaternion.Euler(0f, 0f, -side * 7f);

        WBOrganicMesh.CreateLimb("ArmLimb", Vector3.zero, 0.96f, 0.048f, 0.028f, -0.07f, skin, armP.transform);

        GameObject hand = WBProceduralAnimator.MakePivot("Hand", new Vector3(0f, -0.95f, -0.03f), armP.transform);
        for (int i = 0; i != 5; i++)
        {
            float spread = (i - 2) * 14f;
            float len = (i == 2) ? 0.34f : 0.27f;
            GameObject talon = WBOrganicMesh.CreateCurvedClaw("Talon_" + i, new Vector3(0f, 0f, (i - 2) * 0.015f), len, 0.012f, claw, hand.transform);
            talon.transform.localRotation = Quaternion.Euler(spread * 0.6f, 0f, -side * (10f + i * 5f));
        }

        if (isLeft) anim.armL = armP.transform;
        else anim.armR = armP.transform;
    }
}
