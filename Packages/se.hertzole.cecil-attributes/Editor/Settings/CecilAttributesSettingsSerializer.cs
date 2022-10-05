using UnityEngine;

namespace Hertzole.CecilAttributes.Editor
{
	public static class CecilAttributesSettingsSerializer
	{
		public static void Serialize(CecilAttributesSettings.SettingData settings, string path)
		{
			using (SimpleTextWriter writer = new SimpleTextWriter(path))
			{
				writer.WriteBoolean(nameof(CecilAttributesSettings.IncludeResetStaticInBuild), settings.includeResetStaticInBuild);
				writer.WriteInt(nameof(CecilAttributesSettings.ResetStaticMode), (int) settings.resetStaticMode);

				writer.WriteBoolean(nameof(CecilAttributesSettings.IncludeLogsInBuild), settings.includeLogsInBuild);
				writer.WriteString(nameof(CecilAttributesSettings.ParametersSeparator), settings.parametersSeparator);
				writer.WriteString(nameof(CecilAttributesSettings.MethodLogFormat), settings.methodLogFormat);
				writer.WriteString(nameof(CecilAttributesSettings.PropertyGetLogFormat), settings.propertyGetLogFormat);
				writer.WriteString(nameof(CecilAttributesSettings.PropertySetLogFormat), settings.propertySetLogFormat);

				writer.WriteBoolean(nameof(CecilAttributesSettings.IncludeTimedInBuild), settings.includeTimedInBuild);
				writer.WriteString(nameof(CecilAttributesSettings.TimedMethodFormat), settings.timedMethodFormat);
				writer.WriteString(nameof(CecilAttributesSettings.TimedPropertyGetFormat), settings.timedPropertyGetFormat);
				writer.WriteString(nameof(CecilAttributesSettings.TimedPropertySetFormat), settings.timedPropertySetFormat);

				writer.WriteString(nameof(CecilAttributesSettings.MarkInProfilerFormat), settings.markInProfilerFormat);
				
				writer.WriteBoolean(nameof(CecilAttributesSettings.RunPrefabProcessorOnReload), settings.runPrefabProcessor);
				writer.WriteBoolean(nameof(CecilAttributesSettings.RunSceneProcessorOnReload), settings.runSceneProcessor);
				
				writer.WriteBoolean(nameof(CecilAttributesSettings.IncludeRequiredInBuild), settings.includeRequiredInBuild);
			}
		}

		public static CecilAttributesSettings.SettingData Deserialize(string path)
		{
			CecilAttributesSettings.SettingData data = new CecilAttributesSettings.SettingData();
			
			using (SimpleTextReader reader = new SimpleTextReader(path))
			{
				data.includeResetStaticInBuild = reader.ReadBoolean(nameof(CecilAttributesSettings.IncludeResetStaticInBuild), CecilAttributesSettings.INCLUDE_RESET_STATIC_IN_BUILD);
				data.resetStaticMode = (RuntimeInitializeLoadType) reader.ReadInt(nameof(CecilAttributesSettings.ResetStaticMode), (int) CecilAttributesSettings.RESET_STATIC_MODE);

				data.includeLogsInBuild = reader.ReadBoolean(nameof(CecilAttributesSettings.IncludeLogsInBuild), CecilAttributesSettings.INCLUDE_LOGS_IN_BUILD);
				data.parametersSeparator = reader.ReadString(nameof(CecilAttributesSettings.ParametersSeparator), CecilAttributesSettings.PARAMETERS_SEPARATOR);
				data.methodLogFormat = reader.ReadString(nameof(CecilAttributesSettings.MethodLogFormat), CecilAttributesSettings.METHOD_LOG_FORMAT);
				data.propertyGetLogFormat = reader.ReadString(nameof(CecilAttributesSettings.PropertyGetLogFormat), CecilAttributesSettings.PROPERTY_GET_LOG_FORMAT);
				data.propertySetLogFormat = reader.ReadString(nameof(CecilAttributesSettings.PropertySetLogFormat), CecilAttributesSettings.PROPERTY_SET_LOG_FORMAT);

				data.includeTimedInBuild = reader.ReadBoolean(nameof(CecilAttributesSettings.IncludeTimedInBuild), CecilAttributesSettings.INCLUDE_TIMED_IN_BUILD);
				data.timedMethodFormat = reader.ReadString(nameof(CecilAttributesSettings.TimedMethodFormat), CecilAttributesSettings.TIMED_METHOD_FORMAT);
				data.timedPropertyGetFormat = reader.ReadString(nameof(CecilAttributesSettings.TimedPropertyGetFormat), CecilAttributesSettings.TIMED_PROPERTY_GET_FORMAT);
				data.timedPropertySetFormat = reader.ReadString(nameof(CecilAttributesSettings.TimedPropertySetFormat), CecilAttributesSettings.TIMED_PROPERTY_SET_FORMAT);

				data.markInProfilerFormat = reader.ReadString(nameof(CecilAttributesSettings.MarkInProfilerFormat), CecilAttributesSettings.MARK_IN_PROFILER_FORMAT);
				
				data.runPrefabProcessor = reader.ReadBoolean(nameof(CecilAttributesSettings.RunPrefabProcessorOnReload), CecilAttributesSettings.RUN_PREFAB_PROCESSOR);
				data.runSceneProcessor = reader.ReadBoolean(nameof(CecilAttributesSettings.RunSceneProcessorOnReload), CecilAttributesSettings.RUN_SCENE_PROCESSOR);
				
				data.includeRequiredInBuild = reader.ReadBoolean(nameof(CecilAttributesSettings.IncludeRequiredInBuild), CecilAttributesSettings.INCLUDE_REQUIRED_IN_BUILD);
			}

			return data;
		}
	}
}