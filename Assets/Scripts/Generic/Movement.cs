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

    private float _turnLeftRatio;

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
        _turnLeftRatio = 0.5f;
	}

	public void Update()
	{
        if (InputLeft() && InputRight())
            _turnLeftRatio = 0.5f;
        else if (InputLeft())
            _turnLeftRatio = 1;
        else if (InputRight())
            _turnLeftRatio = 0;

        if (Input.touchCount > 0)
		{
            Vector2? leftTouchPosition = null;
            Vector2? rightTouchPosition = null;

            // Get left and right touches
			foreach(Touch touch in Input.touches)
			{
                Vector2 touchPosition = touch.position;
				if(touchPosition.x < Screen.width / 2)
				{
                    leftTouchPosition = touchPosition;

                    if (rightTouchPosition != null)
                        break;
				}
				else
				{
                    rightTouchPosition = touchPosition;

                    if (leftTouchPosition != null)
                        break;
				}
			}

            if (leftTouchPosition != null && rightTouchPosition == null)
                _turnLeftRatio = 1;
            else if (leftTouchPosition == null && rightTouchPosition != null)
                _turnLeftRatio = 0;
            else
            {
                // Min Position is right - half screen height
                float heightAtMinimumLeftRatio = ((Vector2)rightTouchPosition).y - Screen.height / 2;
                float heightAtMaximumLeftRatio = ((Vector2)rightTouchPosition).y + Screen.height / 2;

                _turnLeftRatio = ((((Vector2)leftTouchPosition).y - heightAtMinimumLeftRatio) / (heightAtMaximumLeftRatio - heightAtMinimumLeftRatio));
            }
        }

        measuredSpeed = Mathf.Lerp(measuredSpeed, (transform.position - prevPosition).magnitude / Time.deltaTime, 0.5f);
        prevPosition = transform.position;
    }

	// Update is called once per frame
	protected void FixedUpdate () 
	{
		bool canMove = false;

		if (InputLeft() || InputRight())
		{
			//Only move in a forward direction if both buttons pressed.
			canMove = true;

            // Decompress ratio as rotation angle unit
            float decompressedRotationAngle = -(_turnLeftRatio * 2 - 1);
            Debug.Log(decompressedRotationAngle);
            transform.RotateAround(pivot.position, Vector3.up, rotationAngle * decompressedRotationAngle * Time.deltaTime);
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
            //transform.position = new Vector3(transform.position.x + velocity.x, transform.position.y, transform.position.z + velocity.z);
            rb.MovePosition(new Vector3(transform.position.x + velocity.x, transform.position.y, transform.position.z + velocity.z));
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

	public static bool InputLeft() { return (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)); }
	public static bool InputRight() { return (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)); }
	public static bool InputUp() { return (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)); }

	void LeftButtonClicked()
	{
        _turnLeftRatio = 1;
	}

	void RightButtonClicked()
	{
        _turnLeftRatio = 0;
	}

	public void OnSelect(BaseEventData eventData)
	{
		//Debug.Log("On Select");
		if(eventData.selectedObject.gameObject.name.Equals("TurnLeftButton"))
		{
			//_turnLeftPressed = true;
		}

		if(eventData.selectedObject.gameObject.name.Equals("TurnRightButton"))
		{
			//_turnRightPressed = true;
		}
	}

	public void OnDeselect(BaseEventData eventData)
	{
		//Debug.Log("On Deselect");
		if (eventData.selectedObject.gameObject.name.Equals("TurnLeftButton"))
		{
			//_turnLeftPressed = false;
		}

		if (eventData.selectedObject.gameObject.name.Equals("TurnRightButton"))
		{
			//_turnRightPressed = false;
		}
	}
}
