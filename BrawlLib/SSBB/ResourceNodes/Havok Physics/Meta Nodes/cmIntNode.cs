﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrawlLib.SSBB.ResourceNodes
{
    public unsafe class cmIntNode : ClassMemberInstanceNode
    {
        public override int GetSize() { return 4; }

        public int _value;
        public int Value { get { return _value; } set { _value = value; SignalPropertyChange(); } }

        public override bool OnInitialize()
        {
            _value = *(bint*)Data;
            return false;
        }

        public override void OnRebuild(VoidPtr address, int length, bool force)
        {
            *(bint*)address = _value;
        }

        public override void WriteParams(System.Xml.XmlWriter writer, Dictionary<HavokClassNode, int> classNodes)
        {
            writer.WriteString(_value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
    }
}
