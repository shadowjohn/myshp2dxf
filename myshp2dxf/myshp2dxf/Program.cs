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
    internal class Program
    {
        public static myinclude my = new myinclude();
        public static ShapeHelper sh = new ShapeHelper();
        static string usageMessage = @"
This program can transfer shp file to dxf 2000.

myshp2dxf.exe [source shp] -o [target dxf] [options]
options:
    -s_srs EPSG:4326 (Source Coordination)
    -t_srs EPSG:4326 [Output SRS]
    -label [field]
    -h, -H, -? (顯示簡單說明)    
    -shpInfo (列出 Shp 基本資訊)
    -listFields (列出 Shp 欄位清單)
    -limit (要跑幾筆，不填就全部)
    -o [Output DXF file]
";
        static void Main(string[] args)
        {
            string sourceShp = null;
            string targetDxf = null;
            string sourceSrs = "EPSG:4326"; // Default source SRS
            string targetSrs = null;
            string labelField = null;
            long limit = -1;
            bool showHelp = false;
            bool showShpInfo = false;
            bool listFields = false;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].ToLower() == "-s_srs" && i + 1 < args.Length)
                {
                    sourceSrs = args[i + 1];
                    i++; // Skip the next argument as it has been processed as the value of -s_srs.
                }
                else if (args[i].ToLower() == "-t_srs" && i + 1 < args.Length)
                {
                    targetSrs = args[i + 1];
                    i++; // Skip the next argument as it has been processed as the value of -t_srs.
                }
                else if (args[i].ToLower() == "-o" && i + 1 < args.Length)
                {
                    targetDxf = args[i + 1];
                    i++; // Skip the next argument as it has been processed as the value of -o.
                }
                else if (args[i].ToLower() == "-label" && i + 1 < args.Length)
                {
                    labelField = args[i + 1];
                    i++; // Skip the next argument as it has been processed as the value of -label.
                }
                else if (args[i].ToLower() == "-limit" && i + 1 < args.Length)
                {
                    limit = Convert.ToInt64(args[i + 1]);
                    i++; // Skip the next argument as it has been processed as the value of -label.
                }
                else if (my.in_array(args[i].ToLower(), new List<string>() { "-h", "-?" }))
                {
                    showHelp = true;
                }
                else if (args[i].ToLower() == "-shpinfo")
                {
                    showShpInfo = true;
                }
                else if (args[i].ToLower() == "-listfields")
                {
                    listFields = true;
                }
                else
                {
                    sourceShp = args[i];
                }
            }
            if (args.Length == 0)
            {
                showHelp = true;
            }
            // Print the values read from the command line
            Console.WriteLine("Source SHP: " + sourceShp);
            Console.WriteLine("Target DXF: " + targetDxf);
            Console.WriteLine("Source SRS: " + sourceSrs);
            Console.WriteLine("Target SRS: " + targetSrs);
            Console.WriteLine("Label Field: " + labelField);
            Console.WriteLine("Show Help: " + showHelp);
            Console.WriteLine("List Fields: " + listFields);
            Console.WriteLine("Limit: " + limit.ToString());

            // Perform the SHP to DXF conversion and other processing based on the provided options.
            if (showHelp)
            {
                Console.WriteLine(usageMessage);
                return;
            }

            if (showShpInfo)
            {
                var ra = sh.readShpInfo(sourceShp, sourceSrs, targetSrs);
                Console.WriteLine(my.json_format_utf8(my.json_encode(ra)));
                return;
            }

            var rb = sh.readShp(sourceShp, sourceSrs, targetSrs, limit);
            sh.WriteDXF(targetDxf, (List<Feature>)rb["LIST_FEATURES"]);
            //Console.WriteLine(my.json_format_utf8(my.json_encode(rb)));
            return;



        }
    }
}
