using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SSRSUtility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            DirectoryInfo = fileInfo.Directory;
            InitializeComponent();
            txtRdlFolder.Text = DirectoryInfo.FullName;
        }
        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            txtRdlFolder.Text = GetNewFolderPath();
        }

        private void AddStyleSheetIfNeeded(string StyleSheetDatasetName, string ElementName, XmlDocument d)
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(d.NameTable);
            ns.AddNamespace("", "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition");
            ns.AddNamespace("rd", "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner");
            ns.AddNamespace("report", "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition");
            XmlElement stylesheet = (XmlElement)d.SelectSingleNode("//report:DataSet[@Name='" + StyleSheetDatasetName + "']", ns);

            if (stylesheet == null)
            {
                stylesheet = d.CreateElement("", "DataSet", ns.DefaultNamespace);
                stylesheet.Attributes.Append(d.CreateAttribute("Name"));
                stylesheet.Attributes["Name"].InnerText = StyleSheetDatasetName;


                XmlElement DataSets = (XmlElement)d.SelectSingleNode("//report:DataSets", ns);
                if (DataSets == null)
                {
                    DataSets = d.CreateElement("", "DataSets", ns.DefaultNamespace);
                    d.DocumentElement.AppendChild(DataSets);
                }
                DataSets.AppendChild(stylesheet);

                XmlElement Fields = d.CreateElement("", "Fields", ns.DefaultNamespace);
                stylesheet.AppendChild(Fields);

                Dictionary<string, string> fieldTypes = new Dictionary<string, string>();
                fieldTypes.Add("StyleSheetID", "System.Int32");
                fieldTypes.Add("StyleSheetName", "System.String");
                fieldTypes.Add("StyleSetID", "System.Int32");
                fieldTypes.Add("DisplayName", "System.String");
                fieldTypes.Add("ShowFootnotes", "System.Int32");
                fieldTypes.Add("PageWidthInches", "System.Decimal");
                fieldTypes.Add("PageHeightInches", "System.Decimal");
                fieldTypes.Add("TopMarginInches", "System.Decimal");
                fieldTypes.Add("BottomMarginInches", "System.Decimal");
                fieldTypes.Add("LeftMarginInches", "System.Decimal");
                fieldTypes.Add("RightMarginInches", "System.Decimal");
                fieldTypes.Add("ElementName", "System.String");
                fieldTypes.Add("FontName", "System.String");
                fieldTypes.Add("FontSize", "System.String");
                fieldTypes.Add("FontStyleOriginal", "System.String");
                fieldTypes.Add("FontWeight", "System.String");
                fieldTypes.Add("FontStyle", "System.String");
                fieldTypes.Add("TextDecoration", "System.String");
                foreach (String field in fieldTypes.Keys)
                {
                    XmlElement Field = d.CreateElement("", "Field", ns.DefaultNamespace);
                    Fields.AppendChild(Field);
                    Field.Attributes.Append(d.CreateAttribute("Name"));
                    Field.Attributes["Name"].InnerText = field;
                    Field.AppendChild(d.CreateElement("", "DataField", ns.DefaultNamespace));
                    Field["DataField"].InnerText = field;
                    Field.AppendChild(d.CreateElement("rd", "TypeName", ns.LookupNamespace("rd")));
                    Field["rd:TypeName"].InnerText = fieldTypes[field];
                }

                stylesheet.AppendChild(d.CreateElement("", "Query", ns.DefaultNamespace));
                stylesheet["Query"].AppendChild(d.CreateElement("", "DataSourceName", ns.DefaultNamespace));
                stylesheet["Query"]["DataSourceName"].InnerText = d.SelectSingleNode("//report:DataSource[report:DataSourceReference='APXFirm']", ns).Attributes["Name"].Value;
                stylesheet["Query"].AppendChild(d.CreateElement("", "CommandText", ns.DefaultNamespace));
                stylesheet["Query"]["CommandText"].InnerText = "exec APXUser.pSessionInfoSetGuid @SessionGuid Select * from APXUserCustom.vStyleSheet WHERE DisplayName = @StyleSheet and StyleSetID = @StyleSetID and ElementName = '" + ElementName + "'";
                stylesheet["Query"].AppendChild(d.CreateElement("", "QueryParameters", ns.DefaultNamespace));
                XmlElement sessionguidparameter = d.CreateElement("", "QueryParameter", ns.DefaultNamespace);
                XmlElement stylesheetparameter = d.CreateElement("", "QueryParameter", ns.DefaultNamespace);
                XmlElement stylesetparameter = d.CreateElement("", "QueryParameter", ns.DefaultNamespace);
                sessionguidparameter.Attributes.Append(d.CreateAttribute("Name")).InnerText = "@SessionGuid";
                stylesheetparameter.Attributes.Append(d.CreateAttribute("Name")).InnerText = "@StyleSheet";
                stylesetparameter.Attributes.Append(d.CreateAttribute("Name")).InnerText = "@StyleSetID";
                sessionguidparameter.AppendChild(d.CreateElement("", "Value", ns.DefaultNamespace)).InnerText = "=Parameters!SessionGuid.Value";
                stylesheetparameter.AppendChild(d.CreateElement("", "Value", ns.DefaultNamespace)).InnerText = "=Parameters!StyleSheet.Value";
                stylesetparameter.AppendChild(d.CreateElement("", "Value", ns.DefaultNamespace)).InnerText = "=Parameters!StyleSetID.Value";

                stylesheet["Query"]["QueryParameters"].AppendChild(sessionguidparameter);
                stylesheet["Query"]["QueryParameters"].AppendChild(stylesheetparameter);
                stylesheet["Query"]["QueryParameters"].AppendChild(stylesetparameter);
                stylesheet["Query"].AppendChild(d.CreateElement("rd", "UseGenericDesigner", ns.LookupNamespace("rd"))).InnerText = "true";
            }

        }
        private string GetElementName(string tooltip)
        {
            switch (tooltip)
            {
                case "FirmName": return "Firm Name";
                case "ReportTitle": return "Report Title";
                case "PortfolioName": return "Portfolio Name";
                case "ColumnHeader": return "Column Header";
                case "SectionHeader": return "Section Header";
                case "SectionSubtotal": return "Section Subtotal";
                case "GrandTotal": return "Grand Total";
                default:

                    break;
            }
            return tooltip;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = System.Windows.Input.Cursors.Wait;
            //ToolTip of text items should contain the following values
            List<string> styles = new List<string>();
            styles.Add("FirmName");
            styles.Add("ReportTitle");
            styles.Add("PortfolioName");
            styles.Add("Date");
            styles.Add("ColumnHeader");
            styles.Add("SectionHeader");
            styles.Add("Header");
            styles.Add("Detail");
            styles.Add("SectionSubtotal");
            styles.Add("GrandTotal");
            styles.Add("Warning");
            styles.Add("Footnote");
            styles.Add("Footer");
            styles.Add("ChartDetail");
            styles.Add("BackgroundColor");
            styles.Add("Color");

            Dictionary<string, string> StyleElements = new Dictionary<string, string>();
            //StyleElements.Add("FontStyle", "FontStyle");
            //StyleElements.Add("FontFamily", "FontName");
            //StyleElements.Add("FontSize", "FontSize");
            //StyleElements.Add("FontWeight", "FontWeight");
            //StyleElements.Add("TextDecoration", "TextDecoration");
            StyleElements.Add("FontStyle", "Style");
            StyleElements.Add("FontFamily", "Family");
            StyleElements.Add("FontSize", "Size");
            StyleElements.Add("FontWeight", "Weight");
            StyleElements.Add("Color", "Color");
            StyleElements.Add("TextDecoration", "Decoration");
            StyleElements.Add("BackgroundColor", "BackgroundColor");

            foreach (System.IO.FileInfo fileInfo in DirectoryInfo.GetFiles("*.rdl"))
            {
                string fileName = fileInfo.FullName;
                XmlDocument d = new XmlDocument();

                d.Load(fileName);
                d.Schemas.Add("http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition", "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition/ReportDefinition.xsd");

                foreach (XmlElement ToolTip in d.GetElementsByTagName("ToolTip"))
                {
                    try
                    {
                        string tooltip = ToolTip.InnerText.Replace(" ", "");

                        string elementName = GetElementName(tooltip);


                        if (styles.Contains(tooltip))
                        {
                            XmlElement ToolTipParent = (XmlElement)ToolTip.ParentNode;
                            foreach (XmlElement Style in ToolTipParent.GetElementsByTagName("Style"))
                            {
                                //Check for existing tooltip - mostly an issue with charts
                                string oldElementName = elementName;
                                string oldTooltip = tooltip;

                                if (Style.ParentNode["ToolTip"] != null)
                                {
                                    string currentTooltip = Style.ParentNode["ToolTip"].InnerText.Replace(" ", "");
                                    if (!currentTooltip.Equals(tooltip))
                                    {
                                        tooltip = currentTooltip;
                                        elementName = GetElementName(currentTooltip);
                                    }
                                }

                                //}
                                //XmlElement Style = ToolTip.ParentNode["Style"];
                                //if (Style == null)
                                //{
                                //   Style = d.CreateElement("", "Style", "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition");
                                //    ToolTip.AppendChild(Style);
                                //}

                                //Actual Style update

                                foreach (string ElementKey in StyleElements.Keys)
                                {

                                    XmlElement FontElement = Style[ElementKey];

                                    if (FontElement == null)
                                    {
                                        FontElement = d.CreateElement("", ElementKey, "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition");
                                        Style.AppendChild(FontElement);
                                    }
                                    //string StyleSheetDatasetName = "StyleSheet" + tooltip;
                                    string StyleSheetDatasetName = tooltip;
                                    //AddStyleSheetIfNeeded(StyleSheetDatasetName, elementName, d);
                                    //=Code.StyleSheetValue("PortfolioNameStyle")
                                    //FontElement.InnerText = "=First(Fields!" + StyleElements[ElementKey] + ".Value, \"" + StyleSheetDatasetName + "\")";
                                    FontElement.InnerText = "=Code.StyleSheetValue(\"" + StyleSheetDatasetName + StyleElements[ElementKey] + "\")";
                                    try
                                    {
                                        d.Validate(new System.Xml.Schema.ValidationEventHandler(Report.ValidateXml), FontElement);
                                    }
                                    catch
                                    {
                                        Style.RemoveChild(FontElement);
                                    }
                                }
                                tooltip = oldTooltip;
                                elementName = oldElementName;
                            }

                        }
                    }
                    catch { }
                }
                d.Validate(new System.Xml.Schema.ValidationEventHandler(Report.ValidateXml));
                d.Save(fileName);
            }
            this.Cursor = System.Windows.Input.Cursors.Arrow;
        }

        private void CreateBaseFile(object sender, RoutedEventArgs e)
        {
            this.Cursor = System.Windows.Input.Cursors.Wait;
            Report d = new Report();

            bool IsSingleDateReport = false;
            if (chkAppraisal.IsChecked.Value)
            {
                IsSingleDateReport = true;
            }

            string tablixName = "test";
            string datasetName = "PortfolioOverview";
            string subReport = "somereport";

            d.CreateParentReport(tablixName, datasetName, subReport);
            
            d.CreateReportParameter("Portfolios", "String");

            //if (chkAppraisal.IsChecked.Value || chkIncludePerformance.IsChecked.Value || chkPerfHistory.IsChecked.Value || chkRealizedGain.IsChecked.Value || chkTrans.IsChecked.Value)
            if (chkAppraisal.IsChecked.Value || chkIncludePerformance.IsChecked.Value || chkPerfHistory.IsChecked.Value)
            {
                switch (IsSingleDateReport)
                {
                    case false:
                        d.CreateReportParameter("FromDate", "DateTime", "From Date");
                        d.CreateReportParameter("ToDate", "DateTime", "To Date");
                        break;
                    default:
                        d.CreateReportParameter("Date", "DateTime");
                        break;
                }
            }

            if (chkAccruedInterest.IsChecked.Value)
            {
                d.CreateDataSet_AccruedInterest();
                XmlElement AccruedInterest = d.CreateReportParameter("AccruedInterestID", "Integer", "Accrued Interest");
                d.AddReportParameterDefault(AccruedInterest, "Configuration", "AccruedInterestID");
                d.AddReportParameterAvailableValues(AccruedInterest, "AccruedInterest", "AccruedInterestName", "AccruedInterestID");
            }
            if (chkAccruePerfFees.IsChecked.Value)
            {
                XmlElement AccruePerfFees = d.CreateReportParameter("AccruePerfFees", "Boolean", "Accrue Fees");
                d.AddReportParameterDefault(AccruePerfFees, "Configuration", "AccruePerfFees");
            }
            if (chkAllocatePerfFees.IsChecked.Value)
            {
                XmlElement AllocatePerfFees = d.CreateReportParameter("AllocatePerfFees", "Boolean", "Allocate Fees");
                d.AddReportParameterDefault(AllocatePerfFees, "Configuration", "AllocatePerfFees");
            }
            if (chkAnnualizeReturns.IsChecked.Value)
            {
                if (rd401.IsSelected || rd41.IsSelected)
                {
                    d.CreateDataSet_AnnualizeReturns();
                    XmlElement Annualize = d.CreateReportParameter("AnnualizeReturns", "String", "Annualized Returns");
                    d.AddReportParameterDefault(Annualize, "Configuration", "AnnualizeReturns");
                    d.AddReportParameterAvailableValues(Annualize, "AnnualizeReturns", "Name", "Value");
                }
                else
                {
                    XmlElement Annualize = d.CreateReportParameter("AnnualizeReturns", "Boolean", "Annualized Returns");
                    d.AddReportParameterDefault(Annualize, "Configuration", "AnnualizeReturns");
                }
            }
            if (chkBondCostBasis.IsChecked.Value)
            {
                d.CreateDataSet_BondCostBasis();
                XmlElement BondCostBasis = d.CreateReportParameter("BondCostBasis", "Integer", "Bond Cost Basis");
                d.AddReportParameterDefault(BondCostBasis, "Configuration", "BondCostBasis");
                d.AddReportParameterAvailableValues(BondCostBasis, "BondCostBasis", "BondCostBasisDesc", "BondCostBasisID");
            }
            d.CreateDataSet_Configuration();

            //if (chkCurrency.IsChecked.Value)
            //{
                d.CreateDataSet_Currencies();
                XmlElement ReportingCurrencyCode = d.CreateReportParameter("ReportingCurrencyCode", "String", "Reporting Currency");
                d.AddReportParameterDefault(ReportingCurrencyCode, "Configuration", "ReportingCurrencyCode");
                d.AddReportParameterAvailableValues(ReportingCurrencyCode, "Currency", "CurrencyDisplayName", "CurrencyCode");
            //}

            //if (chkCurrencyPrecision.IsChecked.Value)
            //{
                d.CreateDataSet_CurrencyPrecision();
                XmlElement Precision = d.CreateReportParameter("ShowCurrencyFullPrecision", "Integer", "Currency Display");
                d.AddReportParameterDefault(Precision, "Configuration", "ShowCurrencyFullPrecision");
                d.AddReportParameterAvailableValues(Precision, "ShowCurrentFullPrecision", "Name", "Value");
            //}

            if (chkFeeMethod.IsChecked.Value)
            {
                d.CreateDataSet_FeeMethod();
                XmlElement FeeMethod = d.CreateReportParameter("FeeMethod", "Integer", "Calculate Performance");
                d.AddReportParameterDefault(FeeMethod, "Configuration", "FeeMethod");
                d.AddReportParameterAvailableValues(FeeMethod, "FeeMethod", "Name", "Value");
            }

            d.CreateDataSet_Disclaimer();
            XmlElement Disclaimer = d.CreateReportParameter("Disclaimer", "Integer");
            d.AddReportParameterDefault(Disclaimer, "Configuration", "RequiredDisclaimerID");
            d.AddReportParameterAvailableValues(Disclaimer, "Disclaimer", "DisclaimerName", "DisclaimerID");
            XmlElement FirmName = d.CreateReportParameter("FirmName", "String", true, true, true);
            d.AddReportParameterDefault(FirmName, "Configuration", "FirmName");

            //if (chkIncludeStyle.IsChecked.Value)
            //{
            //    XmlElement StyleSetID = d.CreateReportParameter("StyleSetID", "Integer", true, false, false);
            //    d.AddReportParameterDefault(StyleSetID, "Configuration", "StyleSetID");

            //    d.CreateDataSet_StyleSheets();
            //    XmlElement StyleSheet = d.CreateReportParameter("StyleSheet", "String");
            //    d.AddReportParameterDefault(StyleSheet, "Default A");
            //    d.AddReportParameterAvailableValues(StyleSheet, "StyleSheets", "DisplayName", "DisplayName");
            //}
            if (chkIndustryGroup.IsChecked.Value)
            {
                d.CreateDataSet_IndustryGroup();
                XmlElement Industry = d.CreateReportParameter("ShowIndustryGroup", "Integer", "Industry Group");
                d.AddReportParameterDefault(Industry, "Configuration", "ShowIndustryGroup");
                d.AddReportParameterAvailableValues(Industry, "ShowIndustryGroup", "Name", "Value");
            }
            if (chkIndustrySector.IsChecked.Value)
            {
                d.CreateDataSet_IndustrySector();
                XmlElement Sector = d.CreateReportParameter("ShowIndustrySector", "Integer", "Industry Sector");
                d.AddReportParameterDefault(Sector, "Configuration", "ShowIndustrySector");
                d.AddReportParameterAvailableValues(Sector, "ShowIndustrySector", "Name", "Value");
            }
            //if (chkGroupMembers.IsChecked.Value)
            //{
            //    d.CreateDataSet_GroupMembersFlattened();
            //}

            d.CreateDataSet_LocaleInfo();
            XmlElement LocaleID = d.CreateReportParameter("LocaleID", "Integer", "Locale");
            d.AddReportParameterDefault(LocaleID, "Configuration", "LocaleID");
            d.AddReportParameterAvailableValues(LocaleID, "LocaleInfo", "LocaleName", "LocaleID");

            if (chkMBSFace.IsChecked.Value)
            {
                d.CreateDataSet_MBSFace();
                XmlElement MBSFace = d.CreateReportParameter("ShowCurrentMBSFace", "Integer", "MBS Face");
                d.AddReportParameterDefault(MBSFace, "Configuration", "ShowCurrentMBSFace");
                d.AddReportParameterAvailableValues(MBSFace, "ShowCurrentMBSFace", "Name", "Value");
            }
            if (chkMFBasisIncludeReinvest.IsChecked.Value)
            {
                d.CreateDataSet_MFBasisIncludeReinvest();
                XmlElement MFBasis = d.CreateReportParameter("MFBasisIncludeReinvest", "Integer", "Mutual Fund Cost Basis");
                d.AddReportParameterDefault(MFBasis, "Configuration", "MFBasisIncludeReinvest");
                d.AddReportParameterAvailableValues(MFBasis, "MFBasisIncludeReinvest", "Name", "Value");
            }
            if (chkClassification.IsChecked.Value)
            {
                d.CreateDataSet_ReportingClassification();
                XmlElement ReportClass = d.CreateReportParameter("ReportingClassification", "Integer", "Reporting Classification");
                d.AddReportParameterAvailableValues(ReportClass, "ReportingClassification", "ClassificationName", "ClassificationID");
            }
            if (chkPriceType.IsChecked.Value)
            {
                d.CreateDataSet_PriceType();
                XmlElement PriceType = d.CreateReportParameter("PriceTypeID", "Integer", "Price Set");
                d.AddReportParameterDefault(PriceType, "Configuration", "PriceTypeID");
                d.AddReportParameterAvailableValues(PriceType, "PriceType", "PriceTypeName", "PriceTypeID");
            }
            if (chkTIPSFace.IsChecked.Value)
            {
                d.CreateDataSet_TIPSFace();
                XmlElement TIPSFace = d.CreateReportParameter("ShowCurrentTIPSFace", "Integer", "TIPS Face");
                d.AddReportParameterDefault(TIPSFace, "Configuration", "ShowCurrentTIPSFace");
                d.AddReportParameterAvailableValues(TIPSFace, "ShowCurrentTIPSFace", "Name", "Value");
            }
            if (chkShowSymbol.IsChecked.Value)
            {
                d.CreateDataSet_ShowSymbol();
                XmlElement Symbol = d.CreateReportParameter("ShowSecuritySymbol", "String", "Security Symbol");
                d.AddReportParameterDefault(Symbol, "Configuration", "ShowSecuritySymbol");
                d.AddReportParameterAvailableValues(Symbol, "ShowSecuritySymbol", "Name", "Value");
            }
            if (chkShowTaxLots.IsChecked.Value)
            {
                d.CreateDataSet_ShowTaxLots();
                XmlElement TaxLots = d.CreateReportParameter("ShowTaxLotsLumped", "Integer", "Tax Lots");
                d.AddReportParameterDefault(TaxLots, "Configuration", "ShowTaxlotsLumped");
                d.AddReportParameterAvailableValues(TaxLots, "ShowTaxLotsLumped", "Name", "Value");
            }

            XmlElement ServerURL = d.CreateReportParameter("ServerURL", "String", true, true, true);
            d.AddReportParameterDefault(ServerURL, "Configuration", "ServerURL");

            d.CreateDataSet_StyleSheet();
            XmlElement StyleSheet = d.CreateReportParameter("StyleSheetXML", "String", true, false, false);
            d.AddReportParameterDefault(StyleSheet, "StyleSheet", "StyleSheetXML");

            if (chkUseACB.IsChecked.Value)
            {
                d.CreateDataSet_UseACB();
                XmlElement UseACB = d.CreateReportParameter("UseACB", "Integer", "IRR Calculation Method");
                d.AddReportParameterDefault(UseACB, "Configuration", "UseACB");
                d.AddReportParameterAvailableValues(UseACB, "UseACB", "Name", "Value");
            }
            if (chkUseIRR.IsChecked.Value)
            {
                XmlElement UseIRR = d.CreateReportParameter("UseIRR", "Boolean", "Show IRR Calc");
                d.AddReportParameterDefault(UseIRR, "False");
            }
            if (chkUseSettlementDate.IsChecked.Value)
            {
                d.CreateDataSet_SettlementDate();
                XmlElement UseSettlementDate = d.CreateReportParameter("UseSettlementDate", "Integer", "Settlement Date");
                d.AddReportParameterDefault(UseSettlementDate, "Configuration", "UseSettlementDate");
                d.AddReportParameterAvailableValues(UseSettlementDate, "UseSettlementDate", "Name", "Value");
            }
            if (chkYieldOption.IsChecked.Value)
            {
                d.CreateDataSet_Yield();
                XmlElement Yield = d.CreateReportParameter("YieldOptionID", "Integer", "Yield");
                d.AddReportParameterDefault(Yield, "Configuration", "YieldOptionID");
                d.AddReportParameterAvailableValues(Yield, "Yield", "YieldOptionDesc", "YieldOptionID");
            }
            if (chkIncludePerformance.IsChecked.Value || chkPerfHistory.IsChecked.Value || chkPerformanceClassification.IsChecked.Value)
            {
                d.CreateDataSet_PerformanceClassifications();
                XmlElement PerformanceClassificationID = d.CreateReportParameter("ClassificationID", "Integer", "Classification");
                d.AddReportParameterDefault(PerformanceClassificationID, "-9");
                d.AddReportParameterAvailableValues(PerformanceClassificationID, "PerformanceClassifications", "ClassificationName", "ClassificationID");
            }
            if (chkIncludePerformance.IsChecked.Value)
            {
                d.CreateDataSet_Performance();
            }
            if (chkPerfHistory.IsChecked.Value)
            {
                d.CreateDataSet_PerformanceHistory();
            }
            if (chkAppraisal.IsChecked.Value)
            {
                d.CreateDataSet_Appraisal();
            }
            //if (chkRealizedGain.IsChecked.Value)
            //{
            //    d.CreateDataSet_RealizedGainLoss();
            //}
            //if (chkTrans.IsChecked.Value)
            //{
            //    d.CreateDataSet_Transactions();
            //}

            //if (!chkIsPortrait.IsChecked.Value)
            //{
            //    //(11m, 8.5m, .37m, .36m, .25m, .25m) = landscape
            //    d.SetPageLayout(11m, 8.5m, .25m, .25m, .37m, .36m);
            //}
            //else
            //{
                //(11m, 8.5m, .25m, .25m, .37m, .36m) = portrait
            d.SetPageLayout(11m, 8.5m, .37m, .36m, .25m, .25m);
            //}
            try
            {
                d.Save(txtRdlFolder.Text + @"\" + txtFileName.Text);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error in RDL: " + ex.Message);
            }
            this.Cursor = System.Windows.Input.Cursors.Arrow;
        }
        System.IO.DirectoryInfo _DirectoryInfo;
        System.IO.DirectoryInfo DirectoryInfo
        {
            get
            {
                return _DirectoryInfo;
            }
            set
            {
                _DirectoryInfo = value;
                System.Windows.Controls.TextBox[] controls = new System.Windows.Controls.TextBox[] { txtRdlFolder };
                foreach (System.Windows.Controls.TextBox control in controls)
                {
                    if (control != null)
                    {
                        //Avoid infinite looping of events
                        if (!control.Text.Equals(_DirectoryInfo.FullName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            control.Text = _DirectoryInfo.FullName;
                        }
                    }
                }
            }
        }
        private void txtRdlFolder_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetNewFolderPath(txtRdlFolder);
        }
        private string GetNewFolderPath()
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            d.ShowDialog();
            return d.SelectedPath;
        }
        private void SetNewFolderPath(System.Windows.Controls.TextBox textBox)
        {
            try
            {
                DirectoryInfo = new System.IO.DirectoryInfo(textBox.Text);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error changing directory: " + ex.Message);
            }
        }
    }
}