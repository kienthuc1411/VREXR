using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent
{
    // Camera change 
    public static string UPDATE_CAMERA_POSITION = "game:update_camera_position";
    public static string SHUFFLE_TO_LAST = "game:shuffle_to_last"; // Shuffle Theater to last element of layout grid

    // Camera effect events
    public static string SHOW_CAMERA_BLUR = "game:show_camera:blur";
    public static string HIDE_CAMERA_BLUR = "game:hide_camera:blur";

    // Profile UI events
    public static string ON_PROFILE_CHANGED = "game:on_profile_changed";
    public static string ON_CLOSE_AVATAR_LIST = "game:on_close_avatar_list";

}
