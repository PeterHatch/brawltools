﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace BrawlLib.SSBBTypes {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public unsafe struct STDT//Stage Trap Data Table
   {
		public const uint Tag = 0x54445453;

		public uint _tag;
		public bint _version;
		public bint _unk1;
		public bint _unk2;
		public bint _entryOffset;

		public VoidPtr Address { get { fixed (void* ptr = &this)return ptr; } }
		public bfloat* Entries { get { return (bfloat*)(Address + _entryOffset); } }
	}
}
