using System;
using System.Collections.Generic;
using Modding;
using Localisation;
using UnityEngine;
using NLNL;

namespace NLNL
{
    class LaungageManager : MonoBehaviour
    {
        public static string CurrLangName = SingleInstance<LocalisationManager>.Instance.currLangName;
        public struct LightEntity
        {
            public static string Range
            {
                get
                {
                    switch (CurrLangName)
                    {
                        case "日本語":
                            return "光源サイズ";
                        case "简体中文":
                        case "台灣繁體中文":
                        case "香港繁體中文":
                            return "光源大小";
                        case "English":
                        default:
                            return "Range";
                    }
                }
            }
            public static string SpotAngle
            {
                get
                {
                    switch (CurrLangName)
                    {
                        case "日本語":
                            return "円錐角";
                        case "简体中文":
                        case "台灣繁體中文":
                        case "香港繁體中文":
                            return "锥体角度";
                        case "English":
                        default:
                            return "Spot Angle";
                    }
                }
            }
            public static string Color
            {
                get
                {
                    switch (CurrLangName)
                    {
                        case "日本語":
                            return "光源色";
                        case "简体中文":
                        case "台灣繁體中文":
                        case "香港繁體中文":
                            return "光源颜色";
                        case "English":
                        default:
                            return "Color";
                    }
                }
            }
            public static string Intensity
            {
                get
                {
                    switch (CurrLangName)
                    {
                        case "日本語":
                            return "強度";
                        case "简体中文":
                        case "台灣繁體中文":
                        case "香港繁體中文":
                            return "强度";
                        case "English":
                        default:
                            return "Intensity";
                    }
                }
            }
            public static string BounceIntensity
            {
                get
                {
                    switch (CurrLangName)
                    {
                        case "日本語":
                            return "反射強度";
                        case "简体中文":
                        case "台灣繁體中文":
                        case "香港繁體中文":
                            return "反射强度";
                        case "English":
                        default:
                            return "Bounce Intensity";
                    }
                }
            }
            public struct LightShafts
            {
                public static string useToggle
                {
                    get
                    {
                        switch (CurrLangName)
                        {
                            case "日本語":
                                return "ライトシャフト";
                            case "简体中文":
                            case "台灣繁體中文":
                            case "香港繁體中文":
                                return "使用光束";
                            case "English":
                            default:
                                return "Use Light Shafts";
                        }
                    }
                }
                public static string VolumeSize
                {
                    get
                    {
                        switch (CurrLangName)
                        {
                            case "日本語":
                                return "範囲設定";
                            case "简体中文":
                            case "台灣繁體中文":
                            case "香港繁體中文":
                                return "范围设置";
                            case "English":
                            default:
                                return "Volume Size";
                        }
                    }
                }
                public static string LightShaftsSetting
                {
                    get
                    {
                        switch (CurrLangName)
                        {
                            case "日本語":
                                return "光線設定";
                            case "简体中文":
                            case "台灣繁體中文":
                            case "香港繁體中文":
                                return "光束设置";
                            case "English":
                            default:
                                return "Light Shafts Setting";
                        }
                    }
                }
                public static string LightSetting
                {
                    get
                    {
                        switch (CurrLangName)
                        {
                            case "日本語":
                                return "光源設定";
                            case "简体中文":
                            case "台灣繁體中文":
                            case "香港繁體中文":
                                return "光源设置";
                            case "English":
                            default:
                                return "Light Setting";
                        }
                    }
                }
                public static string VolumeStart
                {
                    get
                    {
                        switch (CurrLangName)
                        {
                            case "日本語":
                                return "始点";
                            case "简体中文":
                            case "台灣繁體中文":
                            case "香港繁體中文":
                                return "起点";
                            case "English":
                            default:
                                return "Start";
                        }
                    }
                }
                public static string VolumeEnd
                {
                    get
                    {
                        switch (CurrLangName)
                        {
                            case "日本語":
                                return "終点";
                            case "简体中文":
                            case "台灣繁體中文":
                            case "香港繁體中文":
                                return "终点";
                            case "English":
                            default:
                                return "End";
                        }
                    }
                }
                public static string Brightness
                {
                    get
                    {
                        switch (CurrLangName)
                        {
                            case "日本語":
                                return "光線強度";
                            case "简体中文":
                            case "台灣繁體中文":
                            case "香港繁體中文":
                                return "光束强度";
                            case "English":
                            default:
                                return "Brightness";
                        }
                    }
                }
                public static string Attenuation
                {
                    get
                    {
                        switch (CurrLangName)
                        {
                            case "日本語":
                                return "減衰";
                            case "简体中文":
                            case "台灣繁體中文":
                            case "香港繁體中文":
                                return "衰减";
                            case "English":
                            default:
                                return "Attenuation";
                        }
                    }
                }
                public static string AttenuationCurve
                {
                    get
                    {
                        switch (CurrLangName)
                        {
                            case "日本語":
                            //return "減衰";
                            case "简体中文":
                            case "台灣繁體中文":
                            case "香港繁體中文":
                            //return "衰减";
                            case "English":
                            default:
                                return "EaseInOut";
                        }
                    }
                }

            }

        }
    }
}
