using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class NetChecker {
    public enum Connection { Unknown, Connected, NoInternet}
    public static Connection connection = Connection.Unknown;

    private static ConnectionTesterStatus status = ConnectionTesterStatus.Undetermined;

    public static void Test(params Button[] buttonsToDisable) {
        status = Network.TestConnection();
        switch (status) {
            case ConnectionTesterStatus.LimitedNATPunchthroughPortRestricted:
            case ConnectionTesterStatus.LimitedNATPunchthroughSymmetric:
            case ConnectionTesterStatus.NATpunchthroughAddressRestrictedCone:
            case ConnectionTesterStatus.NATpunchthroughFullCone:
            case ConnectionTesterStatus.Error:
                foreach (Button button in buttonsToDisable)
                    button.interactable = false;
                break;
            case ConnectionTesterStatus.PublicIPIsConnectable:
                foreach (Button button in buttonsToDisable)
                    button.interactable = false;
                break;
            
        }
    }

    public static IEnumerator checkInternetConnection(Action<bool> action) {
        WWW www = new WWW("http://google.com");
        yield return www;
        if (www.error != null) {
            action(false);
        } else {
            action(true);
        }
    }

    void Start() {
       
    }

}
