// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// MIT License
// Copyright (c) 2017 Stained Glass Guild
// See file "LICENSE.txt" at project root for complete license
// ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~
// File: AlbumRatingUpdater.cs
// Creation: 2017-06
// Author: Jérémie Coulombe
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

using System;
using System.Collections.Generic;
using System.Linq;

using MusicBeePlugin.Processor.Framework;

using Tag = MusicBeePlugin.Plugin.MetaDataType;

namespace MusicBeePlugin.Processor
{
   [RequiredTags(Tag.Album, Tag.Rating, Tag.RatingAlbum)]
   public sealed class AlbumRatingUpdater : ILibraryProcessor
   {
      #region Methods

      public void Process(SharedLibraryData a_SharedLibData)
      {
         var songsByAlbum = new Dictionary<string, List<string>>();

         foreach (var songUrlAndTags in a_SharedLibData.SongTags)
         {
            if (songUrlAndTags.Value[Tag.RatingAlbum] != string.Empty)
            {
               Plugin.MusicBeeApi.Library_SetFileTag(songUrlAndTags.Key, Tag.RatingAlbum,
                  string.Empty);
            }

            string album = songUrlAndTags.Value[Tag.Album];

            if (album == string.Empty)
            {
               continue;
            }

            List<string> albumSongs;
            if (!songsByAlbum.TryGetValue(album, out albumSongs))
            {
               albumSongs = new List<string>();
               songsByAlbum.Add(album, albumSongs);
            }
            albumSongs.Add(songUrlAndTags.Key);
         }

         var fullyRatedAlbums = (from albumSongs in songsByAlbum
                                 let isFullyRated = albumSongs.Value.All(
                                    a_SongUrl =>
                                       a_SharedLibData.SongTags[a_SongUrl][Tag.Rating] !=
                                       string.Empty)
                                 where isFullyRated
                                 select albumSongs.Key).ToList();

         foreach (string albumUrl in fullyRatedAlbums)
         {
            var songUrls = songsByAlbum[albumUrl];

            var songDurations = songUrls
               .Select(a_SongUrl => Plugin.MusicBeeApi.Library_GetFileProperty(
                  a_SongUrl, Plugin.FilePropertyType.Duration))
               .Select(TimeSpan.Parse)
               .Select(a_TimeSpan => a_TimeSpan.Ticks).ToArray();

            long albumDuration = songDurations.Sum();

            double finalRating =
               songUrls
                  .Select(a_SongUrl => a_SharedLibData.SongTags[a_SongUrl][Tag.Rating])
                  .Select(double.Parse)
                  .Select((a_RatingOn5, a_Idx) =>
                     a_RatingOn5 * 20 * (songDurations[a_Idx] / (double) albumDuration))
                  .Sum();

            foreach (string songUrl in songUrls)
            {
               Plugin.MusicBeeApi.Library_SetFileTag(songUrl, Tag.RatingAlbum,
                  finalRating.ToString());
            }
         }
      }

      #endregion
   }
}
