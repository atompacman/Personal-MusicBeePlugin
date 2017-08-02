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

using MusicBeePlugin.LibraryProcessor;

namespace MusicBeePlugin.Processor
{
   public abstract class APlaylistUpdater : ALibraryProcessor
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

      public override void Process(SharedLibraryData a_SharedLibData)
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

         if (!MusicBeeApi.Playlist_SetFiles(playlistToUpdateUrl, filesUrls.ToArray()))
         {
            throw new Exception();
         }
      }

      #endregion
   }
}
