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

using System.Collections.Generic;
using System.Linq;

using MusicBeePlugin.LibraryProcessor;

namespace MusicBeePlugin.Processor
{
   public abstract class ASimplePlaylistUpdater : APlaylistUpdater
   {
      #region Constructors

      protected ASimplePlaylistUpdater(string a_PlaylistName)
         : base(a_PlaylistName)
      {}

      #endregion

      #region Abstract methods

      protected abstract bool IsFileInPlaylist(string a_FileUrl,
                                               SharedLibraryData a_SharedLibData);

      #endregion

      #region Methods

      protected override List<string> CreatePlaylist(SharedLibraryData a_SharedLibData)
      {
         return a_SharedLibData.SongTags.Keys.Where(
            a_FileUrl => IsFileInPlaylist(a_FileUrl, a_SharedLibData)).ToList();
      }

      #endregion
   }
}
