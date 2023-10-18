using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SG;
using static SG.SG_HandPose;


public class getRightAngles : MonoBehaviour
{
    //float [] everyFlexion = SG_HandPose.TotalFlexions;
    //bool isRightHand = SG_HapticGlove.IsRight;
    //private Vector3[][] jointAngles;
    //float[] indexFlexR;
    //double R2D = 180/Mathf.PI;
    public Vector3[][] jointAngles;

        /// <summary> The quaternion rotation of each joint, relative to a Wrist Transform: JointRotation * WristRotation = 3D Rotation. 
        /// The first index [0..4] determines the finger (thumb..pinky), while the second [0..2]  determines joint (CMC, MCP, IP for thumb. MCP, PIP, DIP for fingers.) </summary>
    public Quaternion[][] jointRotations;

        /// <summary> The position of each joint, in meters, relative to a Wrist Transform: (JointPosition * WristRotation) + WristPosition = 3D Position. 
        /// The first index [0..4] determines the finger (thumb..pinky), while the second [0..2]  determines joint (CMC, MCP, IP for thumb. MCP, PIP, DIP for fingers.) </summary>
    public Vector3[][] jointPositions;

        /// <summary> The total flexion of each finger, normalized to values between 0 (fingers fully extended) and 1 (fingers fully flexed). 
        /// The index [0..4] determines the finger (thumb..pinky). </summary>
    public float[] normalizedFlexion;

    void Start()
    {
        
    }

    
    // Update is called once per frame
    void Update()
    {
        
        //indexFlex = sums jointAngles 0-2 that belong to finger 1 (index)
        // for (int i=0; i>=2; i++)
        // {
        //     indexFlexR += fingerAngles.jointAngles[1][i];
        // }
        //translates index flexion to degrees
        //double totalFlex = indexFlexR;            
    }
}
