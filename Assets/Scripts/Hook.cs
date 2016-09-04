using UnityEngine;
using System.Collections;

public class Hook : MonoBehaviour {
    public Player player;
    public float angle, windSpeed;
    public float hookSpeed { set; get; }
    public ParticleSystem emitter { set; get; }

    [HideInInspector]
    public Rigidbody2D rigidBody;
    private float dirX, dirY;

	// Use this for initialization
	void Start () {
        this.rigidBody = this.gameObject.GetComponent<Rigidbody2D>();
        this.dirX = (Mathf.Cos(angle)*Mathf.Rad2Deg) * hookSpeed;
        this.dirY = (Mathf.Sin(angle)*Mathf.Rad2Deg) * hookSpeed;

        if(dirX < 0) this.transform.localScale = new Vector3(-this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);

        this.GetComponent<Rigidbody2D>().velocity = new Vector3(dirX, dirY, 0);
        Physics2D.IgnoreCollision(player.GetComponent<BoxCollider2D>(), this.GetComponent<CircleCollider2D>());
        Physics2D.IgnoreLayerCollision(8, 8); 
    }
	
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Wall")) {
            this.player.ConnectToHook(this);
            this.transform.parent = coll.transform;
            this.GetComponent<Rigidbody2D>().isKinematic = true;
            this.GetComponent<AudioSource>().Play();
            this.emitter.Play();
            this.emitter.gameObject.transform.position = this.transform.position;
            if (this.transform.localScale.x < 0)
                this.emitter.transform.rotation = Quaternion.Euler(120, -90, -270);
            else
                this.emitter.transform.rotation = Quaternion.Euler(60, -90, -270);
        }
    }

    void OnBecameInvisible(){
        //In the rare case that we connect to a wall and go invisible, don't go nuts and destroy the hook
        //This is mainly when you aim to high and the hook barely goes off the top of the screen.
        if(this.transform.parent == null)
            Destroy(this.gameObject);
    }
}
