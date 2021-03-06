﻿    using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BrawlLib.SSBB.Types
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct SndBgmTitleHeader
    {
        public bint _Length;
        public bint _DataLength;

        public bint _OffCount; // always 0
        public bint _DataTable; // always 1 entry, which is a concatenation of several SndBgmTitleEntry structures

        public int _pad0;
        public int _pad1;
        public int _pad2;
        public int _pad3;

        private VoidPtr Address { get { fixed (void* ptr = &this)return ptr; } }
        public string Str { get { return new string((sbyte*)Address + sizeof(SndBgmTitleHeader) + _DataLength + (_OffCount * 4) + (_DataTable * 8)); } }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct SndBgmTitleEntry
    {
        public bint _ID;
        public bint _unknown04;
        public bint _unknown08;
        public bint _unknown0c;
        public bint _SongTitleIndex;
        public bint _unknown14;
        public bint _unknown18;
        public bint _unknown1c;
        public bint _unknown20;
        public bint _unknown24;
        public bint _unknown28;
        public bint _unknown2c;

        private VoidPtr Address { get { fixed (void* ptr = &this)return ptr; } }
    }
}
