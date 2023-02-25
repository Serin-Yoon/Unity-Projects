using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK : MonoBehaviour {

    Transform RightUpLeg, RightLeg, RightFoot;
    Transform LeftArm, LeftForeArm, LeftHand;
    GameObject RightLegTarget;
    GameObject LeftArmTarget;

    void Start() {
        // Get RightUpLeg, RightLeg, RightFoot
        RightUpLeg = transform.GetChild(2).GetChild(1);
        RightLeg = RightUpLeg.GetChild(0);
        RightFoot = RightLeg.GetChild(0);

        // Set Right Leg Target position
        RightLegTarget = GameObject.Find("RightLegTarget");
        RightLegTarget.transform.position = new Vector3(0.0f, 0.05f, 0.5f);

        // Get LeftArm, LeftForeArm, LeftHand
        LeftArm = transform.GetChild(2).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0);
        LeftForeArm = LeftArm.GetChild(0);
        LeftHand = LeftForeArm.GetChild(0);

        // Set Left Arm Target position
        LeftArmTarget = GameObject.Find("LeftArmTarget");
        LeftArmTarget.transform.position = new Vector3(-1.0f, 1.5f, 0.5f);

        Debug.Log(LeftArm.name);
        Debug.Log(LeftForeArm.name);
        Debug.Log(LeftHand.name);
    }

    void Update() {
        InverseKinematics(RightUpLeg, RightLeg, RightFoot, RightLegTarget);
        InverseKinematics(LeftArm, LeftForeArm, LeftHand, LeftArmTarget);
    }

    /* 
        Inverse Kinematics
        Ref: https://theorangeduck.com/page/simple-two-joint
    */

    private void InverseKinematics(Transform a, Transform b, Transform c, GameObject t) {
        /* Step 1: Extend/Contract the joint chain */
        float eps = 0.01f;
        float lab = Vector3.Distance(b.position, a.position);
        float lcb = Vector3.Distance(b.position, c.position);
        float lat = Mathf.Clamp(Vector3.Distance(t.transform.position, b.position), eps, lab+lcb-eps);

        float ac_ab_0 = Mathf.Acos(Mathf.Clamp(
            Vector3.Dot((c.position-a.position).normalized, (b.position-a.position).normalized), -1, 1));
        float ba_bc_0 = Mathf.Acos(Mathf.Clamp(
            Vector3.Dot((a.position-b.position).normalized, (c.position-b.position).normalized), -1, 1));

        float ac_ab_1 = Mathf.Acos(Mathf.Clamp((lcb*lcb-lab*lab-lat*lat)/(-2*lab*lat), -1, 1));
        float ba_bc_1 = Mathf.Acos(Mathf.Clamp((lat*lat-lab*lab-lcb*lcb)/(-2*lab*lcb), -1, 1));

        Vector3 axis0 = Vector3.Cross(c.position-a.position, b.position-a.position).normalized;
        Quaternion r0 = Quaternion.AngleAxis(ac_ab_1-ac_ab_0, Quaternion.Inverse(a.rotation)*axis0);
        Quaternion r1 = Quaternion.AngleAxis(ba_bc_1-ba_bc_0, Quaternion.Inverse(b.rotation)*axis0);
        
        a.localRotation = a.localRotation * r0;
        b.localRotation = b.localRotation * r1;

        /* Step 2: Rotate the end effector into target */
        float ac_at_0 = Mathf.Acos(Mathf.Clamp(
            Vector3.Dot((c.position-a.position).normalized, (t.transform.position-a.position).normalized), -1, 1));
        Vector3 axis1 = Vector3.Cross(c.position-a.position, t.transform.position-a.position).normalized;
        Quaternion r2 = Quaternion.AngleAxis(ac_at_0, Quaternion.Inverse(a.rotation)*axis1);
        
        a.localRotation = a.localRotation * r2;
    }
}
