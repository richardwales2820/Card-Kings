using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour {

    public GameObject cam;
    public Vector3 newPosition;
    public Quaternion endRotation;
    public float moveSpeed = 1.0F;
    public float rotateSpeed = 1.0F;
    private float startTime;
    private float journeyLength;
    private Vector3 startPos;
    private Quaternion startRot;
    void Start()
    {
        
        startTime = Time.time;
        journeyLength = Vector3.Distance(cam.transform.position, newPosition);
        Destroy(GameObject.Find("Main Menu"));
    }
    void Update()
    {
        float distCovered = (Time.time - startTime) * moveSpeed;
        float fracJourney = distCovered / journeyLength;
        transform.position = Vector3.Lerp(startPos, newPosition, fracJourney);
        
    }

    private void OnEnable()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        transform.rotation = Quaternion.Euler(new Vector3(27.195f, -179.716f, 0f));
    }
}
