using UnityEngine;
using System.Collections;

namespace RootMotion
{
    public class CameraController : MonoBehaviour
    {
        [System.Serializable]
        public enum UpdateMode
        {
            Update,
            FixedUpdate,
            LateUpdate,
            FixedLateUpdate
        }

        public Transform target;
        public Transform rotationSpace;
        public UpdateMode updateMode = UpdateMode.LateUpdate;
        public bool lockCursor = true;

        [Header("Position")]
        public bool smoothFollow;
        public Vector3 offset = new Vector3(0, 1.5f, 0.5f);
        public float followSpeed = 10f;

        [Header("Rotation")]
        public float rotationSensitivity = 3.5f;
        public float yMinLimit = -20;
        public float yMaxLimit = 80;
        public bool rotateAlways = true;
        public bool rotateOnLeftButton;
        public bool rotateOnRightButton;
        public bool rotateOnMiddleButton;

        [Header("Distance")]
        public float distance = 10.0f;
        public float minDistance = 4;
        public float maxDistance = 10;
        public float zoomSpeed = 10f;
        public float zoomSensitivity = 1f;

        [Header("Blocking")]
        public LayerMask blockingLayers;
        public float blockingRadius = 1f;
        public float blockingSmoothTime = 0.1f;
        public float blockingOriginOffset;
        [Range(0f, 1f)] public float blockedOffset = 0.5f;

        public float x { get; private set; }
        public float y { get; private set; }
        public float distanceTarget { get; private set; }

        private Vector3 targetDistance, position;
        private Quaternion rotation = Quaternion.identity;
        private Vector3 smoothPosition;
        private Camera cam;
        private bool fixedFrame;
        private float fixedDeltaTime;
        private Quaternion r = Quaternion.identity;
        private Vector3 lastUp;
        private float blockedDistance = 10f, blockedDistanceV;

        public void SetAngles(Quaternion rotation)
        {
            Vector3 euler = rotation.eulerAngles;
            this.x = euler.y;
            this.y = euler.x;
        }

        public void SetAngles(float yaw, float pitch)
        {
            this.x = yaw;
            this.y = pitch;
        }

        protected virtual void Awake()
        {
            Vector3 angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;

            distanceTarget = distance;
            smoothPosition = transform.position;

            cam = GetComponent<Camera>();

            lastUp = rotationSpace != null ? rotationSpace.up : Vector3.up;
        }

        protected virtual void Update()
        {
            if (updateMode == UpdateMode.Update) UpdateTransform();
        }

        protected virtual void FixedUpdate()
        {
            fixedFrame = true;
            fixedDeltaTime += Time.deltaTime;
            if (updateMode == UpdateMode.FixedUpdate) UpdateTransform();
        }

        protected virtual void LateUpdate()
        {
            UpdateInput();

            if (updateMode == UpdateMode.LateUpdate) UpdateTransform();

            if (updateMode == UpdateMode.FixedLateUpdate && fixedFrame)
            {
                UpdateTransform(fixedDeltaTime);
                fixedDeltaTime = 0f;
                fixedFrame = false;
            }
        }

        public void UpdateInput()
        {
            if (!cam.enabled) return;

            Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = lockCursor ? false : true;

            bool rotate = rotateAlways || (rotateOnLeftButton && Input.GetMouseButton(0)) || (rotateOnRightButton && Input.GetMouseButton(1)) || (rotateOnMiddleButton && Input.GetMouseButton(2));

            if (rotate)
            {
                x += Input.GetAxis("Mouse X") * rotationSensitivity;
                y = ClampAngle(y - Input.GetAxis("Mouse Y") * rotationSensitivity, yMinLimit, yMaxLimit);
            }

            distanceTarget = Mathf.Clamp(distanceTarget + zoomAdd, minDistance, maxDistance);
        }

        public void UpdateTransform()
        {
            UpdateTransform(Time.deltaTime);
        }

        public void UpdateTransform(float deltaTime)
        {
            if (!cam.enabled) return;

            rotation = Quaternion.AngleAxis(x, Vector3.up) * Quaternion.AngleAxis(y, Vector3.right);

            if (rotationSpace != null)
            {
                r = Quaternion.FromToRotation(lastUp, rotationSpace.up) * r;
                rotation = r * rotation;

                lastUp = rotationSpace.up;

            }

            if (target != null)
            {
                distance += (distanceTarget - distance) * zoomSpeed * deltaTime;

                if (!smoothFollow) smoothPosition = target.position;
                else smoothPosition = Vector3.Lerp(smoothPosition, target.position, deltaTime * followSpeed);

                Vector3 t = smoothPosition + rotation * offset;
                Vector3 f = rotation * -Vector3.forward;

                if (blockingLayers != -1)
                {
                    RaycastHit hit;
                    if (Physics.SphereCast(t - f * blockingOriginOffset, blockingRadius, f, out hit, blockingOriginOffset + distanceTarget - blockingRadius, blockingLayers))
                    {
                        blockedDistance = Mathf.SmoothDamp(blockedDistance, hit.distance + blockingRadius * (1f - blockedOffset) - blockingOriginOffset, ref blockedDistanceV, blockingSmoothTime);
                    }
                    else blockedDistance = distanceTarget;

                    distance = Mathf.Min(distance, blockedDistance);
                }

                position = t + f * distance;

                transform.position = position;
            }

            transform.rotation = rotation;
        }

        private float zoomAdd
        {
            get
            {
                float scrollAxis = Input.GetAxis("Mouse ScrollWheel");
                if (scrollAxis > 0) return -zoomSensitivity;
                if (scrollAxis < 0) return zoomSensitivity;
                return 0;
            }
        }

        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360) angle += 360;
            if (angle > 360) angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }

    }
}