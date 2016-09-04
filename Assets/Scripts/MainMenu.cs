using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour{
    public UpgradeManager upgradeManagerScript;

    public Text coinText;
    public GameObject menuRoot, mainPanel, startPanel, gameTypePanel, itemPanel, devPanel, canvas;
    public Text loggedInText;
    public Button loginGoogleButton, leaderboardButton, videoAdButton, interAdButton;

    private static Text _coinText;

    [HideInInspector]
    public RectTransform mainPanelRectTransform, mainMenuPanelRectTransform, gameTypePanelRectTransform, itemPanelRectTransform, canvasRectTransform, devPanelTransform;

    [HideInInspector]
    public RectTransform currPanel;

    [HideInInspector]
    public Transform offScreenGuiAnchor;

    public static MainMenu inst { get; private set; }

    private static bool startedOnce = false;

    // Use this for initialization
    void Start() {
        inst = this;
        _coinText = coinText;
        MainMenu.RefreshCoinAmount();

        this.mainPanelRectTransform = mainPanel.GetComponent<RectTransform>();
        this.mainMenuPanelRectTransform = startPanel.GetComponent<RectTransform>();
        this.gameTypePanelRectTransform = gameTypePanel.GetComponent<RectTransform>();
        this.itemPanelRectTransform = itemPanel.GetComponent<RectTransform>();
        this.canvasRectTransform = canvas.GetComponent<RectTransform>();
        this.devPanelTransform = devPanel.GetComponent<RectTransform>();
        //PlayerPrefs.SetInt("Coins", 0); //Used to reset the coins when they get out of hand.

        this.spaceButtonsOnGameTypePanel();
        this.checkHash();

        if (startedOnce) {

        }

        if (GPGFunctions.isAuthenticated())
            this.loginGoogleButton.gameObject.SetActive(false);

        if (NetChecker.connection == NetChecker.Connection.NoInternet) {
            this.loginGoogleButton.interactable = false;
            this.leaderboardButton.interactable = false;
            this.videoAdButton.interactable = false;
            this.interAdButton.interactable = false;
        }

        if (!GPGFunctions.isAuthenticated()) {
            this.leaderboardButton.interactable = false;
            MainMenu.inst.loginGoogleButton.transform.GetChild(0).GetComponent<Text>().text = "Log In to \n Google Play";
        } else {
            this.leaderboardButton.interactable = true;
            this.loginGoogleButton.transform.GetChild(0).GetComponent<Text>().text = "Log Out of \n Google Play";
        }

        if (!Debug.isDebugBuild) {
            GameObject.Find("Dev").SetActive(false);
            GameObject.Find("DevPanel").SetActive(false);
        }

        startedOnce = true;

        currPanel = mainMenuPanelRectTransform;
    }

    /// <summary>
    /// Checks some secret stuff. Not going to use this. Not worth it.
    /// </summary>
    private void checkHash() {
        float hash = SecretStuff.getHash2();
        float savedHash = PlayerPrefs.GetInt("hash");

        Debug.Log("hash: " + hash + ", savedHash: " + savedHash);
    }

    /// <summary>
    /// Refreshes the coin amount to match the stored value in player preferences
    /// </summary>
    public static void RefreshCoinAmount() {
        if(_coinText != null)
            _coinText.text = PlayerPrefs.GetInt(Constants.CoinPrefString).ToString();
    }

    /// <summary>
    /// Loads/refreshes the entire item store.
    /// </summary>
    public void loadItemDataInfoIntoItemStore() {
        foreach (KeyValuePair<string, UpgradeManager.Upgrade> entry in UpgradeManager.getDictionary()) {
            // do something with entry.Value or entry.Key
            entry.Value.costText.text = "" + (entry.Value.cost + entry.Value.costIncr * entry.Value.curr).ToString();
            entry.Value.currText.text = entry.Value.curr + "/" + entry.Value.max;
        }
    }

    /// <summary>
    /// Centers the buttons on the gameTypePanel both vertically and horizontally. Couldn't find an easier way to do this...
    /// </summary>
    private void spaceButtonsOnGameTypePanel() {
        List<RectTransform> list = new List<RectTransform>();
        foreach(Transform child in this.gameTypePanel.transform) {
            if(child.gameObject.activeSelf)
                list.Add(child.GetComponent<RectTransform>());
        }

        RectTransform parentRect = this.gameTypePanel.GetComponent<RectTransform>();
        float height = parentRect.rect.height;
        float width = parentRect.rect.width;
        int num = list.Count + 1;
        float spaceY = (height - ((list[0].rect.height)* num)) / num;

        for(int i = 0; i < list.Count; i++) {
            list[i].position = new Vector2(parentRect.position.x, parentRect.position.y - parentRect.rect.height/2 + (list[i].rect.height + spaceY)*(i+1));
        }
    }

    void OnDestroy() {
        //Appodeal.hide(Appodeal.BANNER_BOTTOM);
    }

    void OnApplicationQuit() {
        PlayerPrefs.SetInt("hash", SecretStuff.getHash2());
    }
}
