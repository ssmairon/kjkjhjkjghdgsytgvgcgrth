using System;
using System.ComponentModel;

namespace Lovatto.Localization
{
    [Serializable]
    public enum LocalizationLanguageCode
    {
        None = -1,
        [Description("es")]
        Spanish = 0,
        [Description("en")]
        English = 1,
        [Description("fr")]
        French = 2,
        [Description("zh-cn")]
        Chinese = 3,
        [Description("pt")]
        Portuguese = 4,
        [Description("de")]
        German = 5,
        [Description("it")]
        Italian = 6,
        [Description("ru")]
        Russian = 7,
        [Description("jp")]
        Japan = 8,
        [Description("ar")]
        Arabic = 9,
        [Description("hi")]
        Hindi = 10,
        [Description("cs")]
        Czech = 11,
        [Description("pl")]
        Polish = 12,
        [Description("tr")]
        Turkish = 13,
    }
}