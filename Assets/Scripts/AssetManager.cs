using UnityEngine;
using System.Collections.Generic;

public class AssetManager : MonoBehaviour {

    private static Dictionary<string, object> assetMap;

    // Use this for initialization
    void Start () {
        assetMap = new Dictionary<string, object>();

        var assets = Resources.LoadAll("");

        for (int i = 0; i < assets.Length; i++) {
            assetMap.Add(assets[i].name, assets[i]);
        }

        assets = null;
	}

    public static T getAsset<T>(string name) {
        object asset;
        assetMap.TryGetValue(name, out asset);
        return (T)asset;
    }

    

}
