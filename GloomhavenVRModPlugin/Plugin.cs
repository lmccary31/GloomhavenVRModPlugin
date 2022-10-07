using BepInEx;
using UnityEngine;
using UnityEngine.XR.Management;
using Unity.XR.OpenVR;
using Valve.VR;
using UnityEngine.XR;
using System.Dynamic;
using Valve.VR.Extras;
using UnityEngine.PlayerLoop;
using System;
using UnityEngine.XR.OpenXR.Features.OculusQuestSupport;
using System.Reflection;
using UnityEngine.SceneManagement;
using System.ComponentModel;
using UnityEngine.Rendering;
using UnityEngine.Assertions.Must;
using System.Drawing.Drawing2D;

namespace GloomhavenVRModPlugin
{
    [BepInPlugin("com.McCary.GloomhavenVRMod", "GloomhavenVRMod", "1.0.0")]
    public class Plugin : BaseUnityPlugin

    {

        private GameObject cameraParent;
        private bool vrStarting = false;

        public static void PrintCamera(Camera c)
        {
            if (c == null)
            {
                Debug.Log("Null camera, cannot print properties!");
                return;
            }
            Debug.Log("Camera: " + c.name + " Enabled: " + c.enabled);
            Debug.Log("  activeTexture: " + c.activeTexture);
            Debug.Log("  actualRenderingPath: " + c.actualRenderingPath);
            Debug.Log("  allowDynamicResolution: " + c.allowDynamicResolution);
            Debug.Log("  allowHDR: " + c.allowHDR);
            Debug.Log("  allowMSAA: " + c.allowMSAA);
            Debug.Log("  areVRStereoViewMatricesWithinSingleCullTolerance: " + c.areVRStereoViewMatricesWithinSingleCullTolerance);
            Debug.Log("  aspect: " + c.aspect);
            Debug.Log("  backgroundColor: " + c.backgroundColor);
            Debug.Log("  cameraToWorldMatrix: " + c.cameraToWorldMatrix);
            Debug.Log("  cameraType: " + c.cameraType);
            Debug.Log("  clearFlags: " + c.clearFlags);
            Debug.Log("  clearStencilAfterLightingPass: " + c.clearStencilAfterLightingPass);
            Debug.Log("  commandBufferCount: " + c.commandBufferCount);
            Debug.Log("  cullingMask: " + c.cullingMask);
            Debug.Log("  cullingMatrix: " + c.cullingMatrix);
            Debug.Log("  depth: " + c.depth);
            Debug.Log("  depthTextureMode: " + c.depthTextureMode);
            Debug.Log("  eventMask: " + c.eventMask);
            Debug.Log("  farClipPlane: " + c.farClipPlane);
            Debug.Log("  fieldOfView: " + c.fieldOfView);
            Debug.Log("  focalLength: " + c.focalLength);
            Debug.Log("  forceIntoRenderTexture: " + c.forceIntoRenderTexture);
            Debug.Log("  getFit: " + c.gateFit);
            Debug.Log("  layerCullDistances: " + c.layerCullDistances);
            Debug.Log("  layerCullSpherical: " + c.layerCullSpherical);
            Debug.Log("  lensShift: " + c.lensShift);
            Debug.Log("  nearClipPlane: " + c.nearClipPlane);
            Debug.Log("  nonJitteredProjectionMatrix: " + c.nonJitteredProjectionMatrix);
            Debug.Log("  opaqueSortMode: " + c.opaqueSortMode);
            Debug.Log("  orthographic: " + c.orthographic);
            Debug.Log("  orthographicSize: " + c.orthographicSize);
            Debug.Log("  orverrideSceneCullingMask: " + c.overrideSceneCullingMask);
            Debug.Log("  pixelHeight: " + c.pixelHeight);
            Debug.Log("  pixelRect: " + c.pixelRect);
            Debug.Log("  pixelWidth: " + c.pixelWidth);
            Debug.Log("  previousViewProjectionMatrix: " + c.previousViewProjectionMatrix);
            Debug.Log("  projectionMatrix: " + c.projectionMatrix);
            Debug.Log("  rect: " + c.rect);
            Debug.Log("  renderingPath: " + c.renderingPath);
            Debug.Log("  scaledPixelHeight: " + c.scaledPixelHeight);
            Debug.Log("  scaledPixelWidth: " + c.scaledPixelWidth);
            Debug.Log("  scene: " + c.scene);
            Debug.Log("  sensorSize: " + c.sensorSize);
            Debug.Log("  stereoActiveEye: " + c.stereoActiveEye);
            Debug.Log("  stereoConvergence: " + c.stereoConvergence);
            Debug.Log("  stereoEnabled: " + c.stereoEnabled);
            Debug.Log("  stereoSeparation: " + c.stereoSeparation);
            Debug.Log("  stereoTargetEye: " + c.stereoTargetEye);
            Debug.Log("  targetDisplay: " + c.targetDisplay);
            Debug.Log("  targetTexture: " + c.targetTexture);
            Debug.Log("  transparencySortAxis: " + c.transparencySortAxis);
            Debug.Log("  transparencySortMode: " + c.transparencySortMode);
            Debug.Log("  useJitteredProjectionMatrixForTransparentRendering: "
                + c.useJitteredProjectionMatrixForTransparentRendering);
            Debug.Log("  useOcclusionCulling: " + c.useOcclusionCulling);
            Debug.Log("  usePhysicalProperties: " + c.usePhysicalProperties);
            Debug.Log("  velocity: " + c.velocity);
            Debug.Log("  worldToCameraMatrix: " + c.worldToCameraMatrix);
            var skybox = c.GetComponent<Skybox>();
            if (skybox != null)
            {
                Debug.Log("Skybox : " + skybox.name + "  Material: " + skybox.material);
            }
        }

        private void Update()
        {
            /*
            foreach (Camera c in GameObject.FindObjectsOfType<Camera>())
            {
                PrintCamera(c);
            }
            foreach (var canvas in GameObject.FindObjectsOfType<Canvas>()) {
                Debug.Log("CANVAS NAME= " + canvas.name);
            }*/
            foreach (Camera c in GameObject.FindObjectsOfType<Camera>())
            {
                if (vrStarting)
                {
                    Debug.Log("Setting Camera " + c.name + ".orthographic to false");
                    c.orthographic = false;
                }
                if (c.name == "Main Camera" && cameraParent)
                {
                    // Update each frame to move the custom camera to the position of the Main Camera
                    cameraParent.transform.position = c.gameObject.transform.position;

                }
            }
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("OnSceneLoaded: " + scene.name);
            if (scene.name == "Game")
            {
                StartVR();
            }
        }

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {"com.McCary.GloomhavenVRMod"} is loaded!");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void StartVR() 
            {
            Debug.Log("Starting VR!");
            vrStarting = true;
            SteamVR_Actions.PreInitialize();

            var generalSettings = ScriptableObject.CreateInstance<XRGeneralSettings>();
            var managerSettings = ScriptableObject.CreateInstance<XRManagerSettings>();
            var xrLoader = ScriptableObject.CreateInstance<OpenVRLoader>();
            cameraParent = new GameObject("testCamera");
            var camera = cameraParent.AddComponent<Camera>();
            var settings = OpenVRSettings.GetSettings();
            settings.StereoRenderingMode = OpenVRSettings.StereoRenderingModes.MultiPass;
            generalSettings.Manager = managerSettings;
            

            camera.gameObject.AddComponent<SteamVR_TrackedObject>();
            camera.stereoTargetEye = StereoTargetEyeMask.Both;

            managerSettings.loaders.Add(xrLoader);
            managerSettings.InitializeLoaderSync();
            managerSettings.StartSubsystems();
            managerSettings.automaticLoading = true;
            XRGeneralSettings.AttemptInitializeXRSDKOnLoad();
            XRGeneralSettings.AttemptStartXRSDKOnBeforeSplashScreen();

            SteamVR.Initialize();

            



        }
    }
}
