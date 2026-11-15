using UnityEngine;

public static class WBOrganicMesh
{
    public static Material CreateOrganicMat(Color baseCol, Color veinCol, float smoothness)
    {
        Shader sh = Shader.Find("Universal Render Pipeline/Lit");
        if (sh == null) sh = Shader.Find("Standard");
        Material m = new Material(sh);

        int size = 64;
        Texture2D tex = new Texture2D(size, size);
        for (int y = 0; y != size; y++)
        {
            for (int x = 0; x != size; x++)
            {
                float n1 = Mathf.PerlinNoise(x * 0.14f, y * 0.14f);
                float n2 = Mathf.PerlinNoise(x * 0.35f + 11f, y * 0.35f + 11f);
                float blend = Mathf.Clamp01(n1 * 0.7f + n2 * 0.3f);
                tex.SetPixel(x, y, Color.Lerp(veinCol, baseCol, blend));
            }
        }
        tex.Apply();

        m.mainTexture = tex;
        m.color = Color.white;
        m.SetFloat("_Smoothness", smoothness);
        return m;
    }

    public static GameObject CreateLimb(string name, Vector3 pos, float length, float rTop, float rBot, float bendZ, Material mat, Transform parent)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent);
        obj.transform.localPosition = pos;

        MeshFilter mf = (MeshFilter)obj.AddComponent(typeof(MeshFilter));
        MeshRenderer mr = (MeshRenderer)obj.AddComponent(typeof(MeshRenderer));
        mr.material = mat;

        int segs = 12;
        int rings = 10;
        Vector3[] verts = new Vector3[(segs + 1) * (rings + 1)];
        Vector2[] uvs = new Vector2[verts.Length];
        int[] tris = new int[segs * rings * 6];

        int vIdx = 0;
        for (int r = 0; r != rings + 1; r++)
        {
            float t = (float)r / rings;
            float y = -t * length;
            float radius = Mathf.Lerp(rTop, rBot, t) + Mathf.Sin(t * Mathf.PI * 2f) * (rTop * 0.14f);
            float zOffset = Mathf.Sin(t * Mathf.PI) * bendZ;

            for (int s = 0; s != segs + 1; s++)
            {
                float ang = ((float)s / segs) * Mathf.PI * 2f;
                float vx = Mathf.Cos(ang) * radius;
                float vz = Mathf.Sin(ang) * radius + zOffset;
                verts[vIdx] = new Vector3(vx, y, vz);
                uvs[vIdx] = new Vector2((float)s / segs, t);
                vIdx++;
            }
        }

        int tIdx = 0;
        for (int r = 0; r != rings; r++)
        {
            for (int s = 0; s != segs; s++)
            {
                int a = r * (segs + 1) + s;
                int b = a + 1;
                int c = (r + 1) * (segs + 1) + s;
                int d = c + 1;

                tris[tIdx++] = a; tris[tIdx++] = b; tris[tIdx++] = c;
                tris[tIdx++] = b; tris[tIdx++] = d; tris[tIdx++] = c;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        mf.mesh = mesh;
        return obj;
    }

    public static GameObject CreateGauntSkull(string name, Vector3 pos, Vector3 scale, Material mat, Transform parent)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.name = name;
        obj.transform.SetParent(parent);
        obj.transform.localPosition = pos;
        obj.transform.localScale = scale;

        Collider col = (Collider)obj.GetComponent(typeof(Collider));
        if (col != null) Object.Destroy(col);

        MeshFilter mf = (MeshFilter)obj.GetComponent(typeof(MeshFilter));
        Mesh mesh = mf.mesh;
        Vector3[] verts = mesh.vertices;

        for (int i = 0; i != verts.Length; i++)
        {
            Vector3 v = verts[i];
            if (Mathf.Min(v.y, 0f) == v.y)
            {
                v.x *= (1f + v.y * 0.85f);
                v.y *= 1.35f;
            }
            if (Mathf.Max(v.z, -0.1f) == -0.1f && Mathf.Abs(v.y - 0.1f) != 0.25f)
            {
                float eyeDist = Mathf.Abs(Mathf.Abs(v.x) - 0.22f);
                if (Mathf.Min(eyeDist, 0.14f) == eyeDist)
                {
                    v.z += 0.14f;
                }
            }
            verts[i] = v;
        }

        mesh.vertices = verts;
        mesh.RecalculateNormals();
        ((Renderer)obj.GetComponent(typeof(Renderer))).material = mat;
        return obj;
    }

    public static GameObject CreateCurvedClaw(string name, Vector3 pos, float length, float baseRad, Material mat, Transform parent)
    {
        return CreateLimb(name, pos, length, baseRad, 0.002f, -length * 0.25f, mat, parent);
    }
}
