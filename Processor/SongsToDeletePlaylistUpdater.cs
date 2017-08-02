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

using MusicBeePlugin.LibraryProcessor;

namespace MusicBeePlugin.Processor
{
   [RequiredTags(Plugin.MetaDataType.Rating)]
   public sealed class SongsToDeletePlaylistUpdater : ASimplePlaylistUpdater
   {
      #region Constructors

      public SongsToDeletePlaylistUpdater()
         : base("QA\\To delete")
      {}

      #endregion

      #region Methods

      protected override bool IsFileInPlaylist(string a_FileUrl, SharedLibraryData a_SharedLibData)
      {
         string ratingStr = a_SharedLibData.SongTags[a_FileUrl][Plugin.MetaDataType.Rating];

         if (ratingStr == string.Empty)
         {
            return false;
         }

         double rating;

         if (!double.TryParse(ratingStr, out rating))
         {
            throw new Exception();
         }

         // ReSharper disable once CompareOfFloatsByEqualityOperator
         return rating == 0;
      }

      #endregion
   }
}
