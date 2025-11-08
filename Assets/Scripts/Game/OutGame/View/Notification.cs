using TMPro;
using UnityEngine;

namespace Game.OutGame.View
{
    public class Notification : MonoBehaviour
    {
        public TMP_Text notificationText;

        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
        }

        public void setNotification(string notification)
        {
            notificationText.text = notification;
        }
    }
}