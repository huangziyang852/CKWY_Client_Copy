using UnityEngine;

namespace Game.OutGame.UIComp
{
    [ExecuteAlways]
    public class RoundButtonCom : MonoBehaviour
    {
        public Transform center;
        public GameObject[] roundButtons;
        public float radius = 100f;
        public float startAngle;
        public float endAngle = 180f;
        private bool isArranged;

        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
        }

        private void OnEnable()
        {
            if (!isArranged)
            {
                ArrangeButtonsInCircle();
                isArranged = true;
            }
        }

        private void OnValidate()
        {
            ArrangeButtonsInCircle();
        }

        private void ArrangeButtonsInCircle()
        {
            if (roundButtons == null || roundButtons.Length == 0) return;

            var count = roundButtons.Length;
            if (count == 1)
            {
                // 只有一个按钮时，直接放置在起始角度
                PlaceButton(roundButtons[0].transform, startAngle);
                return;
            }

            var angleStep = (endAngle - startAngle) / (count - 1);

            for (var i = 0; i < count; i++)
            {
                var angle = startAngle + angleStep * i; // 计算当前按钮的角度
                PlaceButton(roundButtons[i].transform, angle);
            }
        }

        private void PlaceButton(Transform button, float angle)
        {
            var radian = angle * Mathf.Deg2Rad; // 角度转弧度

            // 计算按钮的位置
            var pos = center.position + new Vector3(Mathf.Cos(radian) * radius, Mathf.Sin(radian) * radius, 0);
            button.position = pos; // 设置按钮位置

            // 让按钮朝向圆心
            var direction = center.position - button.position; // 计算朝向
            var angleToCenter = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // 计算角度
            button.rotation = Quaternion.Euler(0, 0, angleToCenter + 90f); // 旋转按钮
        }
    }
}