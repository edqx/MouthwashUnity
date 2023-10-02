using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace Assets.Editor
{
    [Serializable]
    public class CosmeticBundleManifest
    {
        public Dictionary<string, string> References = new Dictionary<string, string>();
    }

    [Serializable]
    public struct AuthLoginRequestBody
    {
        public string email;
        public string password;
    }

    [Serializable]
    public struct AuthLoginResponseBody
    {
        public string display_name;
        public string client_token;
    }
    
    [Serializable]
    public struct UploadToBucketVec2
    {
        public float x;
        public float y;
    }

    [Serializable]
    public struct UploadToBucketAsset
    {
        public string type;
        public string main;
        public string back;
        public string left_main;
        public string left_back;
        public string climb;
        public string floor;
        public string left_climb;
        public string left_floor;
        public string thumb;
        public string product_id;
        public bool in_front;
        public bool player_material;
        public UploadToBucketVec2 chip_offset;
        public string asset_bundle_path;
    }

    [Serializable]
    public struct UploadToBucketRequestBody
    {
        public string file_uuid;
        public string bundle_data_base_64;
        public UploadToBucketAsset[] bundle_assets_listing;
        public int base_resource_id;
    }

    [Serializable]
    public struct PublishCosmeticBundleItem
    {
        public string id;
        public string name;
        public string among_us_id;
        public string resource_path;
        public string type;
        public int resource_id;
        public string valuation;
    }
    
    [Serializable]
    public struct PublishCosmeticBundleRequestBody
    {
        public string id;
        public string name;
        public string thumbnail_url;
        public string author_id;
        public int base_resource_id;
        public int price_usd;
        public string file_uuid;
        public string valuation;
        public string[] tags;
        public string description;
        public string[] feature_tags;
        public PublishCosmeticBundleItem[] items;
    }

    [Serializable]
    public struct StandardApiResponse<T>
    {
        public bool success { get; set; }
        public T data { get; set; }
    }

    [CustomEditor(typeof(CosmeticBundle))]
    public class CosmeticBundleEditor : UnityEditor.Editor
    {
        protected string email;
        protected string password;

        protected List<UploadToBucketAsset> assetMetadata = new List<UploadToBucketAsset>();

        protected AuthLoginResponseBody? logInInformation;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CosmeticBundle bundle = (CosmeticBundle)target;
            
            if (GUILayout.Button("Build Asset Bundle"))
            {
                string manifestJsonPath = Path.Combine(Path.GetDirectoryName(AssetDatabase.GetAssetPath(target)),
                    "Manifest.json");
                CosmeticBundleManifest cosmeticManifest = new CosmeticBundleManifest();
                foreach (CosmeticDataAndView dataAndView in bundle.cosmeticItems)
                {
                    cosmeticManifest.References.Add(AssetDatabase.GetAssetPath(dataAndView.cosmeticData), AssetDatabase.GetAssetPath(dataAndView.cosmeticViewData));
                }
                File.WriteAllText(manifestJsonPath, JsonConvert.SerializeObject(cosmeticManifest));

                TextAsset manifestTextAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(manifestJsonPath);
                
                ScriptableObject[] distinctAssets = bundle.cosmeticItems.Select(x => x.cosmeticData)
                    .Concat(bundle.cosmeticItems.Select(x => x.cosmeticViewData)).ToArray();

                AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles("Assets/AssetBundles", new []
                {
                    new AssetBundleBuild
                    {
                        assetBundleName = bundle.bundleName.ToLower(),
                        assetNames = distinctAssets.Select(AssetDatabase.GetAssetPath).Append(AssetDatabase.GetAssetPath(manifestTextAsset)).ToArray()
                    }
                }, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
                serializedObject.FindProperty("assetBundlePath").stringValue =
                    Path.Combine(Path.GetDirectoryName(Application.dataPath), "Assets/AssetBundles", bundle.bundleName.ToLower());
                serializedObject.ApplyModifiedProperties();
            }
            
            GUILayout.Space(16);

            if (logInInformation != null)
            {
                GUILayout.Label($"Logged in as: {logInInformation.Value.display_name}");
                if (GUILayout.Button("Logout"))
                {
                    logInInformation = null;
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Email: ");
                email = GUILayout.TextField(email);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Password: ");
                password = GUILayout.TextField(password);
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Upload or Update Assets in Bucket"))
            {
                GetAssetPaths();
                Task.Run(CoUploadAssetsToBucket);
            }

            if (GUILayout.Button("Publish as Cosmetic Bundle"))
            {
                GetAssetPaths();
                Task.Run(CoPublishCosmeticItems);
            }
        }

        public string LoadAssetAsBase64(UnityEngine.Object asset)
        {
            if (asset == null)
                return null;
            string assetPath = AssetDatabase.GetAssetPath(asset);
            string absolutePath = Path.Combine(Path.GetDirectoryName(Application.dataPath), assetPath);
            byte[] assetBundleData = File.ReadAllBytes(absolutePath);
            return Convert.ToBase64String(assetBundleData);
        }

        public void GetAssetPaths()
        {
            CosmeticBundle bundle = (CosmeticBundle)target;
            assetMetadata.Clear();
            foreach (CosmeticDataAndView dataAndView in bundle.cosmeticItems)
            {
                string assetPath = AssetDatabase.GetAssetPath(dataAndView.cosmeticData);
                Debug.Log(((HatViewData)dataAndView.cosmeticViewData).AltShader);
                assetMetadata.Add(new UploadToBucketAsset
                {
                    asset_bundle_path = assetPath,
                    type = "HAT",
                    main = LoadAssetAsBase64(((HatViewData)dataAndView.cosmeticViewData).MainImage),
                    back = LoadAssetAsBase64(((HatViewData)dataAndView.cosmeticViewData).BackImage),
                    left_main = LoadAssetAsBase64(((HatViewData)dataAndView.cosmeticViewData).LeftMainImage),
                    left_back = LoadAssetAsBase64(((HatViewData)dataAndView.cosmeticViewData).LeftBackImage),
                    climb = LoadAssetAsBase64(((HatViewData)dataAndView.cosmeticViewData).ClimbImage),
                    floor = LoadAssetAsBase64(((HatViewData)dataAndView.cosmeticViewData).FloorImage),
                    left_climb = LoadAssetAsBase64(((HatViewData)dataAndView.cosmeticViewData).LeftClimbImage),
                    left_floor = LoadAssetAsBase64(((HatViewData)dataAndView.cosmeticViewData).LeftFloorImage),
                    thumb = LoadAssetAsBase64(dataAndView.cosmeticData.SpritePreview.texture),
                    product_id = dataAndView.cosmeticData.ProductId,
                    in_front = ((HatData)dataAndView.cosmeticData).InFront,
                    player_material = ((HatViewData)dataAndView.cosmeticViewData).AltShader != null,
                    chip_offset = new UploadToBucketVec2{ x = dataAndView.cosmeticData.ChipOffset.x, y = dataAndView.cosmeticData.ChipOffset.y }
                });
            }
        }

        public async Task CoLogin(HttpClient httpClient)
        {
            if (logInInformation == null)
            {
                string loginRequestBody = JsonConvert.SerializeObject(new AuthLoginRequestBody
                {
                    email = email,
                    password = password
                });
                HttpRequestMessage loginRequest = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("http://localhost:8000/api/v2/auth/token"),
                    Content = new StringContent(loginRequestBody, Encoding.Default, "application/json")
                };
                Debug.Log($"Requesting login for {email}..");
                HttpResponseMessage loginResponse = await httpClient.SendAsync(loginRequest);
                if (!loginResponse.IsSuccessStatusCode)
                {
                    Debug.Log($"Failed to login! ({loginResponse.StatusCode})");
                    Debug.Log(await loginResponse.Content.ReadAsStringAsync());
                    return;
                }

                string loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
                logInInformation = JsonConvert.DeserializeObject<StandardApiResponse<AuthLoginResponseBody>>(loginResponseContent).data;
                Debug.Log($"Successfully logged in!");
            }
            password = "";
        }

        public async Task CoUploadAssetsToBucket()
        {
            Debug.Log("Beginning asset bundle upload..");
            HttpClient httpClient = new HttpClient();

            await CoLogin(httpClient);
            
            Debug.Log("Requesting bundle upload..");
            CosmeticBundle bundle = (CosmeticBundle)target;
            byte[] assetBundleData = File.ReadAllBytes(bundle.assetBundlePath);
            string assetBundleDataBase64 = Convert.ToBase64String(assetBundleData);

            UploadToBucketRequestBody requestBody = new UploadToBucketRequestBody
            {
                file_uuid = bundle.fileUuid,
                bundle_data_base_64 = assetBundleDataBase64,
                bundle_assets_listing = assetMetadata.ToArray(),
                base_resource_id = (int)bundle.baseResourceId
            };
            string requestBodyContent = JsonConvert.SerializeObject(requestBody);
            HttpRequestMessage uploadRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("http://localhost:8000/api/v2/commerce/upload"),
                Content = new StringContent(requestBodyContent, Encoding.Default, "application/json")
            };
            uploadRequest.Headers.Add("Authorization", $"Bearer {logInInformation.Value.client_token}");
            Debug.Log($"Uploading {requestBodyContent.Length} bytes..");
            HttpResponseMessage uploadResponse = await httpClient.SendAsync(uploadRequest);
            if (!uploadResponse.IsSuccessStatusCode)
            {
                Debug.Log($"Failed to upload bundle to bucket! ({uploadResponse.StatusCode})");
                Debug.Log(await uploadResponse.Content.ReadAsStringAsync());
                return;
            }
            Debug.Log("Successfully uploaded bundle!");
        }

        public string GetValuationString(BundleValuation valuation)
        {
            switch (valuation)
            {
                case BundleValuation.Ghost:
                    return "GHOST";
                case BundleValuation.Crewmate:
                    return "CREWMATE";
                case BundleValuation.Impostor:
                    return "IMPOSTOR";
                case BundleValuation.Polus:
                    return "POLUS";
            }

            return "";
        }
        
        public async Task CoPublishCosmeticItems()
        {
            Debug.Log("Beginning asset bundle publish..");
            HttpClient httpClient = new HttpClient();

            await CoLogin(httpClient);
            
            Debug.Log("Requesting bundle publish..");
            CosmeticBundle bundle = (CosmeticBundle)target;

            PublishCosmeticBundleRequestBody requestBody = new PublishCosmeticBundleRequestBody
            {
                id = bundle.bundleUuid,
                name = bundle.bundleName,
                thumbnail_url = bundle.thumbnailUrl,
                author_id = bundle.authorId,
                base_resource_id = (int)bundle.baseResourceId,
                price_usd = (int)bundle.priceUsd,
                file_uuid = bundle.fileUuid,
                valuation = GetValuationString(bundle.valuation),
                tags = bundle.tags.Split(','),
                description = bundle.description,
                feature_tags = bundle.featureTags.Split(','),
                items = bundle.cosmeticItems.Select((x, idx) => new PublishCosmeticBundleItem()
                {
                    id = x.itemUuid,
                    name = x.name,
                    among_us_id = x.cosmeticData.ProductId,
                    resource_path = assetMetadata[idx].asset_bundle_path,
                    type = x.cosmeticData is HatData ? "HAT" : "UNKNOWN",
                    resource_id = (int)bundle.baseResourceId + (idx + 1) * 2,
                    valuation = GetValuationString(x.valuation)
                }).ToArray()
            };
            string requestBodyContent = JsonConvert.SerializeObject(requestBody);
            HttpRequestMessage publishRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("http://localhost:8000/api/v2/commerce/publish"),
                Content = new StringContent(requestBodyContent, Encoding.Default, "application/json")
            };
            publishRequest.Headers.Add("Authorization", $"Bearer {logInInformation.Value.client_token}");
            Debug.Log($"Uploading {requestBodyContent.Length} bytes..");
            HttpResponseMessage uploadResponse = await httpClient.SendAsync(publishRequest);
            if (!uploadResponse.IsSuccessStatusCode)
            {
                Debug.Log($"Failed to publish bundle! ({uploadResponse.StatusCode})");
                Debug.Log(await uploadResponse.Content.ReadAsStringAsync());
                return;
            }
            Debug.Log("Successfully published bundle!");
        }
    }
}