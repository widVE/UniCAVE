/*
The MIT License(MIT)
Copyright(c) 2016 Digital Ruby, LLC
http://www.digitalruby.com

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using UnityEngine;
using System.Collections.Generic;

namespace DigitalRuby.Earth
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    public class SphereScript : MonoBehaviour
    {
        private struct MiddlePointCacheKey
        {
            public int Key1;
            public int Key2;

            public override bool Equals(object obj)
            {
                if (obj is MiddlePointCacheKey)
                {
                    MiddlePointCacheKey k = (MiddlePointCacheKey)obj;
                    return (k.Key1 == Key1 && k.Key2 == Key2);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return Key1.GetHashCode() + Key2.GetHashCode();
            }
        }

        private struct TriangleIndices
        {
            public int v1;
            public int v2;
            public int v3;

            public TriangleIndices(int v1, int v2, int v3)
            {
                this.v1 = v1;
                this.v2 = v2;
                this.v3 = v3;
            }
        }

        [Range(0.1f, 100.0f)]
        [Tooltip("Sphere Radius")]
        public float Radius = 20.0f;

        [Range(2, 6)]
        [Tooltip("Detail level. The higher, the more triangles.")]
        public int Detail = 4;

        [Tooltip("Whether to use an IcoSphere or UV sphere. IcoSphere are better for planets. Both use UV coordinates properly.")]
        public bool IcoSphere = true;

        [HideInInspector]
        [SerializeField]
        private float lastRadius;

        [HideInInspector]
        [SerializeField]
        private int lastDetail;

        [HideInInspector]
        [SerializeField]
        private bool lastIcoSphere;

        [HideInInspector]
        [SerializeField]
        private bool dirty = true;

#if UNITY_EDITOR

        private void CheckDirty()
        {
            if (Radius != lastRadius)
            {
                lastRadius = Radius;
                dirty = true;
            }
            if (Detail != lastDetail)
            {
                lastDetail = Detail;
                dirty = true;
            }
            if (IcoSphere != lastIcoSphere)
            {
                lastIcoSphere = IcoSphere;
                dirty = true;
            }
            if (GetComponent<MeshFilter>().sharedMesh == null)
            {
                dirty = true;
            }
        }

        private void CreateUVSphere()
        {
            #region Setup

            MeshFilter filter = GetComponent<MeshFilter>();
            Mesh mesh = filter.sharedMesh;
            if (mesh == null)
            {
                mesh = filter.sharedMesh = new Mesh();
                mesh.name = gameObject.name + "_Mesh";
            }
            mesh.Clear();

            int latitudeCount = 10 * Detail;
            int longitudeCount = 15 * Detail;

            #endregion Setup

            #region Vertices
            Vector3[] vertices = new Vector3[(longitudeCount + 1) * latitudeCount + 2];
            float _pi = Mathf.PI;
            float _2pi = _pi * 2f;

            vertices[0] = Vector3.up * Radius;
            for (int lat = 0; lat < latitudeCount; lat++)
            {
                float a1 = _pi * (float)(lat + 1) / (latitudeCount + 1);
                float sin1 = Mathf.Sin(a1);
                float cos1 = Mathf.Cos(a1);

                for (int lon = 0; lon <= longitudeCount; lon++)
                {
                    float a2 = _2pi * (float)(lon == longitudeCount ? 0 : lon) / longitudeCount;
                    float sin2 = Mathf.Sin(a2);
                    float cos2 = Mathf.Cos(a2);

                    vertices[lon + lat * (longitudeCount + 1) + 1] = new Vector3(sin1 * cos2, cos1, sin1 * sin2) * Radius;
                }
            }
            vertices[vertices.Length - 1] = Vector3.up * -Radius;
            #endregion

            #region Normales		
            Vector3[] normales = new Vector3[vertices.Length];
            for (int n = 0; n < vertices.Length; n++)
                normales[n] = vertices[n].normalized;
            #endregion

            #region UVs
            Vector2[] uvs = new Vector2[vertices.Length];
            uvs[0] = Vector2.up;
            uvs[uvs.Length - 1] = Vector2.zero;
            for (int lat = 0; lat < latitudeCount; lat++)
                for (int lon = 0; lon <= longitudeCount; lon++)
                    uvs[lon + lat * (longitudeCount + 1) + 1] = new Vector2((float)lon / longitudeCount, 1f - (float)(lat + 1) / (latitudeCount + 1));
            #endregion

            #region Triangles
            int nbFaces = vertices.Length;
            int nbTriangles = nbFaces * 2;
            int nbIndexes = nbTriangles * 3;
            int[] triangles = new int[nbIndexes];

            //Top Cap
            int i = 0;
            for (int lon = 0; lon < longitudeCount; lon++)
            {
                triangles[i++] = lon + 2;
                triangles[i++] = lon + 1;
                triangles[i++] = 0;
            }

            //Middle
            for (int lat = 0; lat < latitudeCount - 1; lat++)
            {
                for (int lon = 0; lon < longitudeCount; lon++)
                {
                    int current = lon + lat * (longitudeCount + 1) + 1;
                    int next = current + longitudeCount + 1;

                    triangles[i++] = current;
                    triangles[i++] = current + 1;
                    triangles[i++] = next + 1;

                    triangles[i++] = current;
                    triangles[i++] = next + 1;
                    triangles[i++] = next;
                }
            }

            //Bottom Cap
            for (int lon = 0; lon < longitudeCount; lon++)
            {
                triangles[i++] = vertices.Length - 1;
                triangles[i++] = vertices.Length - (lon + 2) - 1;
                triangles[i++] = vertices.Length - (lon + 1) - 1;
            }
            #endregion

            #region Finish Up

            mesh.vertices = vertices;
            mesh.normals = normales;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
            ;

            #endregion Finish Up
        }

        // return index of point in the middle of p1 and p2
        private int GetMiddlePoint(int p1, int p2, List<Vector3> vertices, Dictionary<MiddlePointCacheKey, int> middlePointIndexCache)
        {
            // first check if we have it already
            int smallerIndex = Mathf.Min(p1, p2);
            int greaterIndex = Mathf.Max(p1, p2);
            MiddlePointCacheKey key = new MiddlePointCacheKey { Key1 = smallerIndex, Key2 = greaterIndex };

            int ret;
            if (middlePointIndexCache.TryGetValue(key, out ret))
            {
                return ret;
            }

            // not in cache, calculate it
            Vector3 point1 = vertices[p1];
            Vector3 point2 = vertices[p2];
            Vector3 middle = new Vector3
            (
                (point1.x + point2.x) / 2f,
                (point1.y + point2.y) / 2f,
                (point1.z + point2.z) / 2f
            );

            // add vertex makes sure point is on unit sphere
            int i = vertices.Count;
            vertices.Add(middle.normalized * Radius);

            // store it, return index
            middlePointIndexCache.Add(key, i);

            return i;
        }

        public void CreateIcoSphere()
        {
            MeshFilter filter = GetComponent<MeshFilter>();
            Mesh mesh = filter.sharedMesh;
            if (mesh == null)
            {
                mesh = filter.sharedMesh = new Mesh();
                mesh.name = gameObject.name + "_Mesh";
            }
            mesh.Clear();

            Dictionary<MiddlePointCacheKey, int> middlePointIndexCache = new Dictionary<MiddlePointCacheKey, int>();
            List<Vector3> vertList = new List<Vector3>();
            List<int> triList = new List<int>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();

            // create 12 vertices of a icosahedron
            float t = (1f + Mathf.Sqrt(5f)) / 2f;

            vertList.Add(new Vector3(-1f, t, 0f).normalized * Radius);
            vertList.Add(new Vector3(1f, t, 0f).normalized * Radius);
            vertList.Add(new Vector3(-1f, -t, 0f).normalized * Radius);
            vertList.Add(new Vector3(1f, -t, 0f).normalized * Radius);

            vertList.Add(new Vector3(0f, -1f, t).normalized * Radius);
            vertList.Add(new Vector3(0f, 1f, t).normalized * Radius);
            vertList.Add(new Vector3(0f, -1f, -t).normalized * Radius);
            vertList.Add(new Vector3(0f, 1f, -t).normalized * Radius);

            vertList.Add(new Vector3(t, 0f, -1f).normalized * Radius);
            vertList.Add(new Vector3(t, 0f, 1f).normalized * Radius);
            vertList.Add(new Vector3(-t, 0f, -1f).normalized * Radius);
            vertList.Add(new Vector3(-t, 0f, 1f).normalized * Radius);

            // create 20 triangles of the icosahedron
            List<TriangleIndices> faces = new List<TriangleIndices>();

            // 5 faces around point 0
            faces.Add(new TriangleIndices(0, 11, 5));
            faces.Add(new TriangleIndices(0, 5, 1));
            faces.Add(new TriangleIndices(0, 1, 7));
            faces.Add(new TriangleIndices(0, 7, 10));
            faces.Add(new TriangleIndices(0, 10, 11));

            // 5 adjacent faces 
            faces.Add(new TriangleIndices(1, 5, 9));
            faces.Add(new TriangleIndices(5, 11, 4));
            faces.Add(new TriangleIndices(11, 10, 2));
            faces.Add(new TriangleIndices(10, 7, 6));
            faces.Add(new TriangleIndices(7, 1, 8));

            // 5 faces around point 3
            faces.Add(new TriangleIndices(3, 9, 4));
            faces.Add(new TriangleIndices(3, 4, 2));
            faces.Add(new TriangleIndices(3, 2, 6));
            faces.Add(new TriangleIndices(3, 6, 8));
            faces.Add(new TriangleIndices(3, 8, 9));

            // 5 adjacent faces 
            faces.Add(new TriangleIndices(4, 9, 5));
            faces.Add(new TriangleIndices(2, 4, 11));
            faces.Add(new TriangleIndices(6, 2, 10));
            faces.Add(new TriangleIndices(8, 6, 7));
            faces.Add(new TriangleIndices(9, 8, 1));

            // refine triangles
            for (int i = 0; i < Detail; i++)
            {
                List<TriangleIndices> faces2 = new List<TriangleIndices>();
                foreach (var tri in faces)
                {
                    // replace triangle by 4 triangles
                    int a = GetMiddlePoint(tri.v1, tri.v2, vertList, middlePointIndexCache);
                    int b = GetMiddlePoint(tri.v2, tri.v3, vertList, middlePointIndexCache);
                    int c = GetMiddlePoint(tri.v3, tri.v1, vertList, middlePointIndexCache);

                    faces2.Add(new TriangleIndices(tri.v1, a, c));
                    faces2.Add(new TriangleIndices(tri.v2, b, a));
                    faces2.Add(new TriangleIndices(tri.v3, c, b));
                    faces2.Add(new TriangleIndices(a, b, c));
                }
                faces = faces2;
            }

            for (int i = 0; i < vertList.Count; i++)
            {
                Vector3 n = vertList[i].normalized;
                normals.Add(n);
                float u = 0.5f - (0.5f * Mathf.Atan2(n.x, n.z) / Mathf.PI);
                float v = 1.0f - Mathf.Acos(n.y) / Mathf.PI;
                uvs.Add(new Vector2(u, v));
            }

            for (int i = 0; i < faces.Count; i++)
            {
                triList.Add(faces[i].v1);
                triList.Add(faces[i].v2);
                triList.Add(faces[i].v3);
            }

            System.Action<int, Vector2> fixVertex = (int i, Vector2 uv) =>
            {
                int index = triList[i];
                triList[i] = vertList.Count;
                vertList.Add(vertList[index]);
                normals.Add(normals[index]);
                uvs.Add(uv);
            };

            // fix texture seams
            for (int i = 0; i < triList.Count; i += 3)
            {
                Vector2 uv0 = uvs[triList[i]];
                Vector2 uv1 = uvs[triList[i + 1]];
                Vector2 uv2 = uvs[triList[i + 2]];
                float d1 = uv1.x - uv0.x;
                float d2 = uv2.x - uv0.x;
                if (Mathf.Abs(d1) > 0.5f)
                {
                    if (Mathf.Abs(d2) > 0.5f)
                    {
                        fixVertex(i, uv0 + new Vector2((d1 > 0.0f) ? 1.0f : -1.0f, 0.0f));
                    }
                    else
                    {
                        fixVertex(i + 1, uv1 + new Vector2((d1 < 0.0f) ? 1.0f : -1.0f, 0.0f));
                    }
                }
                else if (Mathf.Abs(d2) > 0.5f)
                {
                    fixVertex(i + 2, uv2 + new Vector2((d2 < 0.0f) ? 1.0f : -1.0f, 0.0f));
                }
            }

            mesh.SetVertices(vertList);
            mesh.SetUVs(0, uvs);
            mesh.SetNormals(normals);
            mesh.SetTriangles(triList, 0);
            mesh.RecalculateBounds();
            ;

            middlePointIndexCache.Clear();
        }

#endif

        protected virtual void Start()
        {

#if UNITY_EDITOR

            if (Application.isPlaying)
            {
                CheckDirty();
                dirty = false;
            }

#endif

        }

        protected virtual void Update()
        {

#if UNITY_EDITOR

            CheckDirty();

            if (dirty)
            {
                dirty = false;
                if (IcoSphere)
                {
                    CreateIcoSphere();
                }
                else
                {
                    CreateUVSphere();
                }
            }

#endif

        }
    }
}