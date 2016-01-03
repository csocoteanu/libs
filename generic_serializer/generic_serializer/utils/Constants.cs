using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using GenericSerializer.XmlUtils.Extensions;

namespace GenericSerializer.XmlUtils
{
    internal static class Constants
    {
        internal const string kNullString = "null";
        internal const string kRootElement = "root";
        internal const string kEmptyStruct = "empty_struct";
        internal const string kCompositeType = "composyte_type";
        internal const string kMemberType = "type";
        internal const string kClassString = "class";
        internal const string kStructString = "struct";
        internal const string kAssembly = "assembly";

        internal static readonly XmlWriterSettings kWriterSettings = Xml.CreateWriterSettings();
    }
}
