using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace UniCAVE
{
    /// <summary>
    /// Dewarp is responsible for generating the dewarp meshes 
    /// used for warp calibration.
    /// 
    /// Author: Christoffer A Træen
    /// </summary>
    public class Dewarp
    {
        /// <summary>
        /// Holds the positions for the Dewarp mesh
        /// </summary>
        [Serializable]
        public class DewarpMeshPosition
        {

            [Header("Mesh calibrated vertices (do not add extra verts):")]
            public Vector3[] verts;

            /// <summary>
            /// Holds generated vertecies: filled if verts are empty
            /// </summary>
            public Vector3[] generatedVerts;

        }

        /// <summary>
        /// The vertices size on the x axis
        /// </summary>
        public int XSize { get; private set; } = 7;

        /// <summary>
        /// The vertices size on the y axis
        /// </summary>
        public int YSize { get; private set; } = 7;

        /// <summary>
        /// The game object name for the dewarp mesh
        /// </summary>
        private readonly string _objectName = "Dewarp Mesh For:";

        /// <summary>
        /// Holds the gameobject reference for the dewarp mesh
        /// </summary>
        private GameObject _dewarpObject;

        /// <summary>
        /// Holds the reference to the dewarp mesh
        /// </summary>
        private Mesh _warpMesh;

        /// <summary>
        /// Holds the reference to the dewarp mesh filter
        /// </summary>
        private MeshFilter _warpMeshFilter;

        /// <summary>
        /// Render materioal
        /// </summary>
        private Material _renderMaterial;

        private DewarpMeshPosition _calibratedVerticesPositions;

        /// <summary>
        /// Holds the calibrated dewarp positions
        /// </summary>
        public DewarpMeshPosition CalibratedVerticesPositions
        {
            get => _calibratedVerticesPositions;
            private set
            {
                Assert.IsNotNull(value, "Vertices positions object cant be null");
                _calibratedVerticesPositions = value;
            }
        }

        private Material _postProcessMaterial;

        /// <summary>
        /// Holds the Post process material for the mesh.
        /// </summary>
        /// <value></value>
        private Material PostProcessMaterial
        {
            get => _postProcessMaterial;
            set
            {
                Assert.IsNotNull(value, "Postprocess material can't be null");
                _postProcessMaterial = value;
            }
        }

        private RenderTexture _cameraTexture;

        /// <summary>
        /// Holds the Render texture for the render camera
        /// </summary>
        /// <value></value>
        private RenderTexture CameraTexture
        {
            get => _cameraTexture;
            set
            {
                Assert.IsNotNull(value,"Dewarp render texture can't be null");
                _cameraTexture = value;
            }
        }

        /// <summary>
        /// Creates a new dewarp mesh game object, and sets the nessecary dependncies for generation of the mesh.
        /// 
        /// /// The dewarp mesh takes a post process material, Dewarp mesh verticies positions and a render texture.
        /// The Post process material can be the UniCave postprocess material which enables edge blending and debugging.
        /// The DewarpMeshPositions is positions of imported verticies from config. So it can generate a mesh based
        /// on your loaded mesh calibration.
        /// The render texture is for the camera displaying this dewarp mesh.
        /// 
        /// </summary>
        /// <param name="name">the name of the gameobject</param>
        /// <param name="postProcessMaterial">post process material</param>
        /// <param name="verticesPositions">positions of vertices from calibration</param>
        /// <param name="cameraTexture">render texture for the camera</param>
        public Dewarp(string name, Material postProcessMaterial, DewarpMeshPosition verticesPositions, RenderTexture cameraTexture)
        {

            _dewarpObject = new GameObject(_objectName + name);
            PostProcessMaterial = postProcessMaterial;
            CalibratedVerticesPositions = verticesPositions;
            CameraTexture = cameraTexture;
            GenerateMesh();
            GenerateAndAssignMaterials(name);
        }

        /// <summary>
        /// Sets the resolution of the warp mesh.
        /// The total resolution will be x*y
        /// </summary>
        /// <param name="x">vertices on the x axis</param>
        /// <param name="y">vertices on the y axis</param>
        public void SetMeshResolution(int x = 2, int y = 2)
        {
            Assert.IsTrue(XSize < 2, "X must have at least 2 vertices");
            Assert.IsTrue(YSize < 2, "Y must have at least 2 vertices");
            XSize = x;
            YSize = y;
        }

        /// <summary>
        /// Generates the dewarp mesh and assign the .
        /// </summary>
        private void GenerateMesh()
        {
            _warpMeshFilter = _dewarpObject.AddComponent<MeshFilter>();
            _warpMeshFilter.mesh = Generate();
        }

        /// <summary>
        /// Creates the materials and textures for the dewarp mesh
        /// </summary>
        private void GenerateAndAssignMaterials(string nameForMaterial)
        {

            _renderMaterial = new Material(_postProcessMaterial)
            {
                name = $"Material for {nameForMaterial}",
                mainTexture = _cameraTexture
            };

            _dewarpObject.AddComponent<MeshRenderer>().material = _renderMaterial;
        }

        /// <summary>
        /// Sets the layer of where the gameobject should render.
        /// </summary>
        /// <param name="layer">the layer to render on</param>
        public void SetPostprocessLayer(int layer)
        {
            Assert.IsNotNull(_dewarpObject,"Cant set layer, game object is null");
            _dewarpObject.layer = layer;
        }

        /// <summary>
        /// Generates the dewarp mesh.
        /// The mesh has a total of <c>xSize*ySize</c> vertices.
        /// Normals, tangtents, UVs and triangles are automaticaly generated.
        /// 
        /// Vertices starts from bottom left.
        /// </summary>
        /// <returns>returns the generated mesh</returns>
        private Mesh Generate()
        {
            _warpMesh = new Mesh
            {
                name = "Warp mesh"
            };

            int vertCount = _calibratedVerticesPositions.generatedVerts.Length;
            
            _calibratedVerticesPositions.generatedVerts = new Vector3[(XSize + 1) * (YSize + 1)];
            Vector2[] uv = new Vector2[vertCount];
            Vector4[] tangents = new Vector4[vertCount];
            Vector4 tangent = new(1f, 0f, 0f, -1f);

            decimal xx = XSize; // Coercing from int
            decimal yy = YSize;

            decimal ymodifier = (2 / yy);
            decimal lastY = -1;

            decimal xmodifier = (2 / xx);
            decimal lastX = -1;

            // Set verices positions and generate uvs and tangents
            for(int i = 0, y = 0; y <= YSize; y++)
            {
                for(int x = 0; x <= XSize; x++, i++)
                {
                    if(_calibratedVerticesPositions.verts.Length == _calibratedVerticesPositions.generatedVerts.Length)
                    {
                        _calibratedVerticesPositions.generatedVerts[i] = new Vector3(
                            _calibratedVerticesPositions.verts[i].x, 
                            _calibratedVerticesPositions.verts[i].y); // z = 0 by constructor
                    }
                    else
                    {
                        _calibratedVerticesPositions.generatedVerts[i] = new Vector3((float)lastX, (float)lastY);
                    }
                    // Flat UV mapping
                    uv[i] = new Vector2((float)x / XSize, (float)y / YSize);
                    tangents[i] = tangent;
                    lastX += xmodifier;

                }
                lastY += ymodifier;
                lastX = -1;
            }

            // Generates mesh triangles
            int[] triangles = new int[XSize * YSize * 6];
            for(int ti = 0, vi = 0, y = 0; y < YSize; y++, vi++)
            {
                // Generate a pair of triangles covering the x/y square
                for(int x = 0; x < XSize; x++, ti += 6, vi++)
                {
                    triangles[ti] = vi;   // Top left
                    triangles[ti + 2] = triangles[ti + 3] = vi + 1; // Top right
                    triangles[ti + 1] = triangles[ti + 4] = vi + XSize + 1; // Bottom left
                    triangles[ti + 5] = vi + XSize + 2; // Bottom right
                }
            }
            _warpMesh.vertices = _calibratedVerticesPositions.generatedVerts;
            _warpMesh.triangles = triangles;
            _warpMesh.uv = uv;
            _warpMesh.tangents = tangents;
            _warpMesh.RecalculateNormals();
            _warpMesh.RecalculateTangents();
            if(_calibratedVerticesPositions.verts.Length == 0)
            {
                _calibratedVerticesPositions.verts = _calibratedVerticesPositions.generatedVerts;
            }

            return _warpMesh;
        }

        /// <summary>
        /// Returns the GameObject for the dewarp mesh
        /// </summary>
        /// <returns>GameObject for the dewarp mesh</returns>
        public GameObject GetDewarpGameObject()
        {
            return _dewarpObject;
        }

        /// <summary>
        /// Returns the Dewarp mesh
        /// </summary>
        /// <returns>reference of the warp mesh</returns>
        public Mesh GetDewarpMesh()
        {
            return _warpMesh;
        }

        /// <summary>
        /// Returns the Dewarp mesh filter
        /// </summary>
        /// <returns>reference of the warp mesh filter </returns>
        public MeshFilter GetDewarpMeshFilter()
        {
            return _warpMeshFilter;
        }

    }
}