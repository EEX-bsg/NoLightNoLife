using System;
using System.Collections.Generic;
using System.Globalization;
using Modding;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using NLNL;
using Besiege.UI;
using Besiege.UI.Serialization;

namespace NLNL
{
    class MainLightSettings : MonoBehaviour
    {
        public static Light MainLight;
        public static Vector3 Rotation
        {
            get
            {
                return (MainLight == null) ? Vector3.zero : MainLight.transform.eulerAngles;
            }
            set
            {
                UpdateRotation(value);
            }
        }
        public static Color Color
        {
            get
            {
                return (MainLight == null) ? Color.black : MainLight.color;
            }
            set
            {
                MainLight.color = value;
            }
        }
        public static float Alpha
        {
            get
            {
                return (MainLight == null) ? 0f : MainLight.color.r;
            }
            set
            {
                MainLight.color = new Color(MainLight.color.r, MainLight.color.g, MainLight.color.g, value);
            }
        }
        public static float Intensity
        {
            get
            {
                return (MainLight == null) ? 0f : MainLight.intensity;
            }
            set
            {
                MainLight.intensity = value;
            }
        }
        public static float BounceIntensity
        {
            get
            {
                return (MainLight == null) ? 0f : MainLight.bounceIntensity;
            }
            set
            {
                MainLight.bounceIntensity = value;
            }
        }
        private Project project;
        private GameObject parent;

        void Awake()
        {
            SceneManager.activeSceneChanged += OnSceneChanged;
            if (StatMaster.isMP)
            {
                OnSceneChanged(SceneManager.GetActiveScene(), SceneManager.GetActiveScene());
            }
        }

        void Start()
        {
            project = Mod.NLNLController.GetComponent<EnvironmentSettingUI>().GetProject();
            parent = project["MainLightSettings"].gameObject;

            Dropdown flareDropdown = parent.transform.FindChild("Flare/FlareDropdown").GetComponent<Dropdown>();
            flareDropdown.ClearOptions();
            flareDropdown.AddOptions(new List<string> { "Work In Progress" });

            project["Elevation"].transform.FindChild("InputField").GetComponent<InputField>().characterValidation = InputField.CharacterValidation.Decimal;
            project["Azimuth"].transform.FindChild("InputField").GetComponent<InputField>().characterValidation = InputField.CharacterValidation.Decimal;
            project["Intensity"].transform.FindChild("InputField").GetComponent<InputField>().characterValidation = InputField.CharacterValidation.Decimal;
            project["BounceIntensity"].transform.FindChild("InputField").GetComponent<InputField>().characterValidation = InputField.CharacterValidation.Decimal;
            project["MainLightAlpha"].transform.FindChild("InputField").GetComponent<InputField>().characterValidation = InputField.CharacterValidation.Decimal;

            project["Elevation"].transform.FindChild("Slider").GetComponent<UnityEngine.UI.Slider>().onValueChanged.AddListener(ElevationChanged);
            project["Elevation"].transform.FindChild("InputField").GetComponent<InputField>().onValueChanged.AddListener(ElevationChanged);
            project["Azimuth"].transform.FindChild("Slider").GetComponent<UnityEngine.UI.Slider>().onValueChanged.AddListener(AzimuthChanged);
            project["Azimuth"].transform.FindChild("InputField").GetComponent<InputField>().onValueChanged.AddListener(AzimuthChanged);
            project["Intensity"].transform.FindChild("Slider").GetComponent<UnityEngine.UI.Slider>().onValueChanged.AddListener(IntensityChanged);
            project["Intensity"].transform.FindChild("InputField").GetComponent<InputField>().onValueChanged.AddListener(IntensityChanged);
            project["BounceIntensity"].transform.FindChild("Slider").GetComponent<UnityEngine.UI.Slider>().onValueChanged.AddListener(BounceIntensityChanged);
            project["BounceIntensity"].transform.FindChild("InputField").GetComponent<InputField>().onValueChanged.AddListener(BounceIntensityChanged);
            project["MainLightColor"].transform.FindChild("Content/Red/InputField").GetComponent<InputField>().onEndEdit.AddListener((value) => { ColorChanged(value, "Red"); });
            project["MainLightColor"].transform.FindChild("Content/Blue/InputField").GetComponent<InputField>().onEndEdit.AddListener((value) => { ColorChanged(value, "Blue"); });
            project["MainLightColor"].transform.FindChild("Content/Green/InputField").GetComponent<InputField>().onEndEdit.AddListener((value) => { ColorChanged(value, "Green"); });
            project["MainLightAlpha"].transform.FindChild("Slider").GetComponent<UnityEngine.UI.Slider>().onValueChanged.AddListener(AlphaChanged);
            project["MainLightAlpha"].transform.FindChild("InputField").GetComponent<InputField>().onValueChanged.AddListener(AlphaChanged);

            GetMainLight();
            ElevationChanged(Rotation.x);
            AzimuthChanged(Rotation.y);
            IntensityChanged(Intensity);
            BounceIntensityChanged(BounceIntensity);
            ColorChanged(Color.r.ToString(), "Red");
            AlphaChanged(Alpha);
        }
        private void OnSceneChanged(Scene arg0, Scene arg1)
        {
            GetMainLight();
        }

        private void GetMainLight()
        {
            if (StatMaster.isMP)
            {
                MainLight = GameObject.Find("Environments").transform.FindChild("Directional light").GetComponent<Light>();
            }
            else
            {
                GameObject obj = GameObject.Find("Directional light");
                MainLight = (obj != null) ? obj.GetComponent<Light>() : null;
            }
        }

        private void ElevationChanged(string value)
        {
            if (value == null || value == "")
            {
                value = "0";
            }
            if (value == "0.") return;
            try
            {
                ElevationChanged(float.Parse(value, CultureInfo.InvariantCulture.NumberFormat));
            }
            catch
            {
                return;
            }
        }
        private void ElevationChanged(float value)
        {
            if(value < 0f)
            {
                value = 0f;
            }
            else if(value >= 90f)
            {
                value = 89.99999f;
            }
            GameObject container = project["Elevation"].gameObject;
            container.transform.FindChild("Slider").GetComponent<UnityEngine.UI.Slider>().value = value;
            container.transform.FindChild("InputField").GetComponent<InputField>().text = value.ToString();
            Rotation = new Vector3(value, Rotation.y, Rotation.z);
        }
        private void AzimuthChanged(string value)
        {
            if (value == null || value == "")
            {
                value = "0";
            }
            if (value == "0.") return;
            try
            {
                AzimuthChanged(float.Parse(value, CultureInfo.InvariantCulture.NumberFormat));
            }
            catch
            {
                return;
            }
        }
        private void AzimuthChanged(float value)
        {
            if (value < 0f)
            {
                value = 0f;
            }
            else if (value >= 360f)
            {
                value = 0f;
            }
            GameObject container = project["Azimuth"].gameObject;
            container.transform.FindChild("Slider").GetComponent<UnityEngine.UI.Slider>().value = value;
            container.transform.FindChild("InputField").GetComponent<InputField>().text = value.ToString();
            Rotation = new Vector3(Rotation.x, value, Rotation.z);
        }
        private void IntensityChanged(string value)
        {
            if (value == null || value == "")
            {
                value = "0";
            }
            if (value == "0.") return;
            try
            {
                IntensityChanged(float.Parse(value, CultureInfo.InvariantCulture.NumberFormat));
            }
            catch
            {
                return;
            }
        }
        private void IntensityChanged(float value)
        {
            if (value < 0f)
            {
                value = 0f;
            }
            GameObject container = project["Intensity"].gameObject;
            container.transform.FindChild("Slider").GetComponent<UnityEngine.UI.Slider>().value = value;
            container.transform.FindChild("InputField").GetComponent<InputField>().text = value.ToString();
            Intensity = value;
        }
        private void BounceIntensityChanged(string value)
        {
            if (value == null || value == "")
            {
                value = "0";
            }
            if (value == "0.") return;
            try
            {
                BounceIntensityChanged(float.Parse(value, CultureInfo.InvariantCulture.NumberFormat));
            }
            catch
            {
                return;
            }
        }
        private void BounceIntensityChanged(float value)
        {
            if (value < 0f)
            {
                value = 0f;
            }
            GameObject container = project["BounceIntensity"].gameObject;
            container.transform.FindChild("Slider").GetComponent<UnityEngine.UI.Slider>().value = value;
            container.transform.FindChild("InputField").GetComponent<InputField>().text = value.ToString();
            BounceIntensity = value;
        }
        private void ColorChanged(string value, string color){
            //if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[-+]?[0-9]+[.]?[0-9]*")) return;
            if (value == null || value == "")
            {
                value = "0";
            }
            float val;
            try
            {
                val = float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
            }
            catch
            {
                return;
            }
            if(val < 0)
            {
                val = 0;
            }
            else if(val > 1)
            {
                val = 1;
            }
            switch (color)
            {
                case "Red":
                    Color = new Color(val, Color.g, Color.b);
                    break;
                case "Green":
                    Color = new Color(Color.r, val, Color.b);
                    break;
                case "Blue":
                    Color = new Color(Color.r, Color.g, val);
                    break;
            }
            project["MainLightColor"].transform.FindChild("Content/Red/InputField").GetComponent<InputField>().text = Color.r.ToString();
            project["MainLightColor"].transform.FindChild("Content/Green/InputField").GetComponent<InputField>().text = Color.g.ToString();
            project["MainLightColor"].transform.FindChild("Content/Blue/InputField").GetComponent<InputField>().text = Color.b.ToString();
        }
        private void AlphaChanged(string value)
        {
            if (value == null || value == "")
            {
                value = "0";
            }
            if (value == "0.") return;
            try
            {
                AlphaChanged(float.Parse(value, CultureInfo.InvariantCulture.NumberFormat));
            }
            catch
            {
                return;
            }
        }
        private void AlphaChanged(float value)
        {
            if(value < 0)
            {
                value = 0;
            }
            else if(value > 1)
            {
                value = 1;
            }
            project["MainLightAlpha"].transform.FindChild("Slider").GetComponent<UnityEngine.UI.Slider>().value = value;
            project["MainLightAlpha"].transform.FindChild("InputField").GetComponent<InputField>().text = value.ToString();
            Alpha = value;
        }

        private static void UpdateRotation(Vector3 value)
        {
            if (MainLight == null) return;
            MainLight.transform.rotation = Quaternion.identity;
            MainLight.transform.RotateAround(Vector3.zero, Vector3.right, value.x);
            MainLight.transform.RotateAround(Vector3.zero, Vector3.up, value.y);
        }
    }
}
