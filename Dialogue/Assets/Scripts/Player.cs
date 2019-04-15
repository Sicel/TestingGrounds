using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public float movementSpeed = 0.5f;
    public float displayedTime = 0;

    public Vector2 teleportPosition;
    //Vector2 returnPosition;

    public Text keepReading;
    public GameObject textBox;
    public GameObject choiceBox;

    public Rigidbody2D rigidBody;

	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        Move();
        KeepReading();
    }

    // Moves player using rigid body
    void Move()
    {
        float xPosition = transform.position.x;
        float yPosition = transform.position.y;
        Vector2 movement = new Vector2(xPosition, yPosition);
        if (Input.GetKey("w"))
        {
            movement += new Vector2(0, movementSpeed) * Time.deltaTime;
        }
        if (Input.GetKey("s"))
        {
            movement += new Vector2(0, -movementSpeed) * Time.deltaTime;
        }
        if (Input.GetKey("a"))
        {
            movement += new Vector2(-movementSpeed, 0) * Time.deltaTime;
        }
        if (Input.GetKey("d"))
        {
            movement += new Vector2(movementSpeed, 0) * Time.deltaTime;
        }
        rigidBody.MovePosition(movement);
    }

    // Goes to next line when space is pressed
    void KeepReading()
    {
        if (textBox.activeSelf || choiceBox.activeSelf)
        {
            rigidBody.constraints = RigidbodyConstraints2D.FreezeAll; // Stops movement when text is being displayed
            displayedTime += Time.deltaTime;
            if (textBox.activeSelf)
            {
                // Prevents going to next line immediately after conversation starts
                if (Input.GetKeyDown(KeyCode.Space) && displayedTime >= 0.5f)
                    keepReading.GetComponent<Button>().onClick.Invoke();
            }
            if (choiceBox.activeSelf)
            {
                return;
            }
        }
        // Allows movement
        else
        {
            rigidBody.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
            displayedTime = 0;
        }
    }

    public void Teleport()
    {
        Debug.Log("first " + rigidBody.position);
        //returnPosition = rigidBody.position;
        transform.position = teleportPosition;
        rigidBody.MovePosition(teleportPosition);
        Debug.Log(rigidBody.position);
    }
}
