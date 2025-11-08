using Game.Common.Const;
using Game.Core.Manager;
using Game.OutGame.View.Hero;

namespace Game.OutGame.Controller
{
    public class HeroMgr : BaseMgr<HeroMgr>
    {
        private HeroListPanel heroListPanel { get; set; }

        public override void Initialize()
        {
            InitHeroListPanel();
        }

        private void InitHeroListPanel()
        {
            //UiManager.Instance.CloseUIPanel(GameConst.UiPanel.HomePanel);
            UiManager.Instance.InitUIPanel(GameConst.UiPanel.HeroListPanel, panel =>
            {
                heroListPanel = panel.GetComponent<HeroListPanel>();
                heroListPanel.Init();
            });
            UiManager.Instance.InitSubPanel(GameConst.UiPanel.GuildPanel, panel => { });
        }
    }
}