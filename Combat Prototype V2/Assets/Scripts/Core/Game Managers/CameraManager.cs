using UnityEngine;
using System;
using System.Collections;

namespace Wayfinder {
  public class CameraManager : MonoBehaviour {
    public static CameraManager Instance { get; private set; }

    public float cameraSpeed;

    public int scrollBound;
    public int scrollSpeed;
    private float screenWidth;
    private float screenHeight;
    private float smoothSpeed = 2.0f;

    public int zoomSpeed;
    // The target size of the orthographic camera (zoom), calculated when zooming
    private float targetOrtho;
    private float minOrtho = 3.0f;
    private float maxOrtho = 10.0f;

    private float minCameraX;
    private float maxCameraX;
    private float minCameraY;
    private float maxCameraY;

    // Lock manual camera movement
    private bool locked;
    private IEnumerator cameraCoroutine;

    public event EventHandler CameraEdgeScroll;
    public event EventHandler CameraZoom;

    void Awake() {
      if (!Instance) {
        Instance = this;
        DontDestroyOnLoad(gameObject);
      }
      else
        Destroy(gameObject);
    }

    void Start () {
      this.screenWidth = Screen.width;
      this.screenHeight = Screen.height;
      this.targetOrtho = Camera.main.orthographicSize;

      this.CalculateCameraBounds();
    }

    public void EdgeScroll(Vector3 direction) {
      this.transform.position = new Vector3(
        Mathf.Clamp(this.transform.position.x + direction.x * Time.deltaTime, this.minCameraX, this.maxCameraX),
        Mathf.Clamp(this.transform.position.y + direction.y * Time.deltaTime, this.minCameraY, this.maxCameraY),
        this.transform.position.z + direction.z * Time.deltaTime
      );
    }

    void Update() {
      if (!this.locked) {
        this.ScrollCameraOnScreenEdge();
        this.ZoomCameraOnMouseWheel();
      }
    }

    public void ZoomScroll(float zoom) {
      this.targetOrtho -= zoom * this.zoomSpeed;
      this.targetOrtho = Mathf.Clamp(this.targetOrtho, this.minOrtho, this.maxOrtho);

      Camera.main.orthographicSize = Mathf.MoveTowards(
        Camera.main.orthographicSize,
        this.targetOrtho,
        this.smoothSpeed
        );

      this.RecenterCameraWithinBounds();
    }

    public void FocusCameraOn(dynamic target) {
      GameObject targetGameObject = target as GameObject;

      if (this.cameraCoroutine != null) StopCoroutine(this.cameraCoroutine);
      this.cameraCoroutine = FocusCameraOnCoroutine(targetGameObject);
      StartCoroutine(this.cameraCoroutine);
    }

    private void InvokeCameraScroll(Vector3 direction) {
      CameraEventArgs cameraEventArgs = new CameraEventArgs();
      cameraEventArgs.direction = direction;

      this.CameraEdgeScroll.Invoke(this, cameraEventArgs);
    }

    private void InvokeCameraZoom(float zoom) {
      CameraEventArgs cameraEventArgs = new CameraEventArgs();
      cameraEventArgs.zoom = zoom;

      this.CameraZoom.Invoke(this, cameraEventArgs);
    }

    private void ScrollCameraOnScreenEdge() {
      if (Input.mousePosition.x > this.screenWidth - this.scrollBound) {
        this.InvokeCameraScroll(new Vector3(this.scrollSpeed, 0, 0));
      }
      if (Input.mousePosition.x < 0 + this.scrollBound) {
        this.InvokeCameraScroll(new Vector3(-this.scrollSpeed, 0, 0));
      }
      if (Input.mousePosition.y > this.screenHeight - this.scrollBound) {
        this.InvokeCameraScroll(new Vector3(0, this.scrollSpeed, 0));
      }
      if (Input.mousePosition.y < 0 + this.scrollBound) {
        this.InvokeCameraScroll(new Vector3(0, -this.scrollSpeed, 0));
      }
    }

    private void ZoomCameraOnMouseWheel() {
      if (Input.GetAxis("Mouse ScrollWheel") != 0f) {
        this.InvokeCameraZoom(Input.GetAxis("Mouse ScrollWheel"));
      }
    }

    private void CalculateCameraBounds() {
      this.minCameraX = Cell.cellBoundsMinX + Camera.main.orthographicSize * Camera.main.aspect;
      this.maxCameraX = Cell.cellBoundsMaxX - Camera.main.orthographicSize * Camera.main.aspect;
      this.minCameraY = Cell.cellBoundsMinY + Camera.main.orthographicSize;
      this.maxCameraY = Cell.cellBoundsMaxY - Camera.main.orthographicSize;
    }

    private void RecenterCameraWithinBounds() {
      this.CalculateCameraBounds();
      this.transform.position = new Vector3(
        Mathf.Clamp(this.transform.position.x, this.minCameraX, this.maxCameraX),
        Mathf.Clamp(this.transform.position.y, this.minCameraY, this.maxCameraY),
        this.transform.position.z
      );
    }

    private IEnumerator FocusCameraOnCoroutine(GameObject target) {
      Vector3 targetPosition = new Vector3(
        Mathf.Clamp(target.transform.position.x, this.minCameraX, this.maxCameraX),
        Mathf.Clamp(target.transform.position.y, this.minCameraY, this.maxCameraY),
        this.transform.position.z
        );

      while ((Vector3.Distance(this.transform.position, targetPosition) > 0.05f)) {
        this.locked = true;
        this.transform.position = Vector3.Lerp(
          this.transform.position,
          targetPosition,
          this.cameraSpeed * Time.deltaTime
        );

        yield return 0;
      }

      this.locked = false;
    }
  }

  public class CameraEventArgs : EventArgs {
    public Vector3 direction;
    public float zoom;
  }
}
