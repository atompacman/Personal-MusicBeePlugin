// MIT License
// 
// Copyright (c) 2017 Atompacman
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
using System.Linq;

using MusicBeePlugin.LibraryProcessor;

using Tag = MusicBeePlugin.Plugin.MetaDataType;

namespace MusicBeePlugin.Processor
{
   [RequiredTags(Tag.Album, Tag.Rating, Tag.RatingAlbum)]
   public sealed class AlbumRatingUpdater : ALibraryProcessor
   {
      #region Methods

      public override void Process(SharedLibraryData a_SharedLibData)
      {
         var songsByAlbum = new Dictionary<string, List<string>>();

         foreach (var songUrlAndTags in a_SharedLibData.SongTags)
         {
            if (songUrlAndTags.Value[Tag.RatingAlbum] != string.Empty)
            {
               MusicBeeApi.Library_SetFileTag(songUrlAndTags.Key, Tag.RatingAlbum, string.Empty);
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
               .Select(a_SongUrl => MusicBeeApi.Library_GetFileProperty(
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
               MusicBeeApi.Library_SetFileTag(songUrl, Tag.RatingAlbum, finalRating.ToString());
            }
         }
      }

      #endregion
   }
}
