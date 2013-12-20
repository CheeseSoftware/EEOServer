using System;
namespace Movement
{

    public class Config : object
    {
        public readonly string playerio_game_id = "everybody-edits-su9rn58o40itdbnw69plyw";
        public readonly int server_type_version = 176;
        public readonly string server_type_normalroom = "Everybodyedits";// + 176/*server_type_version*/;
        public readonly string server_type_betaroom = "Beta";// + server_type_version;
        public readonly string server_type_guestserviceroom = "LobbyGuest";// + server_type_version;
        public readonly string server_type_serviceroom = "Lobby";// + server_type_version;
        public readonly string server_type_authroom = "Auth";// + server_type_version;
        public readonly string server_type_blacklistroom = "QuickInviteHandler";// + server_type_version;
        public readonly string server_type_tutorialroom = "Tutorial";// + server_type_version + "_world_";
        public readonly string server_type_trackingroom = "Tracking";// + server_type_version;
        public readonly string url_blog = "http://blog.everybodyedits.com";
        public readonly string url_clubmember_about_page = "http://everybodyedits.com/club";
        public readonly string url_terms_page = "http://everybodyedits.com/terms";
        public readonly string url_help_page = "http://everybodyedits.com/help";
        public readonly bool use_debug_server = false;
        public readonly bool run_in_development_mode = false;
        public readonly bool show_disabled_shopitems = false;
        public readonly string development_mode_autojoin_room = "PWvOaRIeIvbUI";
        public readonly string debug_news = "";
        public readonly string developer_server = "127.0.0.1:8184";
        public readonly bool forceArmor = false;
        public readonly string armor_userid = null;
        public readonly string armor_authtoken = null;
        public readonly bool forceMouseBreaker = false;
        public readonly string mousebreaker_authtoken = null;
        public readonly bool forceBeta = false;
        public readonly bool show_debug_profile = true;
        public readonly string debug_profile = "";
        public readonly bool disableCookie = false;
        public readonly bool show_debug_friendrequest = false;
        public readonly string debug_friendrequest = "";
        public readonly bool show_blacklist_invitation = false;
        public readonly string debug_invitation = "";
        public readonly int physics_ms_per_tick = 10;
        public readonly double physics_variable_multiplyer = 7.752;
        public readonly double physics_base_drag = Math.Pow(0.9981, 10/*physics_ms_per_tick*/) * 1.00016;
        public readonly double physics_no_modifier_drag = Math.Pow(0.99, 10/*physics_ms_per_tick*/) * 1.00016;
        public readonly double physics_water_drag = Math.Pow(0.995, 10/*physics_ms_per_tick*/) * 1.00016;
        public readonly double physics_mud_drag = Math.Pow(0.975, 10/*physics_ms_per_tick*/) * 1.00016;
        public readonly double physics_jump_height = 26;
        public readonly double physics_gravity = 2;
        public readonly double physics_boost = 16;
        public readonly double physics_water_buoyancy = -0.5;
        public readonly double physics_mud_buoyancy = 0.4;
        public readonly int physics_queue_length = 2;
        public readonly int shop_potion_max = 10;
        public readonly double camera_lag = 0.0625;
        public readonly bool isMobile = false;
        public readonly bool enableDebugShadow = false;
        public readonly int maxwidth = 850;
        public readonly int minwidth = 640;
        public readonly int width = 640;
        public readonly int height = 500;
        public readonly int maxFrameRate = 120;
        public readonly int max_daily_woot = 10;
        public readonly uint guest_color = 3355443;
        public readonly uint default_color = 15658734;
        public readonly uint default_color_dark = 13421772;
        public readonly uint friend_color = 65280;
        public readonly uint friend_color_dark = 47872;
        public readonly uint mod_color = 16759552;
        public readonly uint admin_color = 16757760;
        public readonly string[] tutorial_names = { "Moving", "Gravity", "Edit" };
        public readonly bool disable_tracking = false;

        public Config()
        {
            return;
        }// end function

    }
}
