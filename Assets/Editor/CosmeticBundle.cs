using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Assets.Editor
{
    [Serializable]
    public struct CosmeticDataAndView
    {
        public string name;
        public string itemUuid;
        public BundleValuation valuation;
        public CosmeticData cosmeticData;
        public ScriptableObject cosmeticViewData;
    }

    public enum BundleValuation
    {
        Ghost,
        Crewmate,
        Impostor,
        Polus
    }
    
    [CreateAssetMenu(fileName = "Cosmetic Bundle", menuName = "Mouthwash/Cosmetic Bundle")]
    public class CosmeticBundle : ScriptableObject
    {
        public string bundleName;
        public string bundleUuid;
        public string fileUuid;
        public string authorId;
        public uint priceUsd;
        public BundleValuation valuation;
        public string thumbnailUrl;
        public string description;
        public string tags;
        public string featureTags;
        public uint baseResourceId;
        public List<CosmeticDataAndView> cosmeticItems;
        
        public string assetBundlePath;
    }
}