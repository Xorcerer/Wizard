using System;
using Xorcerer.Wizard.Utilities;
using System.IO;
using ProtoBuf.Meta;

namespace Xorcerer.Wizard.Network
{
    public class ProtobufMessageSerializer : LengthPrefixMessageSerializerBase
    {
        TypeDict _protoDict;

        public ProtobufMessageSerializer(TypeDict protoDict)
        {
            _protoDict = protoDict;
        }

        #region implemented abstract members of LengthPrefixMessageSerializerBase

        protected override object DoDeserialize(ArraySegment<byte> segment)
        {
            var input = new MemoryStream(segment.Array, segment.Offset, segment.Count);

            var reader = new BinaryReader(input);
            int typeId = reader.ReadInt32();

            return RuntimeTypeModel.Default.Deserialize(input, null, _protoDict[typeId]);
        }

        protected override int DoSerialize(ArraySegment<byte> segment, object message)
        {
            var output = new MemoryStream(segment.Array, segment.Offset, segment.Count);

            var writer = new BinaryWriter(output);
            writer.Write(_protoDict[message.GetType()]);

            RuntimeTypeModel.Default.Serialize(output, message);

            return (int)output.Position;
        }

        #endregion
    }
}

