using System;
using System.Collections;
using Cosmetics;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class VisorData : CosmeticData, IAddressableAssetProvider<VisorViewData>
{
	public bool BehindHats
	{
		get
		{
			return this.behindHats;
		}
	}

	public bool IsEmpty
	{
		get
		{
			return this.ProductId == "visor_EmptyVisor";
		}
	}

	public AddressableAsset<VisorViewData> CreateAddressableAsset()
	{
		return new AddressableAsset<VisorViewData>(this.ViewDataRef);
	}

	public AssetReference GetAssetReference()
	{
		return this.ViewDataRef;
	}

	[SerializeField]
	private bool behindHats;
	public const string EmptyId = "visor_EmptyVisor";
	public AssetReference ViewDataRef;
}