using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UIAssistant
{
    public class HelpWindow : SettingsWindow
    {
        #region Variables
        readonly GUIContent WebsiteContent = new("Visit Website", WebsiteLink);
        const string WebsiteLink = "https://sites.google.com/view/uiassistant/";
        readonly GUIContent YouTubeContent = new("Browse YouTube", YouTubeLink);
        const string YouTubeLink = "https://www.youtube.com/@UIAssistant";
        readonly GUIContent DiscordContent = new("Join Discord", DiscordLink);
        const string DiscordLink = "https://discord.gg/veSW5a4r";
        readonly GUIContent AssetStoreContent = new("Rate Package", AssetStoreLink);
        const string AssetStoreLink = "https://u3d.as/3dXj";

        GUIStyle TextStyle;
        #endregion

        #region Function
        [MenuItem("Tools/UI Assistant/Help", priority = -9999)]
        public static void OpenWindow()
        {
            GetWindow<HelpWindow>("UI Assistant Help");
        }

        override protected void OnGUI()
        {
            base.OnGUI();

            TextStyle = new()
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
            };

            EditorGUILayout.Space();
            GUILayout.Label("Thank you for using the UI Assistant! Here are some resources to help you get the most out of the tool.", TextStyle);
            EditorGUILayout.Space();

            BeginScrollArea();

            EditorGUILayout.Space();
            URLButton("Access detailed documentation and guides on the website.", WebsiteContent, WebsiteLink);
            Space();
            URLButton("Watch step-by-step video tutorials on YouTube.", YouTubeContent, YouTubeLink);
            Space();
            URLButton("Join us on Discord to ask questions, share feedback, and connect with other users.", DiscordContent, DiscordLink);
            Space();
            URLButton("More feedback ensures higher asset quality. If this package is useful to you, please leave a rating on the Unity Asset Store!", AssetStoreContent, AssetStoreLink);
            EditorGUILayout.Space();

            EndScrollArea();
        }
        void Space()
        {
            GUILayout.Space(20);
        }
        void URLButton(string text, GUIContent content, string url)
        {
            GUILayout.Label(text, BoxedLabelStyle);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(content, GUILayout.Width(ContentLibrary.AddButtonWidth), GUILayout.Height(ContentLibrary.AddButtonHeight)))
                Application.OpenURL(url);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        #endregion
    }
}