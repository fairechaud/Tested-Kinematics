using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class testReceivedString : MonoBehaviour
{
    public GameObject server; // you will need this if scriptB is in another GameObject
                     // if not, you can omit this
                     // you'll realize in the inspector a field GameObject will appear
                     // assign it just by dragging the game object there
    public UDPReceive script; // this will be the container of the script
    

    void Start(){
        // first you need to get the script component from game object A
        // getComponent can get any components, rigidbody, collider, etc from a game object
        // giving it <scriptA> meaning you want to get a component with type scriptA
        // note that if your script is not from another game object, you don't need "a."
        // script = a.gameObject.getComponent<scriptA>(); <-- this is a bit wrong, thanks to user2320445 for spotting that
        // don't need .gameObject because a itself is already a gameObject
        script = server.GetComponent<UDPReceive>();
    }

    void Update(){
        // and you can access the variable like this
        // even modifying it works
        string emptyString = "";
        //script.ReceiveData();
        if(!String.Equals(script.lastReceivedUDPPacket,emptyString))
        {
            Debug.Log("From testReceivedString: "+script.lastReceivedUDPPacket);
        }
    }
}

