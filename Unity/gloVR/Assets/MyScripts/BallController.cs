using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
	//Serial Port Handler
	private SerialPortHandler SPHandler;
	
	// hand
	private GameObject hand;

	// get HandController class
	private HandController handController;

	// target vector
	private Vector3 startPos;
	private Vector3 targetPos;
	private Vector3 moveVector;

	// about difficulty & game
	private float speed;
	private float distance;
	private float range;
	public int level;

	public GameObject baseballPlayerBall;
	public bool throwBall;
	private bool isFlying;
	
	void Start()
    {

		try{
     		SPHandler = GameObject.Find("SP").GetComponent<SerialPortHandler>();
    	}
    	catch(System.Exception e){
      		Debug.Log(e);
    	}
		// find Hand object
		hand = GameObject.Find("Hand");

		handController = hand.GetComponent<HandController>();

		// put ball at start point
		startPos = baseballPlayerBall.transform.position; ;
		targetPos = startPos;
		this.transform.position = targetPos;
		moveVector = new Vector3(0, 0, 0);

		// initialization
		speed = 0.4f;
		distance = 1f;
		range = 1f;
		level = 0;

		isFlying = false;
		throwBall = false;
	}

    // Update is called once per frame
    void Update()
    {
		if (!isFlying)
		{
			this.transform.position = baseballPlayerBall.transform.position;
			targetPos = this.transform.position;
			moveVector = new Vector3(0, 0, 0);
		}

		if (throwBall && !handController.isHolding)
		{
			level = Random.Range(0, 4);

			try
			{
				SPHandler.setServo(level);
			}
			catch(System.Exception e)
			{
				Debug.Log(e);
			}

			// put ball at start point
			this.gameObject.SetActive(true);
			this.transform.position = baseballPlayerBall.transform.position;

			// set target position
			float targetX = Random.Range(35f - range, 35f + range);
			float targetY = Random.Range(2.5f - range, 2.5f + range) + 0.95f;
			if (targetY < 1.6f)
			{
				targetY = 1.6f;
			}
			float targetZ = hand.transform.position.z - 0.46f;
			
			targetPos = new Vector3(targetX, targetY, targetZ);
			moveVector = targetPos - this.transform.position;

			throwBall = false;
			isFlying = true;
		}
		
		if (isFlying && !throwBall)
		{
			this.transform.Translate(moveVector * Time.deltaTime * speed, Space.World);
			this.transform.Rotate(moveVector);
		}

		// catch ball (distance, z value, flex value)
		if (Vector3.Distance(this.transform.position, new Vector3(hand.transform.position.x, hand.transform.position.y + 0.95f, hand.transform.position.z)) < distance
			&& this.transform.position.z < (hand.transform.position.z - 0.46f)
			&& handController.thumbFlex < 140 
			&& handController.indexFingerFlex < 120
			&& handController.middleFingerFlex < 125
			&& handController.ringFingerFlex < 125
			&& handController.pinkyFlex < 135
			&& handController.thumbFlex > 120
			&& handController.indexFingerFlex > 100
			&& handController.middleFingerFlex > 105
			&& handController.ringFingerFlex > 105
			&& handController.pinkyFlex > 115
			&& isFlying)
		{
			isFlying = false;

			// put ball at start point
			this.transform.position = startPos;
			// stop ball
			targetPos = this.transform.position;

			handController.isCatch = true;
		}

		// fail ball
		if (this.transform.position.z > (hand.transform.position.z + 2f) && isFlying)
		{
			isFlying = false;

			// set hand motion's value
			handController.failBallNum = handController.failBallNum + 1;

			// put ball at start point
			this.transform.position = startPos;
			// stop ball
			targetPos = this.transform.position;

			if (handController.failBallNum == 1)
			{
				handController.baseball3.gameObject.SetActive(false);
			}

			if (handController.failBallNum == 2)
			{
				handController.baseball2.gameObject.SetActive(false);
			}

			if (handController.failBallNum == 3)
			{
				handController.baseball1.gameObject.SetActive(false);
			}
		}
	}
}
