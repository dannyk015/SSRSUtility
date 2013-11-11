declare @SessionGuid nvarchar(max)
	,@Portfolios nvarchar(32) = '@casetrade'
	,@FromDate datetime = '12/31/05'
	,@ToDate datetime = '12/31/06'
	,@ReportingCurrencyCode char(2)
	,@CompositeFromDate datetime
	,@CompositeToDate datetime
	,@FiscalYearStartDate datetime
	,@IncludeClosedPortfolios bit
	,@IncludeUnsupervisedAssets bit
	,@ShowTaxLotsLumped int
	,@AccruedInterestID int
	,@YieldOptionID int
	,@BondCostBasisID  int
	,@MFBasisIncludeReinvest int
	,@UseSettlementDate int
	,@ShowCurrentMBSFace  int
	,@ShowCurrentTIPSFace  int
	,@LocaleID int
	,@PriceTypeID int
	,@OverridePortfolioSettings bit

exec APXUser.pSessionCreate 'admin','advs'
--exec APXUser.pSessionInfoSetGuid @SessionGuid

declare @DataHandle uniqueidentifier = newid()

exec APXUser.pAppraisalBatch @DataHandle = @DataHandle
	,@DataName = 'Appraisal'
	,@Portfolios = @Portfolios
	,@Date = @ToDate

	-- Optional Parameters
	,@ReportingCurrencyCode = @ReportingCurrencyCode out
	,@CompositeFromDate = @CompositeFromDate
	,@CompositeToDate = @CompositeToDate
	,@FiscalYearStartDate = @FiscalYearStartDate
	,@IncludeClosedPortfolios = @IncludeClosedPortfolios
	,@IncludeUnsupervisedAssets = @IncludeUnsupervisedAssets
	,@ShowTaxLotsLumped = @ShowTaxLotsLumped out
	,@AccruedInterestID = @AccruedInterestID
	,@YieldOptionID = @YieldOptionID out
	,@BondCostBasisID  = @BondCostBasisID out
	,@MFBasisIncludeReinvest = @MFBasisIncludeReinvest out
	,@UseSettlementDate = @UseSettlementDate out
	,@ShowCurrentMBSFace  = @ShowCurrentMBSFace out
	,@ShowCurrentTIPSFace  = @ShowCurrentTIPSFace out
	,@LocaleID = @LocaleID
	,@PriceTypeID = @PriceTypeID
	,@OverridePortfolioSettings = @OverridePortfolioSettings

exec APXUser.pReportBatchExecute @DataHandle, @ExplodeData= 1

declare @ReportData varbinary(max)
exec APXUser.pReportDataGetFromHandle @DataHandle = @DataHandle, @DataName = 'Appraisal', @ReportData = @ReportData out

select
	DataHandle = @DataHandle,
	FirmLogo = APX.fPortfolioCustomLabel(a.PortfolioBaseID, '$flogo', 'logo.jpg'),
	p.LocaleID,
	p.PrefixedPortfolioBaseCode,
	a.PortfolioBaseID,
	a.PortfolioBaseIDOrder,
	a.ReportDate,
	p.ReportHeading1,	
	p.ReportHeading2,
	p.ReportHeading3,
	p.ReportingCurrencyCode,
    p.ReportingCurrencyName,
	UseSettlementDate = @UseSettlementDate
from APXUser.fReportDataIndex(@ReportData) a
join APXSSRS.fPortfolioBase(@LocaleID, @ReportingCurrencyCode, 0) p on
	p.PortfolioBaseID = a.PortfolioBaseID
order by a.PortfolioBaseIDOrder
