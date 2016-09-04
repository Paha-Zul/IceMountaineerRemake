using UnityEngine;

public class Player : MonoBehaviour {
    public GameObject hook;
    public GameLevel levelScript;
    public GameObject ropePrefab;
    public GameObject hardHat;
    public GameObject birdShield;
    public ParticleSystem emitter;
    public GameLevel gameLevelScript;
    public float ropeLength, ropeSpeed, hookSpeed;

    private bool aTap;
    private Vector3 touchPos;
    private float distToHook, touchDownTime, holdDelay = 0.2f;
    private DistanceJoint2D joint;
    private Hook connectedHook;
    private GameObject rope;
    private MeshRenderer ropeRenderer;

	// Use this for initialization
	void Start () {
        this.joint = GetComponent<DistanceJoint2D>();
        this.rope = Instantiate(this.ropePrefab);
        this.ropeRenderer = this.rope.transform.GetChild(0).GetComponent<MeshRenderer>();
        this.emitter = GameObject.Find("ImpactParticles").GetComponent<ParticleSystem>();
        this.levelScript = Camera.main.GetComponent<GameLevel>();

        this.calculateBonuses();
    }

    /// <summary>
    /// Calculates bonuses for the player from upgrades.
    /// </summary>
    void calculateBonuses()
    {
        this.ropeSpeed = Constants.RopeSpeed + PlayerPrefs.GetInt("RopeSpeed") * Constants.RopeSpeedIncrease;
        this.ropeLength = Constants.RopeLength + PlayerPrefs.GetInt("RopeLength") * Constants.RopeLengthDecrease;
        this.hookSpeed = Constants.HookSpeed + PlayerPrefs.GetInt("HookSpeed") * Constants.HookSpeedIncrease;

        if (PlayerPrefs.GetInt("HardHat") == 0) this.hardHat.SetActive(false);
        if (PlayerPrefs.GetInt("BirdShield") == 0) this.birdShield.SetActive(false);

        BoxCollider2D collider = this.GetComponent<BoxCollider2D>();

        UpgradeManager.Upgrade up = UpgradeManager.getUpgrade("bounciness");
        collider.sharedMaterial.bounciness = Constants.Bounciness + up.curr * Constants.BouncinessDecrease;

        //Apparently you need to toggle this for the material to change.
        collider.enabled = false;
        collider.enabled = true;
    }
	
	// Update is called once per frame
	void Update () {
        this.aTap = false;

        //If not Iphone or Android, use mouse button.
        if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.Android)
        {
            // use the input stuff
            this.aTap = Input.GetMouseButtonDown(0);
            this.touchPos = Input.mousePosition;
        }
        //Otherwise, use touch events.
        else
        {
            if (Input.touchCount > 0) //For future reference, if we try to get GetTouch(0) with a touch count of 0, it will mess up the game badly (weird things happen).
            {
                //Adnroid and Iphone
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                    this.touchDownTime = Time.time;
                if (Input.GetTouch(0).phase == TouchPhase.Ended && Time.time <= this.touchDownTime + holdDelay) //If we end the touch within the delay period, we have 'tapped'
                    this.aTap = true;
            }

            this.touchPos = Input.mousePosition;
        }

        //If we tapped, tap!
        if (this.aTap)
            this.OnScreenTap();

        //If we're not falling, reel in the hook and adjust the rope display.
        if (!isFalling()) {
            ReelInHook();
            DrawRope();

        //If we are falling, make the rope invisible!
        } else
            this.rope.transform.localScale = new Vector3(0, 0, 0);
    }

    //When the screen is tapped.
    void OnScreenTap() {
        //If we're not started, start!
        if (!this.gameLevelScript.started)
            this.gameLevelScript.setStarted();

        //Otherwise, proceed as normal.
        else {
            //Spawn the hook and get the hookComp
            GameObject newHook = Instantiate(hook, this.transform.position, Quaternion.identity) as GameObject;
            Hook newHookComp = newHook.GetComponent<Hook>();

            Vector3 playerPos = Camera.main.WorldToScreenPoint(this.transform.position); //Player screen position

            float angle = Mathf.Atan2(touchPos.y - playerPos.y, touchPos.x - playerPos.x); //Angle to mouse

            //Set some stuff.
            newHookComp.angle = angle;
            newHookComp.player = this;
            newHookComp.hookSpeed = this.hookSpeed;
            newHookComp.emitter = this.emitter;
        }
    }

    //Connects to a hook.
    public void ConnectToHook(Hook hook)
    {
        //If the joint was destroyed, that means we are falling.
        if (this.joint != null)
        {
            this.connectedHook = hook;
            this.distToHook = Vector3.Distance(this.transform.position, hook.transform.position);
            this.joint.distance = this.distToHook;
            this.joint.connectedBody = hook.GetComponent<Rigidbody2D>();
        }
    }

    //Reels in the hook.
    void ReelInHook()
    {
        if (this.joint.distance > this.ropeLength)
        {
            this.joint.distance -= this.ropeSpeed;
            if (this.joint.distance <= this.ropeLength) this.joint.distance = this.ropeLength;
        }
    }

    void DrawRope(){
        GameObject r = this.rope;
        Vector2 connAnchor = this.joint.connectedBody == null ? this.joint.connectedAnchor : this.joint.connectedBody.position;
        float disToHook = this.joint.distance; //Initially set as joint distance
        if(this.joint.connectedBody != null) //If the connect body is not null, get the distance to that.
            disToHook = Vector3.Distance(this.joint.connectedBody.transform.position, this.transform.position);

        //Set the position, the scale (to the wall), and the angle of the rope.
        r.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        r.transform.localScale = new Vector3(r.transform.localScale.x, disToHook, r.transform.localScale.z);

        float angle = Mathf.Atan2(connAnchor.y - r.transform.position.y, connAnchor.x - r.transform.position.x); //Angle to mouse
        r.transform.rotation = Quaternion.Euler(0, 0, angle*Mathf.Rad2Deg - 90);

        //Here we set the material tile scaling to match the rope length.
        Vector2 scale = ropeRenderer.material.mainTextureScale;
        scale.y = r.transform.localScale.y;
        ropeRenderer.material.mainTextureScale = scale;
    }

    public bool isFalling(){
        return this.joint == null;
    }

    void OnTriggerEnter2D(Collider2D coll){
        bool isShard = coll.gameObject.CompareTag("Shard");
        bool isBird = coll.gameObject.CompareTag("Bird");
        if (isShard || isBird){
            if(isShard && this.hardHat.activeSelf) { 
                //If we have a hard hat, break it but don't kill us!
                this.hardHat.SetActive(false);
                this.GetComponent<AudioSource>().Play();
                PlayerPrefs.SetInt("HardHat", 0);
            }else if(isBird && this.birdShield.activeSelf) {
                //If we have a bird shield, don't kill us!
                this.birdShield.SetActive(false);
                this.GetComponent<AudioSource>().Play();
                PlayerPrefs.SetInt("BirdShield", 0);
                coll.gameObject.SendMessage("Kill");
            } else {
                //Otherwise, kill us!
                DistanceJoint2D joint = this.GetComponent<DistanceJoint2D>();
                Destroy(joint);
                this.connectedHook = null;
            }
        }
    }

    void OnBecameInvisible()
    {
        levelScript.GameOver();
        Destroy(this.gameObject);
    }
}
