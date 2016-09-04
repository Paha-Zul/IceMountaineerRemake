using UnityEngine;
using System.Collections;

public class MasterRope : MonoBehaviour {

    public GameObject ropePrefab, weight;
    public int segments = 10;
    public float segmentLength  = 0.25f;
    public float angle = 0f;

    public void Awake() {
        GameObject lastRope = this.gameObject;
        var x = Mathf.Cos(angle * Mathf.Deg2Rad) * segmentLength;
        var y = Mathf.Sin(angle*Mathf.Deg2Rad) * segmentLength;

        for (int i = 0; i < segments; i++) {
            var newRope = Instantiate(ropePrefab, new Vector3(lastRope.transform.position.x + x, lastRope.transform.position.y + y, lastRope.transform.position.z), Quaternion.Euler(angle, 0f, 0f)) as GameObject;
            var lastHinge = lastRope.transform.GetComponent<HingeJoint2D>();
            var newHinge = newRope.GetComponent<HingeJoint2D>();

            //Connect them together.
            //lastHinge.connectedBody = newRope.GetComponent<Rigidbody>();
            lastHinge.connectedBody = newRope.GetComponent<Rigidbody2D>();

            //Set parent.
            newRope.transform.parent = lastRope.transform;

            //Set last rope
            lastRope = newRope;
        }

        var weightObj = Instantiate(weight);
        weightObj.transform.parent = lastRope.transform;
        weightObj.transform.localPosition = new Vector3(0f, 0f, 0f);

        var hinge = lastRope.transform.GetComponent<HingeJoint2D>();
        hinge.connectedBody = weightObj.GetComponent<Rigidbody2D>();
    }

    // Use this for initialization
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {

    }
}
