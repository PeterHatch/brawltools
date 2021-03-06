﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrawlLib.SSBB.ResourceNodes
{
    public unsafe class cmShortNode : ClassMemberInstanceNode
    {
        public override int GetSize() { return 2; }

        public short _value;
        public short Value { get { return _value; } set { _value = value; SignalPropertyChange(); } }

        public override bool OnInitialize()
        {
            _value = *(bshort*)Data;
            return false;
        }

        public override void OnRebuild(VoidPtr address, int length, bool force)
        {
            *(bshort*)address = _value;
        }

        public override void WriteParams(System.Xml.XmlWriter writer, Dictionary<HavokClassNode, int> classNodes)
        {
            writer.WriteString(_value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
    }
}
