using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    // For camera on unit focusing
    public Transform UnitsParent;
    public float cameraSpeed;

    // For keeping the camera within the map boundaries
    public Transform CurrentMap;
    private float minX;
    private float maxX;
    private float minY;
    private float maxY;

    // Variables for screen edge camera scrolling
    public int scrollBound;
    public int scrollSpeed;
    private bool focusing;      // Used to prevent camera scrolling when camera is in process of focusing on something
    private float screenWidth;
    private float screenHeight;

    // Use this for initialization
    void Start () {
        
        // Subscribe to unit camera focus events
        for (int i = 0; i < UnitsParent.childCount; i++)
        {
            var unit = UnitsParent.GetChild(i).GetComponent<Unit>();
            if (unit != null)
            {
                unit.CameraFocusOn += CameraFocusOn;
                unit.CameraFocusOff += CameraFocusOff;
            }
            else
                Debug.LogError("Invalid object in Units Parent game object for the Main Camera");
        }

        // Calculate max and min camera positions for clamping
        Renderer rend = CurrentMap.GetComponent<Renderer>();
        Camera cam = Camera.main;

        minX = -rend.bounds.extents.x + cam.orthographicSize * cam.aspect;
        maxX = rend.bounds.extents.x - cam.orthographicSize * cam.aspect;
        minY = -rend.bounds.extents.y + cam.orthographicSize;
        maxY = rend.bounds.extents.y - cam.orthographicSize;

        // Get screen size
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        focusing = false;
    }

    private void CameraFocusOn(object sender, EventArgs e)
    {
        var unit = sender as Unit;
        unit.cameraFocus = true;
        StartCoroutine(FocusOn(unit));
    }

    private void CameraFocusOff(object sender, EventArgs e)
    {
        var unit = sender as Unit;
        unit.cameraFocus = false;
        focusing = false;
        StopCoroutine(FocusOn(unit));
    }

    private IEnumerator FocusOn(Unit unit)
    {
        // Get unit position we want to focus on, but clamp it within the bounds of the map (so the camera stays in bounds)
        var targetPos = new Vector3(Mathf.Clamp(unit.transform.position.x, minX, maxX),
            Mathf.Clamp(unit.transform.position.y, minY, maxY),
            transform.position.z);
        // Move camera
        while ((Vector3.Distance(transform.position, targetPos) > 0.05f) && unit.cameraFocus)
        {
            focusing = true;    // Prevent edge scrolling
            transform.position = Vector3.Lerp(transform.position, targetPos, cameraSpeed * Time.deltaTime);
            yield return 0;
        }

        focusing = false;   // Enable edge scrolling
    }

    // Update is called once per frame
    void FixedUpdate () {

        // Scroll Camera when mouse reaches edges
        if (!focusing)
        {
            if (Input.mousePosition.x > screenWidth - scrollBound)
            {
                transform.position = new Vector3(Mathf.Clamp(transform.position.x + scrollSpeed * Time.deltaTime, minX, maxX), transform.position.y, transform.position.z);
            }
            if (Input.mousePosition.x < 0 + scrollBound)
            {
                transform.position = new Vector3(Mathf.Clamp(transform.position.x - scrollSpeed * Time.deltaTime, minX, maxX), transform.position.y, transform.position.z);
            }
            if (Input.mousePosition.y > screenHeight - scrollBound)
            {
                transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y + scrollSpeed * Time.deltaTime, minY, maxY), transform.position.z);
            }
            if (Input.mousePosition.y < 0 + scrollBound)
            {
                transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y - scrollSpeed * Time.deltaTime, minY, maxY), transform.position.z);
            }
        }
	}
}
