using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SSRSUtility
{
    public class Report
    {
        private XmlDocument d = new XmlDocument();
        
        private XmlElement Document;
        private String ApxFirmDataSourceName = "APXFirm";
        private String XmlCodeModule = "System.Xml, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

        private string ReportNamespaceURI = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition";
        private string ReportDesignerNamespaceURI = "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner";
        public Report()
        {
            d.Schemas.Add(ReportNamespaceURI, "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition/ReportDefinition.xsd");
            d.CreateXmlDeclaration("1.0", "UTF-8", null);

            Document = (XmlElement)d.AppendChild(CreateElement("Report"));
            Document.Attributes.Append(CreateAttribute("xmlns:rd"));
            Document.Attributes["xmlns:rd"].Value = ReportDesignerNamespaceURI;

            SetPageLayout(11m, 8.5m, .25m, .25m, .37m, .36m);
            ReportCode();

            XmlElement DataSources = GetChildElement(Document, "DataSources");
            XmlNode DataSource = DataSources.AppendChild(CreateElement("DataSource"));
            DataSource.AppendChild(CreateElement("DataSourceReference")).InnerText = ApxFirmDataSourceName;
            XmlAttribute ApxFirmName = DataSource.Attributes.Append(CreateAttribute("Name"));
            ApxFirmName.Value = ApxFirmDataSourceName;

            CreateReportParameter("SessionGuid", "String", true, true, true);
            XmlElement CodeModule = GetChildElement(Document, "CodeModules");
            GetChildElement(CodeModule, "CodeModule").InnerText = XmlCodeModule;
        }
        public void CreateTablixColumn(XmlElement tablix, decimal widthInches)
        {
            XmlElement tablixBody = GetChildElement(tablix, "TablixBody");
            XmlElement tablixColumns = GetChildElement(tablixBody, "TablixColumns");
            XmlElement tablixColumn = AppendElement(tablixColumns, "TablixColumn");
            
            XmlElement columnWidth = GetChildElement(tablixColumn, "Width");
            columnWidth.InnerText = widthInches.ToString() + "in";

            XmlElement columnHierarchy = GetChildElement(tablix, "TablixColumnHierarchy");
            XmlElement tablixMembers = GetChildElement(columnHierarchy, "TablixMembers");
            AppendElement(tablixMembers, "TablixMember");
        }
        public XmlElement CreateTablixRow(XmlElement tablix, decimal heightInches)
        {
            XmlElement tablixBody = GetChildElement(tablix, "TablixBody");
            XmlElement tablixRows = GetChildElement(tablixBody, "TablixRows");
            XmlElement tablixRow = AppendElement(tablixRows, "TablixRow");

            XmlElement rowWidth = GetChildElement(tablixRow, "Height");
            rowWidth.InnerText = heightInches.ToString() + "in";
            return tablixRow;
        }
        public void CreateRowHierarchy(XmlElement tablix)
        {
            XmlElement rowHierarchy = GetChildElement(tablix, "TablixRowHierarchy");
            XmlElement tablixMembers = GetChildElement(rowHierarchy, "TablixMembers");

            XmlElement tablixMember = GetChildElement(tablixMembers, "TablixMember");
            
            XmlElement portfoliosGroup = GetChildElement(tablixMember, "Group", "Portfolios");
            AppendElement(portfoliosGroup, "DocumentMapLabel", "=Fields!ReportHeading1.Value");
            XmlElement portfolioGroupExpression = GetChildElement(portfoliosGroup, "GroupExpressions");
            AppendElement(portfolioGroupExpression, "GroupExpression", "Fields!PortfolioBaseIDOrder.Value");
            XmlElement pageBreak = GetChildElement(portfoliosGroup, "PageBreak");
            AppendElement(pageBreak, "BreakLocation", "Between");
            XmlElement variables = GetChildElement(portfoliosGroup, "Variables");
            XmlElement variable1 = GetChildElement(variables, "Variable", "Heading1");
            AppendElement(variable1, "Value", "=IIf(Not IsNothing(Fields!ReportHeading2.Value) And Not IsNothing(Fields!ReportHeading3.Value), Fields!ReportHeading1.Value, \"\")");
            XmlElement variable2 = GetChildElement(variables, "Variable", "Heading2");
            AppendElement(variable2, "Value", "=IIf(Not IsNothing(Fields!ReportHeading2.Value) And Not IsNothing(Fields!ReportHeading3.Value), Fields!ReportHeading2.Value, iif((isnothing(Fields!ReportHeading2.Value) and not isnothing(Fields!ReportHeading3.Value)) or (not isnothing(Fields!ReportHeading2.Value) and isnothing(Fields!ReportHeading3.Value)),Fields!ReportHeading1.Value,\"\"))");
            XmlElement variable3 = GetChildElement(variables, "Variable", "Heading3");
            AppendElement(variable3, "Value", "=iif(not isnothing(Fields!ReportHeading2.Value) and not isnothing(Fields!ReportHeading3.Value), Fields!ReportHeading3.Value, iif((isnothing(Fields!ReportHeading2.Value) and not isnothing(Fields!ReportHeading3.Value)),Fields!ReportHeading3.Value, iif((not isnothing(Fields!ReportHeading2.Value) and isnothing(Fields!ReportHeading3.Value)),Fields!ReportHeading2.Value,  Fields!ReportHeading1.Value)))");

            XmlElement sortExpressions = GetChildElement(tablixMember, "SortExpressions");
            XmlElement sortExpression = GetChildElement(sortExpressions, "SortExpression");
            AppendElement(sortExpression, "Value", "=Fields!PortfolioBaseIDOrder.Value");

            XmlElement pTablixMembers = GetChildElement(tablixMember, "TablixMembers");
            XmlElement pTablixMember1 = AppendElement(pTablixMembers, "TablixMember");
            AppendElement(pTablixMember1, "KeepWithGroup", "After");
            AppendElement(pTablixMember1, "RepeatOnNewPage", "true");

            XmlElement pTablixMember2 = AppendElement(pTablixMembers, "TablixMember");
            GetChildElement(pTablixMember2, "Group", "list1_Details_Group");
            XmlElement gTablixMembers = GetChildElement(pTablixMember2, "TablixMembers");
            AppendElement(gTablixMembers, "TablixMember");
            AppendElement(pTablixMember2, "DataElementOutput", "Output");
            AppendElement(pTablixMember2, "KeepTogether", "true");
        }
        public XmlElement CreateReportElement(XmlElement parent, string element, string elementName)
        {
            XmlElement cellContents = parent;
            XmlElement cellItem = GetChildElement(cellContents, element, elementName);
            return cellItem;
        }
        public XmlElement AddTextboxContent(XmlElement parent, string value)
        {
            XmlElement textBox = parent;
            XmlElement keepTogether = AppendElement(textBox, "KeepTogether", "true");
            XmlElement Paragraphs = GetChildElement(textBox, "Paragraphs");
            XmlElement paragraph = GetChildElement(Paragraphs, "Paragraph");
            XmlElement TextRuns = GetChildElement(paragraph, "TextRuns");
            XmlElement textRun = GetChildElement(TextRuns, "TextRun");
            XmlElement Value = AppendElement(textRun, "Value", value);
            XmlElement Style = GetChildElement(textRun, "Style");
            XmlElement color = AppendElement(Style, "Color", "White");
            return Value;
        }
        public void CreateParentReport(string tablixName, string datasetName, string subReportName)
        {
            XmlElement Body = GetChildElement(Document, "Body");
            XmlElement ReportItems = GetChildElement(Body, "ReportItems");
            XmlElement Tablix = GetChildElement(ReportItems, "Tablix", tablixName);
            CreateTablixColumn(Tablix, 1.50079m);
            CreateTablixColumn(Tablix, 1.50079m);
            CreateTablixColumn(Tablix, 1.50079m);
            CreateTablixColumn(Tablix, 1.50079m);
            CreateTablixColumn(Tablix, 1.50079m);
            CreateTablixColumn(Tablix, 1.50079m);
            CreateTablixColumn(Tablix, 1.49526m);

            XmlElement tablixRow = CreateTablixRow(Tablix, 0.03125m);
            XmlElement tablixCells = GetChildElement(tablixRow, "TablixCells");
            
            XmlElement tablixCell1 = GetChildElement(tablixCells, "TablixCell");
            XmlElement tablixCell2 = AppendElement(tablixCells, "TablixCell");
            XmlElement tablixCell3 = AppendElement(tablixCells, "TablixCell");
            XmlElement tablixCell4 = AppendElement(tablixCells, "TablixCell");
            XmlElement tablixCell5 = AppendElement(tablixCells, "TablixCell");
            XmlElement tablixCell6 = AppendElement(tablixCells, "TablixCell");
            XmlElement tablixCell7 = AppendElement(tablixCells, "TablixCell");
            
            XmlElement cellContents1 = GetChildElement(tablixCell1, "CellContents");
            XmlElement cellContents2 = GetChildElement(tablixCell2, "CellContents");
            XmlElement cellContents3 = GetChildElement(tablixCell3, "CellContents");
            XmlElement cellContents4 = GetChildElement(tablixCell4, "CellContents");
            XmlElement cellContents5 = GetChildElement(tablixCell5, "CellContents");
            XmlElement cellContents6 = GetChildElement(tablixCell6, "CellContents");
            XmlElement cellContents7 = GetChildElement(tablixCell7, "CellContents");

            XmlElement reportHeading1 = CreateReportElement(cellContents1, "Textbox", "ReportHeading1");
            AddTextboxContent(reportHeading1, "=Variables!Heading1.Value");

            XmlElement reportHeading2 = CreateReportElement(cellContents2, "Textbox", "ReportHeading2");
            AddTextboxContent(reportHeading2, "=Variables!Heading2.Value");

            XmlElement reportHeading3 = CreateReportElement(cellContents3, "Textbox", "ReportHeading3");
            AddTextboxContent(reportHeading3, "=Variables!Heading3.Value");

            XmlElement reportingCurrency = CreateReportElement(cellContents4, "Textbox", "ReportingCurrency");
            AddTextboxContent(reportingCurrency, "=iif(Parameters!FeeMethod.Value = 1, \"Net of \", \"Gross of \") + iif(CBool(Parameters!AccruePerfFees.Value) and Parameters!FeeMethod.Value = 1,\"Accrued \",nothing) + \"Fees | \"+ Fields!ReportingCurrencyName.Value");

            XmlElement reportDate = CreateReportElement(cellContents5, "Textbox", "ReportDate");
            AddTextboxContent(reportDate, "=Convert.ToDateTime(Fields!FromDate.Value).ToString(\"d\", Globalization.CultureInfo.GetCultureInfo(Fields!LocaleID.Value)) + \" - \" + Convert.ToDateTime(Fields!ThruDate.Value).ToString(\"d\", Globalization.CultureInfo.GetCultureInfo(Fields!LocaleID.Value))");

            XmlElement firmLogo = CreateReportElement(cellContents6, "Textbox", "FirmLogo");
            AddTextboxContent(firmLogo, "=Fields!FirmLogo.Value");

            XmlElement settledTrades = CreateReportElement(cellContents7, "Textbox", "SettledTrades");
            AddTextboxContent(settledTrades, "=iif(Fields!UseSettlementDate.Value, \" - Settled Trades\", nothing)");

            XmlElement tablixRow2 = CreateTablixRow(Tablix, 6.63875m);
            XmlElement tablixCells2 = GetChildElement(tablixRow2, "TablixCells");

            //  Sub-report
            XmlElement tablixCell8 = AppendElement(tablixCells2, "TablixCell");
            XmlElement subReportContent = GetChildElement(tablixCell8, "CellContents");
            XmlElement subReport = CreateReportElement(subReportContent, "Subreport", "Subreport1");
            
            XmlElement reportName = AppendElement(subReport, "ReportName", subReportName);
            XmlElement Parameters = GetChildElement(subReport, "Parameters");
            XmlElement parameter1 = GetChildElement(Parameters, "Parameter", "SessionGuid");
            AppendElement(parameter1, "Value", "=Parameters!SessionGuid.Value");
            XmlElement parameter2 = GetChildElement(Parameters, "Parameter", "PortfolioBaseID");
            AppendElement(parameter2, "Value", "=Fields!PortfolioBaseID.Value");
            XmlElement parameter3 = GetChildElement(Parameters, "Parameter", "PortfolioBaseIDOrder");
            AppendElement(parameter3, "Value", "=Fields!PortfolioBaseIDOrder.Value");
            XmlElement parameter4 = GetChildElement(Parameters, "Parameter", "DataHandle");
            AppendElement(parameter4, "Value", "=Fields!DataHandle.Value");
            XmlElement parameter5 = GetChildElement(Parameters, "Parameter", "FromDate");
            AppendElement(parameter5, "Value", "=Parameters!FromDate.Value");
            XmlElement parameter6 = GetChildElement(Parameters, "Parameter", "ToDate");
            AppendElement(parameter6, "Value", "=Parameters!ToDate.Value");
            XmlElement parameter7 = GetChildElement(Parameters, "Parameter", "ReportingCurrencyCode");
            AppendElement(parameter7, "Value", "=Fields!ReportingCurrencyCode.Value");

            AppendElement(subReport, "NoRowsMessage", "No Data Available");

            XmlElement subReportStyle= GetChildElement(subReport, "Style");
            XmlElement Border = GetChildElement(subReportStyle, "Border");
            AppendElement(Border, "Style", "None");
            AppendElement(subReportContent, "ColSpan", "7");

            AppendElement(tablixCells2, "TablixCell");
            AppendElement(tablixCells2, "TablixCell");
            AppendElement(tablixCells2, "TablixCell");
            AppendElement(tablixCells2, "TablixCell");
            AppendElement(tablixCells2, "TablixCell");
            AppendElement(tablixCells2, "TablixCell");

            CreateRowHierarchy(Tablix);
            AppendElement(Tablix, "NoRowsMessage", "No Portfolios to Display");
            AppendElement(Tablix, "DataSetName", datasetName);
            AppendElement(Tablix, "Height", "6.67in");
            AppendElement(Tablix, "Width", "10.5in");

            XmlElement TablixStyle = GetChildElement(Tablix, "Style");
            AppendElement(TablixStyle, "FontStyle", "=Code.StyleSheetValue(\"DetailStyle\")");
            AppendElement(TablixStyle, "FontFamily", "=Code.StyleSheetValue(\"DetailFamily\")");
            AppendElement(TablixStyle, "FontWeight", "=Code.StyleSheetValue(\"DetailWeight\")");
            AppendElement(TablixStyle, "TextDecoration", "=Code.StyleSheetValue(\"DetailDecoration\")");
            AppendElement(TablixStyle, "Color", "=Code.StyleSheetValue(\"DetailColor\")");

            SetHeaderFooter();

        }
        public void SetPageLayout(decimal heightInches, decimal widthInches, decimal LeftmarginInches, decimal RightmarginInches, decimal TopmarginInches, decimal BottommarginInches)
        {
            XmlElement Page = GetChildElement(Document, "Page");
            XmlElement Body = GetChildElement(Document, "Body");
            XmlElement Width = GetChildElement(Document, "Width");
            GetChildElement(Body, "Height").InnerText = "6.67in";
            GetChildElement(Page, "PageWidth").InnerText = widthInches.ToString() + "in";
            GetChildElement(Page, "PageHeight").InnerText = heightInches.ToString() + "in";
            GetChildElement(Page, "InteractiveHeight").InnerText = "0in";
            GetChildElement(Page, "InteractiveWidth").InnerText = "0in";

            decimal BodyWidth = widthInches - LeftmarginInches - RightmarginInches;
            Width.InnerText = BodyWidth.ToString() + "in";

            GetChildElement(Page, "TopMargin").InnerText = TopmarginInches.ToString() + "in";
            GetChildElement(Page, "BottomMargin").InnerText = BottommarginInches.ToString() + "in";
            GetChildElement(Page, "LeftMargin").InnerText = LeftmarginInches.ToString() + "in";
            GetChildElement(Page, "RightMargin").InnerText = RightmarginInches.ToString() + "in";
        }
        public void SetHeaderFooter()
        {
            XmlElement Page = GetChildElement(Document, "Page");
            XmlElement pageHeader = GetChildElement(Page, "PageHeader");
            XmlElement pageFooter = GetChildElement(Page, "PageFooter");

            //  Page Header
            AppendElement(pageHeader, "Height", "0.8in");
            AppendElement(pageHeader, "PrintOnFirstPage", "true");
            AppendElement(pageHeader, "PrintOnLastPage", "true");
            XmlElement ReportItems = GetChildElement(pageHeader, "ReportItems");
            XmlElement rectangle = GetChildElement(ReportItems, "Rectangle", "Rectangle6");
            XmlElement reportItems = GetChildElement(rectangle, "ReportItems");

            XmlElement firmLogo = GetChildElement(reportItems, "Image", "FirmLogoImage");
            AppendElement(firmLogo, "Source", "External");
            AppendElement(firmLogo, "Value", "=Code.GetFirmLogo(ReportItems!FirmLogo.Value)");
            AppendElement(firmLogo, "Sizing", "FitProportional");
            AppendElement(firmLogo, "Height", "0.7in");
            AppendElement(firmLogo, "Width", "1.25in");
            XmlElement firmLogoStyle = GetChildElement(firmLogo, "Style");
            XmlElement firmLogoBorder = GetChildElement(firmLogoStyle, "Border");
            AppendElement(firmLogoBorder, "Style", "None");

            XmlElement verticalLine = GetChildElement(reportItems, "Image", "HeaderVerticalLine");
            AppendElement(verticalLine, "Source", "External");
            AppendElement(verticalLine, "Value", "HeaderVertLine.jpg");
            AppendElement(verticalLine, "Sizing", "FitProportional");
            AppendElement(verticalLine, "Left", "1.25556in");
            AppendElement(verticalLine, "Height", "0.746in");
            AppendElement(verticalLine, "Width", "0.1in");
            AppendElement(verticalLine, "ZIndex", "1");
            XmlElement verticalLineStyle = GetChildElement(verticalLine, "Style");
            XmlElement verticalLineBorder = GetChildElement(verticalLineStyle, "Border");
            AppendElement(verticalLineBorder, "Style", "None");

            XmlElement txtReportHeading1 = GetChildElement(reportItems, "Textbox", "txtReportHeading1");
            AppendElement(txtReportHeading1, "KeepTogether", "true");
            XmlElement Paragraphs1 = GetChildElement(txtReportHeading1, "Paragraphs");
            XmlElement paragraph1 = GetChildElement(Paragraphs1, "Paragraph");
            XmlElement Style1 = GetChildElement(paragraph1, "Style");
            AppendElement(Style1, "TextAlign", "Left");
            XmlElement textRuns1 = GetChildElement(paragraph1, "TextRuns");
            XmlElement textRun1 = GetChildElement(textRuns1, "TextRun");
            AppendElement(textRun1, "Value", "=Code.GetReportHeading1(ReportItems!ReportHeading1.Value)");
            XmlElement style1 = GetChildElement(textRun1, "Style");
            AppendElement(style1, "FontStyle", "=Code.StyleSheetValue(\"PortfolioNameStyle\")");
            AppendElement(style1, "FontFamily", "=Code.StyleSheetValue(\"PortfolioNameFamily\")");
            AppendElement(style1, "FontWeight", "=Code.StyleSheetValue(\"PortfolioNameWeight\")");
            AppendElement(style1, "TextDecoration", "=Code.StyleSheetValue(\"PortfolioNameDecoration\")");
            AppendElement(style1, "Color", "=Code.StyleSheetValue(\"PortfolioNameColor\")");

            AppendElement(txtReportHeading1, "Top", "0.24in");
            AppendElement(txtReportHeading1, "Left", "1.36945in");
            AppendElement(txtReportHeading1, "Height", "0.16in");
            AppendElement(txtReportHeading1, "Width", "4in");
            AppendElement(txtReportHeading1, "ZIndex", "2");
            XmlElement txtReportHeading1Style = GetChildElement(txtReportHeading1, "Style");
            XmlElement txtReportHeading1Border = GetChildElement(txtReportHeading1Style, "Border");
            AppendElement(txtReportHeading1Border, "Style", "None");
            AppendElement(txtReportHeading1Style, "BackgroundColor", "=Code.StyleSheetValue(\"PortfolioNameBackgroundColor\")");
            AppendElement(txtReportHeading1Style, "VerticalAlign", "Bottom");

            XmlElement txtReportHeading2 = GetChildElement(reportItems, "Textbox", "txtReportHeading2");
            AppendElement(txtReportHeading2, "KeepTogether", "true");
            XmlElement Paragraphs2 = GetChildElement(txtReportHeading2, "Paragraphs");
            XmlElement paragraph2 = GetChildElement(Paragraphs2, "Paragraph");
            XmlElement Style2 = GetChildElement(paragraph2, "Style");
            AppendElement(Style2, "TextAlign", "Left");
            XmlElement textRuns2 = GetChildElement(paragraph2, "TextRuns");
            XmlElement textRun2 = GetChildElement(textRuns2, "TextRun");
            AppendElement(textRun2, "Value", "=Code.GetReportHeading2(ReportItems!ReportHeading2.Value)");
            XmlElement style2 = GetChildElement(textRun2, "Style");
            AppendElement(style2, "FontStyle", "=Code.StyleSheetValue(\"PortfolioNameStyle\")");
            AppendElement(style2, "FontFamily", "=Code.StyleSheetValue(\"PortfolioNameFamily\")");
            AppendElement(style2, "FontWeight", "=Code.StyleSheetValue(\"PortfolioNameWeight\")");
            AppendElement(style2, "TextDecoration", "=Code.StyleSheetValue(\"PortfolioNameDecoration\")");
            AppendElement(style2, "Color", "=Code.StyleSheetValue(\"PortfolioNameColor\")");

            AppendElement(txtReportHeading2, "Top", "0.4in");
            AppendElement(txtReportHeading2, "Left", "1.36945in");
            AppendElement(txtReportHeading2, "Height", "0.16in");
            AppendElement(txtReportHeading2, "Width", "4in");
            AppendElement(txtReportHeading2, "ZIndex", "3");
            XmlElement txtReportHeading2Style = GetChildElement(txtReportHeading2, "Style");
            XmlElement txtReportHeading2Border = GetChildElement(txtReportHeading2Style, "Border");
            AppendElement(txtReportHeading2Border, "Style", "None");
            AppendElement(txtReportHeading2Style, "BackgroundColor", "=Code.StyleSheetValue(\"PortfolioNameBackgroundColor\")");
            AppendElement(txtReportHeading2Style, "VerticalAlign", "Bottom");

            XmlElement txtReportHeading3 = GetChildElement(reportItems, "Textbox", "txtReportHeading3");
            AppendElement(txtReportHeading3, "KeepTogether", "true");
            XmlElement Paragraphs3 = GetChildElement(txtReportHeading3, "Paragraphs");
            XmlElement paragraph3 = GetChildElement(Paragraphs3, "Paragraph");
            XmlElement Style3 = GetChildElement(paragraph3, "Style");
            AppendElement(Style3, "TextAlign", "Left");
            XmlElement textRuns3 = GetChildElement(paragraph3, "TextRuns");
            XmlElement textRun3 = GetChildElement(textRuns3, "TextRun");
            AppendElement(textRun3, "Value", "=Code.GetReportHeading3(ReportItems!ReportHeading3.Value)");
            XmlElement style3 = GetChildElement(textRun3, "Style");
            AppendElement(style3, "FontStyle", "=Code.StyleSheetValue(\"PortfolioNameStyle\")");
            AppendElement(style3, "FontFamily", "=Code.StyleSheetValue(\"PortfolioNameFamily\")");
            AppendElement(style3, "FontWeight", "=Code.StyleSheetValue(\"PortfolioNameWeight\")");
            AppendElement(style3, "TextDecoration", "=Code.StyleSheetValue(\"PortfolioNameDecoration\")");
            AppendElement(style3, "Color", "=Code.StyleSheetValue(\"PortfolioNameColor\")");

            AppendElement(txtReportHeading3, "Top", "0.56in");
            AppendElement(txtReportHeading3, "Left", "1.36945in");
            AppendElement(txtReportHeading3, "Height", "0.16in");
            AppendElement(txtReportHeading3, "Width", "4in");
            AppendElement(txtReportHeading3, "ZIndex", "4");
            XmlElement txtReportHeading3Style = GetChildElement(txtReportHeading3, "Style");
            XmlElement txtReportHeading3Border = GetChildElement(txtReportHeading3Style, "Border");
            AppendElement(txtReportHeading3Border, "Style", "None");
            AppendElement(txtReportHeading3Style, "BackgroundColor", "=Code.StyleSheetValue(\"PortfolioNameBackgroundColor\")");
            AppendElement(txtReportHeading3Style, "VerticalAlign", "Bottom");

            XmlElement headerLine = GetChildElement(reportItems, "Line", "line2");
            AppendElement(headerLine, "Top", "0.75in");
            AppendElement(headerLine, "Height", "0in");
            AppendElement(headerLine, "Width", "10.5in");
            AppendElement(headerLine, "ZIndex", "5");
            XmlElement headerLineStyle = GetChildElement(headerLine, "Style");
            XmlElement headerLineBorder = GetChildElement(headerLineStyle, "Border");
            AppendElement(headerLineBorder, "Color", "=Code.StyleSheetValue(\"HeaderLineColor\")");
            AppendElement(headerLineBorder, "Style", "Solid");
            AppendElement(headerLineBorder, "Width", "=Code.StyleSheetValue(\"HeaderLineWidth\")");

            XmlElement reportTitle = GetChildElement(reportItems, "Textbox", "ReportTitle");
            AppendElement(reportTitle, "KeepTogether", "true");
            XmlElement reportTitleParagraphs = GetChildElement(reportTitle, "Paragraphs");
            XmlElement reportTitleParagraph = GetChildElement(reportTitleParagraphs, "Paragraph");
            XmlElement reportParagraphStyle = GetChildElement(reportTitleParagraph, "Style");
            AppendElement(reportParagraphStyle, "TextAlign", "Right");
            XmlElement reportTitleTextRuns = GetChildElement(reportTitleParagraph, "TextRuns");
            XmlElement reportTitleTextRun = GetChildElement(reportTitleTextRuns, "TextRun");
            AppendElement(reportTitleTextRun, "Value", "=Parameters!ReportTitle.Value &amp; Code.GetSettlement(ReportItems!SettledTrades.Value)");
            XmlElement reportTitleTextRunStyle = GetChildElement(reportTitleTextRun, "Style");
            AppendElement(reportTitleTextRunStyle, "FontStyle", "=Code.StyleSheetValue(\"ReportTitleStyle\")");
            AppendElement(reportTitleTextRunStyle, "FontFamily", "=Code.StyleSheetValue(\"ReportTitleFamily\")");
            AppendElement(reportTitleTextRunStyle, "FontWeight", "=Code.StyleSheetValue(\"ReportTitleWeight\")");
            AppendElement(reportTitleTextRunStyle, "TextDecoration", "=Code.StyleSheetValue(\"ReportTitleDecoration\")");
            AppendElement(reportTitleTextRunStyle, "Color", "=Code.StyleSheetValue(\"ReportTitleColor\")");
            AppendElement(reportTitle, "Top", "0.1in");
            AppendElement(reportTitle, "Height", "0.3in");
            AppendElement(reportTitle, "Left", "5.5in");
            AppendElement(reportTitle, "Width", "5in");
            AppendElement(reportTitle, "ZIndex", "6");
            XmlElement reportTitleStyle = GetChildElement(reportTitle, "Style");
            AppendElement(reportTitleStyle, "Border");
            AppendElement(reportTitleStyle, "BackgroundColor", "=Code.StyleSheetValue(\"ReportTitleBackgroundColor\")");
            AppendElement(reportTitleStyle, "VerticalAlign", "Bottom");
            AppendElement(reportTitleStyle, "PaddingRight", "2pt");

            XmlElement txtReportingCurrency = GetChildElement(reportItems, "Textbox", "txtReportingCurrency");
            AppendElement(txtReportingCurrency, "KeepTogether", "true");
            XmlElement reportingCurrencyParagraphs = GetChildElement(txtReportingCurrency, "Paragraphs");
            XmlElement reportingCurrencyParagraph = GetChildElement(reportingCurrencyParagraphs, "Paragraph");
            XmlElement reportingCurrencyParagraphStyle = GetChildElement(reportingCurrencyParagraph, "Style");
            AppendElement(reportingCurrencyParagraphStyle, "TextAlign", "Right");
            XmlElement reportingCurrencyTextRuns = GetChildElement(reportingCurrencyParagraph, "TextRuns");
            XmlElement reportingCurrencyTextRun = GetChildElement(reportingCurrencyTextRuns, "TextRun");
            AppendElement(reportingCurrencyTextRun, "Value", "=Code.GetCurrency(ReportItems!ReportingCurrency.Value)");
            XmlElement reportingCurrencyTextrunStyle = GetChildElement(reportingCurrencyTextRun, "Style");
            AppendElement(reportingCurrencyTextrunStyle, "FontStyle", "=Code.StyleSheetValue(\"ReportDateStyle\")");
            AppendElement(reportingCurrencyTextrunStyle, "FontFamily", "=Code.StyleSheetValue(\"ReportDateFamily\")");
            AppendElement(reportingCurrencyTextrunStyle, "FontWeight", "=Code.StyleSheetValue(\"ReportDateWeight\")");
            AppendElement(reportingCurrencyTextrunStyle, "TextDecoration", "=Code.StyleSheetValue(\"ReportDateDecoration\")");
            AppendElement(reportingCurrencyTextrunStyle, "Color", "=Code.StyleSheetValue(\"ReportDateColor\")");

            AppendElement(txtReportingCurrency, "Top", "0.4in");
            AppendElement(txtReportingCurrency, "Left", "5.5in");
            AppendElement(txtReportingCurrency, "Height", "0.16in");
            AppendElement(txtReportingCurrency, "Width", "5in");
            AppendElement(txtReportingCurrency, "ZIndex", "7");
            XmlElement txtReportingCurrencyStyle = GetChildElement(txtReportingCurrency, "Style");
            AppendElement(txtReportingCurrencyStyle, "Border");
            AppendElement(txtReportingCurrencyStyle, "BackgroundColor", "=Code.StyleSheetValue(\"ReportDateBackgroundColor\")");
            AppendElement(txtReportingCurrencyStyle, "VerticalAlign", "Bottom");
            AppendElement(txtReportingCurrencyStyle, "PaddingRight", "2pt");

            XmlElement txtReportDate = GetChildElement(reportItems, "Textbox", "txtReportDate");
            AppendElement(txtReportDate, "KeepTogether", "true");
            XmlElement reportDateParagraphs = GetChildElement(txtReportDate, "Paragraphs");
            XmlElement reportDateParagraph = GetChildElement(reportDateParagraphs, "Paragraph");
            XmlElement reportDateParagraphStyle = GetChildElement(reportDateParagraph, "Style");
            AppendElement(reportDateParagraphStyle, "TextAlign", "Right");
            XmlElement reportDateTextRuns = GetChildElement(reportDateParagraph, "TextRuns");
            XmlElement reportDateTextRun = GetChildElement(reportDateTextRuns, "TextRun");
            AppendElement(reportDateTextRun, "Value", "=Code.GetCurrency(ReportItems!ReportingCurrency.Value)");
            XmlElement reportDateTextrunStyle = GetChildElement(reportDateTextRun, "Style");
            AppendElement(reportDateTextrunStyle, "FontStyle", "=Code.StyleSheetValue(\"ReportDateStyle\")");
            AppendElement(reportDateTextrunStyle, "FontFamily", "=Code.StyleSheetValue(\"ReportDateFamily\")");
            AppendElement(reportDateTextrunStyle, "FontWeight", "=Code.StyleSheetValue(\"ReportDateWeight\")");
            AppendElement(reportDateTextrunStyle, "TextDecoration", "=Code.StyleSheetValue(\"ReportDateDecoration\")");
            AppendElement(reportDateTextrunStyle, "Color", "=Code.StyleSheetValue(\"ReportDateColor\")");

            AppendElement(txtReportDate, "Top", "0.56in");
            AppendElement(txtReportDate, "Left", "5.5in");
            AppendElement(txtReportDate, "Height", "0.16in");
            AppendElement(txtReportDate, "Width", "5in");
            AppendElement(txtReportDate, "ZIndex", "8");
            XmlElement txtReportDateStyle = GetChildElement(txtReportDate, "Style");
            AppendElement(txtReportDateStyle, "Border");
            AppendElement(txtReportDateStyle, "BackgroundColor", "=Code.StyleSheetValue(\"ReportDateBackgroundColor\")");
            AppendElement(txtReportDateStyle, "VerticalAlign", "Bottom");
            AppendElement(txtReportDateStyle, "PaddingRight", "2pt");

            AppendElement(rectangle, "KeepTogether", "true");
            AppendElement(rectangle, "Height", "0.8in");
            AppendElement(rectangle, "Width", "10.5in");
            XmlElement rectangleStyle = GetChildElement(rectangle, "Style");
            XmlElement rectangleBorder = GetChildElement(rectangleStyle, "Border");
            AppendElement(rectangleBorder, "Style", "None");

            XmlElement pageHeaderStyle = GetChildElement(pageHeader, "Style");
            XmlElement pageHeaderBorder = GetChildElement(pageHeaderStyle, "Border");
            AppendElement(pageHeaderBorder, "Style", "None");

            //  Page Footer
            AppendElement(pageFooter, "Height", "0.3in");
            AppendElement(pageFooter, "PrintOnFirstPage", "true");
            AppendElement(pageFooter, "PrintOnLastPage", "true");
            XmlElement pageFooterReportItems = GetChildElement(pageFooter, "ReportItems");
            XmlElement pageFooterLine = GetChildElement(pageFooterReportItems, "Line", "line1");
            AppendElement(pageFooterLine, "Top", "0.05in");
            AppendElement(pageFooterLine, "Height", "0in");
            AppendElement(pageFooterLine, "Width", "10.5in");
            XmlElement pageFooterLineStyle = GetChildElement(pageFooterLine, "Style");
            XmlElement pageFooterLineBorder = GetChildElement(pageFooterLineStyle, "Border");
            AppendElement(pageFooterLineBorder, "Color", "=Code.StyleSheetValue(\"FooterLineColor\")");
            AppendElement(pageFooterLineBorder, "Style", "Solid");
            AppendElement(pageFooterLineBorder, "Width", "=Code.StyleSheetValue(\"FooterLineWidth\")");

            XmlElement FirmName = GetChildElement(pageFooterReportItems, "Textbox", "FirmName");
            AppendElement(FirmName, "CanGrow", "true");
            AppendElement(FirmName, "KeepTogether", "true");
            XmlElement firmNameParagraphs = GetChildElement(FirmName, "Paragraphs");
            XmlElement firmNameParagraph = GetChildElement(firmNameParagraphs, "Paragraph");
            XmlElement firmNameParagraphStyle = GetChildElement(firmNameParagraph, "Style");
            AppendElement(firmNameParagraphStyle, "TextAlign", "Left");
            XmlElement firmNameTextRuns = GetChildElement(firmNameParagraph, "TextRuns");
            XmlElement firmNameTextRun = GetChildElement(firmNameTextRuns, "TextRun");
            AppendElement(firmNameTextRun, "Value", "=Parameters!FirmName.Value");
            XmlElement firmNameTextrunStyle = GetChildElement(firmNameTextRun, "Style");
            AppendElement(firmNameTextrunStyle, "FontStyle", "=Code.StyleSheetValue(\"FirmNameStyle\")");
            AppendElement(firmNameTextrunStyle, "FontFamily", "=Code.StyleSheetValue(\"FirmNameFamily\")");
            AppendElement(firmNameTextrunStyle, "FontWeight", "=Code.StyleSheetValue(\"FirmNameWeight\")");
            AppendElement(firmNameTextrunStyle, "TextDecoration", "=Code.StyleSheetValue(\"FirmNameDecoration\")");
            AppendElement(firmNameTextrunStyle, "Color", "=Code.StyleSheetValue(\"FirmNameColor\")");

            AppendElement(FirmName, "Top", "0.1in");
            AppendElement(FirmName, "Height", "0.2in");
            AppendElement(FirmName, "Width", "10.5in");
            AppendElement(FirmName, "ZIndex", "1");
            XmlElement FirmNameStyle = GetChildElement(FirmName, "Style");
            AppendElement(FirmNameStyle, "Border");
            AppendElement(FirmNameStyle, "BackgroundColor", "=Code.StyleSheetValue(\"FirmNameBackgroundColor\")");
            AppendElement(FirmNameStyle, "VerticalAlign", "Middle");
            AppendElement(FirmNameStyle, "PaddingLeft", "2pt");
        }
        public void ReportCode()
        {
            string reportcode = "Private cHeading1 as Object";
                reportcode += "\n   Private cHeading2 as Object";
                reportcode += "\n   Private cHeading3 as Object";
                reportcode += "\n   Private cCurrency as Object";
                reportcode += "\n   Private cReportDate as Object";
                reportcode += "\n   Private cSettlement as Object";
                reportcode += "\n   Private cFirmLogo as Object";
                reportcode += "\n   Protected Overrides Sub OnInit() ";
                reportcode += "\n   cHeading1 = nothing";
                reportcode += "\n   cHeading2 = nothing";
                reportcode += "\n   cHeading3 = nothing";
                reportcode += "\n   cCurrency = nothing";
                reportcode += "\n   cReportDate = nothing";
                reportcode += "\n   cSettlement = nothing";
                reportcode += "\n   cFirmLogo = nothing";
                reportcode += "\n   End Sub";
                reportcode += "\n   Public Function GetReportHeading1(ReportHeading1 as Object) as String";
                reportcode += "\n   if ReportHeading1 is nothing";
                reportcode += "\n      return cHeading1 ";
                reportcode += "\n   else ";
                reportcode += "\n      cHeading1 = ReportHeading1";
                reportcode += "\n      return cHeading1";
                reportcode += "\n   end if";
                reportcode += "\n   End Function";
                reportcode += "\n   Public Function GetReportHeading2(ReportHeading2 as Object) as String";
                reportcode += "\n   if ReportHeading2 is nothing";
                reportcode += "\n      return cHeading2 ";
                reportcode += "\n   else ";
                reportcode += "\n      cHeading2 = ReportHeading2";
                reportcode += "\n      return cHeading2";
                reportcode += "\n   end if";
                reportcode += "\n   End Function";
                reportcode += "\n   Public Function GetReportHeading3(ReportHeading3 as Object) as String";
                reportcode += "\n   if ReportHeading3 is nothing";
                reportcode += "\n      return cHeading3 ";
                reportcode += "\n   else ";
                reportcode += "\n      cHeading3 = ReportHeading3";
                reportcode += "\n      return cHeading3";
                reportcode += "\n   end if";
                reportcode += "\n   End Function";
                reportcode += "\n   Public Function GetCurrency(Currency as Object) as String";
                reportcode += "\n   if Currency is nothing";
                reportcode += "\n      return cCurrency";
                reportcode += "\n   else ";
                reportcode += "\n      cCurrency = Currency";
                reportcode += "\n      return cCurrency";
                reportcode += "\n   end if";
                reportcode += "\n   End Function";
                reportcode += "\n   Public Function GetReportDate( ReportDate as Object) as String";
                reportcode += "\n   if ReportDate is nothing";
                reportcode += "\n      return cReportDate";
                reportcode += "\n   else ";
                reportcode += "\n      cReportDate = ReportDate";
                reportcode += "\n      return cReportDate";
                reportcode += "\n   end if";
                reportcode += "\n   End Function";
                reportcode += "\n   Public Function GetSettlement( Settlement as Object) as String";
                reportcode += "\n   if Settlement is nothing";
                reportcode += "\n      return cSettlement";
                reportcode += "\n   else ";
                reportcode += "\n      cSettlement = Settlement ";
                reportcode += "\n      return cSettlement ";
                reportcode += "\n   end if";
                reportcode += "\n   End Function";
                reportcode += "\n   Public Function GetFirmLogo( FirmLogo as Object) as String";
                reportcode += "\n   if FirmLogo is nothing";
                reportcode += "\n      return cFirmLogo";
                reportcode += "\n   else ";
                reportcode += "\n      cFirmLogo = FirmLogo ";
                reportcode += "\n      return cFirmLogo ";
                reportcode += "\n   end if";
                reportcode += "\n   End Function";
                reportcode += "\n   Dim StyleSheetHash As System.Collections.Hashtable";
                reportcode += "\n   Public Function InitStyleSheet()";
                reportcode += "\n   Dim xmlDocument As System.Xml.XmlDocument = New System.Xml.XmlDocument()";
                reportcode += "\n   xmlDocument.LoadXml(Report.Parameters!StyleSheetXML.Value)";
                reportcode += "\n   Dim rootNode As System.Xml.XmlElement = xmlDocument.SelectSingleNode(\"StyleSheetInfo\")";
                reportcode += "\n   StyleSheetHash = New System.Collections.Hashtable()";
                reportcode += "\n   For Each a As System.Xml.XmlAttribute In rootNode.Attributes";
                reportcode += "\n   StyleSheetHash.Add(a.Name, a.Value)";
                reportcode += "\n   Next";
                reportcode += "\n   End Function";
                reportcode += "\n   Public Function StyleSheetValue(StyleSheetValueName As String) As String";
                reportcode += "\n   If (styleSheetHash Is Nothing)";
                reportcode += "\n   InitStyleSheet()";
                reportcode += "\n   End If";
                reportcode += "\n   If (StyleSheetHash.ContainsKey(StyleSheetValueName))";
                reportcode += "\n   Return StyleSheetHash(StyleSheetValueName)";
                reportcode += "\n   End If";
                reportcode += "\n   Return Nothing";
                reportcode += "\n   End Function";
                reportcode += "\n   Private ColorCount As Integer = 0";
                reportcode += "\n   Private ColorMapping As New System.Collections.Hashtable()";
                reportcode += "\n   Private Colors() As String = nothing";
                reportcode += "\n   Public Function GetColor(ByVal groupingValue As String, ByVal delimitedColors As String) As String";
                reportcode += "\n   If ColorMapping.ContainsKey(groupingValue) Then Return ColorMapping(groupingValue)";
                reportcode += "\n   If Colors Is Nothing Then Colors = delimitedColors.Split(New Char() {\",\"c}, StringSplitOptions.RemoveEmptyEntries)";
                reportcode += "\n   Dim color As String = Colors(ColorCount Mod Colors.Length)";
                reportcode += "\n   ColorCount = ColorCount + 1";
                reportcode += "\n   ColorMapping.Add(groupingValue, color)";
                reportcode += "\n   Return color";
                reportcode += "\n   End Function";

            GetChildElement(Document, "Code").InnerText = reportcode;
        }
        public XmlElement GetChildElement(XmlElement Parent, String ChildName)
        {
            XmlElement Child = Parent[ChildName];
            if (Child == null)
            {
                Child = AppendElement(Parent, ChildName);
            }
            return Child;
        }
        public XmlElement GetChildElement(XmlElement Parent, String ChildName, String ChildNameAttributeValue)
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(d.NameTable);
            ns.AddNamespace("", ReportNamespaceURI);
            ns.AddNamespace("rd", ReportDesignerNamespaceURI);

            XmlElement Child = (XmlElement)Parent.SelectSingleNode("child::*[@Name='" + ChildNameAttributeValue + "']", ns);
            if (Child == null)
            {
                Child = AppendElement(Parent, ChildName);
                Child.Attributes.Append(CreateAttribute("Name")).Value = ChildNameAttributeValue;
            }
            return Child;
        }
        public XmlAttribute CreateAttribute(String Name)
        {
            return d.CreateAttribute(Name);
        }
        public XmlElement CreateElement(String Name)
        {
            return d.CreateElement(Name, ReportNamespaceURI);
        }
        public XmlElement CreateRdElement(String Name)
        {
            return d.CreateElement("rd", Name, ReportDesignerNamespaceURI);
        }
        public XmlElement AppendElement(XmlElement Parent, String ChildName, String ChildInnerText)
        {
            XmlElement Child = AppendElement(Parent, ChildName);
            Child.InnerText = ChildInnerText;
            return Child;
        }
        public XmlElement AppendElement(XmlElement Parent, String ChildName)
        {
            return (XmlElement)Parent.AppendChild(Parent.OwnerDocument.CreateElement(ChildName, Parent.NamespaceURI));
        }

        #region parameters datasets
        public void CreateDataSet_AccruedInterest()
        {
            XmlElement AccruedInterest = CreateDataSet("AccruedInterest");
            XmlElement Query = GetChildElement(AccruedInterest, "Query");
            GetChildElement(Query, "CommandText").InnerText = "EXEC APXUser.pGetAccruedInterest";

            CreateDataSetField(AccruedInterest, "AccruedInterestName", "System.String");
            CreateDataSetField(AccruedInterest, "AccruedInterestID", "System.Int16");
        }
        public void CreateDataSet_AnnualizeReturns()
        {
            XmlElement AnnualizeReturns = CreateDataSet("AnnualizeReturns");
            XmlElement Query = GetChildElement(AnnualizeReturns, "Query");
            GetChildElement(Query, "CommandText").InnerText = "EXEC APXUser.pGetAnnualizeReturns";

            CreateDataSetField(AnnualizeReturns, "Name", "System.String");
            CreateDataSetField(AnnualizeReturns, "Value", "System.String");
        }
        public void CreateDataSet_BondCostBasis()
        {
            XmlElement BondCostBasis = CreateDataSet("BondCostBasis");
            XmlElement Query = GetChildElement(BondCostBasis, "Query");
            GetChildElement(Query, "CommandText").InnerText = "EXEC APXUser.pGetBondCostBasis";

            CreateDataSetField(BondCostBasis, "BondCostBasisDesc", "System.String");
            CreateDataSetField(BondCostBasis, "BondCostBasisID", "System.Int16");
        }
        public void CreateDataSet_Configuration()
        {
            XmlElement Configuration = CreateDataSet("Configuration");
            XmlElement Query = GetChildElement(Configuration, "Query");
            GetChildElement(Query, "CommandText").InnerText = "EXEC APXUser.pSessionInfoSetGuid @SessionGuid\nexec APXUser.pGetConfiguration";

            CreateDataSetParameter(Configuration, "SessionGuid", "String");
            CreateDataSetField(Configuration, "AccruedInterestID", "System.Int16");
            CreateDataSetField(Configuration, "AccruePerfFees", "System.Boolean");
            CreateDataSetField(Configuration, "AllocatePerfFees", "System.Boolean");
            CreateDataSetField(Configuration, "AnnualizeReturns", "System.String");
            CreateDataSetField(Configuration, "BondCostBasis", "System.Int16");
            CreateDataSetField(Configuration, "FeeMethod", "System.Int32");
            CreateDataSetField(Configuration, "FirmLogoURL", "System.String");
            CreateDataSetField(Configuration, "FirmName", "System.String");
            CreateDataSetField(Configuration, "LocaleID", "System.Int32");
            CreateDataSetField(Configuration, "MFBasisIncludeReinvest", "System.Boolean");
            CreateDataSetField(Configuration, "PriceTypeID", "System.Int16");
            CreateDataSetField(Configuration, "ReportingCurrencyCode", "System.String");
            CreateDataSetField(Configuration, "RequiredDisclaimerID", "System.Int32");
            CreateDataSetField(Configuration, "ServerURL", "System.String");
            CreateDataSetField(Configuration, "ShowCurrencyFullPrecision", "System.Boolean");
            CreateDataSetField(Configuration, "ShowCurrentMBSFace", "System.Boolean");
            CreateDataSetField(Configuration, "ShowCurrentTIPSFace", "System.Boolean");
            CreateDataSetField(Configuration, "ShowIndustryGroup", "System.Boolean");
            CreateDataSetField(Configuration, "ShowIndustrySector", "System.Boolean");
            CreateDataSetField(Configuration, "ShowMultiCurrency", "System.Boolean");
            CreateDataSetField(Configuration, "ShowSecuritySymbol", "System.String");
            CreateDataSetField(Configuration, "ShowTaxlotsLumped", "System.Boolean");
            CreateDataSetField(Configuration, "StyleSetID", "System.Int32");
            CreateDataSetField(Configuration, "UseACB", "System.Boolean");
            CreateDataSetField(Configuration, "UseSettlementDate", "System.Boolean");
            CreateDataSetField(Configuration, "YieldOptionID", "System.Int16");
        }
        public void CreateDataSet_Currencies()
        {
            XmlElement Currencies = CreateDataSet("Currency");
            XmlElement Query = GetChildElement(Currencies, "Query");
            GetChildElement(Query, "CommandText").InnerText = "exec APXUser.pSessionInfoSetGuid @SessionGuid\nexec APXUser.pGetCurrency";

            CreateDataSetParameter(Currencies, "SessionGuid", "String");
            CreateDataSetField(Currencies, "CurrencyCode", "System.String");
            CreateDataSetField(Currencies, "CurrencyDisplayName", "System.String");
        }
        public void CreateDataSet_Disclaimer()
        {
            XmlElement Disclaimer = CreateDataSet("Disclaimer");
            XmlElement Query = GetChildElement(Disclaimer, "Query");
            GetChildElement(Query, "CommandText").InnerText = "EXEC APXUser.pGetDisclaimer";

            CreateDataSetField(Disclaimer, "DisclaimerID", "System.Int32");
            CreateDataSetField(Disclaimer, "DisclaimerName", "System.String");
        }
        public void CreateDataSet_FeeMethod()
        {
            XmlElement Configuration = CreateDataSet("FeeMethod");
            XmlElement Query = GetChildElement(Configuration, "Query");
            GetChildElement(Query, "CommandText").InnerText = "EXEC APXUser.pGetFeeMethod";

            CreateDataSetField(Configuration, "Name", "System.String");
            CreateDataSetField(Configuration, "Value", "System.Int32");

        }
        public void CreateDataSet_IncludeCapitalGains()
        {
            XmlElement IncludeCapitalGains = CreateDataSet("IncludeCapitalGains");
            XmlElement Query = GetChildElement(IncludeCapitalGains, "Query");
            GetChildElement(Query, "CommandText").InnerText = "EXEC APXUser.pGetIncludeCapitalGains";

            CreateDataSetField(IncludeCapitalGains, "Name", "System.String");
            CreateDataSetField(IncludeCapitalGains, "Value", "System.Int32");
        }
        public void CreateDataSet_LocaleInfo()
        {
            XmlElement LocaleInfo = CreateDataSet("LocaleInfo");
            XmlElement Query = GetChildElement(LocaleInfo, "Query");
            GetChildElement(Query, "CommandText").InnerText = "EXEC APXUser.pGetLocaleInfo";

            CreateDataSetField(LocaleInfo, "LocaleName", "System.String");
            CreateDataSetField(LocaleInfo, "LocaleID", "System.Int32");
        }
        public void CreateDataSet_MFBasisIncludeReinvest()
        {
            XmlElement MFBasisIncludeReinvest = CreateDataSet("MFBasisIncludeReinvest");
            XmlElement Query = GetChildElement(MFBasisIncludeReinvest, "Query");
            GetChildElement(Query, "CommandText").InnerText = "EXEC APXUser.pGetMFBasisIncludeReinvest";

            CreateDataSetField(MFBasisIncludeReinvest, "Name", "System.String");
            CreateDataSetField(MFBasisIncludeReinvest, "Value", "System.Int32");
        }
        public void CreateDataSet_PerformanceClassifications()
        {
            string sql = "--declare @SessionGuid varchar(max)";
            sql += "\nexec APXUser.pSessionInfoSetGuid @SessionGuid";
            sql += "\nexec APXUser.pGetPerformanceClassification";
            XmlElement PerfClass = CreateDataSet("PerformanceClassifications");
            XmlElement Query = GetChildElement(PerfClass, "Query");
            GetChildElement(Query, "CommandText").InnerText = sql;

            CreateDataSetParameter(PerfClass, "SessionGuid", "String");
            CreateDataSetField(PerfClass, "ClassificationID", "System.Int32");
            CreateDataSetField(PerfClass, "ClassificationName", "System.String");
        }
        public void CreateDataSet_PriceType()
        {
            XmlElement PriceType = CreateDataSet("PriceType");
            XmlElement Query = GetChildElement(PriceType, "Query");
            GetChildElement(Query, "CommandText").InnerText = "EXEC APXUser.pGetPriceType";

            CreateDataSetField(PriceType, "PriceTypeName", "System.String");
            CreateDataSetField(PriceType, "PriceTypeID", "System.Int32");
        }
        public void CreateDataSet_ReportingClassification()
        {
            XmlElement ReportingClassification = CreateDataSet("ReportingClassification");
            XmlElement Query = GetChildElement(ReportingClassification, "Query");
            GetChildElement(Query, "CommandText").InnerText = "EXEC APXUser.pGetReportingClassification";

            CreateDataSetField(ReportingClassification, "ClassificationID", "System.Int32");
            CreateDataSetField(ReportingClassification, "ClassificationName", "System.String");
        }
        public void CreateDataSet_CurrencyPrecision()
        {
            XmlElement CurrencyPrecision = CreateDataSet("ShowCurrencyFullPrecision");
            XmlElement Query = GetChildElement(CurrencyPrecision, "Query");
            GetChildElement(Query, "CommandText").InnerText = "EXEC APXUser.pGetShowCurrencyFullPrecision";

            CreateDataSetField(CurrencyPrecision, "Name", "System.String");
            CreateDataSetField(CurrencyPrecision, "Value", "System.Int32");
        }
        public void CreateDataSet_MBSFace()
        {
            XmlElement MBSFace = CreateDataSet("ShowCurrentMBSFace");
            XmlElement Query = GetChildElement(MBSFace, "Query");
            GetChildElement(Query, "CommandText").InnerText = "EXEC APXUser.pGetShowCurrentMBSFace";

            CreateDataSetField(MBSFace, "Name", "System.String");
            CreateDataSetField(MBSFace, "Value", "System.Int32");
        }
        public void CreateDataSet_TIPSFace()
        {
            XmlElement TIPSFace = CreateDataSet("ShowCurrentTIPSFace");
            XmlElement Query = GetChildElement(TIPSFace, "Query");
            GetChildElement(Query, "CommandText").InnerText = "EXEC APXUser.pGetShowCurrentTIPSFace";

            CreateDataSetField(TIPSFace, "Name", "System.String");
            CreateDataSetField(TIPSFace, "Value", "System.Int32");
        }
        public void CreateDataSet_IndustryGroup()
        {
            XmlElement IndustryGroup = CreateDataSet("ShowIndustryGroup");
            XmlElement Query = GetChildElement(IndustryGroup, "Query");
            GetChildElement(Query, "CommandText").InnerText = "EXEC APXUser.pGetShowIndustryGroup";

            CreateDataSetField(IndustryGroup, "Name", "System.String");
            CreateDataSetField(IndustryGroup, "Value", "System.Int32");
        }
        public void CreateDataSet_IndustrySector()
        {
            XmlElement IndustrySector = CreateDataSet("ShowIndustrySector");
            XmlElement Query = GetChildElement(IndustrySector, "Query");
            GetChildElement(Query, "CommandText").InnerText = "EXEC APXUser.pGetShowIndustrySector";

            CreateDataSetField(IndustrySector, "Name", "System.String");
            CreateDataSetField(IndustrySector, "Value", "System.Int32");
        }
        public void CreateDataSet_ShowSymbol()
        {
            XmlElement ShowSymbol = CreateDataSet("ShowSecuritySymbol");
            XmlElement Query = GetChildElement(ShowSymbol, "Query");
            GetChildElement(Query, "CommandText").InnerText = "EXEC APXUser.pGetShowSecuritySymbol";

            CreateDataSetField(ShowSymbol, "Name", "System.String");
            CreateDataSetField(ShowSymbol, "Value", "System.String");
        }
        public void CreateDataSet_ShowTaxLots()
        {
            XmlElement ShowTaxLots = CreateDataSet("ShowTaxLotsLumped");
            XmlElement Query = GetChildElement(ShowTaxLots, "Query");
            GetChildElement(Query, "CommandText").InnerText = "EXEC APXUser.pGetShowTaxLotsLumped";

            CreateDataSetField(ShowTaxLots, "Name", "System.String");
            CreateDataSetField(ShowTaxLots, "Value", "System.Int32");
        }
        public void CreateDataSet_UseACB()
        {
            XmlElement UseACB = CreateDataSet("UseACB");
            XmlElement Query = GetChildElement(UseACB, "Query");
            GetChildElement(Query, "CommandText").InnerText = "EXEC APXUser.pGetUseACB";

            CreateDataSetField(UseACB, "Name", "System.String");
            CreateDataSetField(UseACB, "Value", "System.Int32");
        }
        public void CreateDataSet_Yield()
        {
            XmlElement Yield = CreateDataSet("Yield");
            XmlElement Query = GetChildElement(Yield, "Query");
            GetChildElement(Query, "CommandText").InnerText = "EXEC APXUser.pGetYieldOption";

            CreateDataSetField(Yield, "YieldOptionDesc", "System.String");
            CreateDataSetField(Yield, "YieldOptionID", "System.Int32");
        }
        public void CreateDataSet_SettlementDate()
        {
            XmlElement SettlementDate = CreateDataSet("UseSettlementDate");
            XmlElement Query = GetChildElement(SettlementDate, "Query");
            GetChildElement(Query, "CommandText").InnerText = "EXEC APXUser.pGetUseSettlementDate";

            CreateDataSetField(SettlementDate, "Name", "System.String");
            CreateDataSetField(SettlementDate, "Value", "System.Int32");
        }
        public void CreateDataSet_StyleSheet()
        {
            XmlElement StyleSheet = CreateDataSet("StyleSheet");
            XmlElement Query = GetChildElement(StyleSheet, "Query");
            GetChildElement(Query, "CommandText").InnerText = "EXEC APXUser.pGetStyleSheetInfo";

            CreateDataSetField(StyleSheet,"StyleSheetXML","System.String");
        }
        #endregion

        #region accounting function datasets
        public void CreateDataSet_Appraisal()
        {
            string sql = "exec APXUser.pSessionInfoSetGuid @SessionGuid";
            sql += "\n\ndeclare @ReportData varbinary(max)";
            sql += "\nexec APXUser.pAppraisal ";
            sql += "\n	@ReportData=@ReportData out,";
            sql += "\n	@Portfolios=@Portfolios,";
            sql += "\n	@Date=@Date,";
            sql += "\n	@ReportingCurrencyCode=@ReportingCurrencyCode";
            sql += "\n\nSelect * from APXUser.fAppraisal(@ReportData)";

            XmlElement AppraisalClass = CreateDataSet("Appraisal");
            XmlElement Query = GetChildElement(AppraisalClass, "Query");
            GetChildElement(Query, "CommandText").InnerText = sql;

            CreateDataSetParameter(AppraisalClass, "SessionGuid", "String");
            CreateDataSetParameter(AppraisalClass, "Portfolios", "String");
            CreateDataSetParameter(AppraisalClass, "Date", "DateTime");
            CreateDataSetParameter(AppraisalClass, "ReportingCurrencyCode", "String");

            //More fields are available
            CreateDataSetField(AppraisalClass, "AccruedInterest", "System.Decimal");
            CreateDataSetField(AppraisalClass, "AnnualIncome", "System.Decimal");
            CreateDataSetField(AppraisalClass, "CostBasis", "System.Decimal");
            CreateDataSetField(AppraisalClass, "MarketValue", "System.Decimal");
            CreateDataSetField(AppraisalClass, "PercentAssets", "System.Decimal");
            CreateDataSetField(AppraisalClass, "PortfolioBaseID", "System.Int32");
            CreateDataSetField(AppraisalClass, "Quantity", "System.Decimal");
            CreateDataSetField(AppraisalClass, "ReportingCurrencyInterestOrDividendRate", "System.Decimal");
            CreateDataSetField(AppraisalClass, "SecTypeCode", "System.String");
            CreateDataSetField(AppraisalClass, "SecurityID", "System.Int32");
            CreateDataSetField(AppraisalClass, "UnitCost", "System.Decimal");
            CreateDataSetField(AppraisalClass, "UnrealizedGainLoss", "System.Decimal");
            CreateDataSetField(AppraisalClass, "Yield", "System.Decimal");
        }
        public void CreateDataSet_PerformanceHistory()
        {
            string sql = "\n\nexec APXUser.pSessionInfoSetGuid @SessionGuid";
            sql += "\n\ndeclare @ReportData varbinary(max)";
            sql += "\nexec APXUser.pPerformanceHistory ";
            sql += "\n	@ReportData=@ReportData out,";
            sql += "\n	@Portfolios=@Portfolios,";
            sql += "\n	@FromDate=@FromDate,";
            sql += "\n	@ToDate=@ToDate,";
            sql += "\n	@FeeMethod = @FeeMethod,";
            sql += "\n	@ClassificationID = @ClassificationID,";
            sql += "\n	@ReportingCurrencyCode=@ReportingCurrencyCode";
            sql += "\n\nSelect * from APXUser.fPerformanceHistory(@ReportData)";

            XmlElement PerHist = CreateDataSet("PerformanceHistory");
            XmlElement Query = GetChildElement(PerHist, "Query");
            GetChildElement(Query, "CommandText").InnerText = sql;

            CreateDataSetParameter(PerHist, "SessionGuid", "String");
            CreateDataSetParameter(PerHist, "Portfolios", "String");
            CreateDataSetParameter(PerHist, "FromDate", "DateTime");
            CreateDataSetParameter(PerHist, "ToDate", "DateTime");
            CreateDataSetParameter(PerHist, "ClassificationID", "Integer");
            CreateDataSetParameter(PerHist, "FeeMethod", "Integer");
            CreateDataSetParameter(PerHist, "ReportingCurrencyCode", "String");

            //More fields are available
            CreateDataSetField(PerHist, "BeginningMarketValue", "System.Decimal");
            CreateDataSetField(PerHist, "EndingMarketValue", "System.Decimal");
            CreateDataSetField(PerHist, "PeriodFromDate", "System.DateTime");
            CreateDataSetField(PerHist, "PeriodThruDate", "System.DateTime");
            CreateDataSetField(PerHist, "PortfolioBaseID", "System.Int32");
            CreateDataSetField(PerHist, "PortfolioBaseIDOrder", "System.Int32");
            CreateDataSetField(PerHist, "TWR", "System.Decimal");
            CreateDataSetField(PerHist, "ClassificationMemberCode", "System.String");
            CreateDataSetField(PerHist, "ClassificationMemberID", "System.Int32");
            CreateDataSetField(PerHist, "ClassificationMemberOrder", "System.Int32");
            CreateDataSetField(PerHist, "ClassificationMemberName", "System.String");
            CreateDataSetField(PerHist, "CumulativeTWR", "System.Decimal");
            CreateDataSetField(PerHist, "CumulativeTWRAnnualized", "System.Decimal");
        }
        public void CreateDataSet_Transactions() { }
        public void CreateDataSet_RealizedGainLoss() { }
        public void CreateDataSet_Performance() 
        {
            string sql = "--declare @SessionGuid varchar(max)";
            sql += "\n--declare @Portfolios varchar(max)";
            sql += "\n--declare @ToDate Datetime";
            sql += "\n--declare @FromDate Datetime";
            sql += "\n--declare @ClassificationID int";
            sql += "\n--declare @ReportingCurrencyCode varchar(2)";
            sql += "\n--set @Portfolios = '@master'";
            sql += "\n--set @ToDate = '12/31/2009'";
            sql += "\n--set @FromDate = '12/31/2008'";
            sql += "\n--set @ClassificationID = -9";
            sql += "\n--set @ReportingCurrencyCode = 'PC'";
            sql += "\n\nexec APXUser.pSessionInfoSetGuid @SessionGuid";
            sql += "\n\ndeclare @ReportData varbinary(max)";
            sql += "\nexec APXUser.pPerformance @ReportData=@ReportData out,";
            sql += "\n	@Portfolios=@Portfolios,";
            sql += "\n	@ToDate=@ToDate,";
            sql += "\n	@FromDate=@FromDate,";
            sql += "\n	@ClassificationID=@ClassificationID,";
            sql += "\n	@ReportingCurrencyCode=@ReportingCurrencyCode";
            sql += "\n\nSelect * from APXUser.fPerformance(@ReportData)";

            XmlElement PerfClass = CreateDataSet("Performance");
            XmlElement Query = GetChildElement(PerfClass, "Query");
            GetChildElement(Query, "CommandText").InnerText = sql;

            CreateDataSetParameter(PerfClass, "SessionGuid", "String");
            CreateDataSetParameter(PerfClass, "Portfolios", "String");
            CreateDataSetParameter(PerfClass, "FromDate", "DateTime");
            CreateDataSetParameter(PerfClass, "ToDate", "DateTime");
            CreateDataSetParameter(PerfClass, "ReportingCurrencyCode", "String");
            CreateDataSetParameter(PerfClass, "ClassificationID", "System.Int32");
            
            CreateDataSetField(PerfClass, "ACB", "System.Decimal");
	        CreateDataSetField(PerfClass, "AccruedInterestDate1", "System.Decimal");
	        CreateDataSetField(PerfClass, "AccruedInterestDate2", "System.Decimal");
	        CreateDataSetField(PerfClass, "AccruedInterestDelta", "System.Decimal");
	        CreateDataSetField(PerfClass, "Additions", "System.Decimal");
	        CreateDataSetField(PerfClass, "AnnualizedFXIRR", "System.Decimal");
	        CreateDataSetField(PerfClass, "AnnualizedIRR", "System.Decimal");
	        CreateDataSetField(PerfClass, "AnnualizedPriceIRR", "System.Decimal");
	        CreateDataSetField(PerfClass, "BondDescription", "System.String");
	        CreateDataSetField(PerfClass, "ClassificationMemberCode", "System.String");
	        CreateDataSetField(PerfClass, "ClassificationMemberID", "System.Int32");
	        CreateDataSetField(PerfClass, "ClassificationMemberName", "System.String");
	        CreateDataSetField(PerfClass, "CostBasis", "System.Decimal");
	        CreateDataSetField(PerfClass, "Dividends", "System.Decimal");
	        CreateDataSetField(PerfClass, "Fees", "System.Decimal");
	        CreateDataSetField(PerfClass, "FromDate", "System.DateTime");
	        CreateDataSetField(PerfClass, "FXIRR", "System.Decimal");
	        CreateDataSetField(PerfClass, "FXRealizedGainLossOnMval", "System.Decimal");
	        CreateDataSetField(PerfClass, "FXUnrealizedGainLossOnMval", "System.Decimal");
	        CreateDataSetField(PerfClass, "Hold", "System.String");
	        CreateDataSetField(PerfClass, "HoldCode", "System.String");
	        CreateDataSetField(PerfClass, "Interest", "System.Decimal");
	        CreateDataSetField(PerfClass, "IRR", "System.Decimal");
	        CreateDataSetField(PerfClass, "IsShortPosition", "System.String");
	        CreateDataSetField(PerfClass, "ManagementFees", "System.Decimal");
	        CreateDataSetField(PerfClass, "ManagementFeesPaidByClient", "System.Decimal");
	        CreateDataSetField(PerfClass, "MarketValueDate1", "System.Decimal");
	        CreateDataSetField(PerfClass, "MarketValueDate2", "System.Decimal");
	        CreateDataSetField(PerfClass, "PortfolioBaseID", "System.Int32");
	        CreateDataSetField(PerfClass, "PortfolioBaseIDOrder", "System.Int32");
	        CreateDataSetField(PerfClass, "PortfolioFees", "System.Decimal");
	        CreateDataSetField(PerfClass, "PortfolioFeesPaidByClient", "System.Decimal");
	        CreateDataSetField(PerfClass, "PriceIRR", "System.Decimal");
	        CreateDataSetField(PerfClass, "PriceRealizedGainLossOnMval", "System.Decimal");
	        CreateDataSetField(PerfClass, "PriceUnrealizedGainLossOnMval", "System.Decimal");
	        CreateDataSetField(PerfClass, "Quantity", "System.Decimal");
	        CreateDataSetField(PerfClass, "RealizedGainLossOnCost", "System.Decimal");
	        CreateDataSetField(PerfClass, "RealizedGainLossOnMval", "System.Decimal");
            CreateDataSetField(PerfClass, "SecTypeCode", "System.String");
	        CreateDataSetField(PerfClass, "SecurityID", "System.Int32");
	        CreateDataSetField(PerfClass, "ThruDate", "System.DateTime");
	        CreateDataSetField(PerfClass, "TotalGain", "System.Decimal");
	        CreateDataSetField(PerfClass, "TotalIncome", "System.Decimal");
	        CreateDataSetField(PerfClass, "TransfersIn", "System.Decimal");
	        CreateDataSetField(PerfClass, "TransfersOut", "System.Decimal");
	        CreateDataSetField(PerfClass, "UnitCost", "System.Decimal");
	        CreateDataSetField(PerfClass, "UnrealizedGainLossOnCost", "System.Decimal");
	        CreateDataSetField(PerfClass, "UnrealizedGainLossOnMval", "System.Decimal");
	        CreateDataSetField(PerfClass, "Withdrawals", "System.Decimal");
        }
        #endregion

        //deprecate
        public void CreateDataSet_StyleSheets()
        {
            XmlElement StyleSheets = CreateDataSet("StyleSheets");
            XmlElement Query = GetChildElement(StyleSheets, "Query");
            GetChildElement(Query, "CommandText").InnerText = "EXEC APXUser.pSessionInfoSetGuid @SessionGuid\nexec APXUser.pGetStyleSheet";

            CreateDataSetParameter(StyleSheets, "SessionGuid", "String");
            CreateDataSetField(StyleSheets, "StyleSheetID", "System.Int32");
            CreateDataSetField(StyleSheets, "DisplayName", "System.String");

        }
        public void CreateDataSet_GroupMembersFlattened()
        {
            XmlElement Group = CreateDataSet("GroupMembers");
            XmlElement Query = GetChildElement(Group, "Query");
            string sql = "--declare @SessionGuid varchar(max)";
            sql += "\n--declare @Portfolios varchar(max)";
            sql += "\n--set @Portfolios = '@master'";
            sql += "\n\nexec APXUser.pSessionInfoSetGuid @SessionGuid";
            sql += "\n\nSelect * from APXUser.vPortfolioGroupMemberFlattened WHERE PortfolioGroupCode = REPLACE(REPLACE(@Portfolios, '@', ''), '+','')";

            GetChildElement(Query, "CommandText").InnerText = sql;

            CreateDataSetParameter(Group, "SessionGuid", "String");
            CreateDataSetParameter(Group, "Portfolios", "String");
            CreateDataSetField(Group, "PortfolioGroupID", "System.Int32");
            CreateDataSetField(Group, "PortfolioGroupCode", "System.String");
            CreateDataSetField(Group, "MemberID", "System.Int32");
            CreateDataSetField(Group, "MemberCode", "System.String");
            CreateDataSetField(Group, "MemberTypeCode", "System.String");
            CreateDataSetField(Group, "IsConsolidated", "System.Boolean");
            CreateDataSetField(Group, "DisplayOrder", "System.Int32");

        }

        /// <summary>
        /// Returns a dataset element with APXFirm already set as DataSource
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public XmlElement CreateDataSet(String Name)
        {
            XmlElement DataSets = GetChildElement(Document, "DataSets");

            XmlElement DataSet = (XmlElement)DataSets.SelectSingleNode("//DataSet[@Name='" + Name + "']");
            if (DataSet == null) 
            {    
                DataSet = (XmlElement)DataSets.AppendChild(CreateElement("DataSet"));
                DataSet.Attributes.Append(CreateAttribute("Name")).Value = Name;
            }
            XmlElement Query = GetChildElement(DataSet, "Query");
            GetChildElement(Query, "DataSourceName").InnerText = ApxFirmDataSourceName;

            return DataSet;
        }
        private XmlElement CreateDataSetParameter(XmlElement DataSet, String Name, String ReportParameterDataType)
        {
            XmlElement Query = GetChildElement(DataSet, "Query");
            XmlElement QueryParameters = GetChildElement(Query, "QueryParameters");
            XmlElement QueryParameter = GetChildElement(QueryParameters, "QueryParameter", "@" +  Name);
            GetChildElement(QueryParameter, "Value").InnerText = "=Parameters!" + Name + ".Value";

            //Create report Parameter if it doesn't exist
            CreateReportParameter(Name, "String");

            return QueryParameter;
        }
        private XmlElement CreateDataSetField(XmlElement DataSet, String Name, String DataType)
        {
            XmlElement Fields = GetChildElement(DataSet, "Fields");
            XmlElement Field = GetChildElement(Fields, "Field", Name);
            GetChildElement(Field, "DataField").InnerText = Name;
            XmlElement FieldType = CreateRdElement("TypeName");
            Field.AppendChild(FieldType).InnerText = DataType;
            return Field;
        }
        public void AddReportParameterDefault(XmlElement ReportParameter, String Value)
        {
            XmlElement DefaultValue = GetChildElement(ReportParameter, "DefaultValue");
            XmlElement Values = GetChildElement(DefaultValue, "Values");
            AppendElement(Values, "Value", Value);
        }
        public void AddReportParameterAvailableValues(XmlElement ReportParameter, string DataSetName, String DataSetDisplayField, String DataSetValueField)
        {
            XmlElement ValidValues = GetChildElement(ReportParameter, "ValidValues");
            XmlElement DataSetReference = GetChildElement(ValidValues, "DataSetReference");

            AppendElement(DataSetReference, "DataSetName", DataSetName);
            AppendElement(DataSetReference, "ValueField", DataSetValueField);
            AppendElement(DataSetReference, "LabelField", DataSetDisplayField);
        }
        public void AddReportParameterDefault(XmlElement ReportParameter, string DataSetName, string DataSetValueField)
        {
            XmlElement DefaultValue = GetChildElement(ReportParameter, "DefaultValue");
            DefaultValue.RemoveAll();
            XmlElement DataSetReference = GetChildElement(DefaultValue, "DataSetReference");
            AppendElement(DataSetReference, "DataSetName", DataSetName);
            AppendElement(DataSetReference, "ValueField", DataSetValueField);
        }
        public XmlElement CreateReportParameter(String Name, String DataType, bool Hidden, bool AllowBlank, bool Nullable)
        {
            XmlElement Parameter = CreateReportParameter(Name, DataType);
            if (Nullable)
            {
                AppendElement(Parameter, "Nullable", "true");
            }
            if (AllowBlank)
            {
                AppendElement(Parameter, "AllowBlank", "true");
            }
            if (Hidden)
            {
                AppendElement(Parameter, "Hidden", "true");
            }
            return Parameter;
        }
        public XmlElement CreateReportParameter(String Name, String DataType)
        {
            return CreateReportParameter(Name, DataType, Name);
        }
        public XmlElement CreateReportParameter(String Name, String DataType, String Prompt)
        {
            XmlElement ReportParameters = GetChildElement(Document, "ReportParameters");

            XmlElement Parameter = GetChildElement(ReportParameters, "ReportParameter", Name);
            //Name is automatically set in GetChildElement
            XmlElement pDataType = GetChildElement(Parameter, "DataType");
            if (pDataType.InnerText == "")
            {
                pDataType.InnerText = DataType;
            }
            XmlElement pPrompt = GetChildElement(Parameter, "Prompt");
            if (pPrompt.InnerText == "")
            {
                pPrompt.InnerText = Prompt;
            }
            
            return Parameter;
        }
        public static void ValidateXml(object sender, System.Xml.Schema.ValidationEventArgs e)
        {
            throw (e.Exception);
        }
        public void Save(string filepath)
        {
            try
            {
                d.Normalize();
                d.Validate(new System.Xml.Schema.ValidationEventHandler(ValidateXml));
                d.Save(filepath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
