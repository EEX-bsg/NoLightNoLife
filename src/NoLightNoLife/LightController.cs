using System;
using System.Collections.Generic;
using Modding;
using Modding.Levels;
using UnityEngine;
using NLNL;
using NLNL.SunShafts;

namespace NLNL
{
    class LightController : MonoBehaviour
    {
        public LevelEntity LevelEntity;
        public GenericEntity GenericEntity;
        public Light LightComponent;
        public LightShafts LightShafts;
        public static readonly List<string> LightTypesList = new List<string>() { "Point", "Spot", "Directional" };
        public static readonly List<string> ShadowTypeList = new List<string>() { "No Shadow", "Soft Shadows", "Hard Shadows" };
        public static readonly List<string> LightShaftsMenuList = new List<string>() { LaungageManager.LightEntity.LightShafts.LightSetting, LaungageManager.LightEntity.LightShafts.VolumeSize, LaungageManager.LightEntity.LightShafts.LightShaftsSetting };
        public MMenu Type;
        public MValue Range;
        public MSlider SpotAngle;
        public MColourSlider LightColor;
        public MSlider Intensity;
        public MSlider BounceIntensity;
        public MMenu ShadowType;
        public MToggle UseLightShafts;
        public MMenu LightShaftsMenu;
        public MValue VolumeSizeX;
        public MValue VolumeSizeY;
        public MValue VolumeSizeZ;
        public MValue VolumeStart;
        public MValue VolumeEnd;
        public MSlider Brightness;
        public MSlider Attenuation;
        //public MSlider AttenuationCurve;
        private GameObject lightVisual;
        private GameObject selectVisual;
        private GameObject gridSphere;
        private GameObject gridCone;
        private GameObject upright;
        private bool haveVisual = false;
        private int sendMessageCycle = 50;
        private int sendMessageTime;
        public int SendMessageTime
        {
            get
            {
                return sendMessageTime;
            }
            set
            {
                if (value < 0)
                {
                    value = sendMessageCycle;
                }
                if (sendMessageCycle < value)
                {
                    value = 0;
                }
                sendMessageTime = value;
            }
        }
        public bool InSimulation => LevelEntity.isSimulating || (LevelEntity.isStatic && StatMaster.levelSimulating) || Game.IsSimulatingGlobal || (StatMaster.isClient && StatMaster.isLocalSim);

        void Awake()
        {
            LevelEntity = GetComponent<LevelEntity>();
            GenericEntity = GetComponent<GenericEntity>();
            LightComponent = gameObject.AddComponent<Light>();
            LightComponent.renderMode = LightRenderMode.ForcePixel;
            LightShafts = gameObject.AddComponent<LightShafts>();
            AssetBundle assetBundle = Mod.LightShaftsShadersAsset;
            String shaderPath = "Assets/LightShafts-master/";
            LightShafts.m_DepthShader = assetBundle.LoadAsset<Shader>(shaderPath + "Depth.shader");
            LightShafts.m_ColorFilterShader = assetBundle.LoadAsset<Shader>(shaderPath + "ColorFilter.shader");
            LightShafts.m_CoordShader = assetBundle.LoadAsset<Shader>(shaderPath + "Coord.shader");
            LightShafts.m_DepthBreaksShader = assetBundle.LoadAsset<Shader>(shaderPath + "DepthBreaks.shader");
            LightShafts.m_RaymarchShader = assetBundle.LoadAsset<Shader>(shaderPath + "Raymarch.shader");
            LightShafts.m_InterpolateAlongRaysShader = assetBundle.LoadAsset<Shader>(shaderPath + "InterpolateAlongRays.shader");
            LightShafts.m_FinalInterpolationShader = assetBundle.LoadAsset<Shader>(shaderPath + "FinalInterpolation.shader");
            LightShafts.m_SamplePositionsShader = assetBundle.LoadAsset<Shader>(shaderPath + "SamplePositions.shader");
            LightShafts.enabled = false;
            GenerateGUI();
        }
        void Start()
        {
            SetVisual();
            switch (Type.Value)
            {
                case 0:
                    LightComponent.type = LightType.Point;
                    break;
                case 1:
                    LightComponent.type = LightType.Spot;
                    break;
                case 2:
                    LightComponent.type = LightType.Directional;
                    break;
            }
            LightComponent.range = Range.Value;
            LightComponent.spotAngle = SpotAngle.Value;
            LightComponent.color = LightColor.Value;
            LightComponent.intensity = Intensity.Value;
            LightComponent.bounceIntensity = BounceIntensity.Value;
            switch (ShadowType.Value)
            {
                case 0:
                    LightComponent.shadows = LightShadows.None;
                    break;
                case 1:
                    LightComponent.shadows = LightShadows.Soft;
                    break;
                case 2:
                    LightComponent.shadows = LightShadows.Hard;
                    break;
            }

            Type.ValueChanged += Type_ValueChanged;
            Range.ValueChanged += Range_ValueChanged;
            SpotAngle.ValueChanged += SpotAngle_ValueChanged;
            LightColor.ValueChanged += LightColor_ValueChanged;
            Intensity.ValueChanged += Intensity_ValueChanged;
            BounceIntensity.ValueChanged += BounceIntensity_ValueChanged;
            ShadowType.ValueChanged += ShadowType_ValueChanged;
            UseLightShafts.Toggled += UseLightShafts_Toggled;
            LightShaftsMenu.ValueChanged += LightShaftsMenu_ValueChanged;
            VolumeSizeX.ValueChanged += VolumeSize_ValueChanged;
            VolumeSizeY.ValueChanged += VolumeSize_ValueChanged;
            VolumeSizeZ.ValueChanged += VolumeSize_ValueChanged;
            VolumeStart.ValueChanged += VolumeStart_ValueChanged;
            VolumeEnd.ValueChanged += VolumeEnd_ValueChanged;
            Brightness.ValueChanged += Brightness_ValueChanged;
            Attenuation.ValueChanged += Attenuation_ValueChanged;
            // AttenuationCurve.ValueChanged += AttenuationCurve_ValueChange;
            Type_ValueChanged(0);
            Range_ValueChanged(0);
        }

        void Update()
        {
            if (haveVisual)
            {
                SpriteRenderer renderer = lightVisual.GetComponent<SpriteRenderer>();
                SphereCollider collider = base.transform.GetComponentInChildren<SphereCollider>();
                if (InSimulation)
                {
                    renderer.enabled = false;
                    collider.enabled = false;
                }
                else
                {
                    renderer.enabled = true;
                    collider.enabled = true;
                }
            }
            transform.localScale = new Vector3(1, 1, 1);
            if (LevelEntity.IsSelected && !InSimulation)
            {
                selectVisual.SetActive(true);
            }
            else
            {
                selectVisual.SetActive(false);
            }
        }


        void FixedUpdate()
        {
            SendMessageTime++;
            if (InSimulation && !StatMaster.isClient && SendMessageTime == 0)
            {
                ModNetworking.SendToAll(Mod.TranslateType.CreateMessage(Entity.From(base.gameObject), base.transform.position, base.transform.rotation.eulerAngles));
            }
        }

        void OnDestroy()
        {
            Destroy(lightVisual);
            lightVisual = null;
        }

        private void GenerateGUI()
        {
            Type = GenericEntity.AddMenu("NL2-type", 0, LightTypesList, false);
            Range = GenericEntity.AddValue(LaungageManager.LightEntity.Range, "NL2-range", 10.0f);
            SpotAngle = GenericEntity.AddSlider(LaungageManager.LightEntity.SpotAngle, "NL2-spot-angle", 30.0f, 1f, 180f, "", "°");
            LightColor = GenericEntity.AddColourSlider(LaungageManager.LightEntity.Color, "NL2-color", new Color(1.0f, 1.0f, 1.0f), false);
            Intensity = GenericEntity.AddSlider(LaungageManager.LightEntity.Intensity, "NL2-intensity", 1, 0, 8, "", "");
            BounceIntensity = GenericEntity.AddSlider(LaungageManager.LightEntity.BounceIntensity, "NL2-bounce-intensity", 1.0f, 0f, 8f, "", "");
            ShadowType = GenericEntity.AddMenu("NL2-shadow-type", 0, ShadowTypeList, false);
            UseLightShafts = GenericEntity.AddToggle(LaungageManager.LightEntity.LightShafts.useToggle, "NL2-use-lightshafts", false);
            LightShaftsMenu = GenericEntity.AddMenu("NL2-LS-setting", 0, LightShaftsMenuList, true);
            VolumeSizeX = GenericEntity.AddValue("x", "NL2-LS-volume-x", 10);
            VolumeSizeY = GenericEntity.AddValue("y", "NL2-LS-volume-y", 10);
            VolumeSizeZ = GenericEntity.AddValue("z", "NL2-LS-volume-z", 20);
            VolumeStart = GenericEntity.AddValue(LaungageManager.LightEntity.LightShafts.VolumeStart, "NL2-LS-volume-start", 0.05f);
            VolumeEnd = GenericEntity.AddValue(LaungageManager.LightEntity.LightShafts.VolumeEnd, "NL2-LS-volume-end", 1.0f);
            Brightness = GenericEntity.AddSlider(LaungageManager.LightEntity.LightShafts.Brightness, "NL2-LS-brightness", 3.0f, 0f, 20f, "", "");
            Attenuation = GenericEntity.AddSlider(LaungageManager.LightEntity.LightShafts.Attenuation, "NL2-LS-attenuation", 0.5f, 0f, 20f, "", "");
            // AttenuationCurve = GenericEntity.AddSlider(LaungageManager.LightEntity.LightShafts.AttenuationCurve, "NL2-LS-attenuation-curve", 0, 0, 1, "", "");
        }

        private void Type_ValueChanged(int value)
        {
            ChangeVisual();
        }
        private void Range_ValueChanged(float value)
        {
            LightComponent.range = Range.Value;
            gridSphere.transform.localScale = new Vector3(Range.Value, Range.Value, Range.Value);
            float val = Range.Value / 5 * (float)Math.Tan(SpotAngle.Value * Math.PI / 180d / 2d) * 2f;
            gridCone.transform.localScale = new Vector3(val, val, Range.Value / 5f);
            gridCone.transform.localPosition = new Vector3(0f, 0f, Range.Value / 5f);
        }
        private void SpotAngle_ValueChanged(float value)
        {
            LightComponent.spotAngle = SpotAngle.Value;
            float val = Range.Value / 5 * (float)Math.Tan(SpotAngle.Value * Math.PI / 180d / 2d) * 2f;
            gridCone.transform.localScale = new Vector3(val, val, Range.Value / 5);
        }
        private void LightColor_ValueChanged(Color color)
        {
            LightComponent.color = LightColor.Value;
        }
        private void Intensity_ValueChanged(float value)
        {
            LightComponent.intensity = Intensity.Value;
        }
        private void BounceIntensity_ValueChanged(float value)
        {
            LightComponent.bounceIntensity = BounceIntensity.Value;
        }
        private void ShadowType_ValueChanged(int value)
        {
            switch (ShadowType.Value)
            {
                case 0:
                    LightComponent.shadows = LightShadows.None;
                    break;
                case 1:
                    LightComponent.shadows = LightShadows.Soft;
                    break;
                case 2:
                    LightComponent.shadows = LightShadows.Hard;
                    break;
            }
        }
        private void UseLightShafts_Toggled(bool flag)
        {
            ChangeVisual();
        }
        private void LightShaftsMenu_ValueChanged(int value)
        {
            ChangeVisual();
        }
        private void VolumeSize_ValueChanged(float value)
        {
            LightShafts.m_Size = new Vector3(VolumeSizeX.Value, VolumeSizeY.Value, VolumeSizeZ.Value);
        }
        private void VolumeStart_ValueChanged(float value)
        {
            LightShafts.m_SpotNear = VolumeStart.Value;
        }
        private void VolumeEnd_ValueChanged(float value)
        {
            LightShafts.m_SpotFar = VolumeEnd.Value;
        }
        private void Brightness_ValueChanged(float value)
        {
            LightShafts.m_Brightness = Brightness.Value;
        }
        private void Attenuation_ValueChanged(float value)
        {
            LightShafts.m_Extinction = Attenuation.Value;
            LightShafts.UpdateLUTs();
            /*
            if(AttenuationCurve.Value < 0.5f)
            {
                LightShafts.m_Extinction = Attenuation.Value;
                LightShafts.UpdateLUTs();
            }*/
        }
        // private void AttenuationCurve_ValueChange(float value)
        // {
        //     value = AttenuationCurve.Value;
        //     if(value < 0.5)
        //     {
        //         LightShafts.m_AttenuationCurveOn = true;
        //         Attenuation.SetRange(0f, 20f);
        //         AttenuationCurve.Value = 0f;
        //     }
        //     else
        //     {
        //         LightShafts.m_AttenuationCurveOn = false;
        //         Attenuation.SetRange(0f, 1f);
        //         AttenuationCurve.Value = 1f;
        //         float val = Attenuation.Value;
        //         if(val > 1)
        //         {
        //             val = 1f;
        //         }
        //         LightShafts.m_AttenuationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, val);
        //     }
        //     LightShafts.UpdateLUTs();
        // }

        private void ChangeVisual()
        {
            int type = Type.Value;
            switch (type)
            {
                case 0:
                    upright.SetActive(false);
                    gridSphere.SetActive(true);
                    gridCone.SetActive(false);
                    LightComponent.type = LightType.Point;
                    LightShafts.enabled = false;
                    DisplayLightSetting(type);
                    return;
                case 1:
                    upright.SetActive(false);
                    gridSphere.SetActive(false);
                    gridCone.SetActive(true);
                    LightComponent.type = LightType.Spot;
                    LightShafts.enabled = UseLightShafts.IsActive;
                    break;
                case 2:
                    upright.SetActive(true);
                    gridSphere.SetActive(false);
                    gridCone.SetActive(false);
                    LightComponent.type = LightType.Directional;
                    LightShafts.enabled = UseLightShafts.IsActive;
                    break;
            }
            if (!UseLightShafts.IsActive)
            {
                DisplayLightSetting(type);
            }
            switch (LightShaftsMenu.Value)
            {
                case 0:
                    DisplayLightSetting(type);
                    break;
                case 1:
                    DisplayVolumeSizeSetting(type);
                    break;
                case 2:
                    DisplayLightShaftsSetting(type);
                    break;
            }
        }
        private void DisplayLightSetting(int type)
        {
            switch (type)
            {
                case 0:
                    Range.DisplayInMapper = true;
                    SpotAngle.DisplayInMapper = false;
                    UseLightShafts.DisplayInMapper = false;
                    LightShaftsMenu.DisplayInMapper = false;
                    break;
                case 1:
                    Range.DisplayInMapper = true;
                    SpotAngle.DisplayInMapper = true;
                    UseLightShafts.DisplayInMapper = true;
                    LightShaftsMenu.DisplayInMapper = UseLightShafts.IsActive;
                    break;
                case 2:
                    Range.DisplayInMapper = false;
                    SpotAngle.DisplayInMapper = false;
                    UseLightShafts.DisplayInMapper = true;
                    LightShaftsMenu.DisplayInMapper = UseLightShafts.IsActive;
                    break;
            }
            ShadowType.DisplayInMapper = true;
            LightColor.DisplayInMapper = true;
            Intensity.DisplayInMapper = true;
            BounceIntensity.DisplayInMapper = true;
            ShadowType.DisplayInMapper = true;
            VolumeSizeX.DisplayInMapper = false;
            VolumeSizeY.DisplayInMapper = false;
            VolumeSizeZ.DisplayInMapper = false;
            VolumeStart.DisplayInMapper = false;
            VolumeEnd.DisplayInMapper = false;
            Brightness.DisplayInMapper = false;
            Attenuation.DisplayInMapper = false;
            // AttenuationCurve.DisplayInMapper = false;
        }
        private void DisplayVolumeSizeSetting(int type)
        {
            switch (type)
            {
                case 0:
                    UseLightShafts.SetValue(false);
                    break;
                case 1:
                    VolumeSizeX.DisplayInMapper = false;
                    VolumeSizeY.DisplayInMapper = false;
                    VolumeSizeZ.DisplayInMapper = false;
                    VolumeStart.DisplayInMapper = true;
                    VolumeEnd.DisplayInMapper = true;
                    break;
                case 2:
                    VolumeSizeX.DisplayInMapper = true;
                    VolumeSizeY.DisplayInMapper = true;
                    VolumeSizeZ.DisplayInMapper = true;
                    VolumeStart.DisplayInMapper = false;
                    VolumeEnd.DisplayInMapper = false;
                    break;
            }
            ShadowType.DisplayInMapper = false;
            Range.DisplayInMapper = false;
            SpotAngle.DisplayInMapper = false;
            LightColor.DisplayInMapper = false;
            Intensity.DisplayInMapper = false;
            BounceIntensity.DisplayInMapper = false;
            ShadowType.DisplayInMapper = false;
            UseLightShafts.DisplayInMapper = true;
            LightShaftsMenu.DisplayInMapper = true;
            Brightness.DisplayInMapper = false;
            Attenuation.DisplayInMapper = false;
            // AttenuationCurve.DisplayInMapper = false;
        }
        private void DisplayLightShaftsSetting(int type)
        {
            ShadowType.DisplayInMapper = false;
            Range.DisplayInMapper = false;
            SpotAngle.DisplayInMapper = false;
            LightColor.DisplayInMapper = false;
            Intensity.DisplayInMapper = false;
            BounceIntensity.DisplayInMapper = false;
            ShadowType.DisplayInMapper = false;
            UseLightShafts.DisplayInMapper = true;
            LightShaftsMenu.DisplayInMapper = true;
            VolumeSizeX.DisplayInMapper = false;
            VolumeSizeY.DisplayInMapper = false;
            VolumeSizeZ.DisplayInMapper = false;
            VolumeStart.DisplayInMapper = false;
            VolumeEnd.DisplayInMapper = false;
            Brightness.DisplayInMapper = true;
            Attenuation.DisplayInMapper = true;
            // AttenuationCurve.DisplayInMapper = true;
        }


        private void SetVisual()
        {
            if (haveVisual)
            {
                return;
            }
            lightVisual = new GameObject("LightVisual");
            lightVisual.AddComponent<LightVisController>();
            SpriteRenderer spriteRender = lightVisual.AddComponent<SpriteRenderer>();
            Texture2D texture = ModResource.GetTexture("lightbulb_tex");
            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
            spriteRender.sprite = sprite;
            lightVisual.transform.position = transform.position;
            lightVisual.transform.SetParent(transform);

            GameObject windHideOnPlay = GameObject.Find("_PERSISTENT/OBJECTS/Prefabs/Virtual/Wind/Obj/HideOnPlay");
            selectVisual = Instantiate(windHideOnPlay);
            selectVisual.transform.SetParent(transform);
            selectVisual.transform.position = transform.position;
            selectVisual.transform.rotation = transform.rotation;
            selectVisual.name = "DisplayOnSelect";
            Destroy(selectVisual.transform.Find("SphereCollider").gameObject);
            Destroy(selectVisual.transform.Find("Upright/Glow").gameObject);
            Destroy(selectVisual.transform.Find("Upright/Wind").gameObject);
            gridSphere = selectVisual.transform.Find("Sphere").gameObject;
            upright = selectVisual.transform.Find("Upright").gameObject;
            upright.transform.localScale = new Vector3(50f, 50f, 50f);
            gridSphere.transform.localScale = new Vector3(Range.Value, Range.Value, Range.Value);

            gridCone = new GameObject("Cone");
            gridCone.transform.position = selectVisual.transform.position;
            gridCone.transform.rotation = transform.rotation;
            gridCone.transform.SetParent(selectVisual.transform);
            gridCone.transform.localPosition = new Vector3(0f, 0f, 1f);
            gridCone.transform.Rotate(new Vector3(0f, 180f, 0f));
            MeshFilter meshFilter = gridCone.AddComponent<MeshFilter>();
            meshFilter.mesh = GameObject.Find("_PERSISTENT/OBJECTS/Prefabs/Primitives/Cone/Cone").GetComponent<MeshFilter>().mesh;
            MeshRenderer meshRenderer = gridCone.AddComponent<MeshRenderer>();
            meshRenderer.material = gridSphere.GetComponent<MeshRenderer>().material;
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;
            upright.SetActive(false);
            gridSphere.SetActive(false);
            gridCone.SetActive(false);

            haveVisual = true;
        }
    }
}