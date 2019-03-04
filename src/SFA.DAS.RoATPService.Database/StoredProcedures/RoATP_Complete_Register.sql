CREATE PROCEDURE dbo.RoATP_Complete_Register
AS
BEGIN
	SELECT 
	pt.ProviderType AS [Provider type],
	o.UKPRN,
	o.LegalName AS [Legal name],
	o.TradingName AS [Trading name],
	ot.Type AS [Organisation type],
	JSON_VALUE(o.OrganisationData, '$.CompanyNumber') AS [Company number],
	JSON_VALUE(o.OrganisationData, '$.CharityNumber') AS [Charity number],
	CASE JSON_VALUE(o.OrganisationData, '$.ParentCompanyGuarantee')
	WHEN 'True' THEN 'Yes'
	ELSE 'No'
	END AS [Parent company guarantee],
	CASE JSON_VALUE(o.OrganisationData, '$.FinancialTrackRecord')
	WHEN 'True' THEN 'Yes'
	ELSE 'No'
	END AS [Financial track record],
	os.[Status],
	o.StatusDate AS [Status date time],
	JSON_VALUE(o.OrganisationData, '$.RemovedReason.Reason') AS [Reason]
	FROM Organisations o
	INNER JOIN ProviderTypes pt
	ON pt.Id = o.ProviderTypeId
	INNER JOIN OrganisationTypes ot
	ON ot.Id = o.OrganisationTypeId
	INNER JOIN OrganisationStatus os
	ON os.Id = o.StatusId
	ORDER BY o.LegalName ASC
END
GO
