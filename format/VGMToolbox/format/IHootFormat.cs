﻿using System;
using System.Collections.Generic;
using System.Text;

using VGMToolbox.format.util;

namespace VGMToolbox.format
{
    public interface IHootFormat : IFormat
    {
        int GetStartingSong();
        int GetTotalSongs();
        string GetSongName();

        bool usesPlaylist();
        NezPlugM3uEntry[] GetPlaylistEntries();

        string GetHootDriverAlias();
        string GetHootDriverType();
        string GetHootDriver();    
    }
}
