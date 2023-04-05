using System;
using System.IO;
using UnityEditorInternal;
using UnityEngine;

namespace Hertzole.CecilAttributes.Editor
{
	[Serializable]
	public class CecilAttributesSettings : ScriptableObject
	{
		[SerializeField]
		private bool includeResetStaticInBuild = INCLUDE_RESET_STATIC_IN_BUILD;
		[SerializeField]
		private RuntimeInitializeLoadType resetStaticMode = RESET_STATIC_MODE;
		[SerializeField]
		private bool includeLogsInBuild = INCLUDE_LOGS_IN_BUILD;
		[SerializeField]
		private string methodLogFormat = METHOD_LOG_FORMAT;
		[SerializeField]
		private string parametersSeparator = PARAMETERS_SEPARATOR;
		[SerializeField]
		private string propertyGetLogFormat = PROPERTY_GET_LOG_FORMAT;
		[SerializeField]
		private string propertySetLogFormat = PROPERTY_SET_LOG_FORMAT;
		[SerializeField]
		private bool includeTimedInBuild = INCLUDE_TIMED_IN_BUILD;
		[SerializeField]
		private string timedMethodFormat = TIMED_METHOD_FORMAT;
		[SerializeField]
		private string timedPropertyGetFormat = TIMED_PROPERTY_GET_FORMAT;
		[SerializeField]
		private string timedPropertySetFormat = TIMED_PROPERTY_SET_FORMAT;
		[SerializeField]
		private string markInProfilerFormat = MARK_IN_PROFILER_FORMAT;
		[SerializeField] 
		private bool runPrefabProcessorOnReload = RUN_PREFAB_PROCESSOR;
		[SerializeField] 
		private bool runSceneProcessorOnReload = RUN_SCENE_PROCESSOR;
		[SerializeField] 
		private bool includeRequiredInBuild = INCLUDE_REQUIRED_IN_BUILD;

		private static CecilAttributesSettings instance;

		public bool IncludeResetStaticInBuild
		{
			get { return includeResetStaticInBuild; }
			set
			{
				includeResetStaticInBuild = value;
				Save();
			}
		}

		public RuntimeInitializeLoadType ResetStaticMode
		{
			get { return resetStaticMode; }
			set
			{
				resetStaticMode = value;
				Save();
			}
		}

		public bool IncludeLogsInBuild
		{
			get { return includeLogsInBuild; }
			set
			{
				includeLogsInBuild = value;
				Save();
			}
		}

		public string MethodLogFormat
		{
			get { return methodLogFormat; }
			set
			{
				methodLogFormat = value;
				Save();
			}
		}

		public string ParametersSeparator
		{
			get { return parametersSeparator; }
			set
			{
				parametersSeparator = value;
				Save();
			}
		}

		public string PropertyGetLogFormat
		{
			get { return propertyGetLogFormat; }
			set
			{
				propertyGetLogFormat = value;
				Save();
			}
		}

		public string PropertySetLogFormat
		{
			get { return propertySetLogFormat; }
			set
			{
				propertySetLogFormat = value;
				Save();
			}
		}

		public bool IncludeTimedInBuild
		{
			get { return includeTimedInBuild; }
			set
			{
				includeTimedInBuild = value;
				Save();
			}
		}

		public string TimedMethodFormat
		{
			get { return timedMethodFormat; }
			set
			{
				timedMethodFormat = value;
				Save();
			}
		}

		public string TimedPropertyGetFormat
		{
			get { return timedPropertyGetFormat; }
			set
			{
				timedPropertyGetFormat = value;
				Save();
			}
		}

		public string TimedPropertySetFormat
		{
			get { return timedPropertySetFormat; }
			set
			{
				timedPropertySetFormat = value;
				Save();
			}
		}

		public string MarkInProfilerFormat
		{
			get { return markInProfilerFormat; }
			set
			{
				markInProfilerFormat = value;
				Save();
			}
		}
		
		public bool RunPrefabProcessorOnReload
		{
			get { return runPrefabProcessorOnReload; }
			set
			{
				runPrefabProcessorOnReload = value;
				Save();
			}
		}
		
		public bool RunSceneProcessorOnReload
		{
			get { return runSceneProcessorOnReload; }
			set
			{
				runSceneProcessorOnReload = value;
				Save();
			}
		}
		
		public bool IncludeRequiredInBuild
		{
			get { return includeRequiredInBuild; }
			set
			{
				includeRequiredInBuild = value;
				Save();
			}
		}
		
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
		
		private const string DIRECTORY = "ProjectSettings/Packages/se.hertzole.cecilattributes";
		private const string PATH = DIRECTORY + "/CecilAttributesSettings.txt";

		public const bool INCLUDE_RESET_STATIC_IN_BUILD = false;
		public const RuntimeInitializeLoadType RESET_STATIC_MODE = RuntimeInitializeLoadType.SubsystemRegistration;
		
		public const bool INCLUDE_LOGS_IN_BUILD = true;
		public const string METHOD_LOG_FORMAT = "%class% %method% (%parameters%)";
		public const string PARAMETERS_SEPARATOR = ", ";
		public const string PROPERTY_GET_LOG_FORMAT = "%property% Get %value%";
		public const string PROPERTY_SET_LOG_FORMAT = "%property% Set (Old: %old_value%, New: %new_value%)";
		
		public const bool INCLUDE_TIMED_IN_BUILD = false;
		public const string TIMED_METHOD_FORMAT = "%class% %method% took %milliseconds% milliseconds (%ticks% ticks)";
		public const string TIMED_PROPERTY_SET_FORMAT = "%class% %property% Set took %milliseconds% milliseconds (%ticks% ticks)";
		public const string TIMED_PROPERTY_GET_FORMAT = "%class% %property% Get took %milliseconds% milliseconds (%ticks% ticks)";
		
		public const string MARK_IN_PROFILER_FORMAT = "%class% :: %method%";

		public const bool RUN_PREFAB_PROCESSOR = true;
		public const bool RUN_SCENE_PROCESSOR = true;
		
		public const bool INCLUDE_REQUIRED_IN_BUILD = false;

		private void ApplySettingData(SettingData data)
		{
			includeResetStaticInBuild = data.includeResetStaticInBuild;
			resetStaticMode = data.resetStaticMode;
			
			includeLogsInBuild = data.includeLogsInBuild;
			methodLogFormat = data.methodLogFormat;
			parametersSeparator = data.parametersSeparator;
			propertyGetLogFormat = data.propertyGetLogFormat;
			propertySetLogFormat = data.propertySetLogFormat;

			includeTimedInBuild = data.includeTimedInBuild;
			timedMethodFormat = data.timedMethodFormat;
			timedPropertyGetFormat = data.timedPropertyGetFormat;
			timedPropertySetFormat = data.timedPropertySetFormat;

			markInProfilerFormat = data.markInProfilerFormat;
			
			runPrefabProcessorOnReload = data.runPrefabProcessor;
			runSceneProcessorOnReload = data.runSceneProcessor;
		}
		
		public static void Save()
		{
			SaveInstance(Instance);
		}

		private static CecilAttributesSettings GetOrCreate()
		{
			CecilAttributesSettings settings;

			// Backwards compatibility.
			const string old_path = "ProjectSettings/CecilAttributesSettings.asset";

			if (File.Exists(old_path))
			{
				settings = LoadSettings(old_path);
				RemoveFile(old_path);

				if (settings != null)
				{
					return settings;
				}
			}

			// Need to check for old file for backwards compatibility support.
			if (!File.Exists(PATH) && !File.Exists(DIRECTORY + "/CecilAttributesSettings.asset"))
			{
				settings = CreateNewSettings();
			}
			else if (!File.Exists(Path.GetFullPath($"{Directory.GetCurrentDirectory()}/{PATH}")))
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

			return settings;
		}

		private static CecilAttributesSettings CreateNewSettings()
		{
			CecilAttributesSettings settings = CreateInstance<CecilAttributesSettings>();
			SaveInstance(settings);
			return settings;
		}

		public static SettingData LoadSettingData()
		{
			if (!File.Exists(Path.GetFullPath($"{Directory.GetCurrentDirectory()}/{PATH}")))
			{
				return SettingData.Default;
			}
			
			SettingData data;
			try
			{
				data = CecilAttributesSettingsSerializer.Deserialize(PATH);
			}
			catch (ArgumentException e)
			{
				Console.WriteLine(e);
				data = SettingData.Default;
			}

			return data;
		}

		private static CecilAttributesSettings LoadSettings(string path = "")
		{
			CecilAttributesSettings settings;

			try
			{
				// Backwards compatibility.
				if (File.Exists(DIRECTORY + "/CecilAttributesSettings.asset"))
				{
					settings = (CecilAttributesSettings)InternalEditorUtility.LoadSerializedFileAndForget(DIRECTORY + "/CecilAttributesSettings.asset")[0];
					SaveInstance(settings);
					RemoveFile(DIRECTORY + "/CecilAttributesSettings.asset");
					return settings;
				}
				
				SettingData data = CecilAttributesSettingsSerializer.Deserialize(string.IsNullOrEmpty(path) ? PATH : path);
				settings = CreateInstance<CecilAttributesSettings>();
				settings.ApplySettingData(data);
			}
			catch (Exception ex)
			{
				Debug.LogError($"Couldn't load cecil attribute settings. Settings will be reset.\n{ex}");
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
				CecilAttributesSettingsSerializer.Serialize(new SettingData(settings), PATH);
			}
			catch (Exception ex)
			{
				Debug.LogError($"Can't save cecil attribute settings!\n{ex}");
			}
		}

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
			public bool runPrefabProcessor;
			public bool runSceneProcessor;
			public bool includeRequiredInBuild;

			public static SettingData Default
			{
				get
				{
					return new SettingData
					{
						includeResetStaticInBuild = INCLUDE_RESET_STATIC_IN_BUILD,
						resetStaticMode = RESET_STATIC_MODE,
						
						includeLogsInBuild = INCLUDE_LOGS_IN_BUILD,
						methodLogFormat = METHOD_LOG_FORMAT,
						parametersSeparator = PARAMETERS_SEPARATOR,
						propertyGetLogFormat = PROPERTY_GET_LOG_FORMAT,
						propertySetLogFormat = PROPERTY_SET_LOG_FORMAT,
						includeTimedInBuild = INCLUDE_TIMED_IN_BUILD,
						timedMethodFormat = TIMED_METHOD_FORMAT,
						timedPropertyGetFormat = TIMED_PROPERTY_GET_FORMAT,
						timedPropertySetFormat = TIMED_PROPERTY_SET_FORMAT,
						markInProfilerFormat = MARK_IN_PROFILER_FORMAT,
						runPrefabProcessor = RUN_PREFAB_PROCESSOR,
						runSceneProcessor = RUN_SCENE_PROCESSOR,
						includeRequiredInBuild = INCLUDE_REQUIRED_IN_BUILD
					};
				}
			}

			public SettingData(CecilAttributesSettings settings)
			{
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
				runPrefabProcessor = settings.runPrefabProcessorOnReload;
				runSceneProcessor = settings.runSceneProcessorOnReload;
				includeRequiredInBuild = settings.includeRequiredInBuild;
			}
		}
	}
}