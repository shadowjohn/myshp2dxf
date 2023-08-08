using netDxf.Tables;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTopologySuite.IO.Esri;
using utility;
using utility_ShapeHelper;
using NetTopologySuite.IO.Esri.Shp;
using NetTopologySuite.Features;

namespace myshp2dxf
{
    public class settingEntity
    {
        public string sourceShp;
        public string targetDxf;
        public string sourceSrs;
        public string targetSrs;
        public string labelField;
        public long limits;
        public bool showHelp;
        public bool showShpInfo;
        public bool listFields;
        public bool listData;
        public string encoding;
        public Dictionary<string, int> dxf_line_color; //RGB
        public Dictionary<string, int> dxf_text_color; //RGB
    };
    internal class Program
    {

        public static myinclude my = new myinclude();
        public static ShapeHelper sh = new ShapeHelper();
        static string usageMessage = @"
This program can transfer shp file to dxf 2000.

myshp2dxf.exe [source shp] -o [target dxf] [options]
options:
    -s_srs EPSG:4326 [Source SRS，如果有 .prj 檔可不填]
    -t_srs EPSG:4326 [Output SRS]
    -label [在 dxf 時要顯示的欄位，可用 {SPACE} 或 {-} 或 {\t} 或 {,} 或 {\n} 分格]
        例如 -label ""AA01{SPACE}AA02{\n}AA09""
    -h 或 -? (顯示此說明)    
    -shpInfo (列出 Shp 基本資訊)
    -listFields (列出 Shp 欄位清單)
    -listData (列出資料)
    -limits (要跑幾筆，不填就全部)
    -encoding [UTF-8、BIG5 default: UTF-8]
    -dxf_line_color [#0000FF 或 RGB(0,0,255) default: #0000FF]
    -dxf_text_color [#0000FF 或 RGB(0,0,255) default: #0000FF]
    -o [Output DXF file]
";


        static List<string> parseLabel(string labelField)
        {
            List<string> mlabel = new List<string>();
            if (labelField == null)
            {
                return mlabel;
            }
            var m = my.explode("{", labelField);
            int step = 0;
            foreach (string k in m)
            {
                step++;
                if (step == 1)
                {
                    mlabel.Add(k);
                }
                else
                {
                    var md = my.explode("}", k);
                    if (md.Count() >= 2)
                    {
                        mlabel.Add(md[0]);
                        mlabel.Add(md[1]);
                    }
                    else
                    {
                        mlabel.Add(md[0]);
                    }
                }
            }
            return mlabel;
        }

        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            settingEntity setting = new settingEntity();
            setting.sourceShp = null;
            setting.targetDxf = null;
            setting.sourceSrs = "EPSG:4326"; // Default source SRS
            setting.targetSrs = null;
            setting.labelField = null;
            setting.limits = -1;
            setting.showHelp = false;
            setting.showShpInfo = false;
            setting.listFields = false;
            setting.listData = false;
            setting.encoding = "950";
            setting.dxf_line_color = new Dictionary<string, int>() { { "R", 0 }, { "G", 0 }, { "B", 255 } };
            setting.dxf_text_color = new Dictionary<string, int>() { { "R", 0 }, { "G", 0 }, { "B", 255 } };

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].ToLower() == "-s_srs" && i + 1 < args.Length)
                {
                    setting.sourceSrs = args[i + 1];
                    i++; // Skip the next argument as it has been processed as the value of -s_srs.
                }
                else if (args[i].ToLower() == "-t_srs" && i + 1 < args.Length)
                {
                    setting.targetSrs = args[i + 1];
                    i++; // Skip the next argument as it has been processed as the value of -t_srs.
                }
                else if (args[i].ToLower() == "-o" && i + 1 < args.Length)
                {
                    setting.targetDxf = args[i + 1];
                    i++; // Skip the next argument as it has been processed as the value of -o.
                }
                else if (args[i].ToLower() == "-label" && i + 1 < args.Length)
                {
                    setting.labelField = args[i + 1];
                    i++; // Skip the next argument as it has been processed as the value of -label.
                }
                else if (args[i].ToLower() == "-limits" && i + 1 < args.Length)
                {
                    setting.limits = Convert.ToInt64(args[i + 1]);
                    i++; // Skip the next argument as it has been processed as the value of -label.
                }
                else if (args[i].ToLower() == "-encoding" && i + 1 < args.Length)
                {
                    setting.encoding = args[i + 1];
                    switch (setting.encoding.ToLower())
                    {
                        case "65001":
                        case "utf8":
                        case "utf-8":
                            setting.encoding = "65001";
                            break;
                        case "cp950":
                        case "950":
                        case "big5":
                        case "big-5":
                            setting.encoding = "950";
                            break;
                    }
                    i++; // Skip the next argument as it has been processed as the value of -label.
                }
                else if (args[i].ToLower() == "-dxf_line_color" && i + 1 < args.Length)
                {
                    setting.dxf_line_color = new Dictionary<string, int>();
                    string _rgbhex = args[i + 1];
                    try
                    {
                        setting.dxf_line_color = my.ParseRgbHex(_rgbhex);
                    }
                    catch
                    {
                        Console.WriteLine("線條顏色異常，使用預設的藍色 #0000FF");
                        setting.dxf_line_color = new Dictionary<string, int>() { { "R", 0 }, { "G", 0 }, { "B", 255 } };
                    }
                    i++; // Skip the next argument as it has been processed as the value of -label.
                }
                else if (args[i].ToLower() == "-dxf_text_color" && i + 1 < args.Length)
                {
                    setting.dxf_text_color = new Dictionary<string, int>();
                    string _rgbhex = args[i + 1];                    
                    try
                    {
                        setting.dxf_text_color = my.ParseRgbHex(_rgbhex);
                    }
                    catch
                    {
                        Console.WriteLine("文字顏色異常，使用預設的藍色 #0000FF");
                        setting.dxf_text_color = new Dictionary<string, int>() { { "R", 0 }, { "G", 0 }, { "B", 255 } };
                    }
                    i++; // Skip the next argument as it has been processed as the value of -label.
                }
                else if (my.in_array(args[i].ToLower(), new List<string>() { "-help", "--help", "-h", "--h", "-?", "--?" }))
                {
                    setting.showHelp = true;
                }
                else if (args[i].ToLower() == "-shpinfo")
                {
                    setting.showShpInfo = true;
                }
                else if (args[i].ToLower() == "-listfields")
                {
                    setting.listFields = true;
                }
                else if (args[i].ToLower() == "-listdata")
                {
                    setting.listData = true;
                }
                else
                {
                    setting.sourceShp = args[i];
                }
            }
            if (args.Length == 0)
            {
                setting.showHelp = true;
            }
            // Print the values read from the command line
            Console.WriteLine("Source SHP: " + setting.sourceShp);
            Console.WriteLine("Target DXF: " + setting.targetDxf);
            Console.WriteLine("Source SRS: " + setting.sourceSrs);
            Console.WriteLine("Target SRS: " + setting.targetSrs);
            Console.WriteLine("Label Field: " + setting.labelField);
            Console.WriteLine("Show Help: " + setting.showHelp);
            Console.WriteLine("List Fields: " + setting.listFields);
            Console.WriteLine("Limits: " + setting.limits.ToString());
            Console.WriteLine("Encoding: " + setting.encoding);
            Console.WriteLine("dxf_line_color: " + my.json_encode(setting.dxf_line_color));
            Console.WriteLine("dxf_text_color: " + my.json_encode(setting.dxf_text_color));

            // Perform the SHP to DXF conversion and other processing based on the provided options.
            if (setting.showHelp)
            {
                //顯示說明
                Console.WriteLine(usageMessage);
                return;
            }

            if (setting.showShpInfo)
            {
                //顯示 shp 基本資訊
                var ra = sh.readShpInfo(setting.sourceShp, setting.sourceSrs, setting.targetSrs, setting.encoding);
                Console.WriteLine(my.json_format_utf8(my.json_encode(ra)));
                return;
            }
            if (setting.listFields)
            {
                //顯示 shp 欄位
                var ra = sh.readShpInfo(setting.sourceShp, setting.sourceSrs, setting.targetSrs, setting.encoding);
                Console.WriteLine(my.json_format_utf8(my.json_encode(ra["FIELDS"])));
                return;
            }
            if (setting.listData)
            {
                //列資料用的
                var ra = sh.readShpData(setting.sourceShp, setting.sourceSrs, setting.targetSrs, setting.limits, setting.encoding);
                if (ra["STATUS"].ToString() == "OK")
                {
                    string data = my.json_format_utf8(my.json_encode(ra["DATA"]));

                    if (setting.targetDxf != null)
                    {
                        my.file_put_contents(setting.targetDxf, data);
                        Console.WriteLine("Done..." + setting.targetDxf);
                    }
                    else
                    {
                        Console.WriteLine(data);
                    }
                    return;
                }
                else
                {
                    Console.WriteLine(my.json_format_utf8(my.json_encode(ra)));
                    return;
                }
            }

            var rb = sh.readShp(setting.sourceShp, setting.sourceSrs, setting.targetSrs, setting.limits, setting.encoding);
            var mLabel = parseLabel(setting.labelField);
            //Console.WriteLine(my.json_encode(mLabel));
            //return;
            //Console.WriteLine(my.json_format_utf8(my.json_encode(rb["LIST_ATTRIBUTES"])));
            //return;
            if (rb["STATUS"].ToString() == "NO")
            {
                Console.WriteLine(my.json_encode(rb));
                return;
            }
            sh.WriteDXF(setting.targetDxf, rb, mLabel, setting);
            //Console.WriteLine(my.json_format_utf8(my.json_encode(rb)));
            Console.WriteLine("Done..." + setting.targetDxf);
            return;
        }
    }
}
