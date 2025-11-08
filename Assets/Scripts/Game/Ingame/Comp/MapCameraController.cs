using UnityEngine;

namespace Game.Ingame.Comp
{
    public class MapCameraController : MonoBehaviour
    {
        public float dragSpeed = 0.1f; //拖拽速度
        public Vector2 minPosition; // 最小位置 (X,Z)
        public Vector2 maxPosition; // 最大位置(X,Z)
        private bool isDragging;

        private Vector3 lastTouchPosition;
        private readonly float longPressThreshold = 0.1f; // 长按判定时间
        private float touchStartTime;

        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            HandleMouseDrag();
#else
    HandleTouchDrag();
#endif
        }

        private void HandleTouchDrag()
        {
            if (Input.touchCount == 1)
            {
                var touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    touchStartTime = Time.time;
                    lastTouchPosition = touch.position;
                    isDragging = false;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    if (Time.time - touchStartTime > longPressThreshold)
                    {
                        Vector3 delta = touch.deltaPosition;
                        MoveCamera(delta);
                        isDragging = true;
                    }
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    isDragging = false;
                }
            }
        }

        private void HandleMouseDrag()
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchStartTime = Time.time;
                lastTouchPosition = Input.mousePosition;
                isDragging = false;
            }
            else if (Input.GetMouseButton(0))
            {
                if (Time.time - touchStartTime > longPressThreshold)
                {
                    var delta = Input.mousePosition - lastTouchPosition;
                    lastTouchPosition = Input.mousePosition;

                    MoveCamera(delta);
                    isDragging = true;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }
        }

        private void MoveCamera(Vector3 delta)
        {
            var move = new Vector3(-delta.x, -delta.y, 0) * dragSpeed * Time.deltaTime;
            var newPosition = transform.position + move;
            newPosition.x = Mathf.Clamp(newPosition.x, minPosition.x, maxPosition.x);
            newPosition.y = Mathf.Clamp(newPosition.y, minPosition.y, maxPosition.y);
            transform.position = newPosition;
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }
    }
}