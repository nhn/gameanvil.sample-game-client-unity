using System.Collections.Generic;
using UnityEngine;

namespace Toast.GamebaseTools.Util.Multilanguage.Internal
{
    using StringDictionary = Dictionary<string, string>;

    public class MultilanguageServiceData
    {
        public const string DEFALUT_LANGUAGE_CODE = "en";

        private string selectLanguage;
        private StringDictionary selectLanguageStrings;
        private Dictionary<string, StringDictionary> languagePacks = new Dictionary<string, StringDictionary>();
        
        public MultilanguageResultCode Initialize(MulitlanguageXml languageXml)
        {
            if (languageXml.stringList == null)
            {
                return MultilanguageResultCode.FILE_PARSING_ERROR;
            }

            foreach (var data in languageXml.stringList.list)
            {
                string key = data.key;
                var languages = data.language;

                foreach (var language in languages.list)
                {
                    string languageCode = language.LocalName;

                    StringDictionary languageStringMap;
                    if (languagePacks.TryGetValue(languageCode, out languageStringMap) == false)
                    {
                        languageStringMap = new StringDictionary();
                        languagePacks.Add(languageCode, languageStringMap);
                    }

                    string value = language.InnerText.Replace("\\n", "\n");
                    if (languageStringMap.ContainsKey(key) == true)
                    {
                        Debug.LogWarning(string.Format("Already have a string key. (key: {0}, language: {1})", key, languageCode));
                    }
                    else
                    {
                        languageStringMap.Add(key, value);
                    }
                }
            }

            return InitializeLanguageCode(languageXml.defaultData);
        }

        public MultilanguageResultCode SelectLanguage(string languageCode)
        {
            if (languagePacks.ContainsKey(languageCode) == false)
            {
                Debug.LogError("Language code not found.");
                return MultilanguageResultCode.LANGUAGE_CODE_NOT_FOUND;
            }

            selectLanguage = languageCode;
            selectLanguageStrings = languagePacks[selectLanguage];

            return MultilanguageResultCode.SUCCESS;
        }

        public IEnumerable<string> GetSupportLanguages()
        {
            return languagePacks.Keys;
        }

        public string GetString(string stringKey)
        {
            if (selectLanguageStrings == null)
            {
                Debug.LogError("Language file is not loaded.");
                return stringKey;
            }

            if (selectLanguageStrings.ContainsKey(stringKey) == false)
            {
                Debug.LogError(string.Format("String key not found. (key= {0})", stringKey));
                return stringKey;
            }

            string value = selectLanguageStrings[stringKey];
            if (string.IsNullOrEmpty(value) == true)
            {
                Debug.LogError(string.Format("String value is null or empty. (key= {0})", stringKey));
                return stringKey;
            }

            return value;
        }
        
        public string GetSelectLanguage()
        {
            return selectLanguage;
        }

        private MultilanguageResultCode InitializeLanguageCode(MulitlanguageXml.DefaultData defaultData)
        {
            if (defaultData != null && string.IsNullOrEmpty(defaultData.language) == false &&
                languagePacks.ContainsKey(defaultData.language) == true)
            {
                selectLanguage = defaultData.language;
            }
            else if (languagePacks.ContainsKey(DEFALUT_LANGUAGE_CODE) == true)
            {
                selectLanguage = DEFALUT_LANGUAGE_CODE;
            }
            else if (languagePacks.Count > 0)
            {
                foreach (var key in languagePacks.Keys)
                {
                    selectLanguage = key;
                    break;
                }
            }
            else
            {
                return MultilanguageResultCode.LANGUAGE_LIST_EMPTY;
            }

            SelectLanguage(selectLanguage);

            return MultilanguageResultCode.SUCCESS;
        }
    }
}
