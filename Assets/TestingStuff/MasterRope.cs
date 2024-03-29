﻿using UnityEngine;
using System.Collections;

public class MasterRope : MonoBehaviour {

    public bool testing = false;
    public GameObject ropePrefab, weight;
    public GameObject anchor { get; private set; }
    public int segments = 10;
    public float segmentLength  = 0.25f;
    public float angle = 0f;

    public HingeJoint2D bottomHingeJoint { get; private set; }
    public HingeJoint2D topHingeJoint { get; private set; }

    public void Awake() {
        if (testing) Init();
        var joints = GetComponents<HingeJoint2D>();
        this.bottomHingeJoint = joints[0];
        this.topHingeJoint = joints[1];
    }

    // Use this for initialization
    void Start() {
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>The last rope piece created.</returns>
    public GameObject[] Init() {
        GameObject lastRope = this.gameObject;
        var x = Mathf.Cos(angle * Mathf.Deg2Rad) * segmentLength;
        var y = Mathf.Sin(angle * Mathf.Deg2Rad) * segmentLength;

        var ropes = new GameObject[segments];

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

            ropes[i] = newRope;
        }

        if (testing) addWeight(lastRope);

        return ropes;
    }

    void Update() {
        //this.topHingeJoint.anchor = new Vector2(anchor.transform.position.x, anchor.transform.position.y);
    }

    void addWeight(GameObject lastRope) {
        var weightObj = Instantiate(weight);
        weightObj.transform.parent = lastRope.transform;
        weightObj.transform.localPosition = new Vector3(0f, 0f, 0f);
        //weightObj.GetComponent<MeshRenderer>().enabled = false;

        var hinge = lastRope.transform.GetComponent<HingeJoint2D>();
        hinge.connectedBody = weightObj.GetComponent<Rigidbody2D>();
    }

    public void SetAnchor(GameObject anchor) {
        this.anchor = anchor;
        this.topHingeJoint.connectedBody = anchor.GetComponent<Rigidbody2D>();
    }

    public void addAtEndOfRope(GameObject thingToAdd, GameObject lastRope) {
        thingToAdd.transform.parent = lastRope.transform;
        thingToAdd.transform.localPosition = new Vector3(0f, 0f, 0f);

        var hinge = lastRope.transform.GetComponent<HingeJoint2D>();
        hinge.connectedBody = thingToAdd.GetComponent<Rigidbody2D>();
    }
}
