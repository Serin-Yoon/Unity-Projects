using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseKinematics : MonoBehaviour {

    GameObject Initial, Moving, Last;

    Transform InitialLeftArm, InitialLeftForeArm, InitialLeftHand;
    Transform InitialRightArm, InitialRightForeArm, InitialRightHand;
    Transform InitialLeftUpLeg, InitialLeftLeg, InitialLeftFoot;
    Transform InitialRightUpLeg, InitialRightLeg, InitialRightFoot;

    Transform MovingLeftArm, MovingLeftForeArm, MovingLeftHand;
    Transform MovingRightArm, MovingRightForeArm, MovingRightHand;
    Transform MovingLeftUpLeg, MovingLeftLeg, MovingLeftFoot;
    Transform MovingRightUpLeg, MovingRightLeg, MovingRightFoot;

    Transform LastLeftArm, LastLeftForeArm, LastLeftHand;
    Transform LastRightArm, LastRightForeArm, LastRightHand;
    Transform LastLeftUpLeg, LastLeftLeg, LastLeftFoot;
    Transform LastRightUpLeg, LastRightLeg, LastRightFoot;

    GameObject InitialLeftArmTarget;
    GameObject InitialRightArmTarget;
    GameObject InitialLeftLegTarget;
    GameObject InitialRightLegTarget;

    GameObject LastLeftArmTarget;
    GameObject LastRightArmTarget;
    GameObject LastLeftLegTarget;
    GameObject LastRightLegTarget;

    void Start() {
        Initial = GameObject.Find("Initial");
        Last = GameObject.Find("Last");
        Moving = GameObject.Find("Moving");

        // Find Joints & Targets
        (InitialLeftArm, InitialLeftForeArm, InitialLeftHand) = FindArmJoints(Initial, 0);
        (InitialRightArm, InitialRightForeArm, InitialRightHand) = FindArmJoints(Initial, 2);
        (InitialLeftUpLeg, InitialLeftLeg, InitialLeftFoot) = FindLegJoints(Initial, 0);
        (InitialRightUpLeg, InitialRightLeg, InitialRightFoot) = FindLegJoints(Initial, 1);

        (MovingLeftArm, MovingLeftForeArm, MovingLeftHand) = FindArmJoints(Moving, 0);
        (MovingRightArm, MovingRightForeArm, MovingRightHand) = FindArmJoints(Moving, 2);
        (MovingLeftUpLeg, MovingLeftLeg, MovingLeftFoot) = FindLegJoints(Moving, 0);
        (MovingRightUpLeg, MovingRightLeg, MovingRightFoot) = FindLegJoints(Moving, 1);

        (LastLeftArm, LastLeftForeArm, LastLeftHand) = FindArmJoints(Last, 0);
        (LastRightArm, LastRightForeArm, LastRightHand) = FindArmJoints(Last, 2);
        (LastLeftUpLeg, LastLeftLeg, LastLeftFoot) = FindLegJoints(Last, 0);
        (LastRightUpLeg, LastRightLeg, LastRightFoot) = FindLegJoints(Last, 1);

        InitialLeftArmTarget = GameObject.Find("InitialLeftArm");
        InitialRightArmTarget = GameObject.Find("InitialRightArm");
        InitialLeftLegTarget = GameObject.Find("InitialLeftLeg");
        InitialRightLegTarget = GameObject.Find("InitialRightLeg");

        LastLeftArmTarget = GameObject.Find("LastLeftArm");
        LastRightArmTarget = GameObject.Find("LastRightArm");
        LastLeftLegTarget = GameObject.Find("LastLeftLeg");
        LastRightLegTarget = GameObject.Find("LastRightLeg");
    }

    void Update() {
        IK(InitialLeftArm, InitialLeftForeArm, InitialLeftHand, InitialLeftArmTarget);
        IK(InitialRightArm, InitialRightForeArm, InitialRightHand, InitialRightArmTarget);
        IK(InitialLeftUpLeg, InitialLeftLeg, InitialLeftFoot, InitialLeftLegTarget);
        IK(InitialRightUpLeg, InitialRightLeg, InitialRightFoot, InitialRightLegTarget);

        IK(MovingLeftArm, MovingLeftForeArm, MovingLeftHand, InitialLeftArmTarget);
        IK(MovingRightArm, MovingRightForeArm, MovingRightHand, InitialRightArmTarget);
        IK(MovingLeftUpLeg, MovingLeftLeg, MovingLeftFoot, InitialLeftLegTarget);
        IK(MovingRightUpLeg, MovingRightLeg, MovingRightFoot, InitialRightLegTarget);

        IK(LastLeftArm, LastLeftForeArm, LastLeftHand, LastLeftArmTarget);
        IK(LastRightArm, LastRightForeArm, LastRightHand, LastRightArmTarget);
        IK(LastLeftUpLeg, LastLeftLeg, LastLeftFoot, LastLeftLegTarget);
        IK(LastRightUpLeg, LastRightLeg, LastRightFoot, LastRightLegTarget);
    }

    private (Transform a, Transform b, Transform c) FindArmJoints(GameObject parent, int index) {
        Transform a, b, c;
        a = parent.transform.GetChild(2).GetChild(2).GetChild(0).GetChild(0).GetChild(index).GetChild(0); // Left: 0, Right: 2
        b = a.GetChild(0);
        c = b.GetChild(0);
        return (a, b, c);
    }

    private (Transform a, Transform b, Transform c) FindLegJoints(GameObject parent, int index) {
        Transform a, b, c;
        a = parent.transform.GetChild(2).GetChild(index); // Left: 0, Right: 1
        b = a.GetChild(0);
        c = b.GetChild(0);
        return (a, b, c);
    }

    private void IK(Transform a, Transform b, Transform c, GameObject t) {
        /* Step 1: Extend/Contract the joint chain */
        float eps = 0.01f;
        float lab = Vector3.Distance(b.position, a.position);
        float lcb = Vector3.Distance(b.position, c.position);
        float lat = Mathf.Clamp(Vector3.Distance(t.transform.position, a.position), eps, lab+lcb-eps);

        float ac_ab_0 = Mathf.Acos(Mathf.Clamp(
            Vector3.Dot((c.position-a.position).normalized, (b.position-a.position).normalized), -1, 1));
        float ba_bc_0 = Mathf.Acos(Mathf.Clamp(
            Vector3.Dot((a.position-b.position).normalized, (c.position-b.position).normalized), -1, 1));

        float ac_ab_1 = Mathf.Acos(Mathf.Clamp((lcb*lcb-lab*lab-lat*lat)/(-2*lab*lat), -1, 1));
        float ba_bc_1 = Mathf.Acos(Mathf.Clamp((lat*lat-lab*lab-lcb*lcb)/(-2*lab*lcb), -1, 1));

        // radian to degree
        ac_ab_0 = ac_ab_0 * 180 / Mathf.PI;
        ba_bc_0 = ba_bc_0 * 180 / Mathf.PI;
        ac_ab_1 = ac_ab_1 * 180 / Mathf.PI;
        ba_bc_1 = ba_bc_1 * 180 / Mathf.PI;
        
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
