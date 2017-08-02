// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// MIT License
// Copyright (c) 2017 Stained Glass Guild
// See file "LICENSE.txt" at project root for complete license
// ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~
// File: RequiredTagsAttribute.cs
// Creation: 2017-06
// Author: Jérémie Coulombe
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

using System;

namespace MusicBeePlugin.Processor.Framework
{
   public sealed class RequiredTagsAttribute : Attribute
   {
      #region Properties

      public Plugin.MetaDataType[] Tags { get; }

      #endregion

      #region Constructors

      public RequiredTagsAttribute(params Plugin.MetaDataType[] a_Tags)
      {
         Tags = a_Tags;
      }

      #endregion
   }
}
