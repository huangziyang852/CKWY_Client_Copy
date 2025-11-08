using Game.Common.Const;
using Game.Core.Manager;
using Game.OutGame.View.Home;

namespace Game.OutGame.Controller
{
    public class HomeMgr : BaseMgr<HomeMgr>
    {
        private GuildPanel guildPanel;
        private HomePanel homePanel;

        public override void Initialize()
        {
            //ResourceLoader.Instance.PreloadGroup(ResourceLoader.ResourceGroup.Home);
            UiManager.Instance.CloseAllPanel();
            InitHomePanel();
        }

        private void InitHomePanel()
        {
            UiManager.Instance.InitUIPanel(GameConst.UiPanel.HomePanel,
                panel => { homePanel = panel.GetComponent<HomePanel>(); });

            UiManager.Instance.InitSubPanel(GameConst.UiPanel.GuildPanel,
                panel => { guildPanel = panel.GetComponent<GuildPanel>(); });
                        SoundManager.Instance.PlayBGM(GameConst.AudioName.Home);
            SoundManager.Instance.PlayBGM(GameConst.AudioName.Home);
        }
    }
}