using UnityEngine;

namespace Game.Ingame.Comp
{
    public class ParallaxLayer : MonoBehaviour
    {
        public Transform[] tiles;
        public Transform cameraTarget;
        public float tileWidth = 20.48f;
        public float scrollFactor = 0.2f;
        private int currentIndex = 1;
        private int firstIndex = 0;
        private int tileCount;
        float camHalfWidth;
        float camLeftEdge;
        float camRightEdge;
        public bool useStaticWidth;

        private Camera mainCamera;

        // Start is called before the first frame update
        private void Start()
        {
            mainCamera = BattleRoot.Instance.mainCamera;
            camHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
            if (!useStaticWidth)
            {
                tileWidth = getSpriteWidth();
            }
            tileCount = tiles.Length;
            camRightEdge = cameraTarget.position.x + camHalfWidth;
            camLeftEdge = cameraTarget.position.x - camHalfWidth;
        }

        // Update is called once per frame
        private void Update()
        {
            float camX = cameraTarget.position.x * scrollFactor;
            Vector3 basePos = new Vector3(camX, transform.position.y, transform.position.z);
            transform.position = basePos;

            float camRightEdge = cameraTarget.position.x + camHalfWidth;

            int lastIndex = (firstIndex + tileCount - 1) % tileCount;
            Transform lastTile = tiles[lastIndex];

            //检测相机是否超过了最后一张的左边缘
            float tileRightEdge = lastTile.position.x - tileWidth / 2f;

            if (camRightEdge > tileRightEdge)
            {
                ScrollRight();
            }
        }

        void ScrollRight()
        {
            int leftIndex = firstIndex;
            int lastIndex = (firstIndex + tileCount - 1) % tileCount;

            Transform leftTile = tiles[leftIndex];
            Transform rightTile = tiles[lastIndex];

            leftTile.position = new Vector3(
                rightTile.position.x + tileWidth,
                rightTile.position.y,
                rightTile.position.z
            );

            firstIndex = (firstIndex + 1) % tileCount;
        }

        private float getSpriteWidth()
        {
            Renderer[] renderers = tiles[0].GetComponentsInChildren<Renderer>();
            Bounds bounds = renderers[0].bounds;
            foreach (var r in renderers)
                bounds.Encapsulate(r.bounds);
            float width = bounds.size.x;
            return width;
        }
    }
}