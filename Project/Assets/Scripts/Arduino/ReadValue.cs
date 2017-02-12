using UnityEngine;
using System.Collections;
using System.IO.Ports;


public class ReadValue : MonoBehaviour {
	SerialPort sp = new SerialPort("COM4", 9600);
	
	void Start () {
		sp.Open ();
		sp.ReadTimeout = 10;
	}
	
	void Update () 
	{
		try{
			print (sp.ReadLine());
		}
		catch(System.Exception){
		}
		
		
	}
}