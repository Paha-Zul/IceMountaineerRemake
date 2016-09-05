using UnityEngine;

public class Player : MonoBehaviour {
    public GameObject hook;
    public GameLevel levelScript;
    public GameObject ropePrefab;
    public GameObject hardHat;
    public GameObject birdShield;
    public ParticleSystem emitter;
    public GameLevel gameLevelScript;
    public GameObject ropePrefab2;
    public float ropeLength, ropeSpeed, hookSpeed;

    private bool aTap;
    private Vector3 touchPos;
    private float distToHook, touchDownTime, holdDelay = 0.2f;
    private DistanceJoint2D joint;
    private Hook connectedHook;
    private GameObject rope;
    private MeshRenderer ropeRenderer;
    private Rigidbody2D rigidBody;

    private GameObject currRope;
    private Rope currRopeSegment;
    private Rope nextRopeSegment;
    private float ropeTransitionCounter;

	// Use this for initialization
	void Start () {
        //Ingore
        Physics2D.IgnoreLayerCollision(8, 9);

        this.joint = GetComponent<DistanceJoint2D>();
        this.rope = Instantiate(this.ropePrefab);
        this.ropeRenderer = this.rope.transform.GetChild(0).GetComponent<MeshRenderer>();
        this.emitter = GameObject.Find("ImpactParticles").GetComponent<ParticleSystem>();
        this.levelScript = Camera.main.GetComponent<GameLevel>();

        this.rigidBody = GetComponent<Rigidbody2D>();

        this.calculateBonuses();
    }

    /// <summary>
    /// Calculates bonuses for the player from upgrades.
    /// </summary>
    void calculateBonuses()
    {
        this.ropeSpeed = Constants.RopeSpeed + PlayerPrefs.GetInt("RopeSpeed") * UpgradeManager.getUpgrade("ropespeed").amtChange;
        this.ropeLength = Constants.RopeLength + PlayerPrefs.GetInt("RopeLength") * UpgradeManager.getUpgrade("ropelength").amtChange;
        this.hookSpeed = Constants.HookSpeed + PlayerPrefs.GetInt("HookSpeed") * UpgradeManager.getUpgrade("hookspeed").amtChange;

        if (PlayerPrefs.GetInt("HardHat") == 0) this.hardHat.SetActive(false);
        if (PlayerPrefs.GetInt("BirdShield") == 0) this.birdShield.SetActive(false);

        BoxCollider2D collider = this.GetComponent<BoxCollider2D>();

        UpgradeManager.Upgrade up = UpgradeManager.getUpgrade("bounciness");
        collider.sharedMaterial.bounciness = Constants.Bounciness + up.curr * UpgradeManager.getUpgrade("bounciness").amtChange;

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

    //Called when we should connect to the hook
    public void ConnectToHook(Hook hook)
    {
        //If the joint was destroyed, that means we are falling.
        if (this.joint != null)
        {
            this.transform.parent = null;
            if (currRope)
                Destroy(currRope);

            var masterRope = (Instantiate(ropePrefab2, new Vector3(hook.transform.position.x, hook.transform.position.y, hook.transform.position.z), Quaternion.identity) as GameObject).GetComponent<MasterRope>();
            this.connectedHook = hook;
            this.distToHook = Vector3.Distance(this.transform.position, hook.transform.position);

            var angleRadians = Mathf.Atan2(transform.position.y - hook.transform.position.y, transform.position.x - hook.transform.position.x);

            masterRope.SetAnchor(hook.gameObject);

            currRope = masterRope.gameObject;
            //currRope.transform.parent = hook.transform;
            masterRope.GetComponent<HingeJoint2D>().connectedBody = hook.GetComponent<Rigidbody2D>();

            //Set the angle and number of segments
            masterRope.angle = angleRadians*Mathf.Rad2Deg;
            masterRope.segments = (int)(distToHook / masterRope.segmentLength) + 1;

            //Init the rope, get the last rope, and add the player to the end of the rope.
            var ropes = masterRope.Init();
            masterRope.addAtEndOfRope(this.gameObject, ropes[ropes.Length-1]);

            currRopeSegment = ropes[ropes.Length - 1].GetComponent<Rope>();
            nextRopeSegment = currRopeSegment.transform.parent.GetComponent<Rope>();

            var x = Mathf.Cos(angleRadians) * distToHook;
            var y = Mathf.Sin(angleRadians) * distToHook;

            //this.joint.distance = this.distToHook;
            //this.joint.connectedBody = hook.GetComponent<Rigidbody2D>();
        }
    }

    //Reels in the hook.
    void ReelInHook()
    {
        var dst = Vector3.Distance(this.transform.position, currRope.transform.position);
        if(currRopeSegment && nextRopeSegment && dst > this.ropeLength) {
            this.ropeTransitionCounter += ropeSpeed;

            if(this.ropeTransitionCounter >= 1) {
                this.ropeTransitionCounter = 0;

                //Unhook the connected body.
                this.currRopeSegment.hingeJoint.connectedBody = null;

                //Save this to destroy later
                var destroying = currRopeSegment.gameObject;

                //Set the new curr and next rope segments
                this.currRopeSegment = this.nextRopeSegment;
                this.nextRopeSegment = this.currRopeSegment.transform.parent.GetComponent<Rope>();

                //Hook to the new rope segment
                this.currRopeSegment.hingeJoint.connectedBody = this.rigidBody;
                this.currRopeSegment.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;

                //Parent us to the new rope segment.
                this.transform.parent = this.nextRopeSegment.transform;

                //Destroy the old rope segment.
                Destroy(destroying);
            }else {
                this.transform.position = Vector3.Lerp(currRopeSegment.transform.position, nextRopeSegment.transform.position, ropeTransitionCounter);
            }
        }
    }

    void DrawRope(){
        
    }

    public bool isFalling(){
        return this.transform.parent == null;
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
                currRopeSegment.hingeJoint.connectedBody = null;
                this.transform.parent = null;
                Destroy(joint);
                this.connectedHook = null;
            }
        }
    }

    void OnBecameInvisible()
    {
        levelScript.GameOver();
        //Destroy(this.gameObject);
    }
}
