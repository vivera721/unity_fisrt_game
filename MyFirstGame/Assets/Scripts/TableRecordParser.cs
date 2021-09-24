using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Text;

public class MarshalTableConstant
{
    public const int charBufferSize = 256;
}

public class TableRecordParser<TMarshalStruct>
{
    public TMarshalStruct ParseRecordLine(string line)
    {
        // TMarshalStruct ũ�⿡ ���缭 Byte �迭 �Ҵ�
        Type type = typeof(TMarshalStruct);
        int structSize = Marshal.SizeOf(type);        // System.Runtime.InteropServices.Marshal
        byte[] structBytes = new byte[structSize];
        int structBytesIndex = 0;

        // line ���ڿ��� spliter �� �ڸ�
        const string spliter = ",";
        string[] fieldDataList = line.Split(spliter.ToCharArray());
        // Ÿ���� ���� ���̳ʸ��� �Ľ��Ͽ� ����
        Type dataType;
        string splited;
        byte[] fieldByte;
        byte[] keyBytes;

        FieldInfo[] fieldInfos = type.GetFields();                      // System.Reflection.FieldInfo
        for (int i = 0; i < fieldInfos.Length; i++)
        {
            dataType = fieldInfos[i].FieldType;
            splited = fieldDataList[i];

            fieldByte = new byte[4];
            MakeBytesByFieldType(out fieldByte, dataType, splited);

            // fieldByte�� ���� structBytes�� ����
            //for (int index = 0; index < fieldByte.Length; index++)
            //{
            //    structBytes[structBytesIndex++] = fieldByte[index];
            //}

            Buffer.BlockCopy(fieldByte, 0, structBytes, structBytesIndex, fieldByte.Length);
            structBytesIndex += fieldByte.Length;

            // ù��° �ʵ带 Key ������ ����ϱ� ���� ���
            if (i == 0)
                keyBytes = fieldByte;

        }
        // mashaling
        TMarshalStruct tStruct = MakeStructFromBytes<TMarshalStruct>(structBytes);
        //AddData(keyBytes, tStruct);
        return tStruct;
    }

    /// <summary>
    /// ���ڿ� splite�� �־��� dataType �� �°� fieldByte �迭�� ��ȯ�ؼ� ��ȯ
    /// </summary>
    /// <param name="fieldByte">��� ���� ���� �迭</param>
    /// <param name="dataType">splite�� ��ȯ�Ҷ� ���� �ڷ���</param>
    /// <param name="splite">��ȯ�� ���� �ִ� ���ڿ�</param>
    protected void MakeBytesByFieldType(out byte[] fieldByte, Type dataType, string splite)
    {
        fieldByte = new byte[1];

        if (typeof(int) == dataType)
        {
            fieldByte = BitConverter.GetBytes(int.Parse(splite));    // System.BitConverter
        }
        else if (typeof(float) == dataType)
        {
            fieldByte = BitConverter.GetBytes(float.Parse(splite));
        }
        else if (typeof(bool) == dataType)
        {
            bool value = bool.Parse(splite);
            int temp = value ? 1 : 0;

            fieldByte = BitConverter.GetBytes((int)temp);
        }
        else if (typeof(string) == dataType)
        {
            fieldByte = new byte[MarshalTableConstant.charBufferSize];      // �������� �ϱ����ؼ� ����ũ�� ���۸� ����
            byte[] byteArr = Encoding.UTF8.GetBytes(splite);                // System.Text.Encoding;
            // ��ȯ�� byte �迭�� ����ũ�� ���ۿ� ����
            Buffer.BlockCopy(byteArr, 0, fieldByte, 0, byteArr.Length);     // System.Buffer;
        }
    }

    /// <summary>
    /// �������� ���� byte �迭�� T�� ����ü ��ȯ
    /// </summary>
    /// <typeparam name="T">�������� �����ϰ� ���ǵ� ����ü�� Ÿ��</typeparam>
    /// <param name="bytes">�������� �����Ͱ� ����� �迭</param>
    /// <returns>��ȯ�� T�� ����ü</returns>
    public static T MakeStructFromBytes<T>(byte[] bytes)
    {
        int size = Marshal.SizeOf(typeof(T));
        IntPtr ptr = Marshal.AllocHGlobal(size);    // ���� �޸� �Ҵ�

        Marshal.Copy(bytes, 0, ptr, size);          // ����

        T tStruct = (T)Marshal.PtrToStructure(ptr, typeof(T));  // �޸𸮷κ��� T�� ����ü�� ��ȯ
        Marshal.FreeHGlobal(ptr);       // �Ҵ�� �޸� ����
        return tStruct; // ��ȯ�� �� ��ȯ
    }
}