using InexperiencedDeveloper.Utils.Log;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InexperiencedDeveloper.Core.Settings
{
    public static class Options
    {

        #region Video Settings
        public static int Brightness
        {
            get
            {
                return PlayerPrefs.GetInt("brightness", 10);
            }
            set
            {
                PlayerPrefs.SetInt("brightness", value);
                ApplyBrightness();
            }
        }

        public static int Exposure
        {
            get
            {
                return PlayerPrefs.GetInt("exposure", 1);
            }
            set
            {
                PlayerPrefs.SetInt("exposure", value);
            }
        }

        public static int Bloom
        {
            get
            {
                return PlayerPrefs.GetInt("bloom", 1);
            }
            set
            {
                PlayerPrefs.SetInt("bloom", value);
            }
        }
        #endregion
        #region Audio Settings
        public static int AudioMaster
        {
            get
            {
                return PlayerPrefs.GetInt("audioMaster", 20);
            }
            set
            {
                PlayerPrefs.SetInt("audioMaster", value);
                ApplyAudioMaster();
            }
        }

        public static int AudioFX
        {
            get
            {
                return PlayerPrefs.GetInt("audioFX", 16);
            }
            set
            {
                PlayerPrefs.SetInt("audioFX", value);
                ApplyAudioFX();
            }
        }

        public static int AudioMusic
        {
            get
            {
                return PlayerPrefs.GetInt("audioMusic", 16);
            }
            set
            {
                PlayerPrefs.SetInt("audioMusic", value);
                ApplyAudioMusic();
            }
        }

        public static int AudioVoice
        {
            get
            {
                return PlayerPrefs.GetInt("audioVoice", 16);
            }
            set
            {
                PlayerPrefs.SetInt("audioVoice", value);
                ApplyAudioVoice();
            }
        }
        #endregion
        #region Camera Settings
        public static int CameraFOV
        {
            get
            {
                return PlayerPrefs.GetInt("cameraFOV", 5);
            }
            set
            {
                PlayerPrefs.SetInt("cameraFOV", value);
                ApplyCameraSettings();
            }
        }

        public static int CameraSmoothing
        {
            get
            {
                return PlayerPrefs.GetInt("cameraSmoothing", 10);
            }
            set
            {
                PlayerPrefs.SetInt("cameraSmoothing", value);
                ApplyCameraSettings();
            }
        }
        #endregion
        #region Controller Settings
        public static int ControllerLayout
        {
            get
            {
                return PlayerPrefs.GetInt("controllerLayout", 0);
            }
            set
            {
                PlayerPrefs.SetInt("controllerLayout", value);
                ApplyControllerSettings();
            }
        }

        public static int ControllerLookMode
        {
            get
            {
                return PlayerPrefs.GetInt("controllerLookMode", 0);
            }
            set
            {
                PlayerPrefs.SetInt("controllerLookMode", value);
                ApplyControllerSettings();
            }
        }

        public static int ControllerInvert
        {
            get
            {
                return PlayerPrefs.GetInt("controllerInvert", 0);
            }
            set
            {
                PlayerPrefs.SetInt("controllerInvert", value);
                ApplyControllerSettings();
            }
        }
        public static int ControllerInvertHor
        {
            get
            {
                return PlayerPrefs.GetInt("controllerInvertHor", 0);
            }
            set
            {
                PlayerPrefs.SetInt("controllerInvertHor", value);
                ApplyControllerSettings();
            }
        }
        #endregion
        #region Mouse Settings
        public static int MouseInvert
        {
            get
            {
                return PlayerPrefs.GetInt("mouseInvert", 0);
            }
            set
            {
                PlayerPrefs.SetInt("mouseInvert", value);
                ApplyMouseSettings();
            }
        }

        public static int MouseLookMode
        {
            get
            {
                return PlayerPrefs.GetInt("mouseLookMode", 0);
            }
            set
            {
                PlayerPrefs.SetInt("mouseLookMode", value);
                ApplyMouseSettings();
            }
        }

        public static float MouseSensitivity
        {
            get
            {
                return PlayerPrefs.GetFloat("mouseSensitivity", 0.3f);
            }
            set
            {
                PlayerPrefs.SetFloat("mouseSensitivity", value);
                Options.ApplyMouseSettings();
            }
        }
        #endregion

        public static void LoadOptions()
        {
            if (ControllerLookMode > 1)
                ControllerLookMode = 0;
            ApplyBrightness();
            ApplyCameraEffects();
            ApplyAudioMaster();
            ApplyAudioFX();
            ApplyAudioMusic();
            ApplyAudioVoice();
            ApplyMouseSettings();
            ApplyCameraSettings();
            ApplyControllerSettings();
        }

        private static void ApplyBrightness()
        {
            DebugLogger.LogWarning("TODO: Implement brightness adjustment");
        }

        private static void ApplyCameraEffects()
        {
            DebugLogger.LogWarning("TODO: Implement post process adjustment");
        }

        private static void ApplyAudioMaster()
        {
            DebugLogger.LogWarning("TODO: Implement master audio adjustment");
        }

        private static void ApplyAudioFX()
        {
            DebugLogger.LogWarning("TODO: Implement FX audio adjustment");
        }

        private static void ApplyAudioMusic()
        {
            DebugLogger.LogWarning("TODO: Implement music audio adjustment");
        }

        private static void ApplyAudioVoice()
        {
            DebugLogger.LogWarning("TODO: Implement voice audio adjustment");
        }

        private static void ApplyMouseSettings()
        {
            DebugLogger.LogWarning("TODO: Implement mouse settings");
        }

        //public static PlayerActions CreateInput(InputDevice inputDevice)
        //{
        //    PlayerActions playerActions = PlayerActions.CreateWithControllerBindings(ControllerLayout, ControllerInvert > 0, ControllerInvertHor > 0, Options.controllerHSensitivity, Options.controllerVSensitivity);
        //    playerActions.Device = inputDevice;
        //    return playerActions;
        //}

        private static void ApplyCameraSettings()
        {
            DebugLogger.LogWarning("TODO: Implement Camera settings");
        }

        private static void ApplyControllerSettings()
        {
            DebugLogger.LogWarning("TODO: Implement Camera settings");
        }
    }
}

