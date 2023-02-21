using System;
using Modding;
using UnityEngine;
using NLNL;

namespace NLNL
{
    class WarningUI : MonoBehaviour
    {
        public readonly int windowId = ModUtility.GetWindowId();
        private Rect windowRect = new Rect(20, 100, 250, 90);
        private bool tabHide = false;
        private bool keyHide = true;
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
        }
        void OnGUI()
        {
            if(!StatMaster.isMainMenu && !StatMaster.inMenu && !tabHide && !keyHide)
            {
                windowRect = GUI.Window(windowId, windowRect, delegate (int windowId) {
                    GUI.Label(new Rect(25, 30, 200, 20), "Plase install UIFactory");
                    if (GUI.Button(new Rect(25, 60, 200, 20), "Open steamworkshop"))
                    {
                        Uri uri = new System.Uri("https://steamcommunity.com/sharedfiles/filedetails/?id=2913469777");
                        Application.OpenURL(uri.AbsoluteUri);
                    }
                    GUI.DragWindow();
                }, "Environment Setting[Alt+L]");
            }
        }
    }
}
