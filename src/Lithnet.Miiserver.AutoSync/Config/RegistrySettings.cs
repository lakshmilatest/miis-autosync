﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using System.Text;

namespace Lithnet.Miiserver.AutoSync
{
    internal static class RegistrySettings
    {
        private static RegistryKey key;

        private static HashSet<string> retryCodes;

        public static RegistryKey BaseKey
        {
            get
            {
                if (RegistrySettings.key == null)
                {
                    RegistrySettings.key = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Services\\miisautosync\\Parameters", true);
                }

                return RegistrySettings.key;
            }
        }

        public static bool ExecutionEngineEnabled
        {
            get
            {
                int? value = RegistrySettings.BaseKey.GetValue("ExecutionEngineEnabled", 0) as int?;
                return value.HasValue && value.Value != 0;
            }
            set => RegistrySettings.BaseKey.SetValue("ExecutionEngineEnabled", value ? 1 : 0);
        }

        public static string LogPath
        {
            get
            {
                string logPath = RegistrySettings.BaseKey.GetValue("LogPath") as string;

                if (logPath != null)
                {
                    return Path.Combine(logPath, "autosync.log");
                }

                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                string dirName = Path.GetDirectoryName(path);

                return Path.Combine(dirName ?? Global.AssemblyDirectory, "Logs\\autosync.log");
            }
        }

        public static string AdminGroup
        {
            get
            {
                string value = RegistrySettings.BaseKey.GetValue("ServiceAdminGroup") as string;

                if (value != null)
                {
                    return value;
                }

                return "FimSyncAdmins";
            }
        }

        public static string ConfigurationFile
        {
            get
            {
                string path = RegistrySettings.BaseKey.GetValue("ConfigFile") as string;

                if (path != null)
                {
                    return path;
                }

                return Path.Combine(Global.AssemblyDirectory, "config.xml");
            }
        }

        public static int RetryCount
        {
            get
            {
                string s = RegistrySettings.BaseKey.GetValue("RetryCount") as string;

                int value;

                return int.TryParse(s, out value) ? value : 5;
            }
        }

        public static HashSet<string> RetryCodes
        {
            get
            {
                if (retryCodes == null)
                {
                    retryCodes = new HashSet<string>();

                    string[] values = RegistrySettings.BaseKey.GetValue("RetryCodes") as string[];

                    if (values != null && values.Length > 0)
                    {
                        foreach (string s in values)
                        {
                            retryCodes.Add(s);
                        }
                    }
                    else
                    {
                        retryCodes.Add("stopped-deadlocked");
                    }
                }

                return retryCodes;
            }
        }

        /// <summary>
        /// The amount of time to sleep after a deadlock event before retrying
        /// </summary>
        public static TimeSpan RetrySleepInterval
        {
            get
            {
                string s = RegistrySettings.BaseKey.GetValue("RetrySleepInterval") as string;

                int seconds;

                if (int.TryParse(s, out seconds))
                {
                    return new TimeSpan(0, 0, seconds >= 1 ? seconds : 1);
                }
                else
                {
                    return new TimeSpan(0, 0, 5);
                }
            }
        }

        public static TimeSpan ExecutionStaggerInterval
        {
            get
            {
                string s = RegistrySettings.BaseKey.GetValue("ExecutionStaggerInterval") as string;

                int seconds;

                if (int.TryParse(s, out seconds))
                {
                    return new TimeSpan(0, 0, seconds >= 1 ? seconds : 1);
                }
                else
                {
                    return new TimeSpan(0, 0, 2);
                }
            }
        }

        /// <summary>
        /// The amount of time after the run profile completes before analysis of the run profile results starts
        /// </summary>
        public static TimeSpan PostRunInterval
        {
            get
            {
                string s = RegistrySettings.BaseKey.GetValue("PostRunInterval") as string;

                int seconds;

                if (int.TryParse(s, out seconds))
                {
                    return new TimeSpan(0, 0, seconds >= 1 ? seconds : 1);
                }
                else
                {
                    return new TimeSpan(0, 0, 2);
                }
            }
        }

        public static TimeSpan UnmanagedChangesCheckInterval
        {
            get
            {
                string s = RegistrySettings.BaseKey.GetValue("UnmanagedChangesCheckInterval") as string;

                int seconds;

                if (int.TryParse(s, out seconds))
                {
                    return new TimeSpan(0, 0, seconds > 0 ? seconds : 3600);
                }
                else
                {
                    return new TimeSpan(0, 60, 0);
                }
            }
        }

        public static TimeSpan RunHistoryTimerInterval
        {
            get
            {
                string s = RegistrySettings.BaseKey.GetValue("RunHistoryTimerInterval") as string;

                int seconds;

                if (int.TryParse(s, out seconds))
                {
                    if (seconds > 0)
                    {
                        return new TimeSpan(0, 0, seconds);
                    }
                    else
                    {
                        return TimeSpan.FromHours(8);
                    }
                }
                else
                {
                    return TimeSpan.FromHours(8);
                }
            }
        }

        public static string GetSettingsString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"{nameof(RegistrySettings.LogPath)}: {RegistrySettings.LogPath}");
            builder.AppendLine($"{nameof(RegistrySettings.UnmanagedChangesCheckInterval)}: {RegistrySettings.UnmanagedChangesCheckInterval}");
            builder.AppendLine($"{nameof(RegistrySettings.ExecutionStaggerInterval)}: {RegistrySettings.ExecutionStaggerInterval}");
            builder.AppendLine($"{nameof(RegistrySettings.PostRunInterval)}: {RegistrySettings.PostRunInterval}");
            builder.AppendLine($"{nameof(RegistrySettings.RetryCount)}: {RegistrySettings.RetryCount}");
            builder.AppendLine($"{nameof(RegistrySettings.RetrySleepInterval)}: {RegistrySettings.RetrySleepInterval}");
            builder.AppendLine($"{nameof(RegistrySettings.RetryCodes)}: {string.Join(",", RegistrySettings.RetryCodes)}");

            return builder.ToString();
        }
    }
}

