using UnityEngine;

public static class WBCutsceneActors
{
    public static GameObject BuildHomeAndCarSet(out Transform phoneTransform, out Transform carRoot, out Transform roadLinesGroup)
    {
        GameObject old = GameObject.Find("Intro_Story_Set");
        if (old != null) Object.Destroy(old);

        GameObject root = new GameObject("Intro_Story_Set");

        Material wallMat = WBProceduralAnimator.MakeMat(new Color(0.16f, 0.18f, 0.22f));
        Material deskMat = WBProceduralAnimator.MakeMat(new Color(0.28f, 0.18f, 0.11f));
        Material phoneBody = WBProceduralAnimator.MakeMat(new Color(0.05f, 0.05f, 0.07f));
        Material callScreen = WBProceduralAnimator.MakeMat(new Color(0.2f, 0.85f, 0.35f), true, 1.8f);
        Material asphalt = WBProceduralAnimator.MakeMat(new Color(0.06f, 0.06f, 0.07f));
        Material carBody = WBProceduralAnimator.MakeMat(new Color(0.14f, 0.16f, 0.22f));
        Material dashMat = WBProceduralAnimator.MakeMat(new Color(0.08f, 0.08f, 0.1f));
        Material stripeMat = WBProceduralAnimator.MakeMat(Color.white, true, 1.2f);

        // === 1. ДОМАШНЯЯ КОМНАТА (в координатах Y = 50, чтобы не мешать ПВЗ) ===
        Vector3 homeBase = new Vector3(0f, 50f, 0f);
        WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Home_Floor", homeBase, new Vector3(6f, 0.2f, 6f), wallMat, root.transform);
        WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Home_WallBack", homeBase + new Vector3(0f, 2f, 2.5f), new Vector3(6f, 4f, 0.2f), wallMat, root.transform);
        WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Home_Desk", homeBase + new Vector3(0f, 0.75f, 1.2f), new Vector3(2.2f, 0.1f, 1.1f), deskMat, root.transform);

        // Лампа в комнате
        GameObject lampObj = new GameObject("Home_Lamp");
        lampObj.transform.SetParent(root.transform);
        lampObj.transform.position = homeBase + new Vector3(-0.6f, 1.6f, 1.2f);
        Light hl = (Light)lampObj.AddComponent(typeof(Light));
        hl.type = LightType.Point;
        hl.range = 6f;
        hl.intensity = 2.0f;
        hl.color = new Color(1f, 0.82f, 0.6f);

        // Звонящий смартфон на домашнем столе
        GameObject phone = WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Home_Phone", homeBase + new Vector3(0.15f, 0.82f, 1.05f), new Vector3(0.18f, 0.02f, 0.34f), phoneBody, root.transform);
        phone.transform.rotation = Quaternion.Euler(0f, -15f, 0f);
        WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Phone_Call_Glow", new Vector3(0f, 0.6f, 0f), new Vector3(0.86f, 0.2f, 0.88f), callScreen, phone.transform);
        phoneTransform = phone.transform;

        // === 2. НОЧНАЯ ТРАССА И АВТОМОБИЛЬ (в координатах Y = 100) ===
        Vector3 roadBase = new Vector3(0f, 100f, 0f);
        WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Night_Road", roadBase + new Vector3(0f, 0f, 25f), new Vector3(9f, 0.2f, 90f), asphalt, root.transform);

        GameObject lines = new GameObject("Road_Stripes");
        lines.transform.SetParent(root.transform);
        lines.transform.position = roadBase;
        for (int i = 0; i != 12; i++)
        {
            WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Stripe_" + i, new Vector3(0f, 0.12f, i * 6f), new Vector3(0.22f, 0.02f, 2.5f), stripeMat, lines.transform);
        }
        roadLinesGroup = lines.transform;

        // Салон, капот, руль и фары машины
        GameObject car = new GameObject("Player_Car");
        car.transform.SetParent(root.transform);
        car.transform.position = roadBase + new Vector3(0f, 0.35f, 0f);

        WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Car_Hood", new Vector3(0f, 0.45f, 1.45f), new Vector3(1.75f, 0.25f, 1.5f), carBody, car.transform);
        WBProceduralAnimator.MakePart(PrimitiveType.Cube, "Car_Dash", new Vector3(0f, 0.58f, 0.65f), new Vector3(1.7f, 0.32f, 0.45f), dashMat, car.transform);

        GameObject wheel = WBProceduralAnimator.MakePart(PrimitiveType.Cylinder, "Steering_Wheel", new Vector3(-0.35f, 0.72f, 0.42f), new Vector3(0.36f, 0.03f, 0.36f), dashMat, car.transform);
        wheel.transform.localRotation = Quaternion.Euler(65f, 0f, 0f);

        // Свет фар автомобиля впереди
        GameObject headLightObj = new GameObject("Car_Headlights");
        headLightObj.transform.SetParent(car.transform);
        headLightObj.transform.localPosition = new Vector3(0f, 0.5f, 2.2f);
        Light carLight = (Light)headLightObj.AddComponent(typeof(Light));
        carLight.type = LightType.Spot;
        carLight.range = 45f;
        carLight.spotAngle = 75f;
        carLight.intensity = 4.5f;
        carLight.color = new Color(1f, 0.96f, 0.85f);

        carRoot = car.transform;
        return root;
    }
}
