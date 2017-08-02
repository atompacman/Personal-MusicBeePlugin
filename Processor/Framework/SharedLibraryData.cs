// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// MIT License
// Copyright (c) 2017 Stained Glass Guild
// See file "LICENSE.txt" at project root for complete license
// ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~
// File: SharedLibraryData.cs
// Creation: 2017-08
// Author: Jérémie Coulombe
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicBeePlugin.Processor.Framework
{
   public sealed class SharedLibraryData
   {
      #region Private fields

      private readonly HashSet<Plugin.MetaDataType> m_RequiredTags;

      #endregion

      #region Properties

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

         if (!Plugin.MusicBeeApi.Library_QueryFilesEx("", ref fileUrls))
         {
            throw new Exception();
         }

         var tagsToFetch = m_RequiredTags.ToArray();

         foreach (string fileUrl in fileUrls)
         {
            var tagValueMap = new Dictionary<Plugin.MetaDataType, string>(tagsToFetch.Length);

            if (tagsToFetch.Length != 0)
            {
               string[] tagValues = { };
               if (!Plugin.MusicBeeApi.Library_GetFileTags(fileUrl, tagsToFetch, ref tagValues))
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
         if (!Plugin.MusicBeeApi.Playlist_QueryPlaylists())
         {
            throw new Exception();
         }

         string playlistUrl;

         while ((playlistUrl = Plugin.MusicBeeApi.Playlist_QueryGetNextPlaylist()) != null)
         {
            string playlistName = Plugin.MusicBeeApi.Playlist_GetName(playlistUrl);
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
