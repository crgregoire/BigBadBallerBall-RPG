﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public float baseSpeed = 100;
    public float speed;
    //private bool isHost = false;
    public bool noBackwards = true;
    private RotateAbout camScript;
    private float moveHorizontal = 0;

    private Rigidbody rb;
    private int count;// Reference to the UI's health bar.

    // The colour the damageImage is set to, to flash.

    public void SpeedIncrease(float increase)
    {
        speed += baseSpeed + increase;
    }
    public void OnDeath()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
    

    void Start()
    {
        speed = baseSpeed;
    camScript = GameObject.Find("Main Camera").GetComponent<RotateAbout>();
        rb = GetComponent<Rigidbody>();
}

    void FixedUpdate()
    {
        var angle = Camera.main.transform.eulerAngles.y;
        Vector3 Direction = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), 0f, Mathf.Cos(Mathf.Deg2Rad * angle));
        float moveVertical = Input.GetAxis("Vertical");
        if (noBackwards)
        {
            moveVertical = Mathf.Abs(moveVertical);
        }
        

        Vector3 movement = Vector3.Normalize((new Vector3(Direction.z * (moveHorizontal), 0f, Direction.x * (-moveHorizontal)) + Vector3.Scale(Direction, new Vector3(moveVertical, 0f, moveVertical)))/2);
        
        rb.AddForce(movement * speed);
       
    }
    void Update()
    {

        camScript.distance += Input.GetAxis("Mouse ScrollWheel") * 5;
        //gameObject.transform.eulerAngles = new Vector3(0, rotationAboutYAxis, 0);
    }
}