﻿using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Hertzole.CecilAttributes.Editor
{
    internal static class CecilAttributesSettingsGUI
    {
        private static readonly GUIContent includeInBuildLabel = new GUIContent("Include In Build", "If true, the reset static method will be included in the built version.");
        private static readonly GUIContent resetStaticLabel = new GUIContent("Reset Static Mode", "The type of initialization for resting statics.");
        private static readonly GUIContent includeLogsInBuildLabel = new GUIContent("Include In Build", "If true, logs from LogCalled will be included in build.");
        private static readonly GUIContent logParameterSeparatorLabel = new GUIContent("Parameters Separator", "The separator between each parameter in a method log.");
        private static readonly GUIContent logMethodFormatLabel = new GUIContent("Method Format", "The format of methods when logged.");
        private static readonly GUIContent logPropertyGetFormatLabel = new GUIContent("Property Get Format", "The format of property get methods.");
        private static readonly GUIContent logPropertySetFormatLabel = new GUIContent("Property Set Format", "The format of property set methods.");
        private static readonly GUIContent includeTimedInBuildLabel = new GUIContent("Include In Build", "If true, timed messages will be included in the build.");
        private static readonly GUIContent timedMethodFormatLabel = new GUIContent("Method Format", "The message that will be printed to the console from methods.");
        private static readonly GUIContent timedPropertyGetFormatLabel = new GUIContent("Property Get Format", "The message that will be printed to the console when getting a property.");
        private static readonly GUIContent timedPropertySetFormatLabel = new GUIContent("Property Set Format", "The message that will be printed to the console when setting a property.");
        private static readonly GUIContent markInProfilerFormatLabel = new GUIContent("Mark In Profiler Format", "The format of methods that appear in the profiler.");
        private static readonly GUIContent runPrefabProcessorLabel = new GUIContent("Run Prefab Processor On Reload", "If true, all prefabs in your project will be checked for GetComponent attributes on recompile.");
        private static readonly GUIContent runSceneProcessorLabel = new GUIContent("Run Scene Processor On Reload", "If true, your current scene will be checked for GetComponent attributes on recompile and scene save.");

        public static void OnGUI()
        {
            using (new SettingsGUIScope())
            {
                OnResetStaticSection();

                GUILayout.Space(16f);

                OnLogCalledSection();

                GUILayout.Space(16f);

                TimedSection();

                GUILayout.Space(16f);

                MarkInProfilerSection();

                GUILayout.Space(16f);

                EditorGUILayout.HelpBox("You need to recompile your scripts for these changes to take effect. If you don't do it now, you may see some undesired behavior. " +
                                        "Otherwise it will update at some point when you update your scripts manually or when building.", MessageType.Warning);

                if (GUILayout.Button("Apply changes and recompile"))
                {
                    CompilationPipeline.RequestScriptCompilation();
                }

                GUILayout.Space(16f);
                
                GetComponentSection();
            }
        }

        private static void OnResetStaticSection()
        {
            DrawHeaderLabel("Reset Static");

            bool includeInBuild = CecilAttributesSettings.Instance.IncludeResetStaticInBuild;
            RuntimeInitializeLoadType resetStaticMode = CecilAttributesSettings.Instance.ResetStaticMode;

            EditorGUI.BeginChangeCheck();
            includeInBuild = EditorGUILayout.Toggle(includeInBuildLabel, includeInBuild);
            if (EditorGUI.EndChangeCheck())
            {
                CecilAttributesSettings.Instance.IncludeResetStaticInBuild = includeInBuild;
            }

            EditorGUI.BeginChangeCheck();
            resetStaticMode = (RuntimeInitializeLoadType)EditorGUILayout.EnumPopup(resetStaticLabel, resetStaticMode);

            if (EditorGUI.EndChangeCheck())
            {
                CecilAttributesSettings.Instance.ResetStaticMode = resetStaticMode;
            }
        }

        private static void OnLogCalledSection()
        {
            DrawHeaderLabel("Log Called");

            bool includeLogsInBuild = CecilAttributesSettings.Instance.IncludeLogsInBuild;
            string parametersSeparator = CecilAttributesSettings.Instance.ParametersSeparator;
            string methodFormat = CecilAttributesSettings.Instance.MethodLogFormat;
            string propGetFormat = CecilAttributesSettings.Instance.PropertyGetLogFormat;
            string propSetFormat = CecilAttributesSettings.Instance.PropertySetLogFormat;

            EditorGUI.BeginChangeCheck();
            includeLogsInBuild = EditorGUILayout.Toggle(includeLogsInBuildLabel, includeLogsInBuild);
            if (EditorGUI.EndChangeCheck())
            {
                CecilAttributesSettings.Instance.IncludeLogsInBuild = includeLogsInBuild;
            }

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("You can format each message to fit your needs. There are some special keywords that can be useful for you.\n" +
                "All keywords have an to upper variant. For example: %method% -> %METHOD% turns MyMethod into MYMETHOD.\n" +
                "%class% - Class name\n" +
                "%class_full% - The full class name\n" +
                "%method% - Method name\n" +
                "%method_full% - The full method name\n" +
                "%parameters% - List of parameters\n" +
                "%property% - Property name\n" +
                "%property_full% - The full property name\n" +
                "%value% - The value of a property\n" +
                "%old_value% - The old value of a property\n" +
                "%new_value% - The new value of a property", MessageType.Info);

            EditorGUI.BeginChangeCheck();
            parametersSeparator = EditorGUILayout.TextField(logParameterSeparatorLabel, parametersSeparator);
            if (EditorGUI.EndChangeCheck())
            {
                CecilAttributesSettings.Instance.ParametersSeparator = parametersSeparator;
            }

            EditorGUI.BeginChangeCheck();
            methodFormat = EditorGUILayout.TextField(logMethodFormatLabel, methodFormat);
            if (EditorGUI.EndChangeCheck())
            {
                CecilAttributesSettings.Instance.MethodLogFormat = methodFormat;
            }

            EditorGUI.BeginChangeCheck();
            propGetFormat = EditorGUILayout.TextField(logPropertyGetFormatLabel, propGetFormat);
            if (EditorGUI.EndChangeCheck())
            {
                CecilAttributesSettings.Instance.PropertyGetLogFormat = propGetFormat;
            }

            EditorGUI.BeginChangeCheck();
            propSetFormat = EditorGUILayout.TextField(logPropertySetFormatLabel, propSetFormat);
            if (EditorGUI.EndChangeCheck())
            {
                CecilAttributesSettings.Instance.PropertySetLogFormat = propSetFormat;
            }
        }

        private static void TimedSection()
        {
            DrawHeaderLabel("Timed");

            bool includeInBuild = CecilAttributesSettings.Instance.IncludeTimedInBuild;
            string methodFormat = CecilAttributesSettings.Instance.TimedMethodFormat;
            string getFormat = CecilAttributesSettings.Instance.TimedPropertyGetFormat;
            string setFormat = CecilAttributesSettings.Instance.TimedPropertySetFormat;

            EditorGUI.BeginChangeCheck();
            includeInBuild = EditorGUILayout.Toggle(includeTimedInBuildLabel, includeInBuild);
            if (EditorGUI.EndChangeCheck())
            {
                CecilAttributesSettings.Instance.IncludeTimedInBuild = includeInBuild;
            }

            EditorGUI.BeginChangeCheck();
            methodFormat = EditorGUILayout.TextField(timedMethodFormatLabel, methodFormat);
            if (EditorGUI.EndChangeCheck())
            {
                CecilAttributesSettings.Instance.TimedMethodFormat = methodFormat;
            }

            EditorGUI.BeginChangeCheck();
            getFormat = EditorGUILayout.TextField(timedPropertyGetFormatLabel, getFormat);
            if (EditorGUI.EndChangeCheck())
            {
                CecilAttributesSettings.Instance.TimedPropertyGetFormat = getFormat;
            }

            EditorGUI.BeginChangeCheck();
            setFormat = EditorGUILayout.TextField(timedPropertySetFormatLabel, setFormat);
            if (EditorGUI.EndChangeCheck())
            {
                CecilAttributesSettings.Instance.TimedPropertySetFormat = setFormat;
            }
        }

        private static void MarkInProfilerSection()
        {
            DrawHeaderLabel("Mark In Profiler");

            string format = CecilAttributesSettings.Instance.MarkInProfilerFormat;
            EditorGUI.BeginChangeCheck();
            format = EditorGUILayout.TextField(markInProfilerFormatLabel, format);
            if (EditorGUI.EndChangeCheck())
            {
                CecilAttributesSettings.Instance.MarkInProfilerFormat = format;
            }
        }

        private static void GetComponentSection()
        {
            DrawHeaderLabel("Get Component");

            bool runPrefabProcessor = CecilAttributesSettings.Instance.RunPrefabProcessorOnReload;
            EditorGUI.BeginChangeCheck();
            runPrefabProcessor = EditorGUILayout.Toggle(runPrefabProcessorLabel, runPrefabProcessor);
            if (EditorGUI.EndChangeCheck())
            {
                CecilAttributesSettings.Instance.RunPrefabProcessorOnReload = runPrefabProcessor;
            }
            
            bool runSceneProcessor = CecilAttributesSettings.Instance.RunSceneProcessorOnReload;
            EditorGUI.BeginChangeCheck();
            runSceneProcessor = EditorGUILayout.Toggle(runSceneProcessorLabel, runSceneProcessor);
            if (EditorGUI.EndChangeCheck())
            {
                CecilAttributesSettings.Instance.RunSceneProcessorOnReload = runSceneProcessor;
            }

            if (GUILayout.Button("Run Prefab Processor Manually"))
            {
                GetComponentPrefabProcessor.Process();
            }

            if (GUILayout.Button("Run Scene Processor Manually"))
            {
                GetComponentSceneProcessor.Process();
            }
        }

        private static void DrawHeaderLabel(string label)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        }
        
        private class SettingsGUIScope : GUI.Scope
        {
            private readonly float labelWidth;

            public SettingsGUIScope()
            {
                GUILayout.BeginVertical();

                labelWidth = EditorGUIUtility.labelWidth;

                if (EditorGUILayout.GetControlRect(false, 0).width > 550)
                {
                    EditorGUIUtility.labelWidth = 250;
                }
                GUILayout.BeginHorizontal();
                GUILayout.Space(7);
                GUILayout.BeginVertical();
                GUILayout.Space(4);
            }

            protected override void CloseScope()
            {
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                EditorGUIUtility.labelWidth = labelWidth;
            }
        }
    }
}
