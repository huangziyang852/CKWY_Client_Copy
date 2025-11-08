using Game.Common.Const;
using Game.Core.Manager;
using Game.Core.Net.Service;
using Game.OutGame.View.Gacha;
using System.Runtime.CompilerServices;

namespace Game.OutGame.Controller
{
    public class GachaMgr : BaseMgr<GachaMgr>
    {
        private GachaPanel GachaPanel { get; set; }

        public override void Initialize()
        {
            UiManager.Instance.CloseAllPanel();

            UiManager.Instance.InitUIPanel(GameConst.UiPanel.GachaPanel, panel =>
            {
                GachaPanel = panel.GetComponent<GachaPanel>();
            });
            UiManager.Instance.InitSubPanel(GameConst.UiPanel.GuildPanel, panel => { });
            EventBus<GachaRequestEvent>.Subscribe(OnGachaRequest);
        }


        private async void OnGachaRequest(GachaRequestEvent e)
        {
            var gachaResult = await GachaService.Instance.GachaAsync(e.GachaId, e.Count);

            if (gachaResult != null && gachaResult.GachaHeroResult.Count > 0)
            {
                //await GameModelManager.Instance.RefreshHeroInfoAsync();
                EventBus<GachaResultEvent>.Publish(new GachaResultEvent(gachaResult));
            }
        }
    }
}