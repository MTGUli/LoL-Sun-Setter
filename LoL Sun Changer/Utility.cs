using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Util
{
    class Utility
    {
        public static T ToStructure<T>(Byte[] data) where T : new()
        {
            MemoryStream memoryStream = new MemoryStream(data);
            BinaryReader structureData = new BinaryReader(memoryStream);

            object struc = new T();

            struc = DynamicCaster.BytesToStruct(ref structureData, ref struc, struc.GetType());

            structureData.Close();
            return (T)struc;
        }

        public static Byte[] ToByteArray(object struc)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter structureData = new BinaryWriter(memoryStream);
            Type objType = struc.GetType();

            DynamicCaster.StructToBytes(ref structureData, struc, objType);
            byte[] data = new byte[memoryStream.Length];
            structureData.Close();

            data = memoryStream.ToArray();
            return data;
        }

        public static Char[] ToCharArr(string str, int arrLength)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(str);
            sb.Append(new String(new Char[arrLength - str.Length]));
            return sb.ToString().ToCharArray();
        }

        public static Byte[] ToByteArr(string str, int arrLength)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(str);
            sb.Append(new String(new Char[arrLength - str.Length]));
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public static String ReadNullTerminatedString(byte[] Arr, int index = 0)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = index; i < Arr.Length; i++)
            {
                if (Arr[i] > 0)
                {
                    sb.Append((char)Arr[i]);
                }
                else
                {
                    return sb.ToString();
                }
            }

            return sb.ToString();
        }
    }

    class DynamicCaster
    {
        public static object BytesToStruct(ref BinaryReader data, ref object struc, Type objType)
        {
            FieldInfo[] objFields = objType.GetFields();
            object structure = Activator.CreateInstance(objType);
            int position = -1;
            foreach (FieldInfo objField in objFields)
            {
                position++;
                if (objField.FieldType == typeof(System.Boolean))
                {
                    objField.SetValue(structure, data.ReadBoolean());
                }
                else if (objField.FieldType == typeof(System.Byte))
                {
                    objField.SetValue(structure, data.ReadByte());
                }
                else if (objField.FieldType == typeof(String))
                {
                }
                else if (objField.FieldType == typeof(byte[]))
                {
                    object[] objects = objField.GetCustomAttributes(typeof(DynamicCastSizeAttribute), false);
                    if (objects.Length != 1)
                        throw new Exception("You must add the attribute DynamicCastSizeAttribute to properties of type byte[]");
                    DynamicCastSizeAttribute i = (DynamicCastSizeAttribute)objects[0];

                    int increaseSize = 0;

                    object[] increaseSizeObjs = objField.GetCustomAttributes(typeof(DynamicCastIncreaseSize), false);
                    if (increaseSizeObjs.Length != 0)
                    {
                        DynamicCastIncreaseSize increase = (DynamicCastIncreaseSize)increaseSizeObjs[0];
                        increaseSize = increase.size;
                    }

                    if (i.size == 0)
                        objField.SetValue(structure, data.ReadBytes((int)(data.BaseStream.Length - data.BaseStream.Position)));
                    else if (i.size < 0)
                        objField.SetValue(structure, data.ReadBytes(Convert.ToInt32(objFields[position + i.size].GetValue(structure)) + increaseSize));
                    else
                        objField.SetValue(structure, data.ReadBytes(i.size));
                }
                else if (objField.FieldType == typeof(UInt64[]))
                {
                    object[] objects = objField.GetCustomAttributes(typeof(DynamicCastSizeAttribute), false);

                    if (objects.Length != 1)
                        throw new Exception("You must add the attribute DynamicCastSizeAttribute to properties of type byte[]");

                    DynamicCastSizeAttribute i = (DynamicCastSizeAttribute)objects[0];
                    Type type = objField.FieldType.GetElementType();
                    Array y = Array.CreateInstance(type, i.size);

                    for (int j = 0; j < i.size; j++)
                    {
                        y.SetValue(data.ReadUInt64(), j);
                    }

                    objField.SetValue(structure, y);
                }
                else if (objField.FieldType == typeof(System.Char))
                {
                    objField.SetValue(structure, data.ReadChar());
                }
                else if (objField.FieldType == typeof(char[]))
                {
                    object[] objects = objField.GetCustomAttributes(typeof(DynamicCastSizeAttribute), false);
                    if (objects.Length != 1)
                        throw new Exception("You must add the attribute DynamicCastSizeAttribute to properties of type byte[]");
                    DynamicCastSizeAttribute i = (DynamicCastSizeAttribute)objects[0];
                    objField.SetValue(structure, data.ReadChars(i.size));
                }
                else if (objField.FieldType == typeof(System.Decimal))
                {
                    objField.SetValue(structure, data.ReadDecimal());
                }
                else if (objField.FieldType == typeof(System.Double))
                {
                    objField.SetValue(structure, data.ReadDouble());
                }
                else if (objField.FieldType == typeof(System.SByte))
                {
                    objField.SetValue(structure, data.ReadSByte());
                }
                else if (objField.FieldType == typeof(System.Single))
                {
                    objField.SetValue(structure, data.ReadSingle());
                }
                else if (objField.FieldType == typeof(System.Int16))
                {
                    objField.SetValue(structure, data.ReadInt16());
                }
                else if (objField.FieldType == typeof(System.Int32))
                {
                    objField.SetValue(structure, data.ReadInt32());
                }
                else if (objField.FieldType == typeof(System.Int64))
                {
                    objField.SetValue(structure, data.ReadInt64());
                }
                else if (objField.FieldType == typeof(System.UInt16))
                {
                    objField.SetValue(structure, data.ReadUInt16());
                }
                else if (objField.FieldType == typeof(System.UInt32))
                {
                    objField.SetValue(structure, data.ReadUInt32());
                }
                else if (objField.FieldType == typeof(System.UInt64))
                {
                    objField.SetValue(structure, data.ReadUInt64());
                }
                else
                {
                    object[] objects = objField.GetCustomAttributes(typeof(DynamicCastSizeAttribute), false);
                    if (objects.Length == 0)
                    {
                        object fieldObj = objField.GetValue(struc);
                        objField.SetValue(structure, BytesToStruct(ref data, ref fieldObj, objField.FieldType));
                    }
                    else
                    {
                        DynamicCastSizeAttribute Attribute = (DynamicCastSizeAttribute)objects[0];
                        Type type = objField.FieldType.GetElementType();
                        object fieldObj = Activator.CreateInstance(type);

                        int ArraySize = Attribute.size;

                        //Dynamic Array, SIZE MUST BE IN FIELD BEFORE
                        if (ArraySize == -1)
                        {
                            //Placeholder for Dynamic Sizing
                            //Need to add extra attributes for byte size + how many bytes before it was
                            //Best to store original size then flip back
                            data.BaseStream.Position = data.BaseStream.Position - 4;
                            ArraySize = data.ReadInt32();
                        }

                        Array y = Array.CreateInstance(type, Attribute.size);

                        for (int i = 0; i < Attribute.size; i++)
                        {
                            y.SetValue(BytesToStruct(ref data, ref fieldObj, type), i);
                        }
                        objField.SetValue(structure, y);
                    }
                }
            }

            return structure;
        }

        public static void StructToBytes(ref BinaryWriter data, object struc, Type objType)
        {
            FieldInfo[] objFields = objType.GetFields();
            int position = -1;

            foreach (FieldInfo objField in objFields)
            {
                position++;
                if (objField.FieldType == typeof(System.Boolean))
                {
                    data.Write(BitConverter.GetBytes((Boolean)objField.GetValue(struc)));
                }
                else if (objField.FieldType == typeof(System.Byte))
                {
                    data.Write((Byte)objField.GetValue(struc));
                }
                else if (objField.FieldType == typeof(byte[]))
                {
                    data.Write((byte[])objField.GetValue(struc));
                }
                else if (objField.FieldType == typeof(System.Char))
                {
                    data.Write((Char)objField.GetValue(struc));
                }
                else if (objField.FieldType == typeof(char[]))
                {
                    data.Write((char[])objField.GetValue(struc));
                }
                else if (objField.FieldType == typeof(UInt64[]))
                {
                    object[] objects = objField.GetCustomAttributes(typeof(DynamicCastSizeAttribute), false);
                    DynamicCastSizeAttribute Attribute = (DynamicCastSizeAttribute)objects[0];

                    byte[] decoded = new byte[Attribute.size * sizeof(UInt64)];
                    Buffer.BlockCopy((Array)objField.GetValue(struc), 0, decoded, 0, decoded.Length);

                    data.Write(decoded);
                }
                else if (objField.FieldType == typeof(UInt32[]))
                {
                    object[] objects = objField.GetCustomAttributes(typeof(DynamicCastSizeAttribute), false);
                    DynamicCastSizeAttribute Attribute = (DynamicCastSizeAttribute)objects[0];

                    byte[] decoded = new byte[Attribute.size * sizeof(UInt32)];
                    Buffer.BlockCopy((Array)objField.GetValue(struc), 0, decoded, 0, decoded.Length);

                    data.Write(decoded);
                }
                else if (objField.FieldType == typeof(System.Decimal))
                {
                    data.Write(BitConverter.GetBytes(Convert.ToDouble((Decimal)objField.GetValue(struc))));
                }
                else if (objField.FieldType == typeof(System.Double))
                {
                    data.Write(BitConverter.GetBytes((Double)objField.GetValue(struc)));
                }
                else if (objField.FieldType == typeof(System.SByte))
                {
                    data.Write(BitConverter.GetBytes((SByte)objField.GetValue(struc)));
                }
                else if (objField.FieldType == typeof(System.Single))
                {
                    data.Write(BitConverter.GetBytes((Single)objField.GetValue(struc)));
                }
                else if (objField.FieldType == typeof(System.Int16))
                {
                    data.Write(BitConverter.GetBytes((Int16)objField.GetValue(struc)));
                }
                else if (objField.FieldType == typeof(System.Int32))
                {
                    data.Write(BitConverter.GetBytes((Int32)objField.GetValue(struc)));
                }
                else if (objField.FieldType == typeof(System.Int64))
                {
                    data.Write(BitConverter.GetBytes((Int64)objField.GetValue(struc)));
                }
                else if (objField.FieldType == typeof(System.UInt16))
                {
                    data.Write(BitConverter.GetBytes((UInt16)objField.GetValue(struc)));
                }
                else if (objField.FieldType == typeof(System.UInt32))
                {
                    data.Write(BitConverter.GetBytes((UInt32)objField.GetValue(struc)));
                }
                else if (objField.FieldType == typeof(System.UInt64))
                {
                    data.Write(BitConverter.GetBytes((UInt64)objField.GetValue(struc)));
                }
                else
                {
                    object[] objects = objField.GetCustomAttributes(typeof(DynamicCastSizeAttribute), false);
                    if (objects.Length == 0)
                        StructToBytes(ref data, objField.GetValue(struc), objField.FieldType);
                    else
                    {
                        DynamicCastSizeAttribute Attribute = (DynamicCastSizeAttribute)objects[0];
                        Array y = (Array)objField.GetValue(struc);
                        int ArraySize = Attribute.size;
                        //Dynamic Sizing, its a negative value to find a previous address which contains the size
                        if (ArraySize < 0)
                        {
                            //Get Previous Data
                            ArraySize = Convert.ToInt32(objFields[position + ArraySize].GetValue(struc));
                        }
                        object[] divisionObj = objField.GetCustomAttributes(typeof(DynamicCastDivision), false);
                        int division = 1;

                        if (divisionObj.Length != 0)
                        {
                            DynamicCastDivision divisionAttribute = (DynamicCastDivision)divisionObj[0];
                            division = divisionAttribute.division;
                        }

                        for (int i = 0; i < ArraySize / division; i++)
                        {
                            StructToBytes(ref data, y.GetValue(i), objField.FieldType.GetElementType());
                        }
                    }
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DynamicCastSizeAttribute : Attribute
    {
        internal int size;

        public DynamicCastSizeAttribute(int size)
        {
            this.size = size;
        }
    }
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DynamicCastDivision : Attribute
    {
        internal int division;

        public DynamicCastDivision(int division)
        {
            this.division = division;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DynamicCastIncreaseSize : Attribute
    {
        internal int size;

        public DynamicCastIncreaseSize(int size)
        {
            this.size = size;
        }
    }
}
