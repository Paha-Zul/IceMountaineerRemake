using UnityEngine;
using System.Collections;

public class Constants  {
    public const int VideoRewardAmount = 25;

    public const string CoinPrefString = "Coins";
    public const string RopeLengthPrefString = "RopeLength";
    public const string RopeSpeedPrefString = "RopeSpeed";
    public const string BouncinessPrefString = "Bounciness";
    public const string HookSpeedPrefString = "HookSpeed";

    public const float RopeLength = 4f;
    public const float RopeSpeed = 0.05f;
    public const float Bounciness = 0.3f;
    public const float RopeLengthDecrease = -0.1f, RopeSpeedIncrease = 0.05f, BouncinessDecrease = -0.01f, HookSpeedIncrease = 0.01f;
    public const float WallSpeed = 0.01f, WallSpeedIncrease = 0.0025f, WallSpeedIncreaseDodgedInterval = 5;
    public const float HookSpeed = 0.2f;

    public const float InitialShardSpawnSpeed = 3f; //Initial spawn rate of ice shards
    public const float ShardSpawnIncreaseIncreaseInterval = 8f; //Increases the spawn interval every x amount of time (in seconds)
    public const float ShardSpawnTimeIncreaseAmount = 0.1f; //Amount of time to decrease the spawn speed.

    public const int ShardsDodgedBeforeBirdSpawn = 15;
    public const float InitialBirdSpawnSpeed = 5;
    public const float NumShardsPerBirdSpawnIncrease = 5;
    public const float BirdSpawnIncreaseSpeed = 0.2f;

    public const int RopeSpeedCost = 10, RopeLengthCost = 10, BouncinessCost = 10, HookSpeedCost = 10, HardHatCost = 50;
    public const int RopeSpeedCostIncr = 1, RopeLengthCostIncr = 1, BouncinessCostIncr = 1, HookSpeedCostIncr = 1, HardHatCostIncr = 10;
    public const int maxRopeSpeedUpgrades = 10, MaxRopeLengthUpgrades = 20, maxBouncinessUpgrade = 10, maxHookSpeedUpgrade = 10, maxHardHatUpgrade = 1;

    public const int ScoreThresholdCoinBonus = 15;
    public const float ScoreCoinBonusMultiplier = 1.5f;

    public const int NumTimesToMainMenuForInter = 5;

}
