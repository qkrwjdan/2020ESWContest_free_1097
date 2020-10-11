using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseballPlayerController : MonoBehaviour
{
	private Animator animator;

	private GameObject ball;
	private BallController ballController;

	// hand
	private GameObject hand;

	// get HandController script (for catch_ball)
	private HandController handController;

	// Start is called before the first frame update
	void Start()
    {
		animator = this.GetComponent<Animator>();
		ball = GameObject.Find("Ball");
		ballController = ball.GetComponent<BallController>();

		// find Hand object
		hand = GameObject.Find("Hand");
		handController = hand.GetComponent<HandController>();
	}

    // Update is called once per frame
    void Update()
    {
		// set value for animator
		if (Input.GetKeyUp(KeyCode.S) && !handController.isHolding)
		{
			animator.SetBool("isThrowing", true);
		}
		else
		{
			animator.SetBool("isThrowing", false);
		}
	}

	void throwBall()
	{
		if (!handController.isHolding)
		{
			ballController.throwBall = true;
		}
	}
}
