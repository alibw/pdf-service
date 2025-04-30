using Stimulsoft.Report;
using Stimulsoft.Report.Export;

namespace PdfService;

public static class Reporter
{
  public static Stream Print(string reportTemplate, object data)
  {
    var report = new StiReport();
    
    report.LoadFromString(reportTemplate); 
    report.RegBusinessObject("Data", data);
    report.Render(false);
    
    var stream = new MemoryStream();
    report.ExportDocument(StiExportFormat.Pdf ,stream);
    stream.Position = 0;
    return stream;
  }
}