using UnityEngine;

namespace Game.Ingame.Comp
{
    public class ParallaxManager : MonoBehaviour
    {
        public ParallaxLayer[] layers;

        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            foreach (var layer in layers)
            {
            }
        }
    }
}