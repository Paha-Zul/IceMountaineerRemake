using UnityEngine;
using System.Collections;

public class SecretStuff{

    private static int getHash(params int[] values) {
        int hash = 0;
        foreach(int value in values) {
            hash += value;
        }
        hash = (hash*int.MaxValue) % int.MaxValue;
        return hash;
    }

    public static int getHash2() {
        int coins = PlayerPrefs.GetInt("Coins");
        int speed = PlayerPrefs.GetInt("RopeSpeed");
        int length = PlayerPrefs.GetInt("RopeLength");
        int bounciness = PlayerPrefs.GetInt("Bounciness");
        int hardHat = PlayerPrefs.GetInt("HardHat");
        int birdshield = PlayerPrefs.GetInt("BirdShield");
        int hookSpeed = PlayerPrefs.GetInt(Constants.HookSpeedPrefString);

        return getHash(coins, speed, length, bounciness, hardHat, hookSpeed, birdshield);
    }

}
