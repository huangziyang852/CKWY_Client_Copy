namespace Game.Common.Const
{
    public class GameConst
    {
        public class UiPanel
        {
            public const string LoginPanel = "LoginPanel";
            public const string LoadingPanel = "LoadingPanel";
            public const string HomePanel = "HomePanel";
            public const string GuildPanel = "GuildPanel";
            public const string HeroListPanel = "HeroListPanel";
            public const string GachaPanel = "GachaPanel";
            public const string BattleMapPanel = "BattleMapPanel";
            public const string GuajiPanel = "GuajiPanel";
            public const string ItemListPanel = "ItemListPanel";
        }

        public class Popup
        {
            public const string RigitserPopup = "RigitserPopup";
            public const string HeroShowPopup = "HeroShowPopup";
            public const string RewardCommonPopup = "RewardCommonPopup";
        }

        public class Notification
        {
            public const string SystemNotification = "SystemNotification";
        }

        public class Loading
        {
            public const string LoadingPanel = "LoadingPanel";
        }

        public class CurrencyId
        {
            public const int Gold = 101;
            public const int Diamond = 102;
            public const int NormalGachaTicket = 117;
            public const int SuperGachaTicket = 118;
            public const int FriendGachaTicket = 119;
        }

        public class PathCategory
        {
            public const string Ui = "Ui";
            public const string ServerIcon = "ServerIcon";
            public const string Sound = "Sound";
            public const string HeroImagePath = "HeroImagePath";
        }

        public class BgImageName
        {
            public const string Login = "HB_BG";
            public const string Home = "bg";
            public const string Gacha = "zh_bg";
        }

        public class UISpinePrefabName
        {
            public const string LoginSpine = "LoginSpine";
            public const string GachaBgSpine = "GachaBgSpine";
        }

        public class AudioName
        {
            public const string Home = "Home";
            public const string World = "World";
            public class UIAudioName
            {
                public const string Main = "main";
                public const string Hero = "hero";
                public const string Windows = "windows";
                public const string Gacha = "drawCard";
                public const string Page = "page";
                public const string GachaExcute = "combo";
                public const string Item = "screen";
                public const string Get = "get";
                public const string Union = "union";
            }
        }

        public class ServerState
        {
            public const string Maintenance = "maintenance";
            public const string New = "new";
            public const string Hot = "hot";
        }

        public class ImagesPath
        {
            public class Battle
            {
                public class Map
                {
                    public class Icon
                    {
                        public const string BUILDING_ICON = "Assets/Download/RemoteResource/Images/Battle/Map/Icon/";
                    }
                }
            }

            public class BG
            {
                public class Login
                {
                    public const string LOGIN = "Assets/Download/RemoteResource/Images/BG/Login/";
                }
                public class Home
                {
                    public const string HOME = "Assets/Download/RemoteResource/Images/BG/Home/";
                }
                public class Gacha
                {
                    public const string GACHA = "Assets/Download/RemoteResource/Images/UI/Gacha/";
                }
            }


            public class Character
            {
                public const string HERO_ICON = "Assets/Download/RemoteResource/Images/Character/HeroIcon/image_head_";
                public const string HERO_STAND = "Assets/Download/RemoteResource/Images/Character/Stand/image_stand_";
            }

            public class UI
            {
                public class Hero
                {
                    public class Rarity
                    {
                        public const string N = "Assets/Download/RemoteResource/Images/UI/Hero/cm_tag_N1";
                        public const string R = "Assets/Download/RemoteResource/Images/UI/Hero/cm_tag_R1";
                        public const string SR = "Assets/Download/RemoteResource/Images/UI/Hero/cm_tag_SR1";
                        public const string SSR = "Assets/Download/RemoteResource/Images/UI/Hero/cm_tag_SSR1";
                    }
                    public class Camp
                    {
                        public const string CAMP_RED = "Assets/Download/RemoteResource/Images/UI/Hero/cm_icon_ZhenYing2";
                        public const string CAMP_BLUE = "Assets/Download/RemoteResource/Images/UI/Hero/cm_icon_ZhenYing4";
                        public const string CAMP_GREEN = "Assets/Download/RemoteResource/Images/UI/Hero/cm_icon_ZhenYing5";
                        public const string CAMP_YELLOW = "Assets/Download/RemoteResource/Images/UI/Hero/cm_icon_ZhenYing1";
                        public const string CAMP_PURPLE = "Assets/Download/RemoteResource/Images/UI/Hero/cm_icon_ZhenYing0";
                    }
                }

                public const string ITEM = "Assets/Download/RemoteResource/Images/UI/Item/";
            }
        }

        public class PrefabPath
        {
            public class Batttle
            {
                public class Com
                {
                    public const string CHAPTER_ITEM_PRE =
                        "Assets/Download/RemoteResource/Prefabs/Battle/Com/ChapterItemPre";
                    public const string BATTLE_MAP_PRE = "Assets/Download/RemoteResource/Prefabs/Battle/Map/GameMap";
                }
                public class Unit
                {
                    public const string PLAYER_UNIT = "Assets/Download/RemoteResource/Prefabs/Battle/Unit/PlayerUnit";
                    public const string ENEMY_UNIT = "Assets/Download/RemoteResource/Prefabs/Battle/Unit/EnemyUnit";
                }
                public class Effect
                {
                    public const string BULLET = "Assets/Download/RemoteResource/Prefabs/Battle/Effect/Bullet";
                }
            }

            public const string LOGIN = "Assets/Download/RemoteResource/Prefabs/UiPanel/Login/";
            public const string GACHA = "Assets/Download/RemoteResource/Prefabs/UiPanel/Gacha/";
        }

        public class SpinePath
        {
            public class Battle
            {
                public class Map
                {
                    public const string BUILDING_SPINE = "Assets/Download/RemoteResource/Spine/Battle/Map/JuDian";
                }

                public const string HERO_SPINE_PATH = "Assets/Download/RemoteResource/Spine/Battle/Role/";
                public const string HERO_ATTACK_EFFECT_SPINE_PATH = "Assets/Download/RemoteResource/Spine/Battle/Effect/";
            }

            public class UISpine
            {
                public class Gacha
                {
                    public const string CARD_NORMAL = "Assets/Download/RemoteResource/Spine/UiSpine/Gacha/ZhaoHuan_PuTong";
                    public const string CARD_FRIEND = "Assets/Download/RemoteResource/Spine/UiSpine/Gacha/ZhaoHuan_YouQing";
                    public const string CARD_SUPER = "Assets/Download/RemoteResource/Spine/UiSpine/Gacha/ZhaoHuan_GaoJi";
                }
            }
        }

        public class SoundPath
        {
            public class Effect
            {
                public const string SKILL_SOUND_PATH = "Assets/Download/RemoteResource/Sounds/Effect/Skill/";
            }

            public const string UI_SOUND_PATH = "Assets/Download/RemoteResource/Sounds/UI/";
        }
    }

    public static class UserPrefs
    {
        public static string LAST_USER_ID = "LastUserId";
        public static string LAST_SERVERID = "LastServerId";
        public static string LOGIN_TOKEN = "LoginToken";
        public static string OPEN_ID = "OpenId";
        public static string REFRESH_TOKEN = "RefreshToken";
    }
}