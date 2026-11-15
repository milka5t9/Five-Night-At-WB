using UnityEngine;

public static class WBVentCrawlerBuilder
{
    public static GameObject Build(string name)
    {
        GameObject root = new GameObject(name);
        GameObject rig = WBProceduralAnimator.MakePivot("Rig", Vector3.zero, root.transform);
        WBProceduralAnimator anim = (WBProceduralAnimator)root.AddComponent(typeof(WBProceduralAnimator));
        anim.isMonster = true;
        anim.isVentCrawler = true;
        anim.bodyRoot = rig.transform;

        Material skinBlueGrey = WBProceduralAnimator.MakeMat(new Color(0.16f, 0.21f, 0.24f));
        Material darkClaw = WBProceduralAnimator.MakeMat(new Color(0.05f, 0.06f, 0.07f));
        Material fireEye = WBProceduralAnimator.MakeMat(new Color(1f, 0.55f, 0.05f), true, 5.5f);
        Material toothMat = WBProceduralAnimator.MakeMat(new Color(0.75f, 0.72f, 0.65f));

        GameObject torso = WBProceduralAnimator.MakePart(PrimitiveType.Capsule, "Crawler_Torso", new Vector3(0f, 0.28f, 0.05f), new Vector3(0.32f, 0.30f, 0.24f), skinBlueGrey, rig.transform);
        torso.transform.localRotation = Quaternion.Euler(72f, 0f, 0f);

        // Отростки-щупальца на спине как на фото
        GameObject t1 = WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Tendril_1", new Vector3(-0.14f, 0.42f, 0.10f), new Vector3(0.02f, 0.28f, 0.02f), darkClaw, rig.transform);
        t1.transform.localRotation = Quaternion.Euler(-20f, 0f, 25f);
        GameObject t2 = WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Tendril_2", new Vector3(0.14f, 0.44f, 0.08f), new Vector3(0.02f, 0.30f, 0.02f), darkClaw, rig.transform);
        t2.transform.localRotation = Quaternion.Euler(-15f, 0f, -28f);

        // Череп, огненные глаза и зубастая пасть
        GameObject head = WBProceduralAnimator.MakePart(PrimitiveType.Sphere, "Crawler_Skull", new Vector3(0f, 0.38f, -0.24f), new Vector3(0.24f, 0.28f, 0.25f), skinBlueGrey, rig.transform);
        WBProceduralAnimator.MakePart(PrimitiveType.Sphere, "FireEye_L", new Vector3(-0.22f, 0.10f, -0.43f), new Vector3(0.18f, 0.12f, 0.10f), fireEye, head.transform);
        WBProceduralAnimator.MakePart(PrimitiveType.Sphere, "FireEye_R", new Vector3(0.22f, 0.10f, -0.43f), new Vector3(0.18f, 0.12f, 0.10f), fireEye, head.transform);

        WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Open_Mouth", new Vector3(0f, -0.26f, -0.38f), new Vector3(0.42f, 0.48f, 0.22f), darkClaw, head.transform);
        WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Teeth_Top", new Vector3(0f, -0.08f, -0.48f), new Vector3(0.36f, 0.10f, 0.06f), toothMat, head.transform);
        WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Teeth_Bot", new Vector3(0f, -0.42f, -0.48f), new Vector3(0.34f, 0.10f, 0.06f), toothMat, head.transform);

        // Широкие когтистые лапы
        GameObject armLP = WBProceduralAnimator.MakePivot("CArmL", new Vector3(-0.18f, 0.34f, -0.12f), rig.transform);
        armLP.transform.localRotation = Quaternion.Euler(-25f, 15f, 35f);
        WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Arm_L", new Vector3(0f, -0.22f, 0f), new Vector3(0.07f, 0.46f, 0.07f), skinBlueGrey, armLP.transform);
        BuildClaws(armLP.transform, new Vector3(0f, -0.46f, 0f), darkClaw);
        anim.armL = armLP.transform;

        GameObject armRP = WBProceduralAnimator.MakePivot("CArmR", new Vector3(0.18f, 0.34f, -0.12f), rig.transform);
        armRP.transform.localRotation = Quaternion.Euler(-25f, -15f, -35f);
        WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Arm_R", new Vector3(0f, -0.22f, 0f), new Vector3(0.07f, 0.46f, 0.07f), skinBlueGrey, armRP.transform);
        BuildClaws(armRP.transform, new Vector3(0f, -0.46f, 0f), darkClaw);
        anim.armR = armRP.transform;

        GameObject legLP = WBProceduralAnimator.MakePivot("CLegL", new Vector3(-0.14f, 0.22f, 0.24f), rig.transform);
        WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Leg_L", new Vector3(0f, -0.16f, 0f), new Vector3(0.07f, 0.32f, 0.07f), skinBlueGrey, legLP.transform);
        anim.legL = legLP.transform;

        GameObject legRP = WBProceduralAnimator.MakePivot("CLegR", new Vector3(0.14f, 0.22f, 0.24f), rig.transform);
        WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Leg_R", new Vector3(0f, -0.16f, 0f), new Vector3(0.07f, 0.32f, 0.07f), skinBlueGrey, legRP.transform);
        anim.legR = legRP.transform;

        return root;
    }

    static void BuildClaws(Transform parent, Vector3 pos, Material mat)
    {
        GameObject palm = WBProceduralAnimator.MakePivot("Palm", pos, parent);
        for (int i = -1; i != 3; i++)
        {
            GameObject f = WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Claw_" + i, new Vector3(i * 0.025f, -0.06f, -0.02f), new Vector3(0.016f, 0.14f, 0.016f), mat, palm.transform);
            f.transform.localRotation = Quaternion.Euler(-35f, 0f, i * 14f);
        }
    }
}
