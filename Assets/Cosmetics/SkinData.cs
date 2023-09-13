using System;
using System.Collections;
using Cosmetics;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SkinData : CosmeticData, IAddressableAssetProvider<SkinViewData>
{
    public bool IsEmpty
    {
        get
        {
            return base.ProdId == "skin_None";
        }
    }

    public AddressableAsset<SkinViewData> CreateAddressableAsset()
    {
        return new AddressableAsset<SkinViewData>(this.ViewDataRef);
    }

    public AssetReference GetAssetReference()
    {
        return this.ViewDataRef;
    }

    public const string EmptyId = "skin_None";
    public const string RHMId = "skin_rhm";
    public AssetReference ViewDataRef;
    public string StoreName;
}