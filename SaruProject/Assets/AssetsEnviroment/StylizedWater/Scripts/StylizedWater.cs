// Stylized Water Shader by Staggart Creations http://u3d.as/A2R
// Online documentation can be found at http://staggart.xyz

using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
[HelpURL("http://staggart.xyz/unity/stylized-water-shader/documentation/")]
public class StylizedWater : MonoBehaviour
{
    #region Shader data
    public string[] shaderNames;
    public int shaderIndex = 0;

    public Shader shader;
    private Shader DesktopShader;
    private Shader DesktopTessellationShader;
    private Shader DesktopBetaShader;
    private Shader MobileAdvancedShader;
    private Shader MobileBasicShader;
    #endregion

    #region BETA
    public bool isUnlit;
    public Color waterShallowColor;
    [Range(0f, 1f)]
    public float waveFoam;
    public Vector4 waveDirection;
    [Range(0f, 1f)]
    public float reflectionStrength = 1f;
    public bool showReflection;
    #endregion

    #region Shader properties
    //Color
    public Color waterColor;
    public Color fresnelColor;
    public Color rimColor;
    [Range(0f, 1f)]
    public float normalStrength;
    [Range(-0.2f, 0.2f)]
    public float waveTint;

    //Surface
    [Range(0f, 1f)]
    public float transparency;
    [Range(0f, 1f)]
    public float glossiness;
    public bool worldSpaceTiling;
    [Range(0f, 1f)]
    public float tiling;
    public Texture reflectionCubemap;

    public float fresnel;
    public bool useIntersectionHighlight;
    [Range(-1, 1)]
    public float surfaceHighlight;
    public float surfaceHighlightTiling;
    [Range(0f, 1f)]
    public float surfaceHighlightSize;
    public bool surfaceHighlightPanning;

    [Range(0f, 30f)]
    public float depth;
    [Range(0f, 1f)]
    public float depthDarkness;

    [Range(0f, 10f)]
    public float rimSize;
    [Range(0f, 5f)]
    public float rimFalloff;
    [Range(0f, 1f)]
    public float rimDistance;

    public float rimTiling;

    [Range(0f, 0.2f)]
    public float refractionAmount;

    [Range(0.01f, 10f)]
    public float waveSpeed;
    [Range(0f, 1f)]
    public float waveStrength;

    public Texture customIntersection;
    public Texture customNormal;
    public Texture substanceNormals;
    public Texture substanceShadermap;

    public Texture2D normals;
    public Texture2D shadermap;

    [Range(0.01f, 10f)]
    public float tessellation;
    #endregion

    #region  Substance variables

    //WebGL fix, ProceduralMaterial is not supported
#if UNITY_EDITOR
    public ProceduralMaterial substance;
#endif

    public float seed;

    public string[] intersectionStyleNames = new string[] { "None", "Windwaker", "Foamy", "Foamy2", "Triangular", "Cells", "Perlin" };
    public int intersectionStyle = 1;

    public string[] waveStyleNames = new string[] { "Perlin", "Plasma", "Circular", "Stream", "Sharp", "Cells", "Cloudy" };
    public int waveStyle;

    public string[] waveHeightmapNames = new string[] { "Smooth", "Sharp", "Billowed", "Wide" };
    public int waveHeightmapStyle;
    public float waveSize;


    public bool useCustomIntersection;
    public bool useCustomNormals;
    #endregion

    #region Reflection
    Camera reflectionCamera;
    public bool useReflection;

    public string[] reslist = new string[] { "64x64", "128x128", "256x256", "512x512", "1024x1024", "2048x2048" };
    public int reflectionRes;

    public int reflectionTextureSize = 256;

    public float clipPlaneOffset = 0.07f;
    public LayerMask reflectLayers = -1;
    public LayerMask depthLayers = -1;

    private Dictionary<Camera, Camera> m_ReflectionCameras = new Dictionary<Camera, Camera>(); // Camera -> Camera table

    private RenderTexture m_ReflectionTexture;

    private int m_OldReflectionTextureSize;
    #endregion

    #region Script vars
    private MeshRenderer meshRenderer;
    public Material material;
    public bool isBeta;
    public bool isMobilePlatform;
    public bool isMobileAdvanced = false;
    public bool isMobileBasic = false;
    public string shaderName = null;
    public bool isWaterLayer;
    public bool hasShaderParams = false;
    public bool hasMaterial;
    #endregion

    #region Toggles
    [Header("Toggles")]
    public bool showColors = true;
    public bool showSurface = true;
    public bool showIntersection;
    public bool showHighlights;
    public bool showDepth;
    public bool showWaves;
    public bool showAdvanced;
#if !UNITY_5_5_OR_NEWER
    public bool hideWireframe = false;
#endif
    #endregion

    #region Substance baking
#if UNITY_EDITOR
    private SubstanceBaker sb;
#endif
    public bool useCompression = false;
    #endregion

    #region Editor functions
#if UNITY_EDITOR
    public void BakeSubstance(bool useCompression)
    {
        this.useCompression = useCompression;
        //Before baking, make sure all settings are applied
        SetShaderProperties();
        SetSubstanceProperties(false);

        //Bake Substance outputs to textures
        if (!sb) sb = ScriptableObject.CreateInstance<SubstanceBaker>();
        sb.BakeSubstance(substance, material, useCompression);

        //Retreive the baked textures
        normals = sb.normalmap;
        shadermap = sb.shadermap;

        //Set baked textures to shader
        material.SetTexture("_Shadermap", shadermap);
        material.SetTexture("_Normals", normals);
    }

    public void Init()
    {
        meshRenderer = this.GetComponent<MeshRenderer>();
        //meshRenderer.sharedMaterial.hideFlags = HideFlags.HideInInspector;
    }

    public void OnDestroy()
    {
#if UNITY_EDITOR
        if (!meshRenderer || !meshRenderer.sharedMaterial) return;

#if !UNITY_5_5_OR_NEWER
        EditorUtility.SetSelectedWireframeHidden(meshRenderer, false);
#endif
        meshRenderer.sharedMaterial.hideFlags = HideFlags.None;
#endif


    }

    //Called through: OnEnable, OnInspectorGUI
    public void GetProperties()
    {
        //Debug.Log("StylizedWater.cs: getProperties()");

        //Determine platform, so limitations can be applied
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android ||
         EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
        {
            isMobilePlatform = true;
            shaderNames = new string[] { "Mobile Advanced", "Mobile Basic" };
            //Array.Copy(MobileShaderNames, shaderNames, 2);
        }
        else
        {
            isMobilePlatform = false;
            shaderNames = new string[] { "Desktop", "Desktop (DX11 Tessellation)", "Desktop Beta", "Mobile Advanced", "Mobile Basic" };
            //Array.Copy(DesktopShaderNames, shaderNames, 4);
        }

        //Requires typeof so is always succesfull
        material = meshRenderer.sharedMaterial;

        if (!substance) FindSubstance();

        GetShaderProperties();

#if !UNITY_5_5_OR_NEWER
        EditorUtility.SetSelectedWireframeHidden(meshRenderer, hideWireframe);
#endif

        //If maps are missing, bake new ones
        if (normals == null || shadermap == null)
        {
            BakeSubstance(useCompression);
        }

    }

    //Grab material values
    private void GetShaderProperties()
    {
        if (!material)
        {
            hasMaterial = false;
            return;
        }

        hasMaterial = true;

        //Find shaders
        DesktopShader = Shader.Find("StylizedWater/Desktop");
        DesktopTessellationShader = Shader.Find("StylizedWater/Desktop (DX11 Tessellation)");
        DesktopBetaShader = Shader.Find("StylizedWater/Desktop Beta");
        MobileAdvancedShader = Shader.Find("StylizedWater/Mobile Advanced");
        MobileBasicShader = Shader.Find("StylizedWater/Mobile Basic");

        //get current shader
        shader = material.shader;

        shaderName = shader.name;
        if (!shaderName.Contains("StylizedWater")) return;

        //Recognize shader
        if (isMobilePlatform)
        {
            //Shader level specific properties
            if (shader == MobileBasicShader)
            {
                isMobileBasic = true;
                isMobileAdvanced = false;

                shaderIndex = 1;
            }
            if (shader == MobileAdvancedShader)
            {
                isMobileAdvanced = true;
                isMobileBasic = false;

                shaderIndex = 0;
            }
        }
        else
        {
            if (shader == DesktopShader)
            {
                isMobileAdvanced = false;
                isMobileBasic = false;
                isBeta = false;

                shaderIndex = 0;
            }
            if (shader == DesktopTessellationShader)
            {
                isMobileAdvanced = false;
                isMobileBasic = false;
                isBeta = false;

                shaderIndex = 1;
            }
            if (shader == DesktopBetaShader)
            {
                isMobileAdvanced = false;
                isMobileBasic = false;
                isBeta = true;

                shaderIndex = 2;
            }
            if (shader == MobileAdvancedShader)
            {
                isMobileAdvanced = true;
                isMobileBasic = false;
                isBeta = false;

                shaderIndex = 3;
            }
            if (shader == MobileBasicShader)
            {
                isMobileBasic = true;
                isMobileAdvanced = false;
                isBeta = false;

                shaderIndex = 4;
            }
        }

        if (hasShaderParams == false)

            //Shared by all
            shadermap = material.GetTexture("_Shadermap") as Texture2D;
        normals = material.GetTexture("_Normals") as Texture2D;

        //Color
        waterColor = material.GetColor("_WaterColor");
        rimColor = material.GetColor("_RimColor");

        //Surface
        normalStrength = material.GetFloat("_NormalStrength");
        worldSpaceTiling = (material.GetFloat("_Worldspacetiling") == 1) ? true : false;
        tiling = material.GetFloat("_Tiling");
        transparency = material.GetFloat("_Transparency");
        glossiness = material.GetFloat("_Glossiness");

        //Intersection
        rimSize = material.GetFloat("_RimSize");
        rimFalloff = material.GetFloat("_Rimfalloff");
        rimTiling = material.GetFloat("_Rimtiling");

        //Waves
        waveSpeed = material.GetFloat("_Wavesspeed");

        //Desktop only
        if (!isMobileAdvanced && !isMobileBasic)
        {
            //Color
            waveTint = material.GetFloat("_Wavetint");
            fresnelColor = material.GetColor("_FresnelColor");
            fresnel = material.GetFloat("_Fresnelexponent");

            //Surface
            refractionAmount = material.GetFloat("_RefractionAmount");
            if (!isBeta)
            {
                reflectionCubemap = material.GetTexture("_Reflection");
            }

            //Intersection
            useIntersectionHighlight = (material.GetFloat("_UseIntersectionHighlight") == 1) ? true : false;

            //Surface highlights
            surfaceHighlightPanning = (material.GetFloat("_HighlightPanning") == 1) ? true : false;

            //Waves
            if (isBeta)
            {
                waterShallowColor = material.GetColor("_WaterShallowColor");
                isUnlit = (material.GetFloat("_Unlit") == 1) ? true : false;
                waveDirection = material.GetVector("_WaveDirection");
                waveFoam = material.GetFloat("_WaveFoam");
                tessellation = material.GetFloat("_Tessellation");
                reflectionStrength = material.GetFloat("_ReflectionStrength");
                waveStrength = material.GetFloat("_WaveHeight");
            }
            else
            {
                waveStrength = material.GetFloat("_Wavesstrength");
            }
            waveSize = material.GetFloat("_WaveSize");
        }

        //Desktop and Mobile Advanced
        if (!isMobileBasic)
        {
            surfaceHighlight = material.GetFloat("_SurfaceHighlight");
            surfaceHighlightTiling = material.GetFloat("_SurfaceHightlighttiling");
            surfaceHighlightSize = material.GetFloat("_Surfacehightlightsize");

            depth = material.GetFloat("_Depth");
            if (!isBeta)
            {
                depthDarkness = material.GetFloat("_Depthdarkness");
            }

            rimDistance = material.GetFloat("_RimDistance");
        }

        //Desktop Tesselation shader only
        if (shader == DesktopTessellationShader || shader == DesktopBetaShader)
        {
            tessellation = material.GetFloat("_Tessellation");
        }

        hasShaderParams = true;
    }

    //Apply values to material shader
    public void SetShaderProperties()
    {
        if (!material) return;

        //Shader level specific properties
        if (isMobilePlatform)
        {
            if (shaderIndex == 0)
            {
                shader = MobileAdvancedShader;
            }
            else
            {
                shader = MobileBasicShader;
            }

        }
        else
        {
            switch (shaderIndex)
            {
                case 0:
                    shader = DesktopShader;
                    break;
                case 1:
                    shader = DesktopTessellationShader;
                    break;
                case 2:
                    shader = DesktopBetaShader;
                    break;
                case 3:
                    shader = MobileAdvancedShader;
                    break;
                case 4:
                    shader = MobileBasicShader;
                    break;
            }

        }

        material.shader = shader;

        //Shared by all
        if (shadermap && normals)
        {
            material.SetTexture("_Shadermap", shadermap);
            material.SetTexture("_Normals", normals);
        }

        //Color
        material.SetColor("_WaterColor", waterColor);
        material.SetColor("_RimColor", rimColor);

        //Surface
        material.SetFloat("_NormalStrength", normalStrength);
        material.SetFloat("_Glossiness", glossiness);
        material.SetFloat("_Worldspacetiling", (worldSpaceTiling == true) ? 1 : 0);
        material.SetFloat("_Tiling", tiling);

        //Intersection
        material.SetFloat("_RimSize", rimSize);
        material.SetFloat("_Rimfalloff", rimFalloff);

        material.SetFloat("_Rimtiling", rimTiling);

        //Waves
        material.SetFloat("_Wavesspeed", waveSpeed);

        //Desktop only
        if (!isMobileAdvanced || !isMobileBasic)
        {
            //BETA
            if (isBeta)
            {
                material.SetColor("_WaterShallowColor", waterShallowColor);
                material.SetFloat("_Unlit", (isUnlit == true) ? 1 : 0);
                material.SetVector("_WaveDirection", waveDirection);
                material.SetFloat("_WaveFoam", waveFoam);
                material.SetFloat("_Tessellation", tessellation);
                material.SetFloat("_ReflectionStrength", reflectionStrength);
                material.SetFloat("_WaveHeight", waveStrength);
            }

            //Color
            material.SetFloat("_Wavetint", waveTint);

            //Surface
            material.SetFloat("_Transparency", transparency);
            material.SetFloat("_RefractionAmount", refractionAmount);
            material.SetTexture("_Reflection", reflectionCubemap);

            //Surface hightlighs
            material.SetFloat("_UseIntersectionHighlight", (useIntersectionHighlight == true) ? 1 : 0);
            material.SetFloat("_HighlightPanning", (surfaceHighlightPanning == true) ? 1 : 0);

            //Waves
            material.SetFloat("_Wavesstrength", waveStrength);
            material.SetFloat("_WaveSize", waveSize);

        }

        //Desktop and Mobile Advanced
        if (!isMobileBasic)
        {
            //Color
            material.SetColor("_FresnelColor", fresnelColor);
            material.SetFloat("_Fresnelexponent", fresnel);

            //Surface highlights
            material.SetFloat("_SurfaceHighlight", surfaceHighlight);
            material.SetFloat("_SurfaceHightlighttiling", surfaceHighlightTiling);
            material.SetFloat("_Surfacehightlightsize", surfaceHighlightSize);

            //Intersection
            material.SetFloat("_RimDistance", rimDistance);

            //Depth
            material.SetFloat("_Depth", depth);
            material.SetFloat("_Depthdarkness", depthDarkness);

        }

        //Desktop tessellation shader only
        if (shader == DesktopTessellationShader || shader == DesktopBetaShader)
        {
            material.SetFloat("_Tessellation", tessellation);
        }

    }

    //Locate the StylizedWater Substance material in the project
    private void FindSubstance()
    {

        string[] assets = AssetDatabase.FindAssets("t:ProceduralMaterial StylizedWater");
        string assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);

        SubstanceImporter si = AssetImporter.GetAtPath(assetPath) as SubstanceImporter; //Substance .sbsar container
        ProceduralMaterial[] substanceContainer = si.GetMaterials();

        //Look for the substance instance matching the material name we're looking for
        foreach (ProceduralMaterial substanceInstance in substanceContainer)
        {
            if (substanceInstance.name == "StylizedWater")
            {
                substance = substanceInstance; //Gotcha
                si.SetGenerateAllOutputs(substance, true);
                //Debug.Log("[Stylized Water] Succesfully located \"StylizedWater\" Substance in project.");

            }
        }

        if (!substance) Debug.LogError("[Stylized Water] The \"StylizedWater\" Substance material could not be located in the project. Was it removed or renamed?");

    }

    public void SetSubstanceProperties(bool reBake = true)
    {
        if (!substance) return; //Prevent from setting null values

        //Debug.Log("Setting value " + intersectionStyleList[intersectionStyle] + " to " + substance.name);

        substance.SetProceduralFloat("$randomseed", seed);

        if (useCustomIntersection)
        {
            substance.SetProceduralBoolean("useCustomIntersection", useCustomIntersection);
            if (customIntersection)
            {
                substance.SetProceduralTexture("customIntersectionTex", (Texture2D)customIntersection);
            }
        }
        else
        {
            substance.SetProceduralBoolean("useCustomIntersection", false);
            substance.SetProceduralTexture("customIntersectionTex", null);
        }

        if (useCustomNormals)
        {
            substance.SetProceduralBoolean("useCustomNormal", useCustomNormals);
            if (customNormal)
            {
                substance.SetProceduralTexture("customNormalTex", (Texture2D)customNormal);
            }
        }
        else
        {
            substance.SetProceduralBoolean("useCustomNormal", false);
            substance.SetProceduralTexture("customNormalTex", null);
        }

        //substance.SetProceduralBoolean("useIntersectionAsHighlight", useIntersectionHighlight);

        substance.SetProceduralEnum("intersectionStyle", intersectionStyle);
        substance.SetProceduralEnum("waveStyle", waveStyle);
        substance.SetProceduralEnum("waveHeightmap", waveHeightmapStyle);

        if (!substance.isProcessing && reBake) substance.RebuildTexturesImmediately();
    }
#endif //Editor functions end
    #endregion

    #region Render functions
    public void OnWillRenderObject()
    {

        if (!enabled || !material)
        {
            return;
        }

        Camera cam = Camera.current;
        if (!cam)
        {
            return;
        }

        Camera reflectionCamera;
        CreateWaterObjects(cam, out reflectionCamera);

        // find out the reflection plane: position and normal in world space
        Vector3 pos = transform.position;
        Vector3 normal = transform.up;

        #region Reflection
        // Render reflection
        if (useReflection)
        {
            UpdateCameraModes(cam, reflectionCamera);

            // Reflect camera around reflection plane
            float d = -Vector3.Dot(normal, pos) - clipPlaneOffset;
            Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

            Matrix4x4 reflection = Matrix4x4.zero;
            CalculateReflectionMatrix(ref reflection, reflectionPlane);
            Vector3 oldpos = cam.transform.position;
            Vector3 newpos = reflection.MultiplyPoint(oldpos);
            reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;

            // Setup oblique projection matrix so that near plane is our reflection
            // plane. This way we clip everything below/above it for free.
            Vector4 clipPlane = CameraSpacePlane(reflectionCamera, pos, normal, 1.0f);
            reflectionCamera.projectionMatrix = cam.CalculateObliqueMatrix(clipPlane);

            reflectionCamera.cullingMask = ~(1 << 4) & reflectLayers.value; // never render water layer
            reflectionCamera.targetTexture = m_ReflectionTexture;
            bool oldCulling = GL.invertCulling;
            GL.invertCulling = !oldCulling;
            reflectionCamera.transform.position = newpos;
            Vector3 euler = cam.transform.eulerAngles;
            reflectionCamera.transform.eulerAngles = new Vector3(-euler.x, euler.y, euler.z);

            //Manually invoke render method
            reflectionCamera.Render();
            reflectionCamera.transform.position = oldpos;
            GL.invertCulling = oldCulling;

            material.SetTexture("_ReflectionTex", m_ReflectionTexture);
        }
        #endregion


    }

    void UpdateCameraModes(Camera src, Camera dest)
    {
        if (dest == null)
        {
            return;
        }
        // set water camera to clear the same way as current camera
        dest.clearFlags = src.clearFlags;
        dest.backgroundColor = src.backgroundColor;

        // update other values to match current camera.
        // even if we are supplying custom camera&projection matrices,
        // some of values are used elsewhere (e.g. skybox uses far plane)
        dest.farClipPlane = src.farClipPlane;
        dest.nearClipPlane = src.nearClipPlane;
        dest.orthographic = src.orthographic;
        dest.fieldOfView = src.fieldOfView;
        dest.aspect = src.aspect;
        dest.orthographicSize = src.orthographicSize;
    }

    public void CreateReflectionTexture()
    {
        //Clear texture
        if (m_ReflectionTexture)
        {
            DestroyImmediate(m_ReflectionTexture);
            m_ReflectionTexture = null;
        }

        // Reflection render texture
        if (!m_ReflectionTexture || m_OldReflectionTextureSize != reflectionTextureSize)
        {

            // sws.reflectLayers = reflectLayers;
            switch (reflectionRes)
            {
                case 0:
                    reflectionTextureSize = 64;
                    break;
                case 1:
                    reflectionTextureSize = 128;
                    break;
                case 2:
                    reflectionTextureSize = 256;
                    break;
                case 3:
                    reflectionTextureSize = 512;
                    break;
                case 4:
                    reflectionTextureSize = 1024;
                    break;
                case 5:
                    reflectionTextureSize = 2048;
                    break;
            }

            m_ReflectionTexture = new RenderTexture(reflectionTextureSize, reflectionTextureSize, 16);
            m_ReflectionTexture.name = "__WaterReflection" + GetInstanceID();
            m_ReflectionTexture.isPowerOfTwo = true;
            m_ReflectionTexture.hideFlags = HideFlags.DontSave;
            m_OldReflectionTextureSize = reflectionTextureSize;
        }
    }

    // On-demand create any objects we need for water
    void CreateWaterObjects(Camera currentCamera, out Camera reflectionCamera)
    {

        reflectionCamera = null;

        #region Reflection cam
        if (useReflection)
        {

            CreateReflectionTexture();

            // Camera for reflection
            m_ReflectionCameras.TryGetValue(currentCamera, out reflectionCamera);
            if (!reflectionCamera) // catch both not-in-dictionary and in-dictionary-but-deleted-GO
            {
                GameObject go = new GameObject("", typeof(Camera));
                go.name = "Reflection Camera " + go.GetInstanceID() + " for " + currentCamera.name;
                go.hideFlags = HideFlags.HideAndDontSave;

                reflectionCamera = go.GetComponent<Camera>();
                //Disable component as Render() is to be called manually
                reflectionCamera.enabled = false;

                reflectionCamera.useOcclusionCulling = false;
                reflectionCamera.transform.position = transform.position;
                reflectionCamera.transform.rotation = transform.rotation;

                reflectionCamera.gameObject.AddComponent<FlareLayer>();

                m_ReflectionCameras[currentCamera] = reflectionCamera;
            }


        }
        #endregion
    }

    // Given position/normal of the plane, calculates plane in camera space.
    Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
    {
        Vector3 offsetPos = pos + normal * clipPlaneOffset;
        Matrix4x4 m = cam.worldToCameraMatrix;
        Vector3 cpos = m.MultiplyPoint(offsetPos);
        Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }

    // Calculates reflection matrix around the given plane
    static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
    {
        reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
        reflectionMat.m01 = (-2F * plane[0] * plane[1]);
        reflectionMat.m02 = (-2F * plane[0] * plane[2]);
        reflectionMat.m03 = (-2F * plane[3] * plane[0]);

        reflectionMat.m10 = (-2F * plane[1] * plane[0]);
        reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
        reflectionMat.m12 = (-2F * plane[1] * plane[2]);
        reflectionMat.m13 = (-2F * plane[3] * plane[1]);

        reflectionMat.m20 = (-2F * plane[2] * plane[0]);
        reflectionMat.m21 = (-2F * plane[2] * plane[1]);
        reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
        reflectionMat.m23 = (-2F * plane[3] * plane[2]);

        reflectionMat.m30 = 0F;
        reflectionMat.m31 = 0F;
        reflectionMat.m32 = 0F;
        reflectionMat.m33 = 1F;
    }

    // Cleanup all the objects we possibly have created
    void OnDisable()
    {
        DestroyReflectionCam();
    }

    public void DestroyReflectionCam()
    {
        //Clear texture
        if (m_ReflectionTexture)
        {
            DestroyImmediate(m_ReflectionTexture);
            m_ReflectionTexture = null;
        }

        //Clear cameras
        foreach (var kvp in m_ReflectionCameras)
        {
            DestroyImmediate((kvp.Value).gameObject);
        }
        m_ReflectionCameras.Clear();
    }
    #endregion

}//SWS class end


#if UNITY_EDITOR
internal class SubstanceBaker : ScriptableObject
{
    public static bool useCompression = true;
    SubstanceImporter si;
    private ProceduralMaterial substance;
    private Material material;
    private string targetFolder;

    public Texture2D shadermap;
    public Texture2D normalmap;

    /// <summary>
    /// Bake the Substance outputs to PNG textures in the target folder
    /// A seperate folder /Textures folder will be created next to the material
    /// </summary>
    public void BakeSubstance(ProceduralMaterial substance, Material material, bool useCompression = false)
    {
        this.substance = substance;
        this.material = material;
        SubstanceBaker.useCompression = useCompression;

        //Substance .sbsar container
        string substancePath = AssetDatabase.GetAssetPath(substance.GetInstanceID());
        si = AssetImporter.GetAtPath(substancePath) as SubstanceImporter;

        //Also generate shadermap
        si.SetGenerateAllOutputs(substance, true);

        //Make readable
        substance.isReadable = true;

        //Generate textures
        substance.RebuildTexturesImmediately();

        Texture[] substanceOutputs = substance.GetGeneratedTextures();

        foreach (Texture texture in substanceOutputs)
        {
            {
                ConvertProceduralTexture(texture);
            }
        }

        AssetDatabase.Refresh();


    }

    private void ConvertProceduralTexture(Texture sourceTex)
    {
        //Debug.Log("Converting " + sourceTex.name);

        ProceduralTexture sourceTexture = (Texture)sourceTex as ProceduralTexture;

        Color32[] pixels = sourceTexture.GetPixels32(0, 0, sourceTex.width, sourceTex.height);

        Texture2D destTex = new Texture2D(sourceTexture.width, sourceTexture.height)
        {
            //Copy options from substance texture
            name = sourceTexture.name,
            anisoLevel = sourceTexture.anisoLevel,
            filterMode = sourceTexture.filterMode,
            mipMapBias = sourceTexture.mipMapBias,
            wrapMode = sourceTexture.wrapMode
        };

        destTex.SetPixels32(pixels);

        //Convert normal map to Unity format
        if (sourceTex.name.Contains("_normal"))
        {
            Color targetColor = new Color();
            for (int x = 0; x < sourceTex.width; x++)
            {
                for (int y = 0; y < sourceTex.height; y++)
                {
                    //Red is stored in the alpha component
                    targetColor.r = destTex.GetPixel(x, y).a;
                    //Green channel, already inverted in Substance Designer
                    targetColor.g = destTex.GetPixel(x, y).g;
                    //Full blue channel
                    targetColor.b = 1;
                    //Full alpha
                    targetColor.a = 1;

                    destTex.SetPixel(x, y, targetColor);
                }
            }

        }

        destTex.Apply();

        string targetFolder = AssetDatabase.GetAssetPath(material);

        //Material root folder
        targetFolder = targetFolder.Replace(material.name + ".mat", string.Empty);

        //Create Textures folder if it doesn't exist
        if (!Directory.Exists(targetFolder + "Textures/"))
        {
            Debug.Log("Directory: " + targetFolder + "Textures/" + " doesn't exist, creating...");
            Directory.CreateDirectory(targetFolder + "Textures/");

            AssetDatabase.Refresh();
        }

        //Append textures folder
        targetFolder += "Textures/";

        //Remove Substance material name from texture name
        destTex.name = destTex.name.Replace(substance.name, string.Empty);

        //Compose file path
        string path = targetFolder + material.name + destTex.name + "_baked.png";

        File.WriteAllBytes(path, destTex.EncodeToPNG());

        AssetDatabase.Refresh();

        //Trigger SWSImporter
        AssetDatabase.ImportAsset(path, ImportAssetOptions.Default);

        //Grab a reference to the newly saved files
        if (sourceTex.name.Contains("_normal"))
        {
            normalmap = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
        }
        else if (sourceTex.name.Contains("_shadermap"))
        {
            shadermap = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));

        }

        //Debug.Log("Written file to: " + path);
    }
}

//Catch the normal map when it is being imported and flag it accordingly
internal sealed class SWSImporter : AssetPostprocessor
{

    TextureImporter textureImporter;

    private void OnPreprocessTexture()
    {
        textureImporter = assetImporter as TextureImporter;

        //Only run for SWS created textures, which have the _baked suffix
        if (!assetPath.Contains("_baked")) return;

        if (SubstanceBaker.useCompression)
        {
            textureImporter.textureType = TextureImporterType.Default;
#if UNITY_5_6_OR_NEWER
            textureImporter.textureCompression = TextureImporterCompression.CompressedHQ;
#else
            textureImporter.textureFormat = TextureImporterFormat.PVRTC_RGB2;
#endif
        }
        else
        {
#if UNITY_5_6_OR_NEWER
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
#else
            textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
#endif
        }

        //Look for the given name, this will also apply to textures outside of the SWS package, but it's unlikely this naming convention will occur
        if (assetPath.Contains("_normal_baked"))
        {
#if !UNITY_5_5_OR_NEWER
            textureImporter.normalmap = true;
#endif
            textureImporter.wrapMode = TextureWrapMode.Repeat;
            textureImporter.textureType = TextureImporterType.NormalMap;

        }
        else if (assetPath.Contains("_shadermap_baked"))
        {
#if !UNITY_5_5_OR_NEWER
            textureImporter.normalmap = false;
#endif
            textureImporter.alphaIsTransparency = false;
            textureImporter.wrapMode = TextureWrapMode.Repeat;
            textureImporter.textureType = TextureImporterType.Default;
        }

    }
}
#endif

//Easter egg, good job :)