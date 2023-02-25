using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PoseInterpolation : MonoBehaviour {

    float timeCount = 0.0f;
    bool doInterpolation = false;
    GameObject Initial, Last, Moving;
    Transform InitialRoot, LastRoot, MovingRoot;
    GameObject[] InitialJoints, LastJoints, MovingJoints;

    void Start() {
        // Get Poses
        Initial = GameObject.Find("Initial");
        Last = GameObject.Find("Last");
        Moving = GameObject.Find("Moving");

        // Get Roots & Root Position
        InitialRoot = Initial.transform.GetChild(2);
        LastRoot = Last.transform.GetChild(2);
        MovingRoot = Moving.transform.GetChild(2);

        // Get All Joints
        InitialJoints = GameObject.FindGameObjectsWithTag("InitialJoints");
        LastJoints = GameObject.FindGameObjectsWithTag("LastJoints");
        MovingJoints = GameObject.FindGameObjectsWithTag("MovingJoints");
    }

    void Update() {
        if (Input.GetKey(KeyCode.Escape)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKey(KeyCode.Space) && !doInterpolation) {
            doInterpolation = true;
        }

        // Start Pose Interpolation when IK is done
        if (doInterpolation) {
            RootInterpolation(); // Root Position Linear Interpolation
            JointInterpolation(); // Joint Local Rotation Interpolation
            timeCount += Time.deltaTime;
        }
    }

    private void RootInterpolation() {
        MovingRoot.position = Vector3.Lerp(
            InitialRoot.position, LastRoot.position, timeCount
        );
    }

    private void JointInterpolation() {
        for (int i = 0; i < 67; i++) {
            MovingJoints[i].transform.rotation = Quaternion.Slerp(
                InitialJoints[i].transform.rotation,
                LastJoints[i].transform.rotation,
                timeCount
            );
        }
    }
}
