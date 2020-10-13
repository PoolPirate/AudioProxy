using Common.Configuration;

namespace AudioProxy.Options
{
    [SectionName("General")]
    public class GeneralOptions : Option
    {
        public bool FirstLaunch { get; set; } = true;

        public GeneralOptions()
        {
        }
    }
}
