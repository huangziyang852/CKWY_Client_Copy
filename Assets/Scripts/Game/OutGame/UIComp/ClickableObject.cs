using System;
using UnityEngine;

namespace Game.OutGame.UIComp
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class ClickableObject : MonoBehaviour
    {
        public Vector2 customSize = Vector2.zero;
        public Action onClick;

        private void Reset()
        {
            RefreshCollider();
        }

        private void Start()
        {
            RefreshCollider();
        }

        private void OnMouseDown()
        {
            onClick?.Invoke();
        }

        /// <summary>
        ///     ˢ�µ������ʹ���Զ����С�򱾵����ţ�
        /// </summary>
        public void RefreshCollider()
        {
            var box = GetComponent<BoxCollider2D>();

            if (customSize != Vector2.zero)
                box.size = customSize;
            else
                // ʹ�����屾���� localScale ��Ϊ�������
                box.size = transform.localScale;

            box.offset = Vector2.zero;
        }

        public void SetOnClick(Action callback)
        {
            onClick = callback;
        }
    }
}