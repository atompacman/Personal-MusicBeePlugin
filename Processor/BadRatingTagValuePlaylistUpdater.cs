// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// MIT License
// Copyright (c) 2017 Stained Glass Guild
// See file "LICENSE.txt" at project root for complete license
// ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~
// File: BadRatingTagValuePlaylistUpdater.cs
// Creation: 2017-06
// Author: Jérémie Coulombe
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

using System;

using MusicBeePlugin.Processor.Framework;

using Tag = MusicBeePlugin.Plugin.MetaDataType;

namespace MusicBeePlugin.Processor
{
   [RequiredTags(Tag.Rating, Tag.RatingAlbum)]
   public sealed class BadRatingTagValuePlaylistUpdater : ASimplePlaylistUpdater
   {
      #region Constructors

      public BadRatingTagValuePlaylistUpdater()
         : base("QA\\Bad rating value")
      { }

      #endregion

      #region Static methods

      private static bool IsBadSongRating(string a_Value)
      {
         if (a_Value == string.Empty)
         {
            return false;
         }

         double rating;

         if (!double.TryParse(a_Value, out rating))
         {
            throw new Exception();
         }

         return rating < 0 ||
                rating > 5 ||
                !Equals(rating * 2, Math.Round(rating * 2));
      }

      private static bool IsBadAlbumRating(string a_Value)
      {
         if (a_Value == string.Empty)
         {
            return false;
         }

         double rating;

         if (!double.TryParse(a_Value, out rating))
         {
            throw new Exception();
         }

         return rating < 0 || rating > 100;
      }

      #endregion

      #region Methods

      protected override bool IsFileInPlaylist(string a_FileUrl,
                                               SharedLibraryData a_SharedLibData)
      {
         return IsBadSongRating(a_SharedLibData.SongTags[a_FileUrl][Tag.Rating]) ||
                IsBadAlbumRating(a_SharedLibData.SongTags[a_FileUrl][Tag.RatingAlbum]);
      }

      #endregion
   }
}
