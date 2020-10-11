using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControlGameOverScene : MonoBehaviour
{
	private Text text;

	private SerialPortHandler SPHandler;
	private UDPHandler UHandler;
	string end_string = "s3e";

    // Start is called before the first frame update
    void Start()
    {

		try{
      		SPHandler = GameObject.Find("SP").GetComponent<SerialPortHandler>();
    	}
    	catch(Exception e){
      		Debug.Log(e);
    	}

    	try{
      		UHandler = GameObject.Find("UP").GetComponent<UDPHandler>();
    	}
    	catch(Exception e){
      		Debug.Log(e);
    	}

		text = GetComponent<Text>();
		text.color = new Color(text.color.r, text.color.g, text.color.b, 0);

	}

    // Update is called once per frame
    void Update()
    {
		if (text.color.a < 1.0f)
		{
			text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime/3.0f));
		}

		//serial port로 데이터 보내기
      	try{
        	SPHandler.SendString(end_string);
      	}
      	catch(Exception e){
        	Debug.Log(e);
      	}
		Debug.Log("Send End_string");

      	//udp로 데이터 보내기
      	try{
        	UHandler.SendString("1");
			UHandler.StopThread();
      	}
      	catch(Exception e){
        	Debug.Log(e);
      	}




		if (text.color.a >= 1.0f)
		{
			SceneManager.LoadScene("MainMenu_0");
		}
    }
}
