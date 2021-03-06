﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limb : MonoBehaviour {
    //the list of all the limb parts.
    private List<LimbPart> parts = new List<LimbPart> ();

    // the end Root, with means all out position would be relative to this transform,
    // also we would not include root as part of the limb.
    public Transform root;

    // the target pos we wanna rotate to.
    public Vector2 resetPosition;

    // force to controll how strong/fast the limb would be (its wierd).
    public float force = 100;
    [Range (0f, 1f)]
    // ratio !Just keep it HIGH!, at ratio 1 you would not be able to set new targetPos,
    // at ratio 0, you would get really unstable limbs.
    public float ratio = .9f;

    // in case the limb reaching the reversed direction
    public bool fliped = false;

    // in case the limb is facing the wrong direction with 90 degree Error.
    public bool sin = false;

    [Space]
    public bool resting = false;

    //1public LimbAnimator animator;

    void Start () {
        //Get all the Hinges that are connceted to each other,
        //untill there is none, or the limb has no hingeJoint.
        var hinge = GetComponent<HingeJoint2D> ();
        while (hinge != null && hinge.transform != root) {
            parts.Add (new LimbPart (hinge.attachedRigidbody, fliped, sin));
            hinge = hinge.connectedBody.GetComponent<HingeJoint2D> ();
        }
        //?? not really needed. in this case parts[0] would be the Parent of the limb, the actuall "root".
        parts.Reverse ();
    }

    [Range (1f, 25f)]
    public float addedForce = 1f;
    public void SetPosition (Vector2 targetPos, float addedForce = 1f) {
        currentPos = targetPos + (Vector2) root.position;
        this.addedForce = addedForce;
        resetPos = false;
    }

    // Update is called once per frame
    [SerializeField]
    bool resetPos = true;
    Vector2 currentPos;
    void FixedUpdate () {
        var force = this.force;
        if (resetPos) {
            currentPos = resetPosition + (Vector2) root.position;
            if (resting) {
                addedForce = 1;
                force *= 0.1f;
            }
        } else {
            resetPos = true;
        }
        // Actually, rotate each part to the target.
        // Add more force to the "parent" parts, because they simply need more force, to ignore the upper parts, swings.

        var index = parts.Count;
        foreach (var part in parts) {
            part.RotateToward (currentPos, force * (index * (index + 1) * .5f * addedForce), ratio);
            index--;
        }
        addedForce = 1f;
    }

    private void OnDrawGizmos () {
        // Draw gizmos for debug.
        foreach (var p in parts) {
            p.OnDrawGizmos ();
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere (resetPosition + (Vector2) root.position, .1f);
    }
}