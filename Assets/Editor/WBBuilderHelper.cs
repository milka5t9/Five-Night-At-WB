using UnityEngine;

public static class WBBuilderHelper
{
    public static GameObject CreateFolder(string name, Transform parent)
    {
        GameObject f = new GameObject(name);
        f.transform.SetParent(parent);
        return f;
    }

    public static GameObject CreateBox(string name, Vector3 pos, Vector3 scale, Material mat, Transform parent)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.position = pos;
        obj.transform.localScale = scale;
        obj.GetComponent<Renderer>().sharedMaterial = mat;
        obj.transform.SetParent(parent);
        return obj;
    }

    public static Material CreateMat(Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        Material mat = new Material(shader);
        mat.color = color;
        return mat;
    }

    public static void CreateCam(string name, Vector3 pos, Vector3 rot, Transform parent)
    {
        GameObject c = CreateFolder(name, parent);
        c.transform.position = pos;
        c.transform.rotation = Quaternion.Euler(rot);
        Camera cam = c.AddComponent<Camera>();
        cam.enabled = false;
    }

    public static void CreateDoorWall(string name, Vector3 center, float w, float h, float dw, float dh, bool rot90, Material mat, Transform parent)
    {
        GameObject wg = CreateFolder(name, parent);
        wg.transform.position = center;

        float sw = (w - dw) / 2f;
        float th = h - dh;

        CreateBox("Pillar_L", center + new Vector3(-(dw / 2f + sw / 2f), h / 2f, 0f), new Vector3(sw, h, 0.2f), mat, wg.transform);
        CreateBox("Pillar_R", center + new Vector3(dw / 2f + sw / 2f, h / 2f, 0f), new Vector3(sw, h, 0.2f), mat, wg.transform);
        CreateBox("Top_Beam", center + new Vector3(0f, dh + th / 2f, 0f), new Vector3(dw, th, 0.2f), mat, wg.transform);

        if (rot90) wg.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
    }

    public static void BuildRack(Vector3 pos, Transform parent, Material metal, Material boxMat, Material bagMat)
    {
        GameObject rack = CreateFolder("Warehouse_Rack", parent);
        rack.transform.position = pos;

        float w = 1.0f, l = 2.6f, h = 2.6f;
        CreateBox("Leg_1", pos + new Vector3(-w / 2f, h / 2f, -l / 2f), new Vector3(0.08f, h, 0.08f), metal, rack.transform);
        CreateBox("Leg_2", pos + new Vector3(w / 2f, h / 2f, -l / 2f), new Vector3(0.08f, h, 0.08f), metal, rack.transform);
        CreateBox("Leg_3", pos + new Vector3(-w / 2f, h / 2f, l / 2f), new Vector3(0.08f, h, 0.08f), metal, rack.transform);
        CreateBox("Leg_4", pos + new Vector3(w / 2f, h / 2f, l / 2f), new Vector3(0.08f, h, 0.08f), metal, rack.transform);

        for (int i = 0; i < 4; i++)
        {
            float y = 0.35f + i * 0.68f;
            CreateBox("Shelf_" + i, pos + new Vector3(0f, y, 0f), new Vector3(w, 0.05f, l), metal, rack.transform);
            CreateBox("Box_A_" + i, pos + new Vector3(0f, y + 0.18f, -0.7f), new Vector3(0.55f, 0.3f, 0.6f), boxMat, rack.transform);
            CreateBox("Bag_B_" + i, pos + new Vector3(0.05f, y + 0.14f, 0.1f), new Vector3(0.5f, 0.22f, 0.45f), bagMat, rack.transform);
        }
    }
}
