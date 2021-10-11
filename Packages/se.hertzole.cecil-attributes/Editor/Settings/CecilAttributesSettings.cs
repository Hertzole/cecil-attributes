using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Hertzole.CecilAttributes.Editor
{
    [Serializable]
    public class CecilAttributesSettings : ScriptableObject
    {
        private const string DIRECTORY = "ProjectSettings/Packages/se.hertzole.cecilattributes";
        private const string PATH = DIRECTORY + "/CecilAttributesSettings.asset";

        [SerializeField]
        private bool includeResetStaticInBuild = true;
        [SerializeField]
        private RuntimeInitializeLoadType resetStaticMode = RuntimeInitializeLoadType.SubsystemRegistration;
        [SerializeField]
        private bool includeLogsInBuild = true;
        [SerializeField]
        private string methodLogFormat = "%class% %method% (%parameters%)";
        [SerializeField]
        private string parametersSeparator = ", ";
        [SerializeField]
        private string propertyGetLogFormat = "%property% Get %value%";
        [SerializeField]
        private string propertySetLogFormat = "%property% Set (Old: %old_value%, New: %new_value%)";
        [SerializeField]
        private bool includeTimedInBuild = false;
        [SerializeField]
        private string timedMethodFormat = "%class% %method% took %milliseconds% milliseconds (%ticks% ticks)";
        [SerializeField]
        private string timedPropertyGetFormat = "%class% %property% Get took %milliseconds% milliseconds (%ticks% ticks)";
        [SerializeField]
        private string timedPropertySetFormat = "%class% %property% Set took %milliseconds% milliseconds (%ticks% ticks)";
        [SerializeField]
        private string markInProfilerFormat = "%class% :: %method%";

        public bool IncludeResetStaticInBuild
        {
            get { return includeResetStaticInBuild; }
            set { includeResetStaticInBuild = value; Save(); }
        }

        public RuntimeInitializeLoadType ResetStaticMode
        {
            get { return resetStaticMode; }
            set { resetStaticMode = value; Save(); }
        }

        public bool IncludeLogsInBuild
        {
            get { return includeLogsInBuild; }
            set { includeLogsInBuild = value; Save(); }
        }

        public string MethodLogFormat
        {
            get { return methodLogFormat; }
            set { methodLogFormat = value; Save(); }
        }

        public string ParametersSeparator
        {
            get { return parametersSeparator; }
            set { parametersSeparator = value; Save(); }
        }

        public string PropertyGetLogFormat
        {
            get { return propertyGetLogFormat; }
            set { propertyGetLogFormat = value; Save(); }
        }

        public string PropertySetLogFormat
        {
            get { return propertySetLogFormat; }
            set { propertySetLogFormat = value; Save(); }
        }

        public bool IncludeTimedInBuild
        {
            get { return includeTimedInBuild; }
            set { includeTimedInBuild = value; Save(); }
        }

        public string TimedMethodFormat
        {
            get { return timedMethodFormat; }
            set { timedMethodFormat = value; Save(); }
        }

        public string TimedPropertyGetFormat
        {
            get { return timedPropertyGetFormat; }
            set { timedPropertyGetFormat = value; Save(); }
        }

        public string TimedPropertySetFormat
        {
            get { return timedPropertySetFormat; }
            set { timedPropertySetFormat = value; Save(); }
        }

        public string MarkInProfilerFormat
        {
            get { return markInProfilerFormat; }
            set { markInProfilerFormat = value; Save(); }
        }

        private static CecilAttributesSettings instance;
        public static CecilAttributesSettings Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                instance = GetOrCreate();
                return instance;
            }
        }

        private void OnEnable()
        {
            SaveData(new SettingData(this));
        }

        public static void Save()
        {
            SaveInstance(Instance);
        }
        
        public static SettingData GetData()
        {
            if (instance == null)
            {
                return LoadData();
            }
            else
            {
                return new SettingData(instance);
            }
        }

        private static CecilAttributesSettings GetOrCreate()
        {
            CecilAttributesSettings settings;

            // Backwards compatibility.
            string oldPath = "ProjectSettings/CecilAttributesSettings.asset";

            if (File.Exists(oldPath))
            {
                settings = LoadSettings(oldPath);
                RemoveFile(oldPath);

                if (settings != null)
                {
                    SaveData(new SettingData(settings));
                    return settings;
                }
            }

            if (!File.Exists(PATH))
            {
                settings = CreateNewSettings();
            }
            else
            {
                settings = LoadSettings();

                if (settings == null)
                {
                    RemoveFile(PATH);
                    settings = CreateNewSettings();
                }
            }

            settings.hideFlags = HideFlags.HideAndDontSave;
            SaveData(new SettingData(settings));

            return settings;
        }

        private static CecilAttributesSettings CreateNewSettings()
        {
            CecilAttributesSettings settings = CreateInstance<CecilAttributesSettings>();
            SaveInstance(settings);
            return settings;
        }

        private static CecilAttributesSettings LoadSettings(string path = "")
        {
            CecilAttributesSettings settings;

            try
            {
                settings = (CecilAttributesSettings)InternalEditorUtility.LoadSerializedFileAndForget(string.IsNullOrEmpty(path) ? PATH : path)[0];
            }
            catch (Exception)
            {
                Debug.Log("Couldn't load cecil attribute settings. Settings will be reset.");
                settings = null;
            }

            return settings;
        }

        private static void RemoveFile(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }

            FileAttributes attributes = File.GetAttributes(path);
            if (attributes.HasFlag(FileAttributes.ReadOnly))
            {
                File.SetAttributes(path, attributes & ~FileAttributes.ReadOnly);
            }

            File.Delete(path);
        }

        private static void SaveInstance(CecilAttributesSettings settings)
        {
            if (!Directory.Exists(DIRECTORY))
            {
                Directory.CreateDirectory(DIRECTORY);
            }

            try
            {
                InternalEditorUtility.SaveToSerializedFileAndForget(new Object[] { settings }, PATH, true);
                SaveData(new SettingData(settings));
            }
            catch (Exception ex)
            {
                Debug.LogError("Can't save cecil attribute settings!\n" + ex);
            }
        }

        private static void SaveData(SettingData data)
        {
            string libraryPath = Path.GetFullPath($"{Assembly.GetExecutingAssembly().Location}/../../se.hertzole.CecilAttributes.settings.bin");
            using (FileStream stream = File.OpenWrite(libraryPath))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(data.includeResetStaticInBuild);
                    writer.Write((int) data.resetStaticMode);
                    writer.Write(data.includeLogsInBuild);
                    writer.Write(data.methodLogFormat);
                    writer.Write(data.parametersSeparator);
                    writer.Write(data.propertyGetLogFormat);
                    writer.Write(data.propertySetLogFormat);
                    writer.Write(data.includeTimedInBuild);
                    writer.Write(data.timedMethodFormat);
                    writer.Write(data.timedPropertyGetFormat);
                    writer.Write(data.timedPropertySetFormat);
                    writer.Write(data.markInProfilerFormat);
                }
            }
        }

        private static SettingData LoadData()
        {
            string libraryPath = Path.GetFullPath($"{Assembly.GetExecutingAssembly().Location}/../../se.hertzole.CecilAttributes.settings.bin");
            if (File.Exists(libraryPath))
            {
                SettingData data = new SettingData();
                using (FileStream stream = File.OpenRead(libraryPath))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        data.includeResetStaticInBuild = reader.ReadBoolean();
                        data.resetStaticMode = (RuntimeInitializeLoadType) reader.ReadInt32();
                        data.includeLogsInBuild = reader.ReadBoolean();
                        data.methodLogFormat = reader.ReadString();
                        data.parametersSeparator = reader.ReadString();
                        data.propertyGetLogFormat = reader.ReadString();
                        data.propertySetLogFormat = reader.ReadString();
                        data.includeTimedInBuild = reader.ReadBoolean();
                        data.timedMethodFormat = reader.ReadString();
                        data.timedPropertyGetFormat = reader.ReadString();
                        data.timedPropertySetFormat = reader.ReadString();
                        data.markInProfilerFormat = reader.ReadString();
                    }
                }

                return data;
            }

            return SettingData.Default;
        }

        [Serializable]
        public struct SettingData
        {
            public bool includeResetStaticInBuild;
            public RuntimeInitializeLoadType resetStaticMode;
            public bool includeLogsInBuild;
            public string methodLogFormat;
            public string parametersSeparator;
            public string propertyGetLogFormat;
            public string propertySetLogFormat;
            public bool includeTimedInBuild;
            public string timedMethodFormat;
            public string timedPropertyGetFormat;
            public string timedPropertySetFormat;
            public string markInProfilerFormat;

            public SettingData(CecilAttributesSettings settings)
            {
                Console.WriteLine("CREATE NEW DATA");
                
                includeResetStaticInBuild = settings.includeResetStaticInBuild;
                resetStaticMode = settings.resetStaticMode;
                includeLogsInBuild = settings.includeLogsInBuild;
                methodLogFormat = settings.methodLogFormat;
                parametersSeparator = settings.parametersSeparator;
                propertyGetLogFormat = settings.propertyGetLogFormat;
                propertySetLogFormat = settings.propertySetLogFormat;
                includeTimedInBuild = settings.includeTimedInBuild;
                timedMethodFormat = settings.timedMethodFormat;
                timedPropertyGetFormat = settings.timedPropertyGetFormat;
                timedPropertySetFormat = settings.timedPropertySetFormat;
                markInProfilerFormat = settings.markInProfilerFormat;
            }
            
            public static SettingData Default
            { 
                get
                {
                    return new SettingData()
                    {
                        methodLogFormat = "%class% %method% (%parameters%)"
                    };
                }
                
            }
        }
    }
}
