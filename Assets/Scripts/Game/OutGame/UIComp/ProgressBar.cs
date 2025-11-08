using UnityEngine;
using UnityEngine.UI;

namespace Game.OutGame.UIComp
{
    public class ProgressBar : MonoBehaviour
    {
        public Slider slider;

        // Start is called before the first frame update
        private void Start()
        {
            //slider.value = 0;
        }

        // Update is called once per frame
        private void Update()
        {
            //if (slider.value < 1)
            //{
            //    slider.value += Time.deltaTime * 0.1f; // 增加进度
            //}
        }
    }
}