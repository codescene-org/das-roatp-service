CREATE PROCEDURE [dbo].[RoATP_CSV_SUMMARY]
AS

SET NOCOUNT ON

 select ukprn AS UKPRN, 
 LegalName + 
	 case isnull(tradingName,'') when '' then ''
	 else ' T/A ' + 
	 (select STRING_AGG((Upper(substring(value,1,1))+lower(substring(value,2,len(value)-1))), ' ') as Name
	 from STRING_SPLIT(TradingName,' '))
	 END AS 'Organisation Name',
 pt.ProviderType AS 'Provider Type',
 CASE Json_value(OrganisationData,'$.NonLevyContract')
	WHEN 'true' then 'Y' else 'N' end  AS 'Contracted to deliver to non-levied employers',
 case Json_value(OrganisationData,'$.ParentCompanyGuarantee')
	WHEN 'true' then 'Y' else 'N' end AS 'Parent company guarantee',
 case Json_value(OrganisationData,'$.FinancialTrackRecord')
	WHEN 'true' then 'N' else 'Y' end AS 'New Organisation without financial track record',
 Json_value(OrganisationData,'$.StartDate') AS 'Start Date',
 Case StatusId WHEN 0 then StatusDate else null END AS 'End Date',
 CASE StatusId WHEN 2 THEN StatusDate ELSE NULL END AS 'Provider not currently starting new apprentices'
 from organisations o 
 left outer join providerTypes pt on o.ProviderTypeId = pt.Id