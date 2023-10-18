using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using HTC.UnityPlugin.Vive;
using HTC.UnityPlugin.Utility;
// using HTC.UnityPlugin.VRModuleManagement;


using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
//using System.IO.Ports;
using System.Threading;

public class AngleCalculations : MonoBehaviour
{
    /* 4 de agosto, pendientes:
        Terminar la logica para los calculos de ArmL (al menos replicarla)
        Hacer pruebas con la asignacion dinamica de trackers por codigo
        Probar el descancito en teleoperacion
        */

    /* 5 de agosto, resuelto:
        Amortiguamiento de theta 3 cerca de 
    */


    // booleanos para accionar o desactivar calculos de thetas<- logica de pausa/descancito
    // string sn = "LHR-318C92E6"; 

    
    bool testArmR = false;
    bool testArmL = false;
    bool testHead = false;

    // float scaling = 1.0f;

    // bool testHead = false;
    bool calibTrackersRecorded = false;

    RigidPose trackerChest; //general

    RigidPose trackerHead;     //testHead
    RigidPose rawTrackerChest; //testHead

        RigidPose trackerElbowR;            //testArmR
        RigidPose trackerShoulderR;         //testArmR
        RigidPose trackerForearmR;          //testArmR
        RigidPose trackerWristR;            //testArmR
           
            RigidPose trackerElbowL;        //testArml
            RigidPose trackerShoulderL;     //testArml
            RigidPose trackerForearmL;      //testArml
            RigidPose trackerWristL;        //testArml
  
    RigidPose calibTrackerChest; //general

        RigidPose calibTrackerShoulderR_chest;  //testArmR
        RigidPose calibTrackerElbowR_chest;     //testArmR
        RigidPose calibTrackerElbowR_shoulderR; //testArmR
        RigidPose calibTrackerForearmR_chest;   //testArmR
        RigidPose calibTrackerForearmR_elbowR;  //testArmR
        RigidPose calibTrackerWristR_forearmR;  //testArmR

            RigidPose calibTrackerShoulderL_chest;  //testArml
            RigidPose calibTrackerElbowL_chest;     //testArml
            RigidPose calibTrackerElbowL_shoulderL; //testArml
            RigidPose calibTrackerForearmL_chest;   //testArml
            RigidPose calibTrackerForearmL_elbowL;  //testArml
            RigidPose calibTrackerWristL_forearmL;  //testArml

        RigidPose shoulderR_chest;          //testArmR
        RigidPose elbowR_chest;             //testArmR
        RigidPose elbowR_shoulderR;         //testArmR
        RigidPose forearmR_elbowR;          //testArmR
        RigidPose wristR_forearmR;          //testArmR
        RigidPose theta2ShoulderR_chest;    //testArmR
    
            RigidPose shoulderL_chest;          //testArml
            RigidPose elbowL_chest;             //testArml
            RigidPose elbowL_shoulderL;         //testArml
            RigidPose forearmL_elbowL;          //testArml
            RigidPose wristL_forearmL;          //testArml

    RigidPose chest;
    RigidPose head;
    RigidPose rawChest;

    // RigidPose shoulderL;
    
              
                        // RigidPose trackerShoulderL;
            
            

    
    
    
                         // RigidPose trackerElbowL;
            
    
    
            
    // RigidPose forearmR_chest;
    

    //                     // RigidPose trackerForearmL;
    //         RigidPose forearmL;
    
    
    //                  // RigidPose trackerWristL;
    //         RigidPose calibTrackerWristL;
    //         RigidPose wristL;

        /*Variables para calculos*/
        // RigidPose elbowL_shoulderL;
        

    /* Variables empleadas para head - torso */
    Vector3 fEulerHead;
    Vector3 eulerHead;
    Vector3 fEulerChest;
    Vector3 eulerChest;
  
    
    RigidPose chestTrans;
    RigidPose rawInitialTrackerChest;
    RigidPose initialTrackerChest;
    RigidPose initialTrackerHead;
    //RigidPose trackerHead;

    string cmdChest, cmdHead;

    /* Vectores empleados en la comparación y cálculo para theta 1 brazo derecho
     (...MinusYF, YFixed) y theta 2 (...PlusZ, PlusZFixed)*/
    Vector3 refVecMinusYFixedR = new Vector3(0,-1,0);
    Vector3 refVecPlusZFixedR = new Vector3(0,0,1);
    Vector3 refVecMinusYR;
    Vector3 refVecPlusZR;
    // Vector3 refVecForTheta2Fixed;
    // Vector3 refVecForTheta2;
    Vector3 vecElbowR;
    Vector3 vecElbowRForTheta2;

            /* Vectores empleados en la comparación y cálculo para theta 1 brazo izquierdo
            (...MinusYF, YFixed) y theta 2 (...PlusZ, PlusZFixed)*/
            Vector3 refVecMinusYFixedL = new Vector3(0,-1,0);
            Vector3 refVecPlusZFixedL = new Vector3(0,0,1);
            Vector3 refVecMinusYL;
            Vector3 refVecPlusZL;
            // Vector3 refVecForTheta2FixedL;
            // Vector3 refVecForTheta2L;
            Vector3 vecElbowL;
            Vector3 vecElbowLForTheta2;

    /* Output de los valores calculados por las funciones "calculate_theta_XR" */
    Int16 theta1R;
    Int16 theta1Rmod;
    Int16 theta1RNoMod;
    Int16 theta2R;
    Int16 theta2Rmod;
    Int16 theta3R;
    int compensationT3 = 0;
    Int16 theta4R;
    // Matrix4x4 theta4R_auxMat;
    Int16 theta5R;
    Int16 theta6R;

    int currentJ1;
    int currentJ2;
    int currentJ3;
    int currentJ4;
    int currentJ5;
    int currentJ6;

    int previousJ1 = 0;
    int previousJ2 = 0;
    int previousJ3 = 0;
    int previousJ4 = 0;
    int previousJ5 = 0;
    int previousJ6 = 0;

    

            /* Output de los valores calculados por las funciones "calculate_theta_XL" */
            Int16 theta1L;
            Int16 theta1Lmod;
            Int16 theta1LNoMod;
            Int16 theta2L;
            Int16 theta2Lmod;
            Int16 theta3L;
            int compensationT3L = 0;
            Int16 theta4L;
            // Matrix4x4 theta4R_auxMat;
            Int16 theta5L;
            Int16 theta6L;

            int currentJ1L;
            int currentJ2L;
            int currentJ3L;
            int currentJ4L;
            int currentJ5L;
            int currentJ6L;
            
            int previousJ1L = 0;
            int previousJ2L = 0;
            int previousJ3L = 0;
            int previousJ4L = 0;
            int previousJ5L = 0;
            int previousJ6L = 0;


    string serialNums;

    // float  duration = 0.001f;
    float startTime;
    int threshold = 0;
    int thresholdL = 0;
                

    int j1,j2,j3,j4,j5,j6 = 0;
    int j1L,j2L,j3L,j4L,j5L,j6L = 0;

    UdpClient clientSim = new UdpClient();//create a client
    UdpClient clientSimL = new UdpClient();//create a client
    UdpClient clientHead = new UdpClient();//create a client
    UdpClient clientChest = new UdpClient();//create a client
    UdpClient clientArms = new UdpClient();//create a client
    
    
    // uint chestIndex = 2;
    // uint shoulderRIndex = 5;
    // uint elbowRIndex = 6;
    // uint forearmRIndex = 7;
    // uint wristRIndex = 8;

    Transform spChest;
    Transform spShoulderR;
    Transform spElbowR;
    Transform spForearmR;
    Transform spWristR;
    Transform spOrigin;
    Transform spRef;
    Transform spComp;
    float spScale;

    RunningDoubleAverage  average = new RunningDoubleAverage(20);

    

    //UdpClient client = new UdpClient();
    float map (int value, float lowIn, float highIn, float lowOut, float highOut)
    {
        return (value - lowIn)/(highIn-lowIn)*(highOut-lowOut)+lowOut;
    }
        // filterUpdate
        int filter (int currentIn, int previousIn, int threshold)
            {
                // int dataOut;
                if(Math.Abs(currentIn-previousIn)>threshold)
                {
                    return (currentIn+previousIn)/2;
                }else{
                    return previousIn;
                }
                // return dataOut;
            }
    void extract_poses()
    {
        //Debug.Log(myRoles.Chest);
        // for (uint i=1; i<=5; i++)
        // {
        //     if(String.Equals(myRoles.Chest,serialNums[i]))
        //     {
        //         trackerChest = VivePose.GetPose(i);
        //     }
        // }
        // if(testArmR)
        // {
        //     for (uint i=1; i<=5; i++)
        //     {
        //         if(String.Equals(myRoles.ShoulderR,serialNums[i]))
        //         {
        //             trackerShoulderR = VivePose.GetPose(i);
        //         }else{
        //             if(String.Equals(myRoles.ElbowR,serialNums[i]))
        //             {
        //                 trackerElbowR = VivePose.GetPose(i);
        //             }else{
        //                 if(String.Equals(myRoles.ForearmR,serialNums[i]))
        //                 {
        //                     trackerForearmR = VivePose.GetPose(i);
        //                 }
        //             }
        //         }
        //     }
        // }

        trackerChest = VivePose.GetPoseEx(TrackerRole.Tracker1);
        trackerHead = VivePose.GetPoseEx(DeviceRole.Hmd);

        if(testHead)
        {
            rawTrackerChest = VivePose.GetPoseEx(TrackerRole.Tracker1);
            // Debug.Log(trackerHead);

        }

        spChest.position = trackerChest.pos;
        // trackerChest = VivePose.GetPose(chestIndex);
        if(testArmR)
        {
            trackerShoulderR    = VivePose.GetPoseEx(TrackerRole.Tracker2);
            trackerElbowR       = VivePose.GetPoseEx(TrackerRole.Tracker3);
            trackerForearmR     = VivePose.GetPoseEx(TrackerRole.Tracker4);
            // trackerWristR       = VivePose.GetPoseEx(TrackerRole.Tracker5);
            spElbowR.position   = trackerElbowR.pos;
            
            // trackerShoulderR = VivePose.GetPose(shoulderRIndex);
            // trackerElbowR = VivePose.GetPose(elbowRIndex);
            // trackerForearmR = VivePose.GetPose(forearmRIndex);
            // trackerWristR = VivePose.GetPose(wristRIndex);
        }

        if(testArmL)
        {
            // trackerShoulderL    = VivePose.GetPoseEx(TrackerRole.Tracker2);
            // trackerElbowL       = VivePose.GetPoseEx(TrackerRole.Tracker3);
            // trackerForearmL     = VivePose.GetPoseEx(TrackerRole.Tracker4);
            // trackerWristL       = VivePose.GetPoseEx(TrackerRole.Tracker5);



            // trackerShoulderL    = VivePose.GetPoseEx(TrackerRole.Tracker6);
            // trackerElbowL       = VivePose.GetPoseEx(TrackerRole.Tracker7);
            // trackerForearmL     = VivePose.GetPoseEx(TrackerRole.Tracker8);
            // trackerWristL       = VivePose.GetPoseEx(TrackerRole.Tracker9);

            trackerShoulderL    = VivePose.GetPoseEx(TrackerRole.Tracker5);
            trackerElbowL       = VivePose.GetPoseEx(TrackerRole.Tracker6);
            trackerForearmL     = VivePose.GetPoseEx(TrackerRole.Tracker7);
            trackerWristL       = VivePose.GetPoseEx(TrackerRole.Tracker8);

        }
    }
    void calibrate()
    {
        extract_poses();
        // calib_chest();
        Debug.Log("Chest calibrated.");
        calibTrackerChest = trackerChest;

        if(testArmR)
        {
            calibTrackerShoulderR_chest = trackerShoulderR;
            calibTrackerShoulderR_chest.rot = Quaternion.Inverse(calibTrackerShoulderR_chest.rot)*calibTrackerChest.rot;
            Debug.Log("Right shoulder calibrated.");

                // calibTrackerElbowR_chest = trackerElbowR;
                // calibTrackerElbowR_chest.rot = Quaternion.Inverse(calibTrackerElbowR_chest.rot)*calibTrackerChest.rot;

            calibTrackerElbowR_chest = trackerElbowR;
            calibTrackerElbowR_chest.rot = Quaternion.Inverse(calibTrackerElbowR_chest.rot)*calibTrackerChest.rot;
   
            calibTrackerElbowR_shoulderR = trackerElbowR;
            calibTrackerElbowR_shoulderR.rot = Quaternion.Inverse(calibTrackerElbowR_shoulderR.rot)*trackerShoulderR.rot;
            Debug.Log("Right elbow calibrated.");

            calibTrackerForearmR_chest = trackerForearmR;
            calibTrackerForearmR_chest.rot = Quaternion.Inverse(calibTrackerForearmR_chest.rot)*calibTrackerChest.rot;
                
            calibTrackerForearmR_elbowR = trackerForearmR;
            calibTrackerForearmR_elbowR.rot = Quaternion.Inverse(calibTrackerForearmR_elbowR.rot)*trackerElbowR.rot;
            Debug.Log("Right forearm calibrated.");

            calibTrackerWristR_forearmR = trackerWristR;
            calibTrackerWristR_forearmR.rot = Quaternion.Inverse(calibTrackerWristR_forearmR.rot)*trackerForearmR.rot;
            Debug.Log("Right wrist calibrated.");
        }


        if(testArmL)
        {
            calibTrackerShoulderL_chest = trackerShoulderL;
            calibTrackerShoulderL_chest.rot = Quaternion.Inverse(calibTrackerShoulderL_chest.rot)*calibTrackerChest.rot;
            Debug.Log("Left shoulder calibrated.");

            calibTrackerElbowL_chest = trackerElbowL;
            calibTrackerElbowL_chest.rot = Quaternion.Inverse(calibTrackerElbowL_chest.rot)*calibTrackerChest.rot;

            calibTrackerElbowL_shoulderL = trackerElbowL;
            calibTrackerElbowL_shoulderL.rot = Quaternion.Inverse(calibTrackerElbowL_shoulderL.rot)*trackerShoulderL.rot;
            Debug.Log("Left elbow calibrated.");

            calibTrackerForearmL_chest = trackerForearmL;
            calibTrackerForearmL_chest.rot = Quaternion.Inverse(calibTrackerForearmL_chest.rot)*calibTrackerChest.rot;

            calibTrackerForearmL_elbowL = trackerForearmL;
            calibTrackerForearmL_elbowL.rot = Quaternion.Inverse(calibTrackerForearmL_elbowL.rot)*trackerElbowL.rot;
            Debug.Log("Left forearm calibrated.");

            calibTrackerWristL_forearmL = trackerWristL;
            calibTrackerWristL_forearmL.rot = Quaternion.Inverse(calibTrackerWristL_forearmL.rot)*trackerForearmL.rot;
            Debug.Log("Left wrist calibrated.");
        }
        calibTrackersRecorded = true;
    }

    void adjust_meas()
    {
        extract_poses();
        if(testArmR)
        {
            shoulderR_chest.rot = trackerShoulderR.rot*calibTrackerShoulderR_chest.rot;
            shoulderR_chest.rot = Quaternion.Inverse(shoulderR_chest.rot)*trackerChest.rot;

            elbowR_chest.rot = trackerElbowR.rot*calibTrackerElbowR_chest.rot;
            elbowR_chest.rot = Quaternion.Inverse(elbowR_chest.rot)*trackerChest.rot;
            elbowR_chest.rot = Quaternion.Inverse(elbowR_chest.rot);
            
            // elbowR_chest.rot = trackerElbowR.rot*calibTrackerElbowR_chest.rot;
            // elbowR_chest.rot = Quaternion.Inverse(elbowR_chest.rot)*Quaternion.Inverse(trackerChest.rot);

            elbowR_shoulderR.rot = trackerElbowR.rot*calibTrackerElbowR_shoulderR.rot;
            elbowR_shoulderR.rot = Quaternion.Inverse(elbowR_shoulderR.rot)*trackerShoulderR.rot;

            forearmR_elbowR.rot = trackerForearmR.rot*calibTrackerForearmR_elbowR.rot;
            forearmR_elbowR.rot = Quaternion.Inverse(forearmR_elbowR.rot)*trackerElbowR.rot;
            forearmR_elbowR.rot = Quaternion.Inverse(forearmR_elbowR.rot);

            wristR_forearmR.rot = trackerWristR.rot*calibTrackerWristR_forearmR.rot;
            wristR_forearmR.rot = Quaternion.Inverse(wristR_forearmR.rot)*trackerForearmR.rot;

            theta2ShoulderR_chest.rot = trackerShoulderR.rot*calibTrackerShoulderR_chest.rot;
            theta2ShoulderR_chest.rot = Quaternion.Inverse(theta2ShoulderR_chest.rot)*trackerChest.rot;
        }

        if(testArmL)
        {
            // RigidPose shoulderL_chest;
            shoulderL_chest.rot = trackerShoulderL.rot*calibTrackerShoulderL_chest.rot;
            shoulderL_chest.rot = Quaternion.Inverse(shoulderL_chest.rot)*trackerChest.rot;

            //RigidPose elbowL_chest;
            elbowL_chest.rot = trackerElbowL.rot*calibTrackerElbowL_chest.rot;
            elbowL_chest.rot = Quaternion.Inverse(elbowL_chest.rot)*trackerChest.rot;
            elbowL_chest.rot = Quaternion.Inverse(elbowL_chest.rot);
            
            // elbowR_chest.rot = trackerElbowR.rot*calibTrackerElbowR_chest.rot;
            // elbowR_chest.rot = Quaternion.Inverse(elbowR_chest.rot)*Quaternion.Inverse(trackerChest.rot);

            // RigidPose elbowL_shoulderL;
            elbowL_shoulderL.rot = trackerElbowL.rot*calibTrackerElbowL_shoulderL.rot;
            elbowL_shoulderL.rot = Quaternion.Inverse(elbowL_shoulderL.rot)*trackerShoulderL.rot;

            // RigidPose forearmL_elbowL;
            forearmL_elbowL.rot = trackerForearmL.rot*calibTrackerForearmL_elbowL.rot;
            forearmL_elbowL.rot = Quaternion.Inverse(forearmL_elbowL.rot)*trackerElbowL.rot;
            forearmL_elbowL.rot = Quaternion.Inverse(forearmL_elbowL.rot);

            // RigidPose wristL_forearmL;
            wristL_forearmL.rot = trackerWristL.rot*calibTrackerWristL_forearmL.rot;
            wristL_forearmL.rot = Quaternion.Inverse(wristL_forearmL.rot)*trackerForearmL.rot;

            // theta2ShoulderR_chest.rot = trackerShoulderR.rot*calibTrackerShoulderR_chest.rot;
            // theta2ShoulderR_chest.rot = Quaternion.Inverse(theta2ShoulderR_chest.rot)*trackerChest.rot;
        }
    }

    

    void calculate_theta_1_R()
    {
        refVecMinusYR = chest.rot * refVecMinusYFixedR;
        vecElbowR = elbowR_chest.rot * refVecMinusYR;
        // spRef.position = refVecMinusY;
        // spComp.position = vecElbowR;
        theta1R = Convert.ToInt16(Vector3.Angle(refVecMinusYR,vecElbowR));
        theta1Rmod = theta1R;
        theta1RNoMod = theta1R;
        
        if(vecElbowR.z>0)
        {
            theta1Rmod = Convert.ToInt16(-theta1R);
            // theta1R = 0;
        }

        // th2th1R1 = Convert.ToInt16(theta2ShoulderR_chest.rot.eulerAngles.x);
        // print("T1: "+theta1R);
        // TODO LIMITES
        // if(th2th1R1>180){
        //     th2th1R1 -= 360;
        // }
        // j1 = -th2th1R1;
        
    }

                // CAMBIAR A VECTORES DE REFERENCIA RESPECTO A ArmL
               
        void calculate_theta_1_L()
        {
            refVecMinusYL = chest.rot * refVecMinusYFixedL;
            vecElbowL = elbowL_chest.rot * refVecMinusYL;
            // spRef.position = refVecMinusY;
            // spComp.position = vecElbowR;
            theta1L = Convert.ToInt16(Vector3.Angle(refVecMinusYL,vecElbowL));
            theta1Lmod = theta1L;
            theta1LNoMod = theta1L;
            if(vecElbowL.z>0)
                {
                    theta1Lmod = Convert.ToInt16(-theta1L);
                    // theta1R = 0;
                }

            // th2th1R1 = Convert.ToInt16(theta2ShoulderR_chest.rot.eulerAngles.x);
            // print("T1: "+theta1R);
            // TODO LIMITES
            // if(th2th1R1>180){
            //     th2th1R1 -= 360;
            // }
            // j1 = -th2th1R1;
            
        }


    void calculate_theta_2_R()
    {
        refVecPlusZR = (-refVecPlusZFixedR);
        // refVecPlusZ = chest.rot*(-refVecPlusZFixedR);
        refVecPlusZR.y = 0;
        vecElbowRForTheta2 = vecElbowR;
        // vecElbowRForTheta2 = chest.rot*vecElbowRForTheta2;
        vecElbowRForTheta2.y = 0;

        // spRef.position = refVecPlusZ;
        // spComp.position = vecElbowRForTheta2;
        
        theta2R = Convert.ToInt16(Vector3.Angle(refVecPlusZR,vecElbowRForTheta2));
        if(vecElbowRForTheta2.x < 0){
            theta2R = Convert.ToInt16(-theta2R);
        }

        
        
        float percent = (theta1R*100)/90;
        theta2Rmod = theta2R;

        theta2R =  Convert.ToInt16((percent/100)*theta2R);
        // theta1R -= Convert.ToInt16(theta2R/90*25);
                theta1R -= Convert.ToInt16(theta2R/90.0*60.0);
        // theta1R += Convert.ToInt16(theta2R/90.0*60.0);

        // if(vecElbowR.z>0 && ){}
        

        // if (theta2R>theta1R)
        //     {
        //         int adjustment = 90/theta1R;
        //         j1 = j1-(theta1R*adjustment);
        //         // j1=j1-(int) map(theta1R,)
        //     }else{
        //         j1 = theta1R;
        //     }
        
        // print("T2: "+theta2R);    
    }

                void calculate_theta_2_L()
                {
                    refVecPlusZL = (-refVecPlusZFixedL);
                    // refVecPlusZ = chest.rot*(-refVecPlusZFixedR);
                    refVecPlusZL.y = 0;
                    vecElbowLForTheta2 = vecElbowL;
                    // vecElbowRForTheta2 = chest.rot*vecElbowRForTheta2;
                    vecElbowLForTheta2.y = 0;

                    // spRef.position = refVecPlusZ;
                    // spComp.position = vecElbowRForTheta2;
                    
                    theta2L = Convert.ToInt16(Vector3.Angle(refVecPlusZL,vecElbowLForTheta2));
                    
                    if(vecElbowLForTheta2.x < 0)
                    {
                        theta2L = Convert.ToInt16(-theta2L);
                    }

                    // Angulos con esfera.
                    // if (theta1R<14// && theta2R<20
                    // )
                    // {
                    //     theta1R = 5;
                    //     theta2R = 5;
                    // }
                    // double segTheta1 = theta1R/57.29;
                    // double segTheta2 = theta2R/57.29;
                    // double hypo = Math.Sqrt(segTheta1*segTheta1+segTheta2*segTheta2);
                    // double gamma = Math.Asin(segTheta2/hypo);
                    // double a = Math.Sin(gamma+25)*hypo;
                    // double b = Math.Sqrt(hypo*hypo-a*a);
                    // theta2R = Convert.ToInt16(a * 57.29);
                    // theta1R = Convert.ToInt16(b * 57.29);

                    
                    float percent = (theta1L*100)/90;
                    theta2Lmod = theta2L;

                    theta2L =  Convert.ToInt16((percent/100)*theta2L);
                    // theta1R -= Convert.ToInt16(theta2R/90*25);
                    theta1L += Convert.ToInt16(theta2L/90.0*60.0);
                    // if(vecElbowR.z>0 && ){}
                    

                    // if (theta2R>theta1R)
                    //     {
                    //         int adjustment = 90/theta1R;
                    //         j1 = j1-(theta1R*adjustment);
                    //         // j1=j1-(int) map(theta1R,)
                    //     }else{
                    //         j1 = theta1R;
                    //     }
                    
                    // print("T2: "+theta2R);    
                }

    void calculate_theta_3_R()
    {
        theta3R = Convert.ToInt16(elbowR_shoulderR.rot.eulerAngles.x);
        if(theta3R>180){
            theta3R -= 360;
        }
        // print("T3: "+theta3R);
        
    }
                void calculate_theta_3_L()
                {
                    theta3L = Convert.ToInt16(elbowL_shoulderL.rot.eulerAngles.x);
                    if(theta3L>180){
                        theta3L -= 360;
                    }
                    // print("T3: "+theta3R);
                    
                }

    void calculate_theta_4_R()
    {
        theta4R = Convert.ToInt16(Quaternion.Angle(Quaternion.identity,forearmR_elbowR.rot));
        // print("T4: "+theta4R); 
    }

                void calculate_theta_4_L()
                {
                    theta4L = Convert.ToInt16(Quaternion.Angle(Quaternion.identity,forearmL_elbowL.rot));
                    // print("T4: "+theta4R); 
                }

    void calculate_theta_5_R()
    {
        theta5R = Convert.ToInt16(wristR_forearmR.rot.eulerAngles.y);
        if(theta5R>180){
            theta5R -= 360;
        }
        // print("T5: "+theta5R);
        // j5 = theta5R;
    }

            void calculate_theta_5_L()
            {
                theta5L = Convert.ToInt16(wristL_forearmL.rot.eulerAngles.y);
                if(theta5L>180){
                    theta5L -= 360;
                }
                // print("T5: "+theta5R);
                // j5 = theta5L;
            }

    void calculate_theta_6_R()
    {
        // theta6R = Convert.ToInt16(Quaternion.Angle(Quaternion.identity,handR_wristR.rot));
        theta6R = Convert.ToInt16(wristR_forearmR.rot.eulerAngles.z);
        if(theta6R>180){
            theta6R -= 360;
        }
        // print("T6: "+theta6R);
        // j6 = theta6R;
    }

                void calculate_theta_6_L()
                {
                    // theta6R = Convert.ToInt16(Quaternion.Angle(Quaternion.identity,handR_wristR.rot));
                    theta6L = Convert.ToInt16(wristL_forearmL.rot.eulerAngles.z);
                    if(theta6L>180){
                        theta6L -= 360;
                    }
                    // print("T6: "+theta6R);
                    // j6 = theta6L;
                }
    
    // public Roles myRoles;

    void Awake()
    {
        // string myLoadedRoles = JsonFileReader.LoadJsonAsResource("Roles/config.json");
        // myRoles = JsonUtility.FromJson<Roles>(myLoadedRoles);
        
    }
    void Start()
    {
        //  brazos = 9001
        //  manos = 9002
        //  cabeza = 9003
        //  torso = 9004
        //  pedales = 9005
        //  ip= 192.198.100.63
        // var index = ViveRole.GetDeviceIndexEx(TrackerRole.Tracker2);
        // string res = "";
        // Debug.Log(index);

        // if(VRModule.IsValidDeviceIndex(index))
        // {
        //     Debug.Log(index);
        //     var deviceState = VRModule.GetDeviceState(index);
        //     //Debug.Log(deviceState.serialNumber);
        //     if (String.Equals(deviceState.serialNumber.ToString(),sn))
        //     {
        //         Debug.Log("yes");
        //     }else{
        //         Debug.Log("no");
        //     }
        //     // Debug.Log(res[i]);
        //     res = deviceState.serialNumber.ToString();    
        //     Debug.Log(res);
        // }
        startTime = Time.time;
        clientSim.Connect(new IPEndPoint(IPAddress.Parse  ("192.168.100.19"),10000));
        clientSimL.Connect(new IPEndPoint(IPAddress.Parse ("192.168.100.19"),10001));
 
        clientArms.Connect(new IPEndPoint(IPAddress.Parse ("192.168.100.130"),9001));        
        clientHead.Connect(new IPEndPoint(IPAddress.Parse ("192.168.100.130"),9003)); //connect to client on first frame
        clientChest.Connect(new IPEndPoint(IPAddress.Parse("192.168.100.130"),9004));
        
        //se llama a la funcion que reune los numeros seriales y se imprimen uno por uno
        // foreach (var item in serialNums)
        // {
        //     Debug.Log(item); //por testear, deberia funcionar aun si no estan todos los trackers conectados.
        // //     //Strat: un script para recoger todos los numeros seriales de los trackers 1 al 9 y  construir un array con ellos (HECHO)
        // //     //iterar entre los elementos del array y compararlo con los roles predefenidos por un archivo .json (IN WORK: ##TODO definir si eso va en otro script, se puso aqui porque este era el principal en versiones anteriores)
        // //     //llamar a funcion para asignar el dispositivo segun requiera el joint (NOT YET)

        // //     //TO DO: Archivo .json tendra un registro arbitrario de los numeros de serie, los roles que ocuparan y un ID para cada tracker
        // //     //La idea es facilitar el reconocimiento

        
        // }
        spScale = 0.1f;
        spChest = NewSphere(Vector3.one*spScale,Vector3.zero,Color.white);
        // spChest.x = spChest.y;
        spShoulderR = NewSphere(Vector3.one*spScale,Vector3.zero,Color.white);
        spElbowR = NewSphere(Vector3.one*spScale,Vector3.zero,Color.white);
        spForearmR = NewSphere(Vector3.one*spScale,Vector3.zero,Color.white);
        spWristR = NewSphere(Vector3.one*spScale,Vector3.zero,Color.white);
        spOrigin = NewSphere(Vector3.one*spScale,Vector3.zero,Color.magenta);
        spRef = NewSphere(Vector3.one*spScale,Vector3.zero,Color.cyan);
        spComp = NewSphere(Vector3.one*spScale,Vector3.zero,Color.yellow);
    }

    // Update is called once per frame
    // 0.011 s FixedUpdate()
    // void Update() 
    // {
    //     if(Input.GetKeyDown("b"))
    //     {
    //         Debug.Log("Testing.");
    //     }
    // }

    void FixedUpdate()
    {
        extract_poses();
        // var deviceIndex = ViveRole.GetDeviceIndexEx(TrackerRole.Tracker2);
        // if(deviceIndexInt != deviceIndex)
        // {
        //     deviceIndexInt=deviceIndex;
        //     if(VRModule.IsValidDeviceIndex(deviceIndex));
        //     {
        //         Debug.Log(deviceIndex);
        //         // Debug.Log(TrackerRole.Tracker1.ToString());
        //         var deviceState = VRModule.GetDeviceState(deviceIndex);
        //         string serial = deviceState.serialNumber;
        //         if(String.Equals(serial, sn))
        //         {
        //             Debug.Log("yes");
        //         }
        //         Debug.Log(deviceState.ToString());
        //     }
        // }

        if(Input.GetKeyDown("k"))
        {
            string killAllCmd = "<1.8.9>";
            if (killAllCmd!=null)
            {
                // send the chest message
                byte[] killMessage = Encoding.ASCII.GetBytes(killAllCmd);
                clientArms.Send(killMessage,killMessage.Length);
                Debug.Log("Message "+killAllCmd+" sent with success");
            }

            clientArms.Close();
        }

        if(Input.GetKeyDown("c"))
        {
            // testHead=testHead!;
            testArmR=false;
            testArmL=false;
        }

        if(Input.GetKeyDown("v"))
        {
            // testHead=testHead!;
            testArmR=true;
            // testArmL=true;
        }

        if(Input.GetKeyDown("x"))
        {
            testHead=false;
            // testArmR=true;
            // testArmL=true;
        }

        if(Input.GetKeyDown("z"))
        {
            testHead=true;
            // testArmR=true;
            // testArmL=true;
        }
        
        if(Input.GetKeyDown("l"))
        {
            calibrate();
            chest.rot = Quaternion.Inverse(trackerChest.rot)*calibTrackerChest.rot;
            chest.rot = Quaternion.Inverse(chest.rot);

            initialTrackerChest = trackerChest;
            rawInitialTrackerChest = rawTrackerChest;
            //initialTrackerChest.rot = initialTrackerChest.rot;// * rotAxisChest;
            initialTrackerHead = trackerHead;
            initialTrackerHead.rot = Quaternion.Inverse(initialTrackerHead.rot) * rawInitialTrackerChest.rot;
        }
        
        if(calibTrackersRecorded)
        {
            adjust_meas();
        }
        
        if(//Input.GetKeyDown("q") && 
        calibTrackersRecorded)
        {
            // if(VivePose.IsValidEx(TrackerRole.Tracker1) && VivePose.IsValidEx(DeviceRole.Hmd))// && VivePose.IsValidEx(TrackerRole.Tracker2))// && VivePose.IsValidEx(TrackerRole.Tracker3))
            // { 
                chestTrans = rawTrackerChest;
                        
                rawChest.rot = Quaternion.Inverse(rawTrackerChest.rot) * rawInitialTrackerChest.rot;// * rotAxisChest;
                chest.rot = Quaternion.Inverse(trackerChest.rot) * initialTrackerChest.rot;// * rotAxisChest;
                head.rot = trackerHead.rot * initialTrackerHead.rot;
                head.rot = Quaternion.Inverse(head.rot) * rawTrackerChest.rot;
                
                fEulerChest = rawChest.rot.eulerAngles;
                fEulerHead = head.rot.eulerAngles;
                eulerChest.x = Convert.ToInt16(fEulerChest.x);
                eulerChest.y = Convert.ToInt16(fEulerChest.y);
                eulerChest.z = Convert.ToInt16(fEulerChest.x);

                eulerHead.x = Convert.ToInt16(fEulerHead.x);
                eulerHead.y = Convert.ToInt16(fEulerHead.y);
                eulerHead.z = Convert.ToInt16(fEulerHead.z);
            
        
    // // x es si
                // y es no
                // z es meh
                // Conversion de 0:360 a -180:180
                if(eulerChest.x>180){
                    eulerChest.x -= 360;
                }
                if(eulerChest.y>180){
                    eulerChest.y -= 360;
                }
                if(eulerChest.z>180){
                    eulerChest.z -= 360;
                }
                if(eulerHead.x>180){
                    eulerHead.x -= 360;
                }
                if(eulerHead.y>180){
                    eulerHead.y -= 360;
                }
                if(eulerHead.z>180){
                    eulerHead.z -= 360;
                }
                eulerHead.x = -eulerHead.x;
                // Conversion de limites
                if(eulerChest.x < -6){
                    eulerChest.x = -6;
                }
                if(eulerChest.x > 40){
                    eulerChest.x = 40;
                }
                if(eulerChest.y < -90){
                    eulerChest.y = -90;
                }
                if(eulerChest.y > 90){
                    eulerChest.y = 90;
                }

                if(eulerHead.x < -20){
                    eulerHead.x = -20;
                }
                if(eulerHead.x > 60){
                    eulerHead.x = 60;
                }
                if(eulerHead.y < -80){
                    eulerHead.y = 80;
                }
                if(eulerHead.y > 80){
                    eulerHead.y = 80;
                }
                if(eulerHead.z < -10){
                    eulerHead.z = -10;
                }
                if(eulerHead.z > 10){
                    eulerHead.z = 10;
                }
                

                cmdChest = "<1.6.1."+eulerChest.x+"."+eulerChest.y+">";
                cmdHead = "<1.7.1."+eulerHead.y+"."+eulerHead.x+"."+eulerHead.z+">";
              

        if (cmdChest!=null)
        {
            // send the chest message
            byte[] bytesent = Encoding.ASCII.GetBytes(cmdChest);
            clientChest.Send(bytesent,bytesent.Length);
            // Debug.Log("Message "+cmdChest+" sent with success");
        }
        
        if (cmdHead!=null)
        {
            // send the head message
            byte[] bytesent = Encoding.ASCII.GetBytes(cmdHead);
            clientHead.Send(bytesent,bytesent.Length);
            // Debug.Log("Message "+cmdHead+" sent with success");
        }
    // }

            if(testArmR)
            {
                calculate_theta_1_R();
                calculate_theta_2_R();
                calculate_theta_3_R();
                calculate_theta_4_R();
                // calculate_theta_5_R();
                // calculate_theta_6_R();
            
                float compensate;
                currentJ1 = theta1R;
                // j2 = theta2R;

                // if(j1<-3&&j2<20){
                //     j2= -j2;
                // }

                            // if(theta1Rmod>90)
                            // {
                            //     j2 = theta2R-(int)map(theta1Rmod,90,130,0,30);
                            //     //Debug.Log(theta1Rmod);
                            // }
                
                
                
                // j3 = j3mapped;

                //
                // Debug.Log(j3mapped);
                currentJ4 = (int)map(theta4R,0,105,0,150);
                if(currentJ4>120)
                {
                    currentJ4=120;
                }
                int j3mapped = (int)map(theta3R,-20,20,-40,40);
                // int j3mapped = (int)map(theta3R,-20,40,-20,40);

                
                // if(j3mapped<8)
                // {
                //     float j3upscale = j3mapped*1.95f;
                //     j3mapped=(int)j3upscale;
                // }else

                //     // if(j3mapped>20)
                //     {
                //         float j3upscale = j3mapped*1;
                //         j3mapped=(int)j3upscale;
                //     }



                threshold=2;
            
                    //-(int)map(theta1RNoMod,0,90,0,20);
                    float compensateT1 =    1/((theta1R+0.5f)/20)    ;
                    
                    // currentJ3 = j3mapped+(int)compensationT3;//+(int)compensateT1;

                    if(currentJ3<-5)
                    {
                        compensate = (1/((currentJ3+0.5f)/50));
                    }else{
                        compensate = 0;
                    }
                    // j3 += (int)compensate;
                if(currentJ2>15)
                {
                    compensationT3 = ((currentJ2/80)*45);
                }
                // j3 = j3mapped-(int)map(j2,0,80,0,55);
                currentJ3 = j3mapped+(int)compensationT3;//+(int)compensateT1;

                if(currentJ3>55)
                {
                    currentJ3=55;
                }
                currentJ2 = -(theta2R+(int)map(theta1RNoMod,0,90,0,25)+(int)compensate);
                // Debug.Log(compensate);

                // if(j3<-55)
                // {
                //     j3=-55;
                // }
                        // int j3compensate = (int)map(theta1RNoMod,0,90,0,25);
                        //+(int)map(theta1RNoMod,0,90,0,25);
                        // if(theta1RNoMod<90){
                        //     j3+=j3compensate;
                        // } 
                        //*(-j4)/90+(int)map(theta1RNoMod,0,90,0,25);
                currentJ5 = theta5R;
                currentJ6 = -theta6R; // probablemente negativo

                
                // float t = (Time.time - startTime) / duration;
                                      

                // string rightValues = "#"+currentJ1+"."+currentJ2+"."+currentJ3+"."+currentJ4+"."+currentJ5+"."+currentJ6+"#";
                // Debug.Log(rightValues);

                // Futura iteracion de filtro, guardar valor previo y comparar el valor actual para "filtrar"
                // saltos mayores a 5, 10, o 15 grados.
                // Si se detectan saltos grandes, el valor que se envie al robot sera el promedio
                // entre el valor actual y el previo. 

                // j1 = currentJ1;
                // j2 = currentJ2;
                // j3 = currentJ3;
                // j4 = currentJ4;
                // j5 = currentJ5;
                // j6 = currentJ6;

                // Debug.Log("1 mod: "     +theta1Rmod);
                // Debug.Log("1 NoMod: "   +theta1RNoMod);
                // Debug.Log("1 theta: "   +theta1R);

                        if(Math.Abs(previousJ1-currentJ1)>threshold)
                        {
                            j1 = (currentJ1+previousJ1)/2;
                            
                        }
                        
                        else{
                            j1 = previousJ1;
                        }

                        if(Math.Abs(previousJ2-currentJ2)>threshold)
                        {
                            j2 = (currentJ2+previousJ2)/2;
                            
                        }
                        
                        else{
                            j2 = previousJ2;
                        }

                        if(Math.Abs(previousJ3-currentJ3)>threshold)
                        {
                            j3 = (currentJ3+previousJ3)/2;
                            
                        }
                        
                        else{
                            j3 = previousJ3;
                        }

                        if(Math.Abs(previousJ4-currentJ4)>threshold)
                        {
                            j4 = (currentJ4+previousJ4)/2;
                            
                        }

                        else{
                            j4 = previousJ4;
                        }

                        if(Math.Abs(previousJ5-currentJ5)>threshold)
                        {
                            j5 = (currentJ5+previousJ5)/2;
                            
                        }
                        
                        else{
                            j5 = previousJ5;
                        }

                        if(Math.Abs(previousJ6-currentJ6)>threshold)
                        {
                            j6 = (currentJ6+previousJ6)/2;
                            
                        }
                        
                        else{
                            j6 = previousJ6;
                        }
                        
                // string rightValues = "#"+currentJ1+"."+currentJ2+"."+currentJ3+"."+currentJ4+"."+currentJ5+"."+currentJ6+"#";

                // string rightValues = "#"+j1+"."+j2+"."+j3+"."+j4+"."+j5+"."+j6+"#";
                string rightValues = "<1.2.1."+j1+"."+j2+"."+j3+"."+j4+"."+j5+"."+j6+">";
                
                if(rightValues!=null)
                {
                    byte[] byteToSendRight = Encoding.ASCII.GetBytes(rightValues);
                    clientArms.Send(byteToSendRight,byteToSendRight.Length);
                    Debug.Log(rightValues+" sent.");
                }
                    // if(rightValues!=null)
                    // {
                    //     byte[] byteToSendRight = Encoding.ASCII.GetBytes(rightValues);
                    //     clientSim.Send(byteToSendRight,byteToSendRight.Length);
                    //     Debug.Log(rightValues+" sent.");
                    // }
                
                previousJ1 = j1;
                previousJ2 = j2;
                previousJ3 = j3;
                previousJ4 = j4;
                previousJ5 = j5;
                previousJ6 = j6;  

                // Debug.Log("Previous: "+previousJ1);
                // Debug.Log("Previous: "+previousJ2);      
                // Debug.Log("Previous: "+previousJ3);
                // Debug.Log("Previous: "+previousJ4);      
                // Debug.Log("Previous: "+previousJ5);
                // Debug.Log("Previous: "+previousJ6);      
      
            }

            if(testArmL)
            {
                // int threshold = 1;

                calculate_theta_1_L();
                calculate_theta_2_L();
                calculate_theta_3_L();
                calculate_theta_4_L();
                // calculate_theta_5_L();
                // calculate_theta_6_L();
                
                float compensateL;
                currentJ1L = theta1L;

                currentJ4L = (int) map(theta4L,0,105,0,150);
                if(currentJ4L>120)
                {
                    currentJ4L=120;
                }
                int j3mappedL = (int)map(theta3L,-20,20,-40,40);
                
                    // if(j3mappedL<15)
                    // {
                    //     float j3upscaleL = j3mappedL*1.75f;
                    //     j3mappedL=(int)j3upscaleL;
                    // }
                    // if(j3mappedL>20)
                    // {
                    //     float j3upscaleL = j3mappedL*0.65f;
                    //     j3mappedL=(int)j3upscaleL;
                    // }

                if(currentJ3L<-5)
                {
                        compensateL = (1/((currentJ3L+0.5f)/50));
                }else{
                        compensateL = 0;
                }
                if(currentJ2L>15)
                {
                    compensationT3L = ((currentJ2L/80)*45);
                }

                currentJ3L = j3mappedL+(int)compensationT3L;
                if(currentJ3L>55)
                {
                    currentJ3L=55;
                }
                currentJ2L = (theta2L-(int)map(theta1LNoMod,0,90,0,25));//-(int)compensateL;
                currentJ5L = theta5L;
                currentJ6L = theta6L; // probablemente negativo

                if(Input.GetKeyDown("r"))
                {
                    thresholdL++;
                    Debug.Log(thresholdL);

                }
                if(Input.GetKeyDown("t"))
                {
                    thresholdL--;  
                    Debug.Log(thresholdL);
                }
                if(thresholdL<0){
                    thresholdL=0;
                }
                
                // Debug.Log("Lefty: "+threshold);

                // j1L = filter(currentJ1L,previousJ1L,threshold); 
                // j2L = filter(currentJ2L,previousJ2L,threshold); 
                // j3L = filter(currentJ3L,previousJ3L,threshold); 
                // j3L = currentJ3L;
                // j4L = filter(currentJ4L,previousJ4L,threshold); 
                // j5L = filter(currentJ5L,previousJ5L,threshold); 
                // j6L = filter(currentJ6L,previousJ6L,threshold); 
                // float t = (Time.time - startTime) / duration;


                if(Math.Abs(previousJ1L-currentJ1L)>thresholdL)
                        {
                            j1L = (currentJ1L+previousJ1L)/2;
                            
                        }
                        
                        else{
                            j1L = previousJ1L;
                        }

                        if(Math.Abs(previousJ2L-currentJ2L)>thresholdL)
                        {
                            j2L = (currentJ2L+previousJ2L)/2;
                            
                        }
                        
                        else{
                            j2L = previousJ2L;
                        }

                        if(Math.Abs(previousJ3L-currentJ3L)>thresholdL)
                        {
                            j3L = (currentJ3L+previousJ3L)/2;
                            
                        }
                        
                        else{
                            j3L = previousJ3L;
                        }

                        if(Math.Abs(previousJ4L-currentJ4L)>thresholdL)
                        {
                            j4L = (currentJ4L+previousJ4L)/2;
                            
                        }

                        else{
                            j4L = previousJ4L;
                        }

                        if(Math.Abs(previousJ5L-currentJ5L)>thresholdL)
                        {
                            j5L = (currentJ5L+previousJ5L)/2;
                            
                        }
                        
                        else{
                            j5L = previousJ5L;
                        }

                        if(Math.Abs(previousJ6L-currentJ6L)>thresholdL)
                        {
                            j6L = (currentJ6L+previousJ6L)/2;
                            
                        }
                        
                        else{
                            j6L = previousJ6L;
                        }

                // int currentJ1L = (int)((j1L+0.5f+previousJ1L+0.5f)/2);
                // int currentJ2L = (int)((j2L+0.5f+previousJ2L+0.5f)/2);
                // int currentJ3L = (int)((j3L+0.5f+previousJ3L+0.5f)/2);
                // int currentJ4L = (int)((j4L+0.5f+previousJ4L+0.5f)/2);
                // int currentJ5L = (int)((j5L+0.5f+previousJ5L+0.5f)/2);
                // int currentJ6L = (int)((j6L+0.5f+previousJ6L+0.5f)/2); 

                string leftValues = "<1.4.1."+(-j1L)+"."+(-j2L)+"."+(-j3L)+"."+(-j4L)+"."+j5L+"."+j6L+">";
                // string leftValues = "$"+j1L+","+currentJ2L+","+j3L+","+j4L+","+j5L+","+j6L+"$";

                // string leftValues = "$"+(j1L)+","+(j2L)+","+(j3L)+","+(j4L)+","+j5L+","+j6L+"$";
                // string leftValues = "$"+(currentJ1L)+","+(currentJ2L)+","+currentJ3L+","+currentJ4L+","+currentJ5L+","+currentJ6L+"$";

                // Debug.Log(inputs);
                // Debug.Log(leftValues);

                if(leftValues!=null)
                {
                    byte[] byteToSendLeft = Encoding.ASCII.GetBytes(leftValues);
                    clientArms.Send(byteToSendLeft,byteToSendLeft.Length);
                    Debug.Log(leftValues+" sent.");
                }

                        // if(leftValues!=null)
                        // {
                        //     byte[] byteToSendLeft = Encoding.ASCII.GetBytes(leftValues);
                        //     clientSimL.Send(byteToSendLeft,byteToSendLeft.Length);
                        //     Debug.Log(leftValues+" sent.");
                        // }

                previousJ1L =   j1L;
                previousJ2L =   j2L;
                previousJ3L =   j3L;
                previousJ4L =   j4L;    
                previousJ5L =   j5L;
                previousJ6L =   j6L;
            }
        }
    }
    
    static Transform NewSphere(Vector3 scale, Vector3 position, Color color) {
        Transform shape = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        UnityEngine.Object.Destroy(shape.GetComponent<Collider>()); // no collider, please!
        shape.localScale = scale; // set the rectangular volume size
        shape.position = position;
        //shape.rotation = Quaternion.LookRotation(dir, p2b - p1b);
        shape.GetComponent<Renderer>().material.color = color;
        shape.GetComponent<Renderer>().enabled = true; // show it
        return shape;
    }
}
