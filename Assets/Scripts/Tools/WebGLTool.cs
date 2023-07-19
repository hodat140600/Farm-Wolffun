using System.Runtime.InteropServices;

namespace FarmWolffun
{

    /// <summary>
    /// If you have issue finding the IsMobile function, you need to add a special file in your Assets/Plugins/WebGL folder
    /// </summary>

    public class WebGLTool
    {

#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern bool IsMobile();
#endif

        public static bool isMobile()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return IsMobile();
#else
            return false;
#endif
        }

    }

}