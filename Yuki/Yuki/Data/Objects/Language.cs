using Nett;
using System;
using Yuki.Core;

namespace Yuki.Data.Objects
{
    public class Language
    {
        public string Code { get; set; }

        public TomlTable data;

        public string GetString(string stringName)
        {
            string name;

            try
            {
                name = data[stringName].ToString();
            }
            catch (NullReferenceException)
            {
                name = stringName;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                if(Code != "en_US")
                {
                    string english = Localization.GetLanguage("en_US").GetString(stringName);

                    if (!string.IsNullOrWhiteSpace(english))
                    {
                        return english;   
                    }
                }

                return stringName;
            }

            return name;
        }
    }
}
