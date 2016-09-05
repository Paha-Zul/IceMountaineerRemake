using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour {
    public MainMenu mainMenuScript;

    public Text currSpeedText, currLengthText, currBouncinessText, currHardHatText, currHookSpeedText, currBirdShieldText;
    public Text costSpeedText, costLengthText, costBouncinessText, costHardHatText, costHookSpeedText, costBirdShieldText;

    private static Dictionary<string, Upgrade> upgradeMap = new Dictionary<string, Upgrade>();
    public static bool populated { get; set; }

    // Use this for initialization
    void Start () {
        this.populateUpgrades();
        this.validatePrefLimits();
    }

    private void populateUpgrades() {
        int currRopeSpeed = PlayerPrefs.GetInt(Constants.RopeSpeedPrefString);
        int currRopeLength = PlayerPrefs.GetInt(Constants.RopeLengthPrefString);
        int currBounciness = PlayerPrefs.GetInt(Constants.BouncinessPrefString);
        int currHardHat = PlayerPrefs.GetInt("HardHat");
        int currHookSpeed = PlayerPrefs.GetInt("HookSpeed");
        //int coins = PlayerPrefs.GetInt(Defaults.CoinPrefString);

        this.currSpeedText = GameObject.Find("CostSpeed").GetComponent<Text>();
        this.currLengthText = GameObject.Find("CostLength").GetComponent<Text>();
        this.currBouncinessText = GameObject.Find("CostBounciness").GetComponent<Text>();
        this.currHardHatText = GameObject.Find("CostHardHat").GetComponent<Text>();
        this.currHookSpeedText = GameObject.Find("CostHookSpeed").GetComponent<Text>();
        this.currBirdShieldText = GameObject.Find("CostBirdShield").GetComponent<Text>();

        this.costSpeedText = GameObject.Find("CurrSpeed").GetComponent<Text>();
        this.costLengthText = GameObject.Find("CurrLength").GetComponent<Text>();
        this.costBouncinessText = GameObject.Find("CurrBounciness").GetComponent<Text>();
        this.costHardHatText = GameObject.Find("CurrHardHat").GetComponent<Text>();
        this.costHookSpeedText = GameObject.Find("CurrHookSpeed").GetComponent<Text>();
        this.costBirdShieldText = GameObject.Find("CurrBirdShield").GetComponent<Text>();

        RefreshOrAddUpgrade("ropespeed", "RopeSpeed", 15, 1, 5, currRopeSpeed, 0.05f, costSpeedText, currSpeedText);
        RefreshOrAddUpgrade("ropelength", "RopeLength", 15, 1, 5, currRopeLength, -0.5f, costLengthText, currLengthText);
        RefreshOrAddUpgrade("bounciness", "Bounciness", 15, 1, 5, currBounciness, -0.01f, this.costBouncinessText, this.currBouncinessText);
        RefreshOrAddUpgrade("hookspeed", "HookSpeed", 15, 1, 5, currHookSpeed, 0.01f, this.costHookSpeedText, this.currHookSpeedText);
        RefreshOrAddUpgrade("hardhat", "HardHat", 50, 0, 1, currHardHat, 0, this.costHardHatText, this.currHardHatText);
        RefreshOrAddUpgrade("birdshield", "BirdShield", 50, 0, 1, currHardHat, 0, this.costBirdShieldText, this.currBirdShieldText);
    }

    /// <summary>
    /// Validates the preferences. Maybe a temporary thing?
    /// </summary>
    private void validatePrefLimits() {
        //int ropeLengthUps = PlayerPrefs.GetInt(Constants.RopeLengthPrefString);
        //int ropeSpeedUps = PlayerPrefs.GetInt(Constants.RopeSpeedPrefString);
        //int bouncinessUps = PlayerPrefs.GetInt(Constants.BouncinessPrefString);
        //int hookSpeedUps = PlayerPrefs.GetInt(Constants.HookSpeedPrefString);

        //if (ropeLengthUps > Constants.MaxRopeLengthUpgrades) ropeLengthUps = Constants.MaxRopeLengthUpgrades;
        //if (ropeSpeedUps > Constants.maxRopeSpeedUpgrades) ropeSpeedUps = Constants.maxRopeSpeedUpgrades;
        //if (bouncinessUps > Constants.maxBouncinessUpgrade) bouncinessUps = Constants.maxBouncinessUpgrade;
        //if (bouncinessUps > Constants.maxBouncinessUpgrade) hookSpeedUps = Constants.maxHookSpeedUpgrade;

        //PlayerPrefs.SetInt(Constants.RopeLengthPrefString, ropeLengthUps);
        //PlayerPrefs.SetInt(Constants.RopeSpeedPrefString, ropeSpeedUps);
        //PlayerPrefs.SetInt(Constants.BouncinessPrefString, bouncinessUps);
        //PlayerPrefs.SetInt(Constants.HookSpeedPrefString, hookSpeedUps);
    }

    public bool MakePurchase(string type) {
        int coins = PlayerPrefs.GetInt("Coins");
        int amountSpent = 0;
        int increase = 0;

        Upgrade upgrade = getUpgrade(type);

        increase = 1;
        amountSpent = upgrade.cost + upgrade.costIncr * upgrade.curr;

        //If we don't have enough coins, return false
        if (coins - amountSpent < 0)
            return false;

        //Increase our value, set it!
        int val = PlayerPrefs.GetInt(upgrade.prefName);
        int limit = upgrade.max;

        //If we are going past the limit, return false.
        if (val + increase > limit)
            return false;

        val += increase;
        PlayerPrefs.SetInt(upgrade.prefName, val);
        upgrade.curr = val;

        //Refresh the items/text
        PlayerPrefs.SetInt("Coins", coins - amountSpent);

        MainMenu.RefreshCoinAmount();
        this.UpdateUpgradeText(type);
        //this.loadItemData();

        return true;
    }

    /// <summary>
    /// Updates the Upgrades text to be displayed.
    /// </summary>
    /// <param name="upgradeName">The upgrade name.</param>
    private void UpdateUpgradeText(string upgradeName) {
        Upgrade upgrade = getUpgrade(upgradeName);
        upgrade.costText.text = "" + (upgrade.cost + upgrade.costIncr * upgrade.curr).ToString();
        upgrade.currText.text = PlayerPrefs.GetInt(upgrade.prefName) + "/" + upgrade.max;
    }

    public static Upgrade AddUpgrade(string name, string prefName, int cost, int costIncr, int max, int curr, float amtChange, Text costText, Text currText) {
        Upgrade upgrade = new Upgrade(prefName, cost, costIncr, max, curr, amtChange, costText, currText);
        upgradeMap.Add(name, upgrade);
        return upgrade;
    }

    public static Upgrade RefreshOrAddUpgrade(string name, string prefName, int cost, int costIncr, int max, int curr, float amtChange, Text costText, Text currText) {
        Upgrade upgrade;
        upgradeMap.TryGetValue(name, out upgrade);
        if (upgrade == null) {
            //If we didn't get it out of the dictionary, add a new one.
            upgrade = new Upgrade(prefName, cost, costIncr, max, curr, amtChange, costText, currText);
            upgradeMap.Add(name, upgrade);
        } else {
            //Gotta update these because they are new objects when the MainMenu is reloaded.
            upgrade.costText = costText;
            upgrade.currText = currText;
        }

        return upgrade;
    }

    public static Upgrade getUpgrade(string name) {
        Upgrade upgrade;
        upgradeMap.TryGetValue(name, out upgrade);
        return upgrade;
    }

    public static void maxAllUpgrades() {
        foreach (Upgrade upgrade in upgradeMap.Values) {
            upgrade.curr = upgrade.max;
            PlayerPrefs.SetInt(upgrade.prefName, upgrade.curr);
        }
    }

    public static void clearAllUpgrades() {
        foreach (Upgrade upgrade in upgradeMap.Values) {
            upgrade.curr = 0;
            PlayerPrefs.SetInt(upgrade.prefName, upgrade.curr);
        }
    }

    public static Dictionary<string, Upgrade> getDictionary() {
        return upgradeMap;
    }

    public class Upgrade {
        public string prefName;
        public int cost, costIncr, max, curr;
        public float amtChange;
        public Text costText, currText;

        public Upgrade(string prefName, int cost, int costIncr, int max, int curr, float amtChange, Text costText, Text currText) {
            this.prefName = prefName;
            this.cost = cost;
            this.costIncr = costIncr;
            this.max = max;
            this.curr = curr;
            this.amtChange = amtChange;

            //These link the physical text boxes (in the GUI) to the upgrades.
            this.costText = costText;
            this.currText = currText;
        }
    }
}
