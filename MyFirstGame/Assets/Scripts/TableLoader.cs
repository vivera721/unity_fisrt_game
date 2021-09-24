using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TableLoader<TMarshalStruct> : MonoBehaviour
{
    [SerializeField]
    protected string FilePath;

    TableRecordParser<TMarshalStruct> tableRecordParser = new TableRecordParser<TMarshalStruct>();

    public bool Load()
    {
        TextAsset textAsset = Resources.Load<TextAsset>(FilePath);
        if (textAsset == null)
        {
            Debug.LogError("Load Failed! filePath = " + FilePath);
            return false;
        }

        ParseTable(textAsset.text);

        return true;
    }

    void ParseTable(string text)
    {
        StringReader reader = new StringReader(text);   // System.IO.StringReader

        string line = null;
        bool fieldRead = false;
        while ((line = reader.ReadLine()) != null)      // * ���� ���� ������ ��� ���ڵ� �Ľ�
        {
            if (!fieldRead)
            {
                fieldRead = true;
                continue;
            }

            TMarshalStruct data = tableRecordParser.ParseRecordLine(line);
            AddData(data);
        }
    }

    protected virtual void AddData(TMarshalStruct data)
    {

    }

}