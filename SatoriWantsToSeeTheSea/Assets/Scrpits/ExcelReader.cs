using System.Collections;
using System.Collections.Generic;
using System.IO;
using ExcelDataReader;
using System.Text;
using UnityEngine;

public struct CharaData
{
    public string CharaNum;
    public string Chara1;
    public string Chara1Site;
    public string Chara2;
    public string Chara2Site;
    public string Chara3;
    public string Chara3Site;
}

public struct ChoiceData
{
    public string ChoiceNum;
    public string ChioceTime;
    public string CT0;
    public string Choice1;
    public string CT1;
    public string Choice2;
    public string CT2;
    public string Choice3;
    public string CT3;
    public string Choice4;
    public string CT4;
    public string Read;
}

public struct TrustData
{
    public string RIN;
    public string REK;
}


public class ExcelReader
{
    public struct ExcelData
    {
        public string Name;
        public string Content;
        public string BGM;
        public string BGI;

        public CharaData CD;
        public ChoiceData ChD;
        public TrustData TD;
        
        public string Effect;

        
    }
    public static List<ExcelData> ReadExcel(string filepath)
    {
        List<ExcelData> ed = new List<ExcelData>();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);//¼æÈÝ¹ú±êµÈ

        using (var stream = File.Open(filepath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                do
                {
                    while (reader.Read())
                    {
                        ExcelData data = new ExcelData();
                        data.Name = reader.IsDBNull(0) ? string.Empty : reader.GetValue(0)?.ToString();
                        data.Content = reader.IsDBNull(1) ? string.Empty : reader.GetValue(1)?.ToString();
                        data.BGM = reader.IsDBNull(2) ? string.Empty : reader.GetValue(2)?.ToString();
                        data.BGI = reader.IsDBNull(3) ? string.Empty : reader.GetValue(3)?.ToString();
                        data.CD.CharaNum = reader.IsDBNull(4) ? string.Empty : reader.GetValue(4)?.ToString();
                        data.CD.Chara1 = reader.IsDBNull(5) ? string.Empty : reader.GetValue(5)?.ToString();
                        data.CD.Chara1Site = reader.IsDBNull(6) ? string.Empty : reader.GetValue(6)?.ToString();
                        data.CD.Chara2 = reader.IsDBNull(7) ? string.Empty : reader.GetValue(7)?.ToString();
                        data.CD.Chara2Site = reader.IsDBNull(8) ? string.Empty : reader.GetValue(8)?.ToString();
                        data.CD.Chara3 = reader.IsDBNull(9) ? string.Empty : reader.GetValue(9)?.ToString();
                        data.CD.Chara3Site = reader.IsDBNull(10) ? string.Empty : reader.GetValue(10)?.ToString();
                        data.Effect = reader.IsDBNull(11) ? string.Empty : reader.GetValue(11)?.ToString();
                        data.ChD.ChoiceNum = reader.IsDBNull(12) ? string.Empty : reader.GetValue(12)?.ToString();
                        data.ChD.ChioceTime = reader.IsDBNull(13) ? string.Empty : reader.GetValue(13)?.ToString();
                        data.ChD.CT0 = reader.IsDBNull(14) ? string.Empty : reader.GetValue(14)?.ToString();
                        data.ChD.Choice1 = reader.IsDBNull(15) ? string.Empty : reader.GetValue(15)?.ToString();
                        data.ChD.CT1 = reader.IsDBNull(16) ? string.Empty : reader.GetValue(16)?.ToString();
                        data.ChD.Choice2 = reader.IsDBNull(17) ? string.Empty : reader.GetValue(17)?.ToString();
                        data.ChD.CT2 = reader.IsDBNull(18) ? string.Empty : reader.GetValue(18)?.ToString();
                        data.ChD.Choice3 = reader.IsDBNull(19) ? string.Empty : reader.GetValue(19)?.ToString();
                        data.ChD.CT3 = reader.IsDBNull(20) ? string.Empty : reader.GetValue(20)?.ToString();
                        data.ChD.Choice4 = reader.IsDBNull(21) ? string.Empty : reader.GetValue(21)?.ToString();
                        data.ChD.CT4 = reader.IsDBNull(22) ? string.Empty : reader.GetValue(22)?.ToString();
                        data.ChD.Read = reader.IsDBNull(23) ? string.Empty : reader.GetValue(23)?.ToString();
                        //data.TD.RIN = reader.IsDBNull(24) ? string.Empty : reader.GetValue(24)?.ToString();
                        //data.TD.REK = reader.IsDBNull(25) ? string.Empty : reader.GetValue(25)?.ToString();

                        ed.Add(data);
                    }
                } while (reader.NextResult());
            }
        }
        return ed;
    }
}
