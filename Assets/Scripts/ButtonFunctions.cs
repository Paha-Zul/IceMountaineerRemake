using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using GooglePlayGames;
using UnityEngine.UI;
//using AppodealAds.Unity.Api;

public class ButtonFunctions : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void Play(string type) {
        if (type == "normal") {
            GameLevel.levelType = GameLevel.LevelType.Normal;
            GPGFunctions.incrementEvent("CgkIzpi-qqMDEAIQBw", 1);
        }
        if (type == "angled") {
            GameLevel.levelType = GameLevel.LevelType.Angled;
            GPGFunctions.incrementEvent("CgkIzpi-qqMDEAIQCA", 1);
        }
        if (type == "double") {
            GameLevel.levelType = GameLevel.LevelType.Double;
            GPGFunctions.incrementEvent("CgkIzpi-qqMDEAIQCQ", 1);
        }
        SceneManager.LoadScene("Game");
    }

    public void Restart(){
        SceneManager.LoadScene("Game");
    }

    public void LoadMainMenu(){
        SceneManager.LoadScene("MainMenu");
        AdFunctions.instance().displayInterToMainMenu();
    }

    /// <summary>
    /// Moves from the main menu to the game type panel
    /// </summary>
    public void ToGameType() {
        SwapPanels(MainMenu.inst.gameTypePanelRectTransform);
    }

    /// <summary>
    /// Moves from the main menu to the item panel.
    /// </summary>
    public void ToItems() {
        MainMenu.inst.loadItemDataInfoIntoItemStore();
        SwapPanels(MainMenu.inst.itemPanelRectTransform);
    }

    public void toDevMenu() {
        SwapPanels(MainMenu.inst.devPanelTransform);
    }

    public void ToMainMenu() {
        SwapPanels(MainMenu.inst.mainMenuPanelRectTransform);
    }

    private void SwapPanels(RectTransform swapIn) {
        MainMenu.inst.currPanel.transform.position = MainMenu.inst.menuRoot.transform.position; //Move curr out to the anchor
        MainMenu.inst.currPanel.gameObject.SetActive(false); //Disable it

        MainMenu.inst.currPanel = swapIn; //Switch curr
        MainMenu.inst.currPanel.position = MainMenu.inst.mainPanelRectTransform.position; //Move curr in
        swapIn.gameObject.SetActive(true); //Set curr active
    }

    public void OpenLeaderboards() {
        GPGFunctions.openLeaderboards();
    }

    public void LoginToGooglePlay() {
        if (!Social.localUser.authenticated) {
            Social.localUser.Authenticate((bool success) => {
                // handle success or failure
                //StartCoroutine(loadGame());
                if (success) {
                    MainMenu.inst.loginGoogleButton.transform.GetChild(0).GetComponent<Text>().text = "Log Out of \n Google Play"; //Disable login button.
                    MainMenu.inst.leaderboardButton.interactable = true; //Enable the leaderboard button.
                }
            });
        } else {
            MainMenu.inst.loginGoogleButton.transform.GetChild(0).GetComponent<Text>().text = "Log In to \n Google Play";
            MainMenu.inst.leaderboardButton.interactable = false; //Enable the leaderboard button.
            PlayGamesPlatform.Instance.SignOut();
        }
    }

    public void DisplayInterAd() {
        //AdFunctions.instance().displayInter();
    }

    public void PlayRewardVideo() {
        AdFunctions.instance().displayRewardVideo();
    }

    public void TestAdsHeyzap() {
        // ?
    }

    public void addAllUpgrades() {
        UpgradeManager.maxAllUpgrades();
    }

    public void clearAllUpgrades() {
        UpgradeManager.clearAllUpgrades();
    }

    public void addCoins(int amount) {
        //Refresh the items/text
        int coins = PlayerPrefs.GetInt(Constants.CoinPrefString);
        coins += amount;
        PlayerPrefs.SetInt("Coins", coins);
        MainMenu.RefreshCoinAmount();
    }

    public void clearCoins() {
        PlayerPrefs.SetInt("Coins", 0);
        MainMenu.RefreshCoinAmount();
    }

    public void Purchase(string type) {
        if(MainMenu.inst.upgradeManagerScript.MakePurchase(type)) {
            this.GetComponent<AudioSource>().Play();
        }
    }
}
