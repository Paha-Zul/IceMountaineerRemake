using UnityEngine;
using System.Collections;

public class Bird : MonoBehaviour {
    public Sprite birdUp, birdDown, birdDead;

    public bool left { get; set; }
    public float speed { get; set; }

    private bool dead = false;
    private readonly float flipTime = 0.2f;
    private float nextFlipTime = 0;
    private bool up = false;

    private SpriteRenderer spriteRenderer;

    // Use this for initialization
    void Start() {
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();

        if(!left) {
            this.transform.localScale = new Vector3(-this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);
            this.speed = -this.speed;
        }
    }

    // Update is called once per frame
    void Update() {
        if (!GameLevel.paused && !this.dead) {
            this.transform.Translate(new Vector3(speed, 0, 0));

            if(Time.time >= this.nextFlipTime) {
                this.nextFlipTime = Time.time + this.flipTime;
                this.up = !this.up;
                if (up) this.spriteRenderer.sprite = birdUp;
                else this.spriteRenderer.sprite = birdDown;
            }
        }
    }

    public void OnBecameInvisible() {
        GameLevel.DodgedBird();
        Destroy(this.gameObject);
    }

    public void Kill() {
        this.dead = true;
        this.GetComponent<Rigidbody2D>().isKinematic = false;
        this.spriteRenderer.sprite = birdDead;
        this.GetComponent<Collider2D>().enabled = false;
    }
}
