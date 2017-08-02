// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// MIT License
// Copyright (c) 2017 Stained Glass Guild
// See file "LICENSE.txt" at project root for complete license
// ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~
// File: MissingArtworkPlaylistUpdater.cs
// Creation: 2017-06
// Author: Jérémie Coulombe
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

using MusicBeePlugin.Processor.Framework;

namespace MusicBeePlugin.Processor
{
   public sealed class MissingArtworkPlaylistUpdater : ASimplePlaylistUpdater
   {
      #region Constructors

      public MissingArtworkPlaylistUpdater()
         : base("QA\\No artwork")
      { }

      #endregion

      #region Methods

      protected override bool IsFileInPlaylist(string a_FileUrl, SharedLibraryData a_SharedLibData)
      {
         var locs = new Plugin.PictureLocations();
         string picUrl = "";
         byte[] data = { };
         return !Plugin.MusicBeeApi.Library_GetArtworkEx(a_FileUrl,
                   0, false, ref locs, ref picUrl, ref data) ||
                locs == Plugin.PictureLocations.None;
      }

      #endregion
   }
}
