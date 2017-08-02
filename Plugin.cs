// MIT License
// 
// Copyright (c) 2012 Atompacman
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using MusicBeePlugin.LibraryProcessor;
using MusicBeePlugin.Processor;

namespace MusicBeePlugin
{
   public partial class Plugin
   {
      #region Runtime constants

      private static readonly List<ALibraryProcessor> PROCESSORS;
      private static readonly SharedLibraryData SHARED_LIB_DATA;

      #endregion

      #region Constructors

      static Plugin()
      {
         PROCESSORS = new List<ALibraryProcessor>();

         var requiredTags = new HashSet<MetaDataType>();

         foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
         {
            if (!typeof(ALibraryProcessor).IsAssignableFrom(type) || type.IsAbstract)
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
            if ((ctors.Length != 1) || (ctors[0].GetParameters().Length != 0))
            {
               throw new Exception();
            }

            PROCESSORS.Add(ctors[0].Invoke(BindingFlags.Default, null, new object[] {},
               CultureInfo.CurrentCulture) as ALibraryProcessor);
         }

         SHARED_LIB_DATA = new SharedLibraryData(requiredTags);
      }

      #endregion

      #region Methods

      public PluginInfo Initialise(IntPtr a_ApiInterfacePtr)
      {
         var musicBeeApi = new MusicBeeApiInterface();
         musicBeeApi.Initialise(a_ApiInterfacePtr);

         foreach (var processor in PROCESSORS)
         {
            processor.MusicBeeApi = musicBeeApi;
         }
         SHARED_LIB_DATA.MusicBeeApi = musicBeeApi;

         musicBeeApi.MB_AddMenuItem(
            "mnuTools/Atompacman's Music Library Processors",
            "Run Atompacman's Music Library Processors",
            (a_Sender, a_Args) =>
            {
               SHARED_LIB_DATA.Update();

               foreach (var processor in PROCESSORS)
               {
                  processor.Process(SHARED_LIB_DATA);
               }

               foreach (string songUrl in SHARED_LIB_DATA.SongTags.Keys)
               {
                  musicBeeApi.Library_CommitTagsToFile(songUrl);
               }
            });

         return new PluginInfo
         {
            Author = "Atompacman",
            ConfigurationPanelHeight = PROCESSORS.Count * 30,
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
         var controls = new List<Control>();

         for (int i = 0; i < 3; ++i)
         {
            controls.Add(new CheckBox
            {
               AutoSize = true,
               Location = new Point(0, 3 + i * 40)
            });
            controls.Add(new Label
            {
               AutoSize = true,
               Location = new Point(20, i * 40),
               Text = "prompt:"
            });
         }

         var configPanel = (Panel) Control.FromHandle(a_PanelHandle);

         configPanel.Controls.AddRange(controls.ToArray());

         return false;
      }

      public void SaveSettings()
      {
         // Called by MusicBee when the user clicks Apply or Save in the MusicBee Preferences screen.
         // its up to you to figure out whether anything has changed and needs updating.
         // (Setting_GetPersistentStoragePath)
      }

      public void Close(PluginCloseReason a_Reason)
      {
         // MusicBee is closing the plugin (plugin is being disabled by user or MusicBee is shutting down).
      }

      public void Uninstall()
      {
         // Uninstall this plugin. Clean up any persisted files
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
