// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// MIT License
// Copyright (c) 2017 Stained Glass Guild
// See file "LICENSE.txt" at project root for complete license
// ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~
// File: ASimplePlaylistUpdater.cs
// Creation: 2017-06
// Author: Jérémie Coulombe
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

using System.Collections.Generic;
using System.Linq;

namespace MusicBeePlugin.Processor.Framework
{
   public abstract class ASimplePlaylistUpdater : APlaylistUpdater
   {
      #region Constructors

      protected ASimplePlaylistUpdater(string a_PlaylistName)
         : base(a_PlaylistName)
      { }

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
