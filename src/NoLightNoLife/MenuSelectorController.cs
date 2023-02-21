using System;
using System.Collections.Generic;
using Modding;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using NLNL;
using Besiege.UI;
using Besiege.UI.Serialization;

namespace NLNL
{
    class MenuSelectorController : MonoBehaviour
    {
        enum MenuSelector
        {
            Null,
            MainLight,
            RenderSettings,
            Fog,
            AmplifyColor
        }
        private Project project;
        private MenuSelector selectedMenu = MenuSelector.Null;
        private List<GameObject> menuSelectorList;
        private GameObject MainLightSettings;
        private GameObject WIP;
        
        void Awake()
        {
            project = Mod.NLNLController.GetComponent<EnvironmentSettingUI>().GetProject();
            if (project == null) return;
            menuSelectorList = new List<GameObject>() {project["MainLightButton"].gameObject, project["RenderSettingsButton"].gameObject, project["FogButton"].gameObject, project["AmplifyColorButton"].gameObject };
            MainLightSettings = project["MainLightSettings"].gameObject;
            WIP = project["WIP"].gameObject;
            project["MainLightButton"].GetComponent<Button>().onClick.AddListener(MainLightButtonClicked);
            project["RenderSettingsButton"].GetComponent<Button>().onClick.AddListener(RenderSettingsButtonClicked);
            project["FogButton"].GetComponent<Button>().onClick.AddListener(FogButtonClicked);
            project["AmplifyColorButton"].GetComponent<Button>().onClick.AddListener(AmplifyColorButtonClicked);
            //SceneManager.activeSceneChanged += OnSceneChanged;
            //if (StatMaster.isMP)
            //{
            //    OnSceneChanged(SceneManager.GetActiveScene(), SceneManager.GetActiveScene());
            //}
        }

        void Start()
        {

        }

        private void OnSceneChanged(Scene arg0, Scene arg1)
        {

        }

        void MainLightButtonClicked()
        {
            selectedMenu = MenuSelector.MainLight;
            MainLightSettings.SetActive(true);
            WIP.SetActive(false);
            MenuSelectorUpdate();
        }

        void RenderSettingsButtonClicked()
        {
            selectedMenu = MenuSelector.RenderSettings;
            MainLightSettings.SetActive(false);
            WIP.SetActive(true);
            MenuSelectorUpdate();
        }

        void FogButtonClicked()
        {
            selectedMenu = MenuSelector.Fog;
            MainLightSettings.SetActive(false);
            WIP.SetActive(true);
            MenuSelectorUpdate();
        }

        void AmplifyColorButtonClicked()
        {
            selectedMenu = MenuSelector.AmplifyColor;
            MainLightSettings.SetActive(false);
            WIP.SetActive(true);
            MenuSelectorUpdate();
        }

        void MenuSelectorUpdate()
        {
            foreach (GameObject button in menuSelectorList)
            {
                button.transform.Find("Checkmark").gameObject.SetActive(button.name == selectedMenu.ToString() + "Button");
            }
        }
    }
}
