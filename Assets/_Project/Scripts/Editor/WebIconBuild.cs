using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
namespace HollowSeven.Editor {
public sealed class WebIconBuild : IPostprocessBuildWithReport {
    public int callbackOrder => 1000;
    public void OnPostprocessBuild(BuildReport report){
        if(report.summary.platform==BuildTarget.WebGL)ApplyTo(report.summary.outputPath);
    }
    public static void ApplyTo(string output){
        string index=Path.Combine(output,"index.html");
        if(!File.Exists(index))return;
        string folder=Path.Combine(output,"TemplateData");Directory.CreateDirectory(folder);
        File.Copy("Assets/_Project/Icon/cursed_table_icon_1024.png",Path.Combine(folder,"cursed-table-icon.png"),true);
        string html=File.ReadAllText(index);
        if(html.Contains("TemplateData/favicon.ico"))html=html.Replace("TemplateData/favicon.ico","TemplateData/cursed-table-icon.png");
        else if(!html.Contains("TemplateData/cursed-table-icon.png"))html=html.Replace("</head>","<link rel=\"icon\" type=\"image/png\" href=\"TemplateData/cursed-table-icon.png\">\n</head>");
        File.WriteAllText(index,html);
    }
}
}
