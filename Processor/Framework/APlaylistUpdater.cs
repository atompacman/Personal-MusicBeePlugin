// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// MIT License
// Copyright (c) 2017 Stained Glass Guild
// See file "LICENSE.txt" at project root for complete license
// ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~
// File: APlaylistUpdater.cs
// Creation: 2017-06
// Author: Jérémie Coulombe
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

using System;
using System.Collections.Generic;

namespace MusicBeePlugin.Processor.Framework
{
   public abstract class APlaylistUpdater : ILibraryProcessor
   {
      #region Private fields

      private readonly string m_PlaylistName;

      #endregion

      #region Constructors

      protected APlaylistUpdater(string a_PlaylistName)
      {
         m_PlaylistName = a_PlaylistName;
      }

      #endregion

      #region Abstract methods

      protected abstract List<string> CreatePlaylist(SharedLibraryData a_SharedLibData);

      #endregion

      #region Methods

      public void Process(SharedLibraryData a_SharedLibData)
      {
         string playlistToUpdateUrl = string.Empty;

         foreach (var playlistUrlAndName in a_SharedLibData.Playlists)
         {
            if (playlistUrlAndName.Item2 == m_PlaylistName + " [Plugin]")
            {
               playlistToUpdateUrl = playlistUrlAndName.Item1;
            }
         }

         if (playlistToUpdateUrl.Length == 0)
         {
            throw new Exception();
         }

         var filesUrls = CreatePlaylist(a_SharedLibData);

         if (!Plugin.MusicBeeApi.Playlist_SetFiles(playlistToUpdateUrl, filesUrls.ToArray()))
         {
            throw new Exception();
         }
      }

      #endregion
   }
}
