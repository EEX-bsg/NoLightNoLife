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
    class EnvironmentSettingUI : MonoBehaviour
    {
        public GameObject UI;
        private Project EnviromentSettingUI;
        private Project project;
        private bool tabHide = false;
        private bool keyHide = true;
        private bool finishSetUp = false;
        private Texture2D wipIcon;
        void Awake()
        {
            Make.RegisterSerialisationProvider(Mod.Name, new Besiege.UI.Serialization.SerializationProvider
            {
                CreateText = p => Modding.ModIO.CreateText(p, false),
                ReadAllText = p => Modding.ModIO.ReadAllText(p, false),
                GetFiles = p => Modding.ModIO.GetFiles(p, false),
            });
            wipIcon = ModResource.GetTexture("WIPsign");
            //EnviromentSettingUI = Make.LoadProject(Mod.Name, "EnvironmentSettingUI", Mod.NLNLController.transform);
            //EnviromentSettingUI.gameObject.SetActive(false);
            SceneManager.activeSceneChanged += OnSceneChanged;
            if (StatMaster.isMP)
            {
                OnSceneChanged(SceneManager.GetActiveScene(), SceneManager.GetActiveScene());
            }
        }

        private void OnSceneChanged(Scene arg0, Scene arg1)
        {
            if((arg0.buildIndex >=1 && arg0.buildIndex <= 5) || (arg0.buildIndex == 9 && StatMaster.inMenu) || StatMaster.isMainMenu)
            {
                return;
            }
            tabHide = false;
            keyHide = true;
            //Debug.Log("Scene");
            //project = Instantiate(EnviromentSettingUI);
            //project.RebuildTransformList();
            project = Make.LoadProject(Mod.Name, "EnvironmentSettingUI");
            UI = project.gameObject;
            //SceneManager.MoveGameObjectToScene(UI, SceneManager.GetActiveScene());
            //project.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            Sprite sprite = Sprite.Create(wipIcon, new Rect(0f, 0f, wipIcon.width, wipIcon.height), new Vector2(wipIcon.width / 2, wipIcon.height / 2), 100f);
            project["WIPIcon"].GetComponent<Image>().sprite = sprite;
            project["MenuSelector"].gameObject.AddComponent<MenuSelectorController>();
            project["MainLightSettings"].gameObject.AddComponent<MainLightSettings>();
            UI.SetActive(false);
        }

        void Start()
        {
            
        }

        void Update()
        {
            if (ModKeys.GetKey("UIKey").IsPressed)
            {
                keyHide = !keyHide;
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                tabHide = !tabHide;
            }
            UI.SetActive(!StatMaster.isMainMenu && !StatMaster.inMenu && !tabHide && !keyHide);
        }

        public Project GetProject()
        {
            return project;
        }
    }
}
