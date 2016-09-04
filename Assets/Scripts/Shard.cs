using UnityEngine;
using System.Collections;

public class Shard : MonoBehaviour {
    public int num { get; set; }

    public Rigidbody2D rigidBody { get; set; }

    private float startTime;
    private float speed = 2;
    private bool started = false;
    private Vector3 originalPosition;

    void Awake() {
        this.rigidBody = GetComponent<Rigidbody2D>();
        this.rigidBody.isKinematic = true;
    }

    // Use this for initialization
    void Start () {
        //Get the rigibody, initially set to kinematic, set start time.
        this.originalPosition = this.transform.position;
        this.startTime = Time.time + 1;
	}
	
	// Update is called once per frame
	void Update () {
	    if(!this.started && Time.time > this.startTime){
            //If we are not started but our time is ready!
            this.started = true;
            if(GameLevel.levelType == GameLevel.LevelType.Normal || GameLevel.levelType == GameLevel.LevelType.Double) this.rigidBody.isKinematic = false;
            else {
                float bonusSpeed = 0.001f * num;
                float x = Mathf.Cos(this.transform.rotation.z+(-90*Mathf.Deg2Rad)) * (this.speed + bonusSpeed);
                float y = Mathf.Sin(this.transform.rotation.z+(90*Mathf.Deg2Rad)) * -(this.speed + bonusSpeed);
                this.rigidBody.velocity = new Vector2(x, y);
            }
            this.transform.position = this.originalPosition;

        }else if (!this.started){
            //If we are not started and we're not ready to start yet.
            shake();
        } else {
            //If we are started...
            
        }

	}

    void shake(){
        this.transform.position = new Vector3(Random.Range(-0.05f, 0.05f) + this.originalPosition.x, this.originalPosition.y, this.originalPosition.z);
    }

    void OnBecameInvisible()
    {
        //When it falls of the screen, increase score and destroy the game object.
        GameLevel.DodgedShard();
        Destroy(this.gameObject);
    }
}
