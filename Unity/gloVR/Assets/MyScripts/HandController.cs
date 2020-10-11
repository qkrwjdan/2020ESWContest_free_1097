using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class HandController : MonoBehaviour
{
    //Serial Port Handler
    private SerialPortHandler SPHandler;

	//Udp Socket Handler
	private UDPHandler UHandler;

    //received data from arduino;
    int[] flexData = new int[5];
    float[] ypr = new float[3];

	//data for openCV
	float beforeXPos;
	float beforeYPos;
	float beforeZPos;

	private GameObject hand;

	// finger
	private GameObject[] thumb = new GameObject[3];
	private GameObject[] indexFinger = new GameObject[3];
	private GameObject[] middleFinger = new GameObject[3];
	private GameObject[] ringFinger = new GameObject[3];
	private GameObject[] pinky = new GameObject[3];

	// finger flex data
	public int thumbFlex;
	public int indexFingerFlex;
	public int middleFingerFlex;
	public int ringFingerFlex;
	public int pinkyFlex;

	// catch ball
	private GameObject catchBallObject;
	private GameObject duplicateCatchBall;
	private int score;

	// state value
	public bool isCatch;
	public bool isHolding;

	// fail ball
	public int failBallNum;

	// baseball UI
	public GameObject baseball1;
	public GameObject baseball2;
	public GameObject baseball3;

	// UI
	public Text scoreText;

	// mouse click point
	private Vector3 mouseWorldPosition;

    void Start()
	{
        //get SPHandler
		try
		{
      		SPHandler = GameObject.Find("SP").GetComponent<SerialPortHandler>();
			SPHandler.DiscardBuffer();
    	}
    	catch(Exception e)
		{
      		Debug.Log(e);
    	}
		
		//get UHandler
    	try
		{
			print("GET UHandler");
      		UHandler = GameObject.Find("UP").GetComponent<UDPHandler>();
    	}
    	catch(Exception e)
		{
      		Debug.Log(e);
    	}

		// catch ball object
		catchBallObject = GameObject.Find("Catch_Ball");
		catchBallObject.gameObject.SetActive(false);

		// score
		score = 0;
		scoreText.text = string.Format("Score: {0}", score);
		failBallNum = 0;

		// catching
		isCatch = false;
		isHolding = false;

		// position
		mouseWorldPosition = hand.transform.position;

		// flex data
		thumbFlex = 0;
		indexFingerFlex = 0;
		middleFingerFlex = 0;
		ringFingerFlex = 0;
		pinkyFlex = 0;

		// read all Children of current object
		Transform[] allChildren = GetComponentsInChildren<Transform>();

		// iterate
		foreach (Transform child in allChildren)
		{
			// find hand object
			if (child.name == "Hand")
				hand = child.gameObject;

			// find thumb object
			if (child.name == "thumb_0")
				thumb[0] = child.gameObject;
			if (child.name == "thumb_1")
				thumb[1] = child.gameObject;
			if (child.name == "thumb_2")
				thumb[2] = child.gameObject;

			// find index finger object
			if (child.name == "index_finger_1")
				indexFinger[0] = child.gameObject;
			if (child.name == "index_finger_2")
				indexFinger[1] = child.gameObject;
			if (child.name == "index_finger_3")
				indexFinger[2] = child.gameObject;

			// find middle finger object
			if (child.name == "middle_finger_1")
				middleFinger[0] = child.gameObject;
			if (child.name == "middle_finger_2")
				middleFinger[1] = child.gameObject;
			if (child.name == "middle_finger_3")
				middleFinger[2] = child.gameObject;

			// find ring finger object
			if (child.name == "ring_finger_1")
				ringFinger[0] = child.gameObject;
			if (child.name == "ring_finger_2")
				ringFinger[1] = child.gameObject;
			if (child.name == "ring_finger_3")
				ringFinger[2] = child.gameObject;

			// find pinky object
			if (child.name == "pinky_1")
				pinky[0] = child.gameObject;
			if (child.name == "pinky_2")
				pinky[1] = child.gameObject;
			if (child.name == "pinky_3")
				pinky[2] = child.gameObject;
		}
	}

	void Update()
	{
		SPHandler.ReceiveArduinoData(ref flexData, ref ypr);
		
		// not holding ball (rotate finger, rotate hand)
		if(!isHolding)
		{
			rotateFinger(flexData);
			hand.transform.rotation = Quaternion.Euler(ypr[1] * -1f,ypr[2] * -1f,ypr[0]);
		}

		// holding ball (rotate finger)
		if(isHolding)
		{
			setFingerValue(flexData);
		}
		
		if(UHandler.newData)
		{
			moveHand();
		}

		// catch ball
		if (isCatch)
		{
			try{
				SPHandler.setServo(0);
				SPHandler.SendVibe();
			}
			catch(Exception e){
				Debug.Log(e);
			}
				
			isHolding = true;

			gripHand();

			// create catch ball  as hand's child
			duplicateCatchBall = Instantiate(catchBallObject) as GameObject;
			duplicateCatchBall.transform.parent = this.transform;
			duplicateCatchBall.transform.localPosition = new Vector3(0, -0.04f, -0.115f);
			duplicateCatchBall.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

			// add score and update UI
			score = score + 10;
			scoreText.text = string.Format("Score: {0}", score);

			isCatch = false;
		}

		// catching ball
		if (isHolding)
		{
			duplicateCatchBall.gameObject.SetActive(true);
		}

		// check holding
		if (isHolding
			&& thumbFlex > 140
			&& indexFingerFlex > 120
			&& middleFingerFlex > 125
			&& ringFingerFlex > 125
			&& pinkyFlex > 135)
		{
			isHolding = false;
			duplicateCatchBall.transform.parent = null;
			duplicateCatchBall.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
		}

		// exit game
		if (failBallNum > 2)
		{
			SceneManager.LoadScene("GameOverScene");
		}
	}


	void setFingerValue(int[] intDataArr)
	{
		thumbFlex = intDataArr[0];
		indexFingerFlex = intDataArr[1];
		middleFingerFlex = intDataArr[2];
		ringFingerFlex = intDataArr[3];
		pinkyFlex = intDataArr[4];
	}

	void moveHand()
	{
		string text = UHandler.text;

		Vector3 handScreenPosition = Camera.main.WorldToScreenPoint(hand.transform.position);

    	int index1 = text.IndexOf(',');
    	int index2 = text.IndexOf(',',index1+1);
        
    	String string_xpos = text.Substring(0,index1);
    	String string_ypos = text.Substring(index1+1,index2 - index1 - 1);
        String string_zpos = text.Substring(index2+1,text.Length - index2 - 1);

    	float xPos = float.Parse(string_xpos);
    	float yPos = float.Parse(string_ypos);
        float zPos = float.Parse(string_zpos);

    	//filter1
    	xPos = (float)(xPos * 0.8 + beforeXPos * 0.2);
    	yPos = (float)(yPos * 0.8 + beforeYPos * 0.2);
        zPos = (float)(zPos * 0.8 + beforeZPos * 0.2);

		xPos = 1800 - (xPos * 2.857f);
		yPos = 900 - (yPos * 1.914f);
		zPos = zPos * 0.0032f + 1.5f;

		//filter2
    	if( ((beforeXPos - xPos) * (beforeXPos - xPos) > 10) || ((beforeYPos - yPos) * (beforeYPos - yPos) > 10))
    	{

      		mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(xPos, yPos, zPos));

      		hand.transform.position = new Vector3(mouseWorldPosition.x,mouseWorldPosition.y,mouseWorldPosition.z);

      		beforeXPos = xPos;
      		beforeYPos = yPos;
			beforeZPos = zPos;
    	}
		
		UHandler.newData = false;
	}

	void rotateFinger(int[] intDataArr)
	{

		int[] rotateDegree = new int[5];

		for (int i = 0; i < 5; i++)
		{
			rotateDegree[i] = intDataArr[i] - 180;
		}

		thumb[0].transform.localEulerAngles = new Vector3(-28.32f, ((-rotateDegree[4] - 160) / 5), -25.86f);
		thumb[1].transform.localEulerAngles = new Vector3(2.37f, -0.297f, -rotateDegree[4]) * 0.5f;
		thumb[2].transform.localEulerAngles = new Vector3(1.36f, -0.126f, -rotateDegree[4]) * 0.3f;

		indexFinger[0].transform.localEulerAngles = new Vector3(rotateDegree[3], 0, 0) * 0.5f;
		indexFinger[1].transform.localEulerAngles = new Vector3(rotateDegree[3], 0, 0) * 0.8f;
		indexFinger[2].transform.localEulerAngles = new Vector3(rotateDegree[3], 0, 0) * 0.3f;

		middleFinger[0].transform.localEulerAngles = new Vector3(rotateDegree[2], 0, 0) * 0.5f;
		middleFinger[1].transform.localEulerAngles = new Vector3(rotateDegree[2], 0, 0) * 0.8f;
		middleFinger[2].transform.localEulerAngles = new Vector3(rotateDegree[2], 0, 0) * 0.3f;

		ringFinger[0].transform.localEulerAngles = new Vector3(rotateDegree[1], 0, 0) * 0.5f;
		ringFinger[1].transform.localEulerAngles = new Vector3(rotateDegree[1], 0, 0) * 0.8f;
		ringFinger[2].transform.localEulerAngles = new Vector3(rotateDegree[1], 0, 0) * 0.3f;

		pinky[0].transform.localEulerAngles = new Vector3(rotateDegree[0], 0, 0) * 0.5f;
		pinky[1].transform.localEulerAngles = new Vector3(rotateDegree[0], 0, 0) * 0.8f;
		pinky[2].transform.localEulerAngles = new Vector3(rotateDegree[0], 0, 0) * 0.3f;

		setFingerValue(intDataArr);
	}

	public void gripHand()
	{
		// set finger as grip ball & set finger's flex value
		int[] gripFlex = new int[5];

		gripFlex[0] = 125;
		gripFlex[1] = 115;
		gripFlex[2] = 115;
		gripFlex[3] = 110;
		gripFlex[4] = 130;

		rotateFinger(gripFlex);
	}
}