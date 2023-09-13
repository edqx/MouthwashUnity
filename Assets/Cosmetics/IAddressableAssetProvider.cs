using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public class CallbackGroup
{
	private List<Action> callbacks = new List<Action>();
}


public class CallbackResultGroup
{
	private CallbackGroup onError = new CallbackGroup();
	private CallbackGroup onSuccess = new CallbackGroup();
	private CallbackGroup onFinished = new CallbackGroup();
}

public abstract class AddressableAsset
{
	public abstract void Destroy();
}

[Serializable]
public class AddressableAsset<T> : AddressableAsset where T : UnityEngine.Object
{
    public AddressableAsset()
    {
	    onLoadedCallbackGroup = new CallbackResultGroup();
    }

    public AddressableAsset(AssetReference assetRef)
    {
	    this.assetRef = assetRef;
    }

    ~AddressableAsset()
    {
	    this.Destroy();
    }

    public override void Destroy()
    {
	    assetRef = null;
    }

    [SerializeField]
    protected AssetReference assetRef;
    private CallbackResultGroup onLoadedCallbackGroup = new CallbackResultGroup();
    private AsyncOperationHandle handle;
}

public interface IAddressableAssetProvider<T> where T : UnityEngine.Object
{
    AddressableAsset<T> CreateAddressableAsset();
    AssetReference GetAssetReference();
}