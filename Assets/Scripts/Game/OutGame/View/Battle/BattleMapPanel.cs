using Game.Core.GameRoot;
using Game.Core.Manager;
using Game.Ingame;
using Game.OutGame.Controller;
using UnityEngine.UI;

namespace Game.OutGame.View.Battle
{
    public class BattleMapPanel : BasePanel<BattleMapPanel>
    {
        public Button backBtn;

        // Start is called before the first frame update
        private void Start()
        {
            backBtn.onClick.AddListener(async () =>
            {
                BattleRoot.Instance.mapCamera.gameObject.SetActive(false);
                await GameRoot.Instance.TransiationToScene("Home", () => { HomeMgr.Instance.Initialize(); });
            });
        }

        // Update is called once per frame
        private void Update()
        {
        }

        private void OnEnable()
        {
        }
    }
}