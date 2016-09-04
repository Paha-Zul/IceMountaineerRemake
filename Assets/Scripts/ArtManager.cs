using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArtManager {
    private static List<string> artSetEndings;
    private static bool[] shardLight;

    static ArtManager(){
        artSetEndings = new List<string> { "", "_night" };
        shardLight = new bool[] { false, true };
    }

    public static T getArtSetResource<T>(string name, int artType) {
        var resourceName = name + artSetEndings[artType];
        T item = AssetManager.getAsset<T>(resourceName);
        if (item == null)
            item = AssetManager.getAsset<T>(name);

        return item;
    }

    public static bool getArtSetShardLight(int artType) {
        return shardLight[artType];
    }
}
