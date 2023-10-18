﻿using SG.Util;
using UnityEngine;
using System.Collections;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

// using System;  
using System.Collections.Generic;  
using System.Linq;  
// using System.Text;  
using System.Text.RegularExpressions; 

namespace SG
{
    /// <summary> This script collects the Force Feedback from the hand and sends these to its connected Hardware. </summary>
    public class SG_HandFeedback : SG_HandComponent
    {
        //-------------------------------------------------------------------------------------------------------------------------------------------
        // Member Variables

        /// <summary> This layer's glove Hardware. Used to link the fingers to the hardware. </summary>
        [Header("Feedback Components")]
        public SG_HapticGlove gloveHardware;

        /// <summary> Information about the 3D model this script is connected to. Used to set up tracking for the fingers/wrist. </summary>
        public SG_HandModelInfo handModel;

        /// <summary> Impact script for the wrist, should be linked to this connectedGlove. </summary>
        public SG_BasicFeedback wristFeedbackScript;
        /// <summary> Feedback colliders on each of the fingers, sorted from thumb to pinky. </summary>
        public SG_FingerFeedback[] fingerFeedbackScripts;

        //Codigo FFB para server
        public bool SendData;
        public int[] leftFA=new int[5], rightFA= new int[5];
        public int M;
        byte[] data = new byte[1024];
        public bool[] F = new bool[5] {true,true,true,true,true};
        //Haptics

        public SG_Waveform buzz;
        public int magnitude = 40;
        public float duration_s = 0.2f;

        //Server
        public GameObject server;
        public UDPReceive script;

        public string message;
        public string emptyString = "";
        public string rightMessage;
        public string leftMessage;

        string[] handSides = new string[2] {"SG Right Hand (SG.SG_HapticGlove)","SG Left Hand (SG.SG_HapticGlove)"};
        public string handSide;
        int index;

        //Animation curve
        public AnimationCurve ForceFBCR ;
        public AnimationCurve ForceFBCL ;
        public int maxForceS= 100;
        public int minForceS =40;
        public int Mag;
        public float[] mappedSR ,mappedSL ;
        public float[] forceMagnR = new float[5],forceMagnL = new float[5];
        public int[] CmdFFBR = new int[5],CmdFFBL = new int[5];
        public bool[] FinR = new bool[5], FinL = new bool[5];

        


        //-------------------------------------------------------------------------------------------------------------------------------------------
        // Accessors

        /// <summary> Used to show/hide the feedback colliders of this hand. </summary>
        public override bool DebugEnabled
        {
            get 
            {
                //its only enabled if all of them are.
                if (wristFeedbackScript != null && !wristFeedbackScript.DebugEnabled) { return false; }
                int nulls = 0;
                for (int f=0; f<this.fingerFeedbackScripts.Length; f++)
                {
                    if (fingerFeedbackScripts[f] == null) { nulls++; }
                    else if (!fingerFeedbackScripts[f].DebugEnabled) { return false; }
                }
                return nulls < fingerFeedbackScripts.Length; //if everything is set to null, then there's no Debug to turn on/off.
            }
            set
            {
                wristFeedbackScript.DebugEnabled = value;
                for (int f = 0; f < fingerFeedbackScripts.Length; f++)
                {
                    fingerFeedbackScripts[f].DebugEnabled = value;
                }
            }
        }

        /// <summary> returns the distance (in m) of the fingers inside a SG_Material collider, provided they are touching one. </summary>
        public float[] ColliderDistances
        {
            get
            {
                float[] res = new float[this.fingerFeedbackScripts.Length];
                for (int f = 0; f < fingerFeedbackScripts.Length; f++)
                {
                    res[f] = fingerFeedbackScripts[f].DistanceInCollider;
                }
                return res;
            }
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------
        // Functions

        /// <summary> Returns true if at least one collider is touching a material. </summary>
        /// <returns></returns>
        public bool TouchingMaterial()
        {
            for (int f = 0; f < this.fingerFeedbackScripts.Length; f++)
            {
                if (this.fingerFeedbackScripts[f].IsTouching()) { return true; }
            }
            return false;
        }

        /// <summary> Set ignoreCollision between this layer and another set of rigidbodies. </summary>
        /// <param name="otherLayer"></param>
        /// <param name="ignoreCollision"></param>
        public void SetIgnoreCollision(SG_HandRigidBodies otherLayer, bool ignoreCollision)
        {
            if (otherLayer != null)
            {
                if (this.wristFeedbackScript != null) { otherLayer.SetIgnoreCollision(this.wristFeedbackScript.gameObject, ignoreCollision); }
                for (int f=0; f<fingerFeedbackScripts.Length; f++)
                {
                    if (fingerFeedbackScripts[f] != null)
                    {
                        otherLayer.SetIgnoreCollision(fingerFeedbackScripts[f].gameObject, ignoreCollision);
                    }
                }
            }
        }


        /// <summary> Sets up this script's components to link to the same glove and the appropriate hand section. </summary>
        public void SetupScripts()
        {
            if (fingerFeedbackScripts.Length < 5)
            {
                Debug.LogWarning(this.name + " has only " + fingerFeedbackScripts.Length + "/5 finger scripts connected," +
                    " and will not provide feedback to all fingers.");
            }
            for (int f = 0; f < fingerFeedbackScripts.Length && f < 5; f++)
            {
                fingerFeedbackScripts[f].linkedGlove = this.gloveHardware;
                fingerFeedbackScripts[f].feedbackScript = this;
                fingerFeedbackScripts[f].handLocation = (SG_HandSection)f;
                if (handModel != null)
                {
                    Transform target;
                    if (handModel.GetFingerTip(fingerFeedbackScripts[f].handLocation, out target))
                    {
                        fingerFeedbackScripts[f].SetTrackingTarget(target, true);
                    }
                }
            }
            if (wristFeedbackScript != null)
            {
                wristFeedbackScript.linkedGlove = this.gloveHardware;
                wristFeedbackScript.handLocation = SG_HandSection.Wrist;
                if (handModel != null)
                {
                    wristFeedbackScript.SetTrackingTarget(handModel.wristTransform, true);
                }
            }
        }
        /// <summary> Retrieve the forces for each finger and send these to the glove. </summary>
        public void UpdateForces()
        {
            if (Hardware != null)
            {
                int[] forceLevels = new int[5];
                for (int f = 0; f < forceLevels.Length; f++)
                {
                    if (fingerFeedbackScripts.Length > f)
                    {
                        forceLevels[f] = fingerFeedbackScripts[f].ForceLevel;
                    }
                }
                Hardware.SendCmd(new SGCore.Haptics.SG_FFBCmd(forceLevels));
            }
        }
        /// <summary> Checks for scripts that might be connected to this GameObject. Used in editor and during startup. </summary>
        public override void CheckForScripts()
        {
            base.CheckForScripts();
            if (this.gloveHardware == null)
            {
                this.gloveHardware = this.Hardware;
            }
            SG_Util.CheckForHandInfo(this.transform, ref this.handModel);
        }
        void Stop()
            {
                //Detiene el FFB y Haptics que se le mandan al usuario 
                this.Hardware.StopAllVibrations();
                this.Hardware.StopHaptics();
                Debug.Log("FFB Off");
            }
        //-------------------------------------------------------------------------------------------------------------------------------------------
        // Monobehaviour
        void Awake()
        {
            SetupScripts();
            handSide = handSides[0];
            index = 0;

        }
            // IEnumerator ExampleCoroutine()  
            // {
            //     //Print the time of when the function is first called.
            //     //Debug.Log("Started Coroutine at timestamp : " + Time.time);

            //     //yield on a new YieldInstruction that waits for 5 seconds.
            //     yield return new WaitForSeconds(1);
                

            //     //After we have waited 5 seconds print the time again.
            //     //Debug.Log("Finished Coroutine at timestamp : " + Time.time);
            // }
        void Start()
        {
            script = server.GetComponent<UDPReceive>();
        }
        
        // Update is called once per frame
        void Update()
        {       
            UpdateForces();
            if(Input.GetKeyDown("1"))
            {
                SendData = true;
                this.Hardware.ForceFeedbackEnabled = true;
            }
            if(Input.GetKeyDown("2"))
            {
                Stop();
                this.Hardware.ForceFeedbackEnabled = false;
                SendData=false;

                for(int i=0;i<5;i++)
                {
                    rightFA[i]=0;
                }
                // rightFA=0;
                for(int i=0;i<5;i++)
                {
                    leftFA[i]=0;
                }
                M=0;
            }
            if(Input.GetKeyDown("x"))
            {
                index++;
                if (index>1)
                {
                    index=0;
                }
                handSide=handSides[index];
            }                
            if(SendData)
                    {
                        // StartCoroutine(ExampleCoroutine());                        
                        
                        // if(String.Equals(message,emptyString))
                        // {
                        //     message = "0";
                        // }
                        message = script.lastReceivedUDPPacket;
                        string[] values = message.Trim('$').Split(',');

                        // rightMessage = values[4].Trim('$');
                        // leftMessage = Regex.Replace(message, "Left:","");

                        values[4] = Regex.Replace(values[4], "[^0-9]+", string.Empty);
                        
                        for(int i=0;i<=4;i++)
                        {
                            rightFA[i] = Int32.Parse(values[i]);
                            // Debug.Log(rightFA[i]);
                        }
                            // rightFA = Int32.Parse(rightMessage);
                            // leftFA = Int32.Parse(values[0]);

                        // foreach(var item in rightFA)
                        // {
                        //     Debug.Log(item);
                        // }
                        

                        // Debug.Log(leftFA);
                        // Debug.Log(rightMessage);
                        //Debug.Log(this.Hardware.ToString());
                        
                            if(String.Equals(this.Hardware.ToString(),handSide))
                            {
                                //Debug.Log("YES");
                                //int[] rightForceFB = new int[5];
                                for(int i=0; i<=4 ; i++)
                                {
                                    mappedSR[i] = rightFA[i] / maxForceS;
                                    forceMagnR[i]= ForceFBCR.Evaluate(mappedSR[i]);
                                    //CmdFFBR[i]=(int)(forceMagnR[i]*maxForceS);
                                    CmdFFBR[i] = (int)Mathf.Clamp(forceMagnR[i]*maxForceS,minForceS,maxForceS);
                                    Debug.Log("El valor del mapeo es: "+forceMagnR[i]);
                                    if(CmdFFBR[i]>0 || CmdFFBR[i]>90 )
                                    {
                                        FinR[i]=true;
                                        Mag=30;

                                    }
                                    if(CmdFFBR[i]==0  || CmdFFBR[i]>10 )
                                      {
                                        FinR[i]=false;
                                        Mag=100;
                                    }
                                    
                                    //Debug.Log("El valor del FFBR: "+CmdFFBR[i]);
                                    /*rightForceFB[i]=rightFA;
                                    //Debug.Log("FFB Activo");
                                    Debug.Log("El valor del FFB: "+rightFA);
                                    //Debug.Log(message);*/
                                }
                                this.Hardware.SendCmd(new SGCore.Haptics.SG_FFBCmd(CmdFFBR));
                                this.Hardware.SendCmd(buzz,FinR,Mag,duration_s);                            
                            }

                            // else
                            // {
                            //     int[] leftForceFB = new int[5];
                            //     for(int i=0; i<5 ; i++)
                            //     {
                            //         mappedSL[i] = leftFA / maxForceS;
                            //         forceMagnL[i]= ForceFBCL.Evaluate(mappedSR[i]);
                            //         CmdFFBL[i] = (int)Mathf.Clamp(forceMagnL[i]*maxForceS,minForceS,maxForceS);
                            //         if(CmdFFBL[i]>0)
                            //         {
                            //             FinL[i]=true;
                            //         }
                            //         Debug.Log("El valor del FFBR: "+CmdFFBL);

                            //         /*leftForceFB[i]=leftFA;
                            //         //Debug.Log("FFB Activo");
                            //         Debug.Log("El valor del FFB: "+leftFA);
                            //         //Debug.Log(message);*/
                            //     }
                            //     this.Hardware.SendCmd(new SGCore.Haptics.SG_FFBCmd(CmdFFBL));
                            //     this.Hardware.SendCmd(buzz,FinL,magnitude,duration_s);
                                
                                
                            // }       
                    }
                        
        }

    }

}