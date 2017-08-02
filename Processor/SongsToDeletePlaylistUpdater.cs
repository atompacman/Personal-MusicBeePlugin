// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// MIT License
// Copyright (c) 2017 Stained Glass Guild
// See file "LICENSE.txt" at project root for complete license
// ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~
// File: SongsToDeletePlaylistUpdater.cs
// Creation: 2017-06
// Author: Jérémie Coulombe
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

using System;

using MusicBeePlugin.Processor.Framework;

namespace MusicBeePlugin.Processor
{
   [RequiredTags(Plugin.MetaDataType.Rating)]
   public sealed class SongsToDeletePlaylistUpdater : ASimplePlaylistUpdater
   {
      #region Constructors

      public SongsToDeletePlaylistUpdater()
         : base("QA\\To delete")
      { }

      #endregion

      #region Methods

      protected override bool IsFileInPlaylist(string a_FileUrl, SharedLibraryData a_SharedLibData)
      {
         string ratingStr = a_SharedLibData.SongTags[a_FileUrl][Plugin.MetaDataType.Rating];

         if (ratingStr == string.Empty)
         {
            return false;
         }

         double rating;

         if (!double.TryParse(ratingStr, out rating))
         {
            throw new Exception();
         }

         // ReSharper disable once CompareOfFloatsByEqualityOperator
         return rating == 0;
      }

      #endregion
   }
}
