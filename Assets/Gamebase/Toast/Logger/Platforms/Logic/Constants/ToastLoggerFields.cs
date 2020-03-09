﻿using System;
using System.Reflection;

namespace Toast.Logger
{
    public static class ToastLoggerFields
    {
        public const string PROJECT_KEY = "projectName";
        public const string PROJECT_VERSION = "projectVersion";

        // Log info.
        
        public const string LOG_VERSION = "logVersion";        
        public const string LOG_TYPE = "logType";
        public const string LOG_SOURCE = "logSource";
        public const string LOG_LEVEL = "logLevel";
        public const string LOG_MESSAGE = "body";
        public const string LOG_SEND_TIME = "sendTime";
        public const string LOG_CREATE_TIME = "createTime";
        public const string LOG_BULK_INDEX = "lncBulkIndex";
        public const string LOG_TRANSACTION_ID = "transactionID";

        // Device info.
        
        public const string DEVICE_MODEL = "DeviceModel";
        public const string CARRIER_NAME = "Carrier";
        public const string COUNTRY_CODE = "CountryCode";
        public const string PLATFORM_NAME = "Platform";

        // Network info.
        
        public const string NETWORK_TYPE = "NetworkType";

        // Identify info.
        
        public const string DEVICE_ID = "DeviceID";
        public const string SESSION_ID = "SessionID";
        public const string LAUNCHED_ID = "launchedID";
        public const string USER_ID = "UserID";

        // Version info.
        
        public const string SDK_VERSION = "SdkVersion";

        // Crash Info.
        
        public const string CRASH_STYLE = "CrashStyle";
        public const string CRASH_SYMBOL = "SymMethod";
        public const string CRASH_DUMP_DATA = "dmpData";

        // iOS Crash Info. Add for filter consistency.
        
        public const string FREE_MEMORY = "FreeMemory";
        public const string FREE_DISKSPACE = "FreeDiskSpace";

        // These fields generated by symbolication in server.
        
        public const string SINK_VERSION = "SinkVersion";
        public const string ERROR_CODE = "errorCode";
        public const string CARSH_META = "crashMeta";
        public const string SYMBOLICATION_RESULT = "SymResult";
        public const string EXCEPTION_TYPE = "ExceptionType";
        public const string LOCATION = "Location";
        public const string ISSUE_ID = "lncIssueID";

        // bool isReservedField(string name)
        private static bool IsReservedField(string name)
        {
            string[] fieldInfo = {
                PROJECT_KEY, PROJECT_VERSION,
                LOG_VERSION, LOG_TYPE, LOG_SOURCE, LOG_LEVEL, LOG_MESSAGE, LOG_SEND_TIME, LOG_CREATE_TIME, LOG_BULK_INDEX, LOG_TRANSACTION_ID,
                DEVICE_MODEL, SESSION_ID, LAUNCHED_ID, USER_ID,
                SDK_VERSION,
                CRASH_STYLE, CRASH_SYMBOL, CRASH_DUMP_DATA,
                FREE_MEMORY, FREE_DISKSPACE,
                SINK_VERSION, ERROR_CODE, CARSH_META, SYMBOLICATION_RESULT, EXCEPTION_TYPE, LOCATION, ISSUE_ID
            };

            for (int i = 0; i < fieldInfo.Length; i++)
            {
                if (fieldInfo[i].ToLower().Equals(name.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }

        public static string ConvertField(string field)
        {
            string convertedField = field.Replace(' ', '_');

            if (!IsReservedField(convertedField))
            {
                return convertedField;
            }

            return "reserved_" + convertedField;
        }
    }
}