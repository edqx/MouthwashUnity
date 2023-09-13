using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "Hat Data", menuName = "Mouthwash/Hat Data")]
public class HatData : CosmeticData, IAddressableAssetProvider<HatViewData>
{
    public bool IsEmpty
    {
        get
        {
            return this.ProductId == "hat_NoHat";
        }
    }
    
    public AddressableAsset<HatViewData> CreateAddressableAsset()
    {
        return new AddressableAsset<HatViewData>(ViewDataRef);
    }
    public AssetReference GetAssetReference()
    {
        return ViewDataRef;
    }

    public const string EmptyId = "hat_NoHat";
    public AssetReference ViewDataRef;
    public bool InFront;
    public bool NoBounce;
    public bool BlocksVisors;
    public string StoreName;
    public SkinData RelatedSkin;
}