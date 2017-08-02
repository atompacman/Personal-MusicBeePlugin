// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// MIT License
// Copyright (c) 2012 Stained Glass Guild
// See file "LICENSE.txt" at project root for complete license
// ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~
// File: Plugin.cs
// Creation: 2012-07
// Author: Jérémie Coulombe
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using MusicBeePlugin.Processor.Framework;

namespace MusicBeePlugin
{
   public partial class Plugin
   {
      #region Compile-time constants

      private const int CONFIG_WINDOW_ELEMENT_HEIGHT = 36;
      private const string CONFIG_FILE_NAME = "AMLPConfig.dat";

      #endregion

      #region Runtime constants

      private static readonly List<ILibraryProcessor> PROCESSORS;
      private static readonly SharedLibraryData SHARED_LIB_DATA;
      private static readonly List<CheckBox> ENABLED_PROCESSORS_CHECKBOXES;

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
            return settingsPath + "\\" + CONFIG_FILE_NAME;
         }
      }

      #endregion

      #region Constructors

      static Plugin()
      {
         PROCESSORS = new List<ILibraryProcessor>();

         var requiredTags = new HashSet<MetaDataType>();

         foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
         {
            if (!typeof(ILibraryProcessor).IsAssignableFrom(type) || type.IsAbstract)
            {
               continue;
            }

            var attributes = type.GetCustomAttributes(typeof(RequiredTagsAttribute), true);
            foreach (var attribute in attributes.Cast<RequiredTagsAttribute>())
            {
               foreach (var tag in attribute.Tags)
               {
                  requiredTags.Add(tag);
               }
            }

            var ctors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            if (ctors.Length != 1 || ctors[0].GetParameters().Length != 0)
            {
               throw new Exception();
            }

            PROCESSORS.Add(ctors[0].Invoke(BindingFlags.Default, null, new object[] { },
               CultureInfo.CurrentCulture) as ILibraryProcessor);
         }

         SHARED_LIB_DATA = new SharedLibraryData(requiredTags);

         ENABLED_PROCESSORS_CHECKBOXES = new List<CheckBox>(PROCESSORS.Count);
      }

      #endregion

      #region Methods

      public PluginInfo Initialise(IntPtr a_ApiInterfacePtr)
      {
         MusicBeeApi.Initialise(a_ApiInterfacePtr);

         MusicBeeApi.MB_AddMenuItem(
            "mnuTools/Atompacman's Music Library Processors",
            "Run Atompacman's Music Library Processors",
            (a_Sender, a_Args) =>
            {
               SHARED_LIB_DATA.Update();

               for (int i = 0; i < PROCESSORS.Count; ++i)
               {
                  if (ENABLED_PROCESSORS_CHECKBOXES[i].Checked)
                  {
                     PROCESSORS[i].Process(SHARED_LIB_DATA);
                  }
               }

               foreach (string songUrl in SHARED_LIB_DATA.SongTags.Keys)
               {
                  MusicBeeApi.Library_CommitTagsToFile(songUrl);
               }
            });

         return new PluginInfo
         {
            Author = "Atompacman",
            ConfigurationPanelHeight = PROCESSORS.Count * CONFIG_WINDOW_ELEMENT_HEIGHT,
            Description = "Various music library processors",
            MinApiRevision = MinApiRevision,
            MinInterfaceVersion = MinInterfaceVersion,
            Name = "Atompacman's Music Library Processors",
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
         ENABLED_PROCESSORS_CHECKBOXES.Clear();

         var enabledProcessorsTypeNames = new HashSet<string>();

         if (File.Exists(ConfigFilePath))
         {
            foreach (string line in File.ReadAllLines(ConfigFilePath))
            {
               enabledProcessorsTypeNames.Add(line);
            }
         }

         var controls = new List<Control>();

         for (int i = 0; i < PROCESSORS.Count; ++i)
         {
            var checkbox = new CheckBox
            {
               AutoSize = true,
               Location = new Point(0, 3 + i * CONFIG_WINDOW_ELEMENT_HEIGHT),
               Checked = enabledProcessorsTypeNames.Contains(PROCESSORS[i].GetType().Name)
            };

            ENABLED_PROCESSORS_CHECKBOXES.Add(checkbox);
            controls.Add(checkbox);
            controls.Add(new Label
            {
               AutoSize = true,
               Location = new Point(20, i * CONFIG_WINDOW_ELEMENT_HEIGHT),
               Text = PROCESSORS[i].GetType().Name
            });
         }

         var configPanel = (Panel) Control.FromHandle(a_PanelHandle);

         configPanel.Controls.AddRange(controls.ToArray());

         return false;
      }

      public void SaveSettings()
      {
         var stream = File.CreateText(ConfigFilePath);

         for (int i = 0; i < PROCESSORS.Count; ++i)
         {
            if (ENABLED_PROCESSORS_CHECKBOXES[i].Checked)
            {
               stream.Write(PROCESSORS[i].GetType().Name + stream.NewLine);
            }
         }

         stream.Flush();
         stream.Close();
      }

      public void Close(PluginCloseReason a_Reason)
      { }

      public void Uninstall()
      {
         File.Delete(ConfigFilePath);
      }

      public void ReceiveNotification(string a_SourceFileUrl, NotificationType a_Type)
      {
         // Receive event notifications from MusicBee. 
         // You need to set about.ReceiveNotificationFlags = PlayerEvents to receive all notifications, 
         // and not just the startup event.
      }

      #endregion
   }
}
