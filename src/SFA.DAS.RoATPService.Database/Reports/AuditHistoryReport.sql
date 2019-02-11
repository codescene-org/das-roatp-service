CREATE PROCEDURE [dbo].[AuditHistoryReport]
AS
	SELECT o.UKPRN, ah.UpdatedAt, ah.UpdatedBy, ah.FieldChanged, ah.PreviousValue, ah.NewValue FROM Audit ah
	INNER JOIN Organisations o 
	ON o.Id = ah.OrganisationId

RETURN 0
