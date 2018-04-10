using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Movement : MonoBehaviour, ISelectHandler, IDeselectHandler
{
	public float speed = 1.0f;
	public float rotationAngle = 1.0f;
	public Transform pivot;
	public Vector3 centerOfMass;
	protected Rigidbody rb;

    public AudioClip footsteps;
    AudioSource playerSource;

	private Button _turnLeftButton;
	private Button _turnRightButton;

	static bool _turnLeftPressed;
	static bool _turnRightPressed;

    Vector3 prevPosition;
    public float measuredSpeed;

    // Use this for initialization
    void Start () 
	{
        playerSource = this.gameObject.GetComponent<AudioSource>();
		rb = GetComponent<Rigidbody>();
		rb.centerOfMass = centerOfMass;


		if (!_turnLeftButton) _turnLeftButton = GameObject.Find("TurnLeftButton").GetComponent<Button>();
	   // _turnLeftButton.onClick.AddListener(LeftButtonClicked);
   
		if (!_turnRightButton) _turnRightButton = GameObject.Find("TurnRightButton").GetComponent<Button>();
        // _turnRightButton.onClick.AddListener(RightButtonClicked);

        prevPosition = transform.position;
        measuredSpeed = 0;

	}

	public void Update()
	{
		_turnLeftPressed = false;
		_turnRightPressed = false;
		if (Input.touchCount > 0)
		{
			foreach(Touch touch in Input.touches)
			{
				if(touch.position.x < Screen.width / 2)
				{
					_turnLeftPressed = true;
				}
				else
				{
					_turnRightPressed = true;
				}
			}
        }

        measuredSpeed = Mathf.Lerp(measuredSpeed, (transform.position - prevPosition).magnitude / Time.deltaTime, 0.5f);
        prevPosition = transform.position;
    }

	// Update is called once per frame
	protected void FixedUpdate () 
	{
		bool canMove = InputLeft() || InputRight();

		if (InputLeft() && !InputRight())
		{
			//Move in a counter clockwise direction if left arrow pressed.
			transform.RotateAround(pivot.position, new Vector3(0.0f, 1.0f, 0.0f), -rotationAngle * Time.deltaTime);
		}
		else if (InputRight() && !InputLeft())
		{
			//Move in a clockwise direction if right arrow pressed.
			transform.RotateAround(pivot.position, new Vector3(0.0f, 1.0f, 0.0f), rotationAngle * Time.deltaTime);
		}

		if (canMove)
		{
            if(!playerSource.isPlaying)
            {
                playerSource.PlayOneShot(footsteps);
            }

            Vector3 currentVelocity = rb.velocity;
            currentVelocity.y = 0;

            Vector3 velocity = (transform.forward * speed) * Time.deltaTime;
            transform.position = new Vector3(transform.position.x + velocity.x, transform.position.y, transform.position.z + velocity.z);
            //rb.MovePosition(new Vector3(transform.position.x + velocity.x, transform.position.y, transform.position.z + velocity.z));
            //rb.velocity = velocity;
            //rb.velocity = Vector3.zero;
            //rb.AddForce(velocity, ForceMode.VelocityChange);
            //rb.AddForce(velocity, ForceMode.Acceleration);

            //change direction but not speed
            //rb.velocity = (velocity.normalized * currentVelocity.magnitude) + (Vector3.up * rb.velocity.y);
            //accelerate towards wanted speed
            //rb.AddForce(velocity.normalized * (velocity.magnitude - currentVelocity.magnitude + currentVelocity.magnitude) * 100, ForceMode.Acceleration);
		}
	}

	public static bool InputLeft() { return (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) || _turnLeftPressed); }
	public static bool InputRight() { return (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) || _turnRightPressed); }
	public static bool InputUp() { return (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)); }

	void LeftButtonClicked()
	{
		_turnLeftPressed = true;
	}

	void RightButtonClicked()
	{
		_turnRightPressed = true;
	}

	public void OnSelect(BaseEventData eventData)
	{
		//Debug.Log("On Select");
		if(eventData.selectedObject.gameObject.name.Equals("TurnLeftButton"))
		{
			_turnLeftPressed = true;
		}

		if(eventData.selectedObject.gameObject.name.Equals("TurnRightButton"))
		{
			_turnRightPressed = true;
		}
	}

	public void OnDeselect(BaseEventData eventData)
	{
		//Debug.Log("On Deselect");
		if (eventData.selectedObject.gameObject.name.Equals("TurnLeftButton"))
		{
			_turnLeftPressed = false;
		}

		if (eventData.selectedObject.gameObject.name.Equals("TurnRightButton"))
		{
			_turnRightPressed = false;
		}
	}
}
