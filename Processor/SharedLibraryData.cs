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

namespace MusicBeePlugin.LibraryProcessor
{
   public sealed class SharedLibraryData
   {
      #region Private fields

      private readonly HashSet<Plugin.MetaDataType> m_RequiredTags;

      #endregion

      #region Properties

      public Plugin.MusicBeeApiInterface MusicBeeApi { get; set; }

      // Song URL -> Tag -> Value
      public Dictionary<string, Dictionary<Plugin.MetaDataType, string>> SongTags { get; }

      // Playlist URL and name pairs
      public List<Tuple<string, string>> Playlists { get; }

      #endregion

      #region Constructors

      public SharedLibraryData(HashSet<Plugin.MetaDataType> a_RequiredTags)
      {
         m_RequiredTags = a_RequiredTags;
         SongTags = new Dictionary<string, Dictionary<Plugin.MetaDataType, string>>();
         Playlists = new List<Tuple<string, string>>();
      }

      #endregion

      #region Methods

      private void UpdateSongTags()
      {
         SongTags.Clear();

         var fileUrls = new string[0];

         if (!MusicBeeApi.Library_QueryFilesEx("", ref fileUrls))
         {
            throw new Exception();
         }

         var tagsToFetch = m_RequiredTags.ToArray();

         foreach (string fileUrl in fileUrls)
         {
            var tagValueMap = new Dictionary<Plugin.MetaDataType, string>(tagsToFetch.Length);

            if (tagsToFetch.Length != 0)
            {
               string[] tagValues = {};
               if (!MusicBeeApi.Library_GetFileTags(fileUrl, tagsToFetch, ref tagValues))
               {
                  throw new Exception();
               }

               if (tagValues.Length != tagsToFetch.Length)
               {
                  throw new Exception();
               }

               for (int i = 0; i < tagValues.Length; ++i)
               {
                  tagValueMap.Add(tagsToFetch[i], tagValues[i]);
               }
            }

            SongTags.Add(fileUrl, tagValueMap);
         }
      }

      private void UpdatePlaylists()
      {
         if (!MusicBeeApi.Playlist_QueryPlaylists())
         {
            throw new Exception();
         }

         string playlistUrl;

         while ((playlistUrl = MusicBeeApi.Playlist_QueryGetNextPlaylist()) != null)
         {
            string playlistName = MusicBeeApi.Playlist_GetName(playlistUrl);
            Playlists.Add(Tuple.Create(playlistUrl, playlistName));
         }
      }

      public void Update()
      {
         UpdateSongTags();
         UpdatePlaylists();
      }

      #endregion
   }
}
