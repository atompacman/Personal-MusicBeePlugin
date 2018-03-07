// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// MIT License
// Copyright (c) 2018 Stained Glass Guild
// See file "LICENSE.txt" at project root for complete license
// ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~
// Project: RYMExporter
// File: RYMExporter.cs
// Creation: 2018-02
// Author: Jérémie Coulombe
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

using System;
using System.IO;

using MusicBeePlugin.RYMExporter.Source;

namespace MusicBeePlugin
{
   public partial class Plugin
   {
      #region Compile-time constants

      private const string PLUGIN_NAME = "RYMExporter";
      private const int CONFIG_PANEL_HEIGHT = 36;

      #endregion

      #region Static fields

      public static MusicBeeApiInterface MusicBeeApi;

      #endregion

      #region Properties

      private static string ConfigFilePath
      {
         get
         {
            string settingsPath = MusicBeeApi.Setting_GetPersistentStoragePath();
            return settingsPath + "\\" + PLUGIN_NAME + "_config.dat";
         }
      }

      #endregion

      #region Methods

      public PluginInfo Initialise(IntPtr a_ApiInterfacePtr)
      {
         MusicBeeApi.Initialise(a_ApiInterfacePtr);

         MusicBeeApi.MB_AddMenuItem(
            "mnuTools/AMBPlugins_" + PLUGIN_NAME,
            "Run " + PLUGIN_NAME + " plugin",
            (a_Sender, a_Args) =>
            {
               MusicBeeApi.MB_AddPanel(new Interface(), PluginPanelDock.MainPanel);
            });

         return new PluginInfo
         {
            Author = "Atompacman",
            ConfigurationPanelHeight = CONFIG_PANEL_HEIGHT,
            Description = "No description",
            MinApiRevision = MinApiRevision,
            MinInterfaceVersion = MinInterfaceVersion,
            Name = PLUGIN_NAME,
            PluginInfoVersion = PluginInfoVersion,
            ReceiveNotifications = ReceiveNotificationFlags.StartupOnly,
            Revision = 1,
            TargetApplication = string.Empty,
            Type = PluginType.General,
            VersionMajor = 1,
            VersionMinor = 0
         };
      }

      public bool Configure(IntPtr a_PanelHandle)
      {
         // Configure((Panel) Control.FromHandle(a_PanelHandle));
         return false;
      }

      public void SaveSettings()
      {
         // var stream = File.CreateText(ConfigFilePath);
         // stream.Flush();
         // stream.Close();
      }

      public void Close(PluginCloseReason a_Reason)
      {}

      public void Uninstall()
      {
         File.Delete(ConfigFilePath);
      }

      public void ReceiveNotification(string a_SourceFileUrl, NotificationType a_Type)
      {}

      #endregion
   }
}
