using System;
using Modding;
using Modding.Levels;
using UnityEngine;
using NLNL;

namespace NLNL
{
    public class Mod : ModEntryPoint
    {
        public static GameObject NLNLController;
        public static MessageType TranslateType;
        public static AssetBundle LightShaftsShadersAsset;
        public static readonly string Name = "NoLightNoLife";

        public override void OnLoad()
        {
            UnityEngine.Object.DontDestroyOnLoad(NLNLController = new GameObject("NLNLContoroller"));
            TranslateType = ModNetworking.CreateMessageType(DataType.Entity, DataType.Vector3, DataType.Vector3);
            ModNetworking.Callbacks[TranslateType] += (Action<Message>)delegate (Message msg)
            {
                Entity entity = (Entity)msg.GetData(0);
                entity = entity.SimEntity;
                Vector3 position = (Vector3)msg.GetData(1);
                Vector3 euler = (Vector3)msg.GetData(2);
                entity.GameObject.transform.position = position;
                entity.GameObject.transform.rotation = Quaternion.Euler(euler);
            };
            LoadAssetBundle();
        }

        public override void OnEntityPrefabCreation(int entityId, GameObject prefab)
        {
            switch (entityId)
            {
                case 1:
                    if (prefab.GetComponent<LightController>() == null)
                    {
                        prefab.AddComponent<LightController>();
                    }
                    break;
            }
        }

        private void LoadAssetBundle()
        {
            if (LightShaftsShadersAsset != null) return;
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                LightShaftsShadersAsset = ModResource.GetAssetBundle("LightShaftsShadersWin");
            }
            else
            {
                LightShaftsShadersAsset = ModResource.GetAssetBundle("LightShaftsShadersMac");
            }
        }
    }
}
