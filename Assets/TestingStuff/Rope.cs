using UnityEngine;
using System.Collections;

public class Rope : MonoBehaviour {
    private float width, height;
    public Transform child = null;
    public GameObject sprite;
    public float distance;

    public new HingeJoint2D hingeJoint { get; private set; }

    public void Awake() {
        this.hingeJoint = GetComponent<HingeJoint2D>();
    }

    // Use this for initialization
    void Start() {
        if (transform.childCount > 1) {
            sprite = transform.GetChild(0).gameObject;
            child = transform.GetChild(1);
            if (!child.CompareTag("Rope Piece"))
                child = null;
        }

        if (child == null)
            this.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update() {
        if (child == null) return;

        distance = Vector3.Distance(transform.position, child.position);
        var angleRadians = Mathf.Atan2(transform.position.y - child.position.y, transform.position.x - child.position.x) + Mathf.PI;
        var x = Mathf.Cos(angleRadians) * distance;
        var y = Mathf.Sin(angleRadians) * distance;

        Debug.DrawLine(this.transform.position, child.transform.position, Color.red);

        //Set the position, the scale (to the wall), and the angle of the rope.
        sprite.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        sprite.transform.localScale = new Vector3(0.5f, distance, transform.lossyScale.z);

        sprite.transform.rotation = Quaternion.Euler(0, 0, angleRadians * Mathf.Rad2Deg - 90);
    }

    public void OnDrawGizmos() {
        Gizmos.DrawSphere(this.transform.position, 0.05f);
    }
}
