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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MusicBeePlugin
{
   public partial class Plugin
   {
      #region Nested types

      private struct TrackInfo
      {
         public string Title;
         public int RatingOn10;
         public double DurationInMinutes;
      }

      #endregion

      #region Compile-time constants

      private const string PLUGIN_NAME = "RYMExporter";
      private const int CONFIG_PANEL_HEIGHT = 36;

      #endregion

      #region Runtime constants

      private static readonly string[] RATING_COLORS =
      {
         "E8E8E8",
         "E1E1E1",
         "D5D5D5",
         "C3C3C3",
         "99C390",
         "62C48F",
         "46ABD1",
         "304EDB",
         "6619E5",
         "DD00EF"
      };

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

      #region Static methods

      private static void ShowError(string a_Error)
      {
         MessageBox.Show(a_Error, "RYM exportation error", MessageBoxButtons.OK,
            MessageBoxIcon.Error);
      }

      private static void ShowWarning(string a_Warning)
      {
         MessageBox.Show(a_Warning, "RYM exportation warning", MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
      }

      private static List<TrackInfo> GetSelectedTracksInfo(string[] a_SelectedTrackFiles)
      {
         // On obtient les info nécessaires des pistes sélectionnées
         var selectedTracksInfo = new List<TrackInfo>();

         var trackTagTypes = new[]
         {
            MetaDataType.TrackTitle,
            MetaDataType.Rating,
            MetaDataType.Album
         };

         string albumTitle = null;

         foreach (string trackFile in a_SelectedTrackFiles)
         {
            // On obtient les tags de la piste
            var trackTags = new string[] {};

            if (!MusicBeeApi.Library_GetFileTags(trackFile, trackTagTypes, ref trackTags) ||
                trackTags.Length != trackTagTypes.Length)
            {
               ShowError("Call to Library_GetFileTags failed");
               return null;
            }

            var trackInfo = new TrackInfo {Title = trackTags[0]};
            string trackRatingStr = trackTags[1];
            string trackAlbumTitle = trackTags[2];

            // On valide que les pistes font partie du même album
            if (albumTitle == null)
            {
               if (trackAlbumTitle == string.Empty)
               {
                  ShowWarning("Track with empty album name: " + trackFile);
               }
            }
            else if (trackAlbumTitle != albumTitle)
            {
               ShowWarning("Tracks are not from the same album: " + albumTitle + " versus " +
                           trackAlbumTitle);
            }
            albumTitle = trackAlbumTitle;

            // On obtient la durée de la piste en minutes
            string durationStr = MusicBeeApi.Library_GetFileProperty(trackFile,
               FilePropertyType.Duration);

            if (durationStr.Length == 4)
            {
               durationStr = "0" + durationStr;
            }
            if (durationStr.Length == 5)
            {
               durationStr = "00:" + durationStr;
            }
            try
            {
               trackInfo.DurationInMinutes = TimeSpan.Parse(durationStr).TotalMinutes;
            }
            catch
            {
               ShowError("Track duration is invalid: (" + durationStr + ") " + trackFile);
               return null;
            }

            // On obtient la note de la piste de 1 à 10
            if (trackRatingStr == string.Empty)
            {
               ShowError("Track is not rated: " + trackFile);
               return null;
            }

            if (!double.TryParse(trackRatingStr, out double ratingDouble))
            {
               ShowError("Track rating is not valid: (" + trackRatingStr + ") " + trackFile);
               return null;
            }

            if (ratingDouble < 0.5 || ratingDouble > 5)
            {
               ShowError("Track rating is not between 0.5 and 5: (" + trackRatingStr + ") " +
                         trackFile);
               return null;
            }

            trackInfo.RatingOn10 = (int) Math.Round(ratingDouble * 2);

            if (Math.Abs(trackInfo.RatingOn10 - ratingDouble * 2) > 1e-10)
            {
               ShowError("Track rating is not a multiple of 0.5: (" + trackRatingStr + ") " +
                         trackFile);
               return null;
            }

            selectedTracksInfo.Add(trackInfo);
         }

         return selectedTracksInfo;
      }

      private static double ComputeAlbumRatingOn100(List<TrackInfo> a_TracksInfo)
      {
         // Ex.: Une piste trois étoiles équivaut à N pistes 2 étoiles et demi
         const double qualityLevelRatio = 3;

         // Ex.: Une piste est deux fois meilleure qu'une piste de même qualité si elle est N fois plus longue
         const double lengthRatio = 5;

         // 0 -> Aucun concept d'homogénéité: tu peux avoir une toune malade et 1000 mauvaise
         //      pis le rating final se porte bien.
         // 1 -> Homogénéité directe: on fait la moyenne de bonne toune par toune.
         const double homogeneityRatio = 0.3;

         // Via des équivalences, on réduit toutes les pièces à des pistes de trois étoiles de 4 minutes
         double numThreeStar4MinSongs =
            a_TracksInfo.Sum(a_TrackInfo =>
               Math.Pow(qualityLevelRatio, a_TrackInfo.RatingOn10 - 6) *
               Math.Pow(a_TrackInfo.DurationInMinutes / 4.0,
                  Math.Log(2) / Math.Log(lengthRatio)));

         // On calcule le ratio de bonnes pistes
         // La valeur de homogeneityRatio est basée sur mes 56 premières évaluations
         double goodSongsRatio = numThreeStar4MinSongs /
                                 Math.Pow(a_TracksInfo.Count, homogeneityRatio);

         // Transformation vers une valeur sur 100 basé sur mes 56 premières évaluations
         return Math.Min(39.64 * Math.Pow(goodSongsRatio, 0.2704), 100);
      }

      private static string GetRatingString(int a_RatingOn20, bool a_UseDiamonds)
      {
         var line = new StringBuilder();

         int numFullCircles = a_RatingOn20 / 4;
         line.Append(a_UseDiamonds ? '◆' : '⏺', numFullCircles);

         int remaining = a_RatingOn20 - numFullCircles * 4;

         if (remaining == 1)
         {
            line.Append(a_UseDiamonds ? '?' : '◔');
         }
         else if (remaining == 2)
         {
            line.Append(a_UseDiamonds ? '⬖' : '◐');
         }
         else if (remaining == 3)
         {
            line.Append(a_UseDiamonds ? '?' : '◕');
         }

         line.Append(a_UseDiamonds ? '◇' : '⭘', 5 - (int) Math.Ceiling(a_RatingOn20 / 4.0));

         return line.ToString();
      }

      private static string GetTrackLine(TrackInfo a_TrackInfo, int a_TrackNum)
      {
         var line = new StringBuilder();

         line.Append("[color #");
         line.Append(RATING_COLORS[a_TrackInfo.RatingOn10 - 1]);
         line.Append("] ");
         line.Append(a_TrackNum.ToString("D2"));
         line.Append("  [b]");
         line.Append(GetRatingString(a_TrackInfo.RatingOn10 * 2, true));
         line.Append(" [/b]  ");
         if (a_TrackInfo.RatingOn10 > 5)
         {
            line.Append("[b]");
         }
         line.Append(a_TrackInfo.Title);
         if (a_TrackInfo.RatingOn10 > 5)
         {
            line.Append("[/b]");
         }
         line.Append(" [/color]");

         return line.ToString();
      }

      private static void Export()
      {
         // On obtient la liste des pistes sélectionnées
         var selectedTrackFiles = new string[] {};

         if (!MusicBeeApi.Library_QueryFilesEx("domain=SelectedFiles", ref selectedTrackFiles))
         {
            ShowError("Call to Library_QueryFilesEx failed");
            return;
         }

         if (selectedTrackFiles.Length == 0)
         {
            ShowError("At least one file must be selected");
            return;
         }

         // On obtient les infos des pistes sélectionnées
         var tracksInfo = GetSelectedTracksInfo(selectedTrackFiles);
         if (tracksInfo == null)
         {
            return;
         }

         // On calcule l'évaluation de l'album
         double albumRatingOn100 = ComputeAlbumRatingOn100(tracksInfo);

         if (albumRatingOn100 < 0)
         {
            return;
         }
         int albumRatingOn20 = (int) Math.Round(albumRatingOn100 / 5.0);

         // On obtient le genre de l'album
         var tagsTypes = new[] {MetaDataType.Genre};
         var tagValues = new string[] {};
         if (!MusicBeeApi.Library_GetFileTags(selectedTrackFiles[0], tagsTypes, ref tagValues) ||
             tagValues.Length != tagsTypes.Length)
         {
            ShowError("Call to Library_GetFileTags failed");
            return;
         }

         // On construit l'exportation texte de l'évaluation
         var text = new StringBuilder();

         text.Append("[b]Genre:[/b]  ");
         text.Append(tagValues[0]);
         text.AppendLine();
         text.AppendLine();
         text.Append("[b]Rating:[/b]  ");
         text.AppendLine(GetRatingString(albumRatingOn20, false));
         text.AppendLine();
         int trackNum = 1;
         foreach (var trackInfo in tracksInfo)
         {
            text.AppendLine(GetTrackLine(trackInfo, trackNum));
            ++trackNum;
         }

         // On met le texte dans le presse-papier
         Clipboard.SetText(text.ToString());

         foreach (string trackFile in selectedTrackFiles)
         {
            if (!MusicBeeApi.Library_SetFileTag(trackFile, MetaDataType.RatingAlbum,
               Math.Round(albumRatingOn100).ToString(CultureInfo.InvariantCulture)))
            {
               ShowError("Call to Library_SetFileTag failed");
               return;
            }

            if (!MusicBeeApi.Library_CommitTagsToFile(trackFile))
            {
               ShowError("Call to Library_CommitTagsToFile failed");
               return;
            }
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
            (a_Sender, a_Args) => { Export(); });

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
