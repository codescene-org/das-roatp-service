CREATE PROCEDURE [dbo].[RoATP_Audit_History]
AS
	SELECT
	o.UKPRN, 
	o.LegalName AS [Legal Name],
	ah.FieldChanged AS [Field Changed],
	ah.UpdatedAt, 
	ah.UpdatedBy, 
	ah.PreviousValue AS [Old Value],
	ah.PreviousStatusDate AS [Previous Status Date],
	ah.NewValue AS [New Value],
	ah.UpdatedAt AS [Change Date],
	ah.UpdatedBy AS [Operator Name]
	FROM [Audit] ah
	INNER JOIN Organisations o 
	ON o.Id = ah.OrganisationId
RETURN 0
GO
