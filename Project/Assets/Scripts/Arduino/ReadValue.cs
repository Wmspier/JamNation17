using UnityEngine;
using System.Collections;
using System.IO.Ports;


public class ReadValue : MonoBehaviour {
	SerialPort sp = new SerialPort("/dev/cu.usbmodemFA131", 9600);
	
	void Start () {
//		sp.Open ();
//		sp.ReadTimeout = 10;
	}
	
	void Update () 
	{
		try{
            string readValue = sp.ReadLine();
            if (readValue.Contains("P1:0"))
            {
                //Trigger something
                this.GetComponent<PlayerClimbBehavior>().climbTick();
               // print("OK");

            }
			//print (readValue);
		}
		catch(System.Exception){
		}
		
		
	}
}