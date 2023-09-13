
using System.Collections.Generic;
using Cosmetics;
using UnityEngine;

public enum Platforms
{
    Unknown,
    StandaloneEpicPC,
    StandaloneSteamPC,
    StandaloneMac,
    StandaloneWin10,
    StandaloneItch,
    IPhone,
    Android,
    Switch,
    Xbox,
    Playstation
}

public class CosmeticData : ScriptableObject, IBuyable
{
    public string ProdId
    {
        get
        {
            return this.ProductId;
        }
    }

    public int BeanCost
    {
        get
        {
            return this.beanCost;
        }
    }

    public int StarCost
    {
        get
        {
            return this.starCost;
        }
    }

    public string EpicId
    {
        get
        {
            return this.epicId;
        }
    }

    public bool PaidOnMobile
    {
        get
        {
            return this.paidOnMobile;
        }
    }

    public LimitedTimeStartEnd LimitedTimeAvailable
    {
        get
        {
            return this.limitedTime;
        }
    }
    public const string TranslationPrefix = "Cosmetic.";
    public List<Platforms> unlockOnSelectPlatforms;
    public bool freeRedeemableCosmetic;
    public int redeemPopUpColor = -1;
    public string epicId = "";
    public string BundleId;
    public string ProductId;
    public Vector2 ChipOffset;
    public int beanCost;
    public int starCost;
    public bool paidOnMobile;
    public LimitedTimeStartEnd limitedTime;
    public int displayOrder;
    public bool NotInStore;
    public bool Free;
    public Sprite SpritePreview;
    public bool PreviewCrewmateColor;
}