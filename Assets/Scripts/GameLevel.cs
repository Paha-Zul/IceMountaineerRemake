using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameLevel : MonoBehaviour {
    public enum LevelType { Normal, Angled, Double }
    public enum ArtType { Normal=0, Night=1 }

    public ArtType artType = ArtType.Normal;
    public Text _shardText, _birdText, coinsText, newScore, oldScore;
    public GameObject scorePanel, canvas;
    public GameObject leftWall, rightWall, shard, bird, spawns, underSpawns, leftBirdSpawn, rightBirdSpawn, highScorePanel; //Our prefabs
    public GameObject tapToStartText; //Our prefabs
    public GameObject birdWarning;
    public GameObject player;
    public float wallHeight;

    private GameObject wallCategory;

    public static LevelType levelType;
    public static bool paused { get; private set; }

    private static Text shardText, birdText;

    private List<Wall> walls = new List<Wall>();
    private static Player playerScript;
    private float horzExtent, vertExtent, nextIncrease, nextSpawn, spawnDelay;
    private float ropeSpeed, ropeLength, bounciness;

    private int shardsSpawnedSoFar = 0, birdsSpawnedSoFar;
    private float nextBirdSpawn, birdSpawnSpeed, birdSpawnChance = 0.5f;

    private static int overallScore = 0, shardsDodged = 0, birdsDodged = 0;

    private static float CoinCounter = 0, CoinMulti;

    public bool started { get; private set; }

    // Use this for initialization
    void Start () {
        this.wallCategory = GameObject.Find("Walls");

        configureArtStyle();

        //Initially set the time scale to 0...
        Time.timeScale = 0;

        //Reset score
        overallScore = shardsDodged = birdsDodged = 0;
        
        //Set wall height.
        Vector3 bounds = rightWall.GetComponent<SpriteRenderer>().bounds.size;
        wallHeight = bounds.y;

        GameLevel.shardText = _shardText;
        GameLevel.birdText = _birdText;

        GameLevel.playerScript = this.player.GetComponent<Player>();

        this.horzExtent = Camera.main.orthographicSize * Screen.width / Screen.height;
        this.vertExtent = Camera.main.orthographicSize * Screen.height / Screen.width;

        //Initially lay out all the walls from bottom to top.
        for (int i = 0; i < (vertExtent*2)/wallHeight + 2; i++)
        {
            var yValue = -this.vertExtent + ((wallHeight*0.98f) * i);

            var wallBelowLeft = walls.Count >= 2 ? walls[walls.Count - 2] : null; //Left wall (left always gets added first, ie: second to last index)
            var wallBelowRight = walls.Count >= 2 ? walls[walls.Count - 1] : null; //Right wall (right gets added second, ie: last index)

            Wall wallLeft = new Wall((GameObject)Instantiate(leftWall, new Vector3(-this.horzExtent, yValue, 0), Quaternion.identity), wallBelowLeft);
            Wall wallRight = new Wall((GameObject)Instantiate(rightWall, new Vector3(this.horzExtent, yValue, 0), Quaternion.identity), wallBelowRight);

            walls.Add(wallLeft);
            walls.Add(wallRight);

            wallLeft.wall.transform.parent = wallCategory.transform;
            wallRight.wall.transform.parent = wallCategory.transform;

            //If the wall below left and right aren't null, set the new walls to above them.
            if (wallBelowLeft != null)
                wallBelowLeft.wallAbove = wallLeft;

            if (wallBelowRight != null)
                wallBelowRight.wallAbove = wallRight;
        }

        paused = false;
        overallScore = 0;

        if(levelType == LevelType.Normal) CoinMulti = 0.5f;
        else if(levelType == LevelType.Angled) CoinMulti = 0.7f;
        else if(levelType == LevelType.Double) CoinMulti = 0.9f;

        this.spawnDelay = Constants.InitialShardSpawnSpeed;
        this.birdSpawnSpeed = Constants.InitialBirdSpawnSpeed;
        this.nextBirdSpawn = Time.time + Constants.InitialBirdSpawnSpeed;
    }

    /// <summary>
    /// Configures the art style.
    /// </summary>
    private void configureArtStyle() {
        leftWall.GetComponent<SpriteRenderer>().material = ArtManager.getArtSetResource<Material>("wallMat", (int)artType);
        rightWall.GetComponent<SpriteRenderer>().material = ArtManager.getArtSetResource<Material>("wallMat", (int)artType);

        shard.transform.GetChild(0).gameObject.SetActive(ArtManager.getArtSetShardLight((int)artType));
    }

    // Update is called once per frame
    void Update () {
        if (!paused){
            this.SpawnShards();
            this.spawnBirds();

            this.IncreaseShardSpawn();
        }
	}

    void FixedUpdate() {
        MoveWallsDown();

        //If the topmost walls are about to pass under the top of the screen, spawn more.
        if (walls[walls.Count-1].wall.transform.position.y <= this.vertExtent) {
            var yValue = this.vertExtent + wallHeight * 0.98f;

            var wallBelowLeft = walls.Count >= 2 ? walls[walls.Count - 2] : null; //Left wall (left always gets added first, ie: second to last index)
            var wallBelowRight = walls.Count >= 2 ? walls[walls.Count - 1] : null; //Right wall (right gets added second, ie: last index)

            Wall wallLeft = new Wall((GameObject)Instantiate(leftWall, new Vector3(-this.horzExtent, yValue, 0), Quaternion.identity), wallBelowLeft);
            Wall wallRight = new Wall((GameObject)Instantiate(rightWall, new Vector3(this.horzExtent, yValue, 0), Quaternion.identity), wallBelowRight);

            walls.Add(wallLeft);
            walls.Add(wallRight);

            wallLeft.wall.transform.parent = wallCategory.transform;
            wallRight.wall.transform.parent = wallCategory.transform;

            //If the wall below left and right aren't null, set the new walls to above them.
            if (wallBelowLeft != null)
                wallBelowLeft.wallAbove = wallLeft;

            if (wallBelowRight != null)
                wallBelowRight.wallAbove = wallRight;
        }
    }

    private void MoveWallsDown() {
        float wallSpeed;
        if (shardsSpawnedSoFar == 0) wallSpeed = Constants.WallSpeed;
        else wallSpeed = Constants.WallSpeed + (Constants.WallSpeedIncrease * (this.shardsSpawnedSoFar / Constants.WallSpeedIncreaseDodgedInterval));

        //Moves the walls down and removes them if they are below the screen.
        for (int i = 0; i < walls.Count; i++) {
            Wall wall = walls[i];

            if (wall.wallBelow == null) //If the wall below is null, move down.
                wall.wall.transform.Translate(Vector3.down * wallSpeed);
            else //Otherwise, attach to the top part of the wall below.
                wall.wall.transform.position = new Vector3(wall.wall.transform.position.x, wall.wallBelow.wall.transform.position.y + (wallHeight*0.99f));

            //GameObject wall = walls[i].wall;
            //wall.transform.Translate(Vector3.down * wallSpeed);

            if (wall.wall.transform.position.y + wallHeight < -this.vertExtent) {
                GameObject.Destroy(walls[i].wall);
                walls[i].wallAbove.wallBelow = null; //Clear ourselves from the above reference.
                walls.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Increases the spawn speed of the shard when the correct time is met.
    /// </summary>
    public void IncreaseShardSpawn() {
        if(Time.time > this.nextIncrease) {
            this.spawnDelay -= Constants.ShardSpawnTimeIncreaseAmount;
            this.nextIncrease = Time.time + Constants.ShardSpawnIncreaseIncreaseInterval;
        }
    }

    /// <summary>
    /// Spawns shards to fall when the time is met.
    /// </summary>
    public void SpawnShards()
    {
        if(Time.time > this.nextSpawn)
        {
            this.nextSpawn = Time.time + this.spawnDelay;

            GameObject spawnToUse;

            //If the double game type, choose if we should spawn above or below
            if(levelType != LevelType.Double)
                spawnToUse = this.spawns;
            else
                spawnToUse = Random.Range(0f, 1f) >= 0.5f ? underSpawns : spawns;

            //Spawn a shard!
            int numChilds = spawnToUse.transform.childCount;
            Vector2 spawn = spawnToUse.transform.GetChild((int)Random.Range(0f, numChilds - 0.1f)).transform.position;
            if (shardsSpawnedSoFar == 0) spawn = spawnToUse.transform.GetChild(spawnToUse.transform.childCount / 2).transform.position;
            GameObject newShard = Instantiate(this.shard, spawn, Quaternion.identity) as GameObject;

            //Set the name (and rotation if we are playing rotated game type)
            newShard.name = this.shard.name;
            if (levelType == LevelType.Angled) {
                newShard.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-40, 40));
                if (shardsSpawnedSoFar == 0)
                    newShard.transform.rotation = Quaternion.identity;

            }

            //Set some stuff.
            Shard shard = newShard.GetComponent<Shard>();
            shard.num = this.shardsSpawnedSoFar++;
            if(shard.transform.position.y <= 0) shard.rigidBody.gravityScale = -1;

            //If we are above the bird spawning limit and we are at the interval to increase bird spawn speed...
            if(shardsDodged > Constants.NumShardsPerBirdSpawnIncrease && shardsDodged % Constants.NumShardsPerBirdSpawnIncrease == 0){
                this.birdSpawnSpeed -= Constants.BirdSpawnIncreaseSpeed;
            }
        }
    }

    /// <summary>
    /// Spawns birds when the right conditions are met.
    /// </summary>
    public void spawnBirds() {
        if(Time.time > this.nextBirdSpawn && this.shardsSpawnedSoFar >= Constants.ShardsDodgedBeforeBirdSpawn) {
            this.nextBirdSpawn = Time.time + this.birdSpawnSpeed;

            if(Random.Range(0f, 1f) <= 0.5f) return;

            GameObject birdSpawn;
            bool left = false;
            if(Random.Range(0f, 1f) > 0.5) {
                birdSpawn = leftBirdSpawn;
                left = true;
            } else {
                birdSpawn = rightBirdSpawn;
            }

            StartCoroutine(SpawnBirdDelayed(birdSpawn, left));
        }
    }

    //Games over!
    public void GameOver(){
        paused = true;
        Time.timeScale = 0;

        //Get the current coins we have and add our score to them.
        int coins = PlayerPrefs.GetInt("Coins") ;
        int newCoins = (int)CoinCounter;
        PlayerPrefs.SetInt("Coins", coins + newCoins);

        //Move the game over screen to the center.
        RectTransform canvasTransform = this.canvas.GetComponent<RectTransform>();
        this.scorePanel.GetComponent<RectTransform>().position = new Vector2(canvasTransform.position.x, canvasTransform.position.y);

        //Set the text for score and coins.
        this.coinsText.text = newCoins.ToString();

        this.ShowHighScore();

        CoinCounter = 0;
    }

    public void ShowHighScore() {
        string boardName = "";
        string boardID = "";

        if (levelType == LevelType.Normal) {
            boardID = "CgkIzpi-qqMDEAIQBg";
            boardName = "Normal_Highscore";
        } else if (levelType == LevelType.Angled) {
            boardID = "CgkIzpi-qqMDEAIQCg";
            boardName = "Angled_Highscore";
        } else if (levelType == LevelType.Double) {
            boardID = "CgkIzpi-qqMDEAIQCw";
            boardName = "Double_Highscore";
        }

        //Get the previous highscore
        int previousHighscore = PlayerPrefs.GetInt(boardName);

        //Display the highscores
        this.oldScore.text = "" + previousHighscore;
        this.newScore.text = "" + overallScore;

        //If the new score is not higher than the old one, hide the "NEW HIGH SCORE!" text.
        if (overallScore <= previousHighscore)
            highScorePanel.transform.GetChild(0).gameObject.SetActive(false);
        else {
            //Set the new highscore
            PlayerPrefs.SetInt(boardName, overallScore);
        }

        //If we're authenticated, submit score!
        GPGFunctions.submitScore(overallScore, boardID);
    }

    /// <summary>
    /// Call when the player dodged a shard.
    /// </summary>
    public static void DodgedShard()
    {
        if (GameLevel.playerScript.isFalling()) {
            return;
        }

        GameLevel.shardsDodged++;
        
        shardText.text = GameLevel.shardsDodged.ToString();

        DodgedObstacle();
    }

    /// <summary>
    /// Call when the player dodged a bird.
    /// </summary>
    public static void DodgedBird()
    {
        if (GameLevel.playerScript.isFalling()) return;
        GameLevel.birdsDodged++;

        if (birdText != null)
            birdText.text = GameLevel.birdsDodged.ToString();

        DodgedObstacle();
    }

    private static void DodgedObstacle() {
        GameLevel.overallScore++;

        //Increase our coin amount. There are bonuses for the further into the level we are.
        float coinAmount = CoinMulti + (overallScore/Constants.ScoreThresholdCoinBonus)*Constants.ScoreCoinBonusMultiplier;
        CoinCounter += coinAmount;
    }

    /// <summary>
    /// Spawns a bird on a delay. The delay includes a blinking icon.
    /// </summary>
    /// <param name="birdSpawn"></param>
    /// <param name="left"></param>
    /// <returns></returns>
    IEnumerator SpawnBirdDelayed(GameObject birdSpawn, bool left)
    {
        //Get a child from the spawn group
        birdSpawn = birdSpawn.transform.GetChild((int)Random.Range(0, birdSpawn.transform.childCount - 0.1f)).gameObject;

        //Get the render for size stuff.
        SpriteRenderer renderer = this.birdWarning.GetComponent<SpriteRenderer>();

        //Set the X value.
        float x = horzExtent - renderer.bounds.extents.x;
        if (left) x = -horzExtent + renderer.bounds.extents.x;

        //Find the position to spawn the warning and flip if necessary.
        Vector3 spawnPos = new Vector3(x, birdSpawn.transform.position.y, birdSpawn.transform.position.z);
        GameObject birdWarningObj = Instantiate(this.birdWarning, spawnPos, Quaternion.identity) as GameObject;
        if (left)
            birdWarningObj.transform.localScale = new Vector3(-birdWarningObj.transform.localScale.x, 
                birdWarningObj.transform.localScale.y, birdWarningObj.transform.localScale.z);

        //Blink the warning!
        bool on = false;
        for(int i = 0; i < 6; i++){
            birdWarningObj.SetActive(on = !on);
            yield return new WaitForSeconds(0.3f);
        }

        Destroy(birdWarningObj); //Destroy the warning object.

        //Spawn the bird and fly!
        GameObject bird = Instantiate(this.bird, birdSpawn.transform.position, Quaternion.identity) as GameObject;
        bird.name = this.bird.name;
        Bird birdScript = bird.GetComponent<Bird>();
        birdScript.speed = 0.05f;
        birdScript.left = left;
    }

    public void setStarted() {
        Time.timeScale = 1f;
        this.started = true;
        this.tapToStartText.SetActive(false);
    }

    void OnApplicationQuit() {
        PlayerPrefs.SetInt("hash", SecretStuff.getHash2());
    }

    public void OnDestroy() {
        PlayerPrefs.SetInt("hash", SecretStuff.getHash2());
    }

    private class Wall {
        public Wall wallBelow, wallAbove;
        public GameObject wall;

        public Wall(GameObject wall, Wall wallBelow) {
            this.wall = wall;
            this.wallBelow = wallBelow;
        }
    }
}
