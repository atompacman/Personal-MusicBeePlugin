// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// MIT License
// Copyright (c) 2018 Stained Glass Guild
// See file "LICENSE.txt" at project root for complete license
// ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~
// Project: RYMExporter
// File: Interface.cs
// Creation: 2018-02
// Author: Jérémie Coulombe
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

using System;
using System.Text;
using System.Windows.Forms;

namespace MusicBeePlugin.RYMExporter.Source
{
   public partial class Interface : UserControl
   {
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

      #region Private fields

      private readonly string[] m_TrackFiles;

      #endregion

      #region Constructors

      public Interface()
      {
         InitializeComponent();

         m_TrackFiles = new string[] {};
         if (!Plugin.MusicBeeApi.Library_QueryFilesEx("domain=SelectedFiles", ref m_TrackFiles))
         {
            richTextBox1.Text = "Error: Call to Library_QueryFilesEx failed";
            return;
         }

         label1.Text = GetRatingString(trackBar1.Value);

         if (m_TrackFiles.Length == 0)
         {
            richTextBox1.Text = "(empty selection)";
            return;
         }

         textBox1.Text = Plugin.MusicBeeApi.Library_GetFileTag(m_TrackFiles[0],
            Plugin.MetaDataType.Genre);

         var text = new StringBuilder();

         int lineCount = 0;
         foreach (string trackFile in m_TrackFiles)
         {
            text.AppendLine(GetTrackLine(trackFile, lineCount++));
         }

         richTextBox1.Text = text.ToString();
      }

      #endregion

      #region Static methods

      private static string GetRatingString(int a_RatingOn20)
      {
         if (a_RatingOn20 < 1 || a_RatingOn20 > 20)
         {
            return "RATING ON 20 IS NOT BETWEEN 1 AND 20";
         }

         var line = new StringBuilder();

         int numFullCircles = a_RatingOn20 / 4;
         line.Append('⏺', numFullCircles);

         int remaining = a_RatingOn20 - numFullCircles * 4;

         if (remaining == 1)
         {
            line.Append('◔');
         }
         else if (remaining == 2)
         {
            line.Append('◐');
         }
         else if (remaining == 3)
         {
            line.Append('◕');
         }

         line.Append('⭕', 5 - (int) Math.Ceiling(a_RatingOn20 / 4.0));

         return line.ToString();
      }

      private static string GetTrackLine(string a_TrackFile, int a_LineIdx)
      {
         var tags = new[] {Plugin.MetaDataType.TrackTitle, Plugin.MetaDataType.Rating};
         var tagValues = new string[] {};
         if (!Plugin.MusicBeeApi.Library_GetFileTags(a_TrackFile, tags, ref tagValues) ||
             tagValues.Length != tags.Length)
         {
            return "Library_GetFileTags failed";
         }

         string title = tagValues[0];
         string ratingStr = tagValues[1];

         if (ratingStr == string.Empty)
         {
            return title + ": NOT RATED";
         }

         if (!double.TryParse(ratingStr, out double ratingDouble))
         {
            return title + ": RATING IS NOT A DOUBLE VALUE";
         }

         if (ratingDouble < 0.5 || ratingDouble > 5)
         {
            return title + ": RATING IS NOT BETWEEN 0.5 AND 5";
         }

         int rating = (int) Math.Round(ratingDouble * 2);

         if (Math.Abs(rating - ratingDouble * 2) > 1e-10)
         {
            return title + ": RATING IS NOT A MULTIPLE OF 0.5";
         }

         var line = new StringBuilder();
         line.Append("[color #");
         line.Append(RATING_COLORS[rating - 1]);
         line.Append("] ");
         line.Append((a_LineIdx + 1).ToString("D2"));
         line.Append("  [b]");
         line.Append(GetRatingString(rating * 2));
         line.Append(" [/b]  ");
         if (rating > 5)
         {
            line.Append("[b]");
         }
         line.Append(title);
         if (rating > 5)
         {
            line.Append("[/b]");
         }
         line.Append(" [/color]");

         return line.ToString();
      }

      #endregion

      #region Methods

      private void TrackBar1_Scroll(object a_Sender, EventArgs a_E)
      {
         label1.Text = GetRatingString(trackBar1.Value);
      }

      private void Button1_Click(object a_Sender, EventArgs a_E)
      {
         var text = new StringBuilder();

         text.Append("[b]Overall:[/b]  ");
         text.AppendLine(GetRatingString(trackBar1.Value));
         text.AppendLine();
         text.AppendLine(richTextBox1.Text);
         text.Append("[b]Genre:[/b]  ");
         text.Append(textBox1.Text);

         Clipboard.SetText(text.ToString());

         string ratingOn100 = (trackBar1.Value * 5).ToString();
         foreach (string trackFile in m_TrackFiles)
         {
            if (!Plugin.MusicBeeApi.Library_SetFileTag(trackFile, Plugin.MetaDataType.RatingAlbum,
               ratingOn100))
            {
               richTextBox1.Text = "Error: Call to Library_SetFileTag failed";
            }

            if (!Plugin.MusicBeeApi.Library_CommitTagsToFile(trackFile))
            {
               richTextBox1.Text = "Error: Call to Library_CommitTagsToFile failed";
            }
         }

         Plugin.MusicBeeApi.MB_RemovePanel(this);
      }

      #endregion
   }
}
