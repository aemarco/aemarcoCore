using aemarcoCore.Properties;

namespace aemarcoCore
{
    public class DllSettings
    {

        string test = Settings.Default.Server + "\\" + Settings.Default.Share;


        public void Test()
        {

            var test = Settings.Default.Server + "\\" + Settings.Default.Share;

        }


        public DllSettings()
        {

        }


    }
}
