using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GPGFunctions.start();
        AdFunctions.instance().init();

        NetChecker.Test(null);
        StartCoroutine(NetChecker.checkInternetConnection((isConnected) => {
            if (isConnected)
                NetChecker.connection = NetChecker.Connection.Connected;
            else
                NetChecker.connection = NetChecker.Connection.NoInternet;
        }));

        StartCoroutine(loadGame());
    }

    IEnumerator loadGame() {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("MainMenu");
    }
}
