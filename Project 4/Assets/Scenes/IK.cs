using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK : MonoBehaviour {

    Transform RightUpLeg, RightLeg, RightFoot;
    GameObject target;

    void Start() {
        // Get RightUpLeg, RightLeg, RightFoot
        RightUpLeg = transform.GetChild(2).GetChild(1);
        RightLeg = RightUpLeg.GetChild(0);
        RightFoot = RightLeg.GetChild(0);
        // Set Target position
        target = GameObject.Find("Target");
        target.transform.position = new Vector3(0.0f, 0.05f, 0.5f);
    }

    void Update() {
        MoveTarget();
        InverseKinematics();
    }

    private void MoveTarget() {
        if (Input.GetKey(KeyCode.LeftArrow)) target.transform.Translate(0.01f, 0, 0);
        if (Input.GetKey(KeyCode.RightArrow)) target.transform.Translate(-0.01f, 0, 0);
        if (Input.GetKey(KeyCode.UpArrow)) target.transform.Translate(0, 0, -0.01f);
        if (Input.GetKey(KeyCode.DownArrow)) target.transform.Translate(0, 0, 0.01f);
        if (Input.GetKey(KeyCode.W)) target.transform.Translate(0, 0.01f, 0);
        if (Input.GetKey(KeyCode.S)) target.transform.Translate(0, -0.01f, 0);
    }

    // Ref: https://theorangeduck.com/page/simple-two-joint
    private void InverseKinematics() {
        /* Step 1: Extend/Contract the Joint Chain */
        float eps = 0.01f;
        float lab = Vector3.Distance(RightLeg.position, RightUpLeg.position);
        float lcb = Vector3.Distance(RightLeg.position, RightFoot.position);
        float lat = Mathf.Clamp(Vector3.Distance(target.transform.position, RightLeg.position), eps, lab+lcb-eps);

        float ac_ab_0 = Mathf.Acos(Mathf.Clamp(
            Vector3.Dot((RightFoot.position-RightUpLeg.position).normalized, (RightLeg.position-RightUpLeg.position).normalized), -1, 1));
        float ba_bc_0 = Mathf.Acos(Mathf.Clamp(
            Vector3.Dot((RightUpLeg.position-RightLeg.position).normalized, (RightFoot.position-RightLeg.position).normalized), -1, 1));

        float ac_ab_1 = Mathf.Acos(Mathf.Clamp((lcb*lcb-lab*lab-lat*lat)/(-2*lab*lat), -1, 1));
        float ba_bc_1 = Mathf.Acos(Mathf.Clamp((lat*lat-lab*lab-lcb*lcb)/(-2*lab*lcb), -1, 1));

        Vector3 axis0 = Vector3.Cross(RightFoot.position-RightUpLeg.position, RightLeg.position-RightUpLeg.position).normalized;
        Quaternion r0 = Quaternion.AngleAxis(ac_ab_1-ac_ab_0, Quaternion.Inverse(RightUpLeg.rotation)*axis0);
        Quaternion r1 = Quaternion.AngleAxis(ba_bc_1-ba_bc_0, Quaternion.Inverse(RightLeg.rotation)*axis0);
        
        RightUpLeg.localRotation = RightUpLeg.localRotation * r0;
        RightLeg.localRotation = RightLeg.localRotation * r1;

        /* Step 2: Rotate the RightFoot into Target */
        float ac_at_0 = Mathf.Acos(Mathf.Clamp(
            Vector3.Dot((RightFoot.position-RightUpLeg.position).normalized, (target.transform.position-RightUpLeg.position).normalized), -1, 1));
        Vector3 axis1 = Vector3.Cross(RightFoot.position-RightUpLeg.position, target.transform.position-RightUpLeg.position).normalized;
        Quaternion r2 = Quaternion.AngleAxis(ac_at_0, Quaternion.Inverse(RightUpLeg.rotation)*axis1);
        
        RightUpLeg.localRotation = RightUpLeg.localRotation * r2;
    }
}
