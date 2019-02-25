CREATE PROCEDURE dbo.RoATP_Complete_Register
AS
BEGIN
	SELECT 
	pt.ProviderType AS [Provider Type],
	o.UKPRN,
	o.LegalName AS [Legal Name],
	o.TradingName AS [Trading Name],
	ot.Type AS [Organisation Type],
	JSON_VALUE(o.OrganisationData, '$.CompanyNumber') AS [Companies House Number],
	JSON_VALUE(o.OrganisationData, '$.CharityNumber') AS [Charities Commission Number],
	CASE JSON_VALUE(o.OrganisationData, '$.ParentCompanyGuarantee')
	WHEN 'True' THEN 'Yes'
	ELSE 'No'
	END AS [Parent Company Guarantee],
	CASE JSON_VALUE(o.OrganisationData, '$.FinancialTrackRecord')
	WHEN 'True' THEN 'Yes'
	ELSE 'No'
	END AS [Financial Track Record],
	os.[Status],
	o.StatusDate AS [Status Date],
	JSON_VALUE(o.OrganisationData, '$.RemovedReason.Reason') AS [Removed Reason]
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
