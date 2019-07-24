CREATE PROCEDURE [dbo].[RoATP_CSV_SUMMARY]
	(@ukprn INT = null)
AS

SET NOCOUNT ON

SELECT ukprn AS UKPRN, 
 LegalName + 
	 CASE ISNULL(tradingName,'') WHEN '' THEN ''
	 ELSE ' T/A ' + tradingName
	 END AS 'Organisation Name',
 pt.ProviderType AS 'Provider type',
 CASE JSON_VALUE(OrganisationData,'$.NonLevyContract')
	WHEN 'true' THEN 'Y' ELSE 'N' END  AS 'Contracted to deliver to non-levied employers',
 CASE JSON_VALUE(OrganisationData,'$.ParentCompanyGuarantee')
	WHEN 'true' THEN 'Y' ELSE 'N' END AS 'Parent company guarantee',
 CASE JSON_VALUE(OrganisationData,'$.FinancialTrackRecord')
	WHEN 'true' THEN 'N' ELSE 'Y' END AS 'New Organisation without financial track record',
 CONVERT(VARCHAR(10),CONVERT(DATE,JSON_VALUE(OrganisationData,'$.StartDate')), 111) AS  'Start Date',
 CASE StatusId WHEN 0 THEN CONVERT(VARCHAR(10),StatusDate,111) ELSE NULL END AS 'End Date',
 CASE StatusId WHEN 2 THEN CONVERT(VARCHAR(10),StatusDate,111) ELSE NULL END AS 'Provider not currently starting new apprentices',
 CONVERT(VARCHAR(10),CONVERT(DATE,JSON_VALUE(OrganisationData,'$.ApplicationDeterminedDate')), 111) AS  'Application Determined Date'
 FROM organisations o 
 LEFT OUTER JOIN providerTypes pt ON o.ProviderTypeId = pt.Id
	 WHERE o.StatusId IN (0,1,2) -- exclude on-boarding
	 AND ukprn = ISNULL(@ukprn, ukprn)
	  ORDER BY COALESCE(o.UpdatedAt, o.CreatedAt) DESC